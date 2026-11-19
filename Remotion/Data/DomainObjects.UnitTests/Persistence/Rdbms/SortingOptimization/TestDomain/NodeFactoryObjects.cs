// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

[DBTable]
[Instantiable]
public abstract class NodeFactoryObjectA : SortingOptimizationDomainBase
{
  public NodeFactoryObjectA SelfCyclingProp { get; set; }
}

[DBTable]
[Instantiable]
[Uses(typeof(NodeFactoryObjectMixin))]
public abstract class NodeFactoryObjectB : SortingOptimizationDomainBase
{
  public NodeFactoryObjectA NodeFactoryBPropA { get; set; }
}

[DBTable]
[Instantiable]
public abstract class NodeFactoryObjectC : SortingOptimizationDomainBase
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.Automatic)]
  public NodeFactoryObjectA NodeFactoryCPropA { get; set; }

  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.PreferredBreak)]
  public NodeFactoryObjectB NodeFactoryCPropB { get; set; }
}

[Instantiable]
public abstract class NodeFactoryObjectDWithNoTable : NodeFactoryObjectB
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.NeverBreak)]
  public NodeFactoryObjectC NodeFactoryDPropC { get; set; }

  public NodeFactoryObjectA NodeFactoryDPropA { get; set; }

  [SuppressForeignKeyConstraint]
  public NodeFactoryObjectA NodeFactoryDPropASuppressForeignKey { get; set; }
}

[DBTable]
[Instantiable]
[Uses(typeof(NodeFactoryObjectMixin))]
public abstract class NodeFactoryObjectWithMixin : SortingOptimizationDomainBase
{
}

public class NodeFactoryObjectMixin : DomainObjectMixin<SortingOptimizationDomainBase>
{
  [ForeignKeyCycleBreakHint(ForeignKeyCycleBreakHint.AlwaysBreak)]
  public NodeFactoryObjectA MixinPropA { get; set; }
}
