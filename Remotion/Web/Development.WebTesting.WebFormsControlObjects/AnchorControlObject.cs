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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:System.Web.UI.WebControls.LinkButton"/>, <see cref="T:System.Web.UI.WebControls.HyperLink"/> and all their
  /// derivatives (e.g. also <see cref="T:Remotion.Web.UI.Controls.WebLinkButton"/> or <see cref="T:Remotion.Web.UI.Controls.SmartHyperLink"/>). Also
  /// represents a simple HTML anchor &lt;a&gt; control within an applicaiton.
  /// </summary>
  public class AnchorControlObject : WebFormsControlObject, IClickableControlObject, IControlObjectWithText
  {
    public AnchorControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope.Text;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject Click (IWebTestActionOptions? actionOptions = null)
    {
      var actualActionOptions = MergeWithDefaultActionOptions(Scope, actionOptions);
      ExecuteAction(new ClickAction(this, Scope, Logger), actualActionOptions);
      return UnspecifiedPage();
    }

    /// <inheritdoc/>
    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      if (IsPostBackLink(scope))
        return ((IWebFormsPageObject)Context.PageObject).PostBackCompletionDetectionStrategy;

      if (IsSimpleJavaScriptLink(scope))
        return Continue.Immediately;

      return ((IWebFormsPageObject)Context.PageObject).NavigationCompletionDetectionStrategy;
    }

    private bool IsPostBackLink (ElementScope scope)
    {
      const string doPostBackScript = "__doPostBack";
      const string doPostBackWithOptionsScript = "DoPostBackWithOptions";

      return scope["href"].Contains(doPostBackScript) ||
             scope["href"].Contains(doPostBackWithOptionsScript) ||
             (TargetsCurrentPage(scope["href"]) && scope["onclick"] != null && scope["onclick"].Contains(doPostBackScript));
    }

    private bool IsSimpleJavaScriptLink (ElementScope scope)
    {
      return TargetsCurrentPage(scope["href"]) && scope["onclick"] != null && scope["onclick"].Contains("javascript:");
    }

    private bool TargetsCurrentPage (string href)
    {
      // Note: unfortunately, Selenium sometimes reports wrong href contents, therefore we have to check for the window location as well.
      // See https://code.google.com/p/selenium/issues/detail?id=1824 for why GetAttribute("href") returns an absolute URL.
      var windowLocation = Context.RootScope.Location.ToString();
      return href.Equals("#") || href.Equals(windowLocation) || href.Equals(windowLocation + "#");
    }
  }
}
