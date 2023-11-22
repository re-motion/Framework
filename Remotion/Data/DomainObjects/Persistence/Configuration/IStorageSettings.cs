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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Defines an API for configuring the storage settings.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  /// <seealso cref="StorageSettings"/>
  /// <seealso cref="DeferredStorageSettings"/>
  /// <threadsafety static="true" instance="true"/>
  public interface IStorageSettings
  {
    /// <summary>
    /// Gets the default <see cref="StorageProviderDefinition"/> or <see langword="null"/> if no default has been defined.
    /// </summary>
    StorageProviderDefinition? GetDefaultStorageProviderDefinition ();

    /// <summary>
    /// Gets all <see cref="StorageProviderDefinition"/> instances.
    /// </summary>
    IReadOnlyCollection<StorageProviderDefinition> GetStorageProviderDefinitions ();

    /// <summary>
    /// Gets the <see cref="StorageProviderDefinition"/> for the given <paramref name="classDefinition"/>.
    /// </summary>
    /// <param name="classDefinition">A <see cref="ClassDefinition"/>. Must not be <see langword="null"/>.</param>
    /// <returns>
    /// The <see cref="StorageProviderDefinition"/> associated via the <paramref name="classDefinition"/>'s storage group
    /// or the default <see cref="StorageProviderDefinition"/> if the <see cref="ClassDefinition"/> is not assigned to a storage group.
    /// </returns>
    /// <exception cref="ConfigurationException">Thrown if no <see cref="StorageProviderDefinition"/> could be resolved for the <paramref name="classDefinition"/>.</exception>
    StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition);

    /// <summary>
    /// Gets the <see cref="StorageProviderDefinition"/> for the given <paramref name="storageGroupTypeOrNull"/>.
    /// </summary>
    /// <param name="storageGroupTypeOrNull">The <see cref="StorageGroupAttribute"/>'s <see cref="Type"/> or <see langword="null"/>. </param>
    /// <returns>
    /// The <see cref="StorageProviderDefinition"/> associated with the specified storage group or the default <see cref="StorageProviderDefinition"/>
    /// if <paramref name="storageGroupTypeOrNull"/> is <see langword="null"/> or no  <see cref="StorageProviderDefinition"/> has been assigned to the storage group.
    /// </returns>
    /// <exception cref="ConfigurationException">Thrown if no <see cref="StorageProviderDefinition"/> could be resolved for the <paramref name="storageGroupTypeOrNull"/>.</exception>
    StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull);

    /// <summary>
    /// Gets the <see cref="StorageProviderDefinition"/> for the given <paramref name="storageProviderName"/>.
    /// </summary>
    /// <param name="storageProviderName">The name of the storage provider.</param>
    /// <returns>The <see cref="StorageProviderDefinition"/> with a <see cref="StorageProviderDefinition.Name"/> matching <paramref name="storageProviderName"/>.</returns>
    /// <exception cref="ConfigurationException">Thrown if no <see cref="StorageProviderDefinition"/> could be resolved for the <paramref name="storageProviderName"/>.</exception>
    StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName);
  }
}
