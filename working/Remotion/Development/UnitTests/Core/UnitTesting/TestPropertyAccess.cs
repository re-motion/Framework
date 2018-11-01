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
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{

public class TypeWithProperties
{
  private int _intField;
  public int IntProperty
  {
    get { return _intField; }
    set { _intField = value; }
  }

  private string _stringField;
  protected string StringProperty
  {
    get { return _stringField; }
    set { _stringField = value; }
  }

  private static int s_intField;
  public static int StaticIntProperty
  {
    get { return s_intField; }
    set { s_intField = value; }
  }

  private static string s_stringField;
  protected static string StaticStringProperty
  {
    get { return s_stringField; }
    set { s_stringField = value; }
  }

  // ReSharper disable UnusedAutoPropertyAccessor.Local
  public static string PublicPropertyWithPrivateSetter
  {
    get;
    private set;
  }
  // ReSharper restore UnusedAutoPropertyAccessor.Local

  // ReSharper disable UnusedAutoPropertyAccessor.Local
  public static string PublicPropertyWithPrivateGetter
  {
    private get;
    set;
  }
  // ReSharper restore UnusedAutoPropertyAccessor.Local

}

[TestFixture]
public class TestPropertyAccess
{
  [Test]
	public void TestInstanceProperties()
	{
    TypeWithProperties twp = new TypeWithProperties();

    PrivateInvoke.SetPublicProperty (twp, "IntProperty", 12);
    Assert.That (PrivateInvoke.GetPublicProperty (twp, "IntProperty"), Is.EqualTo (12));

    PrivateInvoke.SetNonPublicProperty (twp, "StringProperty", "test 1");
    Assert.That (PrivateInvoke.GetNonPublicProperty (twp, "StringProperty"), Is.EqualTo ("test 1"));
	}

  [Test]
	public void TestStaticProperties()
	{
    PrivateInvoke.SetPublicStaticProperty (typeof (TypeWithProperties), "StaticIntProperty", 13);
    Assert.That (PrivateInvoke.GetPublicStaticProperty (typeof (TypeWithProperties), "StaticIntProperty"), Is.EqualTo (13));

    PrivateInvoke.SetNonPublicStaticProperty (typeof (TypeWithProperties), "StaticStringProperty", "test 2");
    Assert.That (PrivateInvoke.GetNonPublicStaticProperty (typeof (TypeWithProperties), "StaticStringProperty"), Is.EqualTo ("test 2"));
	}

  [Test]
  public void TestPublicPropertyWithPrivateSetter ()
  {
    PrivateInvoke.SetPublicStaticProperty (typeof (TypeWithProperties), "PublicPropertyWithPrivateSetter", "Test");
    Assert.That (TypeWithProperties.PublicPropertyWithPrivateSetter, Is.EqualTo ("Test"));
  }

  [Test]
  public void TestPublicPropertyWithPrivateGetter ()
  {
    TypeWithProperties.PublicPropertyWithPrivateGetter = "Test2";
    Assert.That (PrivateInvoke.GetPublicStaticProperty (typeof (TypeWithProperties), "PublicPropertyWithPrivateGetter"), Is.EqualTo ("Test2"));
  }
}

}
