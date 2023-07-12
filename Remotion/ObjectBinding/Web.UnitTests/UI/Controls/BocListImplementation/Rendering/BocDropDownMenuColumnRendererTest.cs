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
using System.Collections.ObjectModel;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocDropDownMenuColumnRendererTest : ColumnRendererTestBase<BocDropDownMenuColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocDropDownMenuColumnDefinition> _renderingContext;
    private Mock<DropDownMenu> Menu { get; set; }

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocDropDownMenuColumnDefinition();
      Column.ColumnTitle = WebString.CreateFromText("FirstColumn");
      Column.MenuTitleText = WebString.CreateFromText("Menu Title");
      Column.MenuTitleIcon = new IconInfo("~/Images/MenuTitleIcon.gif", 16, 16);

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      base.SetUp();

      List.Setup(mock => mock.HasMenuBlock).Returns(true);
      List.Setup(mock => mock.RowMenuDisplay).Returns(RowMenuDisplay.Manual);


      Menu = new Mock<DropDownMenu>(List.Object);
      Menu
          .Setup(menuMock => menuMock.RenderControl(Html.Writer))
          .Callback((HtmlTextWriter writer) => writer.Write("mocked dropdown menu"));

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(List.Object.DataSource, List.Object.Property, "Args");
      _renderingContext = new BocColumnRenderingContext<BocDropDownMenuColumnDefinition>(
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
    public void RenderCellWithPopulatedMenu ()
    {
      InitializeRowMenus();
      Menu.Object.MenuItems.Add(
          new WebMenuItem(
              "itemId",
              "category",
              WebString.CreateFromText("text"),
              new IconInfo("~/Images/NullImage.gif"),
              new IconInfo("~/Images/NullImage.gif"),
              WebMenuItemStyle.Text,
              RequiredSelection.Any,
              false,
              new Command()));

      IBocColumnRenderer renderer = new BocDropDownMenuColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(div, "onclick", "BocList.OnCommandClick();");

      Html.AssertTextNode(div, "mocked dropdown menu", 0);
    }

    [Test]
    public void RenderCellWithEmptyMenu ()
    {
      InitializeRowMenus();

      IBocColumnRenderer renderer = new BocDropDownMenuColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(div, "onclick", "BocList.OnCommandClick();");

      Html.AssertTextNode(div, "mocked dropdown menu", 0);
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      InitializeRowMenus();

      IBocColumnRenderer renderer = new BocDropDownMenuColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell, "true");
    }

    private void InitializeRowMenus ()
    {
      var rowMenus = new ReadOnlyCollection<DropDownMenu>(new[] { Menu.Object, Menu.Object });
      List.Setup(mock => mock.RowMenus).Returns(rowMenus);
    }
  }
}
