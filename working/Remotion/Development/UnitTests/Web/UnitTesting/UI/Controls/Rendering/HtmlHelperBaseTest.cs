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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Rhino.Mocks;

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
                                      "  </MidLevelElement>" +
                                      "</TopLevelElement>";

    private HtmlHelperBase _htmlHelper;
    private IAsserter _asserter;

    [SetUp]
    public void SetUp ()
    {
      _asserter = MockRepository.GenerateStub<IAsserter>();
      _asserter.Stub (stub => stub.NotNull (null, null, null)).IgnoreArguments().WhenCalled (
          invocation =>
          {
            if (invocation.Arguments[0] == null)
              throw new Exception (string.Format ((string) invocation.Arguments[1], (object[]) invocation.Arguments[2]));
          });
      _asserter.Stub (stub => stub.IsNull (null, null, null)).IgnoreArguments().WhenCalled (
          invocation =>
          {
            if (invocation.Arguments[0] != null)
              throw new Exception (string.Format ((string) invocation.Arguments[1], (object[]) invocation.Arguments[2]));
          });
      _asserter.Stub (stub => stub.AreEqual (null, null, null, null)).IgnoreArguments().WhenCalled (
          invocation =>
          {
            if (invocation.Arguments[0] != null)
            {
              if (!invocation.Arguments[0].Equals (invocation.Arguments[1]))
                throw new Exception (string.Format ((string) invocation.Arguments[2], (object[]) invocation.Arguments[3]));
            }
            else if (invocation.Arguments[1] != null)
              throw new Exception (string.Format ((string) invocation.Arguments[2], (object[]) invocation.Arguments[3]));
          });
      _asserter.Stub (stub => stub.GreaterThan (null, null, null, null)).IgnoreArguments().WhenCalled (
          invocation =>
          {
            if (invocation.Arguments[0] != null)
            {
              if (((IComparable) invocation.Arguments[0]).CompareTo (invocation.Arguments[1]) <= 0)
                throw new Exception (string.Format ((string) invocation.Arguments[2], (object[]) invocation.Arguments[3]));
            }
          });
      _asserter.Stub (stub => stub.IsTrue (true, null, new object[3])).IgnoreArguments().WhenCalled (
          invocation =>
          {
            if (!((bool) invocation.Arguments[0]))
              throw new Exception (string.Format ((string) invocation.Arguments[1], (object[]) invocation.Arguments[2]));
          })
          ;

      _htmlHelper = new ConcreteHtmlHelper (_asserter);
      _htmlHelper.Writer.Write (c_document);
    }

    [TearDown]
    public void TearDown ()
    {
      _htmlHelper.Dispose();
    }

    [Test]
    public void Succeed_GetDocumentText ()
    {
      Assert.That (_htmlHelper.GetDocumentText(), Is.EqualTo (c_document));
    }

    [Test]
    public void Succeed_GetResultDocument ()
    {
      var document = _htmlHelper.GetResultDocument();
      Assert.That (document, Is.Not.Null);
      Assert.That (document.DocumentElement, Is.Not.Null);
      Assert.That (document.DocumentElement.Name, Is.EqualTo ("TopLevelElement"));
    }

    [Test]
    public void Succeed_GetAssertedElement ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = _htmlHelper.GetAssertedChildElement (document, "TopLevelElement", 0);
      Assert.That (topLevelElement, Is.Not.Null);
      Assert.That (topLevelElement.Name, Is.EqualTo ("TopLevelElement"));

      var midLevelElement = _htmlHelper.GetAssertedChildElement (topLevelElement, "MidLevelElement", 0);
      Assert.That (midLevelElement, Is.Not.Null);
      Assert.That (midLevelElement.Name, Is.EqualTo ("MidLevelElement"));

      var innerElement1 = _htmlHelper.GetAssertedChildElement (midLevelElement, "InnerElement1", 0);
      Assert.That (innerElement1, Is.Not.Null);
      Assert.That (innerElement1.Name, Is.EqualTo ("InnerElement1"));

      var innerElement2 = _htmlHelper.GetAssertedChildElement (midLevelElement, "InnerElement2", 2);
      Assert.That (innerElement2, Is.Not.Null);
      Assert.That (innerElement2.Name, Is.EqualTo ("InnerElement2"));
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Unexpected element tag.")]
    public void Fail_GetAssertedElement_WrongTag ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = _htmlHelper.GetAssertedChildElement (document, "dummy", 0);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Node #document has only 1 children - index 2 out of range.")]
    public void Fail_GetAssertedElement_IndexOutOfRange ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = _htmlHelper.GetAssertedChildElement (document, "TopLevelElement", 2);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "MidLevelElement.ChildNodes[1].NodeType is Text, not Element.")]
    public void Fail_GetAssertedElement_NoElementIndex ()
    {
      var document = _htmlHelper.GetResultDocument();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      _htmlHelper.GetAssertedChildElement (midLevelElement, "Text", 1);
    }

    [Test]
    public void Succeed_AssertChildElementCount ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertChildElementCount (topLevelElement, 1);

      var midLevelElement = topLevelElement.ChildNodes[0];
      _htmlHelper.AssertChildElementCount (midLevelElement, 2);

      var innerElement1 = midLevelElement.ChildNodes[0];
      _htmlHelper.AssertChildElementCount (innerElement1, 0);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Element 'MidLevelElement' has 2 child elements instead of the expected 3.")]
    public void Fail_AssertChildElementCount ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      var midLevelElement = topLevelElement.ChildNodes[0];
      _htmlHelper.AssertChildElementCount (midLevelElement, 3);
    }

    [Test]
    public void Succeed_AssertAttribute_FullAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertAttribute (topLevelElement, "topLevelAttribute", "topLevelAttributeValue");
      _htmlHelper.AssertAttribute (topLevelElement, "topLevelAttribute", "topLevelAttributeValue", HtmlHelperBase.AttributeValueCompareMode.Equal);
      _htmlHelper.AssertAttribute (topLevelElement, "topLevelAttribute", "topLevelAttributeValue", HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    [Test]
    public void Succeed_AssertAttribute_PartialAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      var midLevelElement = topLevelElement.ChildNodes[0];

      _htmlHelper.AssertAttribute (
          midLevelElement,
          "midLevelAttribute",
          "midLevelAttributeValue1",
          HtmlHelperBase.AttributeValueCompareMode.Contains);

      _htmlHelper.AssertAttribute (
          midLevelElement,
          "midLevelAttribute",
          "midLevelAttributeValue2",
          HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Attribute MidLevelElement.midLevelAttribute")]
    public void Fail_AssertAttribute_PartialAttributeValueWithDefaultMode ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertAttribute (topLevelElement, "topLevelAttribute", "topLevelAttributeValue");

      var midLevelElement = topLevelElement.ChildNodes[0];
      _htmlHelper.AssertAttribute (midLevelElement, "midLevelAttribute", "midLevelAttributeValue1");
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Attribute TopLevelElement.topLevelAttribute")]
    public void Fail_AssertAttribute_DifferentAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertAttribute (topLevelElement, "topLevelAttribute", "otherAttributeValue");
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Unexpected attribute value in TopLevelElement.topLevelAttribute: " +
                                                              "should contain otherAttributeValue, but was topLevelAttributeValue")]
    public void Fail_AssertAttribute_NotContainedAttributeValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertAttribute (topLevelElement, "topLevelAttribute", "otherAttributeValue", HtmlHelperBase.AttributeValueCompareMode.Contains);
    }

    [Test]
    public void Succeed_AssertNoAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertNoAttribute (topLevelElement, "nonExistingAttribute");
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Attribute 'topLevelAttribute' is present although it should not be.")]
    public void Fail_AssertNoAttribute_ExistingAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var topLevelElement = document.DocumentElement;
      _htmlHelper.AssertNoAttribute (topLevelElement, "topLevelAttribute");
    }

    [Test]
    public void Succeed_AssertStyleAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var innerElement1 = document.DocumentElement.ChildNodes[0].ChildNodes[0];
      _htmlHelper.AssertStyleAttribute (innerElement1, "styleAttribute1", "styleAttributeValue1");
      _htmlHelper.AssertStyleAttribute (innerElement1, "styleAttribute2", "styleAttributeValue2");
    }

    [Test]
    [ExpectedException (typeof (Exception),
        ExpectedMessage = "Attribute InnerElement1.style does not contain 'styleAttribute1:otherStyleAttributeValue;'" +
                          " - value is 'styleAttribute1:styleAttributeValue1;styleAttribute2:styleAttributeValue2;'.")]
    public void Fail_AssertStyleAttribute_WrongValue ()
    {
      var document = _htmlHelper.GetResultDocument();
      var innerElement1 = document.DocumentElement.ChildNodes[0].ChildNodes[0];
      _htmlHelper.AssertStyleAttribute (innerElement1, "styleAttribute1", "otherStyleAttributeValue");
    }

    [Test]
    [ExpectedException (typeof (Exception),
        ExpectedMessage = "Attribute InnerElement1.style does not contain 'otherStyleAttribute:dummyValue;'" +
                          " - value is 'styleAttribute1:styleAttributeValue1;styleAttribute2:styleAttributeValue2;'.")]
    public void Fail_AssertStyleAttribute_NonExistingAttribute ()
    {
      var document = _htmlHelper.GetResultDocument();
      var innerElement1 = document.DocumentElement.ChildNodes[0].ChildNodes[0];
      _htmlHelper.AssertStyleAttribute (innerElement1, "otherStyleAttribute", "dummyValue");
    }

    [Test]
    public void Succeed_AssertTextNode ()
    {
      var document = _htmlHelper.GetResultDocument();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      _htmlHelper.AssertTextNode (midLevelElement, "MidLevelTextContent", 1);

      var innerElement1 = midLevelElement.ChildNodes[0];
      _htmlHelper.AssertTextNode (innerElement1, "InnerElement1TextContent", 0);

      var innerElement2 = midLevelElement.ChildNodes[2];
      _htmlHelper.AssertTextNode (innerElement2, "InnerElement2TextContent", 0);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "MidLevelElement.ChildNodes[0].NodeType is Element, not Text.")]
    public void Fail_AssertTextNode_WrongIndex ()
    {
      var document = _htmlHelper.GetResultDocument ();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      _htmlHelper.AssertTextNode (midLevelElement, "MidLevelTextContent", 0);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Unexpected text node content.")]
    public void Fail_AssertTextNode_WrongContent ()
    {
      var document = _htmlHelper.GetResultDocument ();
      var midLevelElement = document.DocumentElement.ChildNodes[0];
      _htmlHelper.AssertTextNode (midLevelElement, "OtherTextContent", 1);
    }
  }
}
