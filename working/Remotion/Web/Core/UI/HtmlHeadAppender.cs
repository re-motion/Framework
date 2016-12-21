// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Remotion.Collections;
using Remotion.Context;
using Remotion.Utilities;
using Remotion.Web.Resources;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{
  /// <summary>
  ///   Provides a mechanism to register HTML header elements (e.g., stylesheet or script links).
  /// </summary>
  /// <example>
  ///   Insert the following line into the head element of the webform you want to add 
  ///   the registered controls to.
  ///   <code>
  ///     &lt;rwc:htmlheadcontents runat="server" id="HtmlHeadContents"&gt;&lt;/rwc:htmlheadcontents&gt;
  ///   </code>
  ///   Register a new <c>HTML head element</c> using the following syntax.
  ///   <code>
  ///     HtmlHeadAppender.Current.Register...(key, ...);
  ///   </code>
  /// </example>
  public sealed class HtmlHeadAppender
  {
    public enum Priority
    {
      Script = 0, // Absolute values to emphasize sorted nature of enum values
      Library = 1,
      UserControl = 2,
      Page = 3
    }

    private static readonly SafeContextSingleton<HtmlHeadAppender> s_current =
        new SafeContextSingleton<HtmlHeadAppender> (typeof (HtmlHeadAppender).AssemblyQualifiedName + "_Current", () => new HtmlHeadAppender());

    /// <summary>
    ///   Gets the <see cref="HtmlHeadAppender"/> instance.
    /// </summary>
    public static HtmlHeadAppender Current
    {
      get { return s_current.Current; }
    }

    private readonly Dictionary<string, object> _registeredKeys = new Dictionary<string, object>();

    private readonly MultiDictionary<Priority, HtmlHeadElement> _prioritizedHeadElements = new MultiDictionary<Priority, HtmlHeadElement>();

    /// <summary> <see langword="true"/> if <see cref="SetAppended"/> has already executed. </summary>
    private bool _hasAppendExecuted;

    private WeakReference _handler = new WeakReference (null);
    private TitleTag _title;

    /// <remarks>
    ///   Factory pattern. No public construction.
    /// </remarks>
    /// <exclude/>
    private HtmlHeadAppender ()
    {
    }

    public IEnumerable<HtmlHeadElement> GetHtmlHeadElements ()
    {
      EnsureStateIsClearedAfterServerTransfer();

      if (_title != null)
        yield return _title;

      foreach (var priority in _prioritizedHeadElements.Keys.OrderBy (priority => (int) priority))
      {
        foreach (var element in TransformHtmlHeadElements (_prioritizedHeadElements[priority]))
          yield return element;
      }
    }

    private IEnumerable<HtmlHeadElement> TransformHtmlHeadElements (IEnumerable<HtmlHeadElement> elements)
    {
      var styleSheetImportRules = new List<StyleSheetElement>();
      var styleSheetElements = new List<StyleSheetElement>();

      foreach (var element in elements)
      {
        if (element is StyleSheetImportRule)
          styleSheetImportRules.Add ((StyleSheetImportRule) element);
        else if (element is StyleSheetElement)
          styleSheetElements.Add ((StyleSheetElement) element);
        else
          yield return element;
      }

      foreach (var rules in styleSheetImportRules.Select ((r, i) => new { Rule = r, Index = i }).GroupBy (s => s.Index / 31, s => s.Rule))
        yield return new StyleSheetBlock (rules);

      if (styleSheetElements.Count > 0)
        yield return new StyleSheetBlock (styleSheetElements);
    }

    public void SetAppended ()
    {
      _hasAppendExecuted = true;
    }

    /// <summary> Gets a flag indicating wheter <see cref="SetAppended"/> has been executed. </summary>
    /// <value> <see langword="true"/> if  <see cref="SetAppended"/> has been executed. </value>
    /// <remarks> Use this property to ensure that an <see cref="HtmlHeadContents"/> is present on the page. </remarks>
    public bool HasAppended
    {
      get { return _hasAppendExecuted; }
    }

    /// <summary>
    ///   Sets the <c>title</c> of the page.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="SetTitle"/> must be completed before
    ///     <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Remove the title tag from the aspx-source.
    ///   </para><para>
    ///     Registeres the title with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="title"> The stirng to be isnerted as the title. </param>
    public void SetTitle (string title)
    {
      ArgumentUtility.CheckNotNull ("title", title);

      _title = new TitleTag (title);
    }

    /// <summary> 
    /// Registers a style sheet file. 
    /// <seealso cref="IResourceUrlFactory"/>
    /// </summary>
    /// <remarks>
    ///   All calls to <see cref="RegisterStylesheetLink(string, string, Priority)"/> must be completed before
    ///   <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="url"> The url of the stylesheet file. Must not be <see langword="null"/>. </param>
    /// <param name="priority"> 
    ///   The priority level of the head element. Elements are rendered in the following order:
    ///   Library, UserControl, Page.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, IResourceUrl url, Priority priority)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("url", url);

      RegisterHeadElement (key, new StyleSheetImportRule (url), priority);
    }

    /// <summary> Registers a stylesheet file. </summary>
    /// <remarks>
    ///   All calls to <see cref="RegisterStylesheetLink(string, string, Priority)"/> must be completed before
    ///   <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="href"> The url of the stylesheet file. Must not be <see langword="null"/> or empty. </param>
    /// <param name="priority"> 
    ///   The priority level of the head element. Elements are rendered in the following order:
    ///   Library, UserControl, Page.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, string href, Priority priority)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNullOrEmpty ("href", href);

      RegisterStylesheetLink (key, new StaticResourceUrl (href), priority);
    }

    /// <summary> 
    /// Registers a style sheet file. 
    /// <seealso cref="IResourceUrlFactory"/>
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterStylesheetLink(string, string)"/> must be completed before
    ///     <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="url"> 
    /// The url of the stylesheet file. Must not be <see langword="null"/>. 
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, IResourceUrl url)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("url", url);

      RegisterStylesheetLink (key, url, Priority.Page);
    }

    /// <summary> Registers a stylesheet file. </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterStylesheetLink(string, string)"/> must be completed before
    ///     <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the stylesheet file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="href"> 
    ///   The url of the stylesheet file. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterStylesheetLink (string key, string href)
    {
      RegisterStylesheetLink (key, new StaticResourceUrl (href), Priority.Page);
    }

    /// <summary> 
    /// Registers a javascript file. 
    /// <seealso cref="IResourceUrlFactory"/>
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterJavaScriptInclude(string,Remotion.Web.IResourceUrl)"/> must be completed before
    ///     <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key">
    ///   The unique key identifying the javascript file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="url"> 
    ///   The url of the javascript file. Must not be <see langword="null"/>. 
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterJavaScriptInclude (string key, IResourceUrl url)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("url", url);

      RegisterHeadElement (key, new JavaScriptInclude (url), Priority.Script);
    }

    /// <summary> Registers a javascript file. </summary>
    /// <remarks>
    ///   <para>
    ///     All calls to <see cref="RegisterJavaScriptInclude(string,string)"/> must be completed before
    ///     <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    ///   </para><para>
    ///     Registeres the javascript file with a default priority of Page.
    ///   </para>
    /// </remarks>
    /// <param name="key">
    ///   The unique key identifying the javascript file in the headers collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="src"> 
    ///   The url of the javascript file. Must not be <see langword="null"/> or empty. 
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterJavaScriptInclude (string key, string src)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNullOrEmpty ("src", src);

      RegisterJavaScriptInclude (key, new StaticResourceUrl (src));
    }

    /// <summary> Registers a <see cref="HtmlHeadElement"/>. </summary>
    /// <remarks>
    ///   All calls to <see cref="RegisterHeadElement"/> must be completed before
    ///   <see cref="SetAppended"/> is called. (Typically during the <c>Render</c> phase.)
    /// </remarks>
    /// <param name="key"> 
    ///   The unique key identifying the header element in the collection. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <param name="headElement"> 
    ///   The <see cref="HtmlHeadElement"/> representing the head element. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="priority"> 
    ///   The priority level of the head element. Elements are rendered in the following order:
    ///   Library, UserControl, Page.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if method is called after <see cref="SetAppended"/> has executed.
    /// </exception>
    public void RegisterHeadElement (string key, HtmlHeadElement headElement, Priority priority)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNull ("headElement", headElement);

      EnsureStateIsClearedAfterServerTransfer();

      if (_hasAppendExecuted)
        throw new InvalidOperationException ("RegisterHeadElement must not be called after SetAppended has been called.");

      if (! IsRegistered (key))
      {
        _registeredKeys.Add (key, null);
        _prioritizedHeadElements.Add (priority, headElement);
      }
    }

    /// <summary>
    ///   Test's whether an element with this <paramref name="key"/> has already been registered.
    /// </summary>
    /// <param name="key"> The string to test. Must not be <see langword="null"/> or empty. </param>
    /// <returns>
    ///   <see langword="true"/> if an element with this <paramref name="key"/> has already been 
    ///   registered.
    /// </returns>
    public bool IsRegistered (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);

      EnsureStateIsClearedAfterServerTransfer();

      return _registeredKeys.ContainsKey (key);
    }

    private void EnsureStateIsClearedAfterServerTransfer ()
    {
      HttpContext context = HttpContext.Current;
      if (context != null)
      {
        var handler = context.Handler;
        if (!ReferenceEquals (handler, _handler.Target))
        {
          _registeredKeys.Clear();
          _prioritizedHeadElements.Clear();
          _hasAppendExecuted = false;
          _handler = new WeakReference (handler);
        }
      }
    }
  }
}