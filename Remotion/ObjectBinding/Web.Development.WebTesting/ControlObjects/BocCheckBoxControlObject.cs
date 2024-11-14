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
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox"/> control.
  /// </summary>
  public class BocCheckBoxControlObject : BocControlObject, IControlObjectWithFormElements, ISupportsValidationErrors, ISupportsValidationErrorsForReadOnly
  {
    public BocCheckBoxControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Returns the current state of the check box.
    /// </summary>
    public bool GetState ()
    {
      if (IsReadOnly())
        return ParseState(Scope.FindChild("Value")["data-value"]);

      return Scope.FindChild("Value")["checked"] != null;
    }

    /// <summary>
    /// Sets the state of the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocCheckBox"/> to <paramref name="newState"/>.
    /// </summary>
    /// <exception cref="WebTestException">The element is currently disabled.</exception>
    public UnspecifiedPageObject SetTo (bool newState, [CanBeNull] IWebTestActionOptions? actionOptions = null)
    {
      if (IsDisabled())
        throw AssertionExceptionUtility.CreateControlDisabledException(Driver);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      if (GetState() == newState)
        return UnspecifiedPage();

      var actualActionOptions = MergeWithDefaultActionOptions(Scope, actionOptions);

      if (newState)
        ExecuteAction(new CheckAction(this, Scope.FindChild("Value"), Logger), actualActionOptions);
      else
        ExecuteAction(new UncheckAction(this, Scope.FindChild("Value"), Logger), actualActionOptions);

      return UnspecifiedPage();
    }

    public IReadOnlyList<string> GetValidationErrors ()
    {
      return GetValidationErrors(GetScopeWithReferenceInformation());
    }

    public IReadOnlyList<string> GetValidationErrorsForReadOnly ()
    {
      return GetValidationErrorsForReadOnly(GetScopeWithReferenceInformation());
    }

    protected override ElementScope GetLabeledElementScope ()
    {
      return GetScopeWithReferenceInformation();
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the input[type=checkbox] (value) as only element.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      return new[] { string.Format("{0}_Value", GetHtmlID()) };
    }

    private bool ParseState (string state)
    {
      if (state == "False")
        return false;

      if (state == "True")
        return true;

      throw new ArgumentException("must be either 'True' or 'False'", "state");
    }

    private ElementScope GetScopeWithReferenceInformation ()
    {
      return Scope.FindChild("Value");
    }
  }
}
