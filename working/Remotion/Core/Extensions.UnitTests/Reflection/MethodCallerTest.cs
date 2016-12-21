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
using Remotion.Reflection;

namespace Remotion.Extensions.UnitTests.Reflection
{
  [TestFixture]
  public class MethodCallerTest
  {
    private delegate void NoOpOutInt(A a, out int i);

    private class A
    {
      protected readonly string Name;

      public A (string name)
      {
        Name = name;
      }

      public virtual string Say (string msg)
      {
        return msg + " " + Name + " from A";
      }

      public string GetString()
      {
        return "string";
      }

      public void NoOp()
      {
      }

      public void NoOp (int i)
      {
      }

      public void NoOp (out int i)
      {
        i = 1;
      }
    }

    private class B : A
    {
      public B (string name)
          : base (name)
      {
      }

      public override string Say (string msg)
      {
        return msg + " " + Name + " from B";
      }
    }

    private class C : B
    {
      public C (string name)
          : base (name)
      {
      }

      public new string Say (string msg)
      {
        return msg + " " + Name + " from C";
      }
    }

    //[Test]
    //public void TestOpenDelegate ()
    //{
    //  MethodInfo mi = typeof (A).GetMethod ("Say");
    //  Func<A, string, string> f = (Func<A, string, string>) Delegate.CreateDelegate (typeof (Func<A, string, string>), mi);

    //  A testClass = null;
    //  A TestClass = null;
    //  A myTestClass = null;

    //  Assert.AreEqual ("Hi foo from A", f (foo, "Hi"));
    //  Assert.AreEqual ("Hi bar from A", f (bar, "Hi"));

    //  // Assert.AreEqual ("Hi foo", MethodCaller.Call<string> ("Say").With (foo, "Hi"));

    //  A foo = new A ("foo");
    //  A bar = new A ("bar");
    //  B b = new B ("B");
    //  A b_as_a = b;
    //  C c = new C ("C");
    //  A c_as_a = c;
    //  B c_as_b = c;
    //}

    [Test]
    public void CallFunc()
    {
      A foo = new A ("foo");
      A bar = new A ("bar");
      B b = new B ("B");
      A b_as_a = b;
      C c = new C ("C");
      A c_as_a = c;
      B c_as_b = c;

      Func<A, string, string> f = MethodCaller.CallFunc<string> ("Say").GetDelegateWith<A, string> ();
      Assert.That (f (foo, "Hi"), Is.EqualTo ("Hi foo from A"));
      Assert.That (f (bar, "Hi"), Is.EqualTo ("Hi bar from A"));
      Assert.That (f (b, "Hi"), Is.EqualTo ("Hi B from B"));

      Assert.That (MethodCaller.CallFunc<string> ("Say").With (foo, "Hi"), Is.EqualTo ("Hi foo from A"));
      Assert.That (MethodCaller.CallFunc<string> ("Say").With (bar, "Hi"), Is.EqualTo ("Hi bar from A"));
      Assert.That (MethodCaller.CallFunc<string> ("Say").With (b, "Hi"), Is.EqualTo ("Hi B from B"));
      Assert.That (MethodCaller.CallFunc<string> ("Say").With (b_as_a, "Hi"), Is.EqualTo ("Hi B from B"));
      Assert.That (MethodCaller.CallFunc<string> ("Say").With (c, "Hi"), Is.EqualTo ("Hi C from C"));
      Assert.That (MethodCaller.CallFunc<string> ("Say").With (c_as_b, "Hi"), Is.EqualTo ("Hi C from B"));
      Assert.That (MethodCaller.CallFunc<string> ("Say").With (c_as_a, "Hi"), Is.EqualTo ("Hi C from B"));
    }
    
    [Test]
    public void CallAction()
    {
      A foo = new A ("foo");
      MethodCaller.CallAction ("NoOp").With (foo);
      MethodCaller.CallAction ("NoOp").With (foo, 0);
      int i;
      NoOpOutInt noop = MethodCaller.CallAction ("NoOp").GetDelegate<NoOpOutInt>();
      noop (foo, out i);
      Assert.That (i, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void NoThisArgument()
    {
      MethodCaller.CallFunc<string> ("GetString").With();
    }
  }
}
