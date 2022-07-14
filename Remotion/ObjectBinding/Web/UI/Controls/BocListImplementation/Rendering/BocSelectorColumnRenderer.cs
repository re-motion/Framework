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
using System.Linq;
using System.Web.UI;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the cells containing the row selector controls.
  /// </summary>
  [ImplementationFor(typeof(IBocSelectorColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocSelectorColumnRenderer : IBocSelectorColumnRenderer
  {
    private const string c_whiteSpace = "&nbsp;";

    private readonly IRenderingFeatures _renderingFeatures;
    private readonly BocListCssClassDefinition _cssClasses;
    private readonly ILabelReferenceRenderer _labelReferenceRenderer;

    public BocSelectorColumnRenderer (IRenderingFeatures renderingFeatures, BocListCssClassDefinition cssClasses, ILabelReferenceRenderer labelReferenceRenderer)
    {
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);

      _renderingFeatures = renderingFeatures;
      _cssClasses = cssClasses;
      _labelReferenceRenderer = labelReferenceRenderer;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (BocListRenderingContext renderingContext, BocListRowRenderingContext rowRenderingContext, string[] headerIDs)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("rowRenderingContext", rowRenderingContext);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      string selectorControlID = renderingContext.Control.GetSelectorControlName().Replace('$', '_') + "_" + rowRenderingContext.Row.Index;
      var cssClass = CssClasses.DataCell + " " + CssClasses.Themed + " " + CssClasses.DataCellSelector;
      var selectorControlName = renderingContext.Control.GetSelectorControlName();
      var selectorControlValue = renderingContext.Control.GetSelectorControlValue(rowRenderingContext.Row);
      var isChecked = rowRenderingContext.IsSelected;
      var isFirstColumn = !renderingContext.Control.IsIndexEnabled;
      // JAWS 2021 with Edge 102 and NVDA 2021 with Firefox 101 do not announce the row-header for the first column.
      var columnHeaderCount = 1;
      var headerIDsForSelectorControl = isFirstColumn ? headerIDs.Take(headerIDs.Length - columnHeaderCount) : Array.Empty<string>();

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
#pragma warning disable CS0618 // Type or member is obsolete
      var ariaRoleForTableDataElement = GetAriaRoleForTableDataElement();
#pragma warning restore CS0618 // Type or member is obsolete
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, ariaRoleForTableDataElement);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataListCellIndex(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);
      RenderDataRowSelectorControl(renderingContext, selectorControlID, selectorControlName, selectorControlValue, isChecked, headerIDsForSelectorControl);
      renderingContext.Writer.RenderEndTag();
    }

    [Obsolete("RM-7053: Only intended for ARIA-role workaround. May be removed in future releases without warning once there is infrastructure option for specifying the table type.")]
    protected virtual string GetAriaRoleForTableDataElement ()
    {
      return HtmlRoleAttributeValue.Cell;
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext, string cellID)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty("cellID", cellID);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      var cssClass = CssClasses.TitleCell + " " + CssClasses.Themed + " " + CssClasses.TitleCellSelector;

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, cellID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.ColumnHeader);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataListCellIndex(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Th);
      if (renderingContext.Control.Selection == RowSelection.Multiple)
      {
        string selectorControlName = renderingContext.Control.GetSelectAllControlName();
        RenderTitleRowSelectorControl(renderingContext, selectorControlName);
      }
      else
        renderingContext.Writer.Write(c_whiteSpace);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderTitleRowSelectorControl (BocListRenderingContext renderingContext, string name)
    {
      var labelText = renderingContext.Control.GetResourceManager().GetText(BocList.ResourceIdentifier.SelectAllRowsLabelText);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Name, name);

      if (renderingContext.Control.EditModeController.IsRowEditModeActive)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

      labelText.AddAttributeTo(renderingContext.Writer, HtmlTextWriterAttribute.Title);

      if (renderingContext.Control.HasClientScript)
      {
        var isRowHighlightingEnabled = renderingContext.Control.AreDataRowsClickSensitive();
        string script = "BocList.OnSelectAllSelectorControlClick ("
                        + "document.getElementById ('" + renderingContext.Control.ClientID + "'), "
                        + "this,"
                        + (isRowHighlightingEnabled ? "true" : "false")
                        + ");";
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, script);
      }

      if (_renderingFeatures.EnableDiagnosticMetadata)
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownSelectAllControl, "true");

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag(); // Input-checkbox
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderDataRowSelectorControl (BocListRenderingContext renderingContext, string id, string name, string value, bool isChecked, IEnumerable<string> headerIDs)
    {
      var labelText = renderingContext.Control.GetResourceManager().GetText(BocList.ResourceIdentifier.SelectRowLabelText);
      var labelID = id + "_Label";

      if (renderingContext.Control.Selection == RowSelection.SingleRadioButton)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
      else
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, id);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Name, name);

      if (isChecked)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
      if (renderingContext.Control.EditModeController.IsRowEditModeActive)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Value, value);

      var labelIDs = headerIDs.Concat(labelID).ToArray();
      _labelReferenceRenderer.AddLabelsReference(renderingContext.Writer, labelIDs, Array.Empty<string>());

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, labelID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Hidden, HtmlHiddenAttributeValue.Hidden);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      labelText.WriteTo(renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
    }

    private static void AddDiagnosticMetadataListCellIndex (BocListRenderingContext renderingContext)
    {
      var oneBasedCellIndex = renderingContext.Control.IsIndexEnabled ? 2 : 1;
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());
    }
  }
}
