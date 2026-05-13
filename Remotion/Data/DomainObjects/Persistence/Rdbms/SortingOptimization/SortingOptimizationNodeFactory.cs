// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

/// <summary>
/// <see cref="SortingOptimizationNodeFactory"/> creates <see cref="SortingOptimizationNode"/>s based on <see cref="ClassDefinition"/>s
/// of persistent and non-abstract types.
/// </summary>
/// <threadsafety static="true" instance="true" />
[ImplementationFor(typeof(ISortingOptimizationNodeFactory), RegistrationType = RegistrationType.Single, Lifetime = LifetimeKind.Singleton)]
public class SortingOptimizationNodeFactory : ISortingOptimizationNodeFactory
{
  public SortingOptimizationNodeFactory ()
  {

  }

  /// <inheritdoc />
  public IReadOnlyList<SortingOptimizationNode> CreateNodes (IReadOnlyList<ClassDefinition> classDefinitions)
  {
    ArgumentNullException.ThrowIfNull(classDefinitions);

    var persistentClassDefinitions = classDefinitions.Where(d => !d.IsAbstract).ToArray();
    var nodes = persistentClassDefinitions.GroupBy(GetTableDefinition).Where(g => g.Key != null).Select(g => new SortingOptimizationNode(g.Key!, g.ToList())).ToList();
    AddEdgesToNodes(nodes);

    return nodes;
  }

  private void AddEdgesToNodes (List<SortingOptimizationNode> nodes)
  {
    var entityNameDefinitionToNodeMapping = nodes.ToDictionary(k => k.TableDefinition.TableName, v => v);
    foreach (var node in nodes)
    {
      foreach (var foreignKey in node.TableDefinition.Constraints.OfType<ForeignKeyConstraintDefinition>())
      {
        if (!entityNameDefinitionToNodeMapping.TryGetValue(foreignKey.ReferencedTableName, out var pointingTo))
          throw new InvalidOperationException($"Could not find {nameof(SortingOptimizationNode)} '{foreignKey.ReferencedTableName.EntityName}' for foreign key '{foreignKey.ConstraintName}' on node '{node.TableName}'.");

        node.AddEdge(foreignKey, pointingTo);
      }
    }
  }

  private TableDefinition? GetTableDefinition (ClassDefinition classDefinition)
  {
    if (classDefinition.StorageEntityDefinition is not IRdbmsStorageEntityDefinition storageEntityDefinition)
      return null;

    return InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition?>(
        storageEntityDefinition,
        (table, _) => table,
        (filterView, continuation) => continuation(filterView.BaseEntity),
        (unionView, _) => null,
        (emptyView, _) => null);

  }
}
