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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.UnitTests.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class ValueStoragePropertyDefinitionFactoryTest
  {
    private IStorageTypeInformationProvider _storageTypeInformationProviderMock;
    private IStorageNameProvider _storageNameProviderStub;

    private ValueStoragePropertyDefinitionFactory _factory;
    
    private ClassDefinition _someClassDefinition;
    private ClassDefinition _someClassDefinitionWithoutBaseClass;
    private ClassDefinition _someClassDefinitionWithBaseClass;
    private ClassDefinition _someClassDefinitionWithBaseBaseClass;

    private StorageTypeInformation _fakeStorageTypeInformation1;
    private StorageTypeInformation _fakeStorageTypeInformation2;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationProviderMock = MockRepository.GenerateStrictMock<IStorageTypeInformationProvider> ();
      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();

      _factory = new ValueStoragePropertyDefinitionFactory (_storageTypeInformationProviderMock, _storageNameProviderStub);

      _someClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();

      _someClassDefinitionWithoutBaseClass = ClassDefinitionObjectMother.CreateClassDefinition ();
      _someClassDefinitionWithBaseClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "some", baseClass: _someClassDefinitionWithoutBaseClass);
      _someClassDefinitionWithBaseBaseClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "some", baseClass: _someClassDefinitionWithBaseClass);

      _fakeStorageTypeInformation1 = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
      _fakeStorageTypeInformation2 = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_NotSupportedType ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinition, "Test");
      var exception = new NotSupportedException ("Msg.");
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (Arg.Is (propertyDefinition), Arg<bool>.Is.Anything))
          .Throw (exception);

      var result = _factory.CreateStoragePropertyDefinition (propertyDefinition);

      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      Assert.That (
          result,
          Is.TypeOf<UnsupportedStoragePropertyDefinition>()
              .With.Property<UnsupportedStoragePropertyDefinition> (pd => pd.Message).EqualTo (
                  "There was an error when retrieving storage type for property 'Test' (declaring class: '" + _someClassDefinition.ID + "'): Msg.")
              .And.Property<UnsupportedStoragePropertyDefinition> (pd => pd.PropertyType).EqualTo (typeof (string))
              .And.Property<UnsupportedStoragePropertyDefinition> (pd => pd.InnerException).SameAs (exception));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinition);
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinition, false))
          .Return (_fakeStorageTypeInformation1);

      _storageNameProviderStub
        .Stub (stub => stub.GetColumnName (propertyDefinition))
        .Return ("FakeColumnName");

      var result = _factory.CreateStoragePropertyDefinition (propertyDefinition);

      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      CheckSimplePropertyDefinition (result, typeof (string), "FakeColumnName", _fakeStorageTypeInformation1);
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_RespectsNullability_ForClassAboveDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithoutBaseClass, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithoutBaseClass, true);

      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithoutBaseClass))
          .Return (null);
      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithoutBaseClass.BaseClass))
          .Return (null);

      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, false))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, false))
          .Return (_fakeStorageTypeInformation2);

      _storageNameProviderStub.Stub (stub => stub.GetColumnName (Arg<PropertyDefinition>.Is.Anything)).Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderMock.VerifyAllExpectations ();

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_RespectsNullability_ForClassWithDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithBaseClass, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithBaseClass, true);

      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseClass))
          .Return (new EntityNameDefinition (null, "some"));
      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseClass.BaseClass))
          .Return (null);

      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, false))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, false))
          .Return (_fakeStorageTypeInformation2);

      _storageNameProviderStub.Stub (stub => stub.GetColumnName (Arg<PropertyDefinition>.Is.Anything)).Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_OverridesNullability_ForClassBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithBaseClass, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithBaseClass, true);

      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseClass))
          .Return (null);
      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseClass.BaseClass))
          .Return (new EntityNameDefinition (null, "some"));

      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, true))
          .Return (_fakeStorageTypeInformation2);

      _storageNameProviderStub.Stub (stub => stub.GetColumnName (Arg<PropertyDefinition>.Is.Anything)).Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderMock.VerifyAllExpectations ();

      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_OverridesNullability_ForClassBelowBelowDbTableAttribute ()
    {
      var propertyDefinitionNotNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithBaseBaseClass, false);
      var propertyDefinitionNullable = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_someClassDefinitionWithBaseBaseClass, true);

      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseBaseClass))
          .Return (null);
      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseBaseClass.BaseClass))
          .Return (null);
      _storageNameProviderStub
          .Stub (stub => stub.GetTableName (_someClassDefinitionWithBaseBaseClass.BaseClass.BaseClass))
          .Return (new EntityNameDefinition (null, "some"));

      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNotNullable, true))
          .Return (_fakeStorageTypeInformation1);
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType (propertyDefinitionNullable, true))
          .Return (_fakeStorageTypeInformation2);

      _storageNameProviderStub.Stub (stub => stub.GetColumnName (Arg<PropertyDefinition>.Is.Anything)).Return ("FakeColumnName");

      var resultNotNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNotNullable);
      var resultNullable =
          (SimpleStoragePropertyDefinition) _factory.CreateStoragePropertyDefinition (propertyDefinitionNullable);

      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      Assert.That (resultNotNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation1));
      Assert.That (resultNullable.ColumnDefinition.StorageTypeInfo, Is.SameAs (_fakeStorageTypeInformation2));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_NotSupportedValue ()
    {
      var exception = new NotSupportedException ("Msg.");
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType ("Test"))
          .Throw (exception);

      var result = _factory.CreateStoragePropertyDefinition ("Test", "Column");

      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<UnsupportedStoragePropertyDefinition> ()
          .With.Property<UnsupportedStoragePropertyDefinition> (pd => pd.Message).EqualTo (
            "There was an error when retrieving storage type for value of type 'String': Msg.")
          .And.Property<UnsupportedStoragePropertyDefinition> (pd => pd.PropertyType).EqualTo (typeof (string))
          .And.Property<UnsupportedStoragePropertyDefinition> (pd => pd.InnerException).SameAs (exception));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_NotSupportedValueNull ()
    {
      var exception = new NotSupportedException ("Msg.");
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType ((object) null))
          .Throw (exception);

      var result = _factory.CreateStoragePropertyDefinition (null, "Column");

      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<UnsupportedStoragePropertyDefinition> ()
          .With.Property<UnsupportedStoragePropertyDefinition> (pd => pd.Message).EqualTo (
            "There was an error when retrieving storage type for value of type '<null>': Msg.")
          .And.Property<UnsupportedStoragePropertyDefinition> (pd => pd.PropertyType).EqualTo (typeof (object))
          .And.Property<UnsupportedStoragePropertyDefinition> (pd => pd.InnerException).SameAs (exception));
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_Null ()
    {
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType ((object) null))
          .Return (_fakeStorageTypeInformation1);

      var result = _factory.CreateStoragePropertyDefinition (null, "Column");
      
      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      CheckSimplePropertyDefinition (result, typeof (object), "Column", _fakeStorageTypeInformation1);
    }

    [Test]
    public void CreateStoragePropertyDefinition_ForValue_SimpleValue ()
    {
      _storageTypeInformationProviderMock
          .Expect (mock => mock.GetStorageType ("Test"))
          .Return (_fakeStorageTypeInformation1);

      var result = _factory.CreateStoragePropertyDefinition ("Test", "Column");

      _storageTypeInformationProviderMock.VerifyAllExpectations ();
      CheckSimplePropertyDefinition (result, typeof (string), "Column", _fakeStorageTypeInformation1);
    }

    private void CheckSimplePropertyDefinition (
        IRdbmsStoragePropertyDefinition result,
        Type expectedPropertyType,
        string expectedColumnName,
        StorageTypeInformation expectedStorageTypeInformation)
    {
      Assert.That (result, Is.TypeOf (typeof (SimpleStoragePropertyDefinition)));
      Assert.That (result.PropertyType, Is.SameAs (expectedPropertyType));
      var column = StoragePropertyDefinitionTestHelper.GetSingleColumn (result);
      Assert.That (column.Name, Is.EqualTo (expectedColumnName));
      Assert.That (column.IsPartOfPrimaryKey, Is.False);
      Assert.That (column.StorageTypeInfo, Is.SameAs (expectedStorageTypeInformation));
    }
  }
}