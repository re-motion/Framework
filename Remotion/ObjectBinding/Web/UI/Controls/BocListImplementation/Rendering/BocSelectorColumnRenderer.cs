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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the cells containing the row selector controls.
  /// </summary>
  [ImplementationFor (typeof (IBocSelectorColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocSelectorColumnRenderer : IBocSelectorColumnRenderer
  {
    private const string c_whiteSpace = "&nbsp;";

    private readonly IRenderingFeatures _renderingFeatures;
    private readonly BocListCssClassDefinition _cssClasses;

    public BocSelectorColumnRenderer (IRenderingFeatures renderingFeatures, BocListCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("renderingFeatures", renderingFeatures);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      
      _renderingFeatures = renderingFeatures;
      _cssClasses = cssClasses;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (BocListRenderingContext renderingContext, BocListRowRenderingContext rowRenderingContext, string cssClassTableCell)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("cssClassTableCell", cssClassTableCell);
      ArgumentUtility.CheckNotNullOrEmpty ("cssClassTableCell", cssClassTableCell);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      string selectorControlID = renderingContext.Control.GetSelectorControlName().Replace ('$', '_') + "_" + rowRenderingContext.Row.Index;
      var cssClass = cssClassTableCell + " " + CssClasses.Themed + " " + CssClasses.DataCellSelector;
      var selectorControlName = renderingContext.Control.GetSelectorControlName();
      var selectorControlValue = renderingContext.Control.GetSelectorControlValue (rowRenderingContext.Row);
      var isChecked = rowRenderingContext.IsSelected;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
#pragma warning disable CS0618 // Type or member is obsolete
      var ariaRoleForTableDataElement = GetAriaRoleForTableDataElement();
#pragma warning restore CS0618 // Type or member is obsolete
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute2.Role, ariaRoleForTableDataElement);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataListCellIndex (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderDataRowSelectorControl (renderingContext, selectorControlID, selectorControlName, selectorControlValue, isChecked);
      renderingContext.Writer.RenderEndTag();
    }

    [Obsolete ("RM-7053: Only intended for ARIA-role workaround. May be removed in future releases without warning once there is infrastructure option for specifying the table type.")]
    protected virtual string GetAriaRoleForTableDataElement ()
    {
      return HtmlRoleAttributeValue.Cell;
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      var cssClass = CssClasses.TitleCell + " " + CssClasses.Themed + " " + CssClasses.TitleCellSelector;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.ColumnHeader);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataListCellIndex (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (renderingContext.Control.Selection == RowSelection.Multiple)
      {
        string selectorControlName = renderingContext.Control.GetSelectAllControlName();
        RenderTitleRowSelectorControl (renderingContext, selectorControlName);
      }
      else
        renderingContext.Writer.Write (c_whiteSpace);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderTitleRowSelectorControl (BocListRenderingContext renderingContext, string name)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      string labelText = renderingContext.Control.GetResourceManager().GetString (BocList.ResourceIdentifier.SelectAllRowsLabelText);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Name, name);

      if (renderingContext.Control.EditModeController.IsRowEditModeActive)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Title, labelText);

      if (renderingContext.Control.HasClientScript)
      {
        var isRowHighlightingEnabled = renderingContext.Control.AreDataRowsClickSensitive();
        string script = "BocList.OnSelectAllSelectorControlClick ("
                        + "document.getElementById ('" + renderingContext.Control.ClientID + "'), "
                        + "this,"
                        + (isRowHighlightingEnabled ? "true" : "false")
                        + ");";
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }

      if (_renderingFeatures.EnableDiagnosticMetadata)
        renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownSelectAllControl, "true");

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag(); // Input-checkbox
    }

    private void RenderDataRowSelectorControl (BocListRenderingContext renderingContext, string id, string name, string value, bool isChecked)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      string labelText = renderingContext.Control.GetResourceManager().GetString (BocList.ResourceIdentifier.SelectRowLabelText);

      if (renderingContext.Control.Selection == RowSelection.SingleRadioButton)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Type, "radio");
      else
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Type, "checkbox");

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, id);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Name, name);

      if (isChecked)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Checked, "checked");
      if (renderingContext.Control.EditModeController.IsRowEditModeActive)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Disabled, "disabled");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Value, value);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Title, labelText);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag();
    }

    private static void AddDiagnosticMetadataListCellIndex (BocListRenderingContext renderingContext)
    {
      var oneBasedCellIndex = renderingContext.Control.IsIndexEnabled ? 2 : 1;
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());
    }
  }
}