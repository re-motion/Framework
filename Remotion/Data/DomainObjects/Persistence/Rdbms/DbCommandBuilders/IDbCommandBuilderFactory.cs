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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// <see cref="IDbCommandBuilderFactory"/> defines the API for all implementations that are responsible to instantiate new 
  /// <see cref="IDbCommandBuilder"/> instances.
  /// </summary>
  public interface IDbCommandBuilderFactory
  {
    IDbCommandBuilder CreateForSelect (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumnValues,
        IEnumerable<OrderedColumn> orderedColumns);
    IDbCommandBuilder CreateForSelect (
        TableDefinition table,
        IEnumerable<ColumnDefinition> selectedColumns,
        ColumnValueTable comparedColumnValueTable,
        IEnumerable<OrderedColumn> orderedColumns);
    IDbCommandBuilder CreateForSelect (
        UnionViewDefinition view,
        IEnumerable<ColumnDefinition> selectedColumns,
        IEnumerable<ColumnValue> comparedColumnValue,
        IEnumerable<OrderedColumn> orderedColumns);

    IDbCommandBuilder CreateForQuery (string statement, IEnumerable<QueryParameterWithDataParameterDefinition> parametersWithType);

    IDbCommandBuilder CreateForInsert (TableDefinition tableDefinition, IEnumerable<ColumnValue> insertedColumns);
    IDbCommandBuilder CreateForUpdate (
        TableDefinition tableDefinition,
        IEnumerable<ColumnValue> updatedColumns,
        IEnumerable<ColumnValue> comparedColumnValues);
    IDbCommandBuilder CreateForDelete (TableDefinition tableDefinition, IEnumerable<ColumnValue> comparedColumnValues);
  }
}
