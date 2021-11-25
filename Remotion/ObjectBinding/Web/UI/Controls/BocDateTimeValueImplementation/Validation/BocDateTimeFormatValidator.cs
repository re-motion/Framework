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
using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation
{
  public class BocDateTimeFormatValidator : BocDateTimeValueValidatorBase
  {
    protected enum ValidationError
    {
      None,
      InvalidDateAndTime,
      InvalidDate,
      InvalidTime
    }


    private string? _invalidDateAndTimeErrorMessage;
    private string? _invalidDateErrorMessage;
    private string? _invalidTimeErrorMessage;

    private ValidationError _error;

    protected override bool EvaluateIsValid ()
    {
      Control? control = NamingContainer.FindControl(ControlToValidate);

      BocDateTimeValue? dateTimeValueControl = control as BocDateTimeValue;

      if (dateTimeValueControl == null)
        throw new InvalidOperationException("BocDateTimeValueValidatorBase may only be applied to controls of type BocDateTimeValue");

      bool isValidDate = EvaluateIsValidDate(dateTimeValueControl);
      bool isValidTime = EvaluateIsValidTime(dateTimeValueControl);

      if (!isValidDate && !isValidTime)
        Error = ValidationError.InvalidDateAndTime;
      else if (!isValidDate)
        Error = ValidationError.InvalidDate;
      else if (!isValidTime)
        Error = ValidationError.InvalidTime;

      return isValidDate && isValidTime;
    }

    /// <summary>
    ///   Validates date values in the current culture.
    /// </summary>
    /// <remarks>
    ///   Cannot detect a 0:0 time component included in the date string.
    ///   Since a time-offset of 0:0 will not falsify the result, this is acceptable.
    ///   Prevented by setting the MaxLength attribute of the input field.
    /// </remarks>
    protected virtual bool EvaluateIsValidDate (BocDateTimeValue control)
    {
      bool isValidDateRequired = control.ActualValueType == BocDateTimeValueType.Undefined
                                 || control.ActualValueType == BocDateTimeValueType.DateTime
                                 || control.ActualValueType == BocDateTimeValueType.Date;

      if (!isValidDateRequired)
        return true;

      string? dateValue = control.DateString;

      if (string.IsNullOrWhiteSpace(dateValue))
        return true;

      //  Is a valid date/time value? If not, FormatException will be thrown and caught
      DateTime dateTime;
      if (!DateTime.TryParse(dateValue, out dateTime))
        return false;

      //  Has a time component?
      if (dateTime.TimeOfDay != TimeSpan.Zero)
        return false;

      // In case of the entered value is "00:00" TryParse returns a valid date.
      // Parse the given value with NoCurrentDateDefault and None and compare the Date part.
      // Parse can be used because dateValue is already checked and is a parsable value.

      // Empty date defaults to 01.01.0001 
      DateTime dateTimeFirstDay = DateTime.Parse(dateValue, Thread.CurrentThread.CurrentCulture, DateTimeStyles.NoCurrentDateDefault);

      //  Empty date defaults to today
      DateTime dateTimeToday = DateTime.Parse(dateValue, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None);

      //  That's actually a time instead of a date
      if (dateTimeToday.Date != dateTimeFirstDay.Date)
        return false;

      return true;
    }

    /// <summary> Validates time values in the current culture. </summary>
    /// <remarks> Does not detect an included date of 01.01.0001. </remarks>
    protected virtual bool EvaluateIsValidTime (BocDateTimeValue control)
    {
      bool isValidTimeRequired = control.ActualValueType == BocDateTimeValueType.Undefined
                                 || control.ActualValueType == BocDateTimeValueType.DateTime;

      if (!isValidTimeRequired)
        return true;

      string? timeValue = control.TimeString;

      if (string.IsNullOrWhiteSpace(timeValue))
        return true;

      DateTime dateTime;
      if (!DateTime.TryParse(timeValue, Thread.CurrentThread.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out dateTime))
        return false;

      //  If only a time was entered, the date will default to 01.01.0001
      if (dateTime.Year != 1 || dateTime.Month != 1 || dateTime.Day != 1)
        return false;

      return true;
    }

    protected override void RefreshBaseErrorMessage ()
    {
      switch (_error)
      {
        case ValidationError.InvalidDateAndTime:
        {
          if (!string.IsNullOrEmpty(InvalidDateAndTimeErrorMessage))
            ErrorMessage = InvalidDateAndTimeErrorMessage;
          break;
        }
        case ValidationError.InvalidDate:
        {
          if (!string.IsNullOrEmpty(InvalidDateErrorMessage))
            ErrorMessage = InvalidDateErrorMessage;
          break;
        }
        case ValidationError.InvalidTime:
        {
          if (!string.IsNullOrEmpty(InvalidTimeErrorMessage))
            ErrorMessage = InvalidTimeErrorMessage;
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the date value is invalid.
    /// </summary>
    public string? InvalidDateErrorMessage
    {
      get { return _invalidDateErrorMessage; }
      set
      {
        _invalidDateErrorMessage = value;
        if (_error == ValidationError.InvalidDate)
          ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the time value is invalid.
    /// </summary>
    public string? InvalidTimeErrorMessage
    {
      get { return _invalidTimeErrorMessage; }
      set
      {
        _invalidTimeErrorMessage = value;
        if (_error == ValidationError.InvalidTime)
          ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="BaseValidator.ErrorMessage"/> if the date and time values are invalid.
    /// </summary>
    public string? InvalidDateAndTimeErrorMessage
    {
      get { return _invalidDateAndTimeErrorMessage; }
      set
      {
        _invalidDateAndTimeErrorMessage = value;
        if (_error == ValidationError.InvalidDateAndTime)
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