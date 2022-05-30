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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeIsNotGenericValidationRule;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.RelationEndPointNamesAreConsistentValidationRule;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.RelationEndPointPropertyTypeIsSupportedValidationRule;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest : MappingReflectionTestBase
  {
    private ClassDefinition[] _emptyClassDefinitions;
    private RelationDefinition[] _emptyRelationDefinitions;
    private Mock<IMappingLoader> _mockMappingLoader;
    private ReflectionBasedMemberInformationNameResolver _memberInformationNameResolver;
    private TableDefinition _fakeStorageEntityDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _emptyClassDefinitions = new ClassDefinition[0];
      _emptyRelationDefinitions = new RelationDefinition[0];

      _memberInformationNameResolver = new ReflectionBasedMemberInformationNameResolver();
      _mockMappingLoader = new Mock<IMappingLoader>(MockBehavior.Strict);

      _fakeStorageEntityDefinition = TableDefinitionObjectMother.Create(
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition, new EntityNameDefinition(null, "Test"));
    }

    [Test]
    public void Initialize ()
    {
      var typeDefinitions = new ClassDefinition[0];
      var relationDefinitions = new RelationDefinition[0];

      StubMockMappingLoader(typeDefinitions, relationDefinitions);

      var configuration = new MappingConfiguration(
          _mockMappingLoader.Object, new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage)));

      _mockMappingLoader.Verify();

      Assert.That(configuration.ResolveTypes, Is.True);
      Assert.That(configuration.NameResolver, Is.SameAs(_memberInformationNameResolver));
    }

    [Test]
    public void SetCurrent ()
    {
      try
      {
        StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);

        var configuration = new MappingConfiguration(
            _mockMappingLoader.Object, new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage)));
        MappingConfiguration.SetCurrent(configuration);

        Assert.That(MappingConfiguration.Current, Is.SameAs(configuration));
      }
      finally
      {
        MappingConfiguration.SetCurrent(null);
      }
    }

    [Test]
    public void GetTypeDefinitions ()
    {
      var classDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition("C1", typeof(RelationEndPointPropertyClass));
      classDefinition1.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition1.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());

      var classDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition("C2", typeof(RelationEndPointPropertyClass1));
      classDefinition2.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition2.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());

      StubMockMappingLoader(new[] { classDefinition1, classDefinition2 }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.GetTypeDefinitions(), Is.EquivalentTo(new[] { classDefinition1, classDefinition2 }));
    }

    [Test]
    public void ContainsTypeDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());
      StubMockMappingLoader(new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.ContainsTypeDefinition(typeof(RelationEndPointPropertyClass)), Is.True);
    }

    [Test]
    public void ContainsTypeDefinition_ValueNotFound ()
    {
      StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.ContainsTypeDefinition(typeof(RelationEndPointPropertyClass)), Is.False);
    }

    [Test]
    public void GetTypeDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());
      StubMockMappingLoader(new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.GetTypeDefinition(typeof(RelationEndPointPropertyClass)), Is.SameAs(classDefinition));
    }

    [Test]
    public void GetTypeDefinition_ValueNotFound ()
    {
      StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(
          () => configuration.GetTypeDefinition(typeof(DomainObject)),
          Throws.Exception.TypeOf<MappingException>()
              .And.Message.EqualTo(String.Format("Mapping does not contain class '{0}'.", typeof(DomainObject))));
    }

    [Test]
    public void GetTypeDefinition_ValueNotFound_CustomException ()
    {
      StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(
          () => configuration.GetTypeDefinition(typeof(DomainObject), t =>new ApplicationException(t.Name)),
          Throws.Exception.TypeOf<ApplicationException>()
              .And.Message.EqualTo(typeof(DomainObject).Name));
    }

    [Test]
    public void ContainsClassDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());
      StubMockMappingLoader(new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.ContainsClassDefinition(classDefinition.ID), Is.True);
    }

    [Test]
    public void ContainsClassDefinition_ValueNotFound ()
    {
      StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.ContainsClassDefinition("ID"), Is.False);
    }

    [Test]
    public void GetClassDefinition_ValueFound ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(Enumerable.Empty<ClassDefinition>());
      StubMockMappingLoader(new[] { classDefinition }, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(configuration.GetClassDefinition(classDefinition.ID), Is.SameAs(classDefinition));
    }

    [Test]
    public void GetClassDefinition_ValueNotFound ()
    {
      StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(
          () => configuration.GetClassDefinition("ID"),
          Throws.Exception.TypeOf<MappingException>()
              .And.Message.EqualTo("Mapping does not contain class 'ID'."));
    }

    [Test]
    public void GetClassDefinition_ValueNotFound_CustomException ()
    {
      StubMockMappingLoader(_emptyClassDefinitions, _emptyRelationDefinitions);
      var persistenceModelLoaderStub = CreatePersistenceModelLoaderStub();
      var configuration = new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub);

      Assert.That(
          () => configuration.GetClassDefinition("ID", id =>new ApplicationException(id)),
          Throws.Exception.TypeOf<ApplicationException>()
              .And.Message.EqualTo("ID"));
    }

    [Test]
    public void PersistenceModelIsLoaded ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(new ClassDefinition[0]);

      StubMockMappingLoader(new[] { classDefinition }, new RelationDefinition[0]);

      var persistenceModelLoaderMock = new Mock<IPersistenceModelLoader>(MockBehavior.Strict);
      persistenceModelLoaderMock
          .Setup(mock => mock.ApplyPersistenceModelToHierarchy(classDefinition))
          .Callback((ClassDefinition classDefinition) => classDefinition.SetStorageEntity(TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition)))
          .Verifiable();
      persistenceModelLoaderMock
          .Setup(mock => mock.CreatePersistenceMappingValidator(classDefinition))
          .Returns(new PersistenceMappingValidator())
          .Verifiable();

      new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderMock.Object);

      persistenceModelLoaderMock.Verify();
    }

    [Test]
    public void PersistenceModelIsLoaded_NoStorageEntityIsAppliedToTheRootClass ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition("Order", typeof(Order));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());

      Assert.That(classDefinition.HasStorageEntityDefinitionBeenSet, Is.False);

      var persistenceModelStub = new Mock<IPersistenceModelLoader>();

      StubMockMappingLoader(new[] { classDefinition }, new RelationDefinition[0]);
      Assert.That(
          () => new MappingConfiguration(_mockMappingLoader.Object, persistenceModelStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The persistence model loader did not assign a storage entity to class 'Order'."));
    }

    [Test]
    public void PersistenceModelIsLoaded_NoStoragePropertyIsAppliedToTheRootClassProperty ()
    {
      var fakeStorageEntityDefinition = _fakeStorageEntityDefinition;
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition("Order", typeof(Order));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(classDefinition, "Fake");
      PrivateInvoke.SetNonPublicField(propertyDefinition, "_storagePropertyDefinition", null);

      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      Assert.That(classDefinition.HasStorageEntityDefinitionBeenSet, Is.False);
      Assert.That(propertyDefinition.HasStoragePropertyDefinitionBeenSet, Is.False);

      var persistenceModelStub = new Mock<IPersistenceModelLoader>();

      StubMockMappingLoader(new[] { classDefinition }, new RelationDefinition[0]);

      persistenceModelStub
          .Setup(stub => stub.ApplyPersistenceModelToHierarchy(classDefinition))
          .Callback((ClassDefinition classDefinition) => classDefinition.SetStorageEntity(fakeStorageEntityDefinition));
      Assert.That(
          () => new MappingConfiguration(_mockMappingLoader.Object, persistenceModelStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "StoragePropertyDefinition has not been set for property 'Fake' of class 'Order'."));
    }

    [Test]
    public void PersistenceModelIsLoaded_NoStorageEntityIsAppliedToDerivedClass ()
    {
      var fakeStorageEntityDefinition = _fakeStorageEntityDefinition;
      var companyClass = ClassDefinitionObjectMother.CreateClassDefinition("Company", typeof(Company));
      var partnerClass = ClassDefinitionObjectMother.CreateClassDefinition("Partner", typeof(Partner), baseClass: companyClass);

      companyClass.SetPropertyDefinitions(new PropertyDefinitionCollection());
      partnerClass.SetPropertyDefinitions(new PropertyDefinitionCollection());

      companyClass.SetDerivedClasses(new[] { partnerClass });

      Assert.That(companyClass.HasStorageEntityDefinitionBeenSet, Is.False);
      Assert.That(partnerClass.HasStorageEntityDefinitionBeenSet, Is.False);

      var persistenceModelStub = new Mock<IPersistenceModelLoader>();

      StubMockMappingLoader(new[] { companyClass }, new RelationDefinition[0]);

      persistenceModelStub
          .Setup(stub => stub.ApplyPersistenceModelToHierarchy(companyClass))
          .Callback((ClassDefinition classDefinition) => companyClass.SetStorageEntity(fakeStorageEntityDefinition));
      Assert.That(
          () => new MappingConfiguration(_mockMappingLoader.Object, persistenceModelStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The persistence model loader did not assign a storage entity to class 'Partner'."));
    }

    [Test]
    public void ClassDefinitionsAreValidated ()
    {
      var type = typeof(GenericTypeDomainObject<string>);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(type);

      StubMockMappingLoaderWithValidation(new[] { classDefinition }, new RelationDefinition[0]);
      Assert.That(
          () => new MappingConfiguration(
          _mockMappingLoader.Object, new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Generic domain objects are not supported.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection."
                  + "DomainObjectTypeIsNotGenericValidationRule.GenericTypeDomainObject`1[System.String]"));
    }

    [Test]
    public void PropertyDefinitionsAreValidated ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(DerivedValidationDomainObjectClass));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForRealPropertyInfo(
          classDefinition, typeof(DerivedValidationDomainObjectClass), "PropertyWithStorageClassNone");
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));

      StubMockMappingLoaderWithValidation(new[] { classDefinition }, new RelationDefinition[0]);
      Assert.That(
          () => new MappingConfiguration(
          _mockMappingLoader.Object,
          new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Only StorageClass.Persistent and StorageClass.Transaction are supported for property 'PropertyWithStorageClassNone' of class "
                  + "'DerivedValidationDomainObjectClass'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass\r\n"
                  + "Property: PropertyWithStorageClassNone"));
    }

    [Test]
    public void RelationDefinitionsAreValidated ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationEndPointPropertyClass));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      var relationDefinition =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order:"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer->"
              +"Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders"];

      StubMockMappingLoaderWithValidation(new[] { classDefinition }, new[] { relationDefinition });
      Assert.That(
          () => new MappingConfiguration(
          _mockMappingLoader.Object, new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage))),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The property type of an uni-directional relation property must be assignable to 'DomainObject'.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer\r\n"
                  + "Property: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer.Orders"));
    }

    [Test]
    public void PersistenceModelIsValidated ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create(TestDomainStorageProviderDefinition);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(DerivedValidationDomainObjectClass));
      classDefinition.SetStorageEntity(unionViewDefinition);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      classDefinition.SetDerivedClasses(new ClassDefinition[0]);

      StubMockMappingLoaderWithValidation(new[] { classDefinition }, new RelationDefinition[0]);

      var persistenceModelLoaderStub = new Mock<IPersistenceModelLoader>();
      persistenceModelLoaderStub
          .Setup(stub => stub.ApplyPersistenceModelToHierarchy(It.IsAny<ClassDefinition>()));
      persistenceModelLoaderStub
          .Setup(stub => stub.CreatePersistenceMappingValidator(It.IsAny<ClassDefinition>()))
          .Returns(new PersistenceMappingValidator(new ClassAboveTableIsAbstractValidationRule()));

      Assert.That(
          () => new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderStub.Object),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Neither class 'DerivedValidationDomainObjectClass' nor its base classes are mapped to a table. "
                  + "Make class 'DerivedValidationDomainObjectClass' abstract or define a table for it or one of its base classes.\r\n\r\n"
                  + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass"));
    }

    [Test]
    public void PersistenceModelIsValidated_OverAllRootClasses ()
    {
      var persistenceModelLoaderMock = new Mock<IPersistenceModelLoader>(MockBehavior.Strict);
      var validatorMock1 = new Mock<IPersistenceMappingValidator>(MockBehavior.Strict);
      var validatorMock2 = new Mock<IPersistenceMappingValidator>(MockBehavior.Strict);

      var rootClass1 = ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("Order", typeof(Order));
      var rootClass2 = ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("OrderTicket", typeof(OrderTicket));

      persistenceModelLoaderMock
          .Setup(mock => mock.ApplyPersistenceModelToHierarchy(rootClass1))
          .Callback((ClassDefinition classDefinition) => rootClass1.SetStorageEntity(_fakeStorageEntityDefinition))
          .Verifiable();
      persistenceModelLoaderMock
          .Setup(mock => mock.ApplyPersistenceModelToHierarchy(rootClass2))
          .Callback((ClassDefinition classDefinition) => rootClass2.SetStorageEntity(_fakeStorageEntityDefinition))
          .Verifiable();

      persistenceModelLoaderMock.Setup(mock => mock.CreatePersistenceMappingValidator(rootClass1)).Returns(validatorMock1.Object).Verifiable();
      persistenceModelLoaderMock.Setup(mock => mock.CreatePersistenceMappingValidator(rootClass2)).Returns(validatorMock2.Object).Verifiable();

      validatorMock1
          .Setup(mock => mock.Validate(new[] { rootClass1 }))
          .Returns(new MappingValidationResult[0])
          .Verifiable();
      validatorMock2
          .Setup(mock => mock.Validate(new[] { rootClass2 }))
          .Returns(new MappingValidationResult[0])
          .Verifiable();

      StubMockMappingLoaderWithValidation(new[] { rootClass1, rootClass2 }, new RelationDefinition[0]);

      new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderMock.Object);

      persistenceModelLoaderMock.Verify();
      validatorMock1.Verify();
      validatorMock2.Verify();
    }

    [Test]
    public void PersistenceModelIsValidated_OverAllDerivedClasses ()
    {
      var persistenceModelLoaderMock = new Mock<IPersistenceModelLoader>(MockBehavior.Strict);
      var validatorMock = new Mock<IPersistenceMappingValidator>(MockBehavior.Strict);

      var rootClass = CreateFileSystemItemDefinition_WithEmptyMembers_AndWithDerivedClasses();
      var derivedClass1 = rootClass.DerivedClasses[0];
      var derivedClass2 = rootClass.DerivedClasses[1];

      persistenceModelLoaderMock
          .Setup(mock => mock.ApplyPersistenceModelToHierarchy(rootClass))
          .Callback(
              (ClassDefinition classDefinition) =>
              {
                rootClass.SetStorageEntity(_fakeStorageEntityDefinition);
                derivedClass1.SetStorageEntity(_fakeStorageEntityDefinition);
                derivedClass2.SetStorageEntity(_fakeStorageEntityDefinition);
              })
          .Verifiable();
      persistenceModelLoaderMock.Setup(mock => mock.CreatePersistenceMappingValidator(rootClass)).Returns(validatorMock.Object).Verifiable();

      validatorMock
          .Setup(mock => mock.Validate(new[] { rootClass, derivedClass1, derivedClass2 }))
          .Returns(new MappingValidationResult[0])
          .Verifiable();

      StubMockMappingLoaderWithValidation(new[] { rootClass, derivedClass1, derivedClass2 }, new RelationDefinition[0]);

      new MappingConfiguration(_mockMappingLoader.Object, persistenceModelLoaderMock.Object);

      persistenceModelLoaderMock.Verify();
      validatorMock.Verify();
    }

    [Test]
    public void SetCurrentRejectsUnresolvedTypes ()
    {
      _mockMappingLoader.Setup(_ => _.GetClassDefinitions()).Returns(_emptyClassDefinitions);
      _mockMappingLoader.Setup(_ => _.GetRelationDefinitions(It.IsAny<IDictionary<Type, ClassDefinition>>())).Returns(_emptyRelationDefinitions);
      _mockMappingLoader.Setup(_ => _.ResolveTypes).Returns(false);
      _mockMappingLoader.Setup(_ => _.NameResolver).Returns(_memberInformationNameResolver);
      _mockMappingLoader.Setup(_ => _.CreateClassDefinitionValidator()).Returns(new ClassDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreatePropertyDefinitionValidator()).Returns(new PropertyDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreateRelationDefinitionValidator()).Returns(new RelationDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreateSortExpressionValidator()).Returns(new SortExpressionValidator());

      var configuration = new MappingConfiguration(
          _mockMappingLoader.Object, new PersistenceModelLoader(new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage)));

      _mockMappingLoader.Verify();
      Assert.That(
          () => MappingConfiguration.SetCurrent(configuration),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.", "mappingConfiguration"));
    }

    private void StubMockMappingLoader (ClassDefinition[] classDefinitions, RelationDefinition[] relationDefinitions)
    {
      _mockMappingLoader.Setup(_ => _.GetClassDefinitions()).Returns(classDefinitions);
      _mockMappingLoader.Setup(_ => _.GetRelationDefinitions(It.IsAny<IDictionary<Type, ClassDefinition>>())).Returns(relationDefinitions);
      _mockMappingLoader.Setup(_ => _.ResolveTypes).Returns(true);
      _mockMappingLoader.Setup(_ => _.NameResolver).Returns(_memberInformationNameResolver);
      _mockMappingLoader.Setup(_ => _.CreateClassDefinitionValidator()).Returns(new ClassDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreatePropertyDefinitionValidator()).Returns(new PropertyDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreateRelationDefinitionValidator()).Returns(new RelationDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreateSortExpressionValidator()).Returns(new SortExpressionValidator());
    }

    private void StubMockMappingLoaderWithValidation (ClassDefinition[] classDefinitions, RelationDefinition[] relationDefinitions)
    {
      _mockMappingLoader.Setup(_ => _.GetClassDefinitions()).Returns(classDefinitions);
      _mockMappingLoader.Setup(_ => _.GetRelationDefinitions(It.IsAny<IDictionary<Type, ClassDefinition>>())).Returns(relationDefinitions);
      _mockMappingLoader.Setup(_ => _.ResolveTypes).Returns(true);
      _mockMappingLoader.Setup(_ => _.NameResolver).Returns(_memberInformationNameResolver);
      _mockMappingLoader.Setup(_ => _.CreateClassDefinitionValidator()).Returns(CreateClassDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreatePropertyDefinitionValidator()).Returns(CreatePropertyDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreateRelationDefinitionValidator()).Returns(CreateRelationDefinitionValidator());
      _mockMappingLoader.Setup(_ => _.CreateSortExpressionValidator()).Returns(CreateSortExpressionValidator());
    }

    private ClassDefinitionValidator CreateClassDefinitionValidator ()
    {
      return new ClassDefinitionValidator(new DomainObjectTypeIsNotGenericValidationRule());
    }

    private PropertyDefinitionValidator CreatePropertyDefinitionValidator ()
    {
      return new PropertyDefinitionValidator(new StorageClassIsSupportedValidationRule());
    }

    private RelationDefinitionValidator CreateRelationDefinitionValidator ()
    {
      return new RelationDefinitionValidator(new RelationEndPointPropertyTypeIsSupportedValidationRule());
    }

    private SortExpressionValidator CreateSortExpressionValidator ()
    {
      return new SortExpressionValidator(new SortExpressionIsValidValidationRule());
    }

    private IPersistenceModelLoader CreatePersistenceModelLoaderStub ()
    {
      var persistenceModelLoaderStub = new Mock<IPersistenceModelLoader>();
      persistenceModelLoaderStub
          .Setup(stub => stub.ApplyPersistenceModelToHierarchy(It.IsAny<ClassDefinition>()))
          .Callback((ClassDefinition classDefinition) => classDefinition.SetStorageEntity(TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition)));
      persistenceModelLoaderStub
          .Setup(stub => stub.CreatePersistenceMappingValidator(It.IsAny<ClassDefinition>()))
          .Returns(new PersistenceMappingValidator());
      return persistenceModelLoaderStub.Object;
    }

    private static ClassDefinition CreateFileSystemItemDefinition_WithEmptyMembers_AndWithDerivedClasses ()
    {
      var fileSystemItemClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition("FileSystemItem", typeof(FileSystemItem));
      var fileClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("File", typeof(File), fileSystemItemClassDefinition);
      var folderClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses("Folder", typeof(Folder), fileSystemItemClassDefinition);

      fileSystemItemClassDefinition.SetDerivedClasses(new [] { fileClassDefinition, folderClassDefinition });
      fileSystemItemClassDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      fileSystemItemClassDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());

      return fileSystemItemClassDefinition;
    }
  }
}
