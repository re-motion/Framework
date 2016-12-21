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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReflectionBasedPropertyFinderTestDomain;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class ReflectionBasedPropertyFinderTest
  {
    [Test]
    public void ReturnsPublicInstancePropertiesFromThisAndBase ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestType));
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (
          properties,
          Is.EquivalentTo (
              new object[]
                  {
                      typeof (TestType).GetProperty ("PublicInstanceProperty"),
                      typeof (BaseTestType).GetProperty ("BasePublicInstanceProperty")
                  }));
    }

    [Test]
    public void IgnoresBasePropertiesWithSameName ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeHidingProperties));
      var properties = new List<PropertyInfo> (UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (
          properties,
          Is.EquivalentTo (
              new object[]
                  {
                      typeof (TestTypeHidingProperties).GetProperty ("PublicInstanceProperty"),
                      typeof (TestTypeHidingProperties).GetProperty ("BasePublicInstanceProperty")
                  }));
    }

    [Test]
    public void IgnoresPropertiesWithOjectBindingVisibleFalseAttribute ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (ClassWithReferenceType<object>));
      var properties = finder.GetPropertyInfos();
     
      Assert.That (properties.Where (p => p.Name == "NotVisibleAttributeScalar").Count (), Is.EqualTo (0));
    }

    [Test]
    public void IgnoresIndexedProperties ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (ClassWithReferenceType<object>));
      var properties = finder.GetPropertyInfos();
      
      Assert.That (properties.Where (p => p.Name == "Item").Count(), Is.EqualTo (0));
    }

    [Test]
    public void FindsPropertiesFromImplicitInterfaceImplementations ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      var properties = finder.GetPropertyInfos();
      Assert.That (properties.Where (p => p.Name == "InterfaceProperty").Count(), Is.EqualTo (1));
    }
    
    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementations ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      var properties = finder.GetPropertyInfos();
      Assert.That (properties.Where (p => p.Name == "Remotion.ObjectBinding.UnitTests.BindableObject.ReflectionBasedPropertyFinderTestDomain.IExplicitTestInterface.InterfaceProperty").Count (), Is.EqualTo (1));
    }

    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementationsOnBase ()
    {
      var finder = new ReflectionBasedPropertyFinder (typeof (DerivedTypeWithInterfaces));
      var properties = finder.GetPropertyInfos();
      Assert.That (properties.Where (p => p.Name == "Remotion.ObjectBinding.UnitTests.BindableObject.ReflectionBasedPropertyFinderTestDomain.IExplicitTestInterface.InterfaceProperty").Count (), Is.EqualTo (1));
    }

    [Test]
    public void NoPropertiesFromBindableObjectMixins ()
    {
      Type targetType = typeof (ClassWithIdentity);
      Type concreteType = MixinTypeUtility.GetConcreteMixedType (targetType);

      var targetTypeProperties = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (targetType).GetPropertyInfos ());
      var concreteTypeProperties = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (concreteType).GetPropertyInfos ());

      Assert.That (concreteTypeProperties, Is.EquivalentTo (targetTypeProperties));
    }

    [Test]
    public void PropertyWithDoubleInterfaceMethod ()
    {
      var propertyInfos = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (typeof (ClassWithDoubleInterfaceProperty)).GetPropertyInfos ());
      Assert.That (propertyInfos.Count, Is.EqualTo (1));
      Assert.That (propertyInfos[0].Name, Is.EqualTo ("DisplayName"));
    }

    [Test]
    public void ImplicitInterfaceProperties_GetInterfaceBasedPropertyInfo()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceImplProperty = (from p in propertyInfos
                               where p.Name == "InterfaceProperty"
                               select p).Single ();
      
      Assert.That (interfaceImplProperty, Is.TypeOf (typeof (InterfaceImplementationPropertyInformation)));
      Assert.That (interfaceImplProperty.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (TestTypeWithInterfaces))));
      Assert.That (interfaceImplProperty.FindInterfaceDeclarations().Single().DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (ITestInterface))));
    }

    [Test]
    public void ExplicitInterfaceProperties_GetInterfaceBasedPropertyInfo ()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceImplProperty = (from p in propertyInfos
                               where p.Name == typeof (IExplicitTestInterface).FullName + ".InterfaceProperty"
                               select p).Single ();

      Assert.That (interfaceImplProperty, Is.TypeOf (typeof (InterfaceImplementationPropertyInformation)));
      
      Assert.That (interfaceImplProperty, Is.TypeOf (typeof (InterfaceImplementationPropertyInformation)));
      Assert.That (interfaceImplProperty.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (TestTypeWithInterfaces))));
      Assert.That (interfaceImplProperty.FindInterfaceDeclarations ().Single ().DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (IExplicitTestInterface))));
    }

    [Test]
    public void InterfaceProperties_PropertyWithoutGetter ()
    {
      var propertyInfos = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces)).GetPropertyInfos ().ToArray ();
      var interfaceProperty = (InterfaceImplementationPropertyInformation) (from p in propertyInfos
                                                     where p.Name == "NonGetterInterfaceProperty"
                                                     select p).SingleOrDefault ();
      Assert.That (interfaceProperty, Is.Null);
    }

    [Test]
    public void GetPropertyInfos_MixedProperties ()
    {
      var concreteType = TypeFactory.GetConcreteType (typeof (ClassWithMixedProperty));
      var propertyFinder = new ReflectionBasedPropertyFinder (concreteType);
      var propertyInformations = propertyFinder.GetPropertyInfos().OrderBy (pi => pi.Name).ToArray();

      Assert.That (propertyInformations.Length, Is.EqualTo (10));

      var propertyInformation_0 = propertyInformations[0];
      Assert.That (propertyInformation_0, Is.TypeOf (typeof (InterfaceImplementationPropertyInformation)));
      Assert.That (propertyInformation_0.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (ClassWithMixedProperty))));
      Assert.That (propertyInformation_0.Name, Is.EqualTo ("InterfaceProperty"));

      var propertyInformation_1 = propertyInformations[1];
      Assert.That (propertyInformation_1, Is.TypeOf (typeof (MixinIntroducedPropertyInformation)));
      Assert.That (propertyInformation_1.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (MixinAddingProperty))));
      Assert.That (propertyInformation_1.Name, Is.EqualTo ("MixedProperty"));

      var propertyInformation_2 = propertyInformations[4];
      Assert.That (propertyInformation_2, Is.TypeOf (typeof (MixinIntroducedPropertyInformation)));
      Assert.That (propertyInformation_2.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (MixinAddingProperty))));
      Assert.That (propertyInformation_2.Name, Is.EqualTo ("MixedReadOnlyProperty"));

      var propertyInformation_3 = propertyInformations[5];
      Assert.That (propertyInformation_3, Is.TypeOf (typeof (MixinIntroducedPropertyInformation)));
      Assert.That (propertyInformation_3.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (MixinAddingProperty))));
      Assert.That (propertyInformation_3.Name, Is.EqualTo ("MixedReadOnlyPropertyHavingSetterOnMixin"));

      var propertyInformation_4 = propertyInformations[6];
      Assert.That (propertyInformation_4, Is.TypeOf (typeof (PropertyInfoAdapter)));
      Assert.That (propertyInformation_4.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (ClassWithMixedProperty))));
      Assert.That (propertyInformation_4.Name, Is.EqualTo ("PublicExistingProperty"));

      var propertyInformation_5 = propertyInformations[7];
      Assert.That (propertyInformation_5, Is.TypeOf (typeof (MixinIntroducedPropertyInformation)));
      Assert.That (propertyInformation_5.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (BaseOfMixinAddingProperty))));
      Assert.That (
          propertyInformation_5.Name,
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.TestDomain.IBaseOfMixinAddingProperty.ExplicitMixedPropertyBase"));

      var propertyInformation_6 = propertyInformations[8];
      Assert.That (propertyInformation_6, Is.TypeOf (typeof (MixinIntroducedPropertyInformation)));
      Assert.That (propertyInformation_6.DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (MixinAddingProperty))));
      Assert.That (
          propertyInformation_6.Name, 
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.TestDomain.IMixinAddingProperty.ExplicitMixedProperty"));
    }

    [Test]
    public void GetPropertyInfos_MixedProperties_DuplicatesNotRemoved ()
    {
      var concreteType = TypeFactory.GetConcreteType (typeof (ClassWithMixedPropertyOfSameName));
      var propertyFinder = new ReflectionBasedPropertyFinder (concreteType);
      var propertyInformations = propertyFinder.GetPropertyInfos().OrderBy (pi => pi.Name).ThenBy (pi => pi.DeclaringType.FullName).ToArray();

      Assert.That (propertyInformations[0], Is.TypeOf (typeof (PropertyInfoAdapter)));
      Assert.That (propertyInformations[0].DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (ClassWithMixedPropertyOfSameName))));
      Assert.That (propertyInformations[0].Name, Is.EqualTo ("MixedProperty"));

      Assert.That (propertyInformations[1], Is.TypeOf (typeof (MixinIntroducedPropertyInformation)));
      Assert.That (propertyInformations[1].DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (MixinAddingProperty))));
      Assert.That (propertyInformations[1].Name, Is.EqualTo ("MixedProperty"));
    }

    private IEnumerable<PropertyInfo> UnwrapCollection (IEnumerable<IPropertyInformation> properties)
    {
      return from PropertyInfoAdapter adapter in properties 
             select adapter.PropertyInfo;
    }
  }
}
