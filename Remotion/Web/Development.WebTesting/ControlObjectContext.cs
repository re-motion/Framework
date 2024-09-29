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
using Coypu;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Context for a <see cref="ControlObject"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public class ControlObjectContext : WebTestObjectContext
  {
    private readonly PageObject _pageObject;

    /// <summary>
    /// Private constructor, may be obtained via a <see cref="PageObjectContext"/> or via control selection.
    /// </summary>
    /// <exception cref="WebTestException">
    /// If the control cannot be found.
    /// <para>- or -</para>
    /// If multiple matching controls are found.
    /// </exception>
    internal ControlObjectContext ([NotNull] PageObject pageObject, [NotNull] ElementScope scope, [NotNull] ILoggerFactory loggerFactory)
        : base(scope, loggerFactory)
    {
      ArgumentUtility.CheckNotNull("pageObject", pageObject);

      _pageObject = pageObject;
    }

    /// <inheritdoc/>
    public override IBrowserSession Browser
    {
      get { return PageObject.Context.Browser; }
    }

    /// <inheritdoc/>
    public override BrowserWindow Window
    {
      get { return PageObject.Context.Window; }
    }

    /// <inheritdoc/>
    public ElementScope RootScope
    {
      get { return PageObject.Scope; }
    }

    /// <summary>
    /// Returns the <see cref="PageObject"/> on which the <see cref="ControlObject"/> resides.
    /// </summary>
    public PageObject PageObject
    {
      get { return _pageObject; }
    }

    /// <summary>
    /// Clones the context for another <see cref="ControlObject"/> which resides within the same <see cref="IBrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and on the same page.
    /// </summary>
    /// <param name="scope">The scope of the other <see cref="ControlObject"/>.</param>
    /// <exception cref="WebTestException">
    /// If the control cannot be found.
    /// <para>- or -</para>
    /// If multiple matching controls are found.
    /// <para>- or -</para>
    /// If server error has occurred.
    /// </exception>
    public ControlObjectContext CloneForControl ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      try
      {
        return new ControlObjectContext(PageObject, scope, LoggerFactory);
      }
      catch (WebTestException)
      {
        PageObject.Context.RequestErrorDetectionStrategy.CheckPageForError(PageObject.Scope);

        throw;
      }
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides within the same <see cref="IBrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and replaces the current <see cref="PageObject"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="CloneForNewPage"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because it is only intended for use from <see cref="UnspecifiedPageObject"/>, which are normally called after <see cref="WebTestAction"/>s
    /// which perform error page detection in the <see cref="ICompletionDetectionStrategy"/>.
    /// </remarks>
    public PageObjectContext CloneForNewPage ()
    {
      var rootScope = Window.GetRootScope();

      var cloneForNewPage = new PageObjectContext(Browser, Window, PageObject.Context.RequestErrorDetectionStrategy, rootScope, PageObject.Context.ParentContext, LoggerFactory);

      // No error page detection. See remarks documentation on this method.

      return cloneForNewPage;
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides on a new window (specified by <paramref name="windowLocator"/>) within the
    /// same <see cref="IBrowserSession"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="CloneForNewWindow"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the call represents a <b>GET</b> request performed before any control object for this page is created.
    /// The error page detection is performed during the control selection. If you require an explicit error page detection
    /// use <see cref="WebTestHelper"/>.<see cref="WebTestHelper.CheckPageForError"/>.
    /// </remarks>
    public PageObjectContext CloneForNewWindow ([NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty("windowLocator", windowLocator);

      // No error page detection. See remarks documentation on this method.

      var context = CloneForNewWindowInternal(windowLocator);
      context.Window.MaximiseWindow();
      return context;
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides on a new popup window (specified by <paramref name="windowLocator"/>)
    /// within the same <see cref="IBrowserSession"/>. In contrast to <see cref="CloneForNewWindow"/>, the window is not maximized.
    /// </summary>
    /// <remarks>
    /// <see cref="CloneForNewPopupWindow"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the call represents a <b>GET</b> request performed before any control object for this page is created.
    /// The error page detection is performed during the control selection. If you require an explicit error page detection
    /// use <see cref="WebTestHelper"/>.<see cref="WebTestHelper.CheckPageForError"/>.
    /// </remarks>
    public PageObjectContext CloneForNewPopupWindow ([NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty("windowLocator", windowLocator);

      // No error page detection. See remarks documentation on this method.

      return CloneForNewWindowInternal(windowLocator);
    }

    private PageObjectContext CloneForNewWindowInternal (string windowLocator)
    {
      var window = Browser.FindWindow(windowLocator);
      var rootScope = window.GetRootScope();

      return new PageObjectContext(Browser, window, PageObject.Context.RequestErrorDetectionStrategy, rootScope, PageObject.Context, LoggerFactory);
    }

    /// <summary>
    /// Returns a <see cref="ControlSelectionContext"/> based upon the control object context at hand.
    /// </summary>
    /// <remarks>
    /// <see cref="CloneForControlSelection"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the <see cref="PageObject"/> is only intended for use from within implementations of <see cref="IControlSelector"/>, which in turn
    /// perform their own error page detection via <see cref="ControlObjectContext"/>.<see cref="CloneForControl"/>.
    /// </remarks>
    public ControlSelectionContext CloneForControlSelection ()
    {
      // No error page detection. See remarks documentation on this method.

      return new ControlSelectionContext(PageObject, Scope, LoggerFactory);
    }
  }
}
