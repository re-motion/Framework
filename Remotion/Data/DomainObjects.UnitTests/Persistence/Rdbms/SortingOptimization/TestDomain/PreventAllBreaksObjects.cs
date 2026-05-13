// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class PreventBreaksObjectA : SortingOptimizationDomainBase
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.NeverBreak)]
  public PreventBreaksObjectB PreventBreakAPropB { get; set; }
}

[DBTable]
[Instantiable]
public abstract class PreventBreaksObjectB : SortingOptimizationDomainBase
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.NeverBreak)]
  public PreventBreaksObjectC PreventBreakBPropC { get; set; }
}

[DBTable]
[Instantiable]
public abstract class PreventBreaksObjectC : SortingOptimizationDomainBase
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.NeverBreak)]
  public PreventBreaksObjectA PreventBreakCPropA { get; set; }
}
