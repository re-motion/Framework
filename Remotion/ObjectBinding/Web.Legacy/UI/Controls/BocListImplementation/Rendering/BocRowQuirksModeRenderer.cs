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
using System.Linq;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering single data rows or the title row of a specific <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocRowQuirksModeRenderer : IBocRowRenderer
  {
    /// <summary>Text displayed when control is displayed in desinger and is read-only has no contents.</summary>
    public const string DesignModeDummyColumnTitle = "Column Title {0}";

    /// <summary>Number of columns to show in design mode before actual columns have been defined.</summary>
    public const int DesignModeDummyColumnCount = 3;

    private readonly BocListQuirksModeCssClassDefinition _cssClasses;
    private readonly IBocIndexColumnRenderer _indexColumnRenderer;
    private readonly IBocSelectorColumnRenderer _selectorColumnRenderer;


    public BocRowQuirksModeRenderer (
        BocListQuirksModeCssClassDefinition cssClasses, IBocIndexColumnRenderer indexColumnRenderer, IBocSelectorColumnRenderer selectorColumnRenderer)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("indexColumnRenderer", indexColumnRenderer);
      ArgumentUtility.CheckNotNull ("selectorColumnRenderer", selectorColumnRenderer);

      _cssClasses = cssClasses;
      _indexColumnRenderer = indexColumnRenderer;
      _selectorColumnRenderer = selectorColumnRenderer;
    }

    public BocListQuirksModeCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderTitlesRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      _indexColumnRenderer.RenderTitleCell (renderingContext);
      _selectorColumnRenderer.RenderTitleCell (renderingContext);

      foreach (var renderer in renderingContext.ColumnRenderers)
        renderer.RenderTitleCell (renderingContext);
      
      if (ControlHelper.IsDesignMode (renderingContext.Control) && renderingContext.ColumnRenderers.Length == 0)
      {
        for (int i = 0; i < DesignModeDummyColumnCount; i++)
        {
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
          renderingContext.Writer.Write (String.Format (DesignModeDummyColumnTitle, i + 1));
          renderingContext.Writer.RenderEndTag();
        }
      }

      renderingContext.Writer.RenderEndTag();
    }

    public void RenderEmptyListDataRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      BocColumnRenderer[] columnRenderers = renderingContext.ColumnRenderers;
      int columnCount = 0;

      if (renderingContext.Control.IsIndexEnabled)
        columnCount++;

      if (renderingContext.Control.IsSelectionEnabled)
        columnCount++;

      columnCount += columnRenderers.Count (t => t.IsVisibleColumn);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Colspan, columnCount.ToString());
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      string emptyListMessage;
      if (string.IsNullOrEmpty (renderingContext.Control.EmptyListMessage))
        emptyListMessage = renderingContext.Control.GetResourceManager().GetString (BocList.ResourceIdentifier.EmptyListMessage);
      else
        emptyListMessage = renderingContext.Control.EmptyListMessage;
      // Do not HTML encode
      renderingContext.Writer.Write (emptyListMessage);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderDataRow (BocListRenderingContext renderingContext, BocListRowRenderingContext rowRenderingContext, int rowIndex)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("rowRenderingContext", rowRenderingContext);

      var absoluteRowIndex = rowRenderingContext.SortedIndex;
      var originalRowIndex = rowRenderingContext.Row.Index;
      var businessObject = rowRenderingContext.Row.BusinessObject;

      bool isChecked = rowRenderingContext.IsSelected;
      bool isOddRow = (rowIndex % 2 == 0); // row index is zero-based here, but one-based in rendering => invert even/odd

      string cssClassTableRow = GetCssClassTableRow (renderingContext, isChecked);
      string cssClassTableCell = CssClasses.GetDataCell (isOddRow);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      _indexColumnRenderer.RenderDataCell (renderingContext, originalRowIndex, absoluteRowIndex, cssClassTableCell);
      _selectorColumnRenderer.RenderDataCell (renderingContext, rowRenderingContext, cssClassTableCell);

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (originalRowIndex, businessObject, true, isOddRow);
      renderingContext.Control.OnDataRowRendering (dataRowRenderEventArgs);

      foreach (BocColumnRenderer renderer in renderingContext.ColumnRenderers)
        renderer.RenderDataCell (renderingContext, rowIndex, dataRowRenderEventArgs);

      renderingContext.Writer.RenderEndTag();
    }

    private string GetCssClassTableRow (BocListRenderingContext renderingContext, bool isChecked)
    {
      string cssClassTableRow;
      if (isChecked && renderingContext.Control.AreDataRowsClickSensitive())
        cssClassTableRow = CssClasses.DataRowSelected;
      else
        cssClassTableRow = CssClasses.DataRow;
      return cssClassTableRow;
    }
  }
}