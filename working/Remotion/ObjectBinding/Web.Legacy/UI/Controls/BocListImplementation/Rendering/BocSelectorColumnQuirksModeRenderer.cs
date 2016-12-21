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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the cells containing the row selector controls.
  /// </summary>
  public class BocSelectorColumnQuirksModeRenderer : IBocSelectorColumnRenderer
  {
    private const string c_whiteSpace = "&nbsp;";

    private readonly BocListQuirksModeCssClassDefinition _cssClasses;

    public BocSelectorColumnQuirksModeRenderer (BocListQuirksModeCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
    }

    public BocListQuirksModeCssClassDefinition CssClasses
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

      string selectorControlID = renderingContext.Control.GetSelectorControlName ().Replace('$', '_') + "_" + rowRenderingContext.SortedIndex;
      var selectorControlName = renderingContext.Control.GetSelectorControlName ();
      var selectorControlValue = renderingContext.Control.GetSelectorControlValue (rowRenderingContext.Row);
      var isChecked = rowRenderingContext.IsSelected;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderSelectorControl (renderingContext, selectorControlID, selectorControlName, selectorControlValue, isChecked, false);
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.IsSelectionEnabled)
        return;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TitleCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Th);
      if (renderingContext.Control.Selection == RowSelection.Multiple)
      {
        string selectorControlName = renderingContext.Control.GetSelectAllControlName ();
        var selectorControlID = selectorControlName.Replace ('$', '_');
        RenderSelectorControl (renderingContext, selectorControlID, selectorControlName, null, false, true);
      }
      else
        renderingContext.Writer.Write (c_whiteSpace);
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderSelectorControl (BocListRenderingContext renderingContext, string id, string name, string value, bool isChecked, bool isSelectAllSelectorControl)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

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

      if (isSelectAllSelectorControl)
      {
        AddSelectAllSelectorAttributes (renderingContext);
      }
      else
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Value, value);
        AddRowSelectorAttributes (renderingContext);
      }

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Input);
      renderingContext.Writer.RenderEndTag ();
    }

    private void AddRowSelectorAttributes (BocListRenderingContext renderingContext)
    {
      string alternateText = renderingContext.Control.GetResourceManager ().GetString (BocList.ResourceIdentifier.SelectRowAlternateText);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      if (renderingContext.Control.HasClientScript)
      {
        const string script = "BocList_OnSelectionSelectorControlClick();";
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }

    private void AddSelectAllSelectorAttributes (BocListRenderingContext renderingContext)
    {
      string alternateText = renderingContext.Control.GetResourceManager().GetString (BocList.ResourceIdentifier.SelectAllRowsAlternateText);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Alt, alternateText);

      if (renderingContext.Control.HasClientScript)
      {
        string script = "BocList_OnSelectAllSelectorControlClick ("
                        + "document.getElementById ('" + renderingContext.Control.ClientID + "'), "
                        + "this , '"
                        + renderingContext.Control.GetSelectorControlName ().Replace ('$', '_') + "');";
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, script);
      }
    }
  }
}