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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class RelationReflectorWithMixinsTest : MappingReflectionTestBase
  {
    private ClassDefinition _mixinTargetClassDefinition;
    private ClassDefinition _multiMixinTargetClassDefinition;
    private ClassDefinition _multiMixinRelatedClassDefinition;
    private ClassDefinition _relatedClassDefinition;
    private ClassDefinition _inheritanceRootInheritingMixinClassDefinition;
    private Dictionary<Type, ClassDefinition> _classDefinitions;

    public override void SetUp ()
    {
      base.SetUp();

      _mixinTargetClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(
          typeof(TargetClassForPersistentMixin),
          typeof(MixinAddingPersistentProperties));
      _multiMixinTargetClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(
          typeof(TargetClassReceivingTwoReferencesToDerivedClass),
          typeof(MixinAddingTwoReferencesToDerivedClass1),
          typeof(MixinAddingTwoReferencesToDerivedClass2));
      _multiMixinRelatedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(DerivedClassWithTwoBaseReferencesViaMixins));
      _relatedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(
          typeof(RelationTargetForPersistentMixin),
          typeof(MixinAddingPersistentProperties));
      _inheritanceRootInheritingMixinClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(InheritanceRootInheritingPersistentMixin),
          persistentMixinFinder: new PersistentMixinFinder(typeof(InheritanceRootInheritingPersistentMixin), true));

      _classDefinitions = new[] { _mixinTargetClassDefinition, _relatedClassDefinition, _multiMixinTargetClassDefinition }
          .ToDictionary(cd => cd.ClassType);
    }

    [Test]
    public void GetMetadata_Mixed_RealSide_ID ()
    {
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(
          actualRelationDefinition.ID,
          Is.EqualTo(
              typeof(TargetClassForPersistentMixin).FullName + ":" +
              typeof(MixinAddingPersistentProperties).FullName + ".UnidirectionalRelationProperty"));
    }

    [Test]
    public void GetMetadata_Mixed_VirtualSide ()
    {
      CreateRelationReflectorForProperty(_relatedClassDefinition, typeof(RelationTargetForPersistentMixin), "RelationProperty2");
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "VirtualRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(
          actualRelationDefinition.ID,
          Is.EqualTo(
              typeof(RelationTargetForPersistentMixin).FullName + ":" +
              typeof(RelationTargetForPersistentMixin).FullName + ".RelationProperty2->" + typeof(MixinAddingPersistentProperties)
              + ".VirtualRelationProperty"));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_EndPointDefinition0 ()
    {
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf(typeof(RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition)actualRelationDefinition.EndPointDefinitions[0];

      Assert.That(endPointDefinition.PropertyDefinition, Is.EqualTo(_mixinTargetClassDefinition.MyPropertyDefinitions[0]));
      Assert.That(endPointDefinition.ClassDefinition, Is.SameAs(_mixinTargetClassDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_Unidirectional_EndPointDefinition1 ()
    {
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "UnidirectionalRelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf(typeof(AnonymousRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (AnonymousRelationEndPointDefinition)actualRelationDefinition.EndPointDefinitions[1];

      Assert.That(oppositeEndPointDefinition.ClassDefinition, Is.SameAs(_relatedClassDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_EndPointDefinition0 ()
    {
      CreateRelationReflectorForProperty(_relatedClassDefinition, typeof(RelationTargetForPersistentMixin), "RelationProperty1");
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf(typeof(RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition)actualRelationDefinition.EndPointDefinitions[0];

      Assert.That(endPointDefinition.PropertyDefinition, Is.EqualTo(_mixinTargetClassDefinition.MyPropertyDefinitions[0]));
      Assert.That(endPointDefinition.ClassDefinition, Is.SameAs(_mixinTargetClassDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToOne_EndPointDefinition1 ()
    {
      CreateRelationReflectorForProperty(_relatedClassDefinition, typeof(RelationTargetForPersistentMixin), "RelationProperty1");
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "RelationProperty");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf(typeof(VirtualObjectRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (VirtualObjectRelationEndPointDefinition)actualRelationDefinition.EndPointDefinitions[1];

      Assert.That(oppositeEndPointDefinition.ClassDefinition, Is.SameAs(_relatedClassDefinition));
      Assert.That(
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.RelationTargetForPersistentMixin.RelationProperty1"));
      Assert.That(oppositeEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs(typeof(TargetClassForPersistentMixin)));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_EndPoint0 ()
    {
      CreateRelationReflectorForProperty(_relatedClassDefinition, typeof(RelationTargetForPersistentMixin), "RelationProperty4");
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "CollectionPropertyNSide");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(actualRelationDefinition.EndPointDefinitions[0], Is.InstanceOf(typeof(RelationEndPointDefinition)));

      var endPointDefinition = (RelationEndPointDefinition)actualRelationDefinition.EndPointDefinitions[0];

      Assert.That(endPointDefinition.PropertyDefinition, Is.EqualTo(_mixinTargetClassDefinition.MyPropertyDefinitions[0]));
      Assert.That(endPointDefinition.ClassDefinition, Is.SameAs(_mixinTargetClassDefinition));
    }

    [Test]
    public void GetMetadata_Mixed_BidirectionalOneToMany_EndPoint1 ()
    {
      CreateRelationReflectorForProperty(_relatedClassDefinition, typeof(RelationTargetForPersistentMixin), "RelationProperty4");
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(MixinAddingPersistentProperties),
          "CollectionPropertyNSide");

      RelationDefinition actualRelationDefinition = relationReflector.GetMetadata(_classDefinitions);
      Assert.That(actualRelationDefinition.EndPointDefinitions[1], Is.InstanceOf(typeof(DomainObjectCollectionRelationEndPointDefinition)));
      var oppositeEndPointDefinition = (DomainObjectCollectionRelationEndPointDefinition)actualRelationDefinition.EndPointDefinitions[1];
      Assert.That(oppositeEndPointDefinition.ClassDefinition, Is.SameAs(_relatedClassDefinition));
      Assert.That(
          oppositeEndPointDefinition.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.RelationTargetForPersistentMixin.RelationProperty4"));
      Assert.That(oppositeEndPointDefinition.PropertyInfo.PropertyType, Is.SameAs(typeof(ObjectList<TargetClassForPersistentMixin>)));
    }

    [Test]
    public void GetMetadata_Mixed_OppositePropertyPrivateOnBase ()
    {
      CreateRelationReflectorForProperty(_relatedClassDefinition, typeof(RelationTargetForPersistentMixin), "RelationProperty5");
      var relationReflector = CreateRelationReflectorForProperty(
          _mixinTargetClassDefinition,
          typeof(BaseForMixinAddingPersistentProperties),
          "PrivateBaseRelationProperty");

      Assert.That(relationReflector.GetMetadata(_classDefinitions), Is.Not.Null);
    }

    [Test]
    public void GetMetadata_Mixed_TwoMixins ()
    {
      CreateRelationReflectorForProperty(_multiMixinTargetClassDefinition, typeof(MixinAddingTwoReferencesToDerivedClass1), "MyDerived1");
      var relationReflector1 = CreateRelationReflectorForProperty(
          _multiMixinRelatedClassDefinition,
          typeof(DerivedClassWithTwoBaseReferencesViaMixins),
          "MyBase1");

      CreateRelationReflectorForProperty(_multiMixinTargetClassDefinition, typeof(MixinAddingTwoReferencesToDerivedClass2), "MyDerived2");
      var relationReflector2 = CreateRelationReflectorForProperty(
          _multiMixinRelatedClassDefinition,
          typeof(DerivedClassWithTwoBaseReferencesViaMixins),
          "MyBase2");

      var metadata1 = relationReflector1.GetMetadata(_classDefinitions);
      Assert.That(metadata1, Is.Not.Null);
      var metadata2 = relationReflector2.GetMetadata(_classDefinitions);
      Assert.That(metadata2, Is.Not.Null);
    }

    [Test]
    public void GetMetadata_Mixed_PropertyAboveInheritanceRoot ()
    {
      var classAboveInheritanceRoot = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(RelationTargetForPersistentMixinAboveInheritanceRoot));
      CreateRelationReflectorForProperty(classAboveInheritanceRoot, typeof(RelationTargetForPersistentMixinAboveInheritanceRoot), "RelationProperty1");
      var relationReflector = CreateRelationReflectorForProperty(
          _inheritanceRootInheritingMixinClassDefinition,
          typeof(MixinAddingPersistentPropertiesAboveInheritanceRoot),
          "PersistentRelationProperty");
      _classDefinitions.Add(classAboveInheritanceRoot.ClassType, classAboveInheritanceRoot);

      Assert.That(relationReflector.GetMetadata(_classDefinitions), Is.Not.Null);
    }

    private RelationReflector CreateRelationReflectorForProperty (
        ClassDefinition classDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = PropertyInfoAdapter.Create(declaringType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      var propertyReflector = new PropertyReflector(
          classDefinition,
          propertyInfo,
          new ReflectionBasedMemberInformationNameResolver(),
          PropertyMetadataProvider,
          DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);
      var propertyDefinition = propertyReflector.GetMetadata();
      var properties = new List<PropertyDefinition>();
      properties.Add(propertyDefinition);
      var propertyDefinitionsOfClass = (PropertyDefinitionCollection)PrivateInvoke.GetNonPublicField(classDefinition, "_propertyDefinitions");
      PrivateInvoke.SetNonPublicField(classDefinition, "_propertyDefinitions", null);
      if (propertyDefinitionsOfClass != null)
        properties.AddRange(propertyDefinitionsOfClass);
      classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(properties, true));

      var endPoint = MappingObjectFactory.CreateRelationEndPointDefinition(classDefinition, propertyInfo);
      var endPoints = new List<IRelationEndPointDefinition>();
      endPoints.Add(endPoint);
      var endPointDefinitionsOfClass = (RelationEndPointDefinitionCollection)PrivateInvoke.GetNonPublicField(classDefinition, "_relationEndPoints");
      PrivateInvoke.SetNonPublicField(classDefinition, "_relationEndPoints", null);
      if (endPointDefinitionsOfClass != null)
        endPoints.AddRange(endPointDefinitionsOfClass);
      classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(endPoints, true));

      return new RelationReflector(classDefinition, propertyInfo, new ReflectionBasedMemberInformationNameResolver(), PropertyMetadataProvider);
    }
  }
}
