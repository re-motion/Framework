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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocRowRenderArguments"/> is a parameter object for <see cref="IBocRowRenderer"/>.<see cref="IBocRowRenderer.RenderDataRow"/>.
  /// </summary>
  public struct BocRowRenderArguments
  {
    /// <summary>Gets the zero-based index of the row as rendered by the <see cref="BocList"/>.</summary>
    public int RowIndex { get; }

    /// <summary>
    /// Gets an array indicating what columns in the <see cref="BocList"/> have validation failures.
    /// Array indices mirror <see cref="BocListRenderingContext"/>.<see cref="BocListRenderingContext.ColumnRenderers"/>.
    /// </summary>
    public bool[] ColumnsWithValidationFailures { get; }

    public BocRowRenderArguments (int rowIndex, bool[] columnsWithValidationFailures)
    {
      ArgumentUtility.CheckNotNull(nameof(columnsWithValidationFailures), columnsWithValidationFailures);

      RowIndex = rowIndex;
      ColumnsWithValidationFailures = columnsWithValidationFailures;
    }
  }
}
