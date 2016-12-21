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
using System.Web.UI.WebControls;
using FluentValidation.Results;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public class BocValidator : BaseValidator, IBocValidator
  {
    private List<ValidationFailure> _validationFailures = new List<ValidationFailure> ();

    public IEnumerable<ValidationFailure> ApplyValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      var bocControl = control as BusinessObjectBoundEditableWebControl;
      if (bocControl == null)
        throw new InvalidOperationException ("BocValidator may only be applied to controls of type BusinessObjectBoundEditableWebControl");

      _validationFailures = new List<ValidationFailure>();
      foreach (var failure in failures)
      {
        if (IsMatchingControl (failure, bocControl))
          _validationFailures.Add (failure);
        else
          yield return failure;
      }

      if (_validationFailures.Any())
        Validate();

      ErrorMessage = string.Join ("\r\n", _validationFailures.Select (f => f.ErrorMessage));
    }

    private bool IsMatchingControl (ValidationFailure failure, BusinessObjectBoundEditableWebControl bocControl)
    {
      if (!bocControl.HasValidBinding)
        return false;
      //if (failure.GetValidatedInstance != bocControl.DataSource.BusinessObject)
      // return false;
      if (failure.PropertyName == bocControl.PropertyIdentifier)
        return true;
      if (GetShortPropertyName (failure) == bocControl.PropertyIdentifier)
        return true;
      return false;
    }

    private string GetShortPropertyName (ValidationFailure failure)
    {
      return failure.PropertyName.Split ('c').Last();
    }

    protected override bool EvaluateIsValid ()
    {
      return !_validationFailures.Any ();
    }
  }
}