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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Interface for classes responsible for rendering table cells from a column definition derived from <see cref="BocColumnDefinition"/>.
  /// </summary>
  public interface IBocColumnRenderer : INullObject
  {
    /// <summary>
    /// Renders a table header cell for a <see cref="BocColumnDefinition"/> including title and sorting controls.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext"/>.</param>
    /// <param name="sortingDirection">Specifies if rows are sorted by this column's data, and if so in which direction.</param>
    /// <param name="orderIndex">The zero-based index of the column in a virtual sorted list containing all columns by which data is sorted.</param>
    void RenderTitleCell (BocColumnRenderingContext renderingContext, SortingDirection sortingDirection, int orderIndex);

    /// <summary>
    /// Renders a table cell for a <see cref="BocColumnDefinition"/> containing the appropriate data from the <see cref="IBusinessObject"/>
    /// contained in <paramref name="dataRowRenderEventArgs"/>.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext"/>.</param>
    /// <param name="rowIndex">The zero-based index of the row on the page to be displayed.</param>
    /// <param name="showIcon">Specifies if an object-specific icon will be rendered in the table cell.</param>
    /// <param name="dataRowRenderEventArgs">Specifies row-specific arguments used in rendering the table cell.</param>
    void RenderDataCell (BocColumnRenderingContext renderingContext, int rowIndex, bool showIcon, BocListDataRowRenderEventArgs dataRowRenderEventArgs);

    /// <summary>
    /// Renders a data column declaration for a <see cref="BocColumnDefinition"/>.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext"/>.</param>
    /// <param name="isTextXml">Specifies the text syntax.</param>
    void RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml);
  }
}