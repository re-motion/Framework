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
using Remotion.Tools.Console;

namespace Remotion.Tools.UnitTests.Console
{

[TestFixture]
public class MonospaceTextFormatTest
{
  [Test]
  public void TestSplitTextOnSeparator ()
  {
    AssertTextSplit ("12345 abcde",         10, "12345",        "abcde");
    AssertTextSplit ("1234567890 abcde",    10, "1234567890",   "abcde");
    AssertTextSplit ("12345678901 abcde",   10, "1234567890",   "1 abcde");
    AssertTextSplit ("1234 6789 bcde fghi", 10, "1234 6789",    "bcde fghi");
    AssertTextSplit ("1234567",             10, "1234567",      null);
    AssertTextSplit ("1234567890",          10, "1234567890",   null);
    AssertTextSplit ("12345678901",         10, "1234567890",   "1");
    AssertTextSplit ("",                     0, "",             null);
  }

  private void AssertTextSplit (string text, int splitAt, string expectedBefore, string expectedAfter)
  {
    string before;
    string after;
    MonospaceTextFormat.SplitTextOnSeparator (text, out before, out after, splitAt, new char[] {' '});
    Assert.That (before, Is.EqualTo (expectedBefore));
    Assert.That (after, Is.EqualTo (expectedAfter));
  }

  [Test]
  public void TestAppendIndentedText()
  {
    string label = "this is the label  ";
    string description = "the quick brown fox jumps over the lazy dog. THE (VERY QUICK) FOX JUMPS OVER THE LAZY DOG.";
    StringBuilder sb = new StringBuilder (label);
    MonospaceTextFormat.AppendIndentedText (sb, label.Length, 30, description);
    string expectedText = 
            "this is the label  the quick"
        + "\n                   brown fox"
        + "\n                   jumps over"
        + "\n                   the lazy"
        + "\n                   dog. THE"
        + "\n                   (VERY"
        + "\n                   QUICK) FOX"
        + "\n                   JUMPS OVER"
        + "\n                   THE LAZY"
        + "\n                   DOG.";
    Assert.That (sb.ToString(), Is.EqualTo (expectedText));
  }
}

}
