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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;

namespace Remotion.Development.UnitTests.Web.UnitTesting.UI.Controls.Rendering
{
  [TestFixture]
  public class HtmlHelperBaseTest
  {
    private const string c_document = "<TopLevelElement topLevelAttribute='topLevelAttributeValue'>" +
                                      "  <MidLevelElement midLevelAttribute='midLevelAttributeValue1 midLevelAttributeValue2'>" +
                                      "    <InnerElement1 style='styleAttribute1:styleAttributeValue1;styleAttribute2:styleAttributeValue2;'>" +
                                      "      InnerElement1TextContent" +
                                      "    </InnerElement1>" +
                                      "    MidLevelTextContent" +
                                      "    <InnerElement2>InnerElement2TextContent</InnerElement2>" +
                                      "    <InnerElement3 id='SomeID'></InnerElement3>" +
                                      "    <InnerElement4 class='foo SomeClass'></InnerElement4>" +
                                      "    <InnerElement5 class='SomeClass'></InnerElement5>" +
                                      "    <InnerElement6 id='foo'></InnerElement6>" +
                                      "    <InnerElement7 id='foo'></InnerElement7>" +
                                      "  </MidLevelElement>" +
                                      "</TopLevelElement>";

    private HtmlHelperBase _htmlHelper;
    private Mock<IAsserter> _asserter;

    [SetUp]
    public void SetUp ()
    {
      _asserter = new Mock<IAsserter>();
      _asserter
          .Setup(stub => stub.NotNull(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<object[]>()))
          .Callback(
              (object actual, string message, object[] args) =>
              {
                if (actual == null)
                  throw new Exception(string.Format(message, args));
              });

      _asserter
          .Setup(stub => stub.IsNull(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<object[]>()))
          .Callback(
              (object actual, string message, object[] args) =>
              {
                if (actual != null)
                  throw new Exception(string.Format(message, args));
              });

      _asserter
          .Setup(stub => stub.AreEqual(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<string>(), It.IsAny<object[]>()))
          .Callback(
              (object expected, object actual, string message, object[] args) =>
              {
                if (actual != null)
                {
                  if (!actual.Equals(expected))
                    throw new Exception(string.Format(message, args));
                }
                else if (expected != null)
                  throw new Exception(string.Format(message, args));
              });

      _asserter
          .Setup(stub => stub.GreaterThan(It.IsAny<IComparable>(), It.IsAny<IComparable>(), It.IsAny<string>(), It.IsAny<object[]>()))
          .Callback(
              (IComparable left, IComparable right, string message, object[] args) =>
              {
                if (left != null)
                {
                  if (left.CompareTo(right) <= 0)
                    throw new Exception(string.Format(message, args));
                }
              });

      _asserter
          .Setup(stub => stub.IsTrue(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<object[]>()))
          .Callback(
              (bool condition, string message, object[] args) =>
              {
                if (!condition)
                  throw new Exception(string.Format(message, args));
              });

      _htmlHelper = new ConcreteHtmlHelper(_asserter.Object);
      _htmlHelper.Writer.Write(c_document);
    }

    [TearDown]
    public void TearDown ()
    {
      _htmlHelper.Dispose();
    }

    [Test]
    public void Succeed_GetDocumentText ()
    {
      Assert.That(_htmlHelper.GetDocumentText(), Is.EqualTo(c_document));
    }

    [Test]
    public void Succeed_GetResultDocument ()
    {
      var document = _htmlHelper.GetResultDocument();
      Assert.That(document, Is.Not.Null);
      Assert.That(document.DocumentElement, Is.Not.Null);
      Assert.That(document.DocumentElement.Name, Is.EqualTo("TopLevelElement"));
    }

    [Test]
    public void Fail_GetResultDocument ()
    {
      _htmlHelper.Writer.Write("randomError");

      Assert.That(
          () => _htmlHelper.GetResultDocument(),
          Throws.Exception
              .TypeOf<XmlException>()
              .With.Message.EqualTo("Data at the root level is invalid. Line 1, position 665.\r\n\r\nContent:\r\n" + c_document + "randomError")
              .And.With.InnerException.Null);
    }

    [Test]
    public void Succeed_GetAssertedElement ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = _htmlHelper.GetAssertedChildElement(document, "TopLevelElement", 0);
      Assert.That(topLevelElement, Is.Not.Null);
      Assert.That(topLevelElement.Name, Is.EqualTo("TopLevelElement"));

      var midLevelElement = _htmlHelper.GetAssertedChildElement(topLevelElement, "MidLevelElement", 0);
      Assert.That(midLevelElement, Is.Not.Null);
      Assert.That(midLevelElement.Name, Is.EqualTo("MidLevelElement"));

      var innerElement1 = _htmlHelper.GetAssertedChildElement(midLevelElement, "InnerElement1", 0);
      Assert.That(innerElement1, Is.Not.Null);
      Assert.That(innerElement1.Name, Is.EqualTo("InnerElement1"));

