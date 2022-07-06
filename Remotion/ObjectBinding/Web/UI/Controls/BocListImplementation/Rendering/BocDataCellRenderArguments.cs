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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocDataCellRenderArguments"/> is a parameter object for <see cref="IBocColumnRenderer"/>.<see cref="IBocColumnRenderer.RenderDataCell"/>.
  /// </summary>
  public readonly struct BocDataCellRenderArguments
  {
    /// <summary>Gets the zero-based index of the row as rendered by the <see cref="BocList"/>.</summary>
    /// <remarks>See <see cref="ListIndex"/> for the absolute index of the row in the <see cref="BocList"/>'s <see cref="BocList.Value"/> collection.</remarks>
    public int RowIndex { get; }

    /// <summary>Gets a flag that describes if an icon should be displayed in this cell.</summary>
    public bool ShowIcon { get; }

    private readonly BocListDataRowRenderEventArgs _dataRowRenderEventArgs;

    public BocDataCellRenderArguments (BocListDataRowRenderEventArgs dataRowRenderEventArgs, int rowIndex, bool showIcon)
    {
      ArgumentUtility.CheckNotNull("dataRowRenderEventArgs", dataRowRenderEventArgs);

      _dataRowRenderEventArgs = dataRowRenderEventArgs;
      RowIndex = rowIndex;
      ShowIcon = showIcon;
    }

    /// <inheritdoc cref="BocListItemEventArgs.ListIndex"/>
    public int ListIndex => _dataRowRenderEventArgs.ListIndex;

    /// <inheritdoc cref="BocListItemEventArgs.BusinessObject"/>
    public IBusinessObject BusinessObject => _dataRowRenderEventArgs.BusinessObject;

    /// <inheritdoc cref="BocListDataRowRenderEventArgs.IsEditableRow"/>
    public bool IsEditableRow => _dataRowRenderEventArgs.IsEditableRow;

    /// <inheritdoc cref="BocListDataRowRenderEventArgs.IsEditableRow"/>
    public bool IsOddRow => _dataRowRenderEventArgs.IsOddRow;

    /// <inheritdoc cref="BocListDataRowRenderEventArgs.AdditionalCssClassForDataRow"/>
    public string? AdditionalCssClassForDataRow => _dataRowRenderEventArgs.AdditionalCssClassForDataRow;
  }
}
