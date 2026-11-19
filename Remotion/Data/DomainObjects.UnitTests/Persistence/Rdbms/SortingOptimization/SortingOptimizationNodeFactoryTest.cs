// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class SortingOptimizationNodeFactoryTest : SortingOptimizationTestBase
{
  private SortingOptimizationNodeFactory _nodeFactory;

  [SetUp]
  public void SetUp ()
  {
    _nodeFactory = new SortingOptimizationNodeFactory(new DomainModelConstraintProvider());
  }

  [Test]
  public void CreateNodes_ResultIsValid ()
  {
    var mappingConfiguration = GetMappingConfiguration(
        typeof(NodeFactoryObjectA),
        typeof(NodeFactoryObjectB),
        typeof(NodeFactoryObjectC),
        typeof(NodeFactoryObjectDWithNoTable),
        typeof(NodeFactoryObjectMixin),
        typeof(NodeFactoryObjectWithMixin));

    var objectA = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectA));
    var objectB = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectB));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectC));
    var objectD = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectDWithNoTable));
    var objectE = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectWithMixin));

    var nodes = _nodeFactory.CreateNodes([objectA, objectB, objectC, objectD, objectE]);

    Assert.That(nodes.Count, Is.EqualTo(4));
    var aNode = nodes[0];
    var bNode = nodes[1];
    var cNode = nodes[2];
    var eNode = nodes[3];

    Assert.That(aNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectA)));
    AssertEdgesOfNode(aNode, [("FK_NodeFactoryObjectA_SelfCyclingPropID", aNode, true)]);
    AssertObjectIDPropertySpecification(
        aNode,
        [new ExpectedPropertySpecification(objectA, GetPropertyInfo<NodeFactoryObjectA>(o => o.SelfCyclingProp), true, ForeignKeyCycleBreakHint.Automatic)]);

    Assert.That(bNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectB)));
    Assert.That(bNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectD)));
    AssertEdgesOfNode(
        bNode,
        [
            ("FK_NodeFactoryObjectB_NodeFactoryBPropAID", aNode, false),
            ("FK_NodeFactoryObjectB_NodeFactoryDPropAID", aNode, false),
            ("FK_NodeFactoryObjectB_NodeFactoryDPropCID", cNode, false),
            ("FK_NodeFactoryObjectB_MixinPropAID", aNode, false)
        ]
    );
    AssertObjectIDPropertySpecification(
        bNode,
        [
            new ExpectedPropertySpecification(objectB, GetPropertyInfo<NodeFactoryObjectB>(o => o.NodeFactoryBPropA), true, ForeignKeyCycleBreakHint.Automatic),
            new ExpectedPropertySpecification(objectB, GetPropertyInfo<NodeFactoryObjectMixin>(o => o.MixinPropA), true, ForeignKeyCycleBreakHint.AlwaysBreak),
            new ExpectedPropertySpecification(objectD, GetPropertyInfo<NodeFactoryObjectB>(o => o.NodeFactoryBPropA), true, ForeignKeyCycleBreakHint.Automatic),
            new ExpectedPropertySpecification(objectD, GetPropertyInfo<NodeFactoryObjectMixin>(o => o.MixinPropA), true, ForeignKeyCycleBreakHint.AlwaysBreak),
            new ExpectedPropertySpecification(objectD, GetPropertyInfo<NodeFactoryObjectDWithNoTable>(o => o.NodeFactoryDPropA), true, ForeignKeyCycleBreakHint.Automatic),
            new ExpectedPropertySpecification(objectD, GetPropertyInfo<NodeFactoryObjectDWithNoTable>(o => o.NodeFactoryDPropC), true, ForeignKeyCycleBreakHint.NeverBreak),
            new ExpectedPropertySpecification(objectD, GetPropertyInfo<NodeFactoryObjectDWithNoTable>(o => o.NodeFactoryDPropASuppressForeignKey), false, ForeignKeyCycleBreakHint.Automatic)
        ]);

    Assert.That(cNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectC)));
    AssertEdgesOfNode(
        cNode,
        [
            ("FK_NodeFactoryObjectC_NodeFactoryCPropAID", aNode, false),
            ("FK_NodeFactoryObjectC_NodeFactoryCPropBID", bNode, false)
        ]
    );
    AssertObjectIDPropertySpecification(
        cNode,
        [
            new ExpectedPropertySpecification(objectC, GetPropertyInfo<NodeFactoryObjectC>(o => o.NodeFactoryCPropA), true, ForeignKeyCycleBreakHint.Automatic),
            new ExpectedPropertySpecification(objectC, GetPropertyInfo<NodeFactoryObjectC>(o => o.NodeFactoryCPropB), true, ForeignKeyCycleBreakHint.PreferredBreak)
        ]);

    Assert.That(eNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectE)));
    AssertEdgesOfNode(eNode, [("FK_NodeFactoryObjectWithMixin_MixinPropAID", aNode, false)]);
    AssertObjectIDPropertySpecification(
        eNode,
        [
            new ExpectedPropertySpecification(objectE, GetPropertyInfo<NodeFactoryObjectMixin>(o => o.MixinPropA), true, ForeignKeyCycleBreakHint.AlwaysBreak)
        ]);
  }

  [Test]
  public void CreateNodes_WithMissingLink_ThrowsException ()
  {
    var mappingConfiguration = GetMappingConfiguration(
        typeof(NodeFactoryObjectA),
        typeof(NodeFactoryObjectB),
        typeof(NodeFactoryObjectC),
        typeof(NodeFactoryObjectDWithNoTable),
        typeof(NodeFactoryObjectMixin));

    var objectA = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectA));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectC));

    Assert.That(
        () => _nodeFactory.CreateNodes([objectA, objectC]),
        Throws.InvalidOperationException.With.Message.EqualTo(
            "Could not find SortingOptimizationNode 'NodeFactoryObjectB' for foreign key 'FK_NodeFactoryObjectC_NodeFactoryCPropBID' on node 'NodeFactoryObjectC'."));
  }

  private void AssertEdgesOfNode (SortingOptimizationNode nodeToTest, (string foreignKeyName, SortingOptimizationNode pointingTo, bool isSelfCyclic)[] expected)
  {
    Assert.That(nodeToTest.Edges.Count, Is.EqualTo(expected.Length));

    foreach (var expectedValue in expected)
    {
      var edge = nodeToTest.Edges.FirstOrDefault(n => n.ForeignKey.ConstraintName == expectedValue.foreignKeyName);
      Assert.That(edge, Is.Not.Null, $"Could not find {nameof(SortingOptimizationEdge)} with constraint name '{expectedValue.foreignKeyName}'.");
      Assert.That(edge.IsSelfCyclingEdge, Is.EqualTo(expectedValue.isSelfCyclic));
      Assert.That(edge.PointingTo, Is.EqualTo(expectedValue.pointingTo));
      Assert.That(edge.ForeignKey.ConstraintName, Is.EquivalentTo(expectedValue.foreignKeyName));
    }
  }

  private void AssertObjectIDPropertySpecification (SortingOptimizationNode nodeToTest, ExpectedPropertySpecification[] expectedValues)
  {
    var groupedByClass = expectedValues.GroupBy(e => e.ClassDefinition).ToList();
    Assert.That(nodeToTest.ObjectIDPropertySpecifications.Count, Is.EqualTo(groupedByClass.Count));

    foreach (var expected in groupedByClass)
    {
      Assert.That(nodeToTest.ObjectIDPropertySpecifications.ContainsKey(expected.Key), Is.True);
      var actualProperties = nodeToTest.ObjectIDPropertySpecifications[expected.Key];
      AssertForeignKeyPropertySpecifications(expected.Key, expected, actualProperties);
    }
  }
}
