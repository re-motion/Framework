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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocRowRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderer[] _columnRenderers;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);
      var stubColumnDefinition = new StubColumnDefinition();
      List.Setup(mock => mock.AreDataRowsClickSensitive()).Returns(true);

      _columnRenderers = new[]
                         {
                             new BocColumnRenderer(
                                 new StubColumnRenderer(new FakeResourceUrlFactory()),
                                 stubColumnDefinition,
                                 0,
                                 0,
                                 false,
                                 SortingDirection.Ascending,
                                 0)
                         };

      _bocListCssClassDefinition = new BocListCssClassDefinition();
    }

    [Test]
    public void RenderTitlesRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderTitlesRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "th", 0);
    }

    [Test]
    public void RenderTitlesRowWithIndex ()
    {
      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
      List.Setup(mock => mock.Index).Returns(RowIndex.InitialOrder);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderTitlesRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      var thIndex = Html.GetAssertedChildElement(tr, "th", 0);
      Html.AssertAttribute(thIndex, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(thIndex, "class", _bocListCssClassDefinition.TitleCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Html.GetAssertedChildElement(tr, "th", 1);
    }

    [Test]
    public void RenderTitlesRowWithSelector ()
    {
      List.Setup(mock => mock.IsSelectionEnabled).Returns(true);
      List.Setup(mock => mock.Selection).Returns(RowSelection.Multiple);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderTitlesRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "th", 0);

      Html.GetAssertedChildElement(tr, "th", 1);
    }

    [Test]
    public void RenderDataRow ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderDataRow(
          CreateRenderingContext(),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "class", _bocListCssClassDefinition.DataRow + " " + _bocListCssClassDefinition.DataRowOdd);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "td", 0);
    }

    [Test]
    public void RenderDataRowSelected ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderDataRow(
          CreateRenderingContext(),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, true),
          0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(
          tr,
          "class",
          _bocListCssClassDefinition.DataRow + " " + _bocListCssClassDefinition.DataRowOdd + " " + _bocListCssClassDefinition.DataRowSelected);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRow ()
    {
      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
      List.Setup(mock => mock.IsSelectionEnabled).Returns(true);

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderEmptyListDataRow(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, "role", "row");

      Html.GetAssertedChildElement(tr, "td", 0);
    }

    [Test]
    public void RenderEmptyDataRowEmptyListMessageWebString ()
    {
      List.Setup(mock => mock.EmptyListMessage).Returns(WebString.CreateFromText("Multiline\nEmptyListMessage"));

      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition),
          RenderingFeatures.Default);
      renderer.RenderEmptyListDataRow(CreateRenderingContext());

      var document = Html.GetResultDocument();
      var messageElement = document.SelectSingleNode("/tr/td");
      Assert.That(messageElement.InnerXml, Is.EqualTo("Multiline<br />EmptyListMessage"));
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      IBocRowRenderer renderer = new BocRowRenderer(
          _bocListCssClassDefinition,
          new BocIndexColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition),
          new BocSelectorColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition),
          RenderingFeatures.WithDiagnosticMetadata);
      renderer.RenderDataRow(
          CreateRenderingContext(),
          new BocListRowRenderingContext(new BocListRow(0, BusinessObject), 1, false),
          0);

      var document = Html.GetResultDocument();

      var tr = Html.GetAssertedChildElement(document, "tr", 0);
      Html.AssertAttribute(tr, DiagnosticMetadataAttributes.ItemID, ((IBusinessObjectWithIdentity)BusinessObject).UniqueIdentifier);
      Html.AssertAttribute(tr, DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex, 1.ToString());
    }

    private BocListRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      return new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, _columnRenderers);
    }
  }
}
