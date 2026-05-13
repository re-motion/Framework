// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class NodeFactoryObjectA : SortingOptimizationDomainBase
{
  public NodeFactoryObjectA SelfCyclingProp { get; set; }
}

[DBTable]
[Instantiable]
public abstract class NodeFactoryObjectB : SortingOptimizationDomainBase
{
  public NodeFactoryObjectA NodeFactoryBPropA { get; set; }
}

[DBTable]
[Instantiable]
public abstract class NodeFactoryObjectC : SortingOptimizationDomainBase
{
  public NodeFactoryObjectA NodeFactoryCPropA { get; set; }
  public NodeFactoryObjectB NodeFactoryCPropB { get; set; }
}

[Instantiable]
public abstract class NodeFactoryObjectDWithNoTable : NodeFactoryObjectB
{
  public NodeFactoryObjectC NodeFactoryDPropC { get; set; }

  public NodeFactoryObjectA NodeFactoryDPropA { get; set; }
}
