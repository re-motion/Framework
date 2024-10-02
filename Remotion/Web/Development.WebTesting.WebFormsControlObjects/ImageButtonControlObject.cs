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
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object for <see cref="T:System.Web.UI.WebControls.ImageButton"/>.
  /// </summary>
  public class ImageButtonControlObject : WebFormsControlObject, IClickableControlObject, ISupportsDisabledState
  {
    public ImageButtonControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Returns the src URL of the button's image.
    /// </summary>
    /// <returns></returns>
    public string GetImageSourceUrl ()
    {
      return Scope["src"];
    }

    /// <inheritdoc />
    public bool IsDisabled ()
    {
      return Scope.Disabled;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject Click (IWebTestActionOptions? actionOptions = null)
    {
      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

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

      return ((IWebFormsPageObject)Context.PageObject).NavigationCompletionDetectionStrategy;
    }

    private bool IsPostBackLink (ElementScope scope)
    {
      const string doPostBackWithOptionsScript = "DoPostBackWithOptions";

      return scope["onclick"] != null && scope["onclick"].Contains(doPostBackWithOptionsScript);
    }
  }
}
