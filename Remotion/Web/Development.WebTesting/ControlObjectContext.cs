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
using Remotion.Utilities;
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
    internal ControlObjectContext ([NotNull] PageObject pageObject, [NotNull] ElementScope scope)
        : base (scope)
    {
      ArgumentUtility.CheckNotNull ("pageObject", pageObject);

      _pageObject = pageObject;
    }

    /// <inheritdoc/>
    public override BrowserSession Browser
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
    /// Clones the context for another <see cref="ControlObject"/> which resides within the same <see cref="BrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and on the same page.
    /// </summary>
    /// <param name="scope">The scope of the other <see cref="ControlObject"/>.</param>
    public ControlObjectContext CloneForControl ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return new ControlObjectContext (PageObject, scope);
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides within the same <see cref="BrowserSession"/>, on the same
    /// <see cref="BrowserWindow"/> and replaces the current <see cref="PageObject"/>.
    /// </summary>
    public PageObjectContext CloneForNewPage ()
    {
      var rootScope = Window.GetRootScope();
      return new PageObjectContext (Browser, Window, rootScope, PageObject.Context.ParentContext);
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides on a new window (specified by <paramref name="windowLocator"/>) within the
    /// same <see cref="BrowserSession"/>.
    /// </summary>
    public PageObjectContext CloneForNewWindow ([NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      var context = CloneForNewWindowInternal (windowLocator);
      context.Window.MaximiseWindow();
      return context;
    }

    /// <summary>
    /// Clones the context for a new <see cref="PageObject"/> which resides on a new popup window (specified by <paramref name="windowLocator"/>)
    /// within the same <see cref="BrowserSession"/>. In contrast to <see cref="CloneForNewWindow"/>, the window is not maximized.
    /// </summary>
    public PageObjectContext CloneForNewPopupWindow ([NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return CloneForNewWindowInternal (windowLocator);
    }

    private PageObjectContext CloneForNewWindowInternal (string windowLocator)
    {
      var window = Browser.FindWindow (windowLocator);
      var rootScope = window.GetRootScope();
      return new PageObjectContext (Browser, window, rootScope, PageObject.Context);
    }

    /// <summary>
    /// Returns a <see cref="ControlSelectionContext"/> based upon the control object context at hand.
    /// </summary>
    public ControlSelectionContext CloneForControlSelection ()
    {
      return new ControlSelectionContext (PageObject, Scope);
    }
  }
}