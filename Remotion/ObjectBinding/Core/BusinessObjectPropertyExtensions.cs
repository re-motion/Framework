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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Extensions for the <see cref="IBusinessObjectProperty"/> interface.
  /// </summary>
  public static class BusinessObjectPropertyExtensions
  {
    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used for the evaluation. Must not be <see langword="null" />.</param>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> of the <paramref name="obj"/>. Must not be <see langword="null" />.</param>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can access this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
    [Obsolete ("Use IBusinessObjectProperty.IsAccessible (IBusinessObject) instead. (Version 1.15.20.0)")]
    public static bool IsAccessible (this IBusinessObjectProperty property, [CanBeNull] IBusinessObjectClass objectClass, [CanBeNull] IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return property.IsAccessible (obj);
    }
  }
}