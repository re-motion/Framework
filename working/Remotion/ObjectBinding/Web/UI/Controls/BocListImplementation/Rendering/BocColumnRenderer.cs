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
    private readonly SortingDirection _sortingDirection;
    private readonly int _orderIndex;

    public BocColumnRenderer (
        IBocColumnRenderer columnRenderer,
        BocColumnDefinition columnDefinition,
        int columnIndex,
        int visibleColumnIndex,
        bool showIcon,
        SortingDirection sortingDirection,
        int orderIndex)
    {
      ArgumentUtility.CheckNotNull ("columnRenderer", columnRenderer);
      ArgumentUtility.CheckNotNull ("columnDefinition", columnDefinition);

      _columnRenderer = columnRenderer;
      _columnDefinition = columnDefinition;
      _columnIndex = columnIndex;
      _visibleColumnIndex = visibleColumnIndex;
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

    public void RenderTitleCell (BocRenderingContext<IBocList> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var columnRenderingContext = new BocColumnRenderingContext (
          renderingContext.HttpContext,
          renderingContext.Writer,
          renderingContext.Control,
          ColumnDefinition,
          ColumnIndex,
          VisibleColumnIndex);

      _columnRenderer.RenderTitleCell (columnRenderingContext, _sortingDirection, _orderIndex);
    }

    public void RenderDataColumnDeclaration (BocRenderingContext<IBocList> renderingContext, bool isTextXml)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var columnRenderingContext = new BocColumnRenderingContext (
          renderingContext.HttpContext,
          renderingContext.Writer,
          renderingContext.Control,
          ColumnDefinition,
          ColumnIndex,
          VisibleColumnIndex);

      _columnRenderer.RenderDataColumnDeclaration (columnRenderingContext, isTextXml);
    }

    public void RenderDataCell (BocRenderingContext<IBocList> renderingContext, int rowIndex, BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      var columnRenderingContext = new BocColumnRenderingContext (
          renderingContext.HttpContext,
          renderingContext.Writer,
          renderingContext.Control,
          ColumnDefinition,
          ColumnIndex,
          VisibleColumnIndex);

      _columnRenderer.RenderDataCell (columnRenderingContext, rowIndex, _showIcon, dataRowRenderEventArgs);
    }
  }
}