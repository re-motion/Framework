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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Interface for classes that render the index column of <see cref="IBocList"/> controls.
  /// </summary>
 public interface IBocIndexColumnRenderer
  {
    /// <summary>
    /// Renders the index cell for the title row.
    /// </summary>
    void RenderTitleCell (BocListRenderingContext renderingContext, string cellID);

    /// <summary>
    /// Renders the index cell for the data row identified by <paramref name="originalRowIndex"/>.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocListRenderingContext"/>.</param>
    /// <param name="originalRowIndex">The absolute row index in the original (unsorted) item collection.</param>
    /// <param name="absoluteRowIndex">The absolute row index (including previous pages) after sorting.</param>
    /// <param name="headerIDs">The list of IDs (column and row) that identify this cell.</param>
    void RenderDataCell (BocListRenderingContext renderingContext, int originalRowIndex, int absoluteRowIndex, IReadOnlyCollection<string> headerIDs);
  }
}
