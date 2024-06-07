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
using System.Xml;

namespace Remotion.SecurityManager.AclTools.Expansion.HtmlTagWriter
{
  /// <summary>
  /// Supports convenient writing of HTML to a <see cref="TextWriter"/> / <see cref="XmlWriter"/>.
  /// A mismatched closing tag leads to an <see cref="XmlException"/>.
  /// </summary>
  /// <remarks>
  /// <example>
  /// Example writing HTML to <see cref="StringWriter"/>
  /// <code><![CDATA[
  /// var textWriter = new StringWriter ();
  /// using (var htmlWriter = new HtmlTagWriter (textWriter, false))
  /// {
  ///   htmlWriter.WritePageHeader("My Page Title","myPage.css");
  ///   htmlWriter.Tags.body();
  ///   htmlWriter.Value("some text");
  ///   htmlWriter.Tags.bodyEnd();
  ///   htmlWriter.Tags.htmlEnd();
  /// }
  /// string htmlText = stringWriter.ToString ();
  /// ]]></code>
  /// </example>
  /// </remarks>

  public class HtmlTagWriter : IDisposable
  {
    private readonly XmlWriter _xmlWriter;
    private readonly Stack<string> _openElementStack = new Stack<string>();

    private readonly HtmlTagWriterTags _htmlTagWriterTags;


    public static XmlWriter CreateXmlWriter (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      settings.ConformanceLevel = ConformanceLevel.Document;

      return XmlWriter.Create(textWriter, settings);
    }


    public HtmlTagWriter (TextWriter textWriter, bool indentXml)
        : this(CreateXmlWriter(textWriter, indentXml))
    {}

    public HtmlTagWriter (XmlWriter xmlWriter)
    {
      _xmlWriter = xmlWriter;
      _htmlTagWriterTags = new HtmlTagWriterTags(this);
    }

    public XmlWriter XmlWriter
    {
      get { return _xmlWriter; }
    }

    public HtmlTagWriterTags Tags
    {
      get { return _htmlTagWriterTags; }
    }

    public HtmlTagWriter Tag (string elementName)
    {
      _xmlWriter.WriteStartElement(elementName);
      _openElementStack.Push(elementName);
      return this;
    }

    public HtmlTagWriter TagEnd (string elementName)
    {
      string ElementNameExpected = _openElementStack.Pop();
      if (ElementNameExpected != elementName)
      {
        _xmlWriter.Flush();
        throw new XmlException(String.Format("Wrong closing tag in HTML: Expected {0} but was {1}.", ElementNameExpected, elementName));
      }
      _xmlWriter.WriteEndElement();
      return this;
    }

    public HtmlTagWriter Attribute (string attributeName, string attributeValue)
    {
      _xmlWriter.WriteAttributeString(attributeName,attributeValue);
      return this;
    }



    public HtmlTagWriter Value (string? s)
    {
      _xmlWriter.WriteValue(s ?? string.Empty);
      return this;
    }

    public HtmlTagWriter Value (object obj)
    {
      _xmlWriter.WriteValue(obj);
      return this;
    }



    public HtmlTagWriter WritePageHeader (string pageTitle, string cssFileName)
    {
      // DOCTYPE
      XmlWriter.WriteDocType("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      // HTML
      Tag("html");
      // HEAD
      Tag("head");

      // TITLE
      if (pageTitle != null)
      {
        Tag("title");
        Value(pageTitle);
        TagEnd("title");
      }

      // STYLE
      if (cssFileName != null)
      {
        Tag("style");
        Value("@import \"" + cssFileName + "\";");
        TagEnd("style");
      }

      // META
      //   <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/> 
      Tag("meta");
      Attribute("http-equiv", "Content-Type");
      Attribute("content", "text/html; charset=UTF-8");
      TagEnd("meta");

      TagEnd("head");

      return this;
    }



    //------------------------------------------------------------
    // Dispose
    //------------------------------------------------------------

    public void Close ()
    {
      _xmlWriter.Close();
    }

    void IDisposable.Dispose ()
    {
      Close();
    }

  }
}
