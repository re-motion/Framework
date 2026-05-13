// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

/// <summary>
/// <see cref="SortingOptimizationEdge"/> represents one edge of an <see cref="SortingOptimizationNode"/>.
/// </summary>
[DebuggerDisplay("Edge: {Owner.TableName} -> {PointingTo.TableName}")]
public class SortingOptimizationEdge
{
  private readonly List<ForeignKeyConstraintDefinition> _foreignKeys;

  public SortingOptimizationEdge (ForeignKeyConstraintDefinition foreignKey, SortingOptimizationNode owner, SortingOptimizationNode pointingTo)
  {
    ArgumentNullException.ThrowIfNull(foreignKey);
    ArgumentNullException.ThrowIfNull(owner);
    ArgumentNullException.ThrowIfNull(pointingTo);

    _foreignKeys = [foreignKey];
    Owner = owner;
    PointingTo = pointingTo;
    IsSelfCyclingEdge = owner == pointingTo;
  }

  /// <summary>
  /// Gets all <see cref="ForeignKeyConstraintDefinition"/> relevant for this <see cref="SortingOptimizationEdge"/>.
  /// </summary>
  public IReadOnlyCollection<ForeignKeyConstraintDefinition> ForeignKeys => _foreignKeys;

  /// <summary>
  /// Gets the owner <see cref="SortingOptimizationNode"/> of this <see cref="SortingOptimizationEdge"/>.
  /// </summary>
  public SortingOptimizationNode Owner { get; }

  /// <summary>
  /// Gets the <see cref="SortingOptimizationNode"/> this <see cref="SortingOptimizationEdge"/> is pointing to.
  /// </summary>
  public SortingOptimizationNode PointingTo { get; }

  /// <summary>
  /// Gets if this <see cref="SortingOptimizationEdge"/> is self cycling, meaning <see cref="Owner"/> and <see cref="PointingTo"/> are the same <see cref="SortingOptimizationNode"/>.
  /// </summary>
  public bool IsSelfCyclingEdge { get; }

  /// <summary>
  /// Breaks the this <see cref="SortingOptimizationEdge"/> so it is "ignored" for ordering.
  /// </summary>
  public void BreakEdge ()
  {
    Owner.BreakEdge(this);
  }

  /// <summary>
  /// Adds a <see cref="ForeignKeyConstraintDefinition"/> to <see cref="ForeignKeys"/>.
  /// </summary>
  public void AddForeignKey (ForeignKeyConstraintDefinition foreignKey)
  {
    ArgumentNullException.ThrowIfNull(foreignKey);

    _foreignKeys.Add(foreignKey);
  }
}
