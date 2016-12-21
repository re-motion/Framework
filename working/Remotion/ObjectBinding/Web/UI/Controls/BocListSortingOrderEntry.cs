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

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> Represents the sorting direction for an individual column. </summary>
  /// <remarks> Used when evaluating the current or new sorting order as well as to persist it into the view state. </remarks>
  [Serializable]
  public sealed class BocListSortingOrderEntry
  {
    /// <summary> Represents a null <see cref="BocListSortingOrderEntry"/>. </summary>
    public static readonly BocListSortingOrderEntry Empty = new BocListSortingOrderEntry();

    private int _columnIndex;
    private readonly SortingDirection _direction;
    private readonly bool _isEmpty;

    [NonSerialized]
    private readonly IBocSortableColumnDefinition _column;

    public BocListSortingOrderEntry (IBocSortableColumnDefinition column, SortingDirection direction)
    {
      ArgumentUtility.CheckNotNull ("column", column);
      if (!column.IsSortable)
        throw new ArgumentException ("BocListSortingOrderEntry can only use columns with IBocSortableColumnDefinition.IsSortable set true.", "column");

      _isEmpty = false;
      _column = column;
      _columnIndex = -1;
      _direction = direction;
    }

    private BocListSortingOrderEntry ()
    {
      _isEmpty = true;
      _columnIndex = -1;
      _direction = SortingDirection.None;
    }

    /// <summary> <see langword="true"/> if this sorting order entry is empty. </summary>
    public bool IsEmpty
    {
      get { return _isEmpty; }
    }

    /// <summary> Gets the column to sort by. </summary>
    public IBocSortableColumnDefinition Column
    {
      get { return _column; }
    }

    /// <summary> Gets the sorting direction. </summary>
    public SortingDirection Direction
    {
      get { return _direction; }
    }

    internal int ColumnIndex
    {
      get { return _columnIndex; }
    }

    internal void SetColumnIndex (int columnIndex)
    {
      if (columnIndex < 0)
        throw new ArgumentOutOfRangeException ("columnIndex", columnIndex, "The column index must not be a negative number.");
      if (_isEmpty)
        throw new InvalidOperationException ("Setting the column index of the empty BocListSortingOrderEntry is not supported.");
      _columnIndex = columnIndex;
    }
  }

  /// <summary> The possible sorting directions. </summary>
  public enum SortingDirection
  {
    /// <summary> Do not sort. </summary>
    None,
    /// <summary> Sort ascending. </summary>
    Ascending,
    /// <summary> Sort descending. </summary>
    Descending
  }
}