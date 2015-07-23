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
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.TestDomain.PropertyInfoExtensions;

namespace Remotion.UnitTests.Reflection.PropertyInfoExtensionsTests
{
  [TestFixture]
  public class GetBaseDefinition
  {
    [Test]
    public void GetBaseDefinition_ForPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("String");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (propertyInfo));
    }

    [Test]
    public void GetBaseDefinition_ForPropertyOnOpenGenericBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (GenericClassWithDifferentProperties<>), "AbstractT");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (propertyInfo));
    }

    [Test]
    public void GetBaseDefinition_ForPropertyOnClosedGenericBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<GenericClassWithDifferentProperties<int>> ("AbstractT");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (propertyInfo));
    }

    [Test]
    public void GetBaseDefinition_ForPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("OtherString");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<DerivedClassWithDifferentProperties> ("OtherString")));
    }

    [Test]
    public void GetBaseDefinition_ForNewPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("String");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<DerivedClassWithDifferentProperties> ("String")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("Int32");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<ClassWithDifferentProperties> ("Int32")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithDifferentProperties> ("Int32");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<ClassWithDifferentProperties> ("Int32")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenPropertyOnOpenGenericDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (DerivedOpenGenericClassWithDifferentProperties<>), "AbstractT");

      var baseDefinition = propertyInfo.GetBaseDefinition();
      Assert.That (baseDefinition.DeclaringType.Name, Is.EqualTo (typeof (GenericClassWithDifferentProperties<>).Name));
      Assert.That (baseDefinition.Name, Is.EqualTo (propertyInfo.Name));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenPropertyOnClosedGenericDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedOpenGenericClassWithDifferentProperties<int>> ("AbstractT");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo (typeof (GenericClassWithDifferentProperties<int>), "AbstractT")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenPropertyOnDerivedClassWithClosedGenericBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClosedGenericClassWithDifferentProperties> ("AbstractT");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo (typeof (GenericClassWithDifferentProperties<int>), "AbstractT")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenPropertyOnDerivedOfDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedOfDerivedClassWithDifferentProperties> ("Int32");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<ClassWithDifferentProperties> ("Int32")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenNonPublicPropertyOnDerivedOfDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedOfDerivedClassWithDifferentProperties> ("ProtectedInt32");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<ClassWithDifferentProperties> ("ProtectedInt32")));
    }

    [Test]
    public void GetBaseDefinition_ForStaticPublicProperty ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedOfDerivedClassWithDifferentProperties> ("StaticInt32");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<DerivedOfDerivedClassWithDifferentProperties> ("StaticInt32")));
    }

    [Test]
    public void GetBaseDefinition_ForStaticNonPublicProperty ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithDifferentProperties> ("PrivateStaticInt32");

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (GetPropertyInfo<ClassWithDifferentProperties> ("PrivateStaticInt32")));
    }

    [Test]
    public void GetBaseDefinition_ForOverriddenIndexedPropertyOnDerivedOfDerivedClass ()
    {
      PropertyInfo propertyInfo = typeof (DerivedOfDerivedClassWithDifferentProperties).GetProperty ("Item", new[] { typeof (int) });

      Assert.That (propertyInfo.GetBaseDefinition(), Is.SameAs (typeof (ClassWithDifferentProperties).GetProperty ("Item", new[] { typeof (int) })));
    }

    private PropertyInfo GetPropertyInfo<T> (string property)
    {
      return GetPropertyInfo (typeof (T), property);
    }

    private PropertyInfo GetPropertyInfo (Type type, string property)
    {
      return type.GetProperty (
          property,
          BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
    }
  }
}