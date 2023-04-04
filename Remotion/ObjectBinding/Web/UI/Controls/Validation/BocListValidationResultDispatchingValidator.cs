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
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public sealed class BocListValidationResultDispatchingValidator : BaseValidator, IBusinessObjectBoundEditableWebControlValidationResultDispatcher
  {
    public void DispatchValidationFailures (IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull(nameof(validationResult), validationResult);

      var bocListControl = GetControlToValidate();
      var rowObjects = bocListControl.Value?.Cast<IBusinessObject>();
      if (rowObjects != null && bocListControl is IBocListWithValidationSupport bocListWithValidationSupport)
      {
        var columnsAndMatchers = bocListWithValidationSupport.GetColumnsWithValidationSupport()
            .Select(c => new { Column = (BocColumnDefinition)c, Matcher = c.GetValidationFailureMatcher() })
            .ToArray();

        var validationFailureRepository = bocListWithValidationSupport.ValidationFailureRepository;
        validationFailureRepository.Clear();

        foreach (var rowObject in rowObjects)
        {
          foreach (var columnAndMatcher in columnsAndMatchers)
          {
            var matched = columnAndMatcher.Matcher.GetMatchingValidationFailures(rowObject, validationResult);
            if (matched.Count > 0)
              validationFailureRepository.AddRowValidationFailures(rowObject, columnAndMatcher.Column, matched);
          }

          var otherRowFailures = validationResult.GetValidationFailures(rowObject);
          if (otherRowFailures.Count > 0)
            validationFailureRepository.AddRowValidationFailures(rowObject, otherRowFailures);
        }

        var businessObject = bocListControl.DataSource?.BusinessObject;
        var property = bocListControl.Property;
        if (businessObject != null && property != null)
        {
          var listPropertyFailures = validationResult.GetValidationFailures(businessObject, property, true);
          if (listPropertyFailures.Count > 0)
            validationFailureRepository.AddListValidationFailures(listPropertyFailures);
        }
      }
    }


    protected override bool EvaluateIsValid ()
    {
      // TODO RM-6056: Shows a validation error if IBusinessObjectValidationResult returned messages for this control.
      return true;
    }

    protected override bool ControlPropertiesValid ()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty(controlToValidate))
        return base.ControlPropertiesValid();
      else
        return NamingContainer.FindControl(controlToValidate) != null;
    }

    private BocList GetControlToValidate ()
    {
      var control = NamingContainer.FindControl(ControlToValidate);
      var bocListControl = control as BocList;
      if (bocListControl == null)
      {
        throw new InvalidOperationException(
            $"'{nameof(BocListValidationResultDispatchingValidator)}' may only be applied to controls of type '{nameof(BocList)}'.");
      }

      return bocListControl;
    }
  }
}
