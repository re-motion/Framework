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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsStorageEntityDefinitionFactoryTest
  {
    private string _storageProviderID;
    private RdbmsStorageEntityDefinitionFactory _factory;
    private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProviderMock;
    private IStoragePropertyDefinitionResolver _storagePropertyDefinitionResolverMock;
    private IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactoryMock;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private SimpleStoragePropertyDefinition _fakeStorageProperty1;
    private SimpleStoragePropertyDefinition _fakeTimestampStorageProperty;
    private ObjectIDStoragePropertyDefinition _fakeObjectIDStorageProperty;
    private ForeignKeyConstraintDefinition _fakeForeignKeyConstraint;
    private IStorageNameProvider _storageNameProviderMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _infrastructureStoragePropertyDefinitionProviderMock = MockRepository.GenerateStrictMock<IInfrastructureStoragePropertyDefinitionProvider>();
      _storagePropertyDefinitionResolverMock = MockRepository.GenerateStrictMock<IStoragePropertyDefinitionResolver>();
      _storageNameProviderMock = MockRepository.GenerateStrictMock<IStorageNameProvider>();
      _foreignKeyConstraintDefinitionFactoryMock = MockRepository.GenerateStrictMock<IForeignKeyConstraintDefinitionFactory>();
      _factory = new RdbmsStorageEntityDefinitionFactory (
          _infrastructureStoragePropertyDefinitionProviderMock,
          _foreignKeyConstraintDefinitionFactoryMock,
          _storagePropertyDefinitionResolverMock,
          _storageNameProviderMock,
          _storageProviderDefinition);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _fakeObjectIDStorageProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _fakeStorageProperty1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test1");
      _fakeTimestampStorageProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;

      _fakeForeignKeyConstraint = new ForeignKeyConstraintDefinition (
          "FakeForeignKeyConstraint",
          new EntityNameDefinition (null, "Test"),
          new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_fakeObjectIDStorageProperty) },
          new[] { _fakeStorageProperty1.ColumnDefinition });
    }

    [Test]
    public void CreateTableDefinition ()
    {
      _storagePropertyDefinitionResolverMock
          .Expect (mock => mock.GetStoragePropertiesForHierarchy (_testModel.TableClassDefinition1))
          .Return (new[] { _fakeStorageProperty1 });
      _storagePropertyDefinitionResolverMock.Replay();

      _foreignKeyConstraintDefinitionFactoryMock
          .Expect (mock => mock.CreateForeignKeyConstraints (_testModel.TableClassDefinition1))
          .Return (new[] { _fakeForeignKeyConstraint });
      _foreignKeyConstraintDefinitionFactoryMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (_testModel.TableClassDefinition1))
          .Return (new EntityNameDefinition(null, "FakeTableName"));
      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.TableClassDefinition1))
          .Return (new EntityNameDefinition(null, "FakeViewName"));
      _storageNameProviderMock
          .Expect (mock => mock.GetPrimaryKeyConstraintName (_testModel.TableClassDefinition1))
          .Return ("FakePrimaryKeyName");
      _storageNameProviderMock.Replay();

      MockStandardProperties();
      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateTableDefinition (_testModel.TableClassDefinition1);

      _storagePropertyDefinitionResolverMock.VerifyAllExpectations();
      _foreignKeyConstraintDefinitionFactoryMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckTableDefinition (
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
              new PrimaryKeyConstraintDefinition (
                  "FakePrimaryKeyName",
                  true,
                  new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_fakeObjectIDStorageProperty) }),
              _fakeForeignKeyConstraint
          },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CreateTableDefinition_WithoutPrimaryKey ()
    {
      _storagePropertyDefinitionResolverMock
          .Expect (mock => mock.GetStoragePropertiesForHierarchy (_testModel.TableClassDefinition1))
          .Return (new IRdbmsStoragePropertyDefinition[0]);
      _storagePropertyDefinitionResolverMock.Replay ();

      _foreignKeyConstraintDefinitionFactoryMock
          .Expect (mock => mock.CreateForeignKeyConstraints (_testModel.TableClassDefinition1))
          .Return (new ForeignKeyConstraintDefinition[0]);
      _foreignKeyConstraintDefinitionFactoryMock.Replay ();

      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (_testModel.TableClassDefinition1))
          .Return (new EntityNameDefinition(null, "FakeTableName"));
      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.TableClassDefinition1))
          .Return (new EntityNameDefinition(null, "FakeViewName"));
      _storageNameProviderMock.Replay ();

      // no primary key columns!
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetObjectIDStoragePropertyDefinition ())
          .Return (new ObjectIDStoragePropertyDefinition (
              SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty (),
              SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ()));
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetTimestampStoragePropertyDefinition ())
          .Return (_fakeTimestampStorageProperty);

      _infrastructureStoragePropertyDefinitionProviderMock.Replay ();

      var result = _factory.CreateTableDefinition (_testModel.TableClassDefinition1);

      Assert.That (result, Is.TypeOf<TableDefinition>().With.Property ("Constraints").Empty);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Table1Class' has no table name defined.")]
    public void CreateTableDefinition_ClassHasNoDBTableAttributeDefined ()
    {
      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (_testModel.TableClassDefinition1))
          .Return (null);
      _storageNameProviderMock.Replay();

      _factory.CreateTableDefinition (_testModel.TableClassDefinition1);
    }

    [Test]
    public void CreateFilterViewDefinition_DerivedClassWithoutDerivations ()
    {
      var fakeBaseEntityDefiniton = TableDefinitionObjectMother.Create (_storageProviderDefinition);

      _storagePropertyDefinitionResolverMock
          .Expect (mock => mock.GetStoragePropertiesForHierarchy (_testModel.DerivedClassDefinition1))
          .Return (new[] { _fakeStorageProperty1 });
      _storagePropertyDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.DerivedClassDefinition1))
          .Return (new EntityNameDefinition(null, "FakeViewName"));
      _storageNameProviderMock.Replay();

      MockStandardProperties();
      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_testModel.DerivedClassDefinition1, fakeBaseEntityDefiniton);

      _storagePropertyDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckFilterViewDefinition (
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
      var fakeBaseEntityDefiniton = TableDefinitionObjectMother.Create (_storageProviderDefinition);

      _storagePropertyDefinitionResolverMock
          .Expect (mock => mock.GetStoragePropertiesForHierarchy (_testModel.DerivedClassDefinition2))
          .Return (new[] { _fakeStorageProperty1 });
      _storagePropertyDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.DerivedClassDefinition2))
          .Return (new EntityNameDefinition(null, "FakeViewName"));
      _storageNameProviderMock.Replay();

      MockStandardProperties();
      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateFilterViewDefinition (_testModel.DerivedClassDefinition2, fakeBaseEntityDefiniton);

      _storagePropertyDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckFilterViewDefinition (
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
      var fakeUnionEntity1 = TableDefinitionObjectMother.Create (_storageProviderDefinition);
      var fakeUnionEntity2 = TableDefinitionObjectMother.Create (_storageProviderDefinition);

      _storagePropertyDefinitionResolverMock
          .Expect (mock => mock.GetStoragePropertiesForHierarchy (_testModel.BaseBaseClassDefinition))
          .Return (new[] { _fakeStorageProperty1 });
      _storagePropertyDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.BaseBaseClassDefinition))
          .Return (new EntityNameDefinition(null, "FakeViewName"));
      _storageNameProviderMock.Replay();

      MockStandardProperties();

      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateUnionViewDefinition (
          _testModel.BaseBaseClassDefinition, new IRdbmsStorageEntityDefinition[] { fakeUnionEntity1, fakeUnionEntity2 });

      _storagePropertyDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckUnionViewDefinition (
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
          .Expect (mock => mock.GetStoragePropertiesForHierarchy (_testModel.BaseBaseClassDefinition))
          .Return (new[] { _fakeStorageProperty1 });
      _storagePropertyDefinitionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetViewName (_testModel.BaseBaseClassDefinition))
          .Return (new EntityNameDefinition(null, "FakeViewName"));
      _storageNameProviderMock.Replay();

      MockStandardProperties();

      _infrastructureStoragePropertyDefinitionProviderMock.Replay();

      var result = _factory.CreateEmptyViewDefinition (_testModel.BaseBaseClassDefinition);

      _storagePropertyDefinitionResolverMock.VerifyAllExpectations();
      _infrastructureStoragePropertyDefinitionProviderMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      CheckEmptyViewDefinition (
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
      Assert.That (actualEntityDefinition, Is.TypeOf (typeof (TableDefinition)));
      Assert.That (((TableDefinition) actualEntityDefinition).TableName, Is.EqualTo (new EntityNameDefinition (null, expectedTableName)));
      CheckEntityDefinition (
          actualEntityDefinition,
          expectedStorageProviderID,
          expectedViewName,
          expectedStoragePropertyDefinitions,
          expectedIndexDefinitions,
          expectedSynonyms);

      var tableConstraints = ((TableDefinition) actualEntityDefinition).Constraints;
      Assert.That (tableConstraints.Count, Is.EqualTo (expectedTableConstraintDefinitions.Length));

      for (var i = 0; i < expectedTableConstraintDefinitions.Length; i++)
      {
        var expectedPrimaryKeyConstraint = expectedTableConstraintDefinitions[i] as PrimaryKeyConstraintDefinition;
        if (expectedPrimaryKeyConstraint != null)
          CheckPrimaryKeyConstraint (tableConstraints[i], expectedPrimaryKeyConstraint);
        else
          Assert.That (tableConstraints[i], Is.EqualTo (expectedTableConstraintDefinitions[i]));
      }
    }

    private void CheckPrimaryKeyConstraint (ITableConstraintDefinition actual, PrimaryKeyConstraintDefinition expected)
    {
      Assert.That (expected.ConstraintName, Is.EqualTo (actual.ConstraintName));
      Assert.That (actual, Is.TypeOf (typeof (PrimaryKeyConstraintDefinition)));
      Assert.That (((PrimaryKeyConstraintDefinition) actual).IsClustered, Is.EqualTo (expected.IsClustered));
      Assert.That (((PrimaryKeyConstraintDefinition) actual).Columns, Is.EqualTo (expected.Columns));
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
      Assert.That (actualEntityDefinition, Is.TypeOf (typeof (FilterViewDefinition)));
      Assert.That (((FilterViewDefinition) actualEntityDefinition).BaseEntity, Is.SameAs (expectedBaseEntity));
      Assert.That (((FilterViewDefinition) actualEntityDefinition).ClassIDs, Is.EqualTo (expectedClassIDs));

      CheckEntityDefinition (
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
      Assert.That (actualEntityDefinition, Is.TypeOf (typeof (UnionViewDefinition)));
      Assert.That (((UnionViewDefinition) actualEntityDefinition).UnionedEntities, Is.EqualTo (expectedStorageEntityDefinitions));

      CheckEntityDefinition (
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
      Assert.That (actualEntityDefinition, Is.TypeOf (typeof (EmptyViewDefinition)));

      CheckEntityDefinition (
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
      Assert.That (expectedEntityDefinition.StorageProviderID, Is.EqualTo (expectedStorageProviderID));
      Assert.That (expectedEntityDefinition.ViewName, Is.EqualTo (new EntityNameDefinition (null, expectedViewName)));
      Assert.That (expectedEntityDefinition.GetAllProperties (), Is.EqualTo (expectedStoragePropertyDefinitions));
      Assert.That (expectedEntityDefinition.Indexes, Is.EqualTo (expectedIndexDefinitions));
      Assert.That (expectedEntityDefinition.Synonyms, Is.EqualTo (expectedSynonyms));
    }

    private void MockStandardProperties ()
    {
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetObjectIDStoragePropertyDefinition())
          .Return (_fakeObjectIDStorageProperty);
      _infrastructureStoragePropertyDefinitionProviderMock
          .Expect (mock => mock.GetTimestampStoragePropertyDefinition())
          .Return (_fakeTimestampStorageProperty);
    }
  }
}