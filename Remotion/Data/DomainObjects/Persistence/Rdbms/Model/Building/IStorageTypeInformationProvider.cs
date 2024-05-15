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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// <see cref="IStorageTypeInformationProvider"/> is the base class for type-calculator implementations which determine the storage-specific type for a 
  /// storable column definition.
  /// </summary>
  public interface IStorageTypeInformationProvider
  {
    [NotNull]
    IStorageTypeInformation GetStorageTypeForID (bool isStorageTypeNullable);

    [NotNull]
    IStorageTypeInformation GetStorageTypeForSerializedObjectID (bool isStorageTypeNullable);

    [NotNull]
    IStorageTypeInformation GetStorageTypeForClassID (bool isStorageTypeNullable);

    [NotNull]
    IStorageTypeInformation GetStorageTypeForTimestamp (bool isStorageTypeNullable);

    /// <summary>
    /// Gets an <see cref="IStorageTypeInformation"/> for the given <paramref name="propertyDefinition"/>.
    /// </summary>
    /// <param name="propertyDefinition">The <see cref="PropertyDefinition"/> for which an <see cref="IStorageTypeInformation"/> object should be
    ///   returned.</param>
    /// <param name="forceNullable">Specifies whether to override the <see cref="PropertyDefinitionBase.IsNullable"/> property to make the property
    /// nullable in the database even when the property is not nullable in memory.</param>
    /// <returns>A <see cref="IStorageTypeInformation"/> for the given <paramref name="propertyDefinition"/>.</returns>
    [NotNull]
    IStorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition, bool forceNullable);

    /// <summary>
    /// Gets an <see cref="IStorageTypeInformation"/> for situations where a .NET <see cref="Type"/> is known that should be mapped to an
    /// Rdbms type.
    /// </summary>
    /// <param name="type">The type for which an <see cref="IStorageTypeInformation"/> object should be returned.</param>
    /// <returns>A <see cref="IStorageTypeInformation"/> for the given <paramref name="type"/>.</returns>
    [NotNull]
    IStorageTypeInformation GetStorageType (Type type);

    /// <summary>
    /// Gets an <see cref="IStorageTypeInformation"/> for situations where no other information is available but a <paramref name="value"/>.
    /// This is similar to <see cref="GetStorageType(System.Type)"/>, but it also works with <see langword="null" /> values.
    /// </summary>
    /// <param name="value">The value for which an <see cref="IStorageTypeInformation"/> object should be returned. Can be <see langword="null" />.</param>
    /// <returns>A best-effort <see cref="IStorageTypeInformation"/> for the given <paramref name="value"/>.</returns>
    /// <remarks>
    /// For  <see langword="null" /> values, a default <see cref="IStorageTypeInformation"/> is returned that is not guaranteed to be compatible with 
    /// all possible data types, although it tries to be as compatible as possible without knowing the context in which the value is to be used.
    /// </remarks>
    [NotNull]
    IStorageTypeInformation GetStorageType (object? value);
  }
}
