// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class GraphBasedPersistenceModelSortingProviderTest : SortingOptimizationTestBase
{
  private Mock<ISortingOptimizationNodeFactory> _nodeFactoryStrictMock;
  private ISortingOptimizationNodeFactory _realNodeFactory;
  private MappingConfiguration _mappingConfiguration;

  [SetUp]
  public void SetUp ()
  {
    _realNodeFactory = new SortingOptimizationNodeFactory();
    _nodeFactoryStrictMock = new Mock<ISortingOptimizationNodeFactory>(MockBehavior.Strict);
    _mappingConfiguration = GetMappingConfiguration();
  }

  [Test]
  public void CreatesCorrectOrder_WithoutAnyBreaks ()
  {
    var objectA = _mappingConfiguration.GetTypeDefinition(typeof(NoBreaksObjectA));
    var objectB = _mappingConfiguration.GetTypeDefinition(typeof(NoBreaksObjectB));
    var objectC = _mappingConfiguration.GetTypeDefinition(typeof(NoBreaksObjectC));

    var provider = new GraphBasedPersistenceModelSortingProvider(_realNodeFactory);
    provider.Initialize([objectC, objectB, objectA]);

    AssertPositionAndCorrectOrder(
        provider,
        [
            (objectA, true),
            (objectC, true),
            (objectB, true)
        ]
    );

    AssertForeignKeyProperties(provider, objectA, []);
    AssertForeignKeyProperties(provider, objectB, [nameof(NoBreaksObjectB.NoBreakBPropA), nameof(NoBreaksObjectB.NoBreakBPropC)]);
    AssertForeignKeyProperties(provider, objectC, [nameof(NoBreaksObjectC.NoBreakCPropA)]);
  }

  [Test]
  public void CreatesCorrectOrder_WithoutAutomaticBreaks_BreakIsDeterminedByTableName ()
  {
    var objectA = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksTableNameObjectA));
    var objectB = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksTableNameObjectB));
    var objectC = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksTableNameObjectC));

    var nodes = _realNodeFactory.CreateNodes([objectB, objectA, objectC]);

    var nodeForA = GetNodeForClassDefinition(objectA, nodes);
    var nodeForB = GetNodeForClassDefinition(objectB, nodes);
    var nodeForC = GetNodeForClassDefinition(objectC, nodes);
    var expectedBrokenEdge = GetEdgeForProperty(nameof(AutomaticBreaksTableNameObjectA.AutomaticBreakTableNameBPropB), nodeForA);

    SetupNodeFactoryMock(nodes);

    Assert.That(nodeForA.HasBrokenEdges, Is.False);
    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);

    var provider = new GraphBasedPersistenceModelSortingProvider(_nodeFactoryStrictMock.Object);
    provider.Initialize([]);

    Assert.That(nodeForA.HasBrokenEdges, Is.True);
    Assert.That(nodeForA.BrokenEdges.Count, Is.EqualTo(1));
    Assert.That(nodeForA.BrokenEdges.Contains(expectedBrokenEdge), Is.True);

    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);

    AssertPositionAndCorrectOrder(
        provider,
        [
            (objectA, false), // A is broken therefore first
            (objectC, true), // C has no other dependencies than A and A is broken therefore second
            (objectB, true) // B has dependency on C therefore third
        ]
    );

    AssertForeignKeyProperties(provider, objectA, [nameof(AutomaticBreaksTableNameObjectA.AutomaticBreakTableNameBPropB)]);
    AssertForeignKeyProperties(provider, objectB, [nameof(AutomaticBreaksTableNameObjectB.AutomaticBreakTableNameBPropC)]);
    AssertForeignKeyProperties(provider, objectC, [nameof(AutomaticBreaksTableNameObjectC.AutomaticBreakTableNameCPropA)]);
  }

  [Test]
  public void CreatesCorrectOrder_WithoutAutomaticBreaks_BreakIsDeterminedByCountOfOccurence ()
  {
    var objectZ = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectZ));
    var objectA = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectA));
    var objectB = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectB));
    var objectC = _mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectC));

    var nodes = _realNodeFactory.CreateNodes([objectZ, objectC, objectA, objectB]);

    foreach(var node in nodes)
      node.CalculateIndirectCyclicDependencies();

    var nodeForZ = GetNodeForClassDefinition(objectZ, nodes);
    var nodeForA = GetNodeForClassDefinition(objectA, nodes);
    var nodeForB = GetNodeForClassDefinition(objectB, nodes);
    var nodeForC = GetNodeForClassDefinition(objectC, nodes);

    var expectedBrokenEdge1 = GetEdgeForProperty(nameof(AutomaticBreaksCountObjectZ.AutomaticBreakCountZPropA), nodeForZ);
    var expectedBrokenEdge2 = GetEdgeForProperty(nameof(AutomaticBreaksCountObjectZ.AutomaticBreakCountTPropB), nodeForZ);

    SetupNodeFactoryMock(nodes);

    Assert.That(nodeForA.HasBrokenEdges, Is.False);
    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);
    Assert.That(nodeForZ.HasBrokenEdges, Is.False);

    var provider = new GraphBasedPersistenceModelSortingProvider(_nodeFactoryStrictMock.Object);
    provider.Initialize([]);

    Assert.That(nodeForA.HasBrokenEdges, Is.False);
    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);

    Assert.That(nodeForZ.HasBrokenEdges, Is.True);
    Assert.That(nodeForZ.BrokenEdges.Count, Is.EqualTo(2));
    Assert.That(nodeForZ.BrokenEdges.Contains(expectedBrokenEdge1), Is.True);
    Assert.That(nodeForZ.BrokenEdges.Contains(expectedBrokenEdge2), Is.True);

    AssertPositionAndCorrectOrder(
        provider,
        [
            (objectZ, false), // Z is broken therefore first
            (objectA, true),  // A and C has no other dependencies than Z and then they are ordered by their table name
            (objectC, true),  // therefore A is second and C is third
            (objectB, true)   // B has dependency on C therefore third
        ]
    );

    AssertForeignKeyProperties(provider, objectA, [nameof(AutomaticBreaksCountObjectA.AutomaticBreakCountAPropZ)]);
    AssertForeignKeyProperties(provider, objectB, [nameof(AutomaticBreaksCountObjectB.AutomaticBreakCountBPropC)]);
    AssertForeignKeyProperties(provider, objectC, [nameof(AutomaticBreaksCountObjectC.AutomaticBreakCountCPropZ)]);
    AssertForeignKeyProperties(provider, objectZ, [nameof(AutomaticBreaksCountObjectZ.AutomaticBreakCountZPropA), nameof(AutomaticBreaksCountObjectZ.AutomaticBreakCountTPropB)]);
  }

  private void AssertPositionAndCorrectOrder (GraphBasedPersistenceModelSortingProvider provider, params (ClassDefinition classDefinition, bool isOrderedCorrect)[] expectedValues)
  {
    for (var i = 0; i < expectedValues.Length; i++)
    {
      var expectedValue = expectedValues[i];
      var tableDefinition = GetTableDefinition(expectedValue.classDefinition);
      Assert.That(provider.GetSortPosition(tableDefinition), Is.EqualTo(i));
      Assert.That(provider.HasBeenSortedCorrectly(tableDefinition), Is.EqualTo(expectedValue.isOrderedCorrect));
    }
  }

  private void AssertForeignKeyProperties (GraphBasedPersistenceModelSortingProvider provider, ClassDefinition classDefinition, string[] expectedProps)
  {
    var actualProps = provider.GetForeignKeyRelevantPropertyDefinitions(classDefinition);
    Assert.That(actualProps.Count, Is.EqualTo(expectedProps.Length));

    var expectedPropertyDefinitions = expectedProps.Select(e => GetPropertyDefinition(classDefinition, e));
    Assert.That(actualProps, Is.EquivalentTo(expectedPropertyDefinitions));
  }

  private SortingOptimizationNode GetNodeForClassDefinition (ClassDefinition classDefinition, IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var tableDefinition = GetTableDefinition(classDefinition);
    return nodes.Single(n => n.TableDefinition == tableDefinition);
  }

  private SortingOptimizationEdge GetEdgeForProperty (string propertyName, SortingOptimizationNode node)
  {
    return node.Edges.Single(e => e.ForeignKeys.Any(f => f.ReferencingColumns.Any(c => c.Name == propertyName + "ID")));
  }

  private PropertyDefinition GetPropertyDefinition (ClassDefinition classDefinition, string propertyName)
  {
    return classDefinition.GetPropertyDefinitions().First(p => p.PropertyName.EndsWith("." + propertyName));
  }

  private void SetupNodeFactoryMock (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    _nodeFactoryStrictMock.Setup(stub => stub.CreateNodes(It.IsAny<IReadOnlyList<ClassDefinition>>())).Returns(nodes);
  }
}
