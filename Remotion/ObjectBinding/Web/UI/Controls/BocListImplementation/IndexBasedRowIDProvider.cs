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
using System.Globalization;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  /// <summary>
  /// Row-index-based implementation of the <see cref="IRowIDProvider"/> interface. 
  /// Used when the <see cref="BocList"/> is bound to objects of type <see cref="IBusinessObject"/> (without identity).
  /// </summary>
  [Serializable]
  public class IndexBasedRowIDProvider : IRowIDProvider
  {
    private readonly List<string> _rowIDs;
    private int _nextID;

    public IndexBasedRowIDProvider (IEnumerable<IBusinessObject> businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);

      _rowIDs = businessObjects.Select(obj => GetNextID()).ToList();
    }

    public string GetControlRowID (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);

      return GetRowID(row);
    }

    public string GetItemRowID (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);

      return GetRowID(row);
    }

    public BocListRow? GetRowFromItemRowID (IReadOnlyList<IBusinessObject> values, string rowID)
    {
      ArgumentUtility.CheckNotNull("values", values);
      ArgumentUtility.CheckNotNull("rowID", rowID);

      var rowIndex = ParseRowID(rowID);

      if (values.Count == _rowIDs.Count && rowIndex < _rowIDs.Count)
        return new BocListRow(rowIndex, values[rowIndex]);
      else
        return null;
    }

    public void AddRow (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);
      if (row.Index > _rowIDs.Count)
      {
        throw new InvalidOperationException(
            string.Format(
                "Tried to add row at index {0} but the current length of the row collection is {1}."
                + "The index must not exceed the length of the row collection.",
                row.Index,
                _rowIDs.Count));
      }

      _rowIDs.Insert(row.Index, GetNextID());
    }

    public void RemoveRow (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);
      if (row.Index > _rowIDs.Count)
      {
        throw new InvalidOperationException(
            string.Format(
                "Tried to remove row at index {0} but the current length of the row collection is {1}."
                + "The index must not exceed the length of the row collection.",
                row.Index,
                _rowIDs.Count));
      }

      _rowIDs.RemoveAt(row.Index);
    }

    private string GetNextID ()
    {
      var id = FormatRowID(_nextID);
      _nextID++;
      return id;
    }

    private string GetRowID (BocListRow row)
    {
      if (row.Index >= _rowIDs.Count)
      {
        throw new InvalidOperationException(
            string.Format(
                "Tried to retrieve the ID for the row at index {0} but the current length of the row collection is {1}."
                + "The index must not exceed the length of the row collection.",
                row.Index,
                _rowIDs.Count));
      }

      return _rowIDs[row.Index];
    }

    private static string FormatRowID (int rowIndex)
    {
      return rowIndex.ToString(CultureInfo.InvariantCulture);
    }

    private int ParseRowID (string rowID)
    {
      int result;
      if (!int.TryParse(rowID, NumberStyles.None, CultureInfo.InvariantCulture, out result))
        throw new FormatException(string.Format("RowID '{0}' could not be parsed as an integer.", rowID));
      return result;
    }
  }
}
