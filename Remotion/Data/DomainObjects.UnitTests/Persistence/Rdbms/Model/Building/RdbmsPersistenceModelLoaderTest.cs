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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RdbmsPersistenceModelLoaderTest
  {
    private string _storageProviderID;
    private StorageProviderDefinition _storageProviderDefinition;

    private RdbmsPersistenceModelLoader _rdbmsPersistenceModelLoader;
    private Mock<IRdbmsStorageEntityDefinitionFactory> _entityDefinitionFactoryMock;

    private Mock<IDataStoragePropertyDefinitionFactory> _dataStoragePropertyDefinitionFactoryMock;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;
    private Mock<IStorageNameProvider> _storageNameProviderStub;

    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionBaseBase;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionBase;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionTable1;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionTable2;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionDerived1;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionDerived2;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionDerivedDerived;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinitionDerivedDerivedDerived;

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
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition(_storageProviderID);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper();

      _entityDefinitionFactoryMock = new Mock<IRdbmsStorageEntityDefinitionFactory>(MockBehavior.Strict);
      _dataStoragePropertyDefinitionFactoryMock = new Mock<IDataStoragePropertyDefinitionFactory>(MockBehavior.Strict);
      _storageNameProviderStub = new Mock<IStorageNameProvider>();
      _storageNameProviderStub
          .Setup(stub => stub.GetTableName(_testModel.TableClassDefinition1))
          .Returns(new EntityNameDefinition(null, _testModel.TableClassDefinition1.ID));
      _storageNameProviderStub
          .Setup(stub => stub.GetTableName(_testModel.TableClassDefinition2))
          .Returns(new EntityNameDefinition(null, _testModel.TableClassDefinition2.ID));

      _rdbmsPersistenceModelLoader = new RdbmsPersistenceModelLoader(
          _entityDefinitionFactoryMock.Object,
          _dataStoragePropertyDefinitionFactoryMock.Object,
          _storageNameProviderStub.Object,
          new RdbmsPersistenceModelProvider());

      _fakeEntityDefinitionBaseBase = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionBase = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionTable1 = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionTable2 = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerived1 = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerived2 = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerivedDerived = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinitionDerivedDerivedDerived = new Mock<IRdbmsStorageEntityDefinition>();

      _fakeColumnDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test1");
      _fakeColumnDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test2");
      _fakeColumnDefinition3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test3");
      _fakeColumnDefinition4 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test4");
      _fakeColumnDefinition5 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test5");
      _fakeColumnDefinition6 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test6");
      _fakeColumnDefinition7 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test7");
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      var validator =
          (PersistenceMappingValidator)_rdbmsPersistenceModelLoader.CreatePersistenceMappingValidator(_testModel.BaseBaseClassDefinition);

      Assert.That(validator.ValidationRules.Count, Is.EqualTo(6));
      Assert.That(validator.ValidationRules[0], Is.TypeOf(typeof(OnlyOneTablePerHierarchyValidationRule)));
      Assert.That(validator.ValidationRules[1], Is.TypeOf(typeof(TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule)));
      Assert.That(validator.ValidationRules[2], Is.TypeOf(typeof(ClassAboveTableIsAbstractValidationRule)));
      Assert.That(validator.ValidationRules[3], Is.TypeOf(typeof(ColumnNamesAreUniqueWithinInheritanceTreeValidationRule)));
      Assert.That(validator.ValidationRules[4], Is.TypeOf(typeof(PropertyTypeIsSupportedByStorageProviderValidationRule)));
      Assert.That(validator.ValidationRules[5], Is.TypeOf(typeof(RelationPropertyStorageClassMatchesReferencedClassDefinitionStorageClassValidationRule)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesEntities_AndProperties_ForClassAndDerivedClasses ()
    {
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.BaseBasePropertyDefinition))
          .Returns(_fakeColumnDefinition1)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.BasePropertyDefinition))
          .Returns(_fakeColumnDefinition2)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.TablePropertyDefinition1))
          .Returns(_fakeColumnDefinition3)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.TablePropertyDefinition2))
          .Returns(_fakeColumnDefinition4)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.DerivedPropertyDefinition1))
          .Returns(_fakeColumnDefinition5)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.DerivedPropertyDefinition2))
          .Returns(_fakeColumnDefinition6)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(_testModel.DerivedDerivedPropertyDefinition))
          .Returns(_fakeColumnDefinition7)
          .Verifiable();

      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateUnionViewDefinition(_testModel.BaseBaseClassDefinition, new[] { _fakeEntityDefinitionBase.Object }))
          .Returns(_fakeEntityDefinitionBaseBase.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateUnionViewDefinition(_testModel.BaseClassDefinition, new[] { _fakeEntityDefinitionTable1.Object, _fakeEntityDefinitionTable2.Object }))
          .Returns(_fakeEntityDefinitionBase.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateTableDefinition(_testModel.TableClassDefinition1))
          .Returns(_fakeEntityDefinitionTable1.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateTableDefinition(_testModel.TableClassDefinition2))
          .Returns(_fakeEntityDefinitionTable2.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateFilterViewDefinition(_testModel.DerivedClassDefinition1, _fakeEntityDefinitionTable2.Object))
          .Returns(_fakeEntityDefinitionDerived1.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateFilterViewDefinition(_testModel.DerivedClassDefinition2, _fakeEntityDefinitionTable2.Object))
          .Returns(_fakeEntityDefinitionDerived2.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateFilterViewDefinition(_testModel.DerivedDerivedClassDefinition, _fakeEntityDefinitionDerived2.Object))
          .Returns(_fakeEntityDefinitionDerivedDerived.Object)
          .Verifiable();
      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateFilterViewDefinition(_testModel.DerivedDerivedDerivedClassDefinition, _fakeEntityDefinitionDerivedDerived.Object))
          .Returns(_fakeEntityDefinitionDerivedDerivedDerived.Object)
          .Verifiable();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.BaseBaseClassDefinition);

      _dataStoragePropertyDefinitionFactoryMock.Verify();
      _entityDefinitionFactoryMock.Verify();

      Assert.That(_testModel.BaseBaseClassDefinition.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionBaseBase.Object));
      Assert.That(_testModel.BaseClassDefinition.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionBase.Object));
      Assert.That(_testModel.TableClassDefinition1.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionTable1.Object));
      Assert.That(_testModel.TableClassDefinition2.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionTable2.Object));
      Assert.That(_testModel.DerivedClassDefinition1.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionDerived1.Object));
      Assert.That(_testModel.DerivedClassDefinition2.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionDerived2.Object));
      Assert.That(_testModel.DerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionDerivedDerived.Object));
      Assert.That(_testModel.DerivedDerivedDerivedClassDefinition.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionDerivedDerivedDerived.Object));

      Assert.That(_testModel.BaseBasePropertyDefinition.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition1));
      Assert.That(_testModel.BasePropertyDefinition.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition2));
      Assert.That(_testModel.TablePropertyDefinition1.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition3));
      Assert.That(_testModel.TablePropertyDefinition2.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition4));
      Assert.That(_testModel.DerivedPropertyDefinition1.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition5));
      Assert.That(_testModel.DerivedPropertyDefinition2.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition6));
      Assert.That(_testModel.DerivedDerivedPropertyDefinition.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition7));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_LeavesExistingEntities_AndProperties ()
    {
      _testModel.TableClassDefinition1.SetStorageEntity(_fakeEntityDefinitionTable1.Object);
      _testModel.TablePropertyDefinition1.SetStorageProperty(_fakeColumnDefinition1);

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.TableClassDefinition1);

      _dataStoragePropertyDefinitionFactoryMock.Verify();
      _entityDefinitionFactoryMock.Verify();

      Assert.That(_testModel.TableClassDefinition1.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionTable1.Object));
      Assert.That(_testModel.TablePropertyDefinition1.StoragePropertyDefinition, Is.SameAs(_fakeColumnDefinition1));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesEmptyView_ForAbstractClass_WithAbstractDerivedClass_WithoutConcreteDerivations ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(AbstractClassWithoutDerivations), baseClass: null);
      var derivedClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Distributor), baseClass: classDefinition);

      derivedClass.SetStorageEntity(EmptyViewDefinitionObjectMother.Create(_storageProviderDefinition));
      classDefinition.SetDerivedClasses(new[] { derivedClass });
      derivedClass.SetDerivedClasses(new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection());

      _entityDefinitionFactoryMock
          .Setup(mock => mock.CreateEmptyViewDefinition(classDefinition))
          .Returns(_fakeEntityDefinitionTable1.Object)
          .Verifiable();

      _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(classDefinition);

      _entityDefinitionFactoryMock.Verify();
      Assert.That(classDefinition.StorageEntityDefinition, Is.SameAs(_fakeEntityDefinitionTable1.Object));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = new Mock<IStorageEntityDefinition>();
      _testModel.DerivedClassDefinition2.SetStorageEntity(invalidStorageEntityDefinition.Object);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty(_fakeColumnDefinition7);
      Assert.That(
          () => _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.DerivedDerivedClassDefinition),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The storage entity definition of class 'Derived2Class' does not implement interface 'IRdbmsStorageEntityDefinition'."));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingPropertyDefinitionDoesNotImplementIColumnDefinition ()
    {
      _testModel.DerivedClassDefinition2.SetStorageEntity(_fakeEntityDefinitionDerived2.Object);
      _testModel.DerivedDerivedPropertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("Fake"));
      Assert.That(
          () => _rdbmsPersistenceModelLoader.ApplyPersistenceModelToHierarchy(_testModel.DerivedDerivedClassDefinition),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The property definition 'DerivedDerivedProperty' of class 'DerivedDerivedClass' does not implement interface 'IRdbmsStoragePropertyDefinition'."));
    }
  }
}
