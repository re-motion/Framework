// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.SortingOptimization;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

/// <summary>
/// <see cref="GraphBasedPersistenceModelSortingProvider"/> is an implementation of <see cref="IPersistenceModelSortingProvider"/>
/// which determines the sort order depending on the foreign keys of the <see cref="TableDefinition"/>s.
/// </summary>
public class GraphBasedPersistenceModelSortingProvider : IPersistenceModelSortingProvider
{
  private readonly ISortingOptimizationNodeFactory _nodeFactory;

  private IDictionary<ClassDefinition, List<PropertyDefinition>> _foreignKeyRelevantPropertyDefinitions = null!;
  private IDictionary<IStorageEntityDefinition, bool> _tableDefinitionToHasBeenOrderedCorrectly = null!;
  private IDictionary<IStorageEntityDefinition, int> _tableSortOrder = null!;
  private bool _hasBeenInitialized;

  public GraphBasedPersistenceModelSortingProvider (ISortingOptimizationNodeFactory nodeFactory)
  {
    ArgumentNullException.ThrowIfNull(nodeFactory);
    _nodeFactory = nodeFactory;
  }

  public void Initialize (IReadOnlyList<ClassDefinition> classDefinitions)
  {
    ArgumentNullException.ThrowIfNull(classDefinitions);

    var nodes = _nodeFactory.CreateNodes(classDefinitions);

    _foreignKeyRelevantPropertyDefinitions = CreateForeignKeyRelevantPropertyDefinitions(nodes);

    ApplyEdgeBreaks(nodes);

    _tableSortOrder = CreateTableSortOrder(nodes);
    _tableDefinitionToHasBeenOrderedCorrectly = nodes.ToDictionary(k => (IStorageEntityDefinition)k.TableDefinition, v => !v.HasBrokenEdges);
    _hasBeenInitialized = true;
  }

  /// <inheritdoc/>
  public int GetSortPosition (IStorageEntityDefinition storageEntityDefinition)
  {
    ArgumentNullException.ThrowIfNull(storageEntityDefinition);

    if (!_hasBeenInitialized)
      throw new InvalidOperationException($"Before accessing {nameof(GetSortPosition)}, {nameof(Initialize)} has to be called.");

    return _tableSortOrder[storageEntityDefinition];
  }

  /// <inheritdoc/>
  public bool HasBeenSortedCorrectly (IStorageEntityDefinition storageEntityDefinition)
  {
    ArgumentNullException.ThrowIfNull(storageEntityDefinition);

    if (!_hasBeenInitialized)
      throw new InvalidOperationException($"Before accessing {nameof(HasBeenSortedCorrectly)}, {nameof(Initialize)} has to be called.");

    return _tableDefinitionToHasBeenOrderedCorrectly[storageEntityDefinition];
  }

  /// <inheritdoc/>
  public IReadOnlyList<PropertyDefinition> GetForeignKeyRelevantPropertyDefinitions (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    if (!_hasBeenInitialized)
      throw new InvalidOperationException($"Before accessing {nameof(GetForeignKeyRelevantPropertyDefinitions)}, {nameof(Initialize)} has to be called.");

    if (_foreignKeyRelevantPropertyDefinitions.TryGetValue(classDefinition, out var list))
      return list;

    return Array.Empty<PropertyDefinition>();
  }

  private IDictionary<ClassDefinition, List<PropertyDefinition>> CreateForeignKeyRelevantPropertyDefinitions (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var columnDefinitionComparer = new ColumnDefinitionEqualityComparer();
    var foreignKeyRelevantProperties = new Dictionary<ClassDefinition, List<PropertyDefinition>>();

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

      foreach (var foreignKey in node.Edges.SelectMany(e => e.ForeignKeys))
      {
        var matchingPropertyDefinition = classAndPropertyDefinitions.Where(cpc => cpc.Columns.SequenceEqual(foreignKey.ReferencingColumns, columnDefinitionComparer)).ToArray();
        foreach (var match in matchingPropertyDefinition)
        {
          classAndPropertyDefinitions.Remove(match);

          if (foreignKeyRelevantProperties.TryGetValue(match.ClassDefinition, out var list))
            list.Add(match.PropertyDefinition);
          else
            foreignKeyRelevantProperties[match.ClassDefinition] = [match.PropertyDefinition];
        }
      }
    }

    return foreignKeyRelevantProperties;
  }

  private IDictionary<IStorageEntityDefinition, int> CreateTableSortOrder (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var resultNodesWithoutBrokenEdges = new List<SortingOptimizationNode>();
    var resultNodesWithBrokenEdges = new List<SortingOptimizationNode>();

    var removedNodes = new HashSet<SortingOptimizationNode>();
    var remainingNodes = nodes.ToList();
    var lastRemainingNodesCount = -1;

    while (remainingNodes.Any())
    {
      if (remainingNodes.Count == lastRemainingNodesCount)
        throw new InvalidOperationException("Could not correctly determine sort order.");

      lastRemainingNodesCount = remainingNodes.Count;
      var currentLeafs = remainingNodes.Where(n => !n.Edges.Any(e => !e.IsSelfCyclingEdge && !removedNodes.Contains(e.PointingTo))).ToArray();
      foreach (var leaf in currentLeafs.OrderBy(c => c.TableName))
      {
        if (leaf.HasBrokenEdges)
          resultNodesWithBrokenEdges.Add(leaf);
        else
          resultNodesWithoutBrokenEdges.Add(leaf);

        removedNodes.Add(leaf);
        remainingNodes.Remove(leaf);
      }
    }

    return resultNodesWithBrokenEdges.Concat(resultNodesWithoutBrokenEdges)
        .Select((node, index) => (node.TableDefinition, index))
        .ToDictionary(k => (IStorageEntityDefinition)k.TableDefinition, v => v.index);
  }

  private void ApplyEdgeBreaks (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    // TODO: RM-9648 here we should get all edges that should always break and break them, maybe even before we calculate the dependencies the first time.

    var potentialNodes = nodes;
    while (true)
    {
      // we need to recalculate the indirect cyclic dependencies always after we broke edges
      foreach (var node in potentialNodes)
        node.CalculateIndirectCyclicDependencies();

      potentialNodes = potentialNodes.Where(n => n.HasIndirectCyclicDependencies).ToList();
      if (potentialNodes.Count == 0)
        break;

      // One instance of an edge can occur in the multiple IndirectSelfCyclicEdges
      // because it maybe in a chain leading to an indirect cycle.
      // So we try to figure out which node is most often the owner of those edges.
      // We then break this edges in hope this is the least amount of breaks we need.

      // TODO: RM-9648 here we need to check if the edge may not be broken eg: e.Value.Where(ed => ed.NoBreak)
      var mostGuiltyCyclicEdges = potentialNodes
          .SelectMany(node => node.IndirectSelfCyclicEdges.SelectMany(e => e.Value))
          .GroupBy(e => e.Owner) // Group edges by the node owner so we break these edges
          .OrderByDescending(g => g.Count()) // Order by nodes which are most often involved in an indirect cyclic dependency
          .ThenBy(g => g.Key.TableName) // Tie-break deterministically by table name
          .Select(k => k.ToList().Distinct()) // Distinct because an edge can occur multiple times because it could be the way to an indirect cycle 
          .First(); // We only take the currently most guilty edges

      foreach (var edge in mostGuiltyCyclicEdges)
        edge.BreakEdge();
    }
  }
}
