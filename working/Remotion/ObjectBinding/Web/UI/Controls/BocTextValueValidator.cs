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
using System.Collections;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary>
///   Creates a VS.NET designer pick list for a property that references a <see cref="BocTextValue"/> control.
/// </summary>
/// <remarks>
///   Use the <see cref="TypeConverter"/> attribute to assign this converter to a property.
/// </remarks>
public class BocTextValueControlToStringConverter: ControlToStringConverter
{
  public BocTextValueControlToStringConverter ()
    : base (typeof (BocTextValue))
  {
  }
}

/// <summary>
///   Validates date/time values in the current culture.
/// </summary>
/// <remarks>
///   This class does not provide client-side validation.
/// </remarks>
[ToolboxItemFilter("System.Web.UI")]
public class DateTimeValidator: BaseValidator
{
  protected override bool EvaluateIsValid()
  {
    string text = GetControlValidationValue (ControlToValidate);
    if (text == null )
      return true;
    text = text.Trim();
    if (text.Length == 0)
      return true;

    try
    {
      DateTime.Parse (text);
    }
    catch (FormatException)
    {
      return false;
    }
    return true;
  }
}

/// <summary>
///   Compound validator for <c>FscValue</c> controls.
/// </summary>
/// <remarks>
///   This compound validator automatically creates the following child validators:
///   <list type="table">
///     <listheader>
///       <term>Validator</term>
///       <description>Condition</description>
///     </listheader>
///     <item>
///       <term><see cref="RequiredFieldValidator"/></term>
///       <description>The validated <c>FscValue</c> control's <c>IsRequired</c> property is true.</description>
///     </item>
///   </list>
/// </remarks>
public class BocTextValueValidator: CompoundValidator
{
  public BocTextValueValidator ()
    : base (typeof (BocTextValue))
  {
  }

  [TypeConverter (typeof (BocTextValueControlToStringConverter))]
  public override string ControlToValidate
  {
    get { return base.ControlToValidate; }
    set { base.ControlToValidate = value; }
  }

  public static BaseValidator[] CreateValidators (BocTextValue textValueControl, string baseID)
  {
    ArrayList validators = new ArrayList();

    if (textValueControl.IsRequired)
    {
      RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
      requiredValidator.ID = baseID + "Required";
      requiredValidator.ControlToValidate = textValueControl.TargetControl.ID;
      requiredValidator.ErrorMessage = "Enter a value.";
      requiredValidator.Display = ValidatorDisplay.Dynamic;
      validators.Add (requiredValidator);
    }

    BocTextValueType valueType = textValueControl.ActualValueType;
    if (valueType == BocTextValueType.DateTime)
    {
      DateTimeValidator typeValidator = new DateTimeValidator();
      typeValidator.ID = baseID + "Type";
      typeValidator.ControlToValidate = textValueControl.TargetControl.ID;
      typeValidator.ErrorMessage = "Wrong type.";
      typeValidator.Display = ValidatorDisplay.Dynamic;
      validators.Add (typeValidator);
    }
    else if (valueType != BocTextValueType.Undefined && valueType != BocTextValueType.String)
    {
      CompareValidator typeValidator = new CompareValidator();
      typeValidator.ID = baseID + "Type";
      typeValidator.ControlToValidate = textValueControl.TargetControl.ID;
      typeValidator.Operator = ValidationCompareOperator.DataTypeCheck;
      typeValidator.Type = GetValidationDataType (valueType);
      typeValidator.ErrorMessage = "Wrong type.";
      typeValidator.Display = ValidatorDisplay.Dynamic;
      //typeValidator.EnableClientScript = false;
      validators.Add (typeValidator);
    }

    return (BaseValidator[]) validators.ToArray (typeof (BaseValidator));
  }

  protected override void CreateChildValidators ()
  {
    if (this.Site != null && this.Site.DesignMode)
      return;

    BocTextValue textValueControl = NamingContainer.FindControl (ControlToValidate) as BocTextValue;
    if (textValueControl == null)
      return;
    if (textValueControl.IsReadOnly)
      return;

    string baseID = this.ID + "_Validator";
    foreach (BaseValidator validator in CreateValidators (textValueControl, baseID))
      Controls.Add (validator);
  }

  private static ValidationDataType GetValidationDataType (BocTextValueType valueType)
  {
    switch (valueType)
    {
      case BocTextValueType.Date:
        return ValidationDataType.Date;
      case BocTextValueType.DateTime:
        return ValidationDataType.Date;
      case BocTextValueType.Int32:
        return ValidationDataType.Integer;
      case BocTextValueType.Double:
        return ValidationDataType.Double;
      default:
        throw new ArgumentException ("Cannot convert " + valueType.ToString() + " to type " + typeof (ValidationDataType).FullName + ".");
    }
  }
}

}
