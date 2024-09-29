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
  /// Context for a <see cref="PageObject"/>. Provides various Coypu-based references into the DOM.
  /// </summary>
  public class PageObjectContext : WebTestObjectContext
  {
    private readonly IBrowserSession _browser;
    private readonly BrowserWindow _window;
    private readonly IRequestErrorDetectionStrategy _requestErrorDetectionStrategy;
    private readonly PageObjectContext? _parentContext;

    /// <summary>
    /// Private constructor, use <see cref="New"/> to create a new root <see cref="PageObjectContext"/>.
    /// </summary>
    /// <exception cref="WebTestException">
    /// If the scope cannot be found.
    /// <para>- or -</para>
    /// If multiple matching scopes are found.
    /// </exception>
    internal PageObjectContext (
        [NotNull] IBrowserSession browser,
        [NotNull] BrowserWindow window,
        [NotNull] IRequestErrorDetectionStrategy requestErrorDetectionStrategy,
        [NotNull] ElementScope scope,
        [CanBeNull] PageObjectContext? parentContext,
        [NotNull] ILoggerFactory loggerFactory)
        : base(scope, loggerFactory)
    {
      ArgumentUtility.CheckNotNull("browser", browser);
      ArgumentUtility.CheckNotNull("window", window);
      ArgumentUtility.CheckNotNull("requestErrorDetectionStrategy", requestErrorDetectionStrategy);

      _browser = browser;
      _window = window;
      _requestErrorDetectionStrategy = requestErrorDetectionStrategy;
      _parentContext = parentContext;
    }

    /// <summary>
    /// Returns a new root context for a <see cref="PageObject"/> without a parent (e.g. a parent frame).
    /// </summary>
    /// <param name="browser">The <see cref="IBrowserSession"/> on which the <see cref="PageObject"/> resides.</param>
    /// <param name="requestErrorDetectionStrategy">The <see cref="IRequestErrorDetectionStrategy"/> which defines the used request error detection.</param>
    /// <param name="loggerFactory">The logger factory used by the web testing infrastructure.</param>
    /// <returns>A new root context.</returns>
    /// <remarks>
    /// <see cref="New"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the call represents a <b>GET</b> request performed before any control object for this page is created.
    /// The error page detection is performed during the control selection. If you require an explicit error page detection
    /// use <see cref="WebTestHelper"/>.<see cref="WebTestHelper.CheckPageForError"/>.
    /// </remarks>
    public static PageObjectContext New (
        [NotNull] IBrowserSession browser,
        [NotNull] IRequestErrorDetectionStrategy requestErrorDetectionStrategy,
        [NotNull] ILoggerFactory loggerFactory)
    {
      ArgumentUtility.CheckNotNull("browser", browser);
      ArgumentUtility.CheckNotNull("requestErrorDetectionStrategy", requestErrorDetectionStrategy);
      ArgumentUtility.CheckNotNull("loggerFactory", loggerFactory);

      var scope = browser.Window.GetRootScope();

      // No error page detection. See remarks documentation on this method.

      return new PageObjectContext(browser, browser.Window, requestErrorDetectionStrategy, scope, null, loggerFactory);
    }

    /// <inheritdoc/>
    public override IBrowserSession Browser
    {
      get { return _browser; }
    }

    /// <inheritdoc/>
    public override BrowserWindow Window
    {
      get { return _window; }
    }

    /// <summary>
    /// Returns the <see cref="PageObject"/>'s parent context, e.g. a <see cref="PageObject"/> representing the parent frame or the parent window.
    /// This property returns <see langword="null"/> for root contexts.
    /// </summary>
    public PageObjectContext? ParentContext
    {
      get { return _parentContext; }
    }

    /// <summary>
    /// Returns the <see cref="IRequestErrorDetectionStrategy"/> implementation passed in the <see cref="PageObjectContext"/>constructor.
    /// </summary>
    public IRequestErrorDetectionStrategy RequestErrorDetectionStrategy
    {
      get { return _requestErrorDetectionStrategy; }
    }

    /// <summary>
    /// Clones the context for a new <see cref="IBrowserSession"/>.
    /// </summary>
    /// <param name="browserSession">The new <see cref="IBrowserSession"/>.</param>
    /// <remarks>
    /// <see cref="CloneForSession"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the call represents a <b>GET</b> request performed before any control object for this page is created.
    /// The error page detection is performed during the control selection. If you require an explicit error page detection
    /// use <see cref="WebTestHelper"/>.<see cref="WebTestHelper.CheckPageForError"/>.
    /// </remarks>
    public PageObjectContext CloneForSession ([NotNull] IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);

      var rootScope = browserSession.Window.GetRootScope();

      var pageObjectContext = new PageObjectContext(browserSession, browserSession.Window, RequestErrorDetectionStrategy, rootScope, this, LoggerFactory);

      // No error page detection. See remarks documentation on this method.

      return pageObjectContext;
    }

    /// <summary>
    /// Clones the context for a child <see cref="PageObject"/> which represents an IFRAME on the page.
    /// </summary>
    /// <param name="frameScope">The scope of the <see cref="PageObject"/> representing the IFRAME.</param>
    /// <exception cref="WebTestException">
    /// If the frame content cannot be found.
    /// <para>- or -</para>
    /// If multiple matching frame contents are found.
    /// </exception>
    public PageObjectContext CloneForFrame ([NotNull] ElementScope frameScope)
    {
      ArgumentUtility.CheckNotNull("frameScope", frameScope);

      var frameRootElement = frameScope.FindCss("html");

      try
      {
        return new PageObjectContext(Browser, Window, RequestErrorDetectionStrategy, frameRootElement, this, LoggerFactory);
      }
      catch (WebTestException)
      {
        RequestErrorDetectionStrategy.CheckPageForError(Scope);

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

      var cloneForNewPage = new PageObjectContext(Browser, Window, RequestErrorDetectionStrategy, rootScope, ParentContext, LoggerFactory);

      // No error page detection. See remarks documentation on this method.

      return cloneForNewPage;
    }

    /// <summary>
    /// Creates a <see cref="ControlObjectContext"/> for a <see cref="ControlObject"/> which resides on the page.
    /// </summary>
    /// <param name="pageObject">
    /// As the <see cref="PageObjectContext"/> does not know about the <see cref="PageObject"/> it belongs to, the <paramref name="pageObject"/> must
    /// be specified.
    /// </param>
    /// <param name="scope">The scope of the <see cref="ControlObject"/>.</param>
    /// <exception cref="WebTestException">
    /// If the control cannot be found.
    /// <para>- or -</para>
    /// If multiple matching controls are found.
    /// </exception>
    public ControlObjectContext CloneForControl ([NotNull] PageObject pageObject, [NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("pageObject", pageObject);
      ArgumentUtility.CheckNotNull("scope", scope);

      try
      {
        return new ControlObjectContext(pageObject, scope, LoggerFactory);
      }
      catch (WebTestException)
      {
        RequestErrorDetectionStrategy.CheckPageForError(pageObject.Context.Scope);

        throw;
      }
    }

    /// <summary>
    /// Returns a <see cref="ControlSelectionContext"/> based upon the page object context at hand. As the <see cref="PageObjectContext"/> does not
    /// know about the <see cref="PageObject"/> it belongs to, the <paramref name="pageObject"/> must be specified.
    /// </summary>
    /// <remarks>
    /// <see cref="CloneForControlSelection"/> does not perform an error page detection via <see cref="IRequestErrorDetectionStrategy"/>
    /// because the <see cref="PageObject"/> is only intended for use from within implementations of <see cref="IControlSelector"/>, which in turn
    /// perform their own error page detection via <see cref="ControlObjectContext"/>.<see cref="CloneForControl"/>.
    /// </remarks>
    public ControlSelectionContext CloneForControlSelection (PageObject pageObject)
    {
      ArgumentUtility.CheckNotNull("pageObject", pageObject);

      // No error page detection. See remarks documentation on this method.

      return new ControlSelectionContext(pageObject, Scope, LoggerFactory);
    }
  }
}
