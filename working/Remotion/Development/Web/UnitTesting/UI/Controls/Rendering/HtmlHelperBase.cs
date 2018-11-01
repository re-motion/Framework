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
using System.Text;
using System.Web.UI;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.UI.Controls.Rendering
{
  /// <summary>
  /// Sets up an <see cref="HtmlTextWriter"/> (including underlying streams) to render to and
  /// provides functionality to easily assert conditions in the rendered HTML document.
  /// </summary>
  /// <remarks>
  /// Deriving classes must provide delegates to perform the actual assertions. 
  /// This is required so that this class does not depend on any automatic testing framework.
  /// </remarks>
  public abstract class HtmlHelperBase : IDisposable
  {
    public delegate void BinaryAssertDelegate (IComparable actual, IComparable expected, string message, params object[] args);

    public delegate void UnaryAssertDelegate (object actual, string message, params object[] args);

    public delegate void ConditionAssertDelegate (bool condition, string message, params object[] args);

    public const string WhiteSpace = "&nbsp;";

    public enum AttributeValueCompareMode
    {
      Equal,
      Contains
    }

    private readonly HtmlTextWriter _writer;
    private readonly MemoryStream _stream;
    private readonly StreamReader _reader;

    private readonly BinaryAssertDelegate _assertAreEqual;
    private readonly BinaryAssertDelegate _assertGreaterThan;
    private readonly UnaryAssertDelegate _assertNotNull;
    private readonly UnaryAssertDelegate _assertIsNull;
    private readonly ConditionAssertDelegate _assertTrue;

    /// <summary>
    /// Initializes the <see cref="HtmlTextWriter"/> and underlying streams.
    /// The delegate parameters are needed for assertions and cannot be <see langword="null"/>.
    /// </summary>
    protected HtmlHelperBase (
        BinaryAssertDelegate areEqual,
        BinaryAssertDelegate greaterThan,
        UnaryAssertDelegate notNull,
        UnaryAssertDelegate isNull,
        ConditionAssertDelegate isTrue)
    {
      ArgumentUtility.CheckNotNull ("areEqual", areEqual);
      ArgumentUtility.CheckNotNull ("greaterThan", greaterThan);
      ArgumentUtility.CheckNotNull ("notNull", notNull);
      ArgumentUtility.CheckNotNull ("isNull", isNull);
      ArgumentUtility.CheckNotNull ("isTrue", isTrue);

      _stream = new MemoryStream (4096);
      _writer = new HtmlTextWriter (new StreamWriter (Stream, Encoding.Unicode));
      _reader = new StreamReader (Stream, Encoding.Unicode);

      _assertAreEqual = areEqual;
      _assertGreaterThan = greaterThan;
      _assertNotNull = notNull;
      _assertIsNull = isNull;
      _assertTrue = isTrue;
    }

    /// <summary>
    /// Gets the whole text that has been rendered to the <see cref="Writer"/>.
    /// </summary>
    /// <returns></returns>
    public string GetDocumentText ()
    {
      Writer.Flush();
      Reader.BaseStream.Seek (0, SeekOrigin.Begin);
      return Reader.ReadToEnd();
    }

    /// <summary>
    /// Parses the string obtained from <see cref="GetDocumentText"/> and builds an XML DOM document.
    /// </summary>
    /// <returns>The <see cref="XmlDocument"/> built from the document text.</returns>
    public XmlDocument GetResultDocument ()
    {
      XmlDocument document = new XmlDocument();
      using (TextReader reader = new StringReader (GetDocumentText().Replace ("&nbsp;", "&amp;nbsp;")))
      {
        document.Load (reader);
      }
      return document;
    }

    /// <summary>
    /// Asserts that <paramref name="parent"/> has exactly <paramref name="count"/> child nodes of type <see cref="XmlNodeType.Element"/>.
    /// </summary>
    public void AssertChildElementCount (XmlNode parent, int count)
    {
      int elementCount = 0;
      foreach (XmlNode node in parent.ChildNodes)
      {
        if (node.NodeType == XmlNodeType.Element)
          ++elementCount;
      }
      AssertAreEqual (count, elementCount, "Element '{0}' has {1} child elements instead of the expected {2}.", parent.Name, elementCount, count);
    }

    /// <summary>
    /// Asserts that <paramref name="parent"/> has a child element with a <see cref="XmlNode.Name"/> 
    /// property value of <paramref name="tag"/> at position <paramref name="index"/> of its child node collection.
    /// </summary>
    /// <returns>The specified child element.</returns>
    public XmlNode GetAssertedChildElement (XmlNode parent, string tag, int index)
    {
      AssertGreaterThan (
          parent.ChildNodes.Count,
          index,
          "Node {0} has only {1} children - index {2} out of range.",
          parent.Name,
          parent.ChildNodes.Count,
          index);

      XmlNode node = parent.ChildNodes[index];

      AssertAreEqual (
          XmlNodeType.Element,
          node.NodeType,
          "{0}.ChildNodes[{1}].NodeType is {2}, not {3}.",
          parent.Name,
          index,
          parent.ChildNodes[index].NodeType,
          XmlNodeType.Element);

      AssertAreEqual (tag, node.Name, "Unexpected element tag.");
      return node;
    }

    /// <summary>
    /// Asserts that <paramref name="parent"/> has a child text node with an <see cref="XmlNode.InnerText"/> 
    /// property value of <paramref name="content"/> at position <paramref name="index"/> of its child node collection.
    /// </summary>
    public void AssertTextNode (XmlNode parent, string content, int index)
    {
      AssertGreaterThan (
          parent.ChildNodes.Count, index, "Node {0} has only {1} children - index {2} out of range.", parent.Name, parent.ChildNodes.Count, index);

      AssertAreEqual (
          XmlNodeType.Text,
          parent.ChildNodes[index].NodeType,
          "{0}.ChildNodes[{1}].NodeType is {2}, not {3}.",
          parent.Name,
          index,
          parent.ChildNodes[index].NodeType,
          XmlNodeType.Text);

      var node = (XmlText) parent.ChildNodes[index];

      AssertAreEqual (content, node.InnerText.Trim(), "Unexpected text node content.");
    }

    /// <summary>
    /// Asserts that <paramref name="node"/> has an attribute named <paramref name="attributeName"/> with value <paramref name="attributeValue"/>.
    /// </summary>
    public void AssertAttribute (XmlNode node, string attributeName, string attributeValue)
    {
      AssertAttribute (node, attributeName, attributeValue, AttributeValueCompareMode.Equal);
    }

    /// <summary>
    /// Asserts that <paramref name="node"/> has an attribute named <paramref name="attributeName"/>,
    /// and that the attribute's value is equal to or contains <paramref name="attributeValue"/>, 
    /// depending on <paramref name="mode"/>.
    /// </summary>
    public void AssertAttribute (XmlNode node, string attributeName, string attributeValue, AttributeValueCompareMode mode)
    {
      XmlAttribute attribute = node.Attributes[attributeName];
      AssertNotNull (attribute, "Attribute {0}.{1} does not exist.", node.Name, attributeName);

      if (attributeValue != null)
      {
        switch (mode)
        {
          case AttributeValueCompareMode.Equal:
            AssertAreEqual (attributeValue, attribute.Value, string.Format ("Attribute {0}.{1}", node.Name, attribute.Name));
            break;
          case AttributeValueCompareMode.Contains:
            AssertTrue (
                attribute.Value.Contains (attributeValue),
                "Unexpected attribute value in {0}.{1}: should contain {2}, but was {3}",
                node.Name,
                attribute.Name,
                attributeValue,
                attribute.Value);
            break;
        }
      }
    }

    /// <summary>
    /// Asserts that <paramref name="node"/> has a 'style' attribute that contains the name-value-pair
    /// specified by <paramref name="cssProperty"/> and <paramref name="cssValue"/>.
    /// </summary>
    public void AssertStyleAttribute (XmlNode node, string cssProperty, string cssValue)
    {
      XmlAttribute attribute = node.Attributes["style"];
      AssertNotNull (attribute, "Attribute {0}.{1}", node.Name, "style");

      string stylePart = string.Format ("{0}:{1};", cssProperty, cssValue);
      AssertTrue (
          attribute.Value.Contains (stylePart),
          "Attribute {0}.{1} does not contain '{2}' - value is '{3}'.",
          node.Name,
          attribute.Name,
          stylePart,
          attribute.Value);
    }

    /// <summary>
    /// Asserts that <paramref name="node"/> does not contain an attribute named <paramref name="attributeName"/>.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="attributeName"></param>
    public void AssertNoAttribute (XmlNode node, string attributeName)
    {
      AssertIsNull (node.Attributes[attributeName], "Attribute '{0}' is present although it should not be.", attributeName);
    }

    /// <summary>
    /// Disposes of the <see cref="Writer"/> and its underlying streams.
    /// </summary>
    public void Dispose ()
    {
      Reader.Dispose();
      Stream.Dispose();
      Writer.Dispose();
    }

    /// <summary>
    /// The <see cref="HtmlTextWriter"/> to use for rendering in order work with this class.
    /// </summary>
    public HtmlTextWriter Writer
    {
      get { return _writer; }
    }

    protected MemoryStream Stream
    {
      get { return _stream; }
    }

    protected StreamReader Reader
    {
      get { return _reader; }
    }

    protected BinaryAssertDelegate AssertAreEqual
    {
      get { return _assertAreEqual; }
    }

    protected BinaryAssertDelegate AssertGreaterThan
    {
      get { return _assertGreaterThan; }
    }

    protected UnaryAssertDelegate AssertIsNull
    {
      get { return _assertIsNull; }
    }

    protected UnaryAssertDelegate AssertNotNull
    {
      get { return _assertNotNull; }
    }

    protected ConditionAssertDelegate AssertTrue
    {
      get { return _assertTrue; }
    }
  }
}
