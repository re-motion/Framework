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
using System.Text;
using NUnit.Framework;
using Remotion.Text.StringExtensions;

namespace Remotion.Extensions.UnitTests.Text.StringExtensions
{
  [TestFixture]
  public class StringExtensionsTest
  {
    private const string _testString = "b A kkk CDC";

    [Test]
    public void LeftUntilCharTest ()
    {
      Assert.That ("".LeftUntilChar ('c'), Is.EqualTo (""));
      Assert.That ("S".LeftUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("Sabc".LeftUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("aSbc".LeftUntilChar ('S'), Is.EqualTo ("a"));
      Assert.That ("abSc".LeftUntilChar ('S'), Is.EqualTo ("ab"));
      Assert.That ("abcS".LeftUntilChar ('S'), Is.EqualTo ("abc"));
      Assert.That ("xyzzz".LeftUntilChar ('z'), Is.EqualTo ("xy"));
      Assert.That ("abcdefg".LeftUntilChar ('z'), Is.EqualTo ("abcdefg"));
    }

    [Test]
    public void LeftUntilCharUpperLowerCaseTest ()
    {
      Assert.That ("SaBcd".LeftUntilChar ('s'), Is.EqualTo ("SaBcd"));
      Assert.That ("sAbCD".LeftUntilChar ('S'), Is.EqualTo ("sAbCD"));
    }


    [Test]
    public void RightUntilCharTest ()
    {
      Assert.That ("".RightUntilChar ('c'), Is.EqualTo (""));
      Assert.That ("S".RightUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("abcS".RightUntilChar ('S'), Is.EqualTo (""));
      Assert.That ("aSbc".RightUntilChar ('S'), Is.EqualTo ("bc"));
      Assert.That ("abSc".RightUntilChar ('S'), Is.EqualTo ("c"));
      Assert.That ("Sabc".RightUntilChar ('S'), Is.EqualTo ("abc"));
      Assert.That ("zzzxy".RightUntilChar ('z'), Is.EqualTo ("xy"));
      Assert.That ("abcdefg".RightUntilChar ('z'), Is.EqualTo ("abcdefg"));
    }

    [Test]
    public void RightUntilCharUpperLowerCaseTest ()
    {
      Assert.That ("SaBcd".RightUntilChar ('s'), Is.EqualTo ("SaBcd"));
      Assert.That ("sAbCD".RightUntilChar ('S'), Is.EqualTo ("sAbCD"));
    }

    [Test]
    public void EscapeStringTest ()
    {
      var testString = "abcdEFG\t\n\"\\ HIJklmn \t\t\n\n\"\"\\\\ \r \b\v\f";
      var result = testString.Escape();
      Assert.That (result, Is.EqualTo ("abcdEFG\\t\\n\\\"\\\\ HIJklmn \\t\\t\\n\\n\\\"\\\"\\\\\\\\ \\r \\b\\v\\f"));
    }

    [Test]
    public void EscapeStringTest2 ()
    {
      var testString = "\t\n\"\\ HIJklmn \t\t\n\n\"\"\\\\ \r \b\v\f";
      var stringBuilder = new StringBuilder ();
      testString.EscapeString (stringBuilder);
      var result = stringBuilder.ToString();
      Assert.That (result, Is.EqualTo ("\\t\\n\\\"\\\\ HIJklmn \\t\\t\\n\\n\\\"\\\"\\\\\\\\ \\r \\b\\v\\f"));
    }

  }
}
