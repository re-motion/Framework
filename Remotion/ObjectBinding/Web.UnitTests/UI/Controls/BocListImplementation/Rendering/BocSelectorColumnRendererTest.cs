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
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocSelectorColumnRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocListRenderingContext _bocListRenderingContext;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.IsSelectionEnabled).Return (true);

      _bocListCssClassDefinition = new BocListCssClassDefinition();
      _bocListRenderingContext = new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]);
    }

    [Test]
    public void RenderTitleCellForMultiSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderTitleCell (_bocListRenderingContext);

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", _bocListCssClassDefinition.TitleCell);

      var input = Html.GetAssertedChildElement (th, "input", 0);
      Html.AssertAttribute (input, "type", "checkbox");
      Html.AssertAttribute (input, "name", List.GetSelectAllControlName ());
      Html.AssertNoAttribute (input, "value");
      Html.AssertAttribute (input, "alt", "Select all rows");
    }

    [Test]
    public void RenderDataCellForMultiSelect ()
    {
      var row = new BocListRow (1, BusinessObject);
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      List.Stub (mock => mock.GetSelectorControlValue (row)).Return ("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (
          _bocListRenderingContext,
          new BocListRowRenderingContext (row, 0, false),
          "bocListTableCell");

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", "bocListTableCell");

      var input = Html.GetAssertedChildElement (td, "input", 0);
      Html.AssertAttribute (input, "type", "checkbox");
      Html.AssertAttribute (input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute (input, "name", "SelectRowControl$UnqiueID");
      Html.AssertAttribute (input, "value", "row1");
      Html.AssertAttribute (input, "alt", "Select this row");
    }

    [Test]
    public void RenderTitleCellForSingleSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderTitleCell (_bocListRenderingContext);

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", _bocListCssClassDefinition.TitleCell);

      Html.AssertTextNode (th, HtmlHelper.WhiteSpace, 0);
    }

    [Test]
    public void RenderDataCellForSingleSelect ()
    {
      var row = new BocListRow (1, BusinessObject);
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      List.Stub (mock => mock.GetSelectorControlValue (row)).Return ("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (
          _bocListRenderingContext,
          new BocListRowRenderingContext (row, 0, false),
          "bocListTableCell");
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", "bocListTableCell");

      var input = Html.GetAssertedChildElement (td, "input", 0);
      Html.AssertAttribute (input, "type", "radio");
      Html.AssertAttribute (input, "id", "SelectRowControl_UnqiueID_1");
      Html.AssertAttribute (input, "name", "SelectRowControl$UnqiueID");
      Html.AssertAttribute (input, "value", "row1");
      Html.AssertAttribute (input, "alt", "Select this row");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInTitleCell ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition);
      renderer.RenderTitleCell (_bocListRenderingContext);

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 1.ToString());

      var input = Html.GetAssertedChildElement (th, "input", 0);
      Html.AssertAttribute (input, DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownSelectAllControl, "true");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInDataCell ()
    {
      var row = new BocListRow (1, BusinessObject);
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      List.Stub (mock => mock.GetSelectorControlValue (row)).Return ("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnRenderer (RenderingFeatures.WithDiagnosticMetadata, _bocListCssClassDefinition);
      renderer.RenderDataCell (
          _bocListRenderingContext,
          new BocListRowRenderingContext (row, 0, false),
          "bocListTableCell");
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 1.ToString());
    }
  }
}