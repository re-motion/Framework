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
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
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
      Column.Command = new BocListItemCommand (CommandType.Event);
      Column.Command.EventCommand = new Command.EventCommandInfo();
      Column.Command.EventCommand.RequiresSynchronousPostBack = true;
      Column.Text = "TestCommand";
      Column.ColumnTitle = "FirstColumn";
      Column.OwnerControl = List;

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      base.SetUp();

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create (null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocCommandColumnDefinition> (
          new BocColumnRenderingContext (HttpContext, Html.Writer, List, businessObjectWebServiceContext, Column, 0, 0));
    }

    [TearDown]
    public void TearDown ()
    {
      WebConfigurationMock.Current = new WebConfigurationMock();
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocCommandColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 5, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");

      var a = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (a, "id", List.ClientID + "_Column_0_Command_Row_10");
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");

      Html.AssertTextNode (a, "TestCommand", 0);
    }

    [Test]
    public void RenderIconCell ()
    {
      IBocColumnRenderer renderer = new BocCommandColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, true, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");

      var a = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");

      Html.AssertIcon (a, EventArgs.BusinessObject, null);

      Html.AssertTextNode (a, HtmlHelper.WhiteSpace + "TestCommand", 1);
    }

    [Test]
    public void RenderCommandIconCell ()
    {
      Column.Icon.Url = "~/Images/CommandIcon.gif";
      Column.Icon.Width = new Unit (16, UnitType.Pixel);
      Column.Icon.Height = new Unit (16, UnitType.Pixel);

      IBocColumnRenderer renderer = new BocCommandColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");

      var a = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");

      Html.AssertIcon (a, EventArgs.BusinessObject, Column.Icon.Url.TrimStart ('~'));

      Html.AssertTextNode (a, "TestCommand", 1);
    }

    [Test]
    public void RenderDisabledCommandForWaiConformanceLevelA ()
    {
      WebConfigurationMock.Current.Wcag.ConformanceLevel = WaiConformanceLevel.A;

      IBocColumnRenderer renderer = new BocCommandColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute (td, "role", "cell");

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      Html.AssertTextNode (span, "TestCommand", 0);
    }
  }
}