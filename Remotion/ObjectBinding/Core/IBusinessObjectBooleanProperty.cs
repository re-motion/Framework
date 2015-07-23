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
  /// <summary> The <b>IBusinessObjectBooleanProperty</b> interface is used for accessing <see cref="bool"/> values. </summary>
  public interface IBusinessObjectBooleanProperty: IBusinessObjectProperty
  {
    /// <summary> Returns the human readable value of the boolean property. </summary>
    /// <param name="value"> The <see cref="bool"/> value to be formatted. </param>
    /// <returns> The human readable string value of the boolean property. </returns>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    string GetDisplayName (bool value);

    /// <summary> Returns the default value to be assumed if the boolean property returns <see langword="null"/>. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> for which to get the property's default value. </param>
    /// <remarks> 
    ///   If <see langword="null"/> is returned, the object model does not define a default value. In case the 
    ///   caller requires a default value, the selection of the appropriate value is left to the caller.
    /// </remarks>
    bool? GetDefaultValue (IBusinessObjectClass objectClass);
  }
}
