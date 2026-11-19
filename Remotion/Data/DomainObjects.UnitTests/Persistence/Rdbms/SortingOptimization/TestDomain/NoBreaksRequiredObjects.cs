// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class NoBreaksObjectA : SortingOptimizationDomainBase
{
}

[DBTable]
[Instantiable]
public abstract class NoBreaksObjectB : SortingOptimizationDomainBase
{
  public NoBreaksObjectA NoBreakBPropA { get; set; }

  public NoBreaksObjectC NoBreakBPropC { get; set; }
}

[DBTable]
[Instantiable]
public abstract class NoBreaksObjectC : SortingOptimizationDomainBase
{
  public NoBreaksObjectA NoBreakCPropA { get; set; }

  [SuppressForeignKeyConstraint]
  public NoBreaksObjectA NoBreakCPropAWithoutForeignKey { get; set; }
}
