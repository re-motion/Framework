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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;
using Rhino.Mocks;

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
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseString")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseUnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateUnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "UnidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (true);
      
      ClassIDProviderStub.Stub(stub => stub.GetClassID (typeof (ClassWithDifferentProperties))).Return ("ClassWithDifferentProperties");
      
      var classReflector = CreateClassReflector (typeof (ClassWithDifferentProperties));
      var expected = CreateClassWithDifferentPropertiesClassDefinition();

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual, Is.Not.Null);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForDerivedClass ()
    {
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "OtherString")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "String")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "PrivateString")))
          .Return (true);

      ClassIDProviderStub.Stub(stub => stub.GetClassID (typeof (DerivedClassWithDifferentProperties))).Return ("DerivedClassWithDifferentProperties");

      var classReflector = CreateClassReflector (typeof (DerivedClassWithDifferentProperties));
      var expected = CreateDerivedClassWithDifferentPropertiesClassDefinition();

      var baseClassDefinition = CreateClassWithDifferentPropertiesClassDefinition();
      var actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.That (actual, Is.Not.Null);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_ForMixedClass ()
    {
      ClassIDProviderStub.Stub (stub => stub.GetClassID (typeof (TargetClassA))).Return ("ClassID");

      var classReflector = CreateClassReflector (typeof (TargetClassA));
      var actual = classReflector.GetMetadata (null);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinA), typeof (MixinC), typeof (MixinD) }));
    }

    [Test]
    public void GetMetadata_ForDerivedMixedClass ()
    {
      ClassIDProviderStub.Stub (stub => stub.GetClassID (typeof (TargetClassA))).Return ("ClassID");
      ClassIDProviderStub.Stub (stub => stub.GetClassID (typeof (TargetClassB))).Return ("ClassID");

      var classReflectorForBaseClass = CreateClassReflector (typeof (TargetClassA));
      var baseClass = classReflectorForBaseClass.GetMetadata (null);

      var classReflector = CreateClassReflector (typeof (TargetClassB));
      var actual = classReflector.GetMetadata (baseClass);
      Assert.That (actual.PersistentMixins, Is.EquivalentTo (new[] { typeof (MixinB), typeof (MixinE) }));
    }

    [Test]
    public void GetMetadata_ForClassWithVirtualRelationEndPoints ()
    {
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BaseBidirectionalOneToMany")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateBidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BasePrivateBidirectionalOneToMany")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NoAttribute")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "NotNullable")))
          .Return (false);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToOne")))
          .Return (true);
      DomainModelConstraintProviderStub
          .Stub (stub => stub.IsNullable (Arg<IPropertyInformation>.Matches (pi => pi.Name == "BidirectionalOneToMany")))
          .Return (true);

      ClassIDProviderStub.Stub(stub => stub.GetClassID (typeof (ClassWithVirtualRelationEndPoints))).Return ("ClassWithVirtualRelationEndPoints");

      var classReflector = CreateClassReflector (typeof (ClassWithVirtualRelationEndPoints));
      var expected = CreateClassWithVirtualRelationEndPointsClassDefinition();
      expected.SetPropertyDefinitions (new PropertyDefinitionCollection());
      CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints (expected);

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual, Is.Not.Null);
      _classDefinitionChecker.Check (expected, actual);
      _endPointDefinitionChecker.Check (expected.MyRelationEndPointDefinitions, actual.MyRelationEndPointDefinitions, false);
    }

    [Test]
    public void GetMetadata_GetClassID ()
    {
      ClassIDProviderStub.Stub(stub => stub.GetClassID (typeof (ClassHavingClassIDAttribute))).Return ("ClassIDForClassHavingClassIDAttribute");

      var classReflector = CreateClassReflector (typeof (ClassHavingClassIDAttribute));

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.ID, Is.EqualTo ("ClassIDForClassHavingClassIDAttribute"));
    }

    [Test]
    public void GetMetadata_ForClosedGenericClass ()
    {
      ClassIDProviderStub.Stub (stub => stub.GetClassID (typeof (ClosedGenericClass))).Return ("ClassID");

      var classReflector = CreateClassReflector (typeof (ClosedGenericClass));

      Assert.That (classReflector.GetMetadata (null), Is.Not.Null);
    }

    [Test]
    public void GetMetadata_ForClassWithoutStorageGroupAttribute ()
    {
      var type = typeof (ClassWithoutStorageGroupWithDifferentProperties);
      ClassIDProviderStub.Stub(stub => stub.GetClassID (type)).Return ("ClassID");
      Assert.That (type.IsDefined (typeof (DBStorageGroupAttribute), false), Is.False);
  
      var classReflector = CreateClassReflector (type);

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.StorageGroupType, Is.Null);
    }

    [Test]
    public void GetMetadata_ForClassWithStorageGroupAttribute ()
    {
      var type = typeof (ClassWithStorageGroupAttributeAndBaseClass);
      ClassIDProviderStub.Stub (stub => stub.GetClassID (type)).Return ("ClassID");
      Assert.That (type.IsDefined (typeof (DBStorageGroupAttribute), false), Is.True);

      var classReflector = CreateClassReflector (type);

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForBaseClass ()
    {
      
      ClassIDProviderStub.Stub(mock => mock.GetClassID (typeof (ClassWithDifferentProperties))).Return ("ClassID");

      var classReflector = CreateClassReflector (typeof (ClassWithDifferentProperties));

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.True);
    }

    [Test]
    public void GetMetadata_PersistentMixinFinder_ForDerivedClass ()
    {
      ClassIDProviderStub.Stub (stub => stub.GetClassID (typeof (DerivedClassWithDifferentProperties))).Return ("ClassID");

      var classReflector = CreateClassReflector (typeof (DerivedClassWithDifferentProperties));
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition_WithEmptyMembers_AndDerivedClasses();

      var actual = classReflector.GetMetadata (baseClassDefinition);

      Assert.That (actual.PersistentMixinFinder.IncludeInherited, Is.False);
    }

    [Test]
    public void GetMetadata_InstanceCreator ()
    {
      ClassIDProviderStub.Stub (stub => stub.GetClassID (typeof (ClassWithDifferentProperties))).Return ("ClassID");

      var classReflector = CreateClassReflector (typeof (ClassWithDifferentProperties));

      var actual = classReflector.GetMetadata (null);

      Assert.That (actual.InstanceCreator, Is.SameAs (DomainObjectCreatorStub));
    }

    private ClassReflector CreateClassReflector (Type type)
    {
      return new ClassReflector (
          type,
          MappingObjectFactory,
          Configuration.NameResolver,
          ClassIDProviderStub,
          PropertyMetadataProvider, 
          DomainModelConstraintProviderStub,
          DomainObjectCreatorStub);
    }

    private ClassDefinition CreateClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = CreateClassDefinition ("ClassWithDifferentProperties", typeof (ClassWithDifferentProperties), false);

      CreatePropertyDefinitionsForClassWithDifferentProperties (classDefinition);
      CreateEndPointDefinitionsForClassWithDifferentProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateDerivedClassWithDifferentPropertiesClassDefinition ()
    {
      var classDefinition = CreateClassDefinition (
          "DerivedClassWithDifferentProperties",
          typeof (DerivedClassWithDifferentProperties),
          false,
          CreateClassWithDifferentPropertiesClassDefinition());
      CreatePropertyDefinitionsForDerivedClassWithDifferentProperties (classDefinition);
      CreateEndPointDefinitionsForDerivedClassWithDifferentProperties (classDefinition);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithVirtualRelationEndPointsClassDefinition ()
    {
      var classDefinition = CreateClassDefinition (
          "ClassWithVirtualRelationEndPoints",
          typeof (ClassWithVirtualRelationEndPoints),
          false);

      return classDefinition;
    }

    private void CreatePropertyDefinitionsForClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BaseString", true, null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (ClassWithDifferentPropertiesNotInMapping),
              "BaseUnidirectionalOneToOne",
              true,
              null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (ClassWithDifferentPropertiesNotInMapping),
              "BasePrivateUnidirectionalOneToOne",
              true,
              null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition, typeof (ClassWithDifferentProperties), "Int32", false, null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition, typeof (ClassWithDifferentProperties), "String", true, null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (ClassWithDifferentProperties),
              "PrivateString",
              true,
              null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (ClassWithDifferentProperties),
              "UnidirectionalOneToOne",
              true,
              null));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    private void CreateEndPointDefinitionsForClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();
      endPoints.Add (CreateRelationEndPointDefinition (classDefinition, typeof (ClassWithDifferentProperties), "UnidirectionalOneToOne", false));
      endPoints.Add (
          CreateRelationEndPointDefinition (classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne", false));
      endPoints.Add (
          CreateRelationEndPointDefinition (
              classDefinition, typeof (ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne", false));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (endPoints, true));
    }

    private void CreatePropertyDefinitionsForDerivedClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var properties = new List<PropertyDefinition>();
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (DerivedClassWithDifferentProperties),
              "String",
              true,
              null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (DerivedClassWithDifferentProperties),
              "PrivateString",
              true,
              null));
      properties.Add (
          CreatePersistentPropertyDefinition (
              classDefinition,
              typeof (DerivedClassWithDifferentProperties),
              "OtherString",
              true,
              null));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));
    }

    private void CreateEndPointDefinitionsForClassWithVirtualRelationEndPoints (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();

      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "NoAttribute", false, CardinalityType.Many, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "NotNullable", true, CardinalityType.Many, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne", false, CardinalityType.One, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithVirtualRelationEndPoints), "BidirectionalOneToMany", false, CardinalityType.Many, "NoAttribute"));

      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition, typeof (ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne", false, CardinalityType.One, null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition,
              typeof (ClassWithOneSideRelationPropertiesNotInMapping),
              "BaseBidirectionalOneToMany",
              false,
              CardinalityType.Many,
              "NoAttribute"));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition,
              typeof (ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToOne",
              false,
              CardinalityType.One,
              null));
      endPoints.Add (
          CreateVirtualRelationEndPointDefinition (
              classDefinition,
              typeof (ClassWithOneSideRelationPropertiesNotInMapping),
              "BasePrivateBidirectionalOneToMany",
              false,
              CardinalityType.Many,
              "NoAttribute"));

      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (endPoints, true));
    }

    private void CreateEndPointDefinitionsForDerivedClassWithDifferentProperties (ClassDefinition classDefinition)
    {
      var endPoints = new List<IRelationEndPointDefinition>();
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (endPoints, true));
    }

    private RelationEndPointDefinition CreateRelationEndPointDefinition (
        ClassDefinition classDefinition, Type declaringType, string shortPropertyName, bool isMandatory)
    {
      var propertyInfo = GetPropertyInfo (declaringType, shortPropertyName);

      return new RelationEndPointDefinition (
          classDefinition[MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo)],
          isMandatory);
    }

    private VirtualRelationEndPointDefinition CreateVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        Type declaringType,
        string shortPropertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string sortExpressionText)
    {
      var propertyInfo = GetPropertyInfo (declaringType, shortPropertyName);

      return new VirtualRelationEndPointDefinition (
          classDefinition,
          MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfo),
          isMandatory,
          cardinality,
          sortExpressionText,
          propertyInfo);
    }

    private IPropertyInformation GetPropertyInfo (Type declaringType, string shortPropertyName)
    {
      var propertyInfo = declaringType.GetProperty (
          shortPropertyName,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.That (propertyInfo, Is.Not.Null, "Property '" + shortPropertyName + "' not found on type '" + declaringType + "'.");
      return PropertyInfoAdapter.Create (propertyInfo);
    }

    private ClassDefinition CreateClassDefinition (string id, Type classType, bool isAbstract, ClassDefinition baseClass = null)
    {
      return new ClassDefinition (id, classType, isAbstract, baseClass, null, new PersistentMixinFinderStub (classType), DomainObjectCreatorStub);
    }

    private PropertyDefinition CreatePersistentPropertyDefinition (
        ClassDefinition classDefinition,
        Type declaringClassType,
        string propertyName,
        bool isNullable,
        int? maxLength)
    {
      var propertyInfoAdapter = PropertyInfoAdapter.Create (declaringClassType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      var fullName = MappingConfiguration.Current.NameResolver.GetPropertyName (propertyInfoAdapter);
      return new PropertyDefinition (
          classDefinition,
          propertyInfoAdapter,
          fullName,
          ReflectionUtility.IsDomainObject (propertyInfoAdapter.PropertyType),
          isNullable,
          maxLength,
          StorageClass.Persistent);
    }
  }
}