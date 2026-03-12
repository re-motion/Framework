// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;

namespace Remotion.Data.DomainObjects.Persistence.SortingOptimization;

/// <summary>
/// <see cref="IPersistenceModelSortingProvider"/> provides functionality for ordering tables
/// for creating commands in the correct order.
/// </summary>
public interface IPersistenceModelSortingProvider
{
  /// <summary>
  /// Initializes the <see cref="IPersistenceModelSortingProvider"/> and performs all necessary calculations.
  /// </summary>
  void Initialize (IReadOnlyList<ClassDefinition> classDefinitions);

  /// <summary>
  /// Gets the sort position for the given <paramref name="storageEntityDefinition"/>.
  /// </summary>
  int GetSortPosition (IStorageEntityDefinition storageEntityDefinition);

  /// <summary>
  /// Gets if <paramref name="storageEntityDefinition"/> has been sorted correctly or not.
  /// </summary>
  bool HasBeenSortedCorrectly (IStorageEntityDefinition storageEntityDefinition);

  /// <summary>
  /// Gets all foreign key relevant <see cref="PropertyDefinition"/>s for the given <paramref name="classDefinition"/>
  /// </summary>
  IReadOnlyList<PropertyDefinition> GetForeignKeyRelevantPropertyDefinitions (ClassDefinition classDefinition);
}
