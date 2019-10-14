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
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class ControlCharactersCharactersValidatorTest
  {
    private class TestableControlCharactersCharactersValidator : ControlCharactersCharactersValidator
    {
      public new bool EvaluateIsTextValid (string text) => base.EvaluateIsTextValid (text);
    }

    [Test]
    public void IsTextValid_WithNull_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (null), Is.True);
    }

    [Test]
    public void IsTextValid_WithEmpty_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (""), Is.True);
    }

    [Test]
    public void IsTextValid_WithWhitespace_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (" "), Is.True);
    }

    [Test]
    public void IsTextValid_WithNonBreakingWhitespace_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (new string ((char) 160, 1)), Is.True);
    }

    [Test]
    public void IsTextValid_WithLineBreak_AndEnableMultilineTextIsTrue_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;

      Assert.That (validator.EvaluateIsTextValid ("\n"), Is.True);
    }

    [Test]
    public void IsTextValid_WithStandaloneLineFeed_AndEnableMultilineTextIsTrue_ReturnsFalse ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("\r"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\r', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithLineBreak_AndEnableMultilineTextIsFalse_ReturnsFalse ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = false;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("\n"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\n', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithLineFeed_AndEnableMultilineTextIsFalse_ReturnsFalse ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = false;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("\r"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\r', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithStandaloneLineFeedInsideText_AndEnableMultilineTextIsTrue_ReturnsFalse ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("A\rB"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'A\rB', line position: 2; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithStandaloneLineFeedAtEndOfText_AndEnableMultilineTextIsTrue_ReturnsFalse ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("A\r"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'A\r', line position: 2; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithMultipleLinesUsingLineFeedAndLineBreak_AndEnableMultilineTextIsTrue_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;

      Assert.That (validator.EvaluateIsTextValid ("A\r\nB\r\n\r\nC"), Is.True);
    }

    [Test]
    public void IsTextValid_WithMultipleLinesUsingOnlyLineBreak_AndEnableMultilineTextIsTrue_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\n\nC"), Is.True);
    }

    [Test]
    public void IsTextValid_WithTextAndWhitespace_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid ("a b"), Is.True);
    }

    [Test]
    public void IsTextValid_WithTextAndNonBreakingWhitespace_ReturnsTrue ()
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid ("a" + new string ((char) 160, 1) + "b"), Is.True);
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_ReturnsFalseAndSetsLinePositionAndLineNumber ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nDDDDX\0XDDDD\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'DDDDX\0XDDDD', line position: 6; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 4;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '3210\00123', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLengthZero_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 0;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\0', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLongerThanLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'D543210\0012345D', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLongerThanLineWithErrorAtStartOfLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\n\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\0012345D', line position: 1; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLongerThanLineWithErrorAtEndOfLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'D543210\0', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorAloneInLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("\n\n\0\n\n"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\0', line position: 1; line number: 3"));
    }

    [Test]
    [TestCase ("a\u0001b", TestName = "IsTextValid_WithASCII01_ReturnsFalse")]
    [TestCase ("a\u0002b", TestName = "IsTextValid_WithASCII02_ReturnsFalse")]
    [TestCase ("a\u0003b", TestName = "IsTextValid_WithASCII03_ReturnsFalse")]
    [TestCase ("a\u0004b", TestName = "IsTextValid_WithASCII04_ReturnsFalse")]
    [TestCase ("a\u0005b", TestName = "IsTextValid_WithASCII05_ReturnsFalse")]
    [TestCase ("a\u0006b", TestName = "IsTextValid_WithASCII06_ReturnsFalse")]
    [TestCase ("a\u0007b", TestName = "IsTextValid_WithASCII07_ReturnsFalse")]
    [TestCase ("a\u0008b", TestName = "IsTextValid_WithASCII08_ReturnsFalse")]
    [TestCase ("a\u0009b", TestName = "IsTextValid_WithASCII09_ReturnsFalse")]
    [TestCase ("a\u000Ab", TestName = "IsTextValid_WithASCII0A_ReturnsFalse")]
    [TestCase ("a\u000Bb", TestName = "IsTextValid_WithASCII0B_ReturnsFalse")]
    [TestCase ("a\u000Cb", TestName = "IsTextValid_WithASCII0C_ReturnsFalse")]
    [TestCase ("a\u000Db", TestName = "IsTextValid_WithASCII0D_ReturnsFalse")]
    [TestCase ("a\u000Eb", TestName = "IsTextValid_WithASCII0E_ReturnsFalse")]
    [TestCase ("a\u000Fb", TestName = "IsTextValid_WithASCII0F_ReturnsFalse")]
    [TestCase ("a\u0010b", TestName = "IsTextValid_WithASCII10_ReturnsFalse")]
    [TestCase ("a\u0011b", TestName = "IsTextValid_WithASCII11_ReturnsFalse")]
    [TestCase ("a\u0012b", TestName = "IsTextValid_WithASCII12_ReturnsFalse")]
    [TestCase ("a\u0013b", TestName = "IsTextValid_WithASCII13_ReturnsFalse")]
    [TestCase ("a\u0014b", TestName = "IsTextValid_WithASCII14_ReturnsFalse")]
    [TestCase ("a\u0015b", TestName = "IsTextValid_WithASCII15_ReturnsFalse")]
    [TestCase ("a\u0016b", TestName = "IsTextValid_WithASCII16_ReturnsFalse")]
    [TestCase ("a\u0017b", TestName = "IsTextValid_WithASCII17_ReturnsFalse")]
    [TestCase ("a\u0018b", TestName = "IsTextValid_WithASCII18_ReturnsFalse")]
    [TestCase ("a\u0019b", TestName = "IsTextValid_WithASCII19_ReturnsFalse")]
    [TestCase ("a\u001Ab", TestName = "IsTextValid_WithASCII1A_ReturnsFalse")]
    [TestCase ("a\u001Bb", TestName = "IsTextValid_WithASCII1B_ReturnsFalse")]
    [TestCase ("a\u001Cb", TestName = "IsTextValid_WithASCII1C_ReturnsFalse")]
    [TestCase ("a\u001Db", TestName = "IsTextValid_WithASCII1D_ReturnsFalse")]
    [TestCase ("a\u001Eb", TestName = "IsTextValid_WithASCII1E_ReturnsFalse")]
    [TestCase ("a\u001Fb", TestName = "IsTextValid_WithASCII1F_ReturnsFalse")]
    [TestCase ("a\vb", TestName = "IsTextValid_WithVerticalTab_ReturnsFalse")]
    [TestCase ("a\tb", TestName = "IsTextValid_WithTab_ReturnsFalse")]
    [TestCase ("a\rb", TestName = "IsTextValid_WithCarriageReturn_ReturnsFalse")]
    [TestCase ("a\nb", TestName = "IsTextValid_WithLineFeed_ReturnsFalse")]
    public void IsTextValid_WithTextAndControlCharacter_ReturnsFalse (string text)
    {
      var validator = new TestableControlCharactersCharactersValidator();
      validator.EnableMultilineText = false;

      Assert.That (validator.EvaluateIsTextValid (text), Is.False);
    }

    [Test]
    [TestCase ("a\u00ADb", TestName = "IsTextValid_WithSoftHyphen_ReturnsTrue")]
    [TestCase ("a\u200Bb", TestName = "IsTextValid_WithZeroWidthSpace_ReturnsTrue")]
    public void IsTextValid_WithTextAndPrintableCharacter_ReturnsTrue (string text)
    {
      var validator = new TestableControlCharactersCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (text), Is.True);
    }
  }
}