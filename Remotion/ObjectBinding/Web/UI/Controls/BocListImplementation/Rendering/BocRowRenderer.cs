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
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering single data rows or the title row of a specific <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  [ImplementationFor(typeof(IBocRowRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocRowRenderer : IBocRowRenderer
  {
    internal static string GetCellIDForValidationMarker (IBocList bocList, int rowIndex, int visibleColumnIndex)
    {
      ArgumentUtility.CheckNotNull(nameof(bocList), bocList);

      return $"{bocList.ClientID}_C{visibleColumnIndex}_R{rowIndex}_ValidationMarker";
    }

    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IBocIndexColumnRenderer _indexColumnRenderer;
    private readonly IBocSelectorColumnRenderer _selectorColumnRenderer;
    private readonly IRenderingFeatures _renderingFeatures;
    private readonly IBocListValidationSummaryRenderer _validationSummaryRenderer;

    public BocRowRenderer (
        BocListCssClassDefinition cssClasses,
        IBocIndexColumnRenderer indexColumnRenderer,
        IBocSelectorColumnRenderer selectorColumnRenderer,
        IRenderingFeatures renderingFeatures,
        IBocListValidationSummaryRenderer validationSummaryRenderer)
    {
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull("validationSummaryRenderer", validationSummaryRenderer);

      _cssClasses = cssClasses;
      _indexColumnRenderer = indexColumnRenderer;
      _selectorColumnRenderer = selectorColumnRenderer;
      _renderingFeatures = renderingFeatures;
      _validationSummaryRenderer = validationSummaryRenderer;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderTitlesRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Row);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tr);

      GetIndexColumnRenderer().RenderTitleCell(renderingContext, cellID: GetTitleCellIDForIndexColumn(renderingContext));
      GetSelectorColumnRenderer().RenderTitleCell(renderingContext, cellID: GetTitleCellIDForSelectorColumn(renderingContext));

      foreach (var columnRenderer in renderingContext.ColumnRenderers)
      {
        var titleCellID = GetTitleCellID(renderingContext, columnRenderer.VisibleColumnIndex);
        columnRenderer.RenderTitleCell(renderingContext, cellID: titleCellID);
      }

      renderingContext.Writer.RenderEndTag();
    }

    public void RenderEmptyListDataRow (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

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

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Row);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Colspan, columnCount.ToString());
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);

      WebString emptyListMessage;
      if (renderingContext.Control.EmptyListMessage.IsEmpty)
        emptyListMessage = renderingContext.Control.GetResourceManager().GetText(BocList.ResourceIdentifier.EmptyListMessage);
      else
        emptyListMessage = renderingContext.Control.EmptyListMessage;

      emptyListMessage.WriteTo(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    public void RenderDataRow (BocListRenderingContext renderingContext, BocListRowRenderingContext rowRenderingContext, in BocRowRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("rowRenderingContext", rowRenderingContext);

      var absoluteRowIndex = rowRenderingContext.SortedIndex;
      var originalRowIndex = rowRenderingContext.Row.Index;
      var businessObject = rowRenderingContext.Row.BusinessObject;

      var rowIndex = arguments.RowIndex;

      bool isChecked = rowRenderingContext.IsSelected;
      bool isOddRow = (rowIndex % 2 == 0); // row index is zero-based here, but one-based in rendering => invert even/odd

      var dataRowRenderEventArgs = new BocListDataRowRenderEventArgs(
          originalRowIndex,
          businessObject,
          true,
          isOddRow);
      renderingContext.Control.OnDataRowRendering(dataRowRenderEventArgs);

      var hasValidationFailures = renderingContext.Control.ValidationFailureRepository.HasValidationFailuresForDataRow(rowRenderingContext.Row.BusinessObject);

      string cssClassTableRow = GetCssClassTableRow(renderingContext, isChecked, hasValidationFailures, dataRowRenderEventArgs);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClassTableRow);
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        // Note: business objects without identity can already be selected via row index.
        var businessObjectWithIdentity = rowRenderingContext.Row.BusinessObject as IBusinessObjectWithIdentity;
        if (businessObjectWithIdentity != null)
          renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.ItemID, businessObjectWithIdentity.UniqueIdentifier);

        var oneBasedRowIndex = rowIndex + 1;
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListRowIndex, oneBasedRowIndex.ToString());
      }
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Row);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tr);

      var dataCellIDs = renderingContext.ColumnRenderers
          .Where(r => r.IsRowHeader)
          .Select(r => GetDataCellID(renderingContext, columnIndex: r.VisibleColumnIndex, rowIndex: rowIndex));
      // ReSharper disable once PossibleMultipleEnumeration
      var dataCellIDsForIndexColumn = dataCellIDs.Concat(GetTitleCellIDForIndexColumn(renderingContext)).ToArray();
      // ReSharper disable once PossibleMultipleEnumeration
      var dataCellIDsForSelectorColumn = dataCellIDs.ToArray(); // Selector column intentionally does not have a header column.

      // Note: The cells preceding the selector-control will also act as selector, allowing adding/removing of the selection.
      // This behavior extends the original behavior, where clicking the selector-control or the associated label in the index-cell 
      // changed the selection state. This improves usability as the user does not have to precisely hit the text or the checkbox/radio button.
      // If this is changed, an update to the Javascript code will be required.
      GetIndexColumnRenderer().RenderDataCell(renderingContext, originalRowIndex: originalRowIndex, absoluteRowIndex: absoluteRowIndex, headerIDs: dataCellIDsForIndexColumn);
      GetSelectorColumnRenderer().RenderDataCell(renderingContext, rowRenderingContext, headerIDs: dataCellIDsForSelectorColumn);

      RenderDataCells(renderingContext, in arguments, dataRowRenderEventArgs);

      renderingContext.Writer.RenderEndTag();

      if (hasValidationFailures)
      {
        var validationFailures =
            renderingContext.Control.ValidationFailureRepository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(
                rowRenderingContext.Row.BusinessObject,
                true);
        RenderValidationRow(renderingContext, rowRenderingContext, dataRowRenderEventArgs, validationFailures, rowIndex);
      }
    }

    private IBocSelectorColumnRenderer GetSelectorColumnRenderer ()
    {
      return _selectorColumnRenderer;
    }

    private IBocIndexColumnRenderer GetIndexColumnRenderer ()
    {
      return _indexColumnRenderer;
    }

    private void RenderValidationRow (
        BocListRenderingContext renderingContext,
        BocListRowRenderingContext rowRenderingContext,
        BocListDataRowRenderEventArgs rowRenderEventArgs,
        IReadOnlyCollection<BocListValidationFailureWithLocationInformation> validationFailuresWithLocationInfo,
        int rowIndex)
    {
      var cssClassValidationRow = GetCssClassValidationRow(rowRenderingContext.IsSelected, rowRenderEventArgs);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClassValidationRow);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tr);

      var columnRenderers = renderingContext.ColumnRenderers;

      var lastColumnIndex = 0;
      for (var i = columnRenderers.Length - 1; i >= 0; i--)
      {
        if (lastColumnIndex <= columnRenderers[i].VisibleColumnIndex)
        {
          lastColumnIndex = columnRenderers[i].VisibleColumnIndex + 1;
          break;
        }
      }

      var validationFailureColumnIndex = 0;

      var validationFailureColumn = columnRenderers.FirstOrDefault(r => r.ColumnDefinition is BocValidationErrorIndicatorColumnDefinition);
      if (validationFailureColumn is not null)
      {
        validationFailureColumnIndex = validationFailureColumn.VisibleColumnIndex;

        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Colspan, validationFailureColumnIndex.ToString());
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);
        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Colspan, (lastColumnIndex - validationFailureColumnIndex).ToString());
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, _cssClasses.ValidationFailureCell);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      var columnIndexProvider = new BocListColumnIndexProvider(columnRenderers);
      var arguments = new BocListValidationSummaryRenderArguments(
          columnIndexProvider,
          validationFailuresWithLocationInfo,
          rowIndex,
          renderCellValidationFailuresAsLinks: true);

      _validationSummaryRenderer.Render(renderingContext, arguments);

      renderingContext.Writer.RenderEndTag(); //Div
      renderingContext.Writer.RenderEndTag(); //Div
      renderingContext.Writer.RenderEndTag(); //Td
      renderingContext.Writer.RenderEndTag(); //Tr
    }

    private void RenderDataCells (BocListRenderingContext renderingContext, in BocRowRenderArguments arguments, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      var rowIndex = arguments.RowIndex;
      var renderInformation = renderingContext.ColumnRenderers
          .Select(r => (ColumnRenderer: r, DataCellID: r.IsRowHeader ? GetDataCellID(renderingContext, columnIndex: r.VisibleColumnIndex, rowIndex: rowIndex) : null))
          .ToArray();

      foreach (var (columnRenderer, dataCellID) in renderInformation)
      {
        var titleCellID = GetTitleCellID(renderingContext, columnRenderer.VisibleColumnIndex);
        string[] headerIDs;
        if (columnRenderer.IsRowHeader)
        {
          // If the current column is part of the row header, only columns to the left of the current column will be integrated as a row header.
          // This prevents the screen reader from announcing duplicate information by including the current cell in the row header. The cells to the right are excluded
          // because skipping just the current column would disrupt the intended information flow (e.g. only read the first name without the last name).
          headerIDs = renderInformation
              .Where(i => i.ColumnRenderer.IsRowHeader && i.ColumnRenderer.VisibleColumnIndex < columnRenderer.VisibleColumnIndex)
              .Select(i => i.DataCellID!)
              .Concat(titleCellID)
              .ToArray();
        }
        else
        {
          // If the current column is not part of the row header, all row header columns will be integrated as the row header.
          // This allows for proper annotation of commands, the index and selector columns, etc
          headerIDs = renderInformation
              .Where(i => i.ColumnRenderer.IsRowHeader)
              .Select(i => i.DataCellID!)
              .Concat(titleCellID)
              .ToArray();
        }

        columnRenderer.RenderDataCell(
            renderingContext,
            rowIndex: rowIndex,
            cellID: dataCellID,
            headerIDs: headerIDs,
            columnsWithValidationFailures: arguments.ColumnsWithValidationFailures,
            dataRowRenderEventArgs);
      }
    }

    private string GetTitleCellID (BocListRenderingContext renderingContext, int columnIndex)
    {
      return renderingContext.Control.ClientID + "_C" + columnIndex;
    }

    private string GetTitleCellIDForIndexColumn (BocListRenderingContext renderingContext)
    {
      return GetTitleCellID(renderingContext, 0);
    }

    private string GetTitleCellIDForSelectorColumn (BocListRenderingContext renderingContext)
    {
      return GetTitleCellID(renderingContext, renderingContext.Control.IsIndexEnabled ? 1 : 0);
    }

    private string GetDataCellID (BocListRenderingContext renderingContext, int columnIndex, int rowIndex)
    {
      return renderingContext.Control.ClientID + "_C" + columnIndex + "_R" + rowIndex;
    }

    private string GetCssClassTableRow (
        BocListRenderingContext renderingContext,
        bool isChecked,
        bool hasValidationErrors,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      string cssClassTableRow = CssClasses.DataRow;

      if (dataRowRenderEventArgs.IsOddRow)
        cssClassTableRow += " " + CssClasses.DataRowOdd;
      else
        cssClassTableRow += " " + CssClasses.DataRowEven;

      if (!string.IsNullOrEmpty(dataRowRenderEventArgs.AdditionalCssClassForDataRow))
        cssClassTableRow += " " + dataRowRenderEventArgs.AdditionalCssClassForDataRow;

      if (isChecked && renderingContext.Control.AreDataRowsClickSensitive())
        cssClassTableRow += " " + CssClasses.DataRowSelected;

      if (hasValidationErrors)
        cssClassTableRow += " " + CssClasses.HasValidationRow;

      return cssClassTableRow;
    }

    private string GetCssClassValidationRow (
        bool isSelected,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      string cssClassTableRow = CssClasses.ValidationRow;

      if (dataRowRenderEventArgs.IsOddRow)
        cssClassTableRow += " " + CssClasses.DataRowOdd;
      else
        cssClassTableRow += " " + CssClasses.DataRowEven;

      if (!string.IsNullOrEmpty(dataRowRenderEventArgs.AdditionalCssClassForDataRow))
        cssClassTableRow += " " + dataRowRenderEventArgs.AdditionalCssClassForDataRow;

      if (isSelected)
        cssClassTableRow += " " + CssClasses.DataRowSelected;

      return cssClassTableRow;
    }
  }
}
