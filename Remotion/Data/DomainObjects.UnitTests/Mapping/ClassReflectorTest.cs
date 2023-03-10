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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class ClassReflectorTest : MappingReflectionTestBase
  {
    private ClassDefinitionChecker _classDefinitionChecker;
    private RelationEndPointDefinitionChecker _endPointDefinitionChecker;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionChecker = new ClassDefinitionChecker();
      _endPointDefinitionChecker = new RelationEndPointDefinitionChecker();
    }

    [Test]
    public void GetMetadata_ForBaseClass ()
    {
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BaseString")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BaseUnidirectionalOneToOne")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BasePrivateUnidirectionalOneToOne")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "String")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "UnidirectionalOneToOne")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "PrivateString")))
          .Returns(true);

      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(ClassWithDifferentProperties))).Returns("ClassWithDifferentProperties");

      var classReflector = CreateClassReflector(typeof(ClassWithDifferentProperties));
      var expected = CreateClassWithDifferentPropertiesClassDefinition();

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual, Is.Not.Null);
      _classDefinitionChecker.Check(expected, actual);
      _endPointDefinitionChecker.Check(expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "OtherString")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "String")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "PrivateString")))
          .Returns(true);

      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(DerivedClassWithDifferentProperties))).Returns("DerivedClassWithDifferentProperties");

      var classReflector = CreateClassReflector(typeof(DerivedClassWithDifferentProperties));
      var expected = CreateDerivedClassWithDifferentPropertiesClassDefinition();

      var baseClassDefinition = CreateClassWithDifferentPropertiesClassDefinition();
      var actual = classReflector.GetMetadata(baseClassDefinition);

      Assert.That(actual, Is.Not.Null);
      _classDefinitionChecker.Check(expected, actual);
      _endPointDefinitionChecker.Check(expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForMixedClass ()
    {
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(TargetClassA))).Returns("ClassID");

      var classReflector = CreateClassReflector(typeof(TargetClassA));
      var actual = classReflector.GetMetadata(null);
      Assert.That(actual.PersistentMixins, Is.EquivalentTo(new[] { typeof(MixinA), typeof(MixinC), typeof(MixinD) }));
    }

    [Test]
    public void GetMetadata_ForDerivedMixedClass ()
    {
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(TargetClassA))).Returns("ClassID");
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(TargetClassB))).Returns("ClassID");

      var classReflectorForBaseClass = CreateClassReflector(typeof(TargetClassA));
      var baseClass = classReflectorForBaseClass.GetMetadata(null);

      var classReflector = CreateClassReflector(typeof(TargetClassB));
      var actual = classReflector.GetMetadata(baseClass);
      Assert.That(actual.PersistentMixins, Is.EquivalentTo(new[] { typeof(MixinB), typeof(MixinE) }));
    }

    [Test]
    public void GetMetadata_ForClassWithVirtualRelationEndPoints ()
    {
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BaseBidirectionalOneToOne")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BaseBidirectionalOneToManyForDomainObjectCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BaseBidirectionalOneToManyForVirtualCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BasePrivateBidirectionalOneToOne")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BasePrivateBidirectionalOneToManyForDomainObjectCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BasePrivateBidirectionalOneToManyForVirtualCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "NoAttributeForDomainObjectCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "NoAttributeForVirtualCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "NotNullableForDomainObjectCollection")))
          .Returns(false);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "NotNullableForVirtualCollection")))
          .Returns(false);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BidirectionalOneToOne")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BidirectionalOneToManyForDomainObjectCollection")))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(It.Is<IPropertyInformation>(pi => pi.Name == "BidirectionalOneToManyForVirtualCollection")))
          .Returns(true);

      var classDefinitionFake = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassWithRealRelationEndPoints));

      SortExpressionDefinitionProviderStub
          .Setup(_ => _.GetSortExpression(It.IsAny<IPropertyInformation>(), It.IsAny<ClassDefinition>(), null))
          .Returns((SortExpressionDefinition)null);
      SortExpressionDefinitionProviderStub
          .Setup(_ => _.GetSortExpression(It.IsAny<IPropertyInformation>(), It.IsAny<ClassDefinition>(), It.IsNotNull<string>()))
          .Returns(
              (IPropertyInformation propertyInfo, ClassDefinition referencedClassDefinition, string sortExpressionText) =>
              {
                var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(
                    referencedClassDefinition,
                    sortExpressionText);
                var sortExpressionDefinition = new SortExpressionDefinition(
                    new[] { SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending(propertyDefinition) });
                return sortExpressionDefinition;
              });

      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(ClassWithVirtualRelationEndPoints))).Returns("ClassWithVirtualRelationEndPoints");

      var classReflector = CreateClassReflector(typeof(ClassWithVirtualRelationEndPoints));
      var expected = CreateClassWithVirtualRelationEndPointsClassDefinition();
      expected.SetPropertyDefinitions(new PropertyDefinitionCollection());
      CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints(expected);

      var actual = classReflector.GetMetadata(null);
      foreach (var actualEndPoint in actual.MyRelationEndPointDefinitions)
      {
        var endPointStub = new Mock<IRelationEndPointDefinition>();
        endPointStub.Setup(stub => stub.ClassDefinition).Returns(classDefinitionFake);
        ((IRelationEndPointDefinitionSetter)actualEndPoint).SetRelationDefinition(
            new RelationDefinition("fake: " + actualEndPoint.PropertyName, actualEndPoint, endPointStub.Object));
      }

      Assert.That(actual, Is.Not.Null);
      _classDefinitionChecker.Check(expected, actual);
      _endPointDefinitionChecker.Check(expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_GetClassID ()
    {
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(ClassHavingClassIDAttribute))).Returns("ClassIDForClassHavingClassIDAttribute");

      var classReflector = CreateClassReflector(typeof(ClassHavingClassIDAttribute));

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.ID, Is.EqualTo("ClassIDForClassHavingClassIDAttribute"));
    }

    [Test]
    public void GetMetadata_ForClosedGenericClass ()
    {
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(ClosedGenericClass))).Returns("ClassID");

      var classReflector = CreateClassReflector(typeof(ClosedGenericClass));

      Assert.That(classReflector.GetMetadata(null), Is.Not.Null);
    }

    [Test]
    public void GetMetadata_ForClassWithoutStorageGroupAttribute ()
    {
      var type = typeof(ClassWithoutStorageGroupWithDifferentProperties);
      ClassIDProviderStub.Setup(stub => stub.GetClassID(type)).Returns("ClassID");
      Assert.That(type.IsDefined(typeof(DBStorageGroupAttribute), false), Is.False);

      var classReflector = CreateClassReflector(type);

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.StorageGroupType, Is.Null);
      Assert.That(actual.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Persistent));
    }

    [Test]
    public void GetMetadata_ForClassWithStorageGroupAttribute ()
    {
      var type = typeof(ClassWithStorageGroupAttributeAndBaseClass);
      ClassIDProviderStub.Setup(stub => stub.GetClassID(type)).Returns("ClassID");
      Assert.That(type.IsDefined(typeof(DBStorageGroupAttribute), false), Is.True);

      var classReflector = CreateClassReflector(type);

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.StorageGroupType, Is.SameAs(typeof(DBStorageGroupAttribute)));
      Assert.That(actual.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Persistent));
    }

    [Test]
    public void GetMetadata_ForClassWithStorageGroupAttributeSupplyingANonDefaultStorageGroup ()
    {
      var type = typeof(ClassWithNonDefaultStorageClass);
      ClassIDProviderStub.Setup(stub => stub.GetClassID(type)).Returns("ClassID");

      var classReflector = CreateClassReflector(type);

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.StorageGroupType, Is.SameAs(typeof(NonDefaultStorageClassStorageGroupAttribute)));
      Assert.That(actual.DefaultStorageClass, Is.Not.EqualTo(DefaultStorageClass.Persistent));
      Assert.That(actual.DefaultStorageClass, Is.EqualTo(NonDefaultStorageClassStorageGroupAttribute.NonDefaultStorageClass));
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForBaseClass ()
    {
      ClassIDProviderStub.Setup(mock => mock.GetClassID(typeof(ClassWithDifferentProperties))).Returns("ClassID");

      var classReflector = CreateClassReflector(typeof(ClassWithDifferentProperties));

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForDerivedClass ()
    {
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(DerivedClassWithDifferentProperties))).Returns("ClassID");

      var classReflector = CreateClassReflector(typeof(DerivedClassWithDifferentProperties));
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses();

      var actual = classReflector.GetMetadata(baseClassDefinition);

      Assert.That(actual.PersistentMixinFinder.IncludeInherited, Is.False);
    }

    [Test]
    public void GetMetadata_InstanceCreator ()
    {
      ClassIDProviderStub.Setup(stub => stub.GetClassID(typeof(ClassWithDifferentProperties))).Returns("ClassID");

      var classReflector = CreateClassReflector(typeof(ClassWithDifferentProperties));

      var actual = classReflector.GetMetadata(null);

      Assert.That(actual.InstanceCreator, Is.SameAs(DomainObjectCreatorStub.Object));
    }

    private ClassReflector CreateClassReflector (Type type)
    {
      return new ClassReflector(
          type,
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderStub.Object,
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          SortExpressionDefinitionProviderStub.Object,
          DomainObjectCreatorStub.Object);
    }

    private ClassDefinition CreateClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = CreateClassDefinition("ClassWithDifferentProperties", typeof(ClassWithDifferentProperties), false);

      CreatePropertyDefinitionsForClassWithDifferentProperties(classDefinition);
      CreateEndPointDefinitionsForClassWithDifferentProperties(classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateDerivedClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = CreateClassDefinition(
          "DerivedClassWithDifferentProperties",
          typeof(DerivedClassWithDifferentProperties),
          false,
          CreateClassWithDifferentPropertiesClassDefinition());
      CreatePropertyDefinitionsForDerivedClassWithDifferentProperties(classDefinition);
      CreateEndPointDefinitionsForDerivedClassWithDifferentProperties(classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithVirtualRelationEndPointsClassDefinition ()
    {
      var classDefinition = CreateClassDefinition(
          "ClassWithVirtualRelationEndPoints",
          typeof(ClassWithVirtualRelationEndPoints),
          false);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition, typeof(ClassWithDifferentPropertiesNotInMapping), "BaseString", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(ClassWithDifferentPropertiesNotInMapping),
              "BaseUnidirectionalOneToOne",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(ClassWithDifferentPropertiesNotInMapping),
              "BasePrivateUnidirectionalOneToOne",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition, typeof(ClassWithDifferentProperties), "Int32", false, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition, typeof(ClassWithDifferentProperties), "String", true, null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(ClassWithDifferentProperties),
              "PrivateString",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(ClassWithDifferentProperties),
              "UnidirectionalOneToOne",
              true,
              null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));
    }

    private void CreateEndPointDefinitionsForClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();
      endPoints.Add(CreateRelationEndPointDefinition(classDefinition, typeof(ClassWithDifferentProperties), "UnidirectionalOneToOne", false));
      endPoints.Add(
          CreateRelationEndPointDefinition(classDefinition, typeof(ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne", false));
      endPoints.Add(
          CreateRelationEndPointDefinition(
              classDefinition, typeof(ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne", false));
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(endPoints, true));
    }

    private void CreatePropertyDefinitionsForDerivedClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(DerivedClassWithDifferentProperties),
              "String",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(DerivedClassWithDifferentProperties),
              "PrivateString",
              true,
              null));
      properties.Add(
          CreatePersistentPropertyDefinition(
              classDefinition,
              typeof(DerivedClassWithDifferentProperties),
              "OtherString",
              true,
              null));
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));
    }

    private void CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();

      endPoints.Add(
          CreateDomainObjectCollectionRelationEndPointDefinition(
              classDefinition, typeof(ClassWithVirtualRelationEndPoints), "NoAttributeForDomainObjectCollection", false, CreateEmptySortExpressionDefinition()));
      endPoints.Add(
          CreateVirtualCollectionRelationEndPointDefinition(
              classDefinition, typeof(ClassWithVirtualRelationEndPoints), "NoAttributeForVirtualCollection", false, CreateEmptySortExpressionDefinition()));
      endPoints.Add(
          CreateDomainObjectCollectionRelationEndPointDefinition(
              classDefinition, typeof(ClassWithVirtualRelationEndPoints), "NotNullableForDomainObjectCollection", true, CreateEmptySortExpressionDefinition()));
      endPoints.Add(
          CreateVirtualCollectionRelationEndPointDefinition(
              classDefinition, typeof(ClassWithVirtualRelationEndPoints), "NotNullableForVirtualCollection", true, CreateEmptySortExpressionDefinition()));
      endPoints.Add(
          CreateVirtualObjectRelationEndPointDefinition(
              classDefinition, typeof(ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne", false));
      endPoints.Add(
          CreateDomainObjectCollectionRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithVirtualRelationEndPoints),
              "BidirectionalOneToManyForDomainObjectCollection",
              false,
              CreateSortExpressionDefinition("NoAttributeForDomainObjectCollection")));
      endPoints.Add(
          CreateVirtualCollectionRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithVirtualRelationEndPoints),
              "BidirectionalOneToManyForVirtualCollection",
              false,
              CreateSortExpressionDefinition("NoAttributeForVirtualCollection")));

      endPoints.Add(
          CreateVirtualObjectRelationEndPointDefinition(
              classDefinition, typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne", false));
      endPoints.Add(
          CreateDomainObjectCollectionRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithOneSideRelationPropertiesNotInMapping),
              "BaseBidirectionalOneToManyForDomainObjectCollection",
              false,
              CreateSortExpressionDefinition("NoAttributeForDomainObjectCollection")));
      endPoints.Add(
          CreateVirtualCollectionRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithOneSideRelationPropertiesNotInMapping),
              "BaseBidirectionalOneToManyForVirtualCollection",
              false,
              CreateSortExpressionDefinition("NoAttributeForVirtualCollection")));
      endPoints.Add(
          CreateVirtualObjectRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToOne",
              false));
      endPoints.Add(
          CreateDomainObjectCollectionRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToManyForDomainObjectCollection",
              false,
              CreateSortExpressionDefinition("NoAttributeForDomainObjectCollection")));
      endPoints.Add(
          CreateVirtualCollectionRelationEndPointDefinition(
              classDefinition,
              typeof(ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToManyForVirtualCollection",
              false,
              CreateSortExpressionDefinition("NoAttributeForVirtualCollection")));

      var classDefinitionFake = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassWithRealRelationEndPoints));
      foreach (var endPoint in endPoints)
      {
        var endPointStub = new Mock<IRelationEndPointDefinition>();
        endPointStub.Setup(stub => stub.ClassDefinition).Returns(classDefinitionFake);
        ((IRelationEndPointDefinitionSetter)endPoint).SetRelationDefinition(new RelationDefinition("fake: " + endPoint.PropertyName, endPoint, endPointStub.Object));
      }

      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(endPoints, true));
    }

    private void CreateEndPointDefinitionsForDerivedClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(endPoints, true));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (
        ClassDefinition classDefinition, Type declaringType, string shortPropertyName, bool isMandatory)
    {
      var propertyInfo = GetPropertyInfo(declaringType, shortPropertyName);

      return new RelationEndPointDefinition(
          classDefinition[MappingConfiguration.Current.NameResolver.GetPropertyName(propertyInfo)],
          isMandatory);
    }

    private VirtualObjectRelationEndPointDefinition CreateVirtualObjectRelationEndPointDefinition (
        ClassDefinition classDefinition,
        Type declaringType,
        string shortPropertyName,
        bool isMandatory)
    {
      var propertyInfo = GetPropertyInfo(declaringType, shortPropertyName);

      return new VirtualObjectRelationEndPointDefinition(
          classDefinition,
          MappingConfiguration.Current.NameResolver.GetPropertyName(propertyInfo),
          isMandatory,
          propertyInfo);
    }

    private DomainObjectCollectionRelationEndPointDefinition CreateDomainObjectCollectionRelationEndPointDefinition (
        ClassDefinition classDefinition,
        Type declaringType,
        string shortPropertyName,
        bool isMandatory,
        Lazy<SortExpressionDefinition> sortExpression)
    {
      var propertyInfo = GetPropertyInfo(declaringType, shortPropertyName);

      return new DomainObjectCollectionRelationEndPointDefinition(
          classDefinition,
          MappingConfiguration.Current.NameResolver.GetPropertyName(propertyInfo),
          isMandatory,
          sortExpression,
          propertyInfo);
    }

    private VirtualCollectionRelationEndPointDefinition CreateVirtualCollectionRelationEndPointDefinition (
        ClassDefinition classDefinition,
        Type declaringType,
        string shortPropertyName,
        bool isMandatory,
        Lazy<SortExpressionDefinition> sortExpression)
    {
      var propertyInfo = GetPropertyInfo(declaringType, shortPropertyName);

      return new VirtualCollectionRelationEndPointDefinition(
          classDefinition,
          MappingConfiguration.Current.NameResolver.GetPropertyName(propertyInfo),
          isMandatory,
          sortExpression,
          propertyInfo);
    }

    private IPropertyInformation GetPropertyInfo (Type declaringType, string shortPropertyName)
    {
      var propertyInfo = declaringType.GetProperty(
          shortPropertyName,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.That(propertyInfo, Is.Not.Null, "Property '" + shortPropertyName + "' not found on type '" + declaringType + "'.");
      return PropertyInfoAdapter.Create(propertyInfo);
    }

    private ClassDefinition CreateClassDefinition (string id, Type classType, bool isAbstract, ClassDefinition baseClass = null)
    {
        return new ClassDefinition(
                id,
                classType,
                isAbstract,
                baseClass,
                null,
                DefaultStorageClass.Persistent,
                new PersistentMixinFinderStub(classType),
                DomainObjectCreatorStub.Object);
    }

    private PropertyDefinition CreatePersistentPropertyDefinition (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        bool isNullable,
        int? maxLength)
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create(declaringClassType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      var fullName = MappingConfiguration.Current.NameResolver.GetPropertyName(propertyInfoAdapter);
      return new PropertyDefinition(
          classDefinition,
          propertyInfoAdapter,
          fullName,
          ReflectionUtility.IsDomainObject(propertyInfoAdapter.PropertyType),
          isNullable,
          maxLength,
          StorageClass.Persistent,
          propertyInfoAdapter.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfoAdapter.PropertyType) : null);
    }

    private Lazy<SortExpressionDefinition> CreateSortExpressionDefinition (string propertyName)
    {
      return new Lazy<SortExpressionDefinition>(
          () => new SortExpressionDefinition(
              new[]
              {
                  new SortedPropertySpecification(
                      PropertyDefinitionObjectMother.CreateForFakePropertyInfo(propertyName, StorageClass.Persistent),
                      SortOrder.Ascending)
              }));
    }

    private Lazy<SortExpressionDefinition> CreateEmptySortExpressionDefinition ()
    {
      return new Lazy<SortExpressionDefinition>(() => null);
    }
  }
}
