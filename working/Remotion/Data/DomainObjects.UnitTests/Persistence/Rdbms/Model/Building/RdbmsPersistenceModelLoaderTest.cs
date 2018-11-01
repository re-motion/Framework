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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderTest
  {
    private string _storageProviderID;
    private StorageProviderDefinition _storageProviderDefinition;

    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;
    private IRdbmsStorageEntityDefinitionFactory _entityDefinitionFactoryMock;

    private IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactoryMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;
    private IStorageNameProvider _storageNameProviderStub;

    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionBaseBase;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionBase;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionTable1;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionTable2;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionDerived1;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionDerived2;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionDerivedDerived;
    private IRdbmsStorageEntityDefinition _fakeEntityDefinitionDerivedDerivedDerived;

    private SimpleStoragePropertyDefinition _fakeColumnDefinition1;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition2;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition3;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition4;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition5;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition6;
    private SimpleStoragePropertyDefinition _fakeColumnDefinition7;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition (_storageProviderID);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _entityDefinitionFactoryMock = MockRepository.GenerateStrictMock<IRdbmsStorageEntityDefinitionFactory>();
      _dataStoragePropertyDefinitionFactoryMock = MockRepository.GenerateStrictMock<IDataStoragePropertyDefinitionFactory>();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_testModel.TableClassDefinition1)).Return (
          new EntityNameDefinition (null, _testModel.TableClassDefinition1.ID));
      _storageNameProviderStub.Stub (stub => stub.GetTableName (_testModel.TableClassDefinition2)).Return (
          new EntityNameDefinition (null, _testModel.TableClassDefinition2.ID));

      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader (
          _entityDefinitionFactoryMock,
          _dataStoragePropertyDefinitionFactoryMock,
          _storageNameProviderStub,
          new RdbmsPersistenceModelProvider());

      _fakeEntityDefinitionBaseBase = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionBase = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionTable1 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionTable2 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerived1 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerived2 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerivedDerived = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerivedDerivedDerived = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition>();

      _fakeColumnDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test1");
      _fakeColumnDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test2");
      _fakeColumnDefinition3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test3");
      _fakeColumnDefinition4 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test4");
      _fakeColumnDefinition5 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test5");
      _fakeColumnDefinition6 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test6");
      _fakeColumnDefinition7 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test7");
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      var validator =
          (PersistenceMappingValidator) _rdbmsPersistenceModelLoader.CreatePersistenceMappingValidator (_testModel.BaseBaseClassDefinition);

      Assert.That (validator.ValidationRules.Count, Is.EqualTo (5));
      Assert.That (validator.ValidationRules[0], Is.TypeOf (typeof (OnlyOneTablePerHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[1], Is.TypeOf (typeof (TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule)));
      Assert.That (validator.ValidationRules[2], Is.TypeOf (typeof (ClassAboveTableIsAbstractValidationRule)));
      Assert.That (validator.ValidationRules[3], Is.TypeOf (typeof (ColumnNamesAreUniqueWithinInheritanceTreeValidationRule)));
      Assert.That (validator.ValidationRules[4], Is.TypeOf (typeof (PropertyTypeIsSupportedByStorageProviderValidationRule)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesEntities_AndProperties_ForClassAndDerivedClasses ()
    {
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.BaseBasePropertyDefinition))
          .Return (_fakeColumnDefinition1);
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.BasePropertyDefinition))
          .Return (_fakeColumnDefinition2);
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.TablePropertyDefinition1))
          .Return (_fakeColumnDefinition3);
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.TablePropertyDefinition2))
          .Return (_fakeColumnDefinition4);
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.DerivedPropertyDefinition1))
          .Return (_fakeColumnDefinition5);
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.DerivedPropertyDefinition2))
          .Return (_fakeColumnDefinition6);
      _dataStoragePropertyDefinitionFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (_testModel.DerivedDerivedPropertyDefinition))
          .Return (_fakeColumnDefinition7);
      _dataStoragePropertyDefinitionFactoryMock.Replay();

      _entityDefinitionFactoryMock
          .Expect (
              mock => mock.CreateUnionViewDefinition (
                  Arg.Is (_testModel.BaseBaseClassDefinition),
                  Arg<IEnumerable<IRdbmsStorageEntityDefinition>>.List.Equal (new[] { _fakeEntityDefinitionBase })))
          .Return (_fakeEntityDefinitionBaseBase);
      _entityDefinitionFactoryMock
          .Expect (
              mock => mock.CreateUnionViewDefinition (
                  Arg.Is (_testModel.BaseClassDefinition),
                  Arg<IEnumerable<IRdbmsStorageEntityDefinition>>.List.Equal (new[] { _fakeEntityDefinitionTable1, _fakeEntityDefinitionTable2 })))
          .Return (_fakeEntityDefinitionBase);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateTableDefinition (_testModel.TableClassDefinition1))
          .Return (_fakeEntityDefinitionTable1);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateTableDefinition (_testModel.TableClassDefinition2))
          .Return (_fakeEntityDefinitionTable2);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedClassDefinition1, _fakeEntityDefinitionTable2))
          .Return (_fakeEntityDefinitionDerived1);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedClassDefinition2, _fakeEntityDefinitionTable2))
          .Return (_fakeEntityDefinitionDerived2);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedDerivedClassDefinition, _fakeEntityDefinitionDerived2))
          .Return (_fakeEntityDefinitionDerivedDerived);
      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateFilterViewDefinition (_testModel.DerivedDerivedDerivedClassDefinition, _fakeEntityDefinitionDerivedDerived))
          .Return (_fakeEntityDefinitionDerivedDerivedDerived);
      _entityDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.BaseBaseClassDefinition);

      _dataStoragePropertyDefinitionFactoryMock.VerifyAllExpectations();
      _entityDefinitionFactoryMock.VerifyAllExpectations();

      Assert.That (_testModel.BaseBaseClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionBaseBase));
      Assert.That (_testModel.BaseClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionBase));
      Assert.That (_testModel.TableClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable1));
      Assert.That (_testModel.TableClassDefinition2.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable2));
      Assert.That (_testModel.DerivedClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerived1));
      Assert.That (_testModel.DerivedClassDefinition2.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerived2));
      Assert.That (_testModel.DerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerivedDerived));
      Assert.That (_testModel.DerivedDerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionDerivedDerivedDerived));

      Assert.That (_testModel.BaseBasePropertyDefinition.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition1));
      Assert.That (_testModel.BasePropertyDefinition.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition2));
      Assert.That (_testModel.TablePropertyDefinition1.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition3));
      Assert.That (_testModel.TablePropertyDefinition2.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition4));
      Assert.That (_testModel.DerivedPropertyDefinition1.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition5));
      Assert.That (_testModel.DerivedPropertyDefinition2.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition6));
      Assert.That (_testModel.DerivedDerivedPropertyDefinition.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition7));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_LeavesExistingEntities_AndProperties ()
    {
      _testModel.TableClassDefinition1.SetStorageEntity (_fakeEntityDefinitionTable1);
      _testModel.TablePropertyDefinition1.SetStorageProperty (_fakeColumnDefinition1);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.TableClassDefinition1);

      _dataStoragePropertyDefinitionFactoryMock.VerifyAllExpectations();
      _entityDefinitionFactoryMock.VerifyAllExpectations();

      Assert.That (_testModel.TableClassDefinition1.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable1));
      Assert.That (_testModel.TablePropertyDefinition1.StoragePropertyDefinition, Is.SameAs (_fakeColumnDefinition1));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesEmptyView_ForAbstractClass_WithAbstractDerivedClass_WithoutConcreteDerivations ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (AbstractClassWithoutDerivations), baseClass: null);
      var derivedClass = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Distributor), baseClass: classDefinition);

      derivedClass.SetStorageEntity (EmptyViewDefinitionObjectMother.Create (_storageProviderDefinition));
      classDefinition.SetDerivedClasses (new[] { derivedClass });
      derivedClass.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
      derivedClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      _entityDefinitionFactoryMock
          .Expect (mock => mock.CreateEmptyViewDefinition (classDefinition))
          .Return (_fakeEntityDefinitionTable1);
      _entityDefinitionFactoryMock.Replay();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (classDefinition);

      _entityDefinitionFactoryMock.VerifyAllExpectations();
      Assert.That (classDefinition.StorageEntityDefinition, Is.SameAs (_fakeEntityDefinitionTable1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The storage entity definition of class 'Derived2Class' does not implement interface 'IRdbmsStorageEntityDefinition'.")]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();
      _testModel.DerivedClassDefinition2.SetStorageEntity (invalidStorageEntityDefinition);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (_fakeColumnDefinition7);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedDerivedClassDefinition);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The property definition 'DerivedDerivedProperty' of class 'DerivedDerivedClass' does not implement interface 'IRdbmsStoragePropertyDefinition'."
        )]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingPropertyDefinitionDoesNotImplementIColumnDefinition ()
    {
      _testModel.DerivedClassDefinition2.SetStorageEntity (_fakeEntityDefinitionDerived2);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty (new FakeStoragePropertyDefinition ("Fake"));

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy (_testModel.DerivedDerivedClassDefinition);
    }
  }
}