// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksTableNameObjectA : SortingOptimizationDomainBase
{
  public AutomaticBreaksTableNameObjectB AutomaticBreakTableNameBPropB { get; set; }
}

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksTableNameObjectB : SortingOptimizationDomainBase
{
  public AutomaticBreaksTableNameObjectC AutomaticBreakTableNameBPropC { get; set; }
}

[DBTable]
[Instantiable]
public abstract class AutomaticBreaksTableNameObjectC : SortingOptimizationDomainBase
{
  public AutomaticBreaksTableNameObjectA AutomaticBreakTableNameCPropA { get; set; }
}