      var innerElement2 = _htmlHelper.GetAssertedChildElement(midLevelElement, "InnerElement2", 2);
      Assert.That(innerElement2, Is.Not.Null);
      Assert.That(innerElement2.Name, Is.EqualTo("InnerElement2"));
    }

    [Test]
    public void Fail_GetAssertedElement_WrongTag ()
    {
      var document = _htmlHelper.GetResultDocument();
      Assert.That(
          () => _htmlHelper.GetAssertedChildElement(document, "dummy", 0),
          Throws.Exception
              .With.Message.EqualTo("Unexpected element tag."));
    }

    [Test]
    public void Fail_GetAssertedElement_IndexOutOfRange ()
    {
      var document = _htmlHelper.GetResultDocument();
      Assert.That(
          () => _htmlHelper.GetAssertedChildElement(document, "TopLevelElement", 2),
          Throws.Exception
              .With.Message.EqualTo(
                  "Node #document has only 1 children - index 2 out of range."));
    }

    [Test]
    public void Fail_GetAssertedElement_NoElementIndex ()
    {
      var document = _htmlHelper.GetResultDocument();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      Assert.That(
          () => _htmlHelper.GetAssertedChildElement(midLevelElement, "Text", 1),
          Throws.Exception
              .With.Message.EqualTo(
                  "MidLevelElement.ChildNodes[1].NodeType is Text, not Element."));
    }

    [Test]
    public void Succeed_AssertChildElementCount ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertChildElementCount(topLevelElement, 1);

      var midLevelElement = topLevelElement.ChildNodes[0];
      _htmlHelper.AssertChildElementCount(midLevelElement, 7);

