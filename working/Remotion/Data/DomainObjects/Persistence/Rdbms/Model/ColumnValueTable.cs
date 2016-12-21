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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Defines a sequence of value rows (<see cref="Rows"/>) associated with a list of <see cref="ColumnDefinition"/> instances 
  /// (<see cref="Columns"/>). Each row has exactly as many elements as the <see cref="Columns"/> sequence, and its elements are associated with
  /// the respective <see cref="ColumnDefinition"/>.
  /// </summary>
  /// <remarks>
  /// For efficiency, this data structure only holds <see cref="IEnumerable{T}"/> sequences and does not ensure that the <see cref="Rows"/> have
  /// the same width as the <see cref="Columns"/> sequence. Code initializing <see cref="ColumnValueTable"/> must ensure that this is always the case.
  /// </remarks>
  public struct ColumnValueTable
  {
    public struct Row
    {
      private readonly IEnumerable<object> _values;

      public Row (IEnumerable<object> values)
      {
        ArgumentUtility.CheckNotNull ("values", values);

        _values = values;
      }

      public IEnumerable<object> Values
      {
        get { return _values; }
      }

      public Row Concat (Row other)
      {
        return new Row (_values.Concat (other._values));
      }
    }

    public static ColumnValueTable Combine (ColumnValueTable table1, ColumnValueTable table2)
    {
      var combinedColumns = table1.Columns.Concat (table2.Columns);
      var combinedRows = table1.Rows.Zip (table2.Rows, (valueRow, classIDRow) => valueRow.Concat (classIDRow));

      return new ColumnValueTable (combinedColumns, combinedRows);
    }

    public static ColumnValueTable Combine (IEnumerable<ColumnValueTable> tables)
    {
      return tables.Aggregate (Combine);
    }

    private readonly IEnumerable<ColumnDefinition> _columns;
    private readonly IEnumerable<Row> _rows;

    public ColumnValueTable (IEnumerable<ColumnDefinition> columns, IEnumerable<Row> rows)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);
      ArgumentUtility.CheckNotNull ("rows", rows);

      _columns = columns;
      _rows = rows;
    }

    public IEnumerable<ColumnDefinition> Columns
    {
      get { return _columns; }
    }

    public IEnumerable<Row> Rows
    {
      get { return _rows; }
    }
  }
}