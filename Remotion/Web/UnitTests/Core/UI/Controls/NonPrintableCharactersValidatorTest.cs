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
  public class NonPrintableCharactersValidatorTest
  {
    private class TestableNonPrintableCharactersValidator : NonPrintableCharactersValidator
    {
      public new bool EvaluateIsTextValid (string text) => base.EvaluateIsTextValid (text);
    }

    [Test]
    public void IsTextValid_WithNull_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (null), Is.True);
    }

    [Test]
    public void IsTextValid_WithEmpty_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (""), Is.True);
    }

    [Test]
    public void IsTextValid_WithWhitespace_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid (" "), Is.True);
    }

    [Test]
    public void IsTextValid_WithNonBreakingWhitespace_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid (new string ((char) 160, 1)), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '" + new string ((char) 160, 1) + "', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithLineBreak_AndEnableMultilineTextIsTrue_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;

      Assert.That (validator.EvaluateIsTextValid ("\n"), Is.True);
    }

    [Test]
    public void IsTextValid_WithStandaloneLineFeed_AndEnableMultilineTextIsTrue_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("\r"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\r', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithLineBreak_AndEnableMultilineTextIsFalse_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = false;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("\n"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\n', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithLineFeed_AndEnableMultilineTextIsFalse_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = false;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("\r"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\r', line position: 1; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithStandaloneLineFeedInsideText_AndEnableMultilineTextIsTrue_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("A\rB"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'A\rB', line position: 2; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithStandaloneLineFeedAtEndOfText_AndEnableMultilineTextIsTrue_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("A\r"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'A\r', line position: 2; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithMultipleLinesUsingLineFeedAndLineBreak_AndEnableMultilineTextIsTrue_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;

      Assert.That (validator.EvaluateIsTextValid ("A\r\nB\r\n\r\nC"), Is.True);
    }

    [Test]
    public void IsTextValid_WithMultipleLinesUsingOnlyLineBreak_AndEnableMultilineTextIsTrue_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\n\nC"), Is.True);
    }

    [Test]
    public void IsTextValid_WithTextAndWhitespace_ReturnsTrue ()
    {
      var validator = new TestableNonPrintableCharactersValidator();

      Assert.That (validator.EvaluateIsTextValid ("a b"), Is.True);
    }

    [Test]
    public void IsTextValid_WithTextAndNonBreakingWhitespace_ReturnsFalse ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("a" + new string ((char) 160, 1) + "b"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'a" + new string ((char) 160, 1) + "b', line position: 2; line number: 1"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_ReturnsFalseAndSetsLinePositionAndLineNumber ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nDDDDX\0XDDDD\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'DDDDX\0XDDDD', line position: 6; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 4;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '3210\00123', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLengthZero_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 0;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\0', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLongerThanLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'D543210\0012345D', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLongerThanLineWithErrorAtStartOfLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\n\0012345D\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\0012345D', line position: 1; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorInFourthLine_AndSampleTextLongerThanLineWithErrorAtEndOfLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("A\nB\nC\nD543210\0\nE\nF"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: 'D543210\0', line position: 8; line number: 4"));
    }

    [Test]
    public void IsTextValid_WithErrorAloneInLine_ReturnsFalseAndSetsCorrectSampleText ()
    {
      var validator = new TestableNonPrintableCharactersValidator();
      validator.EnableMultilineText = true;
      validator.ErrorMessageFormat = "text: '{0}', line position: {1}; line number: {2}";
      validator.SampleTextLength = 10;

      Assert.That (validator.EvaluateIsTextValid ("\n\n\0\n\n"), Is.False);
      Assert.That (validator.ErrorMessage, Is.EqualTo ("text: '\0', line position: 1; line number: 3"));
    }

    [Test]
    [Explicit]
    public void name ()
    {
      Assert.That (char.IsWhiteSpace (' '), Is.True);
      Assert.That (char.IsWhiteSpace ((char)160), Is.True);
      Assert.That (char.IsWhiteSpace ('\r'), Is.True);
      Assert.That (char.IsWhiteSpace ('\n'), Is.True);
      Assert.That (char.IsWhiteSpace ('\t'), Is.True);

      Assert.That (char.IsSeparator (' '), Is.True);
      Assert.That (char.IsSeparator ((char)160), Is.True);
      Assert.That (char.IsSeparator ('\r'), Is.False);
      Assert.That (char.IsSeparator ('\n'), Is.False);
      Assert.That (char.IsSeparator ('\t'), Is.False);
    }
  }
}