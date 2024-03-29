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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class RelationPropertyFinderTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void Initialize ()
    {
      var classDefinition = CreateClassDefinition(typeof(ClassWithDifferentProperties));
      var propertyFinder = new RelationPropertyFinder(
          typeof(DerivedClassWithDifferentProperties),
          true,
          true,
          new ReflectionBasedMemberInformationNameResolver(),
          classDefinition.PersistentMixinFinder,
          new PropertyMetadataReflector());

      Assert.That(propertyFinder.Type, Is.SameAs(typeof(DerivedClassWithDifferentProperties)));
      Assert.That(propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForClassWithMixedProperties ()
    {
      var classDefinition = CreateClassDefinition(typeof(ClassWithDifferentProperties));
      var propertyFinder = new RelationPropertyFinder(
          typeof(ClassWithDifferentProperties),
          true,
          true,
          new ReflectionBasedMemberInformationNameResolver(),
          classDefinition.PersistentMixinFinder,
          new PropertyMetadataReflector());

      Assert.That(
          propertyFinder.FindPropertyInfos(),
          Is.EquivalentTo(
              new[]
              {
                  GetProperty(typeof(ClassWithDifferentPropertiesNotInMapping), "BaseUnidirectionalOneToOne"),
                  GetProperty(typeof(ClassWithDifferentPropertiesNotInMapping), "BasePrivateUnidirectionalOneToOne"),
                  GetProperty(typeof(ClassWithDifferentProperties), "UnidirectionalOneToOne")
              }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      var classDefinition = CreateClassDefinition(typeof(ClassWithVirtualRelationEndPoints));
      var propertyFinder = new RelationPropertyFinder(
          typeof(ClassWithVirtualRelationEndPoints),
          true,
          true,
          new ReflectionBasedMemberInformationNameResolver(),
          classDefinition.PersistentMixinFinder,
          new PropertyMetadataReflector());

      Assert.That(
          propertyFinder.FindPropertyInfos(),
          Is.EquivalentTo(
              new[]
              {
                  GetProperty(typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToOne"),
                  GetProperty(typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToManyForDomainObjectCollection"),
                  GetProperty(typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BaseBidirectionalOneToManyForVirtualCollection"),
                  GetProperty(typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToOne"),
                  GetProperty(typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToManyForDomainObjectCollection"),
                  GetProperty(typeof(ClassWithOneSideRelationPropertiesNotInMapping), "BasePrivateBidirectionalOneToManyForVirtualCollection"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "NoAttributeForDomainObjectCollection"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "NoAttributeForVirtualCollection"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "NotNullableForDomainObjectCollection"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "NotNullableForVirtualCollection"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "BidirectionalOneToOne"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "BidirectionalOneToManyForDomainObjectCollection"),
                  GetProperty(typeof(ClassWithVirtualRelationEndPoints), "BidirectionalOneToManyForVirtualCollection")
              }));
    }
  }
}