      var innerElement1 = midLevelElement.ChildNodes[0];
      _htmlHelper.AssertChildElementCount(innerElement1, 0);
    }

    [Test]
    public void Fail_AssertChildElementCount ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      var midLevelElement = topLevelElement.ChildNodes[0];
      Assert.That(
          () => _htmlHelper.AssertChildElementCount(midLevelElement, 3),
          Throws.Exception
              .With.Message.EqualTo(
                  "Element 'MidLevelElement' has 7 child elements instead of the expected 3."));
    }

    [Test]
    public void Succeed_AssertAttribute_FullAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertAttribute(topLevelElement, "topLevelAttribute", "topLevelAttributeValue");
      _htmlHelper.AssertAttribute(topLevelElement, "topLevelAttribute", "topLevelAttributeValue", HtmlHelperBase.AttributeValueCompareMode.Equal);
      _htmlHelper.AssertAttribute(topLevelElement, "topLevelAttribute", "topLevelAttributeValue", HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    [Test]
    public void Succeed_AssertAttribute_PartialAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      var midLevelElement = topLevelElement.ChildNodes[0];

      _htmlHelper.AssertAttribute(
          midLevelElement,
          "midLevelAttribute",
          "midLevelAttributeValue1",
          HtmlHelperBase.AttributeValueCompareMode.Contains);

      _htmlHelper.AssertAttribute(
          midLevelElement,
          "midLevelAttribute",
          "midLevelAttributeValue2",
          HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    [Test]
    public void Fail_AssertAttribute_PartialAttributeValueWithDefaultMode ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertAttribute(topLevelElement, "topLevelAttribute", "topLevelAttributeValue");

      var midLevelElement = topLevelElement.ChildNodes[0];
      Assert.That(
          () => _htmlHelper.AssertAttribute(midLevelElement, "midLevelAttribute", "midLevelAttributeValue1"),
          Throws.Exception
              .With.Message.EqualTo("Attribute MidLevelElement.midLevelAttribute"));
    }

    [Test]
    public void Fail_AssertAttribute_DifferentAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      Assert.That(
          () => _htmlHelper.AssertAttribute(topLevelElement, "topLevelAttribute", "otherAttributeValue"),
          Throws.Exception
              .With.Message.EqualTo("Attribute TopLevelElement.topLevelAttribute"));
    }

    [Test]
    public void Fail_AssertAttribute_NotContainedAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      Assert.That(
          () => _htmlHelper.AssertAttribute(
              topLevelElement,
              "topLevelAttribute",
              "otherAttributeValue",
              HtmlHelperBase.AttributeValueCompareMode.Contains),
          Throws.Exception
              .With.Message.EqualTo(
                  "Unexpected attribute value in TopLevelElement.topLevelAttribute: " +
                  "should contain otherAttributeValue, but was topLevelAttributeValue"));
    }

    [Test]
    public void Succeed_AssertNoAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertNoAttribute(topLevelElement, "nonExistingAttribute");
    }

    [Test]
    public void Fail_AssertNoAttribute_ExistingAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      Assert.That(
          () => _htmlHelper.AssertNoAttribute(topLevelElement, "topLevelAttribute"),
          Throws.Exception
              .With.Message.EqualTo(
                  "Attribute 'topLevelAttribute' is present although it should not be."));
    }

    [Test]
    public void Succeed_AssertStyleAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var innerElement1 = document.DocumentElement.ChildNodes[0].ChildNodes[0];
      _htmlHelper.AssertStyleAttribute(innerElement1, "styleAttribute1", "styleAttributeValue1");
      _htmlHelper.AssertStyleAttribute(innerElement1, "styleAttribute2", "styleAttributeValue2");
    }

    [Test]
    public void Fail_AssertStyleAttribute_WrongValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var innerElement1 = document.DocumentElement.ChildNodes[0].ChildNodes[0];
      Assert.That(
          () => _htmlHelper.AssertStyleAttribute(innerElement1, "styleAttribute1", "otherStyleAttributeValue"),
          Throws.Exception
              .With.Message.EqualTo(
                  "Attribute InnerElement1.style does not contain 'styleAttribute1:otherStyleAttributeValue;'" +
                  " - value is 'styleAttribute1:styleAttributeValue1;styleAttribute2:styleAttributeValue2;'."));
    }

    [Test]
    public void Fail_AssertStyleAttribute_NonExistingAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var innerElement1 = document.DocumentElement.ChildNodes[0].ChildNodes[0];
      Assert.That(
          () => _htmlHelper.AssertStyleAttribute(innerElement1, "otherStyleAttribute", "dummyValue"),
          Throws.Exception
              .With.Message.EqualTo(
                  "Attribute InnerElement1.style does not contain 'otherStyleAttribute:dummyValue;'" +
                  " - value is 'styleAttribute1:styleAttributeValue1;styleAttribute2:styleAttributeValue2;'."));
    }

    [Test]
    public void Succeed_AssertTextNode ()
    {
      var document = _htmlHelper.GetResultDocument();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      _htmlHelper.AssertTextNode(midLevelElement, "MidLevelTextContent", 1);

      var innerElement1 = midLevelElement.ChildNodes[0];
      _htmlHelper.AssertTextNode(innerElement1, "InnerElement1TextContent", 0);

      var innerElement2 = midLevelElement.ChildNodes[2];
      _htmlHelper.AssertTextNode(innerElement2, "InnerElement2TextContent", 0);
    }

    [Test]
    public void Fail_AssertTextNode_WrongIndex ()
    {
      var document = _htmlHelper.GetResultDocument();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      Assert.That(
          () => _htmlHelper.AssertTextNode(midLevelElement, "MidLevelTextContent", 0),
          Throws.Exception
              .With.Message.EqualTo(
                  "MidLevelElement.ChildNodes[0].NodeType is Element, not Text."));
    }

    [Test]
    public void Fail_AssertTextNode_WrongContent ()
    {
      var document = _htmlHelper.GetResultDocument();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      Assert.That(
          () => _htmlHelper.AssertTextNode(midLevelElement, "OtherTextContent", 1),
          Throws.Exception
              .With.Message.EqualTo("Unexpected text node content."));
    }

    [Test]
    public void Succeed_GetAssertedElementByID ()
    {
      var document = _htmlHelper.GetResultDocument();

      var element = _htmlHelper.GetAssertedElementByID(document, "SomeID");

      Assert.That(element.Name, Is.EqualTo("InnerElement3"));
    }

    [Test]
    public void Succeed_GetAssertedElementByClass ()
    {
      var document = _htmlHelper.GetResultDocument();

      var element = _htmlHelper.GetAssertedElementByClass(document, "SomeClass", 0);

      Assert.That(element.Name, Is.EqualTo("InnerElement4"));
    }

    [Test]
    public void Succeed_GetAssertedElementByClass_WithIndex ()
    {
      var document = _htmlHelper.GetResultDocument();

      var element = _htmlHelper.GetAssertedElementByClass(document, "SomeClass", 1);

      Assert.That(element.Name, Is.EqualTo("InnerElement5"));
    }

    [Test]
    public void Fail_GetAssertedElementByID_DuplicateID ()
    {
      var document = _htmlHelper.GetResultDocument();

      Assert.That(
          () => _htmlHelper.GetAssertedElementByID(document, "foo"),
          Throws.Exception.With.Message.EqualTo("2 elements were found for the ID 'foo', but exactly one element was expected."));
    }

    [Test]
    public void Fail_GetAssertedElementByClass_IndexExceedsCount ()
    {
      var document = _htmlHelper.GetResultDocument();

      Assert.That(
          () => _htmlHelper.GetAssertedElementByClass(document, "SomeClass", 2),
          Throws.Exception.With.Message.EqualTo("Node #document has only 2 nested elements with a class matching 'SomeClass' - index 2 out of range."));
    }
  }
}
