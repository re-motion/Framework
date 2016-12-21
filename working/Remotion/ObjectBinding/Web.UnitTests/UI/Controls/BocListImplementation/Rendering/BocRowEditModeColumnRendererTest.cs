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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

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
      Column.EditText = "Bearbeiten";
      Column.SaveText = "Speichern";
      Column.CancelText = "Abbrechen";
      Column.Show = BocRowEditColumnDefinitionShow.Always;

      base.SetUp();

      EventArgs = new BocListDataRowRenderEventArgs (EventArgs.ListIndex, EventArgs.BusinessObject, true, EventArgs.IsOddRow);

      List.Stub (mock => mock.EnableClientScript).Return (true);
      List.Stub (mock => mock.IsDesignMode).Return (false);
      List.Stub (mock => mock.IsReadOnly).Return (false);
      List.DataSource.Mode = DataSourceMode.Edit;

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      _renderingContext =
          new BocColumnRenderingContext<BocRowEditModeColumnDefinition> (new BocColumnRenderingContext (HttpContext, Html.Writer, List, Column, 0, 0));
    }

    [Test]
    public void RenderEditable ()
    {
      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer (
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition);

      EventArgs = new BocListDataRowRenderEventArgs (EventArgs.ListIndex, EventArgs.BusinessObject, true, EventArgs.IsOddRow);

      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var a = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (a, "id", List.ClientID + "_Column_0_RowEditCommand_Edit_Row_10");
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "postBackEventReference;BocList_OnCommandClick();");
      Html.AssertTextNode (a, "Bearbeiten", 0);
    }

    [Test]
    public void RenderEditing ()
    {
      List.EditModeController.Stub (mock => mock.GetEditableRow (10)).Return (MockRepository.GenerateStub<IEditableRow>());

      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer (
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, "class", _bocListCssClassDefinition.DataCell);

      var save = Html.GetAssertedChildElement (td, "a", 0);
      Html.AssertAttribute (save, "id", List.ClientID + "_Column_0_RowEditCommand_Save_Row_10");
      Html.AssertAttribute (save, "href", "#");
      Html.AssertAttribute (save, "onclick", "postBackEventReference;BocList_OnCommandClick();");
      Html.AssertTextNode (save, "Speichern", 0);

      var cancel = Html.GetAssertedChildElement (td, "a", 1);
      Html.AssertAttribute (cancel, "id", List.ClientID + "_Column_0_RowEditCommand_Cancel_Row_10");
      Html.AssertAttribute (cancel, "href", "#");
      Html.AssertAttribute (cancel, "onclick", "postBackEventReference;BocList_OnCommandClick();");
      Html.AssertTextNode (cancel, "Abbrechen", 0);
    }

    [Test]
    public void TestDiagnosticMetadataRendering ()
    {
      IBocColumnRenderer renderer = new BocRowEditModeColumnRenderer (
          new FakeResourceUrlFactory(),
          RenderingFeatures.WithDiagnosticMetadata,
          _bocListCssClassDefinition);
      renderer.RenderDataCell (_renderingContext, 0, false, EventArgs);

      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement (document, "td", 0);
      Html.AssertAttribute (td, DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
    }
  }
}