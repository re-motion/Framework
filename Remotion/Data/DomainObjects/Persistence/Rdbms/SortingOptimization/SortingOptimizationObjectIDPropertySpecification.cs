// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

/// <summary>
/// <see cref="SortingOptimizationObjectIDPropertySpecification"/> provides information about <see cref="PropertyDefinition"/> required for batched rdbms provider commands.
/// </summary>
public class SortingOptimizationObjectIDPropertySpecification
{
  public SortingOptimizationObjectIDPropertySpecification (PropertyDefinition propertyDefinition, bool hasForeignKeyConstraint, ForeignKeyCycleBreakHint cycleBreakHint)
  {
    ArgumentNullException.ThrowIfNull(propertyDefinition);
    PropertyDefinition = propertyDefinition;
    HasForeignKeyConstraint = hasForeignKeyConstraint;
    CycleBreakHint = cycleBreakHint;
  }

  public PropertyDefinition PropertyDefinition { get; }

  /// <summary>
  /// Gets if the <see cref="PropertyDefinition"/> has an <see cref="ForeignKeyConstraintDefinition"/>.
  /// </summary>
  public bool HasForeignKeyConstraint { get; }

  public ForeignKeyCycleBreakHint CycleBreakHint { get; }
}
