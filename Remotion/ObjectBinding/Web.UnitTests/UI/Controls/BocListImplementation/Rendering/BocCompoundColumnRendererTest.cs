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
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocCompoundColumnRendererTest : ColumnRendererTestBase<BocCompoundColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocCompoundColumnDefinition> _renderingContext;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCompoundColumnDefinition();
      Column.ColumnTitle = "TestColumn1";
      Column.ColumnTitle = "FirstColumn";
      Column.Command = null;
      Column.EnforceWidth = false;
      Column.FormatString = "{0}";

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      base.SetUp();

      Column.PropertyPathBindings.Add (new PropertyPathBinding ("DisplayName"));

      _renderingContext =
          new BocColumnRenderingContext<BocCompoundColumnDefinition> (new BocColumnRenderingContext (HttpContext, Html.Writer, List, Column, 0, 0));
    }

    [Test]
    public void RenderEmptyCell ()
    {
      Column.FormatString = string.Empty;

      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertTextNode (textWrapper, HtmlHelper.WhiteSpace, 0);
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertTextNode (textWrapper, "referencedObject1", 0);
    }

    [Test]
    public void RenderBasicCell_WithNewLineAndEncoding ()
    {
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

      var renderArgs = new BocListDataRowRenderEventArgs (0, (IBusinessObject) TypeWithReference.Create ("value\r\nExtraText<html>"), false, true);
      renderer.RenderDataCell (_renderingContext, 0, false, renderArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertTextNode (textWrapper, "value", 0);
      Html.GetAssertedChildElement (textWrapper, "br", 1);
      Html.AssertTextNode (textWrapper, "ExtraText<html>", 2); //This is actually encoded inside the asserted XmlDocument
    }

    [Test]
    public void RenderEnforcedWidthCell ()
    {
      Column.EnforceWidth = true;
      Column.Width = new Unit (40, UnitType.Pixel);

      IBocColumnRenderer renderer = new BocCompoundColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var cropSpan = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (cropSpan, "title", "referencedObject1");
      Html.AssertStyleAttribute (cropSpan, "width", "40px");
      Html.AssertStyleAttribute (cropSpan, "display", "block");
      Html.AssertStyleAttribute (cropSpan, "overflow", "hidden");
      Html.AssertStyleAttribute (cropSpan, "white-space", "nowrap");

      var span = Html.GetAssertedChildElement (cropSpan, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertTextNode (textWrapper, "referencedObject1", 0);
    }
  }
}