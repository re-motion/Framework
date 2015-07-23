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
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererBaseTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocSimpleColumnDefinition> _renderingContext;
    private const string c_columnCssClass = "cssClassColumn";

    private BocSimpleColumnDefinition Column { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Column = new BocSimpleColumnDefinition();
      Column.ColumnTitle = "TestColumn1";
      Column.CssClass = c_columnCssClass;

      Initialize();

      var editModeController = MockRepository.GenerateMock<IEditModeController>();
      editModeController.Stub (mock => mock.RenderTitleCellMarkers (Html.Writer, Column, 0)).WhenCalled (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write (string.Empty));

      List.Stub (mock => mock.EditModeController).Return (editModeController);

      List.Stub (mock => mock.IsClientSideSortingEnabled).Return (true);
      List.Stub (mock => mock.IsShowSortingOrderEnabled).Return (true);

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      _renderingContext =
          new BocColumnRenderingContext<BocSimpleColumnDefinition> (new BocColumnRenderingContext (HttpContext, Html.Writer, List, Column, 0, 0));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (Column.GetRenderer (DefaultServiceLocator.Create()).IsNull, Is.False);
    }

    [Test]
    public void RenderTitleCellAscendingZero ()
    {
      RenderTitleCell (SortingDirection.Ascending, 0, "SortAscending.gif", "Sorted Ascending");
    }

    [Test]
    public void RenderTitleCellDescendingZero ()
    {
      RenderTitleCell (SortingDirection.Descending, 0, "SortDescending.gif", "Sorted Descending");
    }

    [Test]
    public void RenderTitleCellAscendingThree ()
    {
      RenderTitleCell (SortingDirection.Ascending, 3, "SortAscending.gif", "Sorted Ascending");
    }

    [Test]
    public void RenderTitleCellDescendingFour ()
    {
      RenderTitleCell (SortingDirection.Descending, 4, "SortDescending.gif", "Sorted Descending");
    }

    [Test]
    public void RenderTitleCellNoSorting ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderTitleCell (_renderingContext, SortingDirection.None, -1);

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (th, "class", c_columnCssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Assert.Less (0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement (th, "a", 0);
      Html.AssertAttribute (sortCommandLink, "id", List.ClientID + "_0_SortCommand");
      Html.AssertTextNode (sortCommandLink, Column.ColumnTitleDisplayValue, 0);

      Html.AssertChildElementCount (sortCommandLink, 0);
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition);

      _renderingContext =
          new BocColumnRenderingContext<BocSimpleColumnDefinition> (new BocColumnRenderingContext (HttpContext, Html.Writer, List, Column, 0, 6));

      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 7.ToString());
    }

    [Test]
    public void TestDiagnosticMetadataRenderingInTitle ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition);

      Column.ItemID = "TestItemID";

      _renderingContext =
          new BocColumnRenderingContext<BocSimpleColumnDefinition> (new BocColumnRenderingContext (HttpContext, Html.Writer, List, Column, 0, 6));

      renderer.RenderTitleCell (_renderingContext, SortingDirection.None, 0);

      var document = Html.GetResultDocument();
      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, DiagnosticMetadataAttributes.ItemID, Column.ItemID);
      Html.AssertAttribute (th, DiagnosticMetadataAttributes.Content, Column.ColumnTitleDisplayValue);
      Html.AssertAttribute (th, DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, 7.ToString());
      Html.AssertAttribute (th, DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasDiagnosticMetadata, "true");
    }

    private void RenderTitleCell (
        SortingDirection sortDirection,
        int sortIndex,
        string iconFilename,
        string iconAltText)
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderTitleCell (_renderingContext, sortDirection, sortIndex);

      var document = Html.GetResultDocument();

      var th = Html.GetAssertedChildElement (document, "th", 0);
      Html.AssertAttribute (th, "class", _bocListCssClassDefinition.TitleCell, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (th, "class", c_columnCssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Assert.Less (0, th.ChildNodes.Count);
      var sortCommandLink = Html.GetAssertedChildElement (th, "a", 0);
      Html.AssertAttribute (sortCommandLink, "id", List.ClientID + "_0_SortCommand");
      Html.AssertTextNode (sortCommandLink, Column.ColumnTitleDisplayValue + HtmlHelper.WhiteSpace, 0);

      var sortOrderSpan = Html.GetAssertedChildElement (sortCommandLink, "span", 1);
      Html.AssertAttribute (sortOrderSpan, "class", _bocListCssClassDefinition.SortingOrder, HtmlHelperBase.AttributeValueCompareMode.Contains);

      var sortIcon = Html.GetAssertedChildElement (sortOrderSpan, "img", 0);
      Html.AssertAttribute (sortIcon, "src", iconFilename, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertAttribute (sortIcon, "alt", iconAltText);

      Html.AssertTextNode (sortOrderSpan, HtmlHelper.WhiteSpace + (sortIndex + 1), 1);
    }
  }
}