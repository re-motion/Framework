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
using System.Globalization;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation
{
  /// <summary>
  /// Implements the <see cref="IBocTextValueValidatorFactory"/> inteface and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IBocTextValueValidatorFactory"/>
  [ImplementationFor(typeof(IBocTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocTextValueValidatorFactory : IBocTextValueValidatorFactory
  {
    public const int Position = 0;

    public BocTextValueValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      if (isReadOnly)
        yield break;

      IResourceManager resourceManager = control.GetResourceManager();

      var requiredFieldValidator = CreateRequiredFieldValidator(control, resourceManager);
      if (requiredFieldValidator != null)
        yield return requiredFieldValidator;

      var lengthValidator = CreateLengthValidator(control, resourceManager);
      if (lengthValidator != null)
        yield return lengthValidator;

      var typeValidator = CreateTypeValidator(control, resourceManager);
      if (typeValidator != null)
        yield return typeValidator;
    }

    private RequiredFieldValidator? CreateRequiredFieldValidator (IBocTextValue control, IResourceManager resourceManager)
    {
      var areOptionalValidatorsEnabled = control.AreOptionalValidatorsEnabled;
      var isPropertyTypeRequired = !areOptionalValidatorsEnabled && control.DataSource?.BusinessObject != null && control.Property?.IsNullable == false;
      var isControlRequired = areOptionalValidatorsEnabled && control.IsRequired;

      if (isPropertyTypeRequired || isControlRequired)
      {
        RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
        requiredValidator.ID = control.ID + "_ValidatorRequired";
        requiredValidator.ControlToValidate = control.TargetControl.ID;
        requiredValidator.ErrorMessage = resourceManager.GetString(BocTextValue.ResourceIdentifier.RequiredErrorMessage);
        requiredValidator.EnableViewState = false;

        return requiredValidator;
      }
      else
      {
        return null;
      }
    }

    private LengthValidator? CreateLengthValidator (IBocTextValue control, IResourceManager resourceManager)
    {
      var maxLength = control.TextBoxStyle.GetMaxLength();
      var hasControlMaxLength = control.AreOptionalValidatorsEnabled && maxLength.HasValue;

      if (hasControlMaxLength)
      {
        LengthValidator lengthValidator = new LengthValidator();
        lengthValidator.ID = control.ID + "_ValidatorMaxLength";
        lengthValidator.ControlToValidate = control.TargetControl.ID;
        lengthValidator.MaximumLength = maxLength;
        lengthValidator.ErrorMessage = string.Format(
            resourceManager.GetString(BocTextValue.ResourceIdentifier.MaxLengthValidationMessage),
            maxLength);
        lengthValidator.EnableViewState = false;

        return lengthValidator;
      }
      else
      {
        return null;
      }
    }

    [CanBeNull]
    private BaseValidator? CreateTypeValidator (IBocTextValue control, IResourceManager resourceManager)
    {
      BocTextValueType valueType = control.ActualValueType;
      switch (valueType)
      {
        case BocTextValueType.Undefined:
          return null;
        case BocTextValueType.String:
          return CreateTypeIsStringValidator(control, resourceManager);
        case BocTextValueType.DateTime:
          return CreateTypeIsDateTimeValidator(control, resourceManager);
        case BocTextValueType.Date:
          return CreateTypeIsDateValidator(control, resourceManager);
        case BocTextValueType.Byte:
        case BocTextValueType.Int16:
        case BocTextValueType.Int32:
        case BocTextValueType.Int64:
        case BocTextValueType.Decimal:
        case BocTextValueType.Double:
        case BocTextValueType.Single:
          return CreateTypeIsNumericValidator(control, valueType, resourceManager);
        default:
        {
          throw new InvalidOperationException(
              "BocTextValue '" + control.ID + "': Cannot convert " + valueType + " to type " + typeof(ValidationDataType).GetFullNameSafe() + ".");
        }
      }
    }

    private ControlCharactersCharactersValidator CreateTypeIsStringValidator (IBocTextValue control, IResourceManager resourceManager)
    {
      ControlCharactersCharactersValidator typeValidator = new ControlCharactersCharactersValidator();
      typeValidator.ID = control.ID + "_ValidatorType";
      typeValidator.ControlToValidate = control.TargetControl.ID;
      typeValidator.SampleTextLength = 5;
      if (control.TextBoxStyle.TextMode == BocTextBoxMode.MultiLine)
      {
        typeValidator.EnableMultilineText = true;
        typeValidator.ErrorMessageFormat = resourceManager.GetString(BocTextValue.ResourceIdentifier.InvalidCharactersForMultiLineErrorMessage);
      }
      else
      {
        typeValidator.EnableMultilineText = false;
        typeValidator.ErrorMessageFormat = resourceManager.GetString(BocTextValue.ResourceIdentifier.InvalidCharactersForSingleLineErrorMessage);
      }
      typeValidator.EnableViewState = false;

      return typeValidator;
    }

    private DateTimeValidator CreateTypeIsDateTimeValidator (IBocTextValue control, IResourceManager resourceManager)
    {
      DateTimeValidator typeValidator = new DateTimeValidator();
      typeValidator.ID = control.ID + "_ValidatorType";
      typeValidator.ControlToValidate = control.TargetControl.ID;
      typeValidator.ErrorMessage = resourceManager.GetString(BocTextValue.ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      typeValidator.EnableViewState = false;

      return typeValidator;
    }

    private CompareValidator CreateTypeIsDateValidator (IBocTextValue control, IResourceManager resourceManager)
    {
      CompareValidator typeValidator = new CompareValidator();
      typeValidator.ID = control.ID + "_ValidatorType";
      typeValidator.ControlToValidate = control.TargetControl.ID;
      typeValidator.Operator = ValidationCompareOperator.DataTypeCheck;
      typeValidator.Type = ValidationDataType.Date;
      typeValidator.ErrorMessage = resourceManager.GetString(BocTextValue.ResourceIdentifier.InvalidDateErrorMessage);
      typeValidator.EnableViewState = false;

      return typeValidator;
    }

    private NumericValidator CreateTypeIsNumericValidator (IBocTextValue control, BocTextValueType valueType, IResourceManager resourceManager)
    {
      NumericValidator typeValidator = new NumericValidator();
      typeValidator.ID = control.ID + "_ValidatorType";
      typeValidator.ControlToValidate = control.TargetControl.ID;
      if (control.Property != null)
        typeValidator.AllowNegative = ((IBusinessObjectNumericProperty)control.Property).AllowNegative;
      typeValidator.DataType = GetNumericValidatorDataType(valueType);
      typeValidator.NumberStyle = GetNumberStyle(valueType);
      typeValidator.ErrorMessage = resourceManager.GetString(GetNumericValidatorErrorMessage(GetNumericValidatorDataType(valueType)));
      typeValidator.EnableViewState = false;

      return typeValidator;
    }

    private NumericValidationDataType GetNumericValidatorDataType (BocTextValueType valueType)
    {
      switch (valueType)
      {
        case BocTextValueType.Byte:
          return NumericValidationDataType.Byte;

        case BocTextValueType.Int16:
          return NumericValidationDataType.Int16;

        case BocTextValueType.Int32:
          return NumericValidationDataType.Int32;

        case BocTextValueType.Int64:
          return NumericValidationDataType.Int64;

        case BocTextValueType.Decimal:
          return NumericValidationDataType.Decimal;

        case BocTextValueType.Double:
          return NumericValidationDataType.Double;

        case BocTextValueType.Single:
          return NumericValidationDataType.Single;

        default:
          throw new ArgumentOutOfRangeException("valueType", valueType, "Only numeric value types are supported.");
      }
    }

    private NumberStyles GetNumberStyle (BocTextValueType valueType)
    {
      switch (valueType)
      {
        case BocTextValueType.Byte:
        case BocTextValueType.Int16:
        case BocTextValueType.Int32:
        case BocTextValueType.Int64:
          return NumberStyles.Number & ~NumberStyles.AllowDecimalPoint;

        case BocTextValueType.Decimal:
          return NumberStyles.Number;

        case BocTextValueType.Double:
        case BocTextValueType.Single:
          return NumberStyles.Number | NumberStyles.AllowExponent;

        default:
          throw new ArgumentOutOfRangeException("valueType", valueType, "Only numeric value types are supported.");
      }
    }

    private BocTextValue.ResourceIdentifier GetNumericValidatorErrorMessage (NumericValidationDataType dataType)
    {
      switch (dataType)
      {
        case NumericValidationDataType.Byte:
          return BocTextValue.ResourceIdentifier.InvalidIntegerErrorMessage;

        case NumericValidationDataType.Decimal:
          return BocTextValue.ResourceIdentifier.InvalidDoubleErrorMessage;

        case NumericValidationDataType.Double:
          return BocTextValue.ResourceIdentifier.InvalidDoubleErrorMessage;

        case NumericValidationDataType.Int16:
          return BocTextValue.ResourceIdentifier.InvalidIntegerErrorMessage;

        case NumericValidationDataType.Int32:
          return BocTextValue.ResourceIdentifier.InvalidIntegerErrorMessage;

        case NumericValidationDataType.Int64:
          return BocTextValue.ResourceIdentifier.InvalidIntegerErrorMessage;

        case NumericValidationDataType.Single:
          return BocTextValue.ResourceIdentifier.InvalidDoubleErrorMessage;

        default:
          throw new ArgumentOutOfRangeException("dataType", dataType, "Only numeric value types are supported.");
      }
    }
  }
}
