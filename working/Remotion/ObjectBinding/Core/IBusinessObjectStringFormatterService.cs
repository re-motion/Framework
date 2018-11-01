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
  /// Declares the API for retrieving the string-representation of an <see cref="IBusinessObjectProperty"/>'s value.
  /// </summary>
  public interface IBusinessObjectStringFormatterService : IBusinessObjectService
  {
    /// <summary> 
    ///   Gets the string representation of the value accessed through the specified <see cref="IBusinessObjectProperty"/> 
    ///   from the the passed <see cref="IBusinessObject"/>.
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose property value will be returned. </param>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="format">
    /// The format string passed to the value's <see cref="IFormattable.ToString(string,IFormatProvider)"/> method.
    /// </param>
    /// <returns> 
    ///   The string representation of the property value for the <paramref name="property"/> parameter. 
    /// </returns>
    string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format);
  }
}
