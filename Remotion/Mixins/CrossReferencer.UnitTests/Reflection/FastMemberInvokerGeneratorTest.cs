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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain.FastMethodInvoker;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Reflection
{
  [TestFixture]
  public class FastMemberInvokerGeneratorTest
  {
    private FastMemberInvokerGenerator _generator;

    [SetUp]
    public void SetUp()
    {
      _generator = new FastMemberInvokerGenerator ();
    }

    [Test]
    public void GetFastMethodInvoker_ForStaticMethod ()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType (),
          "IsNullOrEmpty", new Type[0],
          new[] { typeof (string) }, BindingFlags.Public | BindingFlags.Static);

      var output = invoker (null, new object[] { instance });

      Assert.That (output, Is.EqualTo (false));
    }

    [Test]
    public void GetFastMethodInvoker_ForInstanceMethod_WithoutOverloads ()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType (),
          "GetHashCode", new Type[0],
          Type.EmptyTypes, BindingFlags.Public | BindingFlags.Instance);

      var output = invoker (instance, new object[0]);

      Assert.That (output, Is.EqualTo (instance.GetHashCode()));
    }

    [Test]
    public void GetFastMethodInvoker_ForInstanceMethod_WithOverloads()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType(),
          "IndexOf", new Type[0],
          new[] { typeof (char) }, BindingFlags.Public | BindingFlags.Instance);
      
      var output = invoker(instance, new object[] { 't' });

      Assert.That (output, Is.EqualTo (1));
    }

    [Test]
    public void GetFastMethodInvoker_ForGenericMethod()
    {
      var instance = new ClassWithMethods();
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType(),
          "Count", new[] { typeof(int) },
          new[] { typeof (IEnumerable<int>) }, BindingFlags.Public | BindingFlags.Instance);
      
      var output = invoker(instance, new object[] { new[] { 3, 1, 2 } });

      Assert.That (output, Is.EqualTo (3));
    }

    [Test]
    public void GetFastMethodInvoker_ForStaticGenericMethod()
    {
      var instance = new ClassWithMethods();
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType(),
          "Count", new[] { typeof(int) },
          new[] { typeof (IEnumerable<int>), typeof(int) }, BindingFlags.Public | BindingFlags.Static);
      
      var output = invoker(instance, new object[] { new[] { 3, 1, 2 }, 1 });

      Assert.That (output, Is.EqualTo (4));
    }

    [Test]
    public void GetFastMethodInvoker_ForGenericMethodTwoParameters ()
    {
      var instance = new ClassWithMethods ();
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType (),
          "Count", new[] { typeof (int), typeof(string) },
          new[] { typeof (IEnumerable<int>), typeof (string) }, BindingFlags.Public | BindingFlags.Instance);

      var output = invoker (instance, new object[] { new[] { 3, 1, 2 }, "asdf" });

      Assert.That (output, Is.EqualTo (7));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Method 'Foo' not found on type 'System.String'.")]
    public void GetFastMethodInvoker_ForNonExistingMethod ()
    {
      var instance = "stringContent";
      _generator.GetFastMethodInvoker (
          instance.GetType (),
          "Foo", new Type[0],
          new[] { typeof (string) }, BindingFlags.Public | BindingFlags.Static);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Overload of method 'GetHashCode' not found on type 'System.String'.")]
    public void GetFastMethodInvoker_ForExistingMethod_WithInvalidSignature ()
    {
      var instance = "stringContent";
      _generator.GetFastMethodInvoker (
          instance.GetType (),
          "GetHashCode", new Type[0],
          new[] { typeof (string) }, BindingFlags.Public | BindingFlags.Instance);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Void methods are not supported.")]
    public void GetFastMethodInvoker_ForVoidMethod ()
    {
      var instance = new TargetDoSomething();
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType (),
          "DoSomething", new Type[0],
          Type.EmptyTypes,
          BindingFlags.Public | BindingFlags.Instance);

      var output = invoker (instance, new object[0]);

      Assert.That (output, Is.Null);
    }
  }
}
