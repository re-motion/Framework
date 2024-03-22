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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderIntegrationTest : StandardMappingTest
  {
    private RdbmsProviderDefinition _storageProviderDefinition;

    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;

    private RdbmsPersistenceModelLoaderTestHelper _testModel;
    private SimpleStoragePropertyDefinition _fakeBaseBaseStoragePropertyDefinition;
    private SimpleStoragePropertyDefinition _fakeBaseStoragePropertyDefinition;
    private SimpleStoragePropertyDefinition _fakeTableStoragePropertyDefinition1;
    private SimpleStoragePropertyDefinition _fakeTableStoragePropertyDefinition2;
    private SimpleStoragePropertyDefinition _fakeDerivedStoragePropertyDefinition1;
    private SimpleStoragePropertyDefinition _fakeDerivedStoragePropertyDefinition2;
    private SimpleStoragePropertyDefinition _fakeDerivedDerivedStoragePropertyDefinition;
    private ObjectIDStoragePropertyDefinition _infrastructureObjectIDStoragePropertyDefinition;
    private SimpleStoragePropertyDefinition _infrastructureIDStoragePropertyDefinition;
    private SimpleStoragePropertyDefinition _infrastructureClassIDStoragePropertyDefinition;
    private SimpleStoragePropertyDefinition _infrastructureTimestampStoragePropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinition = (RdbmsProviderDefinition)StorageSettings.GetStorageProviderDefinition(storageGroupTypeOrNull: null);

      var factory = _storageProviderDefinition.Factory;
      _rdbmsPersistenceModelLoader = (RdbmsPersistenceModelLoader)factory.CreatePersistenceModelLoader(_storageProviderDefinition);

      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      var infrastructureStoragePropertyDefinitionProvider =
          ((RdbmsStorageEntityDefinitionFactory)_rdbmsPersistenceModelLoader.EntityDefinitionFactory).InfrastructureStoragePropertyDefinitionProvider;
      _infrastructureObjectIDStoragePropertyDefinition = infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition();
      _infrastructureIDStoragePropertyDefinition = (SimpleStoragePropertyDefinition)_infrastructureObjectIDStoragePropertyDefinition.ValueProperty;
      _infrastructureClassIDStoragePropertyDefinition = (SimpleStoragePropertyDefinition)_infrastructureObjectIDStoragePropertyDefinition.ClassIDProperty;
      _infrastructureTimestampStoragePropertyDefinition =
          (SimpleStoragePropertyDefinition)infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition();

      _fakeBaseBaseStoragePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("BaseBaseProperty");
      _fakeBaseStoragePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("BaseProperty");
      _fakeTableStoragePropertyDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("TableProperty1");
      _fakeTableStoragePropertyDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("TableProperty2");
      _fakeDerivedStoragePropertyDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("DerivedProperty1");
      _fakeDerivedStoragePropertyDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("DerivedProperty2");
      _fakeDerivedDerivedStoragePropertyDefinition = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("DerivedDerivedProperty");

      _testModel.BaseBasePropertyDefinition.SetStorageProperty(_fakeBaseBaseStoragePropertyDefinition);
      _testModel.BasePropertyDefinition.SetStorageProperty(_fakeBaseStoragePropertyDefinition);
      _testModel.TablePropertyDefinition1.SetStorageProperty(_fakeTableStoragePropertyDefinition1);
      _testModel.TablePropertyDefinition2.SetStorageProperty(_fakeTableStoragePropertyDefinition2);
      _testModel.DerivedPropertyDefinition1.SetStorageProperty(_fakeDerivedStoragePropertyDefinition1);
      _testModel.DerivedPropertyDefinition2.SetStorageProperty(_fakeDerivedStoragePropertyDefinition2);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty(_fakeDerivedDerivedStoragePropertyDefinition);
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseBaseClassDefinition ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.BaseBaseClassDefinition);

      AssertUnionViewDefinition(
          _testModel.BaseBaseClassDefinition,
          "BaseBaseClassView",
          new[] { _testModel.BaseClassDefinition.StorageEntityDefinition },
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition1.ColumnDefinition,
              _fakeTableStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition1.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedDerivedStoragePropertyDefinition.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_BaseClassDefinition ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.BaseClassDefinition);

      AssertUnionViewDefinition(
          _testModel.BaseClassDefinition,
          "BaseClassView",
          new[] { _testModel.TableClassDefinition1.StorageEntityDefinition, _testModel.TableClassDefinition2.StorageEntityDefinition },
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition1.ColumnDefinition,
              _fakeTableStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition1.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedDerivedStoragePropertyDefinition.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition1 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.TableClassDefinition1);

      AssertTableDefinition(
          _testModel.TableClassDefinition1,
          "Table1Class",
          "Table1ClassView",
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition1.ColumnDefinition
          },
          new PrimaryKeyConstraintDefinition("PK_Table1Class", true, _infrastructureObjectIDStoragePropertyDefinition.GetColumnsForComparison())
          );
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_TableClassDefinition2 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.TableClassDefinition2);

      AssertTableDefinition(
          _testModel.TableClassDefinition2,
          "Table2Class",
          "Table2ClassView",
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition1.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedDerivedStoragePropertyDefinition.ColumnDefinition
          },
          new PrimaryKeyConstraintDefinition("PK_Table2Class", true, _infrastructureObjectIDStoragePropertyDefinition.GetColumnsForComparison())
          );
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition1 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.DerivedClassDefinition1);

      AssertFilterViewDefinition(
          _testModel.DerivedClassDefinition1,
          "Derived1ClassView",
          _testModel.TableClassDefinition2.StorageEntityDefinition,
          new[] { "Derived1Class" },
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition1.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedClassDefinition2 ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.DerivedClassDefinition2);

      AssertFilterViewDefinition(
          _testModel.DerivedClassDefinition2,
          "Derived2ClassView",
          _testModel.TableClassDefinition2.StorageEntityDefinition,
          new[] { "Derived2Class", "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedDerivedStoragePropertyDefinition.ColumnDefinition
          });
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_DerivedDerivedClassDefinition ()
    {
      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.DerivedDerivedClassDefinition);

      AssertFilterViewDefinition(
          _testModel.DerivedDerivedClassDefinition,
          "DerivedDerivedClassView",
          _testModel.DerivedClassDefinition2.StorageEntityDefinition,
          new[] { "DerivedDerivedClass", "DerivedDerivedDerivedClass" },
          new[]
          {
              _infrastructureIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureClassIDStoragePropertyDefinition.ColumnDefinition,
              _infrastructureTimestampStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeBaseStoragePropertyDefinition.ColumnDefinition,
              _fakeTableStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedStoragePropertyDefinition2.ColumnDefinition,
              _fakeDerivedDerivedStoragePropertyDefinition.ColumnDefinition
          });
    }

    private void AssertTableDefinition (
        ClassDefinition classDefinition,
        string tableName,
        string viewName,
        ColumnDefinition[] columnDefinitions,
        PrimaryKeyConstraintDefinition primaryKeyConstraintDefinition)
    {
      Assert.That(classDefinition.StorageEntityDefinition, Is.TypeOf(typeof(TableDefinition)));
      Assert.That(classDefinition.StorageEntityDefinition.StorageProviderDefinition, Is.SameAs(_storageProviderDefinition));
      Assert.That(((TableDefinition)classDefinition.StorageEntityDefinition).TableName.EntityName, Is.EqualTo(tableName));
      Assert.That(((TableDefinition)classDefinition.StorageEntityDefinition).TableName.SchemaName, Is.Null);
      Assert.That(((TableDefinition)classDefinition.StorageEntityDefinition).ViewName.EntityName, Is.EqualTo(viewName));
      Assert.That(((TableDefinition)classDefinition.StorageEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That(((TableDefinition)classDefinition.StorageEntityDefinition).GetAllColumns(), Is.EqualTo(columnDefinitions));
      Assert.That(((TableDefinition)classDefinition.StorageEntityDefinition).Constraints[0], Is.TypeOf(typeof(PrimaryKeyConstraintDefinition)));
      var primaryKey = (PrimaryKeyConstraintDefinition)((TableDefinition)classDefinition.StorageEntityDefinition).Constraints[0];
      Assert.That(primaryKey.ConstraintName, Is.EqualTo(primaryKeyConstraintDefinition.ConstraintName));
      Assert.That(primaryKey.Columns, Is.EqualTo(primaryKeyConstraintDefinition.Columns));
    }

    private void AssertFilterViewDefinition (
        ClassDefinition classDefinition,
        string viewName,
        IStorageEntityDefinition baseEntity,
        string[] classIDs,
        ColumnDefinition[] columnDefinitions)
    {
      Assert.That(classDefinition.StorageEntityDefinition, Is.TypeOf(typeof(FilterViewDefinition)));
      Assert.That(classDefinition.StorageEntityDefinition.StorageProviderDefinition, Is.SameAs(_storageProviderDefinition));
      Assert.That(((FilterViewDefinition)classDefinition.StorageEntityDefinition).ViewName.EntityName, Is.EqualTo(viewName));
      Assert.That(((FilterViewDefinition)classDefinition.StorageEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That(((FilterViewDefinition)classDefinition.StorageEntityDefinition).BaseEntity, Is.SameAs(baseEntity));
      Assert.That(((FilterViewDefinition)classDefinition.StorageEntityDefinition).ClassIDs, Is.EqualTo(classIDs));
      Assert.That(((FilterViewDefinition)classDefinition.StorageEntityDefinition).GetAllColumns(), Is.EqualTo(columnDefinitions));
    }

    private void AssertUnionViewDefinition (
        ClassDefinition classDefinition,
        string viewName,
        IStorageEntityDefinition[] storageEntityDefinitions,
        ColumnDefinition[] columnDefinitions)
    {
      Assert.That(classDefinition.StorageEntityDefinition, Is.TypeOf(typeof(UnionViewDefinition)));
      Assert.That(classDefinition.StorageEntityDefinition.StorageProviderDefinition, Is.SameAs(_storageProviderDefinition));
      Assert.That(((UnionViewDefinition)classDefinition.StorageEntityDefinition).ViewName.EntityName, Is.EqualTo(viewName));
      Assert.That(((UnionViewDefinition)classDefinition.StorageEntityDefinition).ViewName.SchemaName, Is.Null);
      Assert.That(((UnionViewDefinition)classDefinition.StorageEntityDefinition).UnionedEntities, Is.EqualTo(storageEntityDefinitions));
      Assert.That(((UnionViewDefinition)classDefinition.StorageEntityDefinition).GetAllColumns(), Is.EqualTo(columnDefinitions));
    }
  }
}
