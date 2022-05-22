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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.ReflectionBasedPropertyResolver;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class ReflectionBasedPropertyResolverTest : StandardMappingTest
  {
    [Test]
    public void ResolveDefinition ()
    {
      var property = PropertyInfoAdapter.Create(typeof(ClassWithSimpleProperties).GetProperty("StorageClassPersistentProperty"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithSimpleProperties));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithSimpleProperties), "StorageClassPersistentProperty");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_StorageClassNoneProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(ClassWithSimpleProperties).GetProperty("StorageClassNoneProperty"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithSimpleProperties));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithInterface));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithInterface), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromInterfaceProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithInterfaceExplicitImplementation));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var implementationPropertyName = property.DeclaringType.FullName + "." + property.Name;
      var expected = GetPropertyDefinition(typeof(ClassWithInterfaceExplicitImplementation), implementationPropertyName);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromInterfaceProperty_FromDerivedClass ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassDerivedFromClassWithInterfaceExplicitImplementation));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var implementationPropertyName = property.DeclaringType.FullName +  "." + property.Name;
      var expected = GetPropertyDefinition(typeof(ClassWithInterfaceExplicitImplementation), implementationPropertyName);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromImplementationProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(ClassWithInterface).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithInterface));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithInterface), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty_GetAccessorOnly ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithPropertiesWithSingleAccessors).GetProperty("PropertyWithGetAccessorOnly"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithInterfaceWithPropertiesWithSingleAccessors));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithInterfaceWithPropertiesWithSingleAccessors), "PropertyWithGetAccessorOnly");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_FromInterfaceProperty_SetAccessorOnly ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithPropertiesWithSingleAccessors).GetProperty("PropertyWithSetAccessorOnly"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithInterfaceWithPropertiesWithSingleAccessors));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithInterfaceWithPropertiesWithSingleAccessors), "PropertyWithSetAccessorOnly");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_InterfaceImplementation_NotImplemented ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithSimpleProperties)); // does not implement this property
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ResolveDefinition_ExplicitInterfaceImplementation_FromImplementationProperty ()
    {
      var property = CreatePropertyInfoAdapterForExplicitImplementation(
          typeof(IInterfaceWithProperty),
          "Property",
          typeof(ClassWithInterfaceExplicitImplementation));

      var classDefinition = GetTypeDefinition(typeof(ClassWithInterfaceExplicitImplementation));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithInterfaceExplicitImplementation), property.Name);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_MixinProperty_FromInterfaceProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithMixinWithPersistentProperty));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithMixinWithPersistentProperty), typeof(MixinWithPersistentProperty), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitMixinProperty_FromInterfaceProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithMixinWithPersistentPropertyExplicitImplementation));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var implementationPropertyName = property.DeclaringType.FullName + "." + property.Name;
      var expected = GetPropertyDefinition(
          typeof(ClassWithMixinWithPersistentPropertyExplicitImplementation),
          typeof(MixinWithPersistentPropertyExplicitImplementation),
          implementationPropertyName);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_MixinProperty_FromImplementationProperty ()
    {
      var property = PropertyInfoAdapter.Create(typeof(MixinWithPersistentProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithMixinWithPersistentProperty));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithMixinWithPersistentProperty), typeof(MixinWithPersistentProperty), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_ExplicitMixinProperty_FromImplementationProperty ()
    {
      var property = CreatePropertyInfoAdapterForExplicitImplementation(
          typeof(IInterfaceWithProperty),
          "Property",
          typeof(MixinWithPersistentPropertyExplicitImplementation));

      var classDefinition = GetTypeDefinition(typeof(ClassWithMixinWithPersistentPropertyExplicitImplementation));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(
          typeof(ClassWithMixinWithPersistentPropertyExplicitImplementation),
          typeof(MixinWithPersistentPropertyExplicitImplementation),
          property.Name);
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_MixinPropertyOnBaseClass ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassDerivedFromClassWithMixinWithPersistentProperty));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithMixinWithPersistentProperty), typeof(MixinWithPersistentProperty), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_MixinWithDuplicateInterface_DueToInheritance ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithDerivedMixinDerivedFromClassWithMixinWithPersistentProperty));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithMixinWithPersistentProperty), typeof(MixinWithPersistentProperty), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_ClassWithSameInterfaceAsMixin_ButStorageClassNone ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithSameInterfaceAsMixinWithStorageClassNone));
      var result = ReflectionBasedPropertyResolver.ResolveDefinition(property, classDefinition, classDefinition.GetPropertyDefinition);

      var expected = GetPropertyDefinition(typeof(ClassWithSameInterfaceAsMixinWithStorageClassNone), typeof(MixinWithPersistentProperty), "Property");
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void ResolveDefinition_ClassWithSameInterfaceAsMixin_BothStorageClassPersistent ()
    {
      var property = PropertyInfoAdapter.Create(typeof(IInterfaceWithProperty).GetProperty("Property"));

      var classDefinition = GetTypeDefinition(typeof(ClassWithSameInterfaceAsMixinWithStorageClassPersistent));
      Assert.That(() => ReflectionBasedPropertyResolver.ResolveDefinition(
          property, classDefinition, classDefinition.GetPropertyDefinition),
          Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(
              "The property 'Property' is ambiguous, it is implemented by the following types valid in the context of type "
              + "'ClassWithSameInterfaceAsMixinWithStorageClassPersistent': "
              + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.ReflectionBasedPropertyResolver.ClassWithSameInterfaceAsMixinWithStorageClassPersistent', "
              + "'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.ReflectionBasedPropertyResolver.MixinWithPersistentProperty'."));
    }

    private PropertyInfoAdapter CreatePropertyInfoAdapterForExplicitImplementation (Type interfaceType, string propertyName, Type implementationType)
    {
      var implementationPropertyName = interfaceType.FullName + "." + propertyName;
      return PropertyInfoAdapter.Create(implementationType.GetProperty(implementationPropertyName, BindingFlags.NonPublic | BindingFlags.Instance));
    }
  }
}
