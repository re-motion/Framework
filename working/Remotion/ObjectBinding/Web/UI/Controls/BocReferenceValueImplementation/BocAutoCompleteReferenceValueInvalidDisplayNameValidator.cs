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
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  /// <summary>
  /// Validator for <see cref="BocAutoCompleteReferenceValue"/> that checks whether the display name corresponds to a valid item.
  /// </summary>
  public class BocAutoCompleteReferenceValueInvalidDisplayNameValidator : BaseValidator
  {
    public BocAutoCompleteReferenceValueInvalidDisplayNameValidator ()
    {
    }

    protected override bool EvaluateIsValid ()
    {
      var control = NamingContainer.FindControl (ControlToValidate);

      var autoCompleteReferenceValue = control as BocAutoCompleteReferenceValue;

      if (autoCompleteReferenceValue == null)
      {
        throw new InvalidOperationException (
            "BocAutoCompleteReferenceValueInvalidDisplayNameValidator may only be applied to controls of type BocAutoCompleteReferenceValue");
      }

      var validationValue = GetControlValidationValue (ControlToValidate);
      if (string.IsNullOrEmpty (validationValue))
        return true;

      var validationValueParts = validationValue.Split (new[] { '\n' }, StringSplitOptions.None);
      if (validationValueParts.Length != 2)
      {
        throw new InvalidOperationException (
          string.Format ("ValidationValue for control '{0}' has an invalid format.", ControlToValidate));
      }

      if (string.IsNullOrEmpty (validationValueParts[0]) && !string.IsNullOrEmpty (validationValueParts[1]))
        return false;

      return true;
    }
  }
}