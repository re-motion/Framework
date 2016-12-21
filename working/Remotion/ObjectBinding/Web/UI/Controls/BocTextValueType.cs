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

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A list possible data types for the <see cref="BocTextValue"/> </summary>
  public enum BocTextValueType
  {
    /// <summary> 
    ///   Format the value as its default string representation. 
    ///   No parsing is possible, <see cref="BocTextValue.Value"/> will return a string. 
    /// </summary>
    Undefined,
    /// <summary> Interpret the value as a <see cref="String"/>. </summary>
    String,
    /// <summary> Interpret the value as an <see cref="Byte"/>. </summary>
    Byte,
    /// <summary> Interpret the value as a <see cref="Int16"/>. </summary>
    Int16,
    /// <summary> Interpret the value as a <see cref="Int32"/>. </summary>
    Int32,
    /// <summary> Interpret the value as a <see cref="Int64"/>. </summary>
    Int64,
    /// <summary> Interpret the value as a <see cref="DateTime"/> with the time component set to zero. </summary>
    Date,
    /// <summary> Interpret the value as a <see cref="DateTime"/>. </summary>
    DateTime,
    /// <summary> Interpret the value as a <see cref="Decimal"/>. </summary>
    Decimal,
    /// <summary> Interpret the value as a <see cref="Double"/>. </summary>
    Double,
    /// <summary> Interpret the value as a <see cref="Single"/>. </summary>
    Single,
  }
}
