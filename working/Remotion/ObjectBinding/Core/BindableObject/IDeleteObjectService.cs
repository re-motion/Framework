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

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// <see cref="IDeleteObjectService"/> defines the interface required for deleting the <see cref="IBusinessObject"/> instance 
  /// that is assigned to the specified <see cref="IBusinessObjectReferenceProperty"/>.
  /// </summary>
  /// <remarks>
  /// You can register a service-instance with the <see cref="BusinessObjectProvider"/>'s <see cref="IBusinessObjectProvider.AddService"/> method 
  /// using either the <see cref="IDeleteObjectService"/> type or a derived type as the key to identify this service. If you register a service using 
  /// a derived type, you also have to apply the <see cref="DeleteObjectServiceTypeAttribute"/> to the bindable object for which the service is intended.
  /// </remarks>
  /// <seealso cref="DeleteObjectServiceTypeAttribute"/>
    [Obsolete ("The default value feature is not supported. (Version 1.13.142)")]
  public interface IDeleteObjectService : IBusinessObjectService
  {
    /// <summary>
    /// Gets a flag that describes whether the serivce can be used to delete the object assigned to this property.
    /// </summary>
    /// <param name="property">The <see cref="IBusinessObjectReferenceProperty"/> to be tested. Must not be <see langword="null" />.</param>
    /// <returns><see langword="true" /> if the delete object service is compatible with this <paramref name="property"/>.</returns>
    bool SupportsProperty ([NotNull] IBusinessObjectReferenceProperty property);

    /// <summary>
    /// Deletes the <paramref name="value"/> that is assigned to the <paramref name="referencingObject"/> via the <paramref name="property"/>.
    /// </summary>
    /// <param name="referencingObject">
    /// The <see cref="IBusinessObject"/> instance the <paramref name="value"/> is assigned to. Can be <see langword="null" />.
    /// </param>
    /// <param name="property">
    /// The <see cref="IBusinessObjectReferenceProperty"/> that holds the reference to the <paramref name="value"/>. Must not be <see langword="null" />.
    /// </param>
    /// <param name="value">The <see cref="IBusinessObject"/> instance to be deleted. Must not be <see langword="null" />..</param>
    void Delete ([CanBeNull] IBusinessObject referencingObject, [NotNull] IBusinessObjectReferenceProperty property, [NotNull] IBusinessObject value);
  }
}