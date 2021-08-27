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
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocListMenuBlockRendererTest : BocListRendererTestBase
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _bocListCssClassDefinition = new BocListCssClassDefinition();
    }

    [Test]
    public void RenderWithAvailableViews ()
    {
      var dropDownList = new Mock<DropDownList>();
      List.Setup (mock => mock.GetAvailableViewsList()).Returns (dropDownList.Object);
      List.Setup (mock => mock.HasAvailableViewsList).Returns (true);
      List.Setup (mock => mock.AvailableViewsListTitle).Returns ("Views List Title");

      dropDownList.Setup (mock => mock.ClientID).Returns ("MockedDropDownListClientID");
      dropDownList.Setup (mock => mock.RenderControl (Html.Writer)).Callback (
          (HtmlTextWriter writer) => writer.Write ("mocked dropdown list"));

      var renderer = new BocListMenuBlockRenderer (_bocListCssClassDefinition);
      renderer.Render (CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);

      var span = Html.GetAssertedChildElement (div, "label", 0);
      Html.AssertAttribute (span, "for", "MockedDropDownListClientID");
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.AvailableViewsListLabel);
      Html.AssertTextNode (span, "Views List Title", 0);

      Html.AssertTextNode (div, HtmlHelper.WhiteSpace + "mocked dropdown list", 1);
    }

    [Test]
    public void RenderWithOptions ()
    {
      var optionsMenu = new Mock<IDropDownMenu>();
      StateBag bag = new StateBag();
      AttributeCollection attributes = new AttributeCollection (bag);
      optionsMenu.Setup (stub => stub.Style).Returns (attributes.CssStyle);
      optionsMenu.SetupProperty (_ => _.Visible);
      optionsMenu.Object.Visible = true;

      List.Setup (mock => mock.OptionsMenu).Returns (optionsMenu.Object);
      List.Setup (mock => mock.HasOptionsMenu).Returns (true);
      List.Setup (mock => mock.OptionsTitle).Returns ("Options Menu Title");

      optionsMenu.Setup (menuMock => menuMock.RenderControl (Html.Writer)).Callback (
          (HtmlTextWriter writer) => writer.Write ("mocked dropdown menu"));

      var renderer = new BocListMenuBlockRenderer (_bocListCssClassDefinition);
      renderer.Render (CreateRenderingContext());

      Assert.That (Html.GetDocumentText().StartsWith ("mocked dropdown menu"));
    }

    [Test]
    public void RenderWithListMenu ()
    {
      List.Setup (mock => mock.HasListMenu).Returns (true);
      List.Object.ListMenu.Visible = true;

      var renderer = new BocListMenuBlockRenderer (_bocListCssClassDefinition);
      renderer.Render (CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertChildElementCount (div, 0);
      Html.AssertAttribute (div, "class", "bocListListMenuContainer");
    }

    private BocListRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create (List.Object.DataSource, List.Object.Property, "Args");
      return new BocListRenderingContext (HttpContext, Html.Writer, List.Object, businessObjectWebServiceContext, new BocColumnRenderer[0]);
    }
  }
}