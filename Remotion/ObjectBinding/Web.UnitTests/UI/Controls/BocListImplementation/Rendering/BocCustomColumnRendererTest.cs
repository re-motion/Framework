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
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocCustomColumnRendererTest : ColumnRendererTestBase<BocCustomColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocCustomColumnDefinition> _renderingContext;
    private Mock<ISmartControl> _smartControlMock;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCustomColumnDefinition();
      Column.CustomCell = new StubCustomCellDefinition();

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      _smartControlMock = new Mock<Control>().As<ISmartControl>();

      base.SetUp();

      IBusinessObject firstObject = (IBusinessObject)((TypeWithReference)BusinessObject).FirstValue;
      IBusinessObject secondObject = (IBusinessObject)((TypeWithReference)BusinessObject).SecondValue;
      var triplets = new[]
                     {
                         new BocListCustomColumnTuple(firstObject, 10, new WebControl(HtmlTextWriterTag.Div)),
                         new BocListCustomColumnTuple(secondObject, 20, new HtmlGenericControl("div")),
                         new BocListCustomColumnTuple(secondObject, 30, (Control)_smartControlMock.Object)
                     };
      var customColumns =
          new ReadOnlyDictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]>(
              new Dictionary<BocCustomColumnDefinition, BocListCustomColumnTuple[]>
              {
                  { Column, triplets }
              });
      List.Setup(mock => mock.CustomColumns).Returns(customColumns);

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocCustomColumnDefinition>(
          new BocColumnRenderingContext(
              HttpContext,
              Html.Writer,
              List.Object,
              businessObjectWebServiceContext,
              Column,
              ColumnIndexProvider.Object,
              0,
              0));
    }

    [Test]
    public void RenderCellWithInnerWebControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var args = new BocListDataRowRenderEventArgs(10, EventArgs.BusinessObject, EventArgs.IsEditableRow, EventArgs.IsOddRow);
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(args));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(span, "onclick", "BocList.OnCommandClick();");
    }

    [Test]
    public void RenderCellWithInnerHtmlControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var args = new BocListDataRowRenderEventArgs(20, EventArgs.BusinessObject, EventArgs.IsEditableRow, EventArgs.IsOddRow);
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(args));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(span, "onclick", "BocList.OnCommandClick();");
    }

    [Test]
    public void RenderCellWithInnerSmartControl ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.ControlsInAllRows;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      var args = new BocListDataRowRenderEventArgs(30, EventArgs.BusinessObject, EventArgs.IsEditableRow, EventArgs.IsOddRow);
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(args, headerIDs: new[] { "h1", "h2" }));

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var span = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(span, "onclick", "BocList.OnCommandClick();");

      _smartControlMock.Verify(_ => _.AssignLabels(new[] { "h1", "h2" }));
    }

    [Test]
    public void RenderCellDirectly ()
    {
      Column.Mode = BocCustomColumnDefinitionMode.NoControls;

      IBocColumnRenderer renderer = new BocCustomColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");
    }
  }
}
