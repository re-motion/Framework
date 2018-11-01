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
using Remotion.UnitTests.Reflection.TestDomain.MethodInfoExtensions;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class GetOriginalDeclaringType
  {
    [Test]
    public void GetOriginalDeclaringType_ForMethodOnBaseClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<ClassWithDifferentMethods> ("GetString");

      Assert.That (MethodInfoExtensions.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentMethods)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForMethodOnDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedClassWithDifferentMethods> ("GetOtherString");

      Assert.That (MethodInfoExtensions.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (DerivedClassWithDifferentMethods)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForNewMethodOnDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedClassWithDifferentMethods> ("GetString");

      Assert.That (MethodInfoExtensions.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (DerivedClassWithDifferentMethods)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenMethodOnBaseClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<ClassWithDifferentMethods> ("GetInt32");

      Assert.That (MethodInfoExtensions.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentMethods)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenMethodOnDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedClassWithDifferentMethods> ("GetInt32");

      Assert.That (MethodInfoExtensions.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentMethods)));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenMethodOnDerivedOfDerivedClass ()
    {
      MethodInfo MethodInfo = GetMethodInfo<DerivedOfDerivedClassWithDifferentMethods> ("GetInt32");

      Assert.That (MethodInfoExtensions.GetOriginalDeclaringType (MethodInfo), Is.SameAs (typeof (ClassWithDifferentMethods)));
    }


    protected MethodInfo GetMethodInfo<T> (string method)
    {
      return typeof (T).GetMethod (method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }
  }
}
