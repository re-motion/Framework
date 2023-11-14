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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation
{
  /// <summary>
  /// Declares the API for formatting the date and time value representation of a <see cref="BocDateTimeValue"/>.
  /// </summary>
  /// <seealso cref="DateTimeFormatter"/>
 public interface IDateTimeFormatter
  {
    /// <summary> Formats the <see cref="DateTime"/> value's date component according to the current culture. </summary>
    /// <param name="dateValue"> The <see cref="DateTime"/> value to be formatted. </param>
    /// <returns> A formatted string representing the <see cref="DateTime"/> value's date component. </returns>
    string FormatDateValue (DateTime dateValue);

    /// <summary> Formats the <see cref="DateTime"/> value's time component according to the current culture. </summary>
    /// <param name="timeValue"> The <see cref="DateTime"/> value to be formatted. </param>
    /// <param name="showSeconds"> <see langword="true"/> if the time format includes seconds. </param>
    /// <returns>  A formatted string representing the <see cref="DateTime"/> value's time component. </returns>
    string FormatTimeValue (DateTime timeValue, bool showSeconds);

    bool Is12HourTimeFormat ();
    int GetDateMaxLength ();
    int GetTimeMaxLength (bool showSeconds);
  }
}
