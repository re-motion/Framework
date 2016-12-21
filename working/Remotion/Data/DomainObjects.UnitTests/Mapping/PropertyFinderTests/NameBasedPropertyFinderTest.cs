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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class NameBasedPropertyFinderTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void FindMappingProperties_PropertyNameDoesNotExist ()
    {
      var classDefinition = CreateClassDefinition (typeof (DerivedClassWithMappingAttribute));
      var propertyFinder = new NameBasedPropertyFinder (
          "UnknownPropertyName",
          typeof (DerivedClassWithMappingAttribute),
          true,
          true,
          new ReflectionBasedMemberInformationNameResolver(),
          classDefinition.PersistentMixinFinder,
          new PropertyMetadataReflector());

      var properties = propertyFinder.FindPropertyInfos();

      Assert.That (properties.Length, Is.EqualTo (0));
    }

    [Test]
    public void FindMappingProperties_PropertyNameDoesExist ()
    {
      var classDefinition = CreateClassDefinition (typeof (DerivedClassWithMappingAttribute));
      var propertyFinder = new NameBasedPropertyFinder (
          "Property2",
          typeof (DerivedClassWithMappingAttribute),
          true,
          true,
          new ReflectionBasedMemberInformationNameResolver(),
          classDefinition.PersistentMixinFinder,
          new PropertyMetadataReflector());

      var properties = propertyFinder.FindPropertyInfos();

      Assert.That (properties.Length, Is.EqualTo (1));
      Assert.That (properties, Is.EqualTo (new[] { GetProperty (typeof (BaseMappingAttributesClass), "Property2") }));
    }

    [Test]
    public void CreateNewFinder ()
    {
      var classDefinition = CreateClassDefinition (typeof (DerivedClassWithMappingAttribute));
      var nameResolver = new ReflectionBasedMemberInformationNameResolver();
      IPropertyMetadataProvider propertyMetadataReflector = new PropertyMetadataReflector();
      var propertyFinder = new NameBasedPropertyFinder (
          "Property2",
          typeof (DerivedClassWithMappingAttribute),
          true,
          true,
          nameResolver,
          classDefinition.PersistentMixinFinder,
          propertyMetadataReflector);

      var result = (NameBasedPropertyFinder) PrivateInvoke.InvokeNonPublicMethod (
          propertyFinder,
          "CreateNewFinder",
          typeof (string),
          true,
          true,
          nameResolver,
          classDefinition.PersistentMixinFinder,
          propertyMetadataReflector);

      Assert.That (result.Type, Is.SameAs (typeof (string)));
      Assert.That (result.IncludeBaseProperties, Is.True);
      Assert.That (result.IncludeMixinProperties, Is.True);
      Assert.That (result.NameResolver, Is.SameAs(nameResolver));
      Assert.That (PrivateInvoke.GetNonPublicField (result, "_propertyName"), Is.EqualTo ("Property2"));
    }
  }
}