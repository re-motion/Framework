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
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Rhino.Mocks;

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
      DropDownList dropDownList = MockRepository.GenerateMock<DropDownList>();
      List.Stub (mock => mock.GetAvailableViewsList()).Return (dropDownList);
      List.Stub (mock => mock.HasAvailableViewsList).Return (true);
      List.Stub (mock => mock.AvailableViewsListTitle).Return ("Views List Title");

      dropDownList.Stub (mock => mock.ClientID).Return ("MockedDropDownListClientID");
      dropDownList.Stub (mock => mock.RenderControl (Html.Writer)).WhenCalled (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write ("mocked dropdown list"));

      var renderer = new BocListMenuBlockRenderer (_bocListCssClassDefinition);
      renderer.Render (CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "margin-bottom", "5pt");

      var span = Html.GetAssertedChildElement (div, "label", 0);
      Html.AssertAttribute (span, "for", "MockedDropDownListClientID");
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.AvailableViewsListLabel);
      Html.AssertTextNode (span, "Views List Title", 0);

      Html.AssertTextNode (div, HtmlHelper.WhiteSpace + "mocked dropdown list", 1);
    }

    [Test]
    public void RenderWithOptions ()
    {
      IDropDownMenu optionsMenu = MockRepository.GenerateStub<IDropDownMenu> ();
      StateBag bag = new StateBag();
      AttributeCollection attributes = new AttributeCollection (bag);
      optionsMenu.Stub (stub => stub.Style).Return (attributes.CssStyle);
      optionsMenu.Visible = true;

      List.Stub (mock => mock.OptionsMenu).Return (optionsMenu);
      List.Stub (mock => mock.HasOptionsMenu).Return (true);
      List.Stub (mock => mock.OptionsTitle).Return ("Options Menu Title");
      List.Stub (mock => mock.MenuBlockItemOffset).Return (new Unit (7, UnitType.Pixel));

      optionsMenu.Stub (menuMock => menuMock.RenderControl (Html.Writer)).WhenCalled (
          invocation => ((HtmlTextWriter) invocation.Arguments[0]).Write ("mocked dropdown menu"));

      var renderer = new BocListMenuBlockRenderer (_bocListCssClassDefinition);
      renderer.Render (CreateRenderingContext());

      Assert.That (Html.GetDocumentText().StartsWith ("mocked dropdown menu"));
    }

    [Test]
    public void RenderWithListMenu ()
    {
      List.Stub (mock => mock.HasListMenu).Return (true);
      List.ListMenu.Visible = true;

      Unit menuBlockOffset = new Unit (3, UnitType.Pixel);
      List.Stub (mock => mock.MenuBlockItemOffset).Return (menuBlockOffset);

      var renderer = new BocListMenuBlockRenderer (_bocListCssClassDefinition);
      renderer.Render (CreateRenderingContext());

      var document = Html.GetResultDocument();

      var div = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertStyleAttribute (div, "width", "100%");
      Html.AssertStyleAttribute (div, "margin-bottom", menuBlockOffset.ToString());
      Html.AssertChildElementCount (div, 0);
    }

    private BocListRenderingContext CreateRenderingContext ()
    {
      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create (List.DataSource, List.Property, "Args");
      return new BocListRenderingContext (HttpContext, Html.Writer, List, businessObjectWebServiceContext, new BocColumnRenderer[0]);
    }
  }
}