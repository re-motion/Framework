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
using System.Text;
using System.Xml;
using NUnit.Framework.Constraints;

namespace Remotion.Security.UnitTests.XmlAsserter
{
  public abstract class XmlDocumentBaseConstraint : Constraint
  {
    protected delegate void MessageListenerDelegate (string messageInfo);
    protected List<string> Messages;
    private readonly XmlDocument _expectedDocument;

    protected XmlDocumentBaseConstraint (XmlDocument expected)
    {
      _expectedDocument = expected;
      Messages = new List<string>();
    }

    public override bool Matches (object actual)
    {
      base.actual = actual;
      var actualAsXmlDocument = actual as XmlDocument;
      if (actualAsXmlDocument == null && _expectedDocument == null)
        return true;

      if (actualAsXmlDocument == null || _expectedDocument == null)
        return false;

      return CompareDocuments (_expectedDocument, actualAsXmlDocument);
    }

    public override void WriteDescriptionTo (MessageWriter writer)
    {
      throw new NotImplementedException();
    }

    public override void WriteMessageTo (MessageWriter writer)
    {
      writer.Write (String.Join("\n", Messages.ToArray()));
    }

    protected abstract bool CompareDocuments (XmlDocument expectedDocument, XmlDocument actualDocument);

    protected void ShowNodeStack (XmlNode node, MessageListenerDelegate messageListener)
    {
      var nodeStack = GetNodeStack (node);

      while (nodeStack.Count > 0)
        messageListener (GetNodeInfo (nodeStack.Pop ()));
    }

    protected Stack<XmlNode> GetNodeStack (XmlNode node)
    {
      var nodeStack = new Stack<XmlNode> ();

      var currentNode = node;
      while (currentNode != null && !(currentNode is XmlDocument))
      {
        nodeStack.Push (currentNode);
        currentNode = currentNode.ParentNode;
      }

      return nodeStack;
    }

    private string GetNodeInfo (XmlNode node)
    {
      return node.NamespaceURI + ":" + node.LocalName + GetAttributeInfo (node.Attributes) + GetNodeValueInfo (node.Value);
    }

    private string GetAttributeInfo (XmlAttributeCollection attributes)
    {
      if (attributes == null || attributes.Count == 0)
        return string.Empty;

      var attributeInfoBuilder = new StringBuilder ();

      foreach (XmlAttribute attribute in attributes)
      {
        if (attributeInfoBuilder.Length > 0)
          attributeInfoBuilder.Append (", ");

        attributeInfoBuilder.Append (attribute.NamespaceURI + ":" + attribute.Name + "=\"" + attribute.Value + "\"");
      }

      return "[" + attributeInfoBuilder.ToString () + "]";
    }

    private string GetNodeValueInfo (string nodeValue)
    {
      if (nodeValue == null)
        return string.Empty;

      return " = \"" + nodeValue + "\"";
    }
  }
}