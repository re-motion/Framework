// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
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
  private readonly ILogger _logger;
  private readonly ISortingOptimizationNodeFactory _nodeFactory;

  private IDictionary<ClassDefinition, IReadOnlyCollection<SortingOptimizationObjectIDPropertySpecification>> _objectIDPropertyPropertySpecifications = null!;
  private IDictionary<IStorageEntityDefinition, int> _sortOrderByStorageEntityDefinition = null!;
  private IDictionary<ClassDefinition, int> _sortOrderByClassDefinition = null!;

  private bool _hasBeenInitialized;

  public GraphBasedPersistenceModelSortingProvider (ISortingOptimizationNodeFactory nodeFactory, ILoggerFactory loggerFactory)
  {
    ArgumentNullException.ThrowIfNull(nodeFactory);
    ArgumentNullException.ThrowIfNull(loggerFactory);

    _nodeFactory = nodeFactory;
    _logger = loggerFactory.CreateLogger<GraphBasedPersistenceModelSortingProvider>();
  }

  /// <inheritdoc/>
  public void Initialize (IReadOnlyList<ClassDefinition> classDefinitions)
  {
    ArgumentNullException.ThrowIfNull(classDefinitions);

    var nodes = _nodeFactory.CreateNodes(classDefinitions);

    _objectIDPropertyPropertySpecifications = nodes.SelectMany(d => d.ObjectIDPropertySpecifications).ToDictionary(k => k.Key, v => v.Value);

    ResolveIndirectCyclicDependencies(nodes);

    (_sortOrderByStorageEntityDefinition, _sortOrderByClassDefinition) = CreateTableSortOrder(nodes);
    _hasBeenInitialized = true;
  }

  /// <inheritdoc/>
  public int GetSortPosition (IStorageEntityDefinition storageEntityDefinition)
  {
    ArgumentNullException.ThrowIfNull(storageEntityDefinition);

    if (!_hasBeenInitialized)
      throw new InvalidOperationException($"Before accessing {nameof(GetSortPosition)}, {nameof(Initialize)} has to be called.");

    return _sortOrderByStorageEntityDefinition[storageEntityDefinition];
  }

  /// <inheritdoc/>
  public int GetSortPosition (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    if (!_hasBeenInitialized)
      throw new InvalidOperationException($"Before accessing {nameof(GetSortPosition)}, {nameof(Initialize)} has to be called.");

    return _sortOrderByClassDefinition[classDefinition];
  }

  /// <inheritdoc/>
  public IReadOnlyCollection<SortingOptimizationObjectIDPropertySpecification> GetPropertySpecificationsForForeignKeyProperties (ClassDefinition classDefinition)
  {
    ArgumentNullException.ThrowIfNull(classDefinition);

    if (!_hasBeenInitialized)
      throw new InvalidOperationException($"Before accessing {nameof(GetPropertySpecificationsForForeignKeyProperties)}, {nameof(Initialize)} has to be called.");

    if (_objectIDPropertyPropertySpecifications.TryGetValue(classDefinition, out var list))
      return list;

    return Array.Empty<SortingOptimizationObjectIDPropertySpecification>();
  }

  private (IDictionary<IStorageEntityDefinition, int> byTable, IDictionary<ClassDefinition, int> byClassDefiniion) CreateTableSortOrder (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var resultNodesWithoutBrokenEdges = new List<SortingOptimizationNode>();
    var resultNodesWithBrokenEdges = new List<SortingOptimizationNode>();

    var removedNodes = new HashSet<SortingOptimizationNode>();
    var remainingNodes = nodes.ToList();
    var lastRemainingNodesCount = -1;

    while (remainingNodes.Any())
    {
      if (remainingNodes.Count == lastRemainingNodesCount)
      {
        // this should be unreachable because ApplyEdgeBreaks should remove all indirect cyclic edges and
        // if this was not possible ApplyEdgeBreaks already throws an exception and therefore
        // sorting should always be possible because there should always be leaf nodes.
        // So this is a failsafe if something else breaks to prevent endless loops.
        throw new InvalidOperationException("Could not correctly determine sort order. This should be unreachable!");
      }

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

    var concatenatedNodes = resultNodesWithBrokenEdges.Concat(resultNodesWithoutBrokenEdges).ToList();
    var byTableDefinition = concatenatedNodes
        .Select((node, index) => (node.TableDefinition, index))
        .ToDictionary(k => (IStorageEntityDefinition)k.TableDefinition, v => v.index);

    var byClassDefinition = concatenatedNodes.Select((node, index) => (node, index)).SelectMany(x => x.node.ClassDefinitions.Select(cd => (cd, x.index)))
        .ToDictionary(k => k.cd, v => v.index);

    return (byTableDefinition, byClassDefinition);
  }

  private void ResolveIndirectCyclicDependencies (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var logMessageBuilder = new StringBuilder();
    var previousNumberOfCyclicEdges = -1;
    while (true)
    {
      var numberOfCyclicEdges = 0;
      // we need to recalculate the indirect cyclic dependencies always after we broke edges
      foreach (var node in nodes)
      {
        node.CalculateIndirectCyclicDependencies();
        numberOfCyclicEdges += node.IndirectSelfCyclicEdges.Sum(i => i.Value.Count);
      }

      // if there are no cyclic edges we are done.
      if (numberOfCyclicEdges == 0)
        break;

      // if the previous number matches the current number we have hit a dead end.
      if (previousNumberOfCyclicEdges == numberOfCyclicEdges)
        throw new InvalidOperationException($"{nameof(GraphBasedPersistenceModelSortingProvider)} could not resolve indirect cyclic dependencies.{Environment.NewLine}{CreateCyclicDependenciesErrorMessageHint(nodes)}");

      previousNumberOfCyclicEdges = numberOfCyclicEdges;

      // we try to break only necessary edges therefore 
      // if there are any indirect cyclic dependencies we try to break AlwaysBreak first then PreferredBreak and then Automatic
      // in hope that this leads to the least amount of breaks so that most the tables can be sorted correct.
      var edgesToBreak = GetMostGuiltyCyclicEdges(nodes, ForeignKeyCycleBreakHint.AlwaysBreak);
      if (edgesToBreak.Count == 0)
        edgesToBreak = GetMostGuiltyCyclicEdges(nodes, ForeignKeyCycleBreakHint.PreferredBreak);
      if (edgesToBreak.Count == 0)
        edgesToBreak = GetMostGuiltyCyclicEdges(nodes, ForeignKeyCycleBreakHint.Automatic);

      foreach (var edge in edgesToBreak)
      {
        edge.BreakEdge();
        AppendBrokenEdgeLogMessage(logMessageBuilder, edge);
      }
    }

    if (logMessageBuilder.Length > 0)
    {
      logMessageBuilder.Insert(0, $"Persistence dependency cycles with breaks:{Environment.NewLine}");
      _logger.LogInformation(logMessageBuilder.ToString());
    }
  }

  private static List<SortingOptimizationEdge> GetMostGuiltyCyclicEdges (IReadOnlyList<SortingOptimizationNode> nodes, ForeignKeyCycleBreakHint withBreakHint)
  {
    // One instance of an edge can occur in the multiple IndirectSelfCyclicEdges
    // because it maybe in a chain leading to an indirect cycle.
    // So we try to figure out which node is most often the owner of those edges.
    // We then break this edges in hope this is the least amount of breaks we need.

    return nodes
        .SelectMany(node => node.IndirectSelfCyclicEdges.SelectMany(e => e.Value.Where(ed => ed.CycleBreakHint == withBreakHint)))
        .GroupBy(e => e.Owner) // Group edges by the node owner so we break these edges
        .OrderByDescending(g => g.Count()) // Order by nodes which are most often involved in an indirect cyclic dependency
        .ThenBy(g => g.Key.TableName) // Tie-break deterministically by table name
        .Select(k => k.ToList().Distinct()) // Distinct because an edge can occur multiple times because it could be the way to an indirect cycle 
        .FirstOrDefault()?.ToList() ?? [];
  }

  private void AppendBrokenEdgeLogMessage (StringBuilder stringBuilder, SortingOptimizationEdge edge)
  {
    var hint = $"  CYCLEBREAK ({edge.CycleBreakHint}) -> ";

    if (stringBuilder.Length > 0)
      stringBuilder.AppendLine();

    foreach (var edgeAndIndex in edge.Owner.IndirectSelfCyclicEdges[edge].Select((e, index) => new { Edge = e, Index = index + 1 }).ToArray())
    {
      var isBrokenEdge = edgeAndIndex.Edge == edge;
      if (isBrokenEdge)
        stringBuilder.Append($"{hint}{edgeAndIndex.Index}. ");
      else
        stringBuilder.Append(' ', hint.Length).Append($"{edgeAndIndex.Index}. ");

      AppendEdgeConnection(stringBuilder, edgeAndIndex.Edge);
    }
  }

  private string CreateCyclicDependenciesErrorMessageHint (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var messageBuilder = new StringBuilder();

    foreach (var node in nodes)
    {
      messageBuilder.AppendLine($"Table: {node.TableName}");
      foreach (var edges in node.IndirectSelfCyclicEdges.Values)
      {
        foreach (var edgeAndIndex in edges.Select((edge, index) => new { edge, index = index + 1 }).ToArray())
        {
          messageBuilder.Append($"  {edgeAndIndex.index}. ");
          AppendEdgeConnection(messageBuilder, edgeAndIndex.edge);
        }
        messageBuilder.AppendLine();
      }
    }

    return messageBuilder.ToString().Trim();
  }

  private void AppendEdgeConnection (StringBuilder stringBuilder, SortingOptimizationEdge edge)
  {
    stringBuilder.AppendJoin(", ", edge.ForeignKey.ReferencingColumns.Select(rc => edge.Owner.TableName + "." + rc.Name));
    stringBuilder.Append(" -> ");
    stringBuilder.AppendJoin(", ", edge.ForeignKey.ReferencedColumns.Select(rc => edge.PointingTo.TableName + "." + rc.Name));
    stringBuilder.AppendLine();
  }
}
