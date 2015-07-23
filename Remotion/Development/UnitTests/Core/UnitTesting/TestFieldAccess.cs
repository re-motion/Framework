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

  public class TypeWithFields
  {
    public int IntField;
    private string _stringField = null;

    public static int StaticIntField;
    private static string s_stringField = null;

    private void DummyReferenceFieldsToSupressWarnings ()
    {
      _stringField = s_stringField;
    }
  }

  public class DerivedTypeWithFields : TypeWithFields
  {
  }

  [TestFixture]
  public class TestFieldAccess
  {
    [Test]
    public void TestInstanceFields ()
    {
      TypeWithFields twp = new TypeWithFields ();
      DerivedTypeWithFields dtwp = new DerivedTypeWithFields ();

      PrivateInvoke.SetPublicField (twp, "IntField", 21);
      Assert.That (PrivateInvoke.GetPublicField (twp, "IntField"), Is.EqualTo (21));

      PrivateInvoke.SetNonPublicField (twp, "_stringField", "test 3");
      Assert.That (PrivateInvoke.GetNonPublicField (twp, "_stringField"), Is.EqualTo ("test 3"));

      PrivateInvoke.SetNonPublicField (dtwp, "_stringField", "test 3");
      Assert.That (PrivateInvoke.GetNonPublicField (dtwp, "_stringField"), Is.EqualTo ("test 3"));
    }

    [Test]
    public void TestStaticFields ()
    {
      PrivateInvoke.SetPublicStaticField (typeof (TypeWithFields), "StaticIntField", 22);
      Assert.That (PrivateInvoke.GetPublicStaticField (typeof (TypeWithFields), "StaticIntField"), Is.EqualTo (22));

      PrivateInvoke.SetNonPublicStaticField (typeof (TypeWithFields), "s_stringField", "test 4");
      Assert.That (PrivateInvoke.GetNonPublicStaticField (typeof (TypeWithFields), "s_stringField"), Is.EqualTo ("test 4"));
    }
  }

}
