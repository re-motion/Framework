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
using System.Web.UI;
using System.Web.UI.WebControls;
using FluentValidation.Results;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public sealed class UserControlBindingValidationFailureDisptacher : BaseValidator, IBusinessObjectBoundEditableWebControlValidationFailureDispatcher
  {
    public UserControlBindingValidationFailureDisptacher ()
    {
    }

    public IEnumerable<ValidationFailure> DispatchValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      ArgumentUtility.CheckNotNull ("failures", failures);

      var control = NamingContainer.FindControl (ControlToValidate);
      var userControlBinding = control as UserControlBinding;
      if (userControlBinding == null)
        throw new InvalidOperationException ("UserControlBindingValidationFailureDisptacher may only be applied to controls of type UserControlBinding");

      var namingContainer = userControlBinding.UserControl.DataSource.NamingContainer;
      var validator =
          EnumerableUtility.SelectRecursiveDepthFirst (
              namingContainer,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
              .OfType<BocDataSourceValidationFailureDisptachingValidator>()
              .SingleOrDefault (
                  c => c.ControlToValidate == userControlBinding.UserControl.DataSource.ID,
                  () => new InvalidOperationException ("Only zero or one BocDataSourceValidationFailureDisptachingValidator is allowed per UserControlBinding."));

      List<ValidationFailure> unhandledFailures;
      if (validator != null)
        unhandledFailures = validator.DispatchValidationFailures (failures).ToList();
      else
        unhandledFailures = failures.ToList();

      return unhandledFailures;
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
        return base.ControlPropertiesValid ();
      else
        return NamingContainer.FindControl (controlToValidate) != null;
    }
     
  }
}