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
using System.IO;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.AclTools.Expansion.HtmlTagWriter;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.HtmlTagWriter
{
  [TestFixture]
  public class HtmlTagWriterTest
  {
    [Test]
    public void WriteTagTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false))
      {
        htmlWriter.Tag("div").Value("xxx").TagEnd("div");
      }
      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo("<div>xxx</div>"));
    }


    [Test]
    public void WriteAttributeTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false))
      {
        htmlWriter.Tag("div").Attribute("id","myd").TagEnd("div");
      }
      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo(@"<div id=""myd"" />"));
    }

    [Test]
    public void CloseTest ()
    {
      var stringWriter = new StringWriter();
      var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false);
      htmlWriter.Tag("div").TagEnd("div");
      var resultBeforeClose = stringWriter.ToString();
      Assert.That(resultBeforeClose, Is.Empty);
      htmlWriter.Close();
      var resultAfterClose = stringWriter.ToString();
      Assert.That(resultAfterClose, Is.EqualTo("<div />"));
    }

    [Test]
    public void CreateXmlWriterTest ()
    {
      using (XmlWriter xmlWriter = SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter.CreateXmlWriter(TextWriter.Null, true))
      {
        Assert.That(xmlWriter.Settings.OmitXmlDeclaration, Is.True);
        Assert.That(xmlWriter.Settings.Indent, Is.True);
        Assert.That(xmlWriter.Settings.NewLineOnAttributes, Is.False);
        Assert.That(xmlWriter.Settings.ConformanceLevel, Is.EqualTo(ConformanceLevel.Document));
      }
    }


    [Test]
    public void WritePageHeaderTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false))
      {
        htmlWriter.WritePageHeader("Page Header Test", "pageHeaderTest.css");
      }

      var result = stringWriter.ToString();
      Assert.That(
          result,
          Is.EqualTo(
              "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html>"
              + "<head><title>Page Header Test</title><style>@import \"pageHeaderTest.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head>"
              + "</html>"));
    }


    [Test]
    public void BreakTagTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false))
      {
        htmlWriter.Tags.br();
      }
      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo("<br />"));
    }


    [Test]
    public void SpecificTagsTest ()
    {
      string[] tagNames = new[] { "html", "head", "title", "style", "body", "table", "th", "tr", "td", "p", "a"  };
      foreach (string tagName in tagNames)
      {
        AssertTagNameOpenCloseHtml(tagName);
      }
    }



    [Test]
    public void NonMatchingEndTagTest ()
    {
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(TextWriter.Null, false))
      {
        htmlWriter.Tag("abc");
        Assert.That(
            () => htmlWriter.TagEnd("xyz"),
            Throws.InstanceOf<XmlException>()
                .With.Message.EqualTo("Wrong closing tag in HTML: Expected abc but was xyz."));
      }
    }

    [Test]
    public void ComplexNonMatchingEndTagTest ()
    {
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(TextWriter.Null, false))
      {
        htmlWriter.Tag("abc");
        WriteHtmlPage(htmlWriter);
        Assert.That(
            () => htmlWriter.TagEnd("xyz"),
            Throws.InstanceOf<XmlException>()
                .With.Message.EqualTo("Wrong closing tag in HTML: Expected abc but was xyz."));
      }
    }


    [Test]
    public void HtmlPageIntegrationTest ()
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false))
      {
        WriteHtmlPage(htmlWriter);
      }

      var result = stringWriter.ToString();
      Assert.That(
          result,
          Is.EqualTo(
              "<html><head><title>Title: My HTML Page</title></head><body>"
              + "<p id=\"first_paragraph\">Smells like...<br />Victory<table class=\"myTable\"><tr><th>1st column</th></tr><tr><td>some data</td></tr><tr><td>some more data</td></tr></table></p>"
              + "</body></html>"));
    }

    private static void WriteHtmlPage (SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter htmlWriter)
    {
      htmlWriter.Tags.html();
      htmlWriter.Tags.head();
      htmlWriter.Tags.title();
      htmlWriter.Value("Title: My HTML Page");
      htmlWriter.Tags.titleEnd();
      htmlWriter.Tags.headEnd();
      htmlWriter.Tags.body();
      htmlWriter.Tags.p();
      htmlWriter.Attribute("id", "first_paragraph");
      htmlWriter.Value("Smells like...");
      htmlWriter.Tags.br();
      htmlWriter.Value("Victory");
      htmlWriter.Tags.table().Attribute("class", "myTable");
      htmlWriter.Tags.tr().Tags.th().Value("1st column").Tags.thEnd().Tags.trEnd();
      htmlWriter.Tags.tr().Tags.td().Value("some data").Tags.tdEnd().Tags.trEnd();
      htmlWriter.Tags.tr().Tags.td().Value("some more data").Tags.tdEnd().Tags.trEnd();
      htmlWriter.Tags.tableEnd();
      htmlWriter.Tags.pEnd();
      htmlWriter.Tags.bodyEnd();
      htmlWriter.Tags.htmlEnd();
    }


    private static void AssertTagNameOpenCloseHtml (string tagName)
    {
      var tagNameHtmlResult = GetSpecificTagOpenCloseHtml(tagName);
      Assert.That(tagNameHtmlResult, Is.EqualTo("<" + tagName + ">abc</" + tagName + ">"));
    }

    private static string GetSpecificTagOpenCloseHtml (string tagName)
    {
      var stringWriter = new StringWriter();
      using (var htmlWriter = new SecurityManager.AclTools.Expansion.HtmlTagWriter.HtmlTagWriter(stringWriter, false))
      {
        HtmlTagWriterTags htmlTagWriterTags = htmlWriter.Tags;
        PrivateInvoke.InvokePublicMethod(htmlTagWriterTags, tagName);
        htmlWriter.Value("abc");
        PrivateInvoke.InvokePublicMethod(htmlTagWriterTags, tagName + "End");
      }
      return stringWriter.ToString();
    }

  }
}
