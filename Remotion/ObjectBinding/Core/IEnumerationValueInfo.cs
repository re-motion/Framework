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

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   The <b>IEnumerationValueInfo"</b> interface provides fucntionality for encapsulating a native enumeration value 
  ///   for use with an <see cref="IBusinessObjectEnumerationProperty"/>.
  /// </summary>
  /// <remarks> 
  ///   For enumerations of the <see cref="Enum"/> type, the generic <c>EnumerationValueInfo</c> class in Remotion.ObjectBinding.dll can be 
  ///   used.
  ///  </remarks>
  public interface IEnumerationValueInfo
  {
    /// <summary> Gets the object representing the original value, e.g. a System.Enum type. </summary>
    /// <value> The encapsulated enumeration value. </value>
    object Value { get; }

    /// <summary> Gets the string identifier representing the value. </summary>
    /// <value> The encapsulated enumeration value's string representation. </value>
    string Identifier { get; }

    /// <summary> Gets the string presented to the user. </summary>
    /// <value> The human readable value of the encapsulated enumeration value. </value>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    string DisplayName { get; }

    /// <summary>
    ///   Gets a flag indicating whether this value should be presented as an option to the user. 
    ///   (If not, existing objects might still use this value.)
    /// </summary>
    /// <value> <see langword="true"/> if this enumeration value sould be presented as an option to the user. </value>
    bool IsEnabled { get; }
  }
}
