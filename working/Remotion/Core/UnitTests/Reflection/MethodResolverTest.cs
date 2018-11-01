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

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class MethodResolverTest
  {
    [Test]
    public void ResolveMethod_InstanceMethod ()
    {
      var method = MethodResolver.ResolveMethod (typeof (MethodResolverTest), "ResolveMethod_InstanceMethod", "Void ResolveMethod_InstanceMethod()");

      Assert.That (method, Is.EqualTo (MethodInfo.GetCurrentMethod()));
    }

    [Test]
    public void ResolveMethod_StaticMethod ()
    {
      var method = MethodResolver.ResolveMethod (typeof (object), "ReferenceEquals", "Boolean ReferenceEquals(System.Object, System.Object)");

      Assert.That (method, Is.EqualTo (typeof (object).GetMethod ("ReferenceEquals")));
    }

    [Test]
    public void ResolveMethod_MethodWithOverloads ()
    {
      var method = MethodResolver.ResolveMethod (typeof (Console), "WriteLine", "Void WriteLine(System.String)");

      Assert.That (method, Is.EqualTo (typeof (Console).GetMethod ("WriteLine", new[] { typeof (string) })));
    }

    [Test]
    public void ResolveMethod_ProtectedMethod ()
    {
      var method = MethodResolver.ResolveMethod (typeof (object), "Finalize", "Void Finalize()");

      Assert.That (method, Is.EqualTo (typeof (object).GetMethod ("Finalize", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void ResolveMethod_BaseMethodWithSameName ()
    {
      var method = MethodResolver.ResolveMethod (typeof (DateTime), "ToString", "System.String ToString()");

      Assert.That (method, Is.EqualTo (typeof (DateTime).GetMethod ("ToString", Type.EmptyTypes)));
      Assert.That (method, Is.Not.EqualTo (typeof (object).GetMethod ("ToString")));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "The method 'Void Foo()' could not be found on type 'System.Object'.")]
    public void ResolveMethod_NonMatchingName ()
    {
      MethodResolver.ResolveMethod (typeof (object), "Foo", "Void Foo()");
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "The method 'Void Foo()' could not be found on type 'System.Console'.")]
    public void ResolveMethod_NonMatchingSignature ()
    {
      MethodResolver.ResolveMethod (typeof (Console), "WriteLine", "Void Foo()");
    }
  }
}
