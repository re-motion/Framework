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
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListValidationSummaryRendererTest : BocListRendererTestBase
  {
    [Flags]
    private enum TestSettings
    {
      None = 0,
      ScreenReader = 1,
      DiagnosticMetadata = 2,
      NoLinks = 4
    }

    private BocListCssClassDefinition _bocListCssClassDefinition;
    private Mock<IBocListColumnIndexProvider> _columnIndexProviderMock;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _bocListCssClassDefinition = new BocListCssClassDefinition();
      _columnIndexProviderMock = new Mock<IBocListColumnIndexProvider>(MockBehavior.Strict);
    }

    private TypeWithReference RowObject => ((TypeWithReference)BusinessObject).FirstValue;

    [Test]
    public void Render_NoValidationErrors ()
    {
      RenderAndAssertValidationSummary(
          TestSettings.None,
          Array.Empty<BocListValidationFailureWithLocationInformation>(),
          Array.Empty<(string, string)>());
    }

    [Test]
    public void Render_ListValidationFailures ()
    {
      var propertyStub = new Mock<IBusinessObjectProperty>();
      propertyStub.Setup(e => e.DisplayName).Returns("ColumnName");

      var validationFailures = new[]
                               {
                                   CreateValidationFailure("Failure A"),
                                   CreateRowValidationFailure("Failure B")
                               };

      RenderAndAssertValidationSummary(
          TestSettings.None,
          validationFailures,
          new[] { CreateTextAssert("Failure A"), CreateTextAssert("Failure B") });
    }

    [Test]
    public void Render_CellFailureWithoutLink ()
    {
      var propertyStub = new Mock<IBusinessObjectProperty>();
      propertyStub.Setup(e => e.DisplayName).Returns("ColumnName");

      var columnDefinitionStub1 = new Mock<BocColumnDefinition>();
      columnDefinitionStub1.Setup(e => e.ColumnTitle).Returns(WebString.CreateFromText("Incorrect title"));
      columnDefinitionStub1.Setup(e => e.ColumnTitleDisplayValue).Returns(WebString.CreateFromText("MyColumnTitle"));
      columnDefinitionStub1.Setup(e => e.ItemID).Returns("MyColumnItemID");

      _columnIndexProviderMock.Setup(e => e.GetVisibleColumnIndex(columnDefinitionStub1.Object)).Returns(1);

      var validationFailures = new[]
                               {
                                   CreateCellValidationFailure("My first failure", propertyStub.Object, columnDefinitionStub1.Object),
                               };

      RenderAndAssertValidationSummary(
          TestSettings.NoLinks,
          validationFailures,
          new[]
          {
              CreateTextAssert("ColumnName: My first failure"),
          });
    }

    [Test]
    public void Render_UsesColumnTitleDisplayValue ()
    {
      var propertyStub = new Mock<IBusinessObjectProperty>();
      propertyStub.Setup(e => e.DisplayName).Returns("ColumnName");

      var columnDefinitionStub1 = new Mock<BocColumnDefinition>();
      columnDefinitionStub1.Setup(e => e.ColumnTitle).Returns(WebString.CreateFromText("Incorrect title"));
      columnDefinitionStub1.Setup(e => e.ColumnTitleDisplayValue).Returns(WebString.CreateFromText("MyColumnTitle"));
      columnDefinitionStub1.Setup(e => e.ItemID).Returns("MyColumnItemID");

      var columnDefinitionStub2 = new Mock<BocColumnDefinition>();
      columnDefinitionStub2.Setup(e => e.ColumnTitle).Returns(WebString.CreateFromText("Incorrect title"));
      columnDefinitionStub2.Setup(e => e.ColumnTitleDisplayValue).Returns(WebString.CreateFromText(""));
      columnDefinitionStub2.Setup(e => e.ItemID).Returns("MyColumnItemID");

      _columnIndexProviderMock.Setup(e => e.GetVisibleColumnIndex(columnDefinitionStub1.Object)).Returns(1);
      _columnIndexProviderMock.Setup(e => e.GetVisibleColumnIndex(columnDefinitionStub2.Object)).Returns(2);

      var validationFailures = new[]
                               {
                                   CreateCellValidationFailure("My first failure", propertyStub.Object, columnDefinitionStub1.Object),
                                   CreateCellValidationFailure("My second failure", propertyStub.Object, columnDefinitionStub2.Object)
                               };

      RenderAndAssertValidationSummary(
          TestSettings.None,
          validationFailures,
          new[]
          {
              CreateLinkAssert("MyColumnTitle: My first failure", "#MyList_C1_R3_ValidationMarker"),
              CreateLinkAssert("Column 2: My second failure", "#MyList_C2_R3_ValidationMarker")
          });
    }

    [Test]
    public void Render_WithDiagnosticMetadata ()
    {
      var propertyStub = new Mock<IBusinessObjectProperty>();
      propertyStub.Setup(e => e.DisplayName).Returns("MyPropertyDisplayName");
      propertyStub.Setup(e => e.Identifier).Returns("MyPropertyIdentifier");

      var columnDefinitionStub = new Mock<BocColumnDefinition>();
      columnDefinitionStub.Setup(e => e.ColumnTitle).Returns(WebString.CreateFromText("MyColumnTitle"));
      columnDefinitionStub.Setup(e => e.ItemID).Returns("MyColumnItemID");

      _columnIndexProviderMock.Setup(e => e.GetVisibleColumnIndex(columnDefinitionStub.Object)).Returns(1);

      var validationFailures = new[]
                               {
                                   CreateValidationFailure("Failure A"),
                                   CreateRowValidationFailure("Failure B"),
                                   CreateCellValidationFailure("Failure C", propertyStub.Object, columnDefinitionStub.Object)
                               };

      var document = RenderValidationSummary(TestSettings.DiagnosticMetadata, validationFailures);
      Html.AssertChildElementCount(document, 1);

      var ul = Html.GetAssertedChildElement(document, "ul", 0);
      Html.AssertChildElementCount(ul, 3);

      var li1 = Html.GetAssertedChildElement(ul, "li", 0);
      Html.AssertAttribute(li1, DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceRow, "");
      Html.AssertAttribute(li1, DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceColumn, "");
      Html.AssertAttribute(li1, DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceBusinessObject, "");
      Html.AssertAttribute(li1, DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceProperty, "");

      var li2 = Html.GetAssertedChildElement(ul, "li", 1);
      Html.AssertAttribute(li2, DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceRow, RowObject.UniqueIdentifier);
      Html.AssertAttribute(li2, DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceColumn, "");
      Html.AssertAttribute(li2, DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceBusinessObject, RowObject.UniqueIdentifier);
      Html.AssertAttribute(li2, DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceProperty, "");

      var li3 = Html.GetAssertedChildElement(ul, "li", 2);
      Html.AssertAttribute(li3, DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceRow, RowObject.UniqueIdentifier);
      Html.AssertAttribute(li3, DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceColumn, "MyColumnItemID");
      Html.AssertAttribute(li3, DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceBusinessObject, RowObject.UniqueIdentifier);
      Html.AssertAttribute(li3, DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceProperty, "MyPropertyIdentifier");
    }

    private (string Text, string Href) CreateTextAssert (string text) => (text, null);

    private (string Text, string Href) CreateLinkAssert (string text, string href) => (text, href);

    private BocListValidationFailureWithLocationInformation CreateValidationFailure (string errorMessage)
    {
      var failure = BusinessObjectValidationFailure.Create(errorMessage);
      return BocListValidationFailureWithLocationInformation.CreateFailure(failure);
    }

    private BocListValidationFailureWithLocationInformation CreateRowValidationFailure (string errorMessage)
    {
      var rowObject = RowObject;
      var failure = BusinessObjectValidationFailure.CreateForBusinessObject(RowObject, errorMessage);
      return BocListValidationFailureWithLocationInformation.CreateFailureForRow(failure, rowObject);
    }

    private BocListValidationFailureWithLocationInformation CreateCellValidationFailure (string errorMessage, IBusinessObjectProperty property, BocColumnDefinition columnDefinition)
    {
      var rowObject = RowObject;
      var failure = BusinessObjectValidationFailure.CreateForBusinessObjectProperty(errorMessage, rowObject, property);
      return BocListValidationFailureWithLocationInformation.CreateFailureForCell(failure, rowObject, columnDefinition);
    }

    private void RenderAndAssertValidationSummary (TestSettings settings, BocListValidationFailureWithLocationInformation[] validationFailures, (string Text, string Href)[] listElements)
    {
      var document = RenderValidationSummary(settings, validationFailures);
      Html.AssertChildElementCount(document, 1);

      var ul = Html.GetAssertedChildElement(document, "ul", 0);
      Html.AssertChildElementCount(ul, listElements.Length);

      for (var i = 0; i < listElements.Length; i++)
      {
        var (text, href) = listElements[i];
        var li = Html.GetAssertedChildElement(ul, "li", i);
        Html.AssertChildElementCount(li, 1);
        Assert.That(li.InnerText, Is.EqualTo(text));

        var isLink = href != null;
        if (isLink)
        {
          var a = Html.GetAssertedChildElement(li, "a", 0);
          Html.AssertChildElementCount(a, 0);
          Html.AssertAttribute(a, "href", href);
          Html.AssertAttribute(a, "onclick", "BocList.OnInlineValidationEntryClick(event);");
          Html.AssertAttribute(a, "tabindex", "-1");
        }
        else
        {
          var span = Html.GetAssertedChildElement(li, "span", 0);
          Html.AssertChildElementCount(span, 0);
        }
      }
    }

    private XmlDocument RenderValidationSummary (TestSettings settings, BocListValidationFailureWithLocationInformation[] validationFailures)
    {
      var bocRenderingContext = new BocRenderingContext<IBocList>(HttpContext, Html.Writer, List.Object);
      IRenderingFeatures renderingFeatures = settings.HasFlag(TestSettings.DiagnosticMetadata)
          ? new WithDiagnosticMetadataRenderingFeatures()
          : new DefaultRenderingFeatures();

      var validationSummaryRenderer = new BocListValidationSummaryRenderer(renderingFeatures, _bocListCssClassDefinition);
      var renderArguments = new BocListValidationSummaryRenderArguments(
          _columnIndexProviderMock.Object,
          validationFailures,
          3,
          renderCellValidationFailuresAsLinks: (settings & TestSettings.NoLinks) == 0);
      validationSummaryRenderer.Render(bocRenderingContext, in renderArguments);

      return Html.GetResultDocument();
    }
  }
}
