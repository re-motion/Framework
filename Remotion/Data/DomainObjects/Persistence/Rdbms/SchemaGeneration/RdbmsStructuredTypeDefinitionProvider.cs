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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Provides all <see cref="IRdbmsStructuredTypeDefinition"/> for which to generate scripts.
  /// </summary>
  public class RdbmsStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
  {
    public RdbmsStructuredTypeDefinitionProvider ()
    {
    }

    /// <summary>
    /// Gets all <see cref="IRdbmsStructuredTypeDefinition"/>s for which to generate CREATE TYPE and DROP TYPE scripts.
    /// </summary>
    /// <param name="storageProviderDefinition">The storage provider for which to generate scripts.</param>
    /// <param name="classDefinitions">The class definition that should be considered for script generation.</param>
    /// <remarks>
    /// In order to influence the returned collection, override or mix the <see cref="IRdbmsStorageObjectFactory.CreateSingleScalarStructuredTypeDefinitionProvider"/> method
    /// on the <paramref name="storageProviderDefinition"/>'s <see cref="RdbmsProviderDefinition.Factory"/>. 
    /// </remarks>
    public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (RdbmsProviderDefinition storageProviderDefinition, IEnumerable<ClassDefinition> classDefinitions)
    {
      ArgumentNullException.ThrowIfNull(storageProviderDefinition);
      ArgumentNullException.ThrowIfNull(classDefinitions);

      var factory = storageProviderDefinition.Factory;
      var simpleStructuredTypeDefinitionRepository = factory.CreateSingleScalarStructuredTypeDefinitionProvider(storageProviderDefinition);
      var simpleScalarStructuredTypes = simpleStructuredTypeDefinitionRepository.GetAllStructuredTypeDefinitions();

      var rdbmsPersistenceModelProvider = factory.CreateRdbmsPersistenceModelProvider(storageProviderDefinition);
      var tableManipulationRecordDefinitionProvider = factory.CreateTableManipulationRecordDefinitionProvider(storageProviderDefinition);
      var tableManipulationStructuredTypes = CollectStructuredTypeDefinitions(
              tableManipulationRecordDefinitionProvider,
              rdbmsPersistenceModelProvider,
              classDefinitions)
          .Distinct();

      return simpleScalarStructuredTypes
          .Concat(tableManipulationStructuredTypes)
          .ToArray();
    }

    private static IEnumerable<IRdbmsStructuredTypeDefinition> CollectStructuredTypeDefinitions (
        ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider,
        IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider,
        IEnumerable<ClassDefinition> classDefinitions)
    {
      foreach (var classDefinition in classDefinitions)
      {
        var rdbmsStorageEntityDefinition = rdbmsPersistenceModelProvider.GetEntityDefinition(classDefinition);
        var tableDefinition = InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition?>(
            rdbmsStorageEntityDefinition,
            (table, continuation) => table,
            (filterView, continuation) => continuation(filterView.BaseEntity),
            (unionView, continuation) => null,
            (emptyView, continuation) => null);
        if (tableDefinition == null)
          continue;

        yield return tableManipulationRecordDefinitionProvider.GetDeleteRecordDefinition(classDefinition).StructuredTypeDefinition;
        yield return tableManipulationRecordDefinitionProvider.GetLockRecordDefinition(classDefinition).StructuredTypeDefinition;
        yield return tableManipulationRecordDefinitionProvider.GetInsertRecordDefinition(classDefinition).StructuredTypeDefinition;
        yield return tableManipulationRecordDefinitionProvider.GetUpdateRecordDefinition(classDefinition).StructuredTypeDefinition;
      }
    }
  }
}
