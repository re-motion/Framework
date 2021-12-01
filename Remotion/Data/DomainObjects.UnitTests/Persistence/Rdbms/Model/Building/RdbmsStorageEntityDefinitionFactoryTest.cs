// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsStorageEntityDefinitionFactoryTest
  {
    private string _storageProviderID;
    private RdbmsStorageEntityDefinitionFactory _factory;
    private Mock<IInfrastructureStoragePropertyDefinitionProvider> _infrastructureStoragePropertyDefinitionProviderMock;
    private Mock<IStoragePropertyDefinitionResolver> _storagePropertyDefinitionResolverMock;
    private Mock<IForeignKeyConstraintDefinitionFactory> _foreignKeyConstraintDefinitionFactoryMock;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private SimpleStoragePropertyDefinition _fakeStorageProperty1;
    private SimpleStoragePropertyDefinition _fakeTimestampStorageProperty;
    private ObjectIDStoragePropertyDefinition _fakeObjectIDStorageProperty;
    private ForeignKeyConstraintDefinition _fakeForeignKeyConstraint;
    private Mock<IStorageNameProvider> _storageNameProviderMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition(_storageProviderID);
      _infrastructureStoragePropertyDefinitionProviderMock = new Mock<IInfrastructureStoragePropertyDefinitionProvider> (MockBehavior.Strict);
      _storagePropertyDefinitionResolverMock = new Mock<IStoragePropertyDefinitionResolver> (MockBehavior.Strict);
      _storageNameProviderMock = new Mock<IStorageNameProvider> (MockBehavior.Strict);
      _foreignKeyConstraintDefinitionFactoryMock = new Mock<IForeignKeyConstraintDefinitionFactory> (MockBehavior.Strict);
      _factory = new RdbmsStorageEntityDefinitionFactory(
          _infrastructureStoragePropertyDefinitionProviderMock.Object,
          _foreignKeyConstraintDefinitionFactoryMock.Object,
          _storagePropertyDefinitionResolverMock.Object,
          _storageNameProviderMock.Object,
          _storageProviderDefinition);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _fakeObjectIDStorageProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _fakeStorageProperty1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test1");
      _fakeTimestampStorageProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;

      _fakeForeignKeyConstraint = new ForeignKeyConstraintDefinition(
          "FakeForeignKeyConstraint",
          new EntityNameDefinition(null, "Test"),
          new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_fakeObjectIDStorageProperty) },
          new[] { _fakeStorageProperty1.ColumnDefinition });
    }

    [Test]
    public void CreateTableDefinition ()
    {
      _storagePropertyDefinitionResolverMock
          .Setup(mock => mock.GetStoragePropertiesForHierarchy(_testModel.TableClassDefinition1))
          .Returns(new[] { _fakeStorageProperty1 })
          .Verifiable();

      _foreignKeyConstraintDefinitionFactoryMock
          .Setup(mock => mock.CreateForeignKeyConstraints(_testModel.TableClassDefinition1))
          .Returns(new[] { _fakeForeignKeyConstraint })
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetTableName(_testModel.TableClassDefinition1))
          .Returns(new EntityNameDefinition(null, "FakeTableName"))
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetViewName(_testModel.TableClassDefinition1))
          .Returns(new EntityNameDefinition(null, "FakeViewName"))
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetPrimaryKeyConstraintName(_testModel.TableClassDefinition1))
          .Returns("FakePrimaryKeyName")
          .Verifiable();

      MockStandardProperties();

      var result = _factory.CreateTableDefinition(_testModel.TableClassDefinition1);

      _storagePropertyDefinitionResolverMock.Verify();
      _foreignKeyConstraintDefinitionFactoryMock.Verify();
      _infrastructureStoragePropertyDefinitionProviderMock.Verify();
      _storageNameProviderMock.Verify();
      CheckTableDefinition(
          result,
          _storageProviderID,
          "FakeTableName",
          "FakeViewName",
          new IRdbmsStoragePropertyDefinition[]
          {
              _fakeObjectIDStorageProperty,
              _fakeTimestampStorageProperty,
              _fakeStorageProperty1
          },
          new ITableConstraintDefinition[]
          {
              new PrimaryKeyConstraintDefinition(
                  "FakePrimaryKeyName",
                  true,
                  new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_fakeObjectIDStorageProperty) }),
              _fakeForeignKeyConstraint
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateTableDefinition_WithoutPrimaryKey ()
    {
      _storagePropertyDefinitionResolverMock
          .Setup(mock => mock.GetStoragePropertiesForHierarchy(_testModel.TableClassDefinition1))
          .Returns(new IRdbmsStoragePropertyDefinition[0])
          .Verifiable();

      _foreignKeyConstraintDefinitionFactoryMock
          .Setup(mock => mock.CreateForeignKeyConstraints(_testModel.TableClassDefinition1))
          .Returns(new ForeignKeyConstraintDefinition[0])
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetTableName(_testModel.TableClassDefinition1))
          .Returns(new EntityNameDefinition(null, "FakeTableName"))
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetViewName(_testModel.TableClassDefinition1))
          .Returns(new EntityNameDefinition(null, "FakeViewName"))
          .Verifiable();

      // no primary key columns!
      _infrastructureStoragePropertyDefinitionProviderMock
          .Setup(mock => mock.GetObjectIDStoragePropertyDefinition())
          .Returns(new ObjectIDStoragePropertyDefinition(
              SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty(),
              SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty()))
          .Verifiable();
      _infrastructureStoragePropertyDefinitionProviderMock
          .Setup(mock => mock.GetTimestampStoragePropertyDefinition())
          .Returns(_fakeTimestampStorageProperty)
          .Verifiable();

      var result = _factory.CreateTableDefinition(_testModel.TableClassDefinition1);

      Assert.That(result, Is.TypeOf<TableDefinition>().With.Property("Constraints").Empty);
    }

    [Test]
    public void CreateTableDefinition_ClassHasNoDBTableAttributeDefined ()
    {
      _storageNameProviderMock
          .Setup(mock => mock.GetTableName(_testModel.TableClassDefinition1))
          .Returns((EntityNameDefinition) null)
          .Verifiable();
      Assert.That(
          () => _factory.CreateTableDefinition(_testModel.TableClassDefinition1),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo("Class 'Table1Class' has no table name defined."));
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithoutDerivations ()
    {
      var fakeBaseEntityDefiniton = TableDefinitionObjectMother.Create(_storageProviderDefinition);

      _storagePropertyDefinitionResolverMock
          .Setup(mock => mock.GetStoragePropertiesForHierarchy(_testModel.DerivedClassDefinition1))
          .Returns(new[] { _fakeStorageProperty1 })
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetViewName(_testModel.DerivedClassDefinition1))
          .Returns(new EntityNameDefinition(null, "FakeViewName"))
          .Verifiable();

      MockStandardProperties();

      var result = _factory.CreateFilterViewDefinition(_testModel.DerivedClassDefinition1, fakeBaseEntityDefiniton);

      _storagePropertyDefinitionResolverMock.Verify();
      _infrastructureStoragePropertyDefinitionProviderMock.Verify();
      _storageNameProviderMock.Verify();
      CheckFilterViewDefinition(
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived1Class" },
          new IRdbmsStoragePropertyDefinition[]
          {
              _fakeObjectIDStorageProperty,
              _fakeTimestampStorageProperty,
              _fakeStorageProperty1
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithDerivations ()
    {
      var fakeBaseEntityDefiniton = TableDefinitionObjectMother.Create(_storageProviderDefinition);

      _storagePropertyDefinitionResolverMock
          .Setup(mock => mock.GetStoragePropertiesForHierarchy(_testModel.DerivedClassDefinition2))
          .Returns(new[] { _fakeStorageProperty1 })
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetViewName(_testModel.DerivedClassDefinition2))
          .Returns(new EntityNameDefinition(null, "FakeViewName"))
          .Verifiable();

      MockStandardProperties();

      var result = _factory.CreateFilterViewDefinition(_testModel.DerivedClassDefinition2, fakeBaseEntityDefiniton);

      _storagePropertyDefinitionResolverMock.Verify();
      _infrastructureStoragePropertyDefinitionProviderMock.Verify();
      _storageNameProviderMock.Verify();
      CheckFilterViewDefinition(
          result,
          _storageProviderID,
          "FakeViewName",
          fakeBaseEntityDefiniton,
          new[] { "Derived2Class", "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new IRdbmsStoragePropertyDefinition[]
          {
              _fakeObjectIDStorageProperty,
              _fakeTimestampStorageProperty,
              _fakeStorageProperty1
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateUnionViewDefinition ()
    {
      var fakeUnionEntity1 = TableDefinitionObjectMother.Create(_storageProviderDefinition);
      var fakeUnionEntity2 = TableDefinitionObjectMother.Create(_storageProviderDefinition);

      _storagePropertyDefinitionResolverMock
          .Setup(mock => mock.GetStoragePropertiesForHierarchy(_testModel.BaseBaseClassDefinition))
          .Returns(new[] { _fakeStorageProperty1 })
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetViewName(_testModel.BaseBaseClassDefinition))
          .Returns(new EntityNameDefinition(null, "FakeViewName"))
          .Verifiable();

      MockStandardProperties();

      var result = _factory.CreateUnionViewDefinition(
          _testModel.BaseBaseClassDefinition, new IRdbmsStorageEntityDefinition[] { fakeUnionEntity1, fakeUnionEntity2 });

      _storagePropertyDefinitionResolverMock.Verify();
      _infrastructureStoragePropertyDefinitionProviderMock.Verify();
      _storageNameProviderMock.Verify();
      CheckUnionViewDefinition(
          result,
          _storageProviderID,
          "FakeViewName",
          new[] { fakeUnionEntity1, fakeUnionEntity2 },
          new IRdbmsStoragePropertyDefinition[]
          {
              _fakeObjectIDStorageProperty,
              _fakeTimestampStorageProperty,
              _fakeStorageProperty1
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateEmptyViewDefinition ()
    {
      _storagePropertyDefinitionResolverMock
          .Setup(mock => mock.GetStoragePropertiesForHierarchy(_testModel.BaseBaseClassDefinition))
          .Returns(new[] { _fakeStorageProperty1 })
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetViewName(_testModel.BaseBaseClassDefinition))
          .Returns(new EntityNameDefinition(null, "FakeViewName"))
          .Verifiable();

      MockStandardProperties();

      var result = _factory.CreateEmptyViewDefinition(_testModel.BaseBaseClassDefinition);

      _storagePropertyDefinitionResolverMock.Verify();
      _infrastructureStoragePropertyDefinitionProviderMock.Verify();
      _storageNameProviderMock.Verify();
      CheckEmptyViewDefinition(
          result,
          _storageProviderID,
          "FakeViewName",
          new IRdbmsStoragePropertyDefinition[]
          {
              _fakeObjectIDStorageProperty,
              _fakeTimestampStorageProperty,
              _fakeStorageProperty1
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    private void CheckTableDefinition (
        IRdbmsStorageEntityDefinition actualEntityDefinition,
        string expectedStorageProviderID,
        string expectedTableName,
        string expectedViewName,
        IRdbmsStoragePropertyDefinition[] expectedStoragePropertyDefinitions,
        ITableConstraintDefinition[] expectedTableConstraintDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That(actualEntityDefinition, Is.TypeOf(typeof(TableDefinition)));
      Assert.That(((TableDefinition)actualEntityDefinition).TableName, Is.EqualTo(new EntityNameDefinition(null, expectedTableName)));
      CheckEntityDefinition(
          actualEntityDefinition,
          expectedStorageProviderID,
          expectedViewName,
          expectedStoragePropertyDefinitions,
          expectedIndexDefinitions,
          expectedSynonyms);

      var tableConstraints = ((TableDefinition)actualEntityDefinition).Constraints;
      Assert.That(tableConstraints.Count, Is.EqualTo(expectedTableConstraintDefinitions.Length));

      for (var i = 0; i < expectedTableConstraintDefinitions.Length; i++)
      {
        var expectedPrimaryKeyConstraint = expectedTableConstraintDefinitions[i] as PrimaryKeyConstraintDefinition;
        if (expectedPrimaryKeyConstraint != null)
          CheckPrimaryKeyConstraint(tableConstraints[i], expectedPrimaryKeyConstraint);
        else
          Assert.That(tableConstraints[i], Is.EqualTo(expectedTableConstraintDefinitions[i]));
      }
    }

    private void CheckPrimaryKeyConstraint (ITableConstraintDefinition actual, PrimaryKeyConstraintDefinition expected)
    {
      Assert.That(expected.ConstraintName, Is.EqualTo(actual.ConstraintName));
      Assert.That(actual, Is.TypeOf(typeof(PrimaryKeyConstraintDefinition)));
      Assert.That(((PrimaryKeyConstraintDefinition)actual).IsClustered, Is.EqualTo(expected.IsClustered));
      Assert.That(((PrimaryKeyConstraintDefinition)actual).Columns, Is.EqualTo(expected.Columns));
    }

    private void CheckFilterViewDefinition (
        IRdbmsStorageEntityDefinition actualEntityDefinition,
        string expectedStorageProviderID,
        string expectedViewName,
        IStorageEntityDefinition expectedBaseEntity,
        string[] expectedClassIDs,
        IRdbmsStoragePropertyDefinition[] expectedStoragePropertyDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That(actualEntityDefinition, Is.TypeOf(typeof(FilterViewDefinition)));
      Assert.That(((FilterViewDefinition)actualEntityDefinition).BaseEntity, Is.SameAs(expectedBaseEntity));
      Assert.That(((FilterViewDefinition)actualEntityDefinition).ClassIDs, Is.EqualTo(expectedClassIDs));

      CheckEntityDefinition(
          actualEntityDefinition,
          expectedStorageProviderID,
          expectedViewName,
          expectedStoragePropertyDefinitions,
          expectedIndexDefinitions,
          expectedSynonyms);
    }

    private void CheckUnionViewDefinition (
        IRdbmsStorageEntityDefinition actualEntityDefinition,
        string expectedStorageProviderID,
        string expectedViewName,
        IStorageEntityDefinition[] expectedStorageEntityDefinitions,
        IRdbmsStoragePropertyDefinition[] expectedStoragePropertyDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That(actualEntityDefinition, Is.TypeOf(typeof(UnionViewDefinition)));
      Assert.That(((UnionViewDefinition)actualEntityDefinition).UnionedEntities, Is.EqualTo(expectedStorageEntityDefinitions));

      CheckEntityDefinition(
          actualEntityDefinition,
          expectedStorageProviderID,
          expectedViewName,
          expectedStoragePropertyDefinitions,
          expectedIndexDefinitions,
          expectedSynonyms);
    }

    private void CheckEmptyViewDefinition (
        IRdbmsStorageEntityDefinition actualEntityDefinition,
        string expectedStorageProviderID,
        string expectedViewName,
        IRdbmsStoragePropertyDefinition[] expectedStoragePropertyDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That(actualEntityDefinition, Is.TypeOf(typeof(EmptyViewDefinition)));

      CheckEntityDefinition(
          actualEntityDefinition,
          expectedStorageProviderID,
          expectedViewName,
          expectedStoragePropertyDefinitions,
          expectedIndexDefinitions,
          expectedSynonyms);
    }

    private void CheckEntityDefinition (
        IRdbmsStorageEntityDefinition expectedEntityDefinition,
        string expectedStorageProviderID,
        string expectedViewName,
        IRdbmsStoragePropertyDefinition[] expectedStoragePropertyDefinitions,
        IIndexDefinition[] expectedIndexDefinitions,
        EntityNameDefinition[] expectedSynonyms)
    {
      Assert.That(expectedEntityDefinition.StorageProviderID, Is.EqualTo(expectedStorageProviderID));
      Assert.That(expectedEntityDefinition.ViewName, Is.EqualTo(new EntityNameDefinition(null, expectedViewName)));
      Assert.That(expectedEntityDefinition.GetAllProperties(), Is.EqualTo(expectedStoragePropertyDefinitions));
      Assert.That(expectedEntityDefinition.Indexes, Is.EqualTo(expectedIndexDefinitions));
      Assert.That(expectedEntityDefinition.Synonyms, Is.EqualTo(expectedSynonyms));
    }

    private void MockStandardProperties ()
    {
      _infrastructureStoragePropertyDefinitionProviderMock
          .Setup(mock => mock.GetObjectIDStoragePropertyDefinition())
          .Returns(_fakeObjectIDStorageProperty)
          .Verifiable();
      _infrastructureStoragePropertyDefinitionProviderMock
          .Setup(mock => mock.GetTimestampStoragePropertyDefinition())
          .Returns(_fakeTimestampStorageProperty)
          .Verifiable();
    }
  }
}
