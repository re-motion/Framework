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
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocSimpleColumnRendererTest : ColumnRendererTestBase<BocSimpleColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocSimpleColumnDefinition> _renderingContext;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocSimpleColumnDefinition();
      Column.Command = null;
      Column.IsDynamic = false;
      Column.IsReadOnly = false;
      Column.ColumnTitle = "FirstColumn";
      Column.PropertyPathIdentifier = "DisplayName";
      Column.FormatString = "unusedWithReferenceValue";
      Column.OwnerControl = List;

      base.SetUp();

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      _renderingContext =
          new BocColumnRenderingContext<BocSimpleColumnDefinition> (new BocColumnRenderingContext (HttpContext, Html.Writer, List, Column, 0, 0));
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

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
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

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
    public void RenderCommandCell ()
    {
      Column.Command = new BocListItemCommand (CommandType.Href);
      Column.Command.HrefCommand.Href = "url";

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

      renderer.RenderDataCell (_renderingContext, 5, false, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var a = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (a, "id", List.ClientID + "_Column_0_Command_Row_10");
      Html.AssertAttribute (a, "href", "url");
      Html.AssertAttribute (a, "onclick", "BocList_OnCommandClick();");

      var span = Html.GetAssertedChildElement (a, "span", 0);
      Html.AssertTextNode (span, "referencedObject1", 0);
    }

    [Test]
    public void RenderIconCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);

      renderer.RenderDataCell (_renderingContext, 0, true, EventArgs);
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      Html.AssertIcon (span, EventArgs.BusinessObject, null);
      Html.AssertTextNode (span, HtmlHelper.WhiteSpace, 1);

      var textWrapper = Html.GetAssertedChildElement (span, "span", 2);
      Html.AssertTextNode (textWrapper, BusinessObject.GetPropertyString ("FirstValue"), 0);
    }

    [Test]
    public void RenderEditModeControl ()
    {
      var firstObject = (IBusinessObject) ((TypeWithReference) BusinessObject).FirstValue;

      IEditableRow editableRow = MockRepository.GenerateMock<IEditableRow>();
      editableRow.Stub (mock => mock.HasEditControl (0)).IgnoreArguments().Return (true);
      editableRow.Stub (mock => mock.GetEditControl (0)).IgnoreArguments().Return (MockRepository.GenerateStub<IBocTextValue>());
      editableRow.Expect (
          mock => mock.RenderSimpleColumnCellEditModeControl (
              Html.Writer,
              Column,
              firstObject,
              0));

      List.EditModeController.Stub (mock => mock.GetEditableRow (EventArgs.ListIndex)).Return (editableRow);

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), RenderingFeatures.Default, _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var span = Html.GetAssertedChildElement (td, "span", 0);
      Html.AssertAttribute (span, "class", _bocListCssClassDefinition.Content);

      var clickSpan = Html.GetAssertedChildElement (span, "span", 0);
      Html.AssertAttribute (clickSpan, "onclick", "BocList_OnCommandClick();");

      editableRow.AssertWasCalled (
          mock => mock.RenderSimpleColumnCellEditModeControl (
              Html.Writer,
              Column,
              firstObject,
              0));
    }
  }
}