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
  ///   The <see cref="IBusinessObject"/> interface provides functionality to get and set the state of a business object.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     An <see cref="IBusinessObject"/> knows its <see cref="IBusinessObjectClass"/> through the 
  ///     <see cref="BusinessObjectClass"/> property.
  ///   </para><para>
  ///     Its state can be accessed through a number of get and set methods as well as indexers.
  ///   </para>
  ///   <note type="inotes">
  ///     Unless you must extend an existing class with the business object functionality, you can use the 
  ///     <see cref="T:Remotion.ObjectBinding.BusinessObject"/> class as a base implementation.
  ///   </note>
  /// </remarks>
  public interface IBusinessObject
  {
    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
    /// <returns> The property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="BusinessObjectPropertyAccessException">
    ///   Thrown if the <paramref name="property"/>'s value could not be read.
    /// </exception>
    /// <exception cref="Exception">
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    object GetProperty (IBusinessObjectProperty property);

    /// <overloads> Sets the value accessed through the specified property. </overloads>
    /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="value"> The new value for the <paramref name="property"/> parameter. </param>
    /// <exception cref="BusinessObjectPropertyAccessException">
    ///   Thrown if the <paramref name="property"/>'s value could not be written.
    /// </exception>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    void SetProperty (IBusinessObjectProperty property, object value);

    /// <summary> 
    ///   Gets the formatted string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="format"> The format string applied by the <b>ToString</b> method. </param>
    /// <returns> The string representation of the property value for the <paramref name="property"/> parameter.  </returns>
    /// <exception cref="BusinessObjectPropertyAccessException">
    ///   Thrown if the <paramref name="property"/>'s value could not be read.
    /// </exception>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    string GetPropertyString (IBusinessObjectProperty property, string format);

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="IBusinessObjectClass"/> instance acting as the business object's type. </value>
    IBusinessObjectClass BusinessObjectClass { get; }
  }
}
