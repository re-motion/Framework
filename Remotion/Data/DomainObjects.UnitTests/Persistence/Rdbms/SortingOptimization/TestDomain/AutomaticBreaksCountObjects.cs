// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksCountObjectA : SortingOptimizationDomainBase
{
  public AutomaticBreaksCountObjectZ AutomaticBreakCountAPropZ { get; set; }
}

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksCountObjectB : SortingOptimizationDomainBase
{
  public AutomaticBreaksCountObjectC AutomaticBreakCountBPropC { get; set; }
}

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksCountObjectC : SortingOptimizationDomainBase
{
  public AutomaticBreaksCountObjectZ AutomaticBreakCountCPropZ { get; set; }
}

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksCountObjectZ : SortingOptimizationDomainBase
{
  public AutomaticBreaksCountObjectA AutomaticBreakCountZPropA { get; set; }
  public AutomaticBreaksCountObjectB AutomaticBreakCountZPropB { get; set; }
}
