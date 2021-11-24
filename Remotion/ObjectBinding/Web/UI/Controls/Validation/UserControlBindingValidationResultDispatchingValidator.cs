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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public sealed class UserControlBindingValidationResultDispatchingValidator
      : BaseValidator, IBusinessObjectBoundEditableWebControlValidationResultDispatcher
  {
    public UserControlBindingValidationResultDispatchingValidator ()
    {
    }

    public void DispatchValidationFailures (IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull ("validationResult", validationResult);

      var control = GetControlToValidate();

      Assertion.IsNotNull (control.UserControl, "control.UserControl must not be null.");

      var namingContainer = control.UserControl.DataSource.NamingContainer;
      var validator = EnumerableUtility.SelectRecursiveDepthFirst (
              namingContainer,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
          .OfType<BindableObjectDataSourceControlValidationResultDispatchingValidator>()
          .SingleOrDefault (
              c => c.ControlToValidate == control.UserControl.DataSource.ID,
              () => new InvalidOperationException (
                  $"Only one '{nameof (UserControlBindingValidationResultDispatchingValidator)}' is allowed per '{nameof (UserControlBinding)}'."));

      validator?.DispatchValidationFailures (validationResult);
    }

    protected override bool EvaluateIsValid ()
    {
      // This validator is never invalid because it just dispatches the errors.
      return true;
    }

    protected override bool ControlPropertiesValid ()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty (controlToValidate))
        return base.ControlPropertiesValid();
      else
        return NamingContainer.FindControl (controlToValidate) != null;
    }

    private UserControlBinding GetControlToValidate ()
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      var userControlBinding = control as UserControlBinding;
      if (userControlBinding == null)
      {
        throw new InvalidOperationException (
            $"'{nameof (UserControlBindingValidationResultDispatchingValidator)}' may only be applied to controls of type '{nameof (UserControlBinding)}'.");
      }

      return userControlBinding;
    }
  }
}