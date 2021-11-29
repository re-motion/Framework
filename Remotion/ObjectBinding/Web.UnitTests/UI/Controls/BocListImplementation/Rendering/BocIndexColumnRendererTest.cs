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
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocIndexColumnRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      List.Setup(mock => mock.IsIndexEnabled).Returns(true);
    }

    [Test]
    public void RenderIndexTitleCell ()
    {
      List.Setup(mock => mock.Index).Returns(RowIndex.InitialOrder);

      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer(RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderTitleCell(CreateRenderingContext());

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement(document, "th", 0);
      Html.AssertAttribute(th, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(th, "class", _bocListCssClassDefinition.TitleCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(th, "role", "columnheader");

      var span = Html.GetAssertedChildElement(th, "span", 0);
      Html.AssertTextNode(span, "No.", 0);
    }

    [Test]
    public void RenderIndexDataCellInitialOrder ()
    {
      List.Setup(mock => mock.Index).Returns(RowIndex.InitialOrder);

      RenderIndexDataCell(0);
    }

    [Test]
    public void RenderIndexDataCellSortedOrderAndIndexOffset ()
    {
      List.Setup(mock => mock.Index).Returns(RowIndex.SortedOrder);
      List.Setup(mock => mock.IndexOffset).Returns(2);

      RenderIndexDataCell(2);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInTitleCell ()
    {
      List.Setup(mock => mock.Index).Returns(RowIndex.InitialOrder);
      List.Setup(mock => mock.IndexColumnTitle).Returns("My_IndexColumn");

      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition);
      renderer.RenderTitleCell(CreateRenderingContext());

      var document = Html.GetResultDocument();
      var th = Html.GetAssertedChildElement(document, "th", 0);
      Html.AssertAttribute(th, DiagnosticMetadataAttributes.Content, "My_IndexColumn");
      Html.AssertAttribute(th, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 1.ToString());
    }

    private void RenderIndexDataCell (int indexOffset)
    {
      IBocIndexColumnRenderer renderer = new BocIndexColumnRenderer(RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition);
      const string cssClassTableCell = "bocListTableCell";
      renderer.RenderDataCell(CreateRenderingContext(), 0, 0, cssClassTableCell);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", cssClassTableCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCellIndex, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute(td, "role", "cell");
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 1.ToString());

      var label = Html.GetAssertedChildElement(td, "span", 0);
      Html.AssertAttribute(label, "class", _bocListCssClassDefinition.Content);

      Html.AssertTextNode(label, (1 + indexOffset).ToString(), 0);
    }

    private BocListRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      return new BocListRenderingContext(HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, new BocColumnRenderer[0]);
    }
  }
}