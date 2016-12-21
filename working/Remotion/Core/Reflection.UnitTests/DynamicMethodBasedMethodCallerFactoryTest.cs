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
using System.Collections;
using System.Reflection;
using NUnit.Framework;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class DynamicMethodBasedMethodCallerFactoryTest
  {
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Global
    public interface IPublicInterfaceWithMethods
    {
      string ImplicitInterfaceMethod (string value);
      string ExplicitInterfaceMethod (string value);
    }

    private interface IPrivateInterfaceWithMethods
    {
      string ExplicitInterfaceMethod (string value);
    }

    internal interface IInternalInterfaceWithMethods
    {
      string ExplicitInterfaceMethod (string value);
    }

    protected interface IProtectedInterfaceWithMethods
    {
      string ExplicitInterfaceMethod (string value);
    }

    private class ClassWithMethods : IPublicInterfaceWithMethods, IPrivateInterfaceWithMethods, IProtectedInterfaceWithMethods, IInternalInterfaceWithMethods
    {
      public static string StaticValue { get; set; }

      public static string PublicStaticMethod (string value)
      {
        StaticValue = value;
        return value;
      }

      private static string NonPublicStaticMethod (string value)
      {
        StaticValue = value;
        return value;
      }

      public string InstanceValue { get; set; }

      public string PublicInstanceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }

      private string NonPublicInstanceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }

      public string ImplicitInterfaceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }

      string IPublicInterfaceWithMethods.ExplicitInterfaceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }

      string IPrivateInterfaceWithMethods.ExplicitInterfaceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }

      string IProtectedInterfaceWithMethods.ExplicitInterfaceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }

      string IInternalInterfaceWithMethods.ExplicitInterfaceMethod (string value)
      {
        InstanceValue = value;
        return value;
      }
    }

    [Test]
    public void GetMethodDelegate_PublicInstanceMethod ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("PublicInstanceMethod", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<ClassWithMethods, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<ClassWithMethods, string, string>));

      var obj = new ClassWithMethods();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_NonPublicInstanceMethod ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("NonPublicInstanceMethod", BindingFlags.NonPublic | BindingFlags.Instance);

      var @delegate = (Func<ClassWithMethods, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<ClassWithMethods, string, string>));

      var obj = new ClassWithMethods();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_PublicInterface_ImplicitInterfaceMethod ()
    {
      Type declaringType = typeof (IPublicInterfaceWithMethods);
      var methodInfo = declaringType.GetMethod ("ImplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<object, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<object, string, string>));

      var obj = new ClassWithMethods();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_PublicInterface_ExplicitInterfaceMethod ()
    {
      Type declaringType = typeof (IList);
      var methodInfo = declaringType.GetMethod ("Contains", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<object, object, bool>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<object, object, bool>));

      var obj = new[] { new object() };
      Assert.That (@delegate (obj, obj[0]), Is.True);
    }

    [Test]
    public void GetMethodDelegate_NestedPublicInterface_ExplicitInterfaceMethod ()
    {
      Type declaringType = typeof (IPublicInterfaceWithMethods);
      var methodInfo = declaringType.GetMethod ("ExplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<object, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<object, string, string>));

      var obj = new ClassWithMethods();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_NestedPrivateInterface_ExplicitInterfaceMethod ()
    {
      Type declaringType = typeof (IPrivateInterfaceWithMethods);
      var methodInfo = declaringType.GetMethod ("ExplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<object, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<object, string, string>));

      var obj = new ClassWithMethods ();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_NestedInternalInterface_ExplicitInterfaceMethod ()
    {
      Type declaringType = typeof (IInternalInterfaceWithMethods);
      var methodInfo = declaringType.GetMethod ("ExplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<object, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<object, string, string>));

      var obj = new ClassWithMethods ();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_NestedProtectedInterface_ExplicitInterfaceMethod ()
    {
      Type declaringType = typeof (IProtectedInterfaceWithMethods);
      var methodInfo = declaringType.GetMethod ("ExplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);

      var @delegate = (Func<object, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<object, string, string>));

      var obj = new ClassWithMethods ();
      Assert.That (@delegate (obj, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (obj.InstanceValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_PublicStaticMethod ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("PublicStaticMethod", BindingFlags.Public | BindingFlags.Static);

      var @delegate = (Func<ClassWithMethods, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<ClassWithMethods, string, string>));

      Assert.That (@delegate (null, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (ClassWithMethods.StaticValue, Is.EqualTo ("TheValue"));
    }

    [Test]
    public void GetMethodDelegate_NonPublicStaticMethod ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("NonPublicStaticMethod", BindingFlags.NonPublic | BindingFlags.Static);

      var @delegate = (Func<ClassWithMethods, string, string>) DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (
          methodInfo, typeof (Func<ClassWithMethods, string, string>));

      Assert.That (@delegate (null, "TheValue"), Is.EqualTo ("TheValue"));
      Assert.That (ClassWithMethods.StaticValue, Is.EqualTo ("TheValue"));
    }
  }
}