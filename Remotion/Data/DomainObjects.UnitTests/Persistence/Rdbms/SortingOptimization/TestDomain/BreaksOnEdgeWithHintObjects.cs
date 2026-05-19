// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class BreaksOnEdgeWithHintObjectA : SortingOptimizationDomainBase
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.PreferredBreak)]
  public BreaksOnEdgeWithHintObjectZ AutomaticBreakCountAPropZ { get; set; }

  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.AlwaysBreak)]
  public BreaksOnEdgeWithHintObjectB AutomaticBreakCountAPropB { get; set; }
}

[DBTable]
[Instantiable]
public abstract class BreaksOnEdgeWithHintObjectB : SortingOptimizationDomainBase
{
  public BreaksOnEdgeWithHintObjectC AutomaticBreakCountBPropC { get; set; }
}

[DBTable]
[Instantiable]
public abstract class BreaksOnEdgeWithHintObjectC : SortingOptimizationDomainBase
{
  public BreaksOnEdgeWithHintObjectZ AutomaticBreakCountCPropZ { get; set; }
}

[DBTable]
[Instantiable]
public abstract class BreaksOnEdgeWithHintObjectZ : SortingOptimizationDomainBase
{
  // multiple edges ensure this should be broken as first but because BreaksOnEdgeWithHintObjectA has defined
  // break hints that are more important than automatic BreaksOnEdgeWithHintObjectZ should not have breaks
  public BreaksOnEdgeWithHintObjectA AutomaticBreakCountZPropA1 { get; set; }
  public BreaksOnEdgeWithHintObjectA AutomaticBreakCountZPropA2 { get; set; }
  public BreaksOnEdgeWithHintObjectA AutomaticBreakCountZPropA3 { get; set; }
}
