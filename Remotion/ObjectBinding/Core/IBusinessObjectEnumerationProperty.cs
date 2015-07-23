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
  ///   The <b>IBusinessObjectEnumerationProperty</b> interface is used for accessing the values of an enumeration. 
  /// </summary>
  /// <remarks> 
  ///   This property is not restrained to the enumerations derived from the <see cref="Enum"/> type. 
  ///   <note type="inotes">
  ///     The native value must be serializable if this property is to be bound to the 
  ///     <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue"/> control.
  ///   </note>
  /// </remarks>
  public interface IBusinessObjectEnumerationProperty: IBusinessObjectProperty
  {
    /// <summary> Returns a list of all the enumeration's values. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enabled enum values. </param>
    /// <returns>A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. </returns>
    IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject);

    /// <summary> Returns a list of the enumeration's values that can be used in the current context. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enabled enum values. </param>
    /// <returns>A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. </returns>
    /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
    IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject);

    /// <overloads> Returns a specific enumeration value. </overloads>
    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="value"> The enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> 
    /// The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="value"/> or <see langword="null"/> if the 
    /// <paramref name="value"/> represents <see langword="null"/>. 
    /// </returns>
    IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject);

    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="identifier">The string identifying the  enumeration value to return the <see cref="IEnumerationValueInfo"/> for.</param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> 
    /// The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="identifier"/> or <see langword="null"/> if the 
    /// <paramref name="identifier"/> represents <see langword="null"/>. 
    /// </returns>
    IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject);
  }
}
