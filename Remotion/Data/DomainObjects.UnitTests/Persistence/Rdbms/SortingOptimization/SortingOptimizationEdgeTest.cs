// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class SortingOptimizationEdgeTest
{
  private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
  private TableDefinition _tableA;
  private TableDefinition _tableB;
  private SortingOptimizationNode _nodeA;
  private SortingOptimizationNode _nodeB;
  private ForeignKeyConstraintDefinition _foreignKey;

  [SetUp]
  public void SetUp ()
  {
    _storageProviderDefinition = new UnitTestStorageProviderStubDefinition("SPID");
    _tableA = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "TableA"));
    _tableB = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "TableB"));
    _nodeA = new SortingOptimizationNode(_tableA, []);
    _nodeB = new SortingOptimizationNode(_tableB, []);
    _foreignKey = CreateForeignKey("FK_A_To_B", _tableB.TableName);
  }

  [Test]
  public void Initialize_SetsProperties ()
  {
    var edge = new SortingOptimizationEdge(_foreignKey, _nodeA, _nodeB);

    Assert.That(edge.Owner, Is.SameAs(_nodeA));
    Assert.That(edge.PointingTo, Is.SameAs(_nodeB));
    Assert.That(edge.ForeignKey, Is.SameAs(_foreignKey));
    Assert.That(edge.IsSelfCyclingEdge, Is.False);
  }

  [Test]
  public void Initialize_OwnerEqualsPointingTo_IsSelfCyclingEdgeIsTrue ()
  {
    var edge = new SortingOptimizationEdge(_foreignKey, _nodeA, _nodeA);

    Assert.That(edge.Owner, Is.SameAs(_nodeA));
    Assert.That(edge.PointingTo, Is.SameAs(_nodeA));
    Assert.That(edge.IsSelfCyclingEdge, Is.True);
  }

  [Test]
  public void Initialize_OwnerDifferentFromPointingTo_IsSelfCyclingEdgeIsFalse ()
  {
    var edge = new SortingOptimizationEdge(_foreignKey, _nodeA, _nodeB);

    Assert.That(edge.IsSelfCyclingEdge, Is.False);
  }

  [Test]
  public void Initialize_ForeignKeysCollectionContainsConstructorArgument ()
  {
    var edge = new SortingOptimizationEdge(_foreignKey, _nodeA, _nodeB);

    Assert.That(edge.ForeignKey, Is.SameAs(_foreignKey));
  }

  [Test]
  public void Initialize_ForeignKeyNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => new SortingOptimizationEdge(null!, _nodeA, _nodeB),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void Initialize_OwnerNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => new SortingOptimizationEdge(_foreignKey, null!, _nodeB),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void Initialize_PointingToNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => new SortingOptimizationEdge(_foreignKey, _nodeA, null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void BreakEdge_DelegatesToOwner_MovingEdgeToBrokenEdges ()
  {
    // Use the node's AddEdge so the edge is tracked by its owner; otherwise
    // BreakEdge on the owner would not be able to remove it.
    _nodeA.AddEdge(_foreignKey, _nodeB);
    var edge = _nodeA.Edges.Single();

    edge.BreakEdge();

    Assert.That(_nodeA.Edges, Is.Empty);
    Assert.That(_nodeA.BrokenEdges, Is.EquivalentTo([edge]));
  }

  [Test]
  public void BreakEdge_DoesNotChangeOwnerOrPointingToOrForeignKeys ()
  {
    _nodeA.AddEdge(_foreignKey, _nodeB);
    var edge = _nodeA.Edges.Single();

    edge.BreakEdge();

    Assert.That(edge.Owner, Is.SameAs(_nodeA));
    Assert.That(edge.PointingTo, Is.SameAs(_nodeB));
    Assert.That(edge.ForeignKey, Is.SameAs(_foreignKey));
  }

  private static ForeignKeyConstraintDefinition CreateForeignKey (string name, EntityNameDefinition referencedTableName)
  {
    var referencingColumn = ColumnDefinitionObjectMother.CreateColumn(name + "_Referencing");
    var referencedColumn = ColumnDefinitionObjectMother.CreateColumn(name + "_Referenced");
    return new ForeignKeyConstraintDefinition(
        name,
        referencedTableName,
        [referencingColumn],
        [referencedColumn],
        ForeignKeyCycleBreakHint.Automatic);
  }
}
