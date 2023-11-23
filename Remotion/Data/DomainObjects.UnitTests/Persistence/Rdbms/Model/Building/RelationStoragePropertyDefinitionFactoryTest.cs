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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class RelationStoragePropertyDefinitionFactoryTest : StandardMappingTest
  {
    private Mock<IStorageNameProvider> _storageNameProviderMock;
    private Mock<IStorageSettings> _storageSettingsStub;
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStrictMock;
    private RelationStoragePropertyDefinitionFactory _factory;

    private StorageTypeInformation _fakeStorageTypeInformation1;
    private StorageTypeInformation _fakeStorageTypeInformation2;

    public override void SetUp ()
    {
      base.SetUp();

      _storageNameProviderMock = new Mock<IStorageNameProvider>(MockBehavior.Strict);
      _storageSettingsStub = new Mock<IStorageSettings>();
      _storageTypeInformationProviderStrictMock = new Mock<IStorageTypeInformationProvider>(MockBehavior.Strict);

      _factory = new RelationStoragePropertyDefinitionFactory(TestDomainStorageProviderDefinition,
          false, _storageNameProviderMock.Object, _storageTypeInformationProviderStrictMock.Object, _storageSettingsStub.Object);

      _fakeStorageTypeInformation1 = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      _fakeStorageTypeInformation2 = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationToClassDefinitionWithoutHierarchy ()
    {
      var endPointDefinition = GetNonVirtualEndPointDefinition(typeof(ClassWithManySideRelationProperties), "BidirectionalOneToOne");
      var oppositeClassDefinition = endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition;
      Assert.That(oppositeClassDefinition.IsPartOfInheritanceHierarchy, Is.False);

      _storageTypeInformationProviderStrictMock
          .Setup(mock => mock.GetStorageTypeForID(true))
          .Returns(_fakeStorageTypeInformation1)
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetRelationColumnName(endPointDefinition))
          .Returns("FakeRelationColumnName")
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetRelationClassIDColumnName(endPointDefinition))
          .Returns("FakeRelationClassIDColumnName")
          .Verifiable();

      _storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(oppositeClassDefinition))
          .Returns(_factory.StorageProviderDefinition);

      var result = _factory.CreateStoragePropertyDefinition(endPointDefinition);

      _storageTypeInformationProviderStrictMock.Verify();
      _storageNameProviderMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(ObjectIDWithoutClassIDStoragePropertyDefinition)));

      var objectIDWithoutClassIDStorageProperty = ((ObjectIDWithoutClassIDStoragePropertyDefinition)result);
      Assert.That(objectIDWithoutClassIDStorageProperty.ValueProperty, Is.TypeOf(typeof(SimpleStoragePropertyDefinition)));

      var valueStoragePropertyDefinition = ((SimpleStoragePropertyDefinition)objectIDWithoutClassIDStorageProperty.ValueProperty);
      Assert.That(valueStoragePropertyDefinition.PropertyType, Is.SameAs(typeof(object)));
      Assert.That(valueStoragePropertyDefinition.ColumnDefinition.Name, Is.EqualTo("FakeRelationColumnName"));
      Assert.That(valueStoragePropertyDefinition.ColumnDefinition.StorageTypeInfo, Is.SameAs(_fakeStorageTypeInformation1));
      Assert.That(valueStoragePropertyDefinition.ColumnDefinition.IsPartOfPrimaryKey, Is.False);

      Assert.That(
          objectIDWithoutClassIDStorageProperty.ClassDefinition,
          Is.SameAs(endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition));
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationToClassDefinitionWithoutHierarchy_WithForceClassIDTrue ()
    {
      var endPointDefinition = GetNonVirtualEndPointDefinition(typeof(ClassWithManySideRelationProperties), "BidirectionalOneToOne");
      Assert.That(endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition.IsPartOfInheritanceHierarchy, Is.False);

      _storageTypeInformationProviderStrictMock
          .Setup(mock => mock.GetStorageTypeForID(true))
          .Returns(_fakeStorageTypeInformation1)
          .Verifiable();
      _storageTypeInformationProviderStrictMock
          .Setup(mock => mock.GetStorageTypeForClassID(true))
          .Returns(_fakeStorageTypeInformation2)
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetRelationColumnName(endPointDefinition))
          .Returns("FakeRelationColumnName")
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetRelationClassIDColumnName(endPointDefinition))
          .Returns("FakeRelationClassIDColumnName")
          .Verifiable();

      _storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition))
          .Returns(_factory.StorageProviderDefinition);

      var factoryForcingClassID = new RelationStoragePropertyDefinitionFactory(TestDomainStorageProviderDefinition,
          true, _storageNameProviderMock.Object, _storageTypeInformationProviderStrictMock.Object, _storageSettingsStub.Object);
      var result = factoryForcingClassID.CreateStoragePropertyDefinition(endPointDefinition);

      _storageTypeInformationProviderStrictMock.Verify();
      _storageNameProviderMock.Verify();
      CheckForeignKeyObjectIDStorageProperty(
          result,
          "FakeRelationColumnName",
          _fakeStorageTypeInformation1,
          "FakeRelationClassIDColumnName",
          _fakeStorageTypeInformation2);
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationToClassWithInheritanceHierarchy ()
    {
      var relationEndPointDefinition = GetNonVirtualEndPointDefinition(typeof(Ceo), "Company");
      Assert.That(relationEndPointDefinition.GetOppositeEndPointDefinition().ClassDefinition.IsPartOfInheritanceHierarchy, Is.True);

      _storageTypeInformationProviderStrictMock
          .Setup(mock => mock.GetStorageTypeForID(true))
          .Returns(_fakeStorageTypeInformation1)
          .Verifiable();
      _storageTypeInformationProviderStrictMock
          .Setup(mock => mock.GetStorageTypeForClassID(true))
          .Returns(_fakeStorageTypeInformation2)
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetRelationColumnName(relationEndPointDefinition))
          .Returns("FakeRelationColumnName")
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetRelationClassIDColumnName(relationEndPointDefinition))
          .Returns("FakeRelationClassIDColumnName")
          .Verifiable();

      _storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(relationEndPointDefinition.GetOppositeClassDefinition()))
          .Returns(_factory.StorageProviderDefinition);

      var result = _factory.CreateStoragePropertyDefinition(relationEndPointDefinition);

      _storageTypeInformationProviderStrictMock.Verify();
      _storageNameProviderMock.Verify();

      CheckForeignKeyObjectIDStorageProperty(
          result,
          "FakeRelationColumnName",
          _fakeStorageTypeInformation1,
          "FakeRelationClassIDColumnName",
          _fakeStorageTypeInformation2);
    }

    [Test]
    public void CreateStoragePropertyDefinition_RelationToClassDefinitionWithDifferentStorageProvider ()
    {
      var relationEndPointDefinition = GetNonVirtualEndPointDefinition(typeof(Order), "Official");

      _storageTypeInformationProviderStrictMock
          .Setup(mock => mock.GetStorageTypeForSerializedObjectID(true))
          .Returns(_fakeStorageTypeInformation1)
          .Verifiable();

      _storageNameProviderMock
          .Setup(mock => mock.GetRelationColumnName(relationEndPointDefinition))
          .Returns("FakeRelationColumnName")
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetRelationClassIDColumnName(relationEndPointDefinition))
          .Returns("FakeRelationClassIDColumnName")
          .Verifiable();

      _storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(GetTypeDefinition(typeof(Official))))
          .Returns(UnitTestStorageProviderDefinition);

      var result = _factory.CreateStoragePropertyDefinition(relationEndPointDefinition);

      _storageNameProviderMock.Verify();
      _storageTypeInformationProviderStrictMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(SerializedObjectIDStoragePropertyDefinition)));
      Assert.That(((SerializedObjectIDStoragePropertyDefinition)result).SerializedIDProperty, Is.TypeOf<SimpleStoragePropertyDefinition>());
      var serializedIDProperty = ((SimpleStoragePropertyDefinition)((SerializedObjectIDStoragePropertyDefinition)result).SerializedIDProperty);

      Assert.That(serializedIDProperty.PropertyType, Is.SameAs(typeof(ObjectID)));
      Assert.That(serializedIDProperty.ColumnDefinition.Name, Is.EqualTo("FakeRelationColumnName"));
      Assert.That(serializedIDProperty.ColumnDefinition.StorageTypeInfo, Is.SameAs(_fakeStorageTypeInformation1));
      Assert.That(serializedIDProperty.ColumnDefinition.IsPartOfPrimaryKey, Is.False);
    }

    private void CheckForeignKeyObjectIDStorageProperty (
        IRdbmsStoragePropertyDefinition result,
        string expectedValueColumnName,
        StorageTypeInformation expectedValueColumnStorageTypeInfo,
        string expectedClassIDColumnName,
        StorageTypeInformation expectedClassIDStorageTypeInformation)
    {
      Assert.That(result, Is.TypeOf(typeof(ObjectIDStoragePropertyDefinition)));
      var valueProperty = ((ObjectIDStoragePropertyDefinition)result).ValueProperty;
      var classIDValueProperty = ((ObjectIDStoragePropertyDefinition)result).ClassIDProperty;

      Assert.That(valueProperty, Is.TypeOf(typeof(SimpleStoragePropertyDefinition)));
      Assert.That(valueProperty.PropertyType, Is.SameAs(typeof(object)));

      Assert.That(classIDValueProperty, Is.TypeOf(typeof(SimpleStoragePropertyDefinition)));
      Assert.That(classIDValueProperty.PropertyType, Is.SameAs(typeof(string)));

      var valueIDColumn = ((SimpleStoragePropertyDefinition)valueProperty).ColumnDefinition;
      var classIDColumn = ((SimpleStoragePropertyDefinition)classIDValueProperty).ColumnDefinition;

      Assert.That(valueIDColumn.Name, Is.EqualTo(expectedValueColumnName));
      Assert.That(valueIDColumn.StorageTypeInfo, Is.SameAs(expectedValueColumnStorageTypeInfo));
      Assert.That(valueIDColumn.IsPartOfPrimaryKey, Is.False);

      Assert.That(classIDColumn.Name, Is.EqualTo(expectedClassIDColumnName));
      Assert.That(classIDColumn.StorageTypeInfo, Is.SameAs(expectedClassIDStorageTypeInformation));
      Assert.That(classIDColumn.IsPartOfPrimaryKey, Is.False);
    }
  }
}
