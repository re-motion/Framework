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
using System.Xml;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.XmlAsserter
{
  [TestFixture]
  public class XmlDocumentEqualConstraintTest
  {
    [Test]
    public void CompareEmptyDocuments ()
    {
      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches (actual), Is.True);
    }

    [Test]
    public void CompareWithNullDocument ()
    {
      XmlDocument expected = new XmlDocument ();

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches ((object)null), Is.False);
    }

    [Test]
    public void ExpectedDocumentIsNull ()
    {
      XmlDocument actual = new XmlDocument ();

      var constraint = new XmlDocumentEqualConstraint (null);
      Assert.That (constraint.Matches(actual), Is.False);
    }

    [Test]
    public void ExpectedAndActualDocumentsAreNull ()
    {
      var constraint = new XmlDocumentEqualConstraint (null);
      Assert.That (constraint.Matches((object)null), Is.True);
    }

    [Test]
    public void CompareWithDocumentContainingOnlyRootNode ()
    {
      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches (actual), Is.False);
    }

    [Test]
    public void CompareDifferentRootNodes ()
    {
      string expectedXml = @"<rootA xmlns=""http://test"" />";

      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      expected.LoadXml (expectedXml);

      XmlElement actualRootNode = actual.CreateElement ("rootB", "http://test");
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches(actual), Is.False);
    }

    [Test]
    public void CompareEqualRootNodes ()
    {
      string expectedXml = @"<root xmlns=""http://test"" />";

      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      expected.LoadXml (expectedXml);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches (actual), Is.True);
    }

    [Test]
    public void CompareRootNodesWithDifferentChildNodes ()
    {
      string expectedXml = @"
<root xmlns=""http://test"">
  <child />
</root>";

      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      expected.LoadXml (expectedXml);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      XmlElement actualChildNode = actual.CreateElement ("childA", "http://test");
      actualRootNode.AppendChild (actualChildNode);
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches(actual), Is.False);
    }

    [Test]
    public void CompareRootNodesWithDifferentAttributes ()
    {
      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      XmlElement expectedRootNode = expected.CreateElement ("root", "http://test");
      XmlAttribute expectedAttribute = expected.CreateAttribute ("name");
      expectedAttribute.Value = "root";
      expectedRootNode.Attributes.Append (expectedAttribute);
      expected.AppendChild (expectedRootNode);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      XmlAttribute actualAttribute = actual.CreateAttribute ("id");
      actualAttribute.Value = "root";
      actualRootNode.Attributes.Append (actualAttribute);
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches (actual), Is.False);
    }

    [Test]
    public void CompareRootNodesWithSameAttributes ()
    {
      string expectedXml = @"<root xmlns=""http://test"" name=""root"" />";

      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      expected.LoadXml (expectedXml);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      XmlAttribute actualAttribute = actual.CreateAttribute ("name");
      actualAttribute.Value = "root";
      actualRootNode.Attributes.Append (actualAttribute);
      actual.AppendChild (actualRootNode);

      var constraint= new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches (actual), Is.True);
    }

    [Test]
    public void CompareRootNodesWithDifferentAttributeValues ()
    {
      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      XmlElement expectedRootNode = expected.CreateElement ("root", "http://test");
      XmlAttribute expectedAttribute = expected.CreateAttribute ("name");
      expectedAttribute.Value = "root";
      expectedRootNode.Attributes.Append (expectedAttribute);
      expected.AppendChild (expectedRootNode);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      XmlAttribute actualAttribute = actual.CreateAttribute ("name");
      actualAttribute.Value = "second";
      actualRootNode.Attributes.Append (actualAttribute);
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (constraint.Matches (actual), Is.False);
    }

    [Test]
    public void CompareRootNodesWithSameValues ()
    {
      string expectedXml = @"<root xmlns=""http://test"">Hello</root>";

      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      expected.LoadXml (expectedXml);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      XmlText actualText = actual.CreateTextNode ("Hello");
      actualRootNode.AppendChild (actualText);
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint(expected);
      Assert.That (constraint.Matches (actual), Is.True);
    }

    [Test]
    public void IntegrationTest ()
    {
      string expectedXml = @"
<root xmlns=""http://test"">
  <elements>
    <element id=""id1"" name=""Hello"">Hello, </element>
    <element id=""id2"" name=""World"">World!</element>
  </elements>
</root>";

      XmlDocument expected = new XmlDocument ();
      XmlDocument actual = new XmlDocument ();

      expected.LoadXml (expectedXml);

      XmlElement actualRootNode = actual.CreateElement ("root", "http://test");
      XmlElement elementsNode = actual.CreateElement ("elements", "http://test");
      
      XmlElement element1Node = actual.CreateElement ("element", "http://test");
      XmlAttribute element1IDAttribute = actual.CreateAttribute ("id");
      element1IDAttribute.Value = "id1";
      element1Node.Attributes.Append (element1IDAttribute);
      XmlAttribute element1NameAttribute = actual.CreateAttribute ("name");
      element1NameAttribute.Value = "Hello";
      element1Node.Attributes.Append (element1NameAttribute);
      XmlText element1Text = actual.CreateTextNode ("Hello, ");
      element1Node.AppendChild (element1Text);
      elementsNode.AppendChild (element1Node);

      XmlElement element2Node = actual.CreateElement ("element", "http://test");
      XmlAttribute element2IDAttribute = actual.CreateAttribute ("id");
      element2IDAttribute.Value = "id2";
      element2Node.Attributes.Append (element2IDAttribute);
      XmlAttribute element2NameAttribute = actual.CreateAttribute ("name");
      element2NameAttribute.Value = "World";
      element2Node.Attributes.Append (element2NameAttribute);
      XmlText element2Text = actual.CreateTextNode ("World!");
      element2Node.AppendChild (element2Text);
      elementsNode.AppendChild (element2Node);

      actualRootNode.AppendChild (elementsNode);
      actual.AppendChild (actualRootNode);

      var constraint = new XmlDocumentEqualConstraint (expected);
      Assert.That (actual, constraint);
    }
  }
}
