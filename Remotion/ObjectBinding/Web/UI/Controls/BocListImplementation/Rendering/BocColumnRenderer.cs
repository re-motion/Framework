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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocColumnRenderer"/> holds a <see cref="BocColumnDefinition"/>, its corresponding <see cref="IBocColumnRenderer"/> and other 
  /// information needed to render a <see cref="BocList"/> column. It exposes methods which delegate to the <see cref="IBocColumnRenderer"/> 
  /// to render the title cell, data cell, and the data column declaration. 
  /// </summary>
  public class BocColumnRenderer
  {
    private readonly IBocColumnRenderer _columnRenderer;
    private readonly BocColumnDefinition _columnDefinition;
    private readonly int _columnIndex;
    private readonly int _visibleColumnIndex;
    private readonly bool _showIcon;
    private readonly bool _isRowHeader;
    private readonly SortingDirection _sortingDirection;
    private readonly int _orderIndex;

    public BocColumnRenderer (
        IBocColumnRenderer columnRenderer,
        BocColumnDefinition columnDefinition,
        int columnIndex,
        int visibleColumnIndex,
        bool isRowHeader,
        bool showIcon,
        SortingDirection sortingDirection,
        int orderIndex)
    {
      ArgumentUtility.CheckNotNull("columnRenderer", columnRenderer);
      ArgumentUtility.CheckNotNull("columnDefinition", columnDefinition);

      _columnRenderer = columnRenderer;
      _columnDefinition = columnDefinition;
      _columnIndex = columnIndex;
      _visibleColumnIndex = visibleColumnIndex;
      _isRowHeader = isRowHeader;
      _showIcon = showIcon;
      _sortingDirection = sortingDirection;
      _orderIndex = orderIndex;
    }

    public BocColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public bool IsVisibleColumn
    {
      get { return !_columnRenderer.IsNull; }
    }

    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    public int VisibleColumnIndex
    {
      get { return _visibleColumnIndex; }
    }

    public bool IsRowHeader
    {
      get { return _isRowHeader; }
    }

    public bool ShowIcon
    {
      get { return _showIcon; }
    }

    public SortingDirection SortingDirection
    {
      get { return _sortingDirection; }
    }

    public int OrderIndex
    {
      get { return _orderIndex; }
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext, string cellID)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty("cellID", cellID);

      var columnRenderingContext = CreateBocColumnRenderingContext(renderingContext);

      _columnRenderer.RenderTitleCell(
          columnRenderingContext,
          new BocTitleCellRenderArguments(
              sortingDirection: _sortingDirection,
              orderIndex: _orderIndex,
              cellID: cellID,
              isRowHeader: _isRowHeader));
    }

    public void RenderDataColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      var columnRenderingContext = CreateBocColumnRenderingContext(renderingContext);

      _columnRenderer.RenderDataColumnDeclaration(columnRenderingContext, isTextXml);
    }

    public void RenderDataCell (
        BocListRenderingContext renderingContext,
        int rowIndex,
        string? cellID,
        IReadOnlyCollection<string> headerIDs,
        IReadOnlyList<bool> columnsWithValidationFailures,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("dataRowRenderEventArgs", dataRowRenderEventArgs);
      ArgumentUtility.CheckNotNull("columnsWithValidationFailures", columnsWithValidationFailures);
      if (renderingContext.ColumnRenderers.Length != columnsWithValidationFailures.Count)
        throw new ArgumentException("The number of validation failures flags must match the number of column renderers.", nameof(columnsWithValidationFailures));

      var columnRenderingContext = CreateBocColumnRenderingContext(renderingContext);

      _columnRenderer.RenderDataCell(
          columnRenderingContext,
          new BocDataCellRenderArguments(
              dataRowRenderEventArgs,
              rowIndex: rowIndex,
              showIcon: _showIcon,
              cellID: cellID,
              headerIDs: headerIDs,
              columnsWithValidationFailures: columnsWithValidationFailures));
    }

    private BocColumnRenderingContext CreateBocColumnRenderingContext (BocListRenderingContext renderingContext)
    {
      return new BocColumnRenderingContext(
          renderingContext.HttpContext,
          renderingContext.Writer,
          renderingContext.Control,
          renderingContext.BusinessObjectWebServiceContext,
          ColumnDefinition,
          renderingContext.ColumnIndexProvider,
          ColumnIndex,
          VisibleColumnIndex);
    }
  }
}
