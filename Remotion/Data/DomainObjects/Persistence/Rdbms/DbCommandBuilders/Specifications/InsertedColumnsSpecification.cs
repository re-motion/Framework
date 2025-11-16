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
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  /// <summary>
  /// <see cref="InsertedColumnsSpecification"/> defines the API for all implementations that specify the values to insert into the the specified columns 
  /// in a relational  database.
  /// </summary>
  public class InsertedColumnsSpecification : IInsertedColumnsSpecification
  {
    private readonly ColumnValue[] _columnValues;

    public InsertedColumnsSpecification (IEnumerable<ColumnValue> columnValues)
    {
      ArgumentNullException.ThrowIfNull(columnValues);

      _columnValues = columnValues.ToArray();
    }

    public ReadOnlyCollection<ColumnValue> ColumnValues
    {
      get { return Array.AsReadOnly(_columnValues); }
    }

    public void AppendColumnNames (StringBuilder statement, DbCommand dbCommand, ISqlDialect sqlDialect)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(dbCommand);
      ArgumentNullException.ThrowIfNull(sqlDialect);

      var columNames = string.Join(", ", _columnValues.Select(cv => sqlDialect.DelimitIdentifier(cv.Column.Name)));
      statement.Append(columNames);
    }

    public void AppendColumnValues (StringBuilder statement, DbCommand dbCommand, ISqlDialect sqlDialect)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(dbCommand);
      ArgumentNullException.ThrowIfNull(sqlDialect);

      var parameters = _columnValues.Select(
          cv =>
          {
            var storageTypeInfo = cv.Column.StorageTypeInfo;
            var parameterName = sqlDialect.GetParameterName(cv.Column.Name);
            var parameter = sqlDialect.CreateDataParameter(dbCommand, storageTypeInfo, parameterName, cv.Value);
            dbCommand.Parameters.Add(parameter);
            return parameter;
          });

      var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
      statement.Append(parameterNames);
    }
  }
}
