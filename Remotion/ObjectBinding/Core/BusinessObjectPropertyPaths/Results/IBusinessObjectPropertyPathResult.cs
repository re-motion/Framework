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

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results
{
  /// <summary>
  /// Represents the result of an evaluated <see cref="IBusinessObjectPropertyPath"/>.
  /// </summary>
  public interface IBusinessObjectPropertyPathResult : INullObject
  {
    /// <summary>
    /// Gets the value of the property path.
    /// </summary>
    /// <returns>
    /// The <see cref="Object"/> returned for the <see cref="ResultProperty"/> of the <see cref="ResultObject"/>
    /// or <see langword="null" /> if there is no value.
    /// </returns>
    object GetValue ();

    /// <summary>
    /// Gets the string-representation of value of the property path.
    /// </summary>
    /// <returns>
    /// The <see cref="String"/>-representation of the value returned for the <see cref="ResultProperty"/> of the <see cref="ResultObject"/> 
    /// or an empty string if there is no value.
    /// </returns>
    string GetString (string format);

    /// <summary>
    /// Gets the last property of the property path, i.e. the property used to access the value.
    /// </summary>
    IBusinessObjectProperty ResultProperty { get; }
    
    /// <summary>
    /// Gets the <see cref="IBusinessObject"/> that holds the value of the property path.
    /// </summary>
    IBusinessObject ResultObject { get; }
  }
}