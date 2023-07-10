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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    /// <summary>Gets a flag that describes if the <see cref="BocColumnDefinition"/> is configured as a row header.</summary>
    [MemberNotNullWhen(true, nameof(CellID))]
    public bool IsRowHeader { get; }

    /// <summary>Gets the ID of the rendered title cell for referencing as the row header.</summary>
    /// <returns>Returns not <see langword="null"/> if <see cref="IsRowHeader"/> is <see langword="true"/>, otherwise <see langword="null"/>. </returns>
    public string? CellID { get; }

    /// <summary>Gets the list of IDs that describe the header for this cell.</summary>
    public IReadOnlyCollection<string> HeaderIDs { get; }

    /// <summary>
    /// Gets an array indicating what columns in the <see cref="BocList"/> have validation failures.
    /// Array indices mirror <see cref="BocListRenderingContext"/>.<see cref="BocListRenderingContext.ColumnRenderers"/>.
    /// </summary>
    public IReadOnlyList<bool> ColumnsWithValidationFailures { get; }

    private readonly BocListDataRowRenderEventArgs _dataRowRenderEventArgs;

    public BocDataCellRenderArguments (
        BocListDataRowRenderEventArgs dataRowRenderEventArgs,
        int rowIndex,
        bool showIcon,
        string? cellID,
        IReadOnlyCollection<string> headerIDs,
        IReadOnlyList<bool> columnsWithValidationFailures)
    {
      ArgumentUtility.CheckNotNull("dataRowRenderEventArgs", dataRowRenderEventArgs);
      ArgumentUtility.CheckNotEmpty("cellID", cellID);
      ArgumentUtility.CheckNotNull("headerIDs", headerIDs);
      ArgumentUtility.CheckNotNull("columnsWithValidationFailures", columnsWithValidationFailures);

      _dataRowRenderEventArgs = dataRowRenderEventArgs;
      RowIndex = rowIndex;
      ShowIcon = showIcon;
      IsRowHeader = cellID != null;
      CellID = cellID;
      HeaderIDs = headerIDs;
      ColumnsWithValidationFailures = columnsWithValidationFailures;
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
