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
using System.Linq;
using System.Text;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class RenderUtilityTest
  {
    [Test]
    public void JoinLinesWithEncoding_WithEmptySequence_ReturnsEmptyString ()
    {
      Assert.That(
          RenderUtility.JoinLinesWithEncoding(Enumerable.Empty<string>()),
          Is.EqualTo(""));
    }

    [Test]
    public void JoinLinesWithEncoding_WithSingleItem_ReturnsString ()
    {
      Assert.That(
          RenderUtility.JoinLinesWithEncoding(new[] { "First" }),
          Is.EqualTo("First"));
    }

    [Test]
    public void JoinLinesWithEncoding_WithMultipleItems_ReturnsConcatenatedString ()
    {
      Assert.That(
          RenderUtility.JoinLinesWithEncoding(new[] { "First", "Second" }),
          Is.EqualTo("First<br />Second"));
    }

    [Test]
    public void JoinLinesWithEncoding_WithSingleItemAndRequiringEncoding_ReturnsEncodedString ()
    {
      Assert.That(
          RenderUtility.JoinLinesWithEncoding(new[] { "Fir<html>st" }),
          Is.EqualTo("Fir&lt;html&gt;st"));
    }

    [Test]
    public void JoinLinesWithEncoding_WithMultipleItemsAndRequiringEncoding_ReturnsConcatenatedAndEncodedString ()
    {
      Assert.That(
          RenderUtility.JoinLinesWithEncoding(new[] { "Fir<html>st", "Second" }),
          Is.EqualTo("Fir&lt;html&gt;st<br />Second"));
    }

    [Test]
    public void WriteEncodedLines_WithEmptySequence_DoesNotAddToRenderingOutput ()
    {
      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter(stringWriter);

      htmlTextWriter.WriteEncodedLines(Enumerable.Empty<string>());

      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void WriteEncodedLines_WithSingleItem_RendersItem ()
    {
      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter(stringWriter);

      htmlTextWriter.WriteEncodedLines(new[] { "First" });

      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo("First"));
    }

    [Test]
    public void WriteEncodedLines_WithMultipleItems_RendersConcatenatedString ()
    {
      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter(stringWriter);

      htmlTextWriter.WriteEncodedLines(new[] { "First", "Second", "Third" });

      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo("First<br />Second<br />Third"));
    }

    [Test]
    public void WriteEncodedLines_WithMultipleItemsAndEncoding_RendersEncodedText ()
    {
      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter(stringWriter);

      htmlTextWriter.WriteEncodedLines(new[] { "Fir<html>st", "Sec<html>ond", "Thi<html>rd" });

      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo("Fir&lt;html&gt;st<br />Sec&lt;html&gt;ond<br />Thi&lt;html&gt;rd"));
    }

    [Test]
    public void ToJson_ReturnsEmptyObjectForEmptyDictionary ()
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(
          new Dictionary<string, string>(),
          new Dictionary<string, IReadOnlyCollection<string>>());

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{}"));
    }

    [Test]
    public void ToJson_OneValue ()
    {
      var dictionary = new Dictionary<string, string>();
      dictionary.Add("data", "value");
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":\"value\"}"));
    }

    [Test]
    public void ToJson_OneValueEmpty ()
    {
      var dictionary = new Dictionary<string, string>();
      dictionary.Add("data", "");
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":\"\"}"));
    }

    [Test]
    public void ToJson_OneValueNull ()
    {
      var dictionary = new Dictionary<string, string>();
      dictionary.Add("data", null);
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":null}"));
    }

    [Test]
    public void ToJson_MultipleValues ()
    {
      var dictionary = new Dictionary<string, string>();
      dictionary.Add("data1", "value1");
      dictionary.Add("data2", "value2");
      dictionary.Add("data3", null);
      dictionary.Add("data4", "value4");
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(dictionary);

      Assert.That(
          stringBuilder.ToString(),
          Is.EqualTo("{\"data1\":\"value1\",\"data2\":\"value2\",\"data3\":null,\"data4\":\"value4\"}"));
    }

    [Test]
    public void ToJson_EncapsulatesValuesContainingDoubleQuotesIntoSingleQuotes ()
    {
      var dictionary = new Dictionary<string, string>();
      dictionary.Add("data1", "What\"ever");
      dictionary.Add("data2", "Some'thing");
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data1\":'What\"ever',\"data2\":\"Some'thing\"}"));
    }

    [Test]
    public void ToJson_OneArray ()
    {
      var dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      dictionary.Add("data", new[] { "value" });
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(new Dictionary<string, string>(), dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":[\"value\"]}"));
    }

    [Test]
    public void ToJson_OneArrayEmpty ()
    {
      var dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      dictionary.Add("data", new string[0]);
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(new Dictionary<string, string>(), dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":[]}"));
    }

    [Test]
    public void ToJson_OneArrayNull ()
    {
      var dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      dictionary.Add("data", null);
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(new Dictionary<string, string>(), dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":null}"));
    }

    [Test]
    public void ToJson_MultipleArrays ()
    {
      var dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      dictionary.Add("data1", new[] { "value1" });
      dictionary.Add("data2", new string[0]);
      dictionary.Add("data3", null);
      dictionary.Add("data4", new[] { "", null, "value2" });
      dictionary.Add("data5", new[] { "value3", "value4" });
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(new Dictionary<string, string>(), dictionary);

      Assert.That(
          stringBuilder.ToString(),
          Is.EqualTo("{\"data1\":[\"value1\"],\"data2\":[],\"data3\":null,\"data4\":[\"\",null,\"value2\"],\"data5\":[\"value3\",\"value4\"]}"));
    }

    [Test]
    public void ToJson_EncapsulatesArrayValuesContainingDoubleQuotesIntoSingleQuotes ()
    {
      var dictionary = new Dictionary<string, IReadOnlyCollection<string>>();
      dictionary.Add("data", new[] { "What\"ever",  "Some'thing" });
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(new Dictionary<string, string>(), dictionary);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data\":['What\"ever',\"Some'thing\"]}"));
    }

    [Test]
    public void ToJson_CombinationOfStringValueAndStringArray ()
    {
      var dictionary1 = new Dictionary<string, string>();
      dictionary1.Add("data1", "value1");

      var dictionary2 = new Dictionary<string, IReadOnlyCollection<string>>();
      dictionary2.Add("data2", new[] { "value2", "value3" });
      var stringBuilder = new StringBuilder();

      stringBuilder.WriteDictionaryAsJson(dictionary1, dictionary2);

      Assert.That(stringBuilder.ToString(), Is.EqualTo("{\"data1\":\"value1\",\"data2\":[\"value2\",\"value3\"]}"));
    }
  }
}