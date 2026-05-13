// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

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
  /// Gets the sort position for the given <paramref name="classDefinition"/>.
  /// Multiple <see cref="ClassDefinition"/> can have the same sort position which means
  /// they are stored in the same table.
  /// </summary>
  int GetSortPosition (ClassDefinition classDefinition);

  /// <summary>
  /// Gets all <see cref="SortingOptimizationObjectIDPropertySpecification"/> for properties which are <see cref="ObjectID"/>s for the given <paramref name="classDefinition"/>
  /// </summary>
  IReadOnlyCollection<SortingOptimizationObjectIDPropertySpecification> GetPropertySpecificationsForForeignKeyProperties (ClassDefinition classDefinition);
}
