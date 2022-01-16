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
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Web.UI.Controls.Hotkey;

namespace Remotion.Web.UnitTests.Core.UI.Controls.Hotkey
{
  [TestFixture]
  public class HotkeyFormatterBaseTest
  {
    private class TestableHotkeyFormatterBase : HotkeyFormatterBase
    {
      protected override void AppendHotkeyBeginTag (HtmlTextWriter writer, char hotkey)
      {
        writer.AddAttribute("key", hotkey.ToString());
        writer.RenderBeginTag(HtmlTextWriterTag.Span);
      }

      protected override void AppendHotkeyEndTag (HtmlTextWriter writer)
      {
        writer.RenderEndTag();
      }
    }

    public class TestCaseValue
    {
      public string Input;
      public string Output;
      public char? Hotkey;
    }

    public static IEnumerable<TestCaseData> GetTestCaseValues (string testMethodName)
    {
      yield return Test(testName: "TextIsEmpty_ResultIsEmpty", input: "", output: "", hotkey: null);
      yield return Test(testName: "TextWithoutHotkey_ResultIsText", input: "No Hotkey", output: "No Hotkey", hotkey: null);
      yield return Test(testName: "TextWithHotkey_ResultHighlightsHotkey", input: "A &Hotkey", output: "A <span key=\"H\">H</span>otkey", hotkey: 'H');
      yield return Test(testName: "TextWithHotkeyAtStart_ResultIsTextWithHighlightedHotkey", input: "&A Hotkey", output: "<span key=\"A\">A</span> Hotkey", hotkey: 'A');
      yield return Test(testName: "TextWithHotkeyAtEnd_ResultIsTextWithHighlightedHotkey", input: "A Hotke&y", output: "A Hotke<span key=\"Y\">y</span>", hotkey: 'Y');
      yield return Test(testName: "TextWithHotkeyMarkerBeforeWhitespace_ResultIgnoresHotkeyMarker", input: "No & Hotkey", output: "No &amp; Hotkey", hotkey: null);
      yield return Test(testName: "TextWithEscapedHotkeyMarkerBeforeWhitespace_ResultIgnoresHotkeyMarker", input: "No && Hotkey", output: "No &amp; Hotkey", hotkey: null);
      yield return Test(testName: "TextWithEscapedHotkeyMarkerAtEnd_ResultIgnoresHotkeyMarker", input: "No Hotkey&&", output: "No Hotkey&amp;", hotkey: null);
      yield return Test(testName: "TextWithHotkeyMarkerBeforePunctuation_ResultIgnoresHotkeyMarker", input: "No &. Hotkey", output: "No &amp;. Hotkey", hotkey: null);
      yield return Test(testName: "TextWithEscapedHotkeyMarkerBeforePunctuation_ResultIgnoresHotkeyMarker", input: "No &&. Hotkey", output: "No &amp;. Hotkey", hotkey: null);
      yield return Test(testName: "TextWithHotkeyMarkerAsLastCharacter_ResultIgnoresHotkeyMarker", input: "No Hotkey&", output: "No Hotkey&amp;", hotkey: null);
      yield return Test(testName: "TextWithMultipleHotkeyMarkers_ResultIgnoresHotkeyMarkers", input: "&No &Hotkey", output: "&amp;No &amp;Hotkey", hotkey: null);
      yield return Test(testName: "TextWithEscapedHotkeyMarker_ResultIgnoresHotkeyMarker", input: "No &&Hotkey", output: "No &amp;Hotkey", hotkey: null);
      yield return Test(testName: "TextWithRepeatedEscapedHotkeyMarker_ResultIgnoresHotkeyMarker", input: "No &&&&Hotkey", output: "No &amp;&amp;Hotkey", hotkey: null);
      yield return Test(
          testName: "TextWithEscapedHotkeyMarker_AndFollowedByHotkey_ResultHighlightsHotkey",
          input: "A &&&Hotkey",
          output: "A &amp;<span key=\"H\">H</span>otkey",
          hotkey: 'H');
      yield return Test(
          testName: "TextWithHotkey_AndEscapedHotkeyMarkers_ResultHighlightsHotkey_IntegrationTest",
          input: "&&Hotkey & &Integration &&Test&",
          output: "&amp;Hotkey &amp; <span key=\"I\">I</span>ntegration &amp;Test&amp;",
          hotkey: 'I');
      yield return Test(
          testName: "TextWithMultipleHotkeyMarkers_AndEscapedHotkeyMarkers_ResultIgnoresHotkeyMarkers_IntegrationTest",
          input: "&Hotkey &&Integration &Test",
          output: "&amp;Hotkey &amp;&amp;Integration &amp;Test",
          hotkey: null);

      TestCaseData Test (string input, string output, char? hotkey, string testName)
      {
        return new TestCaseData(new TestCaseValue() { Input = input, Output = output, Hotkey = hotkey }) { TestName = testMethodName + "_" + testName };
      }
    }

    [Test]
    [TestCaseSource(nameof(GetTestCaseValues), new object[] { nameof(GetAccessKey_PlainTextWebString) })]
    public void GetAccessKey_PlainTextWebString (TestCaseValue testCaseValue)
    {
      var webString = WebString.CreateFromText(testCaseValue.Input);

      var formatter = new TestableHotkeyFormatterBase();

      var accessKey = formatter.GetAccessKey(webString);
      Assert.That(accessKey, Is.EqualTo(testCaseValue.Hotkey));
    }

    [Test]
    [TestCaseSource(nameof(GetTestCaseValues), new object[] { nameof(WriteTo_PlainTextWebString) })]
    public void WriteTo_PlainTextWebString (TestCaseValue testCaseValue)
    {
      var webString = WebString.CreateFromText(testCaseValue.Input);

      var formatter = new TestableHotkeyFormatterBase();

      var renderedString = ExecuteWithHtmlTextWriter(writer => formatter.WriteTo(writer, webString));
      Assert.That(renderedString, Is.EqualTo(testCaseValue.Output));
    }

    [Test]
    public void GetAccessKey_HtmlWebString_ReturnsNull ()
    {
      var stringValue = "&Test";
      var webString = WebString.CreateFromHtml(stringValue);

      var formatter = new TestableHotkeyFormatterBase();

      var accessKey = formatter.GetAccessKey(webString);
      Assert.That(accessKey, Is.Null);
    }

    [Test]
    public void WriteTo_HtmlWebString_RendersOutputWithoutHotkeyHandling ()
    {
      var stringValue = "&&Test&Test<i>Test</i>";
      var webString = WebString.CreateFromHtml(stringValue);

      var formatter = new TestableHotkeyFormatterBase();

      var renderedString = ExecuteWithHtmlTextWriter(writer => formatter.WriteTo(writer, webString));
      Assert.That(renderedString, Is.EqualTo(stringValue));
    }

    private string ExecuteWithHtmlTextWriter (Action<HtmlTextWriter> action)
    {
      using var stringWriter = new StringWriter();
      using var htmlTextWriter = new HtmlTextWriter(stringWriter);

      action(htmlTextWriter);
      htmlTextWriter.Flush();

      return stringWriter.ToString();
    }
  }
}
