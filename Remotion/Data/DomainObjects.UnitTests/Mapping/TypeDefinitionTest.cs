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
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class TypeDefinitionTest : MappingReflectionTestBase
  {
    private interface IDomainBase
    {
    }

    private interface IPerson : IDomainBase
    {
    }

    private interface ICustomer : IPerson
    {
    }

    private interface IOrganizationalUnit : IDomainBase
    {
    }

    private class TestableTypeDefinition : TypeDefinition
    {
      public Action<IEnumerable<PropertyDefinition>> CheckPropertyDefinitionsCallback { get; set; }

      public Action<IEnumerable<IRelationEndPointDefinition>> CheckRelationEndPointDefinitionsCallback { get; set; }

      public Func<PropertyDefinitionCollection> CreatePropertyDefinitionCollectionCallback { get; set; }

      public Func<RelationEndPointDefinitionCollection> CreateRelationEndPointDefinitionCollectionCallback { get; set; }

      public TestableTypeDefinition (
          [NotNull] Type type,
          [CanBeNull] Type storageGroupType,
          DefaultStorageClass defaultStorageClass)
          : base(type, storageGroupType, defaultStorageClass)
      {
      }

      public override void Accept (ITypeDefinitionVisitor visitor) => throw new NotImplementedException();

      public override T Accept<T> (ITypeDefinitionVisitor<T> visitor) where T : default => throw new NotImplementedException();

      public override bool IsPartOfInheritanceHierarchy => throw new NotImplementedException();

      public override bool IsAssignableFrom (TypeDefinition other) => throw new NotImplementedException();

      protected override void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions)
      {
        CheckPropertyDefinitionsCallback?.Invoke(propertyDefinitions);
      }

      protected override void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints)
      {
        CheckRelationEndPointDefinitionsCallback?.Invoke(relationEndPoints);
      }

      protected override PropertyDefinitionCollection CreatePropertyDefinitionCollection ()
      {
        return CreatePropertyDefinitionCollectionCallback?.Invoke();
      }

      protected override RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection ()
      {
        return CreateRelationEndPointDefinitionCollectionCallback?.Invoke();
      }

      public void SetupForReadOnlyUsage ()
      {
        SetPropertyDefinitions(new PropertyDefinitionCollection());
        SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
        SetStorageEntity(new FakeStorageEntityDefinition(new UnitTestStorageProviderStubDefinition("stub"), "stub"));
      }
    }

    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    private TestableTypeDefinition _domainBaseTypeDefinition;
    private TestableTypeDefinition _personTypeDefinition;
    private TestableTypeDefinition _customerTypeDefinition;
    private TestableTypeDefinition _organizationalUnitTypeDefinition;

    private Mock<Action<IEnumerable<PropertyDefinition>>> _checkPropertyDefinitionsMock;
    private Mock<Action<IEnumerable<IRelationEndPointDefinition>>> _checkRelationEndPointDefinitionsMock;
    private Mock<Func<PropertyDefinitionCollection>> _createPropertyDefinitionCollectionMock;
    private Mock<Func<RelationEndPointDefinitionCollection>> _createRelationEndpointCollectionMock;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition("DefaultStorageProvider");

      _checkPropertyDefinitionsMock = new Mock<Action<IEnumerable<PropertyDefinition>>>();
      _checkRelationEndPointDefinitionsMock = new Mock<Action<IEnumerable<IRelationEndPointDefinition>>>();
      _createPropertyDefinitionCollectionMock = new Mock<Func<PropertyDefinitionCollection>>();
      _createRelationEndpointCollectionMock = new Mock<Func<RelationEndPointDefinitionCollection>>();

      _domainBaseTypeDefinition = new TestableTypeDefinition(typeof(IDomainBase), null, DefaultStorageClass.Persistent);
      _domainBaseTypeDefinition.CheckPropertyDefinitionsCallback = _checkPropertyDefinitionsMock.Object;
      _domainBaseTypeDefinition.CheckRelationEndPointDefinitionsCallback = _checkRelationEndPointDefinitionsMock.Object;
      _domainBaseTypeDefinition.CreatePropertyDefinitionCollectionCallback = _createPropertyDefinitionCollectionMock.Object;
      _domainBaseTypeDefinition.CreateRelationEndPointDefinitionCollectionCallback = _createRelationEndpointCollectionMock.Object;

      _personTypeDefinition = new TestableTypeDefinition(typeof(IPerson), null, DefaultStorageClass.Persistent);
      _customerTypeDefinition = new TestableTypeDefinition(typeof(ICustomer), null, DefaultStorageClass.Persistent);
      _organizationalUnitTypeDefinition = new TestableTypeDefinition(typeof(IOrganizationalUnit), null, DefaultStorageClass.Persistent);
    }

    [Test]
    public void Initialize ()
    {
      var actual = new TestableTypeDefinition(
          typeof(IDomainBase),
          null,
          DefaultStorageClass.Transaction);

      Assert.That(actual.Type, Is.SameAs(typeof(IDomainBase)));
      Assert.That(actual.StorageGroupType, Is.Null);
      Assert.That(actual.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Transaction));

      Assert.That(actual.HasStorageEntityDefinitionBeenSet, Is.False);
      Assert.That(
          () => actual.StorageEntityDefinition,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "StorageEntityDefinition has not been set for type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase'."));

      Assert.That(
          () => actual.MyPropertyDefinitions,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No property definitions have been set for type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase'."));
      Assert.That(
          () => actual.MyRelationEndPointDefinitions,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No relation end point definitions have been set for type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase'."));

      Assert.That(actual.PropertyAccessorDataCache, Is.Not.Null);
      Assert.That(actual.IsReadOnly, Is.False);
    }

    [Test]
    public void SetStorageEntity ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "Tablename"));
      Assert.That(_domainBaseTypeDefinition.HasStorageEntityDefinitionBeenSet, Is.False);

      _domainBaseTypeDefinition.SetStorageEntity(tableDefinition);

      Assert.That(_domainBaseTypeDefinition.StorageEntityDefinition, Is.SameAs(tableDefinition));
      Assert.That(_domainBaseTypeDefinition.HasStorageEntityDefinitionBeenSet, Is.True);
    }

    [Test]
    public void SetStorageEntity_ReadOnly_Throws ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(_storageProviderDefinition, new EntityNameDefinition(null, "Tablename"));
      _domainBaseTypeDefinition.SetupForReadOnlyUsage();
      _domainBaseTypeDefinition.SetReadOnly();

      Assert.That(
          () => _domainBaseTypeDefinition.SetStorageEntity(tableDefinition),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase' is read-only."));
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      var collection = new PropertyDefinitionCollection();
      _createPropertyDefinitionCollectionMock.Setup(_ => _()).Returns(collection);

      var actualCollection = _domainBaseTypeDefinition.GetPropertyDefinitions();

      Assert.That(actualCollection, Is.SameAs(collection));
    }

    [Test]
    public void GetPropertyDefinitions_SecondCall_ReturnsNewCollection ()
    {
      _createPropertyDefinitionCollectionMock.Setup(_ => _()).Returns(() => new PropertyDefinitionCollection());

      var result1 = _domainBaseTypeDefinition.GetPropertyDefinitions();
      var result2 = _domainBaseTypeDefinition.GetPropertyDefinitions();

      Assert.That(result1, Is.Not.Null);
      Assert.That(result1, Is.Not.SameAs(result2));
    }

    [Test]
    public void GetPropertyDefinitions_SecondCallAndReadOnly_ReturnsCachedResult ()
    {
      _createPropertyDefinitionCollectionMock.Setup(_ => _()).Returns(() => new PropertyDefinitionCollection());
      _domainBaseTypeDefinition.SetupForReadOnlyUsage();
      _domainBaseTypeDefinition.SetReadOnly();

      var result1 = _domainBaseTypeDefinition.GetPropertyDefinitions();
      var result2 = _domainBaseTypeDefinition.GetPropertyDefinitions();

      Assert.That(result1, Is.Not.Null);
      Assert.That(result1, Is.SameAs(result2));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      var propertyInformation = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_domainBaseTypeDefinition);
      _createPropertyDefinitionCollectionMock.Setup(_ => _()).Returns(new PropertyDefinitionCollection(new []{propertyInformation}, true));

      Assert.That(
          _domainBaseTypeDefinition.GetPropertyDefinition("Test"),
          Is.SameAs(propertyInformation));
    }

    [Test]
    public void GetPropertyDefinition_EmptyPropertyName_Throws ()
    {
      Assert.That(
          () => _domainBaseTypeDefinition.GetPropertyDefinition(string.Empty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'propertyName' cannot be empty.", "propertyName"));
    }

    [Test]
    public void GetPropertyDefinition_NonExistingProperty_ReturnsNull ()
    {
      _createPropertyDefinitionCollectionMock.Setup(_ => _()).Returns(new PropertyDefinitionCollection());

      Assert.That(_domainBaseTypeDefinition.GetPropertyDefinition("dummy"), Is.Null);
    }

    [Test]
    public void GetMandatoryPropertyDefinition_NonExistingProperty_Throws ()
    {
      _createPropertyDefinitionCollectionMock.Setup(_ => _()).Returns(new PropertyDefinitionCollection());

      Assert.That(
          () => _domainBaseTypeDefinition.GetMandatoryPropertyDefinition("dummy"),
          Throws.TypeOf<MappingException>()
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase' "
                  + "does not contain the property 'dummy'."));
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(_domainBaseTypeDefinition);

      _checkPropertyDefinitionsMock
          .Setup(_ => _(new[] { propertyDefinition }))
          .Callback(
              () => { Assert.That(() => _domainBaseTypeDefinition.MyPropertyDefinitions, Throws.Exception); }
          )
          .Verifiable();

      _domainBaseTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, false));

      Assert.That(_domainBaseTypeDefinition.MyPropertyDefinitions.Count, Is.EqualTo(1));
      Assert.That(_domainBaseTypeDefinition.MyPropertyDefinitions[0], Is.SameAs(propertyDefinition));
      Assert.That(_domainBaseTypeDefinition.MyPropertyDefinitions.IsReadOnly, Is.True);

      _checkPropertyDefinitionsMock.Verify();
    }

    [Test]
    public void SetPropertyDefinitions_IsReadOnly ()
    {
      _domainBaseTypeDefinition.SetupForReadOnlyUsage();
      _domainBaseTypeDefinition.SetReadOnly();

      Assert.That(
          () => _domainBaseTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new PropertyDefinition[0], true)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase' is read-only."));
    }

    [Test]
    public void GetRelationEndPointDefinitions ()
    {
      var collection = new RelationEndPointDefinitionCollection();
      _createRelationEndpointCollectionMock.Setup(_ => _()).Returns(collection);

      var actualCollection = _domainBaseTypeDefinition.GetRelationEndPointDefinitions();

      Assert.That(actualCollection, Is.SameAs(collection));
    }

    [Test]
    public void GetRelationEndPointDefinitions_SecondCall_ReturnsNewCollection ()
    {
      _createRelationEndpointCollectionMock.Setup(_ => _()).Returns(() => new RelationEndPointDefinitionCollection());

      var result1 = _domainBaseTypeDefinition.GetRelationEndPointDefinitions();
      var result2 = _domainBaseTypeDefinition.GetRelationEndPointDefinitions();

      Assert.That(result1, Is.Not.Null);
      Assert.That(result1, Is.Not.SameAs(result2));
    }

    [Test]
    public void GetRelationEndPointDefinitions_SecondCall_ReturnsCachedResult ()
    {
      _createRelationEndpointCollectionMock.Setup(_ => _()).Returns(() => new RelationEndPointDefinitionCollection());
      _domainBaseTypeDefinition.SetupForReadOnlyUsage();
      _domainBaseTypeDefinition.SetReadOnly();

      var result1 = _domainBaseTypeDefinition.GetRelationEndPointDefinitions();
      var result2 = _domainBaseTypeDefinition.GetRelationEndPointDefinitions();

      Assert.That(result1, Is.Not.Null);
      Assert.That(result1, Is.SameAs(result2));
    }

    [Test]
    public void GetRelationEndPointDefinition ()
    {
      var endPoint = new VirtualObjectRelationEndPointDefinition(
          _domainBaseTypeDefinition,
          "Test",
          false,
          new Mock<IPropertyInformation>().Object);

      var collection = new RelationEndPointDefinitionCollection(new[] { endPoint }, true);
      var createRelationEndPointDefinitionCollectionMock = new Mock<Func<RelationEndPointDefinitionCollection>>();
      createRelationEndPointDefinitionCollectionMock.Setup(_ => _()).Returns(collection);

      _domainBaseTypeDefinition.CreateRelationEndPointDefinitionCollectionCallback = createRelationEndPointDefinitionCollectionMock.Object;

      Assert.That(
          _domainBaseTypeDefinition.GetRelationEndPointDefinition("Test"),
          Is.SameAs(endPoint));
    }

    [Test]
    public void GetRelationEndPointDefinition_EmptyRelationEndPointName_Throws ()
    {
      Assert.That(
          () => _domainBaseTypeDefinition.GetRelationEndPointDefinition(string.Empty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'propertyName' cannot be empty.", "propertyName"));
    }

    [Test]
    public void GetRelationEndPointDefinition_NonExistingRelationEndPoint_ReturnsNull ()
    {
      var collection = new RelationEndPointDefinitionCollection();
      _createRelationEndpointCollectionMock.Setup(_ => _()).Returns(collection);

      Assert.That(_domainBaseTypeDefinition.GetRelationEndPointDefinition("dummy"), Is.Null);
    }

    [Test]
    public void GetMandatoryRelationEndPointDefinition_NonExistingRelationEndPoint_Throws ()
    {
      var collection = new RelationEndPointDefinitionCollection();
      _createRelationEndpointCollectionMock.Setup(_ => _()).Returns(collection);

      Assert.That(
          () => _domainBaseTypeDefinition.GetMandatoryRelationEndPointDefinition("dummy"),
          Throws.TypeOf<MappingException>()
              .With.Message.EqualTo(
                  "No relation found for type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase' and property 'dummy'."));
    }

    [Test]
    public void SetRelationEndPointDefinitions ()
    {
      var endPoint = new VirtualObjectRelationEndPointDefinition(
          _domainBaseTypeDefinition,
          "Test",
          false,
          new Mock<IPropertyInformation>().Object);

      _checkRelationEndPointDefinitionsMock.Setup(_ => _(new[] { endPoint }))
          .Callback(
              () => { Assert.That(() => _domainBaseTypeDefinition.MyRelationEndPointDefinitions, Throws.Exception); }
          )
          .Verifiable();

      _domainBaseTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { endPoint }, false));

      Assert.That(_domainBaseTypeDefinition.MyRelationEndPointDefinitions.Count, Is.EqualTo(1));
      Assert.That(_domainBaseTypeDefinition.MyRelationEndPointDefinitions[0], Is.SameAs(endPoint));
      Assert.That(_domainBaseTypeDefinition.MyRelationEndPointDefinitions.IsReadOnly, Is.True);

      _checkRelationEndPointDefinitionsMock.Verify();
    }

    [Test]
    public void SetRelationEndPointDefinitions_IsReadOnly ()
    {
      _domainBaseTypeDefinition.SetupForReadOnlyUsage();
      _domainBaseTypeDefinition.SetReadOnly();
      Assert.That(
          () => _domainBaseTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(Array.Empty<RelationEndPointDefinition>(), true)),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase' is read-only."));
    }

    [Test]
    public void SetReadOnly ()
    {
      Assert.That(_domainBaseTypeDefinition.IsReadOnly, Is.False);

      _domainBaseTypeDefinition.SetupForReadOnlyUsage();
      _domainBaseTypeDefinition.SetReadOnly();

      Assert.That(_domainBaseTypeDefinition.IsReadOnly, Is.True);
    }

    [Test]
    public void SetReadOnly_WithUnsetStorageEntity_Throws ()
    {
      _domainBaseTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _domainBaseTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());

      Assert.That(
          () => _domainBaseTypeDefinition.SetReadOnly(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot set the type definition read-only as the storage entity definition is not set."));
    }

    [Test]
    public void SetReadOnly_WithUnsetPropertyDefinitions_Throws ()
    {
      _domainBaseTypeDefinition.SetStorageEntity(new FakeStorageEntityDefinition(new UnitTestStorageProviderStubDefinition("stub"), "stub"));
      _domainBaseTypeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());

      Assert.That(
          () => _domainBaseTypeDefinition.SetReadOnly(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot set the type definition read-only as the property definitions are not set."));
    }

    [Test]
    public void SetReadOnly_WithUnsetRelationEndPointDefinitions_Throws ()
    {
      _domainBaseTypeDefinition.SetStorageEntity(new FakeStorageEntityDefinition(new UnitTestStorageProviderStubDefinition("stub"), "stub"));
      _domainBaseTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      Assert.That(
          () => _domainBaseTypeDefinition.SetReadOnly(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot set the type definition read-only as the relation endpoint definitions are not set."));
    }

    [Test]
    public void GetToString ()
    {
      Assert.That(
          _domainBaseTypeDefinition.ToString(),
          Is.EqualTo("TestableTypeDefinition: Remotion.Data.DomainObjects.UnitTests.Mapping.TypeDefinitionTest+IDomainBase"));
    }

    [Test]
    public void Indexer ()
    {
      var propertyInformation = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
          _domainBaseTypeDefinition,
          "Test");
      _domainBaseTypeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyInformation }, true));

      var actual = _domainBaseTypeDefinition["Test"];
      Assert.That(actual, Is.EqualTo(propertyInformation));
    }

    [Test]
    public void Indexer_EmptyParameterName_Throws ()
    {
      Assert.That(
          () => _domainBaseTypeDefinition[""],
          Throws.ArgumentException
              .With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyName"));
    }
  }
}
