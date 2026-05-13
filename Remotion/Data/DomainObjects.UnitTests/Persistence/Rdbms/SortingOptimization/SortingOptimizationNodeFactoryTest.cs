// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class SortingOptimizationNodeFactoryTest : SortingOptimizationTestBase
{
  private SortingOptimizationNodeFactory _nodeFactory;
  private MappingConfiguration _mappingConfiguration;

  [SetUp]
  public void SetUp ()
  {
    _nodeFactory = new SortingOptimizationNodeFactory();
    _mappingConfiguration = GetMappingConfiguration();
  }

  [Test]
  public void CreateNodes_ResultIsValid ()
  {
    var objectA = _mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectA));
    var objectB = _mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectB));
    var objectC = _mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectC));
    var objectD = _mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectDWithNoTable));

    var nodes = _nodeFactory.CreateNodes([objectA, objectB, objectC, objectD]);

    Assert.That(nodes.Count, Is.EqualTo(3));
    var aNode = nodes[0];
    var bNode = nodes[1];
    var cNode = nodes[2];

    Assert.That(aNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectA)));
    Assert.That(aNode.Edges.Count, Is.EqualTo(1));
    AssertEdge(aNode.Edges[0], ["FK_NodeFactoryObjectA_SelfCyclingPropID"], aNode, true);

    Assert.That(bNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectB)));
    Assert.That(bNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectD)));
    Assert.That(bNode.Edges.Count, Is.EqualTo(2));
    AssertEdge(bNode.Edges[0], ["FK_NodeFactoryObjectB_NodeFactoryBPropAID", "FK_NodeFactoryObjectB_NodeFactoryDPropAID"], aNode, false);
    AssertEdge(bNode.Edges[1], ["FK_NodeFactoryObjectB_NodeFactoryDPropCID"], cNode, false);

    Assert.That(cNode.TableDefinition, Is.EqualTo(GetTableDefinition(objectC)));
    Assert.That(cNode.Edges.Count, Is.EqualTo(2));

    AssertEdge(cNode.Edges[0], ["FK_NodeFactoryObjectC_NodeFactoryCPropAID"], aNode, false);
    AssertEdge(cNode.Edges[1], ["FK_NodeFactoryObjectC_NodeFactoryCPropBID"], bNode, false);
  }

  [Test]
  public void CreateNodes_WithMissingLink_ThrowsException ()
  {
    var objectA = _mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectA));
    var objectC = _mappingConfiguration.GetTypeDefinition(typeof(NodeFactoryObjectC));

    Assert.That(
        () => _nodeFactory.CreateNodes([objectA, objectC]),
        Throws.InvalidOperationException.With.Message.EqualTo(
            "Could not find SortingOptimizationNode 'NodeFactoryObjectB' for foreign key 'FK_NodeFactoryObjectC_NodeFactoryCPropBID' on node 'NodeFactoryObjectC'."));
  }

  private void AssertEdge (SortingOptimizationEdge edgeToTest, string[] foreignKeyNames, SortingOptimizationNode pointingTo, bool isSelfCyclic)
  {
    Assert.That(edgeToTest.IsSelfCyclingEdge, Is.EqualTo(isSelfCyclic));
    Assert.That(edgeToTest.PointingTo, Is.EqualTo(pointingTo));

    Assert.That(edgeToTest.ForeignKeys.Count, Is.EqualTo(foreignKeyNames.Length));
    Assert.That(edgeToTest.ForeignKeys.Select(k => k.ConstraintName), Is.EquivalentTo(foreignKeyNames));
  }
}
