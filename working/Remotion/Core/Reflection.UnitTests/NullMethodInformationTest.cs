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
using System.Runtime.Remoting.Contexts;
using NUnit.Framework;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class NullMethodInformationTest
  {
    private NullMethodInformation _nullMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _nullMethodInformation = new NullMethodInformation();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_nullMethodInformation.Name, Is.Null);
      Assert.That (_nullMethodInformation.DeclaringType, Is.Null);
      Assert.That (_nullMethodInformation.ReturnType, Is.Null);
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_nullMethodInformation.GetOriginalDeclaringType(), Is.Null);
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (_nullMethodInformation.GetCustomAttribute<SynchronizationAttribute> (false), Is.Null);
      Assert.That (_nullMethodInformation.GetCustomAttribute<SynchronizationAttribute> (true), Is.Null);
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (_nullMethodInformation.GetCustomAttributes<SynchronizationAttribute[]> (false), Is.Empty);
      Assert.That (_nullMethodInformation.GetCustomAttributes<SynchronizationAttribute[]> (true), Is.Empty);
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (_nullMethodInformation.IsDefined<SynchronizationAttribute> (false), Is.False);
      Assert.That (_nullMethodInformation.IsDefined<SynchronizationAttribute> (true), Is.False);
    }

    [Test]
    public void Invoke ()
    {
      Assert.That (_nullMethodInformation.Invoke (new object(), null), Is.Null);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void FindInterfaceImplementation ()
    {
      _nullMethodInformation.FindInterfaceImplementation (typeof (object));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_nullMethodInformation.FindInterfaceDeclarations(), Is.Null);
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      Assert.That (_nullMethodInformation.FindDeclaringProperty (), Is.Null);
    }

    [Test]
    public void GetFastInvoker_WithReferenceTypeReturnType ()
    {
      var invoker = _nullMethodInformation.GetFastInvoker<Func<object, object, object>> ();

      Assert.That (invoker (new object(), null), Is.Null);
    }

    [Test]
    public void GetFastInvoker_WithValueTypeReturnType ()
    {
      var invoker = _nullMethodInformation.GetFastInvoker<Func<object, object, int>> ();

      Assert.That (invoker (new object(), null), Is.EqualTo (0));
    }
    
    [Test]
    public void GetFastInvoker_WithoutReturnType ()
    {
      var invoker = _nullMethodInformation.GetFastInvoker<Action<object, object>> ();

     invoker (new object(), null);
    }

    [Test]
    public void GetParameters ()
    {
      Assert.That (_nullMethodInformation.GetParameters().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetOriginalDeclaration ()
    {
      Assert.That (_nullMethodInformation.GetOriginalDeclaration (), Is.SameAs(_nullMethodInformation));
    }

    [Test]
    public void TestEquals ()
    {
      var nullMethodInformation2 = new NullMethodInformation();

      Assert.That (_nullMethodInformation.Equals (nullMethodInformation2), Is.True);
      Assert.That (_nullMethodInformation.Equals (null), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      Assert.That (_nullMethodInformation.GetHashCode(), Is.EqualTo (0));
    }

    [Test]
    public void To_String ()
    {
      Assert.That (_nullMethodInformation.ToString(), Is.EqualTo ("NullMethodInformation"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_nullMethodInformation.IsNull, Is.True);
    }
  }
}