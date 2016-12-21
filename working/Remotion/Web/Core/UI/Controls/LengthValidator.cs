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
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{
public class LengthValidator : BaseValidator
{
  private int? _minimumLength = null;
  private int? _maximumLength = null;

  // static members and constants

  // member fields

  // construction and disposing

  public LengthValidator ()
  {
    EnableClientScript = false;
  }

  // methods and properties

  protected override bool EvaluateIsValid ()
  {
    string text = base.GetControlValidationValue (base.ControlToValidate);

    if (_minimumLength.HasValue && text.Length < _minimumLength.Value)
      return false;

    if (_maximumLength.HasValue && text.Length > _maximumLength.Value)
      return false;

    return true;
  }
  
  /// <summary> The minimum number of characters allowed. </summary>
  /// <value> 
  ///  The minimum length of the validated string, or <see langword="null"/> to disable the validation of the minimum length.
  /// </value>
  [Category ("Behavior")]
  [Description ("The maximum number of characters allowed in the validated property. Clear the MinimumLength property to not validate the minimum length.")]
  [DefaultValue (null)]
  public int? MinimumLength
  {
    get { return _minimumLength; }
    set
    {
      if (value.HasValue && value.Value < 0)
        throw new ArgumentOutOfRangeException ("value", value, "The MinimumLength must not be less than zero.");
      _minimumLength = value; 
    }
  }

  /// <summary> The maximum number of characters allowed. </summary>
  /// <value> 
  ///  The maximum length of the validated string, or <see langword="null"/> to disable the validation of the maximum length.
  /// </value>
  [Category ("Behavior")]
  [Description ("The maximum number of characters allowed in the validated property. Clear the MaximumLength proprety to not validate the maximum length.")]
  [DefaultValue (null)]
  public int? MaximumLength
  {
    get { return _maximumLength; }
    set
    {
      if (value.HasValue && value.Value < 0)
        throw new ArgumentOutOfRangeException ("value", value, "The MaximumLength must not be less than zero.");
      _maximumLength = value; 
    }
  }

}
}
