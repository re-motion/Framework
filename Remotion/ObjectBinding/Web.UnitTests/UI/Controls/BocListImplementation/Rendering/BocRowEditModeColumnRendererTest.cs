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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocRowEditModeColumnRendererTest : ColumnRendererTestBase<BocRowEditModeColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocRowEditModeColumnDefinition> _renderingContext;

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocRowEditModeColumnDefinition();
      Column.EditText = WebString.CreateFromText("Bearbeiten");
      Column.SaveText = WebString.CreateFromText("Speichern");
      Column.CancelText = WebString.CreateFromText("Abbrechen");
      Column.Show = BocRowEditColumnDefinitionShow.Always;

      base.SetUp();

      EventArgs = new BocListDataRowRenderEventArgs(EventArgs.ListIndex, EventArgs.BusinessObject, true, EventArgs.IsOddRow);

      List.Setup(mock => mock.EnableClientScript).Returns(true);
      List.Setup(mock => mock.IsReadOnly).Returns(false);
      List.Object.DataSource.Mode = DataSourceMode.Edit;

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocRowEditModeColumnDefinition>(
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
    public void RenderEditable ()
    {
      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      EventArgs = new BocListDataRowRenderEventArgs(EventArgs.ListIndex, EventArgs.BusinessObject, true, EventArgs.IsOddRow);

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell + " " + _bocListCssClassDefinition.DataCellEditModeButtons);

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var a = Html.GetAssertedChildElement(cellStructureDiv, "a", 0);
      Html.AssertAttribute(a, "id", List.Object.ClientID + "_Column_0_RowEditCommand_Edit_Row_10");
      Html.AssertAttribute(a, "href", "fakeFallbackUrl");
      Html.AssertAttribute(a, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");
      Html.AssertTextNode(a, "Bearbeiten", 0);
    }

    [Test]
    public void RenderEditTextWebString ()
    {
      Column.EditText = WebString.CreateFromText("Multiline\nEdit");
      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());

      EventArgs = new BocListDataRowRenderEventArgs(EventArgs.ListIndex, EventArgs.BusinessObject, true, EventArgs.IsOddRow);

      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var editTest = document.GetAssertedElementByID(List.Object.ClientID + "_Column_0_RowEditCommand_Edit_Row_10");
      Assert.That(editTest.InnerXml, Is.EqualTo("Multiline<br />Edit"));
    }

    [Test]
    public void RenderEditing ()
    {
      Mock.Get(List.Object.EditModeController).Setup(mock => mock.GetEditableRow(10)).Returns(new Mock<IEditableRow>().Object);

      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", _bocListCssClassDefinition.DataCell + " " + _bocListCssClassDefinition.DataCellEditModeButtons);

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var save = Html.GetAssertedChildElement(cellStructureDiv, "a", 0);
      Html.AssertAttribute(save, "id", List.Object.ClientID + "_Column_0_RowEditCommand_Save_Row_10");
      Html.AssertAttribute(save, "href", "fakeFallbackUrl");
      Html.AssertAttribute(save, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");
      Html.AssertTextNode(save, "Speichern", 0);

      var cancel = Html.GetAssertedChildElement(cellStructureDiv, "a", 1);
      Html.AssertAttribute(cancel, "id", List.Object.ClientID + "_Column_0_RowEditCommand_Cancel_Row_10");
      Html.AssertAttribute(cancel, "href", "fakeFallbackUrl");
      Html.AssertAttribute(cancel, "onclick", "postBackEventReference;BocList.OnCommandClick();return false;");
      Html.AssertTextNode(cancel, "Abbrechen", 0);
    }

    [Test]
    public void RenderSaveTextAndCancelTextWebString ()
    {
      Column.SaveText = WebString.CreateFromText("Multiline\nSave");
      Column.CancelText = WebString.CreateFromText("Multiline\nCancel");
      Mock.Get(List.Object.EditModeController).Setup(mock => mock.GetEditableRow(10)).Returns(new Mock<IEditableRow>().Object);

      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();
      var id = List.Object.ClientID;
      var saveText = document.GetAssertedElementByID(List.Object.ClientID + "_Column_0_RowEditCommand_Save_Row_10");
      Assert.That(saveText.InnerXml, Is.EqualTo("Multiline<br />Save"));
      var cancelText = document.GetAssertedElementByID(List.Object.ClientID + "_Column_0_RowEditCommand_Cancel_Row_10");
      Assert.That(cancelText.InnerXml, Is.EqualTo("Multiline<br />Cancel"));
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider());
      renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
    }
  }
}
