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

public class PublicClass
{
  private class InternalClass: PublicClass
  {
    private string _s;

    public InternalClass (string s)
    {
      _s = s;
    }

    private InternalClass ()
    {
      _s = "private ctor";
    }

    protected override string f()
    {
      return _s;
    }

    public InternalClass (StringBuilder s)
    {
      _s = s.ToString();
    }

  }

  protected virtual string f ()
  {
    return "PublicClass";
  }
}

[TestFixture]
public class TestCreateInstance
{
  const string c_assemblyName = "Remotion.Development.UnitTests";
  const string c_publicClassName = "Remotion.Development.UnitTests.Core.UnitTesting.PublicClass";
  const string c_internalClassName = "Remotion.Development.UnitTests.Core.UnitTesting.PublicClass+InternalClass";

  [Test]
  public void TestCreateInstances()
  {
    PublicClass internalInstance;

    internalInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_internalClassName, 
        "test 1");
    Assert.That (PrivateInvoke.InvokeNonPublicMethod (internalInstance, "f"), Is.EqualTo ("test 1"));

    internalInstance = (PublicClass) PrivateInvoke.CreateInstanceNonPublicCtor (
        c_assemblyName, c_internalClassName);
    Assert.That (PrivateInvoke.InvokeNonPublicMethod (internalInstance, "f"), Is.EqualTo ("private ctor"));

    PublicClass publicInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_publicClassName);
    Assert.That (PrivateInvoke.InvokeNonPublicMethod (publicInstance, "f"), Is.EqualTo ("PublicClass"));
  }

  [Test]
  [ExpectedException (typeof (AmbiguousMatchException))]
  public void TestCreateInstanceAmbiguous()
  {
    PublicClass internalInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_internalClassName, 
        null);
  }
}

}
