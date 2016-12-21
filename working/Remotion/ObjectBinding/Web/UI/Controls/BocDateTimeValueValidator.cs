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
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> Validates a <see cref="BocDateTimeValue"/> control. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocDateTimeValueValidator.xml' path='BocDateTimeValueValidator/Class/*' />
  [ToolboxItem (false)]
  public class BocDateTimeValueValidator : BaseValidator
  {
    protected enum ValidationError
    {
      None,
      MissingDateOrTime,
      MissingDateAndTime,
      MissingDate,
      MissingTime,
      InvalidDateAndTime,
      InvalidDate,
      InvalidTime
    }

    private string _missingDateOrTimeErrorMessage;
    private string _missingDateAndTimeErrorMessage;
    private string _missingDateErrorMessage;
    private string _missingTimeErrorMessage;
    private string _invalidDateAndTimeErrorMessage;
    private string _invalidDateErrorMessage;
    private string _invalidTimeErrorMessage;

    private ValidationError _error;

    /// <summary> Checks the input fields for for valid contents. </summary>
    /// <returns> <see langword="true"/> if all fields are filled out according to the requirements. </returns>
    protected override bool EvaluateIsValid ()
    {
      Control control = NamingContainer.FindControl (ControlToValidate);

      BocDateTimeValue dateTimeValueControl = control as BocDateTimeValue;

      if (dateTimeValueControl == null)
        throw new InvalidOperationException ("BocDateTimeValueValidator may only be applied to controls of type BocDateTimeValue");

      var validationErrorForRequired = EvaluateIsRequiredValid (dateTimeValueControl);
      if (validationErrorForRequired != ValidationError.None)
      {
        Error = validationErrorForRequired;
        return false;
      }

      var validationErrorForComplete = EvaluateIsCompleteValid (dateTimeValueControl);
      if (validationErrorForComplete != ValidationError.None)
      {
        Error = validationErrorForComplete;
        return false;
      }

      bool isValidDate = EvaluateIsValidDate (dateTimeValueControl);
      bool isValidTime = EvaluateIsValidTime (dateTimeValueControl);

      if (! isValidDate && ! isValidTime)
        Error = ValidationError.InvalidDateAndTime;
      else if (! isValidDate)
        Error = ValidationError.InvalidDate;
      else if (! isValidTime)
        Error = ValidationError.InvalidTime;

      return isValidDate && isValidTime;
    }

    private ValidationError EvaluateIsRequiredValid (BocDateTimeValue control)
    {
      if (! control.IsRequired)
        return ValidationError.None;

      bool isDateOrTimeRequired = control.ActualValueType == BocDateTimeValueType.Undefined;

      bool isDateRequired = control.ActualValueType == BocDateTimeValueType.DateTime
                            || control.ActualValueType == BocDateTimeValueType.Date;

      bool isTimeRequired = control.ActualValueType == BocDateTimeValueType.DateTime;

      bool hasDate = !string.IsNullOrWhiteSpace (control.DateString);
      bool hasTime = !string.IsNullOrWhiteSpace (control.TimeString);

      var isDateMissing = isDateRequired && ! hasDate;
      var isTimeMissing = isTimeRequired && ! hasTime;

      if (isDateOrTimeRequired && ! (hasDate || hasTime))
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

      if (! isDateRequired)
        return ValidationError.None;

      bool hasDate = !string.IsNullOrWhiteSpace (control.DateString);
      bool hasTime = !string.IsNullOrWhiteSpace (control.TimeString);

      if (hasTime && !hasDate)
        return ValidationError.MissingDate;

      return ValidationError.None;
    }

    /// <summary>
    ///   Validates date values in the current culture.
    /// </summary>
    /// <remarks>
    ///   Cannot detect a 0:0 time component included in the date string.
    ///   Since a time-offset of 0:0 will not falsify the result, this is acceptable.
    ///   Prevented by setting the MaxLength attribute of the input field.
    /// </remarks>
    private bool EvaluateIsValidDate (BocDateTimeValue control)
    {
      bool isValidDateRequired = control.ActualValueType == BocDateTimeValueType.Undefined
                                 || control.ActualValueType == BocDateTimeValueType.DateTime
                                 || control.ActualValueType == BocDateTimeValueType.Date;

      if (! isValidDateRequired)
        return true;

      string dateValue = control.DateString;

      if (string.IsNullOrWhiteSpace (dateValue))
        return true;

      try
      {
        //  Is a valid date/time value? If not, FormatException will be thrown and caught
        DateTime dateTime = DateTime.Parse (dateValue);

        //  Has a time component?
        if (dateTime.TimeOfDay != TimeSpan.Zero)
          return false;
      }
      catch (FormatException)
      {
        return false;
      }
      catch (IndexOutOfRangeException)
      {
        return false;
      }

      try
      {
        //  If there is only a time value in the date field, 
        //  it will be detected if dateTimeToday and dateTimeFirstDay differ

        //  Empty date defaults to 01.01.0001
        DateTime dateTimeFirstDay = DateTime.Parse (
            dateValue,
            Thread.CurrentThread.CurrentCulture);

        //  Empty date defaults to today
        DateTime dateTimeToday = DateTime.Parse (
            dateValue,
            Thread.CurrentThread.CurrentCulture,
            DateTimeStyles.NoCurrentDateDefault);

        //  That's actually a time instead of a date
        if (dateTimeToday.Date != dateTimeFirstDay.Date)
          return false;
      }
      catch (FormatException)
      {
        //  This exception will most likely never happen. If it does, the value is still a valid date 
        //  or the execution would not have reached this point.
      }
      catch (IndexOutOfRangeException)
      {
        return false;
      }

      return true;
    }

    /// <summary> Validates time values in the current culture. </summary>
    /// <remarks> Does not detect an included date of 01.01.0001. </remarks>
    private bool EvaluateIsValidTime (BocDateTimeValue control)
    {
      bool isValidTimeRequired = control.ActualValueType == BocDateTimeValueType.Undefined
                                 || control.ActualValueType == BocDateTimeValueType.DateTime;

      if (! isValidTimeRequired)
        return true;

      string timeValue = control.TimeString;

      if (string.IsNullOrWhiteSpace (timeValue))
        return true;

      try
      {
        //  Is a valid time value? If not, FormatException will be thrown and caught
        DateTime dateTime = DateTime.Parse (
            timeValue,
            Thread.CurrentThread.CurrentCulture,
            DateTimeStyles.NoCurrentDateDefault);

        //  If only a time was entered, the date will default to 01.01.0001
        if (dateTime.Year != 1 || dateTime.Month != 1 || dateTime.Day != 1)
          return false;
      }
      catch (FormatException)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    ///   Helper function that determines whether the control specified by the 
    ///   <see cref="ControlToValidate"/> property is a valid control.
    /// </summary>
    /// <returns> 
    ///   <see langword="true"/> if the control specified by the <see cref="ControlToValidate"/>
    ///   property is a valid control; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="HttpException"> 
    ///   Thrown if the <see cref="ControlToValidate"/> is not of type <see cref="BocDateTimeValue"/>.
    /// </exception>
    protected override bool ControlPropertiesValid ()
    {
      if (! base.ControlPropertiesValid())
        return false;

      Control control = NamingContainer.FindControl (ControlToValidate);

      if (! (control is BocDateTimeValue))
        throw new HttpException ("Control '" + ControlToValidate + "' is not of type '" + typeof (BocDateTimeValue) + "'");

      return true;
    }

    protected virtual void RefreshBaseErrorMessage ()
    {
      switch (_error)
      {
        case ValidationError.MissingDateAndTime:
        {
          if (! string.IsNullOrEmpty (MissingDateAndTimeErrorMessage))
            base.ErrorMessage = MissingDateAndTimeErrorMessage;
          break;
        }
        case ValidationError.MissingDateOrTime:
        {
          if (! string.IsNullOrEmpty (MissingDateOrTimeErrorMessage))
            base.ErrorMessage = MissingDateOrTimeErrorMessage;
          break;
        }
        case ValidationError.MissingDate:
        {
          if (! string.IsNullOrEmpty (MissingDateErrorMessage))
            base.ErrorMessage = MissingDateErrorMessage;
          break;
        }
        case ValidationError.MissingTime:
        {
          if (! string.IsNullOrEmpty (MissingTimeErrorMessage))
            base.ErrorMessage = MissingTimeErrorMessage;
          break;
        }
        case ValidationError.InvalidDateAndTime:
        {
          if (! string.IsNullOrEmpty (InvalidDateAndTimeErrorMessage))
            base.ErrorMessage = InvalidDateAndTimeErrorMessage;
          break;
        }
        case ValidationError.InvalidDate:
        {
          if (! string.IsNullOrEmpty (InvalidDateErrorMessage))
            base.ErrorMessage = InvalidDateErrorMessage;
          break;
        }
        case ValidationError.InvalidTime:
        {
          if (! string.IsNullOrEmpty (InvalidTimeErrorMessage))
            base.ErrorMessage = InvalidTimeErrorMessage;
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Gets or sets the input control to validate. </summary>
    [TypeConverter (typeof (BocDateTimeValueControlToStringConverter))]
    public new string ControlToValidate
    {
      get { return base.ControlToValidate; }
      set { base.ControlToValidate = value; }
    }

    /// <summary> Gets or sets the text for the error message. </summary>
    /// <remarks> Will be set to one of the more specific error messages, if they are provided. </remarks>
    [Browsable (false)]
    public new string ErrorMessage
    {
      get
      {
        RefreshBaseErrorMessage();
        return base.ErrorMessage;
      }
      set { base.ErrorMessage = value; }
    }

    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the date value is invalid.
    /// </summary>
    public string InvalidDateErrorMessage
    {
      get { return _invalidDateErrorMessage; }
      set
      {
        _invalidDateErrorMessage = value;
        if (_error == ValidationError.InvalidDate)
          base.ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the time value is invalid.
    /// </summary>
    public string InvalidTimeErrorMessage
    {
      get { return _invalidTimeErrorMessage; }
      set
      {
        _invalidTimeErrorMessage = value;
        if (_error == ValidationError.InvalidTime)
          base.ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the date and time values are invalid.
    /// </summary>
    public string InvalidDateAndTimeErrorMessage
    {
      get { return _invalidDateAndTimeErrorMessage; }
      set
      {
        _invalidDateAndTimeErrorMessage = value;
        if (_error == ValidationError.InvalidDateAndTime)
          base.ErrorMessage = value;
      }
    }
    
    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the date value is Missing.
    /// </summary>
    public string MissingDateErrorMessage
    {
      get { return _missingDateErrorMessage; }
      set
      {
        _missingDateErrorMessage = value;
        if (_error == ValidationError.MissingDate)
          base.ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the time value is Missing.
    /// </summary>
    public string MissingTimeErrorMessage
    {
      get { return _missingTimeErrorMessage; }
      set
      {
        _missingTimeErrorMessage = value;
        if (_error == ValidationError.MissingTime)
          base.ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the date and time values are Missing.
    /// </summary>
    public string MissingDateAndTimeErrorMessage
    {
      get { return _missingDateAndTimeErrorMessage; }
      set
      {
        _missingDateAndTimeErrorMessage = value;
        if (_error == ValidationError.MissingDateAndTime)
          base.ErrorMessage = value;
      }
    }

    /// <summary>
    ///   Message returned by <see cref="ErrorMessage"/> if the date or time values are Missing.
    /// </summary>
    public string MissingDateOrTimeErrorMessage
    {
      get { return _missingDateOrTimeErrorMessage; }
      set
      {
        _missingDateOrTimeErrorMessage = value;
        if (_error == ValidationError.MissingDateOrTime)
          base.ErrorMessage = value;
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

  /// <summary>
  ///   Creates a VS.NET designer pick list for a property that references a 
  ///   <see cref="BocDateTimeValue"/> control.
  /// </summary>
  /// <remarks>
  ///   Use the <see cref="TypeConverter"/> attribute to assign this converter to a property.
  /// </remarks>
  public class BocDateTimeValueControlToStringConverter : ControlToStringConverter
  {
    public BocDateTimeValueControlToStringConverter ()
        : base (typeof (BocDateTimeValue))
    {
    }
  }
}
