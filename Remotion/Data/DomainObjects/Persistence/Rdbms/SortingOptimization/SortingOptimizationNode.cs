// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

/// <summary>
/// <see cref="SortingOptimizationNode"/> represents on <see cref="TableDefinition"/> and possible multiple <see cref="ClassDefinitions"/>s
/// </summary>
[DebuggerDisplay("Node: {TableName}")]
public class SortingOptimizationNode
{
  private readonly List<SortingOptimizationEdge> _edges = [];
  private readonly List<SortingOptimizationEdge> _brokenEdges = [];
  private readonly Dictionary<SortingOptimizationEdge, List<SortingOptimizationEdge>> _indirectSelfCyclicEdges = new();

  public SortingOptimizationNode (TableDefinition tableDefinition, IReadOnlyList<ClassDefinition> classDefinitions)
  {
    ArgumentNullException.ThrowIfNull(tableDefinition);
    ArgumentNullException.ThrowIfNull(classDefinitions);

    foreach(var classDefinition in classDefinitions)
    {
      var td = InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition?>(
          (IRdbmsStorageEntityDefinition)classDefinition.StorageEntityDefinition,
          (table, _) => table,
          (filterView, continuation) => continuation(filterView.BaseEntity),
          (unionView, _) => null,
          (emptyView, _) => null);

      if (td != tableDefinition)
      {
        throw new ArgumentException(
            $"{nameof(ClassDefinition)} '{classDefinition.ID}' has a {nameof(IStorageEntityDefinition)} "
            + $"that does not match the {nameof(TableDefinition)} '{tableDefinition.TableName.EntityName}'.",
            nameof(classDefinitions));
      }
    }
    TableDefinition = tableDefinition;
    ClassDefinitions = classDefinitions;
  }

  /// <summary>
  /// Gets the <see cref="TableDefinition"/> for this <see cref="SortingOptimizationNode"/>.
  /// </summary>
  public TableDefinition TableDefinition { get; }

  /// <summary>
  /// Gets all <see cref="ClassDefinition"/>s for this <see cref="SortingOptimizationNode"/>.
  /// </summary>
  public IReadOnlyList<ClassDefinition> ClassDefinitions { get; }

  /// <summary>
  /// Gets the name of the <see cref="TableDefinition"/>
  /// </summary>
  public string TableName => TableDefinition.TableName.EntityName;

  /// <summary>
  /// Gets all non-broken <see cref="SortingOptimizationEdge"/>s.
  /// </summary>
  public IReadOnlyList<SortingOptimizationEdge> Edges => _edges;

  /// <summary>
  /// Gets all broken <see cref="SortingOptimizationEdge"/>s.
  /// </summary>
  public IReadOnlyList<SortingOptimizationEdge> BrokenEdges => _brokenEdges;

  /// <summary>
  /// Gets all <see cref="SortingOptimizationEdge"/> which cause indirect cyclic dependencies.
  /// The key is the <see cref="SortingOptimizationEdge"/> of this <see cref="SortingOptimizationNode"/> starting the cyclic dependency.
  /// The value is the list of all <see cref="SortingOptimizationEdge"/>s involved in the cyclic dependency.
  /// This will be empty until <see cref="CalculateIndirectCyclicDependencies"/> has been called.
  /// </summary>
  public IReadOnlyDictionary<SortingOptimizationEdge, List<SortingOptimizationEdge>> IndirectSelfCyclicEdges => _indirectSelfCyclicEdges;

  /// <summary>
  /// Gets if this <see cref="SortingOptimizationNode"/> has cyclic dependency.
  /// </summary>
  public bool HasIndirectCyclicDependencies => _indirectSelfCyclicEdges.Count > 0;

  /// <summary>
  /// Gets if this <see cref="SortingOptimizationNode"/> has broken edges.
  /// </summary>
  public bool HasBrokenEdges => _brokenEdges.Count > 0;

  /// <summary>
  /// Adds a <see cref="SortingOptimizationEdge"/> to this <see cref="SortingOptimizationNode"/> or
  /// updates the <see cref="SortingOptimizationEdge.ForeignKeys"/> list of an already existing <see cref="SortingOptimizationEdge"/>
  /// </summary>
  public void AddEdge (ForeignKeyConstraintDefinition foreignKey, SortingOptimizationNode pointingTo)
  {
    ArgumentNullException.ThrowIfNull(foreignKey);
    ArgumentNullException.ThrowIfNull(pointingTo);

    // if there is already an edge pointing to the node we just add the foreign key to this edge
    var existingEdge = _edges.FirstOrDefault(e => e.PointingTo == pointingTo);
    if (existingEdge == null)
    {
      var edge = new SortingOptimizationEdge(foreignKey, this, pointingTo);
      _edges.Add(edge);
    }
    else
    {
      existingEdge.AddForeignKey(foreignKey);
    }
  }

  /// <summary>
  /// Breaks the given <paramref name="edge"/>
  /// </summary>
  public void BreakEdge (SortingOptimizationEdge edge)
  {
    ArgumentNullException.ThrowIfNull(edge);

    _edges.Remove(edge);
    _brokenEdges.Add(edge);
  }

  /// <summary>
  /// Calculates if this <see cref="SortingOptimizationNode"/> has cyclic dependencies
  /// and updates <see cref="IndirectSelfCyclicEdges"/>.
  /// </summary>
  public void CalculateIndirectCyclicDependencies ()
  {
    _indirectSelfCyclicEdges.Clear();

    var edges = Edges.Where(e => !e.IsSelfCyclingEdge);

    foreach (var edge in edges)
    {
      var currentStack = new Stack<SortingOptimizationEdge>();
      var visitedNodes = new HashSet<SortingOptimizationNode>();
      if (CalculateHasIndirectCyclicDependency(edge, currentStack, visitedNodes))
      {
        var guiltyEdges = currentStack.ToList();
        guiltyEdges.Reverse();
        _indirectSelfCyclicEdges.Add(edge, guiltyEdges);
      }
    }
  }

  private bool CalculateHasIndirectCyclicDependency (SortingOptimizationEdge currentEdge, Stack<SortingOptimizationEdge> edgeStack, HashSet<SortingOptimizationNode> visitedNodes)
  {
    edgeStack.Push(currentEdge);

    var currentEdgePointingToNode = currentEdge.PointingTo;
    if (currentEdgePointingToNode == this)
      return true;

    visitedNodes.Add(currentEdgePointingToNode);

    foreach (var edge in currentEdgePointingToNode.Edges)
    {
      if (!edge.IsSelfCyclingEdge && !visitedNodes.Contains(edge.PointingTo))
      {
        if (CalculateHasIndirectCyclicDependency(edge, edgeStack, visitedNodes))
          return true;

        edgeStack.Pop();
      }
    }

    return false;
  }
}
