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
using System.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators
{
  /// <summary>
  /// Defines the enumerator used for retrieving the individual properties of an <see cref="IBusinessObjectPropertyPath"/>.
  /// </summary>
  /// <remarks>
  /// This API is meant to be used by <see cref="BusinessObjectPropertyPathBase"/> 
  /// and requires tighter integration with the callsite than the <see cref="IEnumerator"/> class provided by the the .NET Framework.
  /// </remarks>
  public interface IBusinessObjectPropertyPathPropertyEnumerator
  {
    /// <summary>
    /// Gets the <see cref="IBusinessObjectProperty"/> at the current position. 
    /// Returns <see langword="null" /> if the property could not be retrieved.
    /// </summary>
    IBusinessObjectProperty Current { get; }

    /// <summary>
    /// Returns <see langword="true" /> if the next invocation of <see cref="MoveNext"/> will return true. 
    /// Required for peeking into the property enumeration.
    /// </summary>
    bool HasNext { get; }

    /// <summary>
    /// Moves the <see cref="Current"/> <see cref="IBusinessObjectPropertyPath"/> to the next position in the property path.
    /// </summary>
    /// <param name="currentClass">
    /// The <see cref="IBusinessObjectClass"/> for which the property will be resolved. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the property path still contains properties for enumeration,
    /// regardless whether the next property can actually be resolved for the <paramref name="currentClass"/>.
    /// </returns>
    /// <exception cref="ParseException">May be thrown when the property cannot be resolved for the <paramref name="currentClass"/>.</exception>
    bool MoveNext (IBusinessObjectClass currentClass);
  }
}