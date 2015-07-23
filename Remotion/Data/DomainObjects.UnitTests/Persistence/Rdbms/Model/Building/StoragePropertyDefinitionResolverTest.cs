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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class StoragePropertyDefinitionResolverTest
  {
    private IRdbmsPersistenceModelProvider _persistenceModelProviderStub;

    private StoragePropertyDefinitionResolver _resolver;

    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition1;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition2;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition3;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition4;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition5;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition6;
    private SimpleStoragePropertyDefinition _fakeStorageProperyDefinition7;
    private RdbmsPersistenceModelLoaderTestHelper _testModel;


    [SetUp]
    public void SetUp ()
    {
      _persistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _resolver = new StoragePropertyDefinitionResolver (_persistenceModelProviderStub);
      _testModel = new RdbmsPersistenceModelLoaderTestHelper ();
      
      _fakeStorageProperyDefinition1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Test1");
      _fakeStorageProperyDefinition2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test2");
      _fakeStorageProperyDefinition3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test3");
      _fakeStorageProperyDefinition4 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test4");
      _fakeStorageProperyDefinition5 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test5");
      _fakeStorageProperyDefinition6 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test6");
      _fakeStorageProperyDefinition7 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Test7");

      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.BaseBasePropertyDefinition))
          .Return (_fakeStorageProperyDefinition1);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.BaseBasePropertyDefinition))
          .Return (_fakeStorageProperyDefinition1);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.BasePropertyDefinition))
          .Return (_fakeStorageProperyDefinition2);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.TablePropertyDefinition1))
          .Return (_fakeStorageProperyDefinition3);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.TablePropertyDefinition2))
          .Return (_fakeStorageProperyDefinition4);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.DerivedPropertyDefinition1))
          .Return (_fakeStorageProperyDefinition5);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.DerivedPropertyDefinition2))
          .Return (_fakeStorageProperyDefinition6);
      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (_testModel.DerivedDerivedPropertyDefinition))
          .Return (_fakeStorageProperyDefinition7);
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_GetsPropertiesFromDerivedClasses_SortedFromBaseToDerived ()
    {
      var columns = _resolver.GetStoragePropertiesForHierarchy (_testModel.BaseBaseClassDefinition).ToArray();

      Assert.That (
          columns,
          Is.EqualTo (
              new[]
              {
                  _fakeStorageProperyDefinition1, 
                  _fakeStorageProperyDefinition2, 
                  _fakeStorageProperyDefinition3, 
                  _fakeStorageProperyDefinition4, 
                  _fakeStorageProperyDefinition5,
                  _fakeStorageProperyDefinition6, 
                  _fakeStorageProperyDefinition7
              }));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_AlsoGetsPropertiesFromBaseClasses_SortedFromBaseToDerived ()
    {
      var properties = _resolver.GetStoragePropertiesForHierarchy (_testModel.DerivedClassDefinition1).ToArray ();

      Assert.That (properties, Is.EqualTo (
          new[]
          {
              _fakeStorageProperyDefinition1,
              _fakeStorageProperyDefinition2, _fakeStorageProperyDefinition4, _fakeStorageProperyDefinition5
          }));
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_NonPersistentPropertiesAreFiltered ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      var nonPersistentProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition, "NonPersistentProperty", StorageClass.None);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { nonPersistentProperty }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      var properties = _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray ();

      Assert.That (properties, Is.Empty);
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_MultipleStoragePropertiesForSamePropertyInfo_AreUnified ()
    {
      var storageProperty1 = CreateStorageProperyDefinitionWithNameAndNullability ("Test1", false);
      var storageProperty2 = CreateStorageProperyDefinitionWithNameAndNullability ("Test1", false);
      var storageProperty3 = CreateStorageProperyDefinitionWithNameAndNullability ("Test1", true);

      var propertyInfo = CreateFakePropertyInfo (typeof (Order), "OrderNumber");
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      var propertyDefinition1 = CreatePropertyWithStubbedStorageProperty (classDefinition, storageProperty1, "P1", propertyInfo);
      var propertyDefinition2 = CreatePropertyWithStubbedStorageProperty (classDefinition, storageProperty2, "P2", propertyInfo);
      var propertyDefinition3 = CreatePropertyWithStubbedStorageProperty (classDefinition, storageProperty3, "P3", propertyInfo);
      classDefinition.SetPropertyDefinitions (
          new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2, propertyDefinition3 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      var properties = _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray ();

      Assert.That (properties, Has.Length.EqualTo (1)); // instead of 2 properties
      Assert.That (properties[0], Is.TypeOf<SimpleStoragePropertyDefinition> ());
      Assert.That (((SimpleStoragePropertyDefinition) properties[0]).ColumnDefinition.Name, Is.EqualTo ("Test1"));
      Assert.That (((SimpleStoragePropertyDefinition) properties[0]).ColumnDefinition.StorageTypeInfo.IsStorageTypeNullable, Is.True);
    }

    [Test]
    public void GetStoragePropertiesForHierarchy_MultipleStoragePropertiesForSamePropertyInfo_WithIncompatibilities_Throw ()
    {
      var storageProperty1 = CreateStorageProperyDefinitionWithNameAndNullability ("Test1", false);
      var storageProperty2 = CreateStorageProperyDefinitionWithNameAndNullability ("Test2", false);

      var propertyInfo = CreateFakePropertyInfo (typeof (Order), "OrderNumber");
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      var propertyDefinition1 = CreatePropertyWithStubbedStorageProperty (classDefinition, storageProperty1, "P1", propertyInfo);
      var propertyDefinition2 = CreatePropertyWithStubbedStorageProperty (classDefinition, storageProperty2, "P2", propertyInfo);
      classDefinition.SetPropertyDefinitions (
          new PropertyDefinitionCollection (new[] { propertyDefinition1, propertyDefinition2 }, true));
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);

      Assert.That (
          () => _resolver.GetStoragePropertiesForHierarchy (classDefinition).ToArray(),
          Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo (
                    "For property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber', storage properties with conflicting "
                    + "properties were created. This is not allowed, all storage properties for a .NET property must be equivalent. "
                    + "This error indicates a bug in the IValueStoragePropertyDefinitionFactor implementation."));
    }

    private static SimpleStoragePropertyDefinition CreateStorageProperyDefinitionWithNameAndNullability (string columnName, bool isStorageTypeNullable)
    {
      return SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty (
          columnName,
          storageTypeInformation: StorageTypeInformationObjectMother.CreateStorageTypeInformation (isStorageTypeNullable: isStorageTypeNullable));
    }

    private PropertyDefinition CreatePropertyWithStubbedStorageProperty (
        ClassDefinition classDefinition, IRdbmsStoragePropertyDefinition storagePropertyDefinition, string propertyName, IPropertyInformation propertyInfo)
    {
      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForPropertyInformation (classDefinition, propertyName, propertyInfo);

      _persistenceModelProviderStub
          .Stub (stub => stub.GetStoragePropertyDefinition (propertyDefinition))
          .Return (storagePropertyDefinition);
      return propertyDefinition;
    }

    private IPropertyInformation CreateFakePropertyInfo (Type declaringType, string propertyName)
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.Name).Return (propertyName);
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (declaringType));
      propertyInformationStub.Stub (_ => _.PropertyType).Return (typeof (string));
      return propertyInformationStub;
    }
  }
}