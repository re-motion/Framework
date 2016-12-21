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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class ArrayUtilityTest
{
  [Test]
  public void TestCombine()
  {
    string[] s1 = { "a", "b", "c" };
    string[] s2 = { "d" };
    string[] s3 = {};
    string[] s4 = { "e", "f" };

    string[] res = (string[]) ArrayUtility.Combine (s1, s2, s3, s4);
    Assert.That (string.Concat (res), Is.EqualTo ("abcdef"));
  }

  [Test]
  public void TestConvert ()
  {
    object[] o1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Convert<object, string> (o1);
    Assert.That (string.Concat (res), Is.EqualTo ("abcd"));
  }

  [Test]
  public void TestConvertWithNull ()
  {
    Assert.That (ArrayUtility.Convert<object, string> (null), Is.Null);
  }

  [Test]
  public void TestInsertFirst()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 0, "X");
    Assert.That (string.Concat (res), Is.EqualTo ("Xabcd"));
  }
  [Test]
  public void TestInsertMiddle()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 2, "X");
    Assert.That (string.Concat (res), Is.EqualTo ("abXcd"));
  }
  [Test]
  public void TestInsertEnd()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 4, "X");
    Assert.That (string.Concat (res), Is.EqualTo ("abcdX"));
  }
  [ExpectedException (typeof (IndexOutOfRangeException))]
  [Test]
  public void TestInsertPastEnd()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Insert (s1, 5, "X");
  }

  [Test]
  public void TestSkip ()
  {
    string[] s1 = { "a", "b", "c", "d" };
    string[] res = ArrayUtility.Skip (s1, 2);
    Assert.That (string.Concat (res), Is.EqualTo ("cd"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentOutOfRangeException))]
  public void TestSkipFail ()
  {
    ArrayUtility.Skip (new int[3], 4);
  }
}

}
