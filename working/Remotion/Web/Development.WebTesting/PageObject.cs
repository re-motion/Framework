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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all page objects.<br/>
  /// All derived page object types must provide a constructor with a single <see cref="PageObjectContext"/> argument!
  /// </summary>
  /// <remarks>
  /// See http://martinfowler.com/bliki/PageObject.html or https://code.google.com/p/selenium/wiki/PageObjects for more information on the page object
  /// pattern.
  /// </remarks>
  public abstract class PageObject : WebTestObject<PageObjectContext>, IControlHost
  {
    protected PageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the page's title.<br/>
    /// The default implementation returns the HTML &lt;title&gt; tag's trimmed content.
    /// </summary>
    public virtual string GetTitle ()
    {
      // Note: do not use Context.Window.Title - this would return wrong titles for page objects representing the contents of an IFRAME.
      return Scope.FindCss ("title").InnerHTML.Trim();
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void Close ()
    {
      Context.CloseWindow();
    }

    /// <inheritdoc/>
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return controlSelectionCommand.Select (Context.CloneForControlSelection (this));
    }
  }
}