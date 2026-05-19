// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class SortingOptimizationNodeTest
{
  private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
  private TableDefinition _tableA;
  private TableDefinition _tableB;
  private TableDefinition _tableC;

  [SetUp]
  public void SetUp ()
  {
    _storageProviderDefinition = new UnitTestStorageProviderStubDefinition("SPID");

    _tableA = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "TableA"));
    _tableB = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "TableB"));
    _tableC = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "TableC"));
  }

  [Test]
  public void Initialize_WithMatchingClassDefinition_SetsProperties ()
  {
    var classDefinition = CreateClassDefinitionWithStorage("ClassA", _tableA);

    var node = new SortingOptimizationNode(_tableA, new[] { classDefinition });

    Assert.That(node.TableDefinition, Is.SameAs(_tableA));
    Assert.That(node.ClassDefinitions, Is.EquivalentTo(new[] { classDefinition }));
    Assert.That(node.TableName, Is.EqualTo("TableA"));
    Assert.That(node.Edges, Is.Empty);
    Assert.That(node.BrokenEdges, Is.Empty);
    Assert.That(node.IndirectSelfCyclicEdges, Is.Empty);
    Assert.That(node.HasBrokenEdges, Is.False);
    Assert.That(node.HasIndirectCyclicDependencies, Is.False);
  }

  [Test]
  public void Initialize_WithMultipleClassDefinitionsForSameTable_SetsClassDefinitions ()
  {
    var classDefinition1 = CreateClassDefinitionWithStorage("ClassA1", _tableA);
    var classDefinition2 = CreateClassDefinitionWithStorage("ClassA2", _tableA);

    var node = new SortingOptimizationNode(_tableA, new[] { classDefinition1, classDefinition2 });

    Assert.That(node.ClassDefinitions, Is.EquivalentTo(new[] { classDefinition1, classDefinition2 }));
  }

  [Test]
  public void Initialize_WithClassDefinitionUsingFilterViewOverMatchingTable_Succeeds ()
  {
    var filterView = FilterViewDefinitionObjectMother.Create(
        _storageProviderDefinition,
        new EntityNameDefinition(null, "FilterViewOverA"),
        _tableA);
    var classDefinition = CreateClassDefinitionWithStorage("ClassFilter", filterView);

    var node = new SortingOptimizationNode(_tableA, new[] { classDefinition });

    Assert.That(node.TableDefinition, Is.SameAs(_tableA));
    Assert.That(node.ClassDefinitions, Is.EquivalentTo(new[] { classDefinition }));
  }

  [Test]
  public void Initialize_WithNoClassDefinitions_Succeeds ()
  {
    var node = new SortingOptimizationNode(_tableA, Array.Empty<ClassDefinition>());

    Assert.That(node.TableDefinition, Is.SameAs(_tableA));
    Assert.That(node.ClassDefinitions, Is.Empty);
  }

  [Test]
  public void Initialize_TableDefinitionNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => new SortingOptimizationNode(null!, Array.Empty<ClassDefinition>()),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void Initialize_ClassDefinitionsNull_ThrowsArgumentNullException ()
  {
    Assert.That(
        () => new SortingOptimizationNode(_tableA, null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void Initialize_WithClassDefinitionPointingToDifferentTable_ThrowsArgumentException ()
  {
    var classDefinition = CreateClassDefinitionWithStorage("ClassB", _tableB);

    Assert.That(
        () => new SortingOptimizationNode(_tableA, new[] { classDefinition }),
        Throws.ArgumentException
            .With.Message.Contains("ClassB")
            .And.Message.Contains("TableA"));
  }

  [Test]
  public void Initialize_WithClassDefinitionUsingFilterViewOverDifferentTable_ThrowsArgumentException ()
  {
    var filterView = FilterViewDefinitionObjectMother.Create(
        _storageProviderDefinition,
        new EntityNameDefinition(null, "FilterViewOverB"),
        _tableB);
    var classDefinition = CreateClassDefinitionWithStorage("ClassFilter", filterView);

    Assert.That(
        () => new SortingOptimizationNode(_tableA, new[] { classDefinition }),
        Throws.ArgumentException
            .With.Message.Contains("ClassFilter")
            .And.Message.Contains("TableA"));
  }

  [Test]
  public void Initialize_WithClassDefinitionUsingUnionView_ThrowsArgumentException ()
  {
    var unionView = UnionViewDefinitionObjectMother.Create(
        _storageProviderDefinition,
        new EntityNameDefinition(null, "UnionViewOverA"),
        _tableA);
    var classDefinition = CreateClassDefinitionWithStorage("ClassUnion", unionView);

    Assert.That(
        () => new SortingOptimizationNode(_tableA, new[] { classDefinition }),
        Throws.ArgumentException
            .With.Message.Contains("ClassUnion")
            .And.Message.Contains("TableA"));
  }

  [Test]
  public void Initialize_WithClassDefinitionUsingEmptyView_ThrowsArgumentException ()
  {
    var emptyView = EmptyViewDefinitionObjectMother.Create(_storageProviderDefinition);
    var classDefinition = CreateClassDefinitionWithStorage("ClassEmpty", emptyView);

    Assert.That(
        () => new SortingOptimizationNode(_tableA, new[] { classDefinition }),
        Throws.ArgumentException
            .With.Message.Contains("ClassEmpty")
            .And.Message.Contains("TableA"));
  }

  [Test]
  public void TableName_ReturnsEntityNameOfTableDefinition ()
  {
    var table = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition("SomeSchema", "MyTable"));
    var node = new SortingOptimizationNode(table, Array.Empty<ClassDefinition>());

    Assert.That(node.TableName, Is.EqualTo("MyTable"));
  }

  [Test]
  public void AddEdge_NewPointingTo_CreatesAndAddsEdge ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var foreignKey = CreateForeignKey("FK_A_To_B", _tableB.TableName);

    nodeA.AddEdge(foreignKey, nodeB);

    Assert.That(nodeA.Edges.Count, Is.EqualTo(1));
    var edge = nodeA.Edges[0];
    Assert.That(edge.Owner, Is.SameAs(nodeA));
    Assert.That(edge.PointingTo, Is.SameAs(nodeB));
    Assert.That(edge.ForeignKey, Is.SameAs(foreignKey));
    Assert.That(edge.IsSelfCyclingEdge, Is.False);
  }

  [Test]
  public void AddEdge_PointingToSelf_CreatesSelfCyclingEdge ()
  {
    var nodeA = CreateNode(_tableA);
    var foreignKey = CreateForeignKey("FK_A_To_A", _tableA.TableName);

    nodeA.AddEdge(foreignKey, nodeA);

    Assert.That(nodeA.Edges.Count, Is.EqualTo(1));
    Assert.That(nodeA.Edges[0].IsSelfCyclingEdge, Is.True);
    Assert.That(nodeA.Edges[0].PointingTo, Is.SameAs(nodeA));
  }

  [Test]
  public void AddEdge_ExistingPointingTo_AddsForeignKeyToExistingEdge ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var foreignKey1 = CreateForeignKey("FK_A_To_B_1", _tableB.TableName);
    var foreignKey2 = CreateForeignKey("FK_A_To_B_2", _tableB.TableName);

    nodeA.AddEdge(foreignKey1, nodeB);
    nodeA.AddEdge(foreignKey2, nodeB);

    Assert.That(nodeA.Edges.Count, Is.EqualTo(2));
    var edge = nodeA.Edges[0];
    Assert.That(edge.PointingTo, Is.SameAs(nodeB));
    Assert.That(edge.ForeignKey, Is.SameAs(foreignKey1));

    var edge2 = nodeA.Edges[1];
    Assert.That(edge2.PointingTo, Is.SameAs(nodeB));
    Assert.That(edge2.ForeignKey, Is.SameAs(foreignKey2));
  }

  [Test]
  public void AddEdge_DifferentPointingTo_AddsSeparateEdges ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var nodeC = CreateNode(_tableC);
    var foreignKey1 = CreateForeignKey("FK_A_To_B", _tableB.TableName);
    var foreignKey2 = CreateForeignKey("FK_A_To_C", _tableC.TableName);

    nodeA.AddEdge(foreignKey1, nodeB);
    nodeA.AddEdge(foreignKey2, nodeC);

    Assert.That(nodeA.Edges.Count, Is.EqualTo(2));
    Assert.That(nodeA.Edges.Select(e => e.PointingTo), Is.EquivalentTo(new[] { nodeB, nodeC }));
  }

  [Test]
  public void AddEdge_ForeignKeyNull_ThrowsArgumentNullException ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);

    Assert.That(
        () => nodeA.AddEdge(null!, nodeB),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void AddEdge_PointingToNull_ThrowsArgumentNullException ()
  {
    var nodeA = CreateNode(_tableA);
    var foreignKey = CreateForeignKey("FK_A_To_B", _tableB.TableName);

    Assert.That(
        () => nodeA.AddEdge(foreignKey, null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void BreakEdge_MovesEdgeFromEdgesToBrokenEdges ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var foreignKey = CreateForeignKey("FK_A_To_B", _tableB.TableName);
    nodeA.AddEdge(foreignKey, nodeB);
    var edge = nodeA.Edges.Single();

    nodeA.BreakEdge(edge);

    Assert.That(nodeA.Edges, Is.Empty);
    Assert.That(nodeA.BrokenEdges, Is.EquivalentTo(new[] { edge }));
    Assert.That(nodeA.HasBrokenEdges, Is.True);
  }

  [Test]
  public void BreakEdge_DoesNotAffectOtherEdges ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var nodeC = CreateNode(_tableC);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_C", _tableC.TableName), nodeC);
    var edgeToBreak = nodeA.Edges.Single(e => e.PointingTo == nodeB);

    nodeA.BreakEdge(edgeToBreak);

    Assert.That(nodeA.Edges.Count, Is.EqualTo(1));
    Assert.That(nodeA.Edges[0].PointingTo, Is.SameAs(nodeC));
    Assert.That(nodeA.BrokenEdges, Is.EquivalentTo(new[] { edgeToBreak }));
  }

  [Test]
  public void BreakEdge_EdgeNull_ThrowsArgumentNullException ()
  {
    var nodeA = CreateNode(_tableA);

    Assert.That(
        () => nodeA.BreakEdge(null!),
        Throws.TypeOf<ArgumentNullException>());
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_NoEdges_NoCycles ()
  {
    var nodeA = CreateNode(_tableA);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.IndirectSelfCyclicEdges, Is.Empty);
    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.False);
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_LinearGraph_NoCycles ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var nodeC = CreateNode(_tableC);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_C", _tableC.TableName), nodeC);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.IndirectSelfCyclicEdges, Is.Empty);
    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.False);
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_DirectSelfCycle_IsIgnored ()
  {
    var nodeA = CreateNode(_tableA);
    var selfCyclingFk = CreateForeignKey("FK_A_Self", _tableA.TableName);
    nodeA.AddEdge(selfCyclingFk, nodeA);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.IndirectSelfCyclicEdges, Is.Empty);
    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.False);
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_TwoNodeCycle_RecordsCyclePath ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_A", _tableA.TableName), nodeA);
    var edgeAToB = nodeA.Edges.Single();
    var edgeBToA = nodeB.Edges.Single();

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.True);
    Assert.That(nodeA.IndirectSelfCyclicEdges.Count, Is.EqualTo(1));
    Assert.That(nodeA.IndirectSelfCyclicEdges.ContainsKey(edgeAToB), Is.True);
    Assert.That(nodeA.IndirectSelfCyclicEdges[edgeAToB], Is.EquivalentTo(new[] { edgeAToB, edgeBToA }));
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_ThreeNodeCycle_RecordsCyclePath ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var nodeC = CreateNode(_tableC);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_C", _tableC.TableName), nodeC);
    nodeC.AddEdge(CreateForeignKey("FK_C_To_A", _tableA.TableName), nodeA);
    var edgeAToB = nodeA.Edges.Single();
    var edgeBToC = nodeB.Edges.Single();
    var edgeCToA = nodeC.Edges.Single();

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.True);
    Assert.That(nodeA.IndirectSelfCyclicEdges.Count, Is.EqualTo(1));
    Assert.That(nodeA.IndirectSelfCyclicEdges.ContainsKey(edgeAToB), Is.True);
    Assert.That(nodeA.IndirectSelfCyclicEdges[edgeAToB], Is.EquivalentTo(new[] { edgeAToB, edgeBToC, edgeCToA }));
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_MultipleOutgoingEdgesCausingCycles_RecordsAllCycles ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var nodeC = CreateNode(_tableC);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_C", _tableC.TableName), nodeC);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_A", _tableA.TableName), nodeA);
    nodeC.AddEdge(CreateForeignKey("FK_C_To_A", _tableA.TableName), nodeA);
    var edgeAToB = nodeA.Edges.Single(e => e.PointingTo == nodeB);
    var edgeAToC = nodeA.Edges.Single(e => e.PointingTo == nodeC);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.True);
    Assert.That(nodeA.IndirectSelfCyclicEdges.Count, Is.EqualTo(2));
    Assert.That(nodeA.IndirectSelfCyclicEdges.Keys, Is.EquivalentTo(new[] { edgeAToB, edgeAToC }));
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_BrokenEdgesAreIgnored ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_A", _tableA.TableName), nodeA);
    var edgeBToA = nodeB.Edges.Single();
    nodeB.BreakEdge(edgeBToA);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.False);
    Assert.That(nodeA.IndirectSelfCyclicEdges, Is.Empty);
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_ClearsPreviousResultsBeforeRecalculating ()
  {
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_A", _tableA.TableName), nodeA);

    nodeA.CalculateIndirectCyclicDependencies();
    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.True);

    // Break the cycle by breaking the back-edge
    var edgeBToA = nodeB.Edges.Single();
    nodeB.BreakEdge(edgeBToA);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.False);
    Assert.That(nodeA.IndirectSelfCyclicEdges, Is.Empty);
  }

  [Test]
  public void CalculateIndirectCyclicDependencies_DoesNotConsiderCyclesNotInvolvingThisNode ()
  {
    // A → B; B → C; C → B (cycle between B and C, but not involving A)
    var nodeA = CreateNode(_tableA);
    var nodeB = CreateNode(_tableB);
    var nodeC = CreateNode(_tableC);
    nodeA.AddEdge(CreateForeignKey("FK_A_To_B", _tableB.TableName), nodeB);
    nodeB.AddEdge(CreateForeignKey("FK_B_To_C", _tableC.TableName), nodeC);
    nodeC.AddEdge(CreateForeignKey("FK_C_To_B", _tableB.TableName), nodeB);

    nodeA.CalculateIndirectCyclicDependencies();

    Assert.That(nodeA.HasIndirectCyclicDependencies, Is.False);
    Assert.That(nodeA.IndirectSelfCyclicEdges, Is.Empty);
  }

  private SortingOptimizationNode CreateNode (TableDefinition tableDefinition)
  {
    return new SortingOptimizationNode(tableDefinition, Array.Empty<ClassDefinition>());
  }

  private ClassDefinition CreateClassDefinitionWithStorage (string id, IStorageEntityDefinition storageEntityDefinition)
  {
    var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(id: id);
    classDefinition.SetStorageEntity(storageEntityDefinition);
    return classDefinition;
  }

  private ForeignKeyConstraintDefinition CreateForeignKey (string name, EntityNameDefinition referencedTableName)
  {
    var referencingColumn = ColumnDefinitionObjectMother.CreateColumn(name + "_Referencing");
    var referencedColumn = ColumnDefinitionObjectMother.CreateColumn(name + "_Referenced");
    return new ForeignKeyConstraintDefinition(
        name,
        referencedTableName,
        new[] { referencingColumn },
        new[] { referencedColumn },
        ForeignKeyCycleBreakHint.Automatic);
  }
}
