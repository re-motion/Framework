// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
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
  private readonly IDomainModelConstraintProvider _domainConstraintProvider;

  public SortingOptimizationNodeFactory (IDomainModelConstraintProvider domainConstraintProvider)
  {
    ArgumentNullException.ThrowIfNull(domainConstraintProvider);

    _domainConstraintProvider = domainConstraintProvider;
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
    var columnDefinitionComparer = new ColumnDefinitionEqualityComparer();

    var entityNameDefinitionToNodeMapping = nodes.ToDictionary(k => k.TableDefinition.TableName, v => v);
    foreach (var node in nodes)
    {
      var classAndPropertyDefinitions = node.ClassDefinitions
          .SelectMany(c => c.GetPropertyDefinitions().Where(p => p.IsObjectID && p.StorageClass == StorageClass.Persistent)
              .Select(p => (
                      ClassDefinition: c,
                      PropertyDefinition: p,
                      Columns: ((IObjectIDStoragePropertyDefinition)p.StoragePropertyDefinition).GetColumnsForComparison().ToArray()
                  )
              )).ToList();

      foreach (var foreignKey in node.TableDefinition.Constraints.OfType<ForeignKeyConstraintDefinition>())
      {
        if (!entityNameDefinitionToNodeMapping.TryGetValue(foreignKey.ReferencedTableName, out var pointingTo))
        {
          throw new InvalidOperationException($"Could not find {nameof(SortingOptimizationNode)} '{foreignKey.ReferencedTableName.EntityName}' "
                                              + $"for foreign key '{foreignKey.ConstraintName}' on node '{node.TableName}'.");
        }

        node.AddEdge(foreignKey, pointingTo);

        var matchingPropertyDefinition = classAndPropertyDefinitions.Where(cpc => cpc.Columns.SequenceEqual(foreignKey.ReferencingColumns, columnDefinitionComparer)).ToArray();
        foreach (var match in matchingPropertyDefinition)
        {
          classAndPropertyDefinitions.Remove(match);
          node.AddForeignKeyPropertyDefinition(
              match.ClassDefinition,
              match.PropertyDefinition,
              true,
              _domainConstraintProvider.GetForeignKeyCycleBreakHint(match.PropertyDefinition.PropertyInfo));
        }
      }

      // all remaining do not have a foreign key constraint.
      foreach (var entry in classAndPropertyDefinitions)
        node.AddForeignKeyPropertyDefinition(entry.ClassDefinition, entry.PropertyDefinition, false, _domainConstraintProvider.GetForeignKeyCycleBreakHint(entry.PropertyDefinition.PropertyInfo));
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
