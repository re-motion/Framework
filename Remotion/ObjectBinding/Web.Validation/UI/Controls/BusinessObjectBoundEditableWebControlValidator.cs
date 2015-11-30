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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public sealed class BusinessObjectBoundEditableWebControlValidator : BaseValidator, IBusinessObjectBoundEditableWebControlValidationFailureDispatcher
  {
    private List<ValidationFailure> _validationFailures = new List<ValidationFailure>();

    public BusinessObjectBoundEditableWebControlValidator ()
    {
    }

    public IEnumerable<ValidationFailure> DispatchValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      ArgumentUtility.CheckNotNull ("failures", failures);

      var bocControl = GetControlToValidate();
      if (bocControl == null)
      {
        throw new InvalidOperationException (
            "BusinessObjectBoundEditableWebControlValidator may only be applied to controls of type BusinessObjectBoundEditableWebControl");
      }

      _validationFailures = new List<ValidationFailure>();
      foreach (var failure in failures)
      {
        if (BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (bocControl, failure))
          _validationFailures.Add (failure);
        else
          yield return failure;
      }

      if (_validationFailures.Any())
        Validate();

      ErrorMessage = string.Join ("\r\n", _validationFailures.Select (f => f.ErrorMessage));
    }

    protected override bool EvaluateIsValid ()
    {
      return !_validationFailures.Any();
    }

    private BusinessObjectBoundEditableWebControl GetControlToValidate ()
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      return control as BusinessObjectBoundEditableWebControl;
    }
  }
}