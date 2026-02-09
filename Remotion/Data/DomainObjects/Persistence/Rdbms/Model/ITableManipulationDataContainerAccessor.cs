// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

/// <summary>
/// Provides methods to access data in a <see cref="DataContainer"/> as an indirection.
/// This allows changing the data retrieved without changing the actual data stored in the DataContainer.
/// </summary>
/// <seealso cref="TableManipulationRecordDefinitionProvider"/>
public interface ITableManipulationDataContainerAccessor
{
  /// <summary>
  /// Returns the ID for the underlying <see cref="DataContainer"/>.
  /// </summary>
  ObjectID GetID ();

  /// <summary>
  /// Returns the timestamp for the underlying <see cref="DataContainer"/>.
  /// </summary>
  object GetTimestamp ();

  /// <summary>
  /// Returns the effective value for the specified <paramref name="propertyDefinition"/>.
  /// </summary>
  object? GetValue (PropertyDefinition propertyDefinition);

  /// <summary>
  /// Returns the effective value for the specified optional <paramref name="propertyDefinition"/>.
  /// Allows the actual value to be replaced with a dummy value if there was no change.
  /// </summary>
  /// <remarks>
  /// Returning a dummy value might be preferable if the value has not changed as it reduces the amount
  /// of data that needs to be transferred to the SQL Server.
  /// </remarks>
  object? GetOptionalValue (PropertyDefinition propertyDefinition);

  /// <summary>
  /// Returns a boolean indicating if the specified optional <paramref name="propertyDefinition"/> is set.
  /// If this returns <see langword="true"/>, <see cref="GetOptionalValue"/> will return the actual value, otherwise it might return a dummy value.
  /// </summary>
  bool IsOptionalValueSet (PropertyDefinition propertyDefinition);
}
