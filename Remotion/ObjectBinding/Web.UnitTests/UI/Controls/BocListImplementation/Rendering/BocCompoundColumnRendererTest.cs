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
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
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
      Column.ColumnTitle = WebString.CreateFromText("FirstColumn");
      Column.Command = null;
      Column.EnforceWidth = false;
      Column.FormatString = "{0}";

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      base.SetUp();

      Column.PropertyPathBindings.Add(new PropertyPathBinding("DisplayName"));

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocCompoundColumnDefinition>(
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
    public void RenderEmptyCell ()
    {
      Column.FormatString = string.Empty;

      IBocColumnRenderer renderer = new BocCompoundColumnRenderer(
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

      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertTextNode(textWrapper, HtmlHelper.WhiteSpace, 0);
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer(
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

      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertTextNode(textWrapper, "referencedObject1", 0);
    }

    [Test]
    public void RenderCell_WithEmptyDisplayName ()
    {
      var businessObject = TypeWithReference.Create("");
      EventArgs = new BocListDataRowRenderEventArgs(10, (IBusinessObject)businessObject, false, true);
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertAttribute(textWrapper, DiagnosticMetadataAttributesForObjectBinding.BocListCellContents, string.Empty);
    }

    [Test]
    public void RenderCell_WithNullAsContent ()
    {
      var businessObject = TypeWithReference.Create();
      EventArgs = new BocListDataRowRenderEventArgs(10, (IBusinessObject)businessObject, false, true);
      Column.PropertyPathBindings.Clear();
      Column.PropertyPathBindings.Add(new PropertyPathBinding("ReferenceValue"));
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      Assert.That(businessObject.ReferenceValue, Is.Null);
      var document = Html.GetResultDocument();
      var td = Html.GetAssertedChildElement(document, "td", 0);
      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertAttribute(textWrapper, DiagnosticMetadataAttributesForObjectBinding.BocListCellContents, string.Empty);
    }

    [Test]
    public void RenderBasicCell_WithNewLineAndEncoding ()
    {
      IBocColumnRenderer renderer = new BocCompoundColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      var renderArgs = new BocListDataRowRenderEventArgs(0, (IBusinessObject)TypeWithReference.Create("value\r\nExtraText<html>"), false, true);
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments(renderArgs));
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell);
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertTextNode(textWrapper, "value", 0);
      Html.GetAssertedChildElement(textWrapper, "br", 1);
      Html.AssertTextNode(textWrapper, "ExtraText<html>", 2); //This is actually encoded inside the asserted XmlDocument
    }

    [Test]
    public void RenderEnforcedWidthCell ()
    {
      Column.EnforceWidth = true;
      Column.Width = new Unit(40, UnitType.Pixel);

      IBocColumnRenderer renderer = new BocCompoundColumnRenderer(
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

      var cropDiv = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(cropDiv, "class", _bocListCssClassDefinition.CellStructureElement);
      Html.AssertAttribute(cropDiv, "title", "referencedObject1");
      Html.AssertStyleAttribute(cropDiv, "width", "40px");
      Html.AssertStyleAttribute(cropDiv, "display", "block");
      Html.AssertStyleAttribute(cropDiv, "overflow", "hidden");
      Html.AssertStyleAttribute(cropDiv, "white-space", "nowrap");

      var div = Html.GetAssertedChildElement(cropDiv, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);

      var textWrapper = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertTextNode(textWrapper, "referencedObject1", 0);
    }
  }
}
