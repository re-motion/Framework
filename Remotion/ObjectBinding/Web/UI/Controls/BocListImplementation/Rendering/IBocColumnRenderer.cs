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
    /// <param name="arguments">The cell-specific rendering arguments.</param>
    void RenderTitleCell (BocColumnRenderingContext renderingContext, in BocTitleCellRenderArguments arguments);

    /// <summary>
    /// Renders a table cell for a <see cref="BocColumnDefinition"/> containing the appropriate data from the <see cref="IBusinessObject"/>
    /// contained in <paramref name="arguments"/>.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext"/>.</param>
    /// <param name="arguments">The cell-specific rendering arguments.</param>
    void RenderDataCell (BocColumnRenderingContext renderingContext, in BocDataCellRenderArguments arguments);

    /// <summary>
    /// Renders a data column declaration for a <see cref="BocColumnDefinition"/>.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext"/>.</param>
    /// <param name="isTextXml">Specifies the text syntax.</param>
    void RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml);
  }
}
