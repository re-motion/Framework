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
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Validation.Results;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public sealed class BocListValidator : BaseValidator, IBusinessObjectBoundEditableWebControlValidationFailureDispatcher
  {
    private List<ValidationFailure> _validationFailures = new List<ValidationFailure>();

    public BocListValidator ()
    {
    }

    public IEnumerable<ValidationFailure> DispatchValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      ArgumentUtility.CheckNotNull ("failures", failures);

      var bocListControl = GetControlToValidate();
      if (bocListControl == null)
        throw new InvalidOperationException ("BocListValidator may only be applied to controls of type BocList");

    //  var namingContainer = bocListControl.NamingContainer;
      var validator =
          EnumerableUtility.SelectRecursiveDepthFirst (
              bocListControl as Control,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
              .OfType<IBusinessObjectBoundEditableWebControlValidationFailureDispatcher> ();

      
      _validationFailures = new List<ValidationFailure>();
      foreach (var failure in failures)
      {
        if (BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (bocListControl, failure))
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

    private BocList GetControlToValidate()
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      return control as BocList;
    }

    protected override bool ControlPropertiesValid ()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty (controlToValidate))
        return base.ControlPropertiesValid();
      else
        return NamingContainer.FindControl (controlToValidate) != null;
    }
  }
}