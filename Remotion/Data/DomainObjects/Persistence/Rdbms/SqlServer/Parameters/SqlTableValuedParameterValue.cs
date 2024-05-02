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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// Single parameter value of type <see cref="TableTypeName"/> that contains columns as defined by <see cref="ColumnMetaData"/> and any number of <see cref="SqlDataRecord"/>s.
/// </summary>
public class SqlTableValuedParameterValue : IReadOnlyCollection<SqlDataRecord>
{
  /// <summary>
  /// Name of the table type as defined in the database, with columns corresponding to the <see cref="ColumnMetaData"/>.
  /// </summary>
  public string TableTypeName { get; }

  /// <summary>
  /// Representation of the columns in the table type identified by <see cref="TableTypeName"/>.
  /// </summary>
  public IReadOnlyCollection<SqlMetaData> ColumnMetaData => _columnMetaData;

  private readonly List<SqlDataRecord> _records = new List<SqlDataRecord>();
  private readonly SqlMetaData[] _columnMetaData;

  public SqlTableValuedParameterValue (string tableTypeName, IReadOnlyCollection<SqlMetaData> columnMetaData)
  {
    ArgumentUtility.CheckNotNull(nameof(tableTypeName), tableTypeName);
    ArgumentUtility.CheckNotNull(nameof(columnMetaData), columnMetaData);

    TableTypeName = tableTypeName;
    _columnMetaData = columnMetaData.ToArray();
  }

  /// <summary>
  /// Adds an <see cref="SqlDataRecord"/> with the given <paramref name="columnValues"/> to this <see cref="SqlTableValuedParameterValue"/>.
  /// </summary>
  /// <param name="columnValues">Must have the same length as the <see cref="ColumnMetaData"/>
  /// and each value must be of a type compatible with the respective <see cref="SqlMetaData"/> in <see cref="ColumnMetaData"/>.</param>
  public void AddRecord (params object[] columnValues)
  {
    if (columnValues.Length != _columnMetaData.Length)
    {
      throw new ArgumentException(
          $"Record has {_columnMetaData.Length} {(_columnMetaData.Length == 1 ? "value" : "values")} "
          + $"but {columnValues.Length} {(columnValues.Length == 1 ? "was" : "were")} provided.",
          nameof(columnValues));
    }

    var record = new SqlDataRecord(_columnMetaData);
    record.SetValues(columnValues);
    _records.Add(record);
  }

  public IEnumerator GetEnumerator ()
  {
    return ((IEnumerable)_records).GetEnumerator();
  }

  IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator ()
  {
    return _records.GetEnumerator();
  }

  public int Count => _records.Count;
}
