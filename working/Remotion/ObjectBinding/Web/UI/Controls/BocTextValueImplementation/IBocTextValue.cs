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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation
{
  public interface IBocTextValue : IBocTextValueBase
  {
    /// <summary> Gets or sets the current value. </summary>
    /// <value> 
    ///   <para>
    ///     The value has the type specified in the <see cref="ValueType"/> property (<see cref="String"/>, 
    ///     <see cref="Int32"/>, <see cref="Double"/> or <see cref="DateTime"/>). If <see cref="ValueType"/> is not
    ///     set, the type is determined by the bound <see cref="BusinessObjectBoundWebControl.Property"/>.
    ///   </para><para>
    ///     Returns <see langword="null"/> if <see cref="IBocTextValueBase.Text"/> is an empty <see cref="String"/>.
    ///   </para>
    /// </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    /// <exception cref="FormatException"> 
    ///   The value of the <see cref="IBocTextValueBase.Text"/> property cannot be converted to the specified <see cref="ValueType"/>.
    /// </exception>
    [Description ("Gets or sets the current value.")]
    [Browsable (false)]
    new object Value { get; set; }

    /// <summary>
    ///   Gets a flag describing whether it is save (i.e. accessing <see cref="Value"/> does not throw a 
    ///   <see cref="FormatException"/> or <see cref="OverflowException"/>) to read the contents of <see cref="Value"/>.
    /// </summary>
    /// <remarks> Valid values include <see langword="null"/>. </remarks>
    [Browsable (false)]
    bool IsValidValue { get; }

    /// <summary> Gets or sets the <see cref="BocTextValueType"/> assigned from an external source. </summary>
    /// <value> 
    ///   The externally set <see cref="BocTextValueType"/>. The default value is 
    ///   <see cref="BocTextValueType.Undefined"/>. 
    /// </value>
    [Description ("Gets or sets a fixed value type.")]
    [Category ("Data")]
    [DefaultValue (BocTextValueType.Undefined)]
    BocTextValueType ValueType { get; set; }

    /// <summary>
    ///   Gets the controls fixed <see cref="ValueType"/> or, if <see cref="BocTextValueType.Undefined"/>, 
    ///   the <see cref="BusinessObjectBoundWebControl.Property"/>'s value type.
    /// </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    BocTextValueType ActualValueType { get; }

    /// <summary> Gets or sets the format string used to create the string value.  </summary>
    /// <value> 
    ///   A string passed to the <b>ToString</b> method of the object returned by <see cref="Value"/>.
    ///   The default value is an empty <see cref="String"/>. 
    /// </value>
    /// <remarks>
    ///   <see cref="IFormattable"/> is used to format the value using this string. The default is "d" for date-only
    ///   values and "g" for date/time values (use "G" to display seconds too). 
    /// </remarks>
    [Description (
        "Gets or sets the format string used to create the string value. Format must be parsable by the value's type if the control is in edit mode.")
    ]
    [Category ("Style")]
    [DefaultValue ("")]
    string Format { get; set; }
  }
}