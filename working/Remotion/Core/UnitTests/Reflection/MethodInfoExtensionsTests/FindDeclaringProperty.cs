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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.TestDomain.MethodInfoExtensions;

namespace Remotion.UnitTests.Reflection.MethodInfoExtensionsTests
{
  [TestFixture]
  public class FindDeclaringProperty
  {
    [Test]
    public void FindDeclaringProperty_StaticPropertyAccesor ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_StaticScalar");

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty ((typeof (ClassWithReferenceType<object>)), "StaticScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_PublicPropertyAccesor_Get ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty ((typeof (ClassWithReferenceType<object>)), "ImplicitInterfaceScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_PublicPropertyAccesor_Set ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("set_ImplicitInterfaceScalar");

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty ((typeof (ClassWithReferenceType<object>)), "ImplicitInterfaceScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_PrivatePropertyAccesor_Get ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty ((typeof (ClassWithReferenceType<object>)), "PrivateProperty", result);
    }

    [Test]
    public void FindDeclaringProperty_PrivatePropertyAccesor_Set ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("set_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty ((typeof (ClassWithReferenceType<object>)), "PrivateProperty", result);
    }

    [Test]
    public void FindDeclaringProperty_PrivatePropertyAccesorOfPublicProperty ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod (
          "set_ReadOnlyNonPublicSetterScalar", BindingFlags.Instance | BindingFlags.NonPublic);

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty ((typeof (ClassWithReferenceType<object>)), "ReadOnlyNonPublicSetterScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_ExplicitlyImplementedInterfacePropertyAccessorInBaseType ()
    {
      var methodInfo =
          typeof (DerivedClassWithReferenceType<object>)
              .GetInterfaceMap (typeof (IInterfaceWithReferenceType<object>)).TargetMethods
              .Where (
                  m =>
                  m.Name == "Remotion.UnitTests.Reflection.TestDomain.MethodInfoExtensions.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar")
              .Single();

      // We have a private property whose declaring type is different from the reflected type. It is not possible to get such a method via ordinary 
      // Reflection; only via GetInterfaceMap.
      Assert.That (methodInfo.DeclaringType, Is.SameAs (typeof (ClassWithReferenceType<object>)));
      Assert.That (methodInfo.ReflectedType, Is.SameAs (typeof (DerivedClassWithReferenceType<object>)));
      Assert.That (methodInfo.IsPrivate, Is.True);

      var result = methodInfo.FindDeclaringProperty();

      CheckProperty (
          (typeof (ClassWithReferenceType<object>)),
          "Remotion.UnitTests.Reflection.TestDomain.MethodInfoExtensions.IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar",
          result);
    }

    [Test]
    public void FindDeclaringProperty_NoPropertyCanBeFound ()
    {
      var methodInfo = typeof (ClassWithDifferentMethods).GetMethod ("GetStaticInt32");

      var result = methodInfo.FindDeclaringProperty();

      Assert.That (result, Is.Null);
    }

    private void CheckProperty (Type expectedDeclaringType, string expectedName, PropertyInfo actualProperty)
    {
      Assert.That (actualProperty.Name, Is.EqualTo (expectedName));
      Assert.That (actualProperty.DeclaringType, Is.SameAs (expectedDeclaringType));
    } 
  }
}