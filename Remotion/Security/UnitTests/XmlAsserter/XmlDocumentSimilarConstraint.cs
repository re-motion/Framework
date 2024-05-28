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
using System.Xml;

namespace Remotion.Security.UnitTests.XmlAsserter
{
  public class XmlDocumentSimilarConstraint : XmlDocumentBaseConstraint
  {
    private readonly NodeStackToXPathConverter _nodeStackToXPathConverter;

    public XmlDocumentSimilarConstraint (XmlDocument expected)
        : base(expected)
    {
      _nodeStackToXPathConverter = new NodeStackToXPathConverter();
      _nodeStackToXPathConverter.IncludeNamespaces = true;
    }

    protected override bool CompareDocuments (XmlDocument expectedDocument, XmlDocument actualDocument)
    {
      XmlNode expectedFirstChild = expectedDocument.FirstChild;
      if (expectedFirstChild.NodeType == XmlNodeType.XmlDeclaration)
        expectedFirstChild = expectedDocument.ChildNodes[1];

      if (!ContainsNodeStack(expectedFirstChild, actualDocument))
        return false;

      XmlNode actualFirstChild = actualDocument.FirstChild;
      if (actualFirstChild.NodeType == XmlNodeType.XmlDeclaration)
        actualFirstChild = actualDocument.ChildNodes[1];

      return ContainsNodeStack(actualFirstChild, expectedDocument);
    }

    private bool ContainsNodeStack (XmlNode node, XmlDocument testDocument)
    {
      Stack<XmlNode> nodeStack = GetNodeStack(node);
      string xPathExpression = _nodeStackToXPathConverter.GetXPathExpression(nodeStack);
      if (string.IsNullOrEmpty(xPathExpression))
        return true;

      XmlNodeList nodes = testDocument.SelectNodes(xPathExpression, _nodeStackToXPathConverter.NamespaceManager);
      if (nodes.Count == 0)
      {
        Messages.Add(xPathExpression + " Evaluation failed.");
        Messages.Add("Node missing in actual document:");
        ShowNodeStack(node, Messages.Add);

        if (node.ParentNode != null)
        {
          Stack<XmlNode> parentNodeStack = GetNodeStack(node.ParentNode);
          xPathExpression = _nodeStackToXPathConverter.GetXPathExpression(parentNodeStack);
          XmlNodeList actualNodes = testDocument.SelectNodes(xPathExpression, _nodeStackToXPathConverter.NamespaceManager);
          if (actualNodes.Count > 0)
            ShowNodeStack(actualNodes[0], Messages.Add);
        }
        return false;
      }

      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (!ContainsNodeStack(childNode, testDocument))
          return false;
      }

      return true;
    }

    public override string Description
    {
      get
      {
        // typically, NUnit prints the entire content of the expected value since XMLDocument can be long blocks of texts,
        // we are skipping this because it does not seem helpful.
        return "similar to the expected XmlDocument.";
      }
    }
  }
}
