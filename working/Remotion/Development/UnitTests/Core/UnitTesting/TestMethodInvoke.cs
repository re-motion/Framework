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
using System.Text;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  public class TypeWithMethods
  {
    public string f ()
    {
      return "f";
    }

    public string f (int i)
    {
      return "f int";
    }

    public string f (int i, string s)
    {
      return "f int string";
    }

    public string f (string s)
    {
      return "f string";
    }

    public string f (StringBuilder sb)
    {
      return "f StringBuilder";
    }

    private string f (int i, StringBuilder s)
    {
      return "f int StringBuilder";
    }

    public static string s (int i)
    {
      return "s int";
    }

    private static string s (string s)
    {
      return "s string";
    }
  }

  public class DerivedType : TypeWithMethods
  {
  }

  [TestFixture]
	public class TestMethodInvoke
	{
    TypeWithMethods _twm = new TypeWithMethods();
    DerivedType _dt = new DerivedType();

    [Test]
    public void TestInvoke()
    {
      Assert.That (PrivateInvoke.InvokePublicMethod (_twm, "f"), Is.EqualTo ("f"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_twm, "f", 1), Is.EqualTo ("f int"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_twm, "f", 1, null), Is.EqualTo ("f int string"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_twm, "f", "test"), Is.EqualTo ("f string"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_twm, "f", new StringBuilder()), Is.EqualTo ("f StringBuilder"));
      Assert.That (PrivateInvoke.InvokeNonPublicMethod (_twm, "f", 1, new StringBuilder()), Is.EqualTo ("f int StringBuilder"));

      Assert.That (PrivateInvoke.InvokePublicMethod (_dt, "f"), Is.EqualTo ("f"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_dt, "f", 1), Is.EqualTo ("f int"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_dt, "f", 1, null), Is.EqualTo ("f int string"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_dt, "f", "test"), Is.EqualTo ("f string"));
      Assert.That (PrivateInvoke.InvokePublicMethod (_dt, "f", new StringBuilder()), Is.EqualTo ("f StringBuilder"));
      Assert.That (PrivateInvoke.InvokeNonPublicMethod (_dt, typeof (TypeWithMethods), "f", 1, new StringBuilder()), Is.EqualTo ("f int StringBuilder"));
    }

    [Test]
    public void TestStaticInvoke()
    {
      Assert.That (PrivateInvoke.InvokePublicStaticMethod (typeof(TypeWithMethods), "s", 1), Is.EqualTo ("s int"));
      Assert.That (PrivateInvoke.InvokeNonPublicStaticMethod (typeof(TypeWithMethods), "s", "test"), Is.EqualTo ("s string"));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException))]
    public void TestPublicInvokeAmbiguous()
    {
      PrivateInvoke.InvokePublicMethod (_twm, "f", null);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException))]
    public void TestPublicInvokeMethodNotFound()
    {
      PrivateInvoke.InvokePublicMethod (_twm, "f", 1.0);
    }
	}
}
