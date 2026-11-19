// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;

/// <summary>
/// <see cref="ISortingOptimizationNodeFactory"/> creates <see cref="SortingOptimizationNode"/>s for ordering <see cref="ClassDefinition"/>s/<see cref="TableDefinition"/>
/// </summary>
public interface ISortingOptimizationNodeFactory
{
  /// <summary>
  /// Creates all <see cref="SortingOptimizationNode"/> including all <see cref="SortingOptimizationEdge"/>s.
  /// </summary>
  IReadOnlyList<SortingOptimizationNode> CreateNodes (IReadOnlyList<ClassDefinition> classDefinitions);
}
