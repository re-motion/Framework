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
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocCommandColumnRendererTest : ColumnRendererTestBase<BocCommandColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocCommandColumnDefinition> _renderingContext;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCommandColumnDefinition();
      Column.Command = new BocListItemCommand(CommandType.Event);
      Column.Command.EventCommand = new Command.EventCommandInfo();
      Column.Command.EventCommand.RequiresSynchronousPostBack = true;
      Column.Text = WebString.CreateFromText("TestCommand");
      Column.ColumnTitle = WebString.CreateFromText("FirstColumn");

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      base.SetUp();
      Column.OwnerControl = List.Object;

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocCommandColumnDefinition>(
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

    [TearDown]
    public void TearDown ()
    {
      WebConfigurationMock.Current = new WebConfigurationMock();
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocCommandColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(rowIndex: 5));

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var a = Html.GetAssertedChildElement(cellStructureDiv, "a", 0);
      Html.AssertAttribute(a, "id", List.Object.ClientID + "_Column_0_Command_Row_10");
      Html.AssertAttribute(a, "href", "fakeFallbackUrl");
      Html.AssertAttribute(a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");

      Html.AssertTextNode(a, "TestCommand", 0);
    }

    [Test]
    public void RenderTextWebString ()
    {
      Column.Text = WebString.CreateFromText("Multiline\nText");
      IBocColumnRenderer renderer = new BocCommandColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(rowIndex: 5));

      var document = Html.GetResultDocument();
      var title = document.GetAssertedElementByID(List.Object.ClientID + "_Column_0_Command_Row_10");
      Assert.That(title.InnerXml, Is.EqualTo("Multiline<br />Text"));
    }

    [Test]
    public void RenderIconCell ()
    {
      IBocColumnRenderer renderer = new BocCommandColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(showIcon: true));

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var a = Html.GetAssertedChildElement(cellStructureDiv, "a", 0);
      Html.AssertAttribute(a, "href", "fakeFallbackUrl");
      Html.AssertAttribute(a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");

      Html.AssertIcon(a, EventArgs.BusinessObject, null);

      Html.AssertTextNode(a, HtmlHelper.WhiteSpace + "TestCommand", 1);
    }

    [Test]
    public void RenderCommandIconCell ()
    {
      Column.Icon.Url = "~/Images/CommandIcon.gif";
      Column.Icon.Width = new Unit(16, UnitType.Pixel);
      Column.Icon.Height = new Unit(16, UnitType.Pixel);

      IBocColumnRenderer renderer = new BocCommandColumnRenderer(
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

      var a = Html.GetAssertedChildElement(cellStructureDiv, "a", 0);
      Html.AssertAttribute(a, "href", "fakeFallbackUrl");
      Html.AssertAttribute(a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");

      Html.AssertIcon(a, EventArgs.BusinessObject, Column.Icon.Url.TrimStart('~'));

      Html.AssertTextNode(a, "TestCommand", 1);
    }

    [Test]
    public void RenderDisabledCommandForWaiConformanceLevelA ()
    {
      WebConfigurationMock.Current.Wcag.ConformanceLevel = WaiConformanceLevel.A;

      IBocColumnRenderer renderer = new BocCommandColumnRenderer(
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
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);

      Html.AssertTextNode(div, "TestCommand", 0);
    }
  }
}
