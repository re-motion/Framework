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
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocSelectorColumnQuirksModeRendererTest : BocListRendererTestBase
  {
    private BocListQuirksModeCssClassDefinition _bocListQuirksModeCssClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      List.Stub (mock => mock.IsSelectionEnabled).Return (true);

      _bocListQuirksModeCssClassDefinition = new BocListQuirksModeCssClassDefinition();
    }

    [Test]
    public void RenderTitleCellForMultiSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
      renderer.RenderTitleCell (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", _bocListQuirksModeCssClassDefinition.TitleCell);

      var input = Html.GetAssertedChildElement (th, "input", 0);
      Html.AssertAttribute (input, "type", "checkbox");
      Html.AssertAttribute (input, "name", List.GetSelectAllControlName());
      Html.AssertNoAttribute (input, "value");
      Html.AssertAttribute (input, "alt", "Select all rows");
    }

    [Test]
    public void RenderDataCellForMultiSelect ()
    {
      var row = new BocListRow (1, BusinessObject);
      List.Stub (mock => mock.Selection).Return (RowSelection.Multiple);
      List.Stub (mock => mock.GetSelectorControlValue (row)).Return ("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
      renderer.RenderDataCell (
          new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]),
          new BocListRowRenderingContext (row, 0, false),
          "bocListTableCell");
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", "bocListTableCell");

      var input = Html.GetAssertedChildElement (td, "input", 0);
      Html.AssertAttribute (input, "type", "checkbox");
      Html.AssertAttribute (input, "id", "SelectRowControl_UnqiueID_0");
      Html.AssertAttribute (input, "name", "SelectRowControl$UnqiueID");
      Html.AssertAttribute (input, "value", "row1");
      Html.AssertAttribute (input, "alt", "Select this row");
    }

    [Test]
    public void RenderTitleCellForSingleSelect ()
    {
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
      renderer.RenderTitleCell (new BocListRenderingContext(HttpContext, Html.Writer, List, new BocColumnRenderer[0]));

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", _bocListQuirksModeCssClassDefinition.TitleCell);

      Html.AssertTextNode (th, HtmlHelper.WhiteSpace, 0);
    }

    [Test]
    public void RenderDataCellForSingleSelect ()
    {
      var row = new BocListRow (1, BusinessObject);
      List.Stub (mock => mock.Selection).Return (RowSelection.SingleRadioButton);
      List.Stub (mock => mock.GetSelectorControlValue (row)).Return ("row1");
      IBocSelectorColumnRenderer renderer = new BocSelectorColumnQuirksModeRenderer (_bocListQuirksModeCssClassDefinition);
      renderer.RenderDataCell (
          new BocListRenderingContext (HttpContext, Html.Writer, List, new BocColumnRenderer[0]),
          new BocListRowRenderingContext (row, 0, false),
          "bocListTableCell");
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", "bocListTableCell");

      var input = Html.GetAssertedChildElement (td, "input", 0);
      Html.AssertAttribute (input, "type", "radio");
      Html.AssertAttribute (input, "id", "SelectRowControl_UnqiueID_0");
      Html.AssertAttribute (input, "name", "SelectRowControl$UnqiueID");
      Html.AssertAttribute (input, "value", "row1");
      Html.AssertAttribute (input, "alt", "Select this row");
    }
  }
}