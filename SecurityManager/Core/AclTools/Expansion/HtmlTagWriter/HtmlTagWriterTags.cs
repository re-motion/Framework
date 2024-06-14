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

namespace Remotion.SecurityManager.AclTools.Expansion.HtmlTagWriter
{
  /// <summary>
  /// Helper class for convenient writing of opening/closing HTML-tags to a <see cref="StringWriter"/>.
  /// </summary>
  /// <remarks>
  /// <example>
  /// Example writing HTML to <see cref="HtmlTagWriter"/>
  /// <code><![CDATA[
  /// var textWriter = new StringWriter ();
  /// using (var htmlWriter = new HtmlWriter (textWriter, false))
  /// {
  ///   htmlWriter.Tags.html();
  ///   htmlWriter.Tags.head();
  ///   htmlWriter.Tags.headEnd();
  ///   htmlWriter.Tags.body();
  ///   htmlWriter.Value("some text");
  ///   htmlWriter.Tags.bodyEnd();
  ///   htmlWriter.Tags.htmlEnd();
  /// }
  /// ]]></code>
  /// </example>
  /// </remarks>

  public class HtmlTagWriterTags
  {
    private readonly HtmlTagWriter _htmlTagWriter;

    public HtmlTagWriterTags (HtmlTagWriter _htmlTagWriter)
    {
      this._htmlTagWriter = _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;br/&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter br ()
    {
      _htmlTagWriter.Tag("br");
      _htmlTagWriter.TagEnd("br");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;body&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter body ()
    {
      _htmlTagWriter.Tag("body");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/body&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter bodyEnd ()
    {
      _htmlTagWriter.TagEnd("body");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;table&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter table ()
    {
      _htmlTagWriter.Tag("table");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/table&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter tableEnd ()
    {
      _htmlTagWriter.TagEnd("table");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;tr&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter tr ()
    {
      _htmlTagWriter.Tag("tr");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/tr&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter trEnd ()
    {
      _htmlTagWriter.TagEnd("tr");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;td&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter td ()
    {
      _htmlTagWriter.Tag("td");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/td&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter tdEnd ()
    {
      _htmlTagWriter.TagEnd("td");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;th&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter th ()
    {
      _htmlTagWriter.Tag("th");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/th&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter thEnd ()
    {
      _htmlTagWriter.TagEnd("th");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;head&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter head ()
    {
      _htmlTagWriter.Tag("head");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/head&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter headEnd ()
    {
      _htmlTagWriter.TagEnd("head");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;title&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter title ()
    {
      _htmlTagWriter.Tag("title");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/title&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter titleEnd ()
    {
      _htmlTagWriter.TagEnd("title");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;html&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter html ()
    {
      _htmlTagWriter.Tag("html");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/html&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter htmlEnd ()
    {
      _htmlTagWriter.TagEnd("html");
      return _htmlTagWriter;
    }


    /// <summary>
    /// Writes out a &lt;style&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter style ()
    {
      _htmlTagWriter.Tag("style");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/style&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter styleEnd ()
    {
      _htmlTagWriter.TagEnd("style");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;p&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter p ()
    {
      _htmlTagWriter.Tag("p");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out a &lt;/p&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter pEnd ()
    {
      _htmlTagWriter.TagEnd("p");
      return _htmlTagWriter;
    }


    /// <summary>
    /// Writes out an &lt;a&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter a ()
    {
      _htmlTagWriter.Tag("a");
      return _htmlTagWriter;
    }

    /// <summary>
    /// Writes out an &lt;/a&gt; HTML-tag
    /// </summary>
    /// <returns></returns>
    public HtmlTagWriter aEnd ()
    {
      _htmlTagWriter.TagEnd("a");
      return _htmlTagWriter;
    }
  }
}
