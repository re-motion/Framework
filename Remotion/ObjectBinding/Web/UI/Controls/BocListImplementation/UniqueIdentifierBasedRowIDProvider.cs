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
using System.Text;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  /// <summary>
  /// <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>-based implementation of the <see cref="IRowIDProvider"/> interface. 
  /// Used when the <see cref="BocList"/> is bound to objects of type <see cref="IBusinessObjectWithIdentity"/>.
  /// </summary>
  [Serializable]
  public class UniqueIdentifierBasedRowIDProvider : IRowIDProvider
  {
    public string GetControlRowID (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);

      return EscapeUniqueIdentifier(((IBusinessObjectWithIdentity)row.BusinessObject).UniqueIdentifier);
    }

    public string GetItemRowID (BocListRow row)
    {
      ArgumentUtility.CheckNotNull("row", row);

      return FormatItemRowID(row.Index, ((IBusinessObjectWithIdentity)row.BusinessObject).UniqueIdentifier);
    }

    public BocListRow? GetRowFromItemRowID (IReadOnlyList<IBusinessObject> values, string rowID)
    {
      ArgumentUtility.CheckNotNull("values", values);
      ArgumentUtility.CheckNotNull("rowID", rowID);

      var tuple = ParseItemRowID(rowID);
      int rowIndex = tuple.Item1;
      string uniqueIdentifier = tuple.Item2;

      if (rowIndex < values.Count && ((IBusinessObjectWithIdentity)values[rowIndex]).UniqueIdentifier == uniqueIdentifier)
      {
        return new BocListRow(rowIndex, (IBusinessObjectWithIdentity)values[rowIndex]);
      }
      else
      {
        for (int indexDown = rowIndex - 1, indexUp = rowIndex + 1; indexDown >= 0 || indexUp < values.Count; indexDown--, indexUp++)
        {
          if (indexDown >= 0 && indexDown < values.Count && ((IBusinessObjectWithIdentity)values[indexDown]).UniqueIdentifier == uniqueIdentifier)
            return new BocListRow(indexDown, ((IBusinessObjectWithIdentity)values[indexDown]));

          if (indexUp < values.Count && ((IBusinessObjectWithIdentity)values[indexUp]).UniqueIdentifier == uniqueIdentifier)
            return new BocListRow(indexUp,  ((IBusinessObjectWithIdentity)values[indexUp]));
        }
      }

      return null;
    }

    public void AddRow (BocListRow row)
    {
    }

    public void RemoveRow (BocListRow row)
    {
    }

    private static string FormatItemRowID (int rowIndex, string uniqueIdentifier)
    {
      return rowIndex.ToString(CultureInfo.InvariantCulture) + "|" + uniqueIdentifier;
    }

    private Tuple<int, string> ParseItemRowID (string rowID)
    {
      var parts = rowID.Split(new[] { '|' }, 2);
      if (parts.Length != 2)
        throw CreateItemRowIDFormatException(rowID);

      int rowIndex;
      if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out rowIndex))
        throw CreateItemRowIDFormatException(rowID);

      var uniqueIdentifier = parts[1];
      var tuple = Tuple.Create(rowIndex, uniqueIdentifier);
      return tuple;
    }

    private FormatException CreateItemRowIDFormatException (string rowID)
    {
      return new FormatException(
          string.Format("RowID '{0}' could not be parsed. Expected format: '<rowIndex>|<unqiueIdentifier>'", rowID));
    }

    private static string EscapeUniqueIdentifier (string uniqueIdentifier)
    {
      var escapedID = new StringBuilder(uniqueIdentifier);
      for (int i = 0; i < escapedID.Length; i++)
      {
        if (!char.IsLetterOrDigit(escapedID[i]))
          escapedID[i] = '_';
      }
      return escapedID.ToString();
    }
  }
}
