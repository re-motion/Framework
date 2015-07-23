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
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering single data rows or the title row of a specific <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  [ImplementationFor (typeof (IBocRowRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocRowRenderer : IBocRowRenderer
  {
    /// <summary>Text displayed when control is displayed in desinger and is read-only has no contents.</summary>
    public const string DesignModeDummyColumnTitle = "Column Title {0}";

    /// <summary>Number of columns to show in design mode before actual columns have been defined.</summary>
    public const int DesignModeDummyColumnCount = 3;

    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IBocIndexColumnRenderer _indexColumnRenderer;
    private readonly IBocSelectorColumnRenderer _selectorColumnRenderer;
    private readonly IRenderingFeatures _renderingFeatures;

    public BocRowRenderer (
        BocListCssClassDefinition cssClasses,
        IBocIndexColumnRenderer indexColumnRenderer,
        IBocSelectorColumnRenderer selectorColumnRenderer,
        IRenderingFeatures renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
      _indexColumnRenderer = indexColumnRenderer;
      _selectorColumnRenderer = selectorColumnRenderer;
      _renderingFeatures = renderingFeatures;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderTitlesRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderTitleCell (renderingContext);
      GetSelectorColumnRenderer().RenderTitleCell (renderingContext);

      foreach (var columnRenderer in renderingContext.ColumnRenderers)
        columnRenderer.RenderTitleCell (renderingContext);

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

      for (int idxColumns = 0; idxColumns < columnRenderers.Length; idxColumns++)
      {
        BocColumnRenderer columnRenderer = columnRenderers[idxColumns];
        if (columnRenderer.IsVisibleColumn)
          columnCount++;
      }

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

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs (
          originalRowIndex,
          businessObject,
          true,
          isOddRow);
      renderingContext.Control.OnDataRowRendering (dataRowRenderEventArgs);

      string cssClassTableRow = GetCssClassTableRow (renderingContext, isChecked, dataRowRenderEventArgs);
      string cssClassTableCell = CssClasses.DataCell;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableRow);
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        // Note: business objects without identity can already be selected via row index.
        var businessObjectWithIdentity = rowRenderingContext.Row.BusinessObject as IBusinessObjectWithIdentity;
        if (businessObjectWithIdentity != null)
          renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.ItemID, businessObjectWithIdentity.UniqueIdentifier);

        var oneBasedRowIndex = rowIndex + 1;
        renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex, oneBasedRowIndex.ToString());
      }
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderDataCell (renderingContext, originalRowIndex, absoluteRowIndex, cssClassTableCell);
      GetSelectorColumnRenderer().RenderDataCell (renderingContext, rowRenderingContext, cssClassTableCell);

      RenderDataCells (renderingContext, rowIndex, dataRowRenderEventArgs);

      renderingContext.Writer.RenderEndTag();
    }

    private IBocSelectorColumnRenderer GetSelectorColumnRenderer ()
    {
      return _selectorColumnRenderer;
    }

    private IBocIndexColumnRenderer GetIndexColumnRenderer ()
    {
      return _indexColumnRenderer;
    }

    private void RenderDataCells (BocListRenderingContext renderingContext, int rowIndex, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      foreach (BocColumnRenderer columnRenderer in renderingContext.ColumnRenderers)
        columnRenderer.RenderDataCell (renderingContext, rowIndex, dataRowRenderEventArgs);
    }

    private string GetCssClassTableRow (
        BocListRenderingContext renderingContext,
        bool isChecked,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      string cssClassTableRow = CssClasses.DataRow;

      if (dataRowRenderEventArgs.IsOddRow)
        cssClassTableRow += " " + CssClasses.DataRowOdd;
      else
        cssClassTableRow += " " + CssClasses.DataRowEven;

      if (!string.IsNullOrEmpty (dataRowRenderEventArgs.AdditionalCssClassForDataRow))
        cssClassTableRow += " " + dataRowRenderEventArgs.AdditionalCssClassForDataRow;

      if (isChecked && renderingContext.Control.AreDataRowsClickSensitive())
        cssClassTableRow += " " + CssClasses.DataRowSelected;

      return cssClassTableRow;
    }
  }
}