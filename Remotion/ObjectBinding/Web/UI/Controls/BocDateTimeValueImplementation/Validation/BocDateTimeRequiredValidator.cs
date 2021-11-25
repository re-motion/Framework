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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation
{
  public class BocDateTimeRequiredValidator : BocDateTimeValueValidatorBase
  {
    protected enum ValidationError
    {
      None,
      MissingDateOrTime,
      MissingDateAndTime,
      MissingDate,
      MissingTime,
    }

    private string? _missingDateOrTimeErrorMessage;
    private string? _missingDateAndTimeErrorMessage;
    private string? _missingDateErrorMessage;
    private string? _missingTimeErrorMessage;

    private ValidationError _error;

    protected override bool EvaluateIsValid ()
    {
      Control? control = NamingContainer.FindControl(ControlToValidate);

      BocDateTimeValue? dateTimeValueControl = control as BocDateTimeValue;

      if (dateTimeValueControl == null)
        throw new InvalidOperationException("BocDateTimeValueValidatorBase may only be applied to controls of type BocDateTimeValue");

      var validationErrorForRequired = EvaluateIsRequiredValid(dateTimeValueControl);
      if (validationErrorForRequired != ValidationError.None)
      {
        Error = validationErrorForRequired;
        return false;
      }

      var validationErrorForComplete = EvaluateIsCompleteValid(dateTimeValueControl);
      if (validationErrorForComplete != ValidationError.None)
      {
        Error = validationErrorForComplete;
        return false;
      }

      return true;
    }

    private ValidationError EvaluateIsRequiredValid (BocDateTimeValue control)
    {
      if (!control.IsRequired)
        return ValidationError.None;

      bool isDateOrTimeRequired = control.ActualValueType == BocDateTimeValueType.Undefined;

      bool isDateRequired = control.ActualValueType == BocDateTimeValueType.DateTime
                            || control.ActualValueType == BocDateTimeValueType.Date;

      bool isTimeRequired = control.ActualValueType == BocDateTimeValueType.DateTime;

      bool hasDate = !string.IsNullOrWhiteSpace(control.DateString);
      bool hasTime = !string.IsNullOrWhiteSpace(control.TimeString);

      var isDateMissing = isDateRequired && !hasDate;
      var isTimeMissing = isTimeRequired && !hasTime;

      if (isDateOrTimeRequired && !(hasDate || hasTime))
        return ValidationError.MissingDateOrTime;

      if (isDateMissing && isTimeMissing)
        return ValidationError.MissingDateAndTime;

      if (isDateMissing)
        return ValidationError.MissingDate;

      if (isTimeMissing)
        return ValidationError.MissingTime;

      return ValidationError.None;
    }

    private ValidationError EvaluateIsCompleteValid (BocDateTimeValue control)
    {
      bool isDateRequired = control.ActualValueType == BocDateTimeValueType.DateTime
                            || control.ActualValueType == BocDateTimeValueType.Date;

      if (!isDateRequired)
        return ValidationError.None;

      bool hasDate = !string.IsNullOrWhiteSpace(control.DateString);
      bool hasTime = !string.IsNullOrWhiteSpace(control.TimeString);

      if (hasTime && !hasDate)
        return ValidationError.MissingDate;

      return ValidationError.None;
    }

    protected override void RefreshBaseErrorMessage ()
    {
      switch (_error)
      {
        case ValidationError.MissingDateAndTime:
        {
          if (!string.IsNullOrEmpty(MissingDateAndTimeErrorMessage))
            ErrorMessage = MissingDateAndTimeErrorMessage;
          break;
        }
        case ValidationError.MissingDateOrTime:
        {
          if (!string.IsNullOrEmpty(MissingDateOrTimeErrorMessage))
            ErrorMessage = MissingDateOrTimeErrorMessage;
          break;
        }
        case ValidationError.MissingDate:
        {
          if (!string.IsNullOrEmpty(MissingDateErrorMessage))
            ErrorMessage = MissingDateErrorMessage;
          break;
        }
        case ValidationError.MissingTime:
        {
          if (!string.IsNullOrEmpty(MissingTimeErrorMessage))
            ErrorMessage = MissingTimeErrorMessage;
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the date value is Missing.
    /// </summary>
    public string? MissingDateErrorMessage
    {
      get { return _missingDateErrorMessage; }
      set
      {
        _missingDateErrorMessage = value;
        if (_error == ValidationError.MissingDate)
          ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the time value is Missing.
    /// </summary>
    public string? MissingTimeErrorMessage
    {
      get { return _missingTimeErrorMessage; }
      set
      {
        _missingTimeErrorMessage = value;
        if (_error == ValidationError.MissingTime)
          ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the date and time values are Missing.
    /// </summary>
    public string? MissingDateAndTimeErrorMessage
    {
      get { return _missingDateAndTimeErrorMessage; }
      set
      {
        _missingDateAndTimeErrorMessage = value;
        if (_error == ValidationError.MissingDateAndTime)
          ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the date or time values are Missing.
    /// </summary>
    public string? MissingDateOrTimeErrorMessage
    {
      get { return _missingDateOrTimeErrorMessage; }
      set
      {
        _missingDateOrTimeErrorMessage = value;
        if (_error == ValidationError.MissingDateOrTime)
          ErrorMessage = value;
      }
    }

    protected ValidationError Error
    {
      get { return _error; }
      set
      {
        _error = value;
        RefreshBaseErrorMessage();
      }
    }
  }
}