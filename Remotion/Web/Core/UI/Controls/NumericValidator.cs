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
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary> Summary validates the text for a valid numeric value. </summary>
  /// <remarks> Validation is performed using the respective data type's <b>Parse</b> method. </remarks>
  public class NumericValidator : BaseValidator
  {
    private delegate bool TryParseWithNumberStyleDelegate<T> (string s, NumberStyles numberStyle, IFormatProvider? formatProvider, out T result)
        where T: struct;

    private delegate bool TryParseDelegate<T> (string s, out T result) where T: struct;


    private NumericValidationDataType _dataType = NumericValidationDataType.Int32;
    private bool _allowNegative = true;
    private NumberStyles _numberStyle = NumberStyles.None;

    public NumericValidator ()
    {
      EnableClientScript = false;
    }

    protected override bool EvaluateIsValid ()
    {
      string text = GetControlValidationValue(ControlToValidate);

      if (string.IsNullOrWhiteSpace(text))
        return true;

      switch (_dataType)
      {
        case NumericValidationDataType.Byte:
          return EvaluateIsValid<Byte>(text, _numberStyle, Byte.TryParse, Byte.TryParse);
        case NumericValidationDataType.Decimal:
          return EvaluateIsValid<Decimal>(text, _numberStyle, Decimal.TryParse, Decimal.TryParse);
        case NumericValidationDataType.Double:
          return EvaluateIsValid<Double>(text, _numberStyle, Double.TryParse, Double.TryParse);
        case NumericValidationDataType.Int16:
          return EvaluateIsValid<Int16>(text, _numberStyle, Int16.TryParse, Int16.TryParse);
        case NumericValidationDataType.Int32:
          return EvaluateIsValid<Int32>(text, _numberStyle, Int32.TryParse, Int32.TryParse);
        case NumericValidationDataType.Int64:
          return EvaluateIsValid<Int64>(text, _numberStyle, Int64.TryParse, Int64.TryParse);
        case NumericValidationDataType.Single:
          return EvaluateIsValid<Single>(text, _numberStyle, Single.TryParse, Single.TryParse);
        default:
          throw new InvalidOperationException(string.Format("The value '{0}' of the 'DataType' property is not a valid value.", _dataType));
      }
    }

    private bool EvaluateIsValid<T> (
        string text, NumberStyles numberStyle, TryParseDelegate<T> tryParse, TryParseWithNumberStyleDelegate<T> tryParseWithNumberStyle)
        where T: struct, IComparable<T>
    {
      T parsedValue;
      if (_numberStyle != NumberStyles.None)
      {
        try
        {
          if (!tryParseWithNumberStyle(text, numberStyle, null, out parsedValue))
            return false;
        }
        catch (ArgumentException e)
        {
          throw new InvalidOperationException("The combination of the flags in the 'NumberStyle' property is invalid.", e);
        }
      }
      else if (!tryParse(text, out parsedValue))
      {
        return false;
      }

      if (!_allowNegative && parsedValue.CompareTo(default(T)) < 0)
        return false;

      return true;
    }

    /// <summary> Gets or sets the data type to be tested. </summary>
    /// <value> A value of the <see cref="NumericValidationDataType"/> enumeration. Defaults to <see cref="NumericValidationDataType.Int32"/>. </value>
    [Category("Behavior")]
    [Description("The data type to be tested.")]
    [DefaultValue(NumericValidationDataType.Int32)]
    public NumericValidationDataType DataType
    {
      get { return _dataType; }
      set { _dataType = ArgumentUtility.CheckValidEnumValueAndTypeAndNotNull<NumericValidationDataType>("value", value); }
    }

    /// <summary> Gets or sets a value that determines whether negative values are allowed. </summary>
    /// <value>
    /// <see langword="true"/> to allow negative values. Defaults to <see lagnword="true"/>.
    /// </value>
    [Category("Behavior")]
    [Description("A value that determines whether negative values are allowed.")]
    [DefaultValue(true)]
    public bool AllowNegative
    {
      get { return _allowNegative; }
      set { _allowNegative = value; }
    }

    /// <summary> Gets or sets the allowed <see cref="NumberStyles"/> of the value to be tested. </summary>
    /// <value> A combination of the values of the <see cref="NumberStyles"/> enumeration. Defaults to <b>None</b>. </value>
    /// <remarks> 
    ///   The number style is used by the <b>Parse</b> method. If it is set to <b>None</b>, the <b>Parse</b> method is 
    ///   called without the <b>NumberStyles</b> argument.
    /// </remarks>
    [Category("Behavior")]
    [Description("The allowed NumberStyles of the value to be tested.")]
    [DefaultValue(NumberStyles.None)]
    public NumberStyles NumberStyle
    {
      get { return _numberStyle; }
      set { _numberStyle = value; }
    }
  }

  public enum NumericValidationDataType
  {
    Byte,
    Decimal,
    Double,
    Int16,
    Int32,
    Int64,
    Single
  }
}
