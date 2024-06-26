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
using System.Data;
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// Handles parameter values that have to be represented as a table-valued parameter.
/// </summary>
public class SqlTableValuedDataParameterDefinition : IDataParameterDefinition
{
  public RecordDefinition RecordDefinition { get; }
  public TableTypeDefinition TableTypeDefinition { get; }

  public SqlTableValuedDataParameterDefinition (RecordDefinition recordDefinition)
  {
    ArgumentUtility.CheckNotNull(nameof(recordDefinition), recordDefinition);
    ArgumentUtility.CheckNotNullAndType<TableTypeDefinition>(nameof(recordDefinition), recordDefinition.StructuredTypeDefinition);

    RecordDefinition = recordDefinition;
    TableTypeDefinition = (TableTypeDefinition)recordDefinition.StructuredTypeDefinition;
  }

  /// <summary>
  /// Creates an <see cref="SqlTableValuedParameterValue"/> from a <paramref name="value"/> that is a collection of items whose type is compatible with the
  /// <see cref="StorageTypeInformation"/>'s <see cref="IStorageTypeInformation.DotNetType"/>.
  /// </summary>
  public object GetParameterValue (object? value)
  {
    var enumerable = ArgumentUtility.CheckType<IEnumerable?>(nameof(value), value);
    if (enumerable == null)
      return DBNull.Value;

    var tvpValue = TableTypeDefinition.CreateTableValuedParameterValue();

    foreach (var item in enumerable)
    {
      if (item == null)
      {
        throw new NotSupportedException(
            "Items within enumerable parameter values must not be null, because this would result in table rows with all columns being NULL. "
            + "This does not work with WHERE IN, and is not useful in JOIN scenarios, wherefore it is not supported.");
      }

      var columnValues = RecordDefinition.GetColumnValues(item);
      tvpValue.AddRecord(columnValues);
    }

    return tvpValue;
  }

  /// <summary>
  /// Creates a <see cref="SqlDbType.Structured"/> <see cref="SqlParameter"/> with its <see cref="SqlParameter.TypeName"/> determined by the given
  /// <see cref="SqlTableValuedParameterValue"/>.
  /// </summary>
  public IDbDataParameter CreateDataParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(parameterName), parameterName);
    var tvpValue = ArgumentUtility.CheckNotNullAndType<SqlTableValuedParameterValue>(nameof(parameterValue), parameterValue);

    var sqlParameter = (SqlParameter)command.CreateParameter();
    sqlParameter.ParameterName = parameterName;
    sqlParameter.Value = tvpValue.IsEmpty ? null : tvpValue;
    sqlParameter.SqlDbType = SqlDbType.Structured;
    sqlParameter.TypeName = tvpValue.TableTypeName;
    return sqlParameter;
  }
}
