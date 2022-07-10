// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocTitleCellRenderArguments"/> is a parameter object for <see cref="IBocColumnRenderer"/>.<see cref="IBocColumnRenderer.RenderTitleCell"/>.
  /// </summary>
  public readonly struct BocTitleCellRenderArguments
  {
    /// <summary>Gets the <see cref="Controls.SortingDirection"/> for the <see cref="BocColumnDefinition"/> of this column.</summary>
    public SortingDirection SortingDirection { get; }

    /// <summary>
    /// Gets the zero-based index for position of the <see cref="BocColumnDefinition"/> in the group of columns that comprise the <see cref="BocList"/>'s current sorting order.
    /// </summary>
    public int OrderIndex { get; }

    public BocTitleCellRenderArguments (SortingDirection sortingDirection, int orderIndex)
    {
      SortingDirection = sortingDirection;
      OrderIndex = orderIndex;
    }
  }
}
