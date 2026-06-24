// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class GraphBasedPersistenceModelSortingProviderTest : SortingOptimizationTestBase
{
  private Mock<ISortingOptimizationNodeFactory> _nodeFactoryStrictMock;
  private ISortingOptimizationNodeFactory _realNodeFactory;
  private FakeLogCollector _fakeLogCollector;
  private ILoggerFactory _loggerFactory;

  [SetUp]
  public void SetUp ()
  {
    _realNodeFactory = new SortingOptimizationNodeFactory(new DomainModelConstraintProvider());
    _nodeFactoryStrictMock = new Mock<ISortingOptimizationNodeFactory>(MockBehavior.Strict);

    _fakeLogCollector = new FakeLogCollector();
    var fakeLoggerProvider = new FakeLoggerProvider(_fakeLogCollector);
    _loggerFactory = new LoggerFactory([fakeLoggerProvider]);
  }

  [Test]
  public void Initialize_CreatesValidResult_WithoutAnyBreaks ()
  {
    var mappingConfiguration = GetMappingConfiguration(typeof(NoBreaksObjectA), typeof(NoBreaksObjectB), typeof(NoBreaksObjectC));

    var objectA = mappingConfiguration.GetTypeDefinition(typeof(NoBreaksObjectA));
    var objectB = mappingConfiguration.GetTypeDefinition(typeof(NoBreaksObjectB));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(NoBreaksObjectC));

    var provider = new GraphBasedPersistenceModelSortingProvider(_realNodeFactory, _loggerFactory);
    provider.Initialize([objectC, objectB, objectA]);

    AssertLogMessage(_fakeLogCollector.GetSnapshot(), []);

    AssertPositionAndCorrectOrder(
        provider,
        (objectA, true),
        (objectC, true),
        (objectB, true));

    Assert.That(provider.GetPropertySpecificationsForForeignKeyProperties(objectA).Count, Is.EqualTo(0));

    AssertPropertySpecifications(
        provider,
        new ExpectedPropertySpecification(objectB, GetPropertyInfo<NoBreaksObjectB>(o => o.NoBreakBPropA), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectB, GetPropertyInfo<NoBreaksObjectB>(o => o.NoBreakBPropC), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectC, GetPropertyInfo<NoBreaksObjectC>(o => o.NoBreakCPropA), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectC, GetPropertyInfo<NoBreaksObjectC>(o => o.NoBreakCPropAWithoutForeignKey), false, ForeignKeyCycleBreakHint.Automatic)
    );
  }

  [Test]
  public void Initialize_CreatesValidResult_WithAutomaticBreaks_BreakIsDeterminedByTableName ()
  {
    var mappingConfiguration = GetMappingConfiguration(typeof(AutomaticBreaksTableNameObjectA), typeof(AutomaticBreaksTableNameObjectB), typeof(AutomaticBreaksTableNameObjectC));

    var objectA = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksTableNameObjectA));
    var objectB = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksTableNameObjectB));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksTableNameObjectC));

    var nodes = _realNodeFactory.CreateNodes([objectB, objectA, objectC]);

    var nodeForA = GetNodeForClassDefinition(objectA, nodes);
    var nodeForB = GetNodeForClassDefinition(objectB, nodes);
    var nodeForC = GetNodeForClassDefinition(objectC, nodes);
    var expectedBrokenEdge = GetEdgeForProperty(nameof(AutomaticBreaksTableNameObjectA.AutomaticBreakTableNameBPropB), nodeForA);

    SetupNodeFactoryMock(nodes);

    var provider = new GraphBasedPersistenceModelSortingProvider(_nodeFactoryStrictMock.Object, _loggerFactory);
    provider.Initialize([]);

    var expectedLogMessage = """
                             Persistence dependency cycles with breaks:
                               CYCLEBREAK (Automatic) -> 1. AutomaticBreaksTableNameObjectA.AutomaticBreakTableNameBPropBID -> AutomaticBreaksTableNameObjectB.ID
                                                         2. AutomaticBreaksTableNameObjectB.AutomaticBreakTableNameBPropCID -> AutomaticBreaksTableNameObjectC.ID
                                                         3. AutomaticBreaksTableNameObjectC.AutomaticBreakTableNameCPropAID -> AutomaticBreaksTableNameObjectA.ID

                             """.ReplaceLineEndings();
    AssertLogMessage(_fakeLogCollector.GetSnapshot(), [(expectedLogMessage, LogLevel.Information)]);

    Assert.That(nodeForA.HasBrokenEdges, Is.True);
    Assert.That(nodeForA.BrokenEdges.Count, Is.EqualTo(1));
    Assert.That(nodeForA.BrokenEdges.Contains(expectedBrokenEdge), Is.True);

    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);

    AssertPositionAndCorrectOrder(
        provider,
        (objectA, false),
        (objectC, true),
        (objectB, true));

    AssertPropertySpecifications(
        provider,
        new ExpectedPropertySpecification(objectA, GetPropertyInfo<AutomaticBreaksTableNameObjectA>(o => o.AutomaticBreakTableNameBPropB), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectB, GetPropertyInfo<AutomaticBreaksTableNameObjectB>(o => o.AutomaticBreakTableNameBPropC), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectC, GetPropertyInfo<AutomaticBreaksTableNameObjectC>(o => o.AutomaticBreakTableNameCPropA), true, ForeignKeyCycleBreakHint.Automatic)
    );
  }

  [Test]
  public void Initialize_CreatesValidResult_WitAutomaticBreaks_BreakIsDeterminedByCountOfOccurence ()
  {
    var mappingConfiguration = GetMappingConfiguration(
        typeof(AutomaticBreaksCountObjectZ),
        typeof(AutomaticBreaksCountObjectA),
        typeof(AutomaticBreaksCountObjectB),
        typeof(AutomaticBreaksCountObjectC));

    var objectZ = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectZ));
    var objectA = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectA));
    var objectB = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectB));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(AutomaticBreaksCountObjectC));

    var nodes = _realNodeFactory.CreateNodes([objectZ, objectC, objectA, objectB]);

    var nodeForZ = GetNodeForClassDefinition(objectZ, nodes);
    var nodeForA = GetNodeForClassDefinition(objectA, nodes);
    var nodeForB = GetNodeForClassDefinition(objectB, nodes);
    var nodeForC = GetNodeForClassDefinition(objectC, nodes);

    var expectedBrokenEdge1 = GetEdgeForProperty(nameof(AutomaticBreaksCountObjectZ.AutomaticBreakCountZPropA), nodeForZ);
    var expectedBrokenEdge2 = GetEdgeForProperty(nameof(AutomaticBreaksCountObjectZ.AutomaticBreakCountZPropB), nodeForZ);

    SetupNodeFactoryMock(nodes);

    var provider = new GraphBasedPersistenceModelSortingProvider(_nodeFactoryStrictMock.Object, _loggerFactory);

    provider.Initialize([]);

    var expectedLogMessage = """
                              Persistence dependency cycles with breaks:
                                CYCLEBREAK (Automatic) -> 1. AutomaticBreaksCountObjectZ.AutomaticBreakCountZPropAID -> AutomaticBreaksCountObjectA.ID
                                                          2. AutomaticBreaksCountObjectA.AutomaticBreakCountAPropZID -> AutomaticBreaksCountObjectZ.ID

                                CYCLEBREAK (Automatic) -> 1. AutomaticBreaksCountObjectZ.AutomaticBreakCountZPropBID -> AutomaticBreaksCountObjectB.ID
                                                          2. AutomaticBreaksCountObjectB.AutomaticBreakCountBPropCID -> AutomaticBreaksCountObjectC.ID
                                                          3. AutomaticBreaksCountObjectC.AutomaticBreakCountCPropZID -> AutomaticBreaksCountObjectZ.ID

                              """.ReplaceLineEndings();
    AssertLogMessage(_fakeLogCollector.GetSnapshot(), [(expectedLogMessage, LogLevel.Information)]);

    Assert.That(nodeForA.HasBrokenEdges, Is.False);
    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);

    Assert.That(nodeForZ.HasBrokenEdges, Is.True);
    Assert.That(nodeForZ.BrokenEdges.Count, Is.EqualTo(2));
    Assert.That(nodeForZ.BrokenEdges.Contains(expectedBrokenEdge1), Is.True);
    Assert.That(nodeForZ.BrokenEdges.Contains(expectedBrokenEdge2), Is.True);

    AssertPositionAndCorrectOrder(
        provider,
        (objectZ, false),
        (objectA, true),
        (objectC, true),
        (objectB, true));

    AssertPropertySpecifications(
        provider,
        new ExpectedPropertySpecification(objectA, GetPropertyInfo<AutomaticBreaksCountObjectA>(o => o.AutomaticBreakCountAPropZ), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectB, GetPropertyInfo<AutomaticBreaksCountObjectB>(o => o.AutomaticBreakCountBPropC), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectC, GetPropertyInfo<AutomaticBreaksCountObjectC>(o => o.AutomaticBreakCountCPropZ), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectZ, GetPropertyInfo<AutomaticBreaksCountObjectZ>(o => o.AutomaticBreakCountZPropA), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectZ, GetPropertyInfo<AutomaticBreaksCountObjectZ>(o => o.AutomaticBreakCountZPropB), true, ForeignKeyCycleBreakHint.Automatic)
    );
  }

  [Test]
  public void Initialize_CreatesValidResult_WithAutomaticBreaks_BreakIsDeterminedByBreakHints ()
  {
    var mappingConfiguration = GetMappingConfiguration(
        typeof(BreaksOnEdgeWithHintObjectA),
        typeof(BreaksOnEdgeWithHintObjectB),
        typeof(BreaksOnEdgeWithHintObjectC),
        typeof(BreaksOnEdgeWithHintObjectZ));

    var objectZ = mappingConfiguration.GetTypeDefinition(typeof(BreaksOnEdgeWithHintObjectZ));
    var objectA = mappingConfiguration.GetTypeDefinition(typeof(BreaksOnEdgeWithHintObjectA));
    var objectB = mappingConfiguration.GetTypeDefinition(typeof(BreaksOnEdgeWithHintObjectB));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(BreaksOnEdgeWithHintObjectC));

    var nodes = _realNodeFactory.CreateNodes([objectZ, objectC, objectB, objectA]);

    var nodeForZ = GetNodeForClassDefinition(objectZ, nodes);
    var nodeForA = GetNodeForClassDefinition(objectA, nodes);
    var nodeForB = GetNodeForClassDefinition(objectB, nodes);
    var nodeForC = GetNodeForClassDefinition(objectC, nodes);

    var expectedBrokenEdge1 = GetEdgeForProperty(nameof(BreaksOnEdgeWithHintObjectA.AutomaticBreakCountAPropZ), nodeForA);
    var expectedBrokenEdge2 = GetEdgeForProperty(nameof(BreaksOnEdgeWithHintObjectA.AutomaticBreakCountAPropB), nodeForA);

    SetupNodeFactoryMock(nodes);

    var provider = new GraphBasedPersistenceModelSortingProvider(_nodeFactoryStrictMock.Object, _loggerFactory);
    provider.Initialize([]);

    var expectedLogMessage = """
                             Persistence dependency cycles with breaks:
                               CYCLEBREAK (AlwaysBreak) -> 1. BreaksOnEdgeWithHintObjectA.AutomaticBreakCountAPropBID -> BreaksOnEdgeWithHintObjectB.ID
                                                           2. BreaksOnEdgeWithHintObjectB.AutomaticBreakCountBPropCID -> BreaksOnEdgeWithHintObjectC.ID
                                                           3. BreaksOnEdgeWithHintObjectC.AutomaticBreakCountCPropZID -> BreaksOnEdgeWithHintObjectZ.ID
                                                           4. BreaksOnEdgeWithHintObjectZ.AutomaticBreakCountZPropA1ID -> BreaksOnEdgeWithHintObjectA.ID

                               CYCLEBREAK (PreferredBreak) -> 1. BreaksOnEdgeWithHintObjectA.AutomaticBreakCountAPropZID -> BreaksOnEdgeWithHintObjectZ.ID
                                                              2. BreaksOnEdgeWithHintObjectZ.AutomaticBreakCountZPropA1ID -> BreaksOnEdgeWithHintObjectA.ID

                             """.ReplaceLineEndings();
    AssertLogMessage(_fakeLogCollector.GetSnapshot(), [(expectedLogMessage, LogLevel.Information)]);

    Assert.That(nodeForA.HasBrokenEdges, Is.True);
    Assert.That(nodeForA.BrokenEdges.Count, Is.EqualTo(2));
    Assert.That(nodeForA.BrokenEdges.Contains(expectedBrokenEdge1), Is.True);
    Assert.That(nodeForA.BrokenEdges.Contains(expectedBrokenEdge2), Is.True);

    Assert.That(nodeForB.HasBrokenEdges, Is.False);
    Assert.That(nodeForC.HasBrokenEdges, Is.False);
    Assert.That(nodeForZ.HasBrokenEdges, Is.False);

    AssertPositionAndCorrectOrder(
        provider,
        (objectA, false),
        (objectZ, true),
        (objectC, true),
        (objectB, true));

    AssertPropertySpecifications(
        provider,
        new ExpectedPropertySpecification(objectA, GetPropertyInfo<BreaksOnEdgeWithHintObjectA>(o => o.AutomaticBreakCountAPropZ), true, ForeignKeyCycleBreakHint.PreferredBreak),
        new ExpectedPropertySpecification(objectA, GetPropertyInfo<BreaksOnEdgeWithHintObjectA>(o => o.AutomaticBreakCountAPropB), true, ForeignKeyCycleBreakHint.AlwaysBreak),
        new ExpectedPropertySpecification(objectB, GetPropertyInfo<BreaksOnEdgeWithHintObjectB>(o => o.AutomaticBreakCountBPropC), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectC, GetPropertyInfo<BreaksOnEdgeWithHintObjectC>(o => o.AutomaticBreakCountCPropZ), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectZ, GetPropertyInfo<BreaksOnEdgeWithHintObjectZ>(o => o.AutomaticBreakCountZPropA1), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectZ, GetPropertyInfo<BreaksOnEdgeWithHintObjectZ>(o => o.AutomaticBreakCountZPropA2), true, ForeignKeyCycleBreakHint.Automatic),
        new ExpectedPropertySpecification(objectZ, GetPropertyInfo<BreaksOnEdgeWithHintObjectZ>(o => o.AutomaticBreakCountZPropA3), true, ForeignKeyCycleBreakHint.Automatic)
    );
  }

  [Test]
  public void Initialize_ThrowsException_WhenBreaksCouldNotBeDetermined ()
  {
    var mappingConfiguration = GetMappingConfiguration(typeof(PreventBreaksObjectA), typeof(PreventBreaksObjectB), typeof(PreventBreaksObjectC));

    var objectA = mappingConfiguration.GetTypeDefinition(typeof(PreventBreaksObjectA));
    var objectB = mappingConfiguration.GetTypeDefinition(typeof(PreventBreaksObjectB));
    var objectC = mappingConfiguration.GetTypeDefinition(typeof(PreventBreaksObjectC));

    var provider = new GraphBasedPersistenceModelSortingProvider(_realNodeFactory, _loggerFactory);
    Assert.That(
        () => provider.Initialize([objectA, objectB, objectC]),
        Throws.InvalidOperationException.With.Message.EqualTo(
            $"""
             {nameof(GraphBasedPersistenceModelSortingProvider)} could not resolve indirect cyclic dependencies.
             Table: PreventBreaksObjectA
               0. PreventBreaksObjectA.PreventBreakAPropBID -> PreventBreaksObjectB.ID
               1. PreventBreaksObjectB.PreventBreakBPropCID -> PreventBreaksObjectC.ID
               2. PreventBreaksObjectC.PreventBreakCPropAID -> PreventBreaksObjectA.ID

             Table: PreventBreaksObjectB
               0. PreventBreaksObjectB.PreventBreakBPropCID -> PreventBreaksObjectC.ID
               1. PreventBreaksObjectC.PreventBreakCPropAID -> PreventBreaksObjectA.ID
               2. PreventBreaksObjectA.PreventBreakAPropBID -> PreventBreaksObjectB.ID

             Table: PreventBreaksObjectC
               0. PreventBreaksObjectC.PreventBreakCPropAID -> PreventBreaksObjectA.ID
               1. PreventBreaksObjectA.PreventBreakAPropBID -> PreventBreaksObjectB.ID
               2. PreventBreaksObjectB.PreventBreakBPropCID -> PreventBreaksObjectC.ID
             """.ReplaceLineEndings()));
  }

  private void AssertPositionAndCorrectOrder (GraphBasedPersistenceModelSortingProvider provider, params (ClassDefinition classDefinition, bool isOrderedCorrect)[] expectedValues)
  {
    for (var i = 0; i < expectedValues.Length; i++)
    {
      var expectedValue = expectedValues[i];
      var tableDefinition = GetTableDefinition(expectedValue.classDefinition);
      Assert.That(provider.GetSortPosition(tableDefinition), Is.EqualTo(i));
    }
  }

  private void AssertPropertySpecifications (GraphBasedPersistenceModelSortingProvider provider, params ExpectedPropertySpecification[] expectedProps)
  {
    var grouped = expectedProps.GroupBy(e => e.ClassDefinition).ToList();

    foreach (var group in grouped)
    {
      var actualProps = provider.GetPropertySpecificationsForForeignKeyProperties(group.Key);
      Assert.That(actualProps.Count, Is.EqualTo(group.Count()));
      AssertForeignKeyPropertySpecifications(group.Key, group, actualProps);
    }
  }

  private void AssertLogMessage (IReadOnlyList<FakeLogRecord> logRecordSnapshot, (string message, LogLevel logLevel)[] expectedEntries)
  {
    foreach (var expectedEntry in expectedEntries)
    {
      var matchingRecord = logRecordSnapshot.FirstOrDefault(r => r.Level == expectedEntry.logLevel && r.Message == expectedEntry.message);
      Assert.That(matchingRecord, Is.Not.Null, $"No log message found with level '{expectedEntry.logLevel}' and message '{expectedEntry.message}'");
    }
  }

  private SortingOptimizationNode GetNodeForClassDefinition (ClassDefinition classDefinition, IReadOnlyList<SortingOptimizationNode> nodes)
  {
    var tableDefinition = GetTableDefinition(classDefinition);
    return nodes.Single(n => n.TableDefinition == tableDefinition);
  }

  private SortingOptimizationEdge GetEdgeForProperty (string propertyName, SortingOptimizationNode node)
  {
    return node.Edges.Single(e => e.ForeignKey.ReferencingColumns.Any(c => c.Name == propertyName + "ID"));
  }

  private void SetupNodeFactoryMock (IReadOnlyList<SortingOptimizationNode> nodes)
  {
    _nodeFactoryStrictMock.Setup(stub => stub.CreateNodes(It.IsAny<IReadOnlyList<ClassDefinition>>())).Returns(nodes);
  }
}
