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
using System.Linq;
using Microsoft.SqlServer.Server;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// Handles parameter values that have to be represented as a table-valued parameter.
/// </summary>
public class SqlTableValuedDataParameterDefinition : IDataParameterDefinition
{
  public const string DistinctCollectionTableTypeNameSuffix = "_distinct";
  public bool IsDistinct { get; }
  public IStorageTypeInformation StorageTypeInformation { get; }

  public SqlTableValuedDataParameterDefinition (IStorageTypeInformation storageTypeInformation, bool isDistinct)
  {
    ArgumentUtility.CheckNotNull(nameof(storageTypeInformation), storageTypeInformation);

    IsDistinct = isDistinct;
    StorageTypeInformation = storageTypeInformation;
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

    var tableTypeNameSuffix = IsDistinct ? DistinctCollectionTableTypeNameSuffix : string.Empty;
    var tableTypeName = $"TVP_{StorageTypeInformation.StorageDbType}{tableTypeNameSuffix}";

    var sqlDbType = GetSqlDbType(StorageTypeInformation);
    var sqlMetaData = StorageTypeInformation.StorageTypeLength.HasValue
        ? new SqlMetaData("Value", sqlDbType, StorageTypeInformation.StorageTypeLength.Value)
        : new SqlMetaData("Value", sqlDbType);

    var columnMetaData = new[] { sqlMetaData };

    var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);

    foreach (var item in enumerable)
    {
      if (item == null)
        throw ArgumentUtility.CreateArgumentItemNullException(nameof(value), tvpValue.Count);

      if (!StorageTypeInformation.DotNetType.IsInstanceOfType(item))
        throw ArgumentUtility.CreateArgumentItemTypeException(nameof(value), tvpValue.Count, StorageTypeInformation.DotNetType, item.GetType());

      tvpValue.AddRecord(item);
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
    sqlParameter.Value = tvpValue.IsEmpty ? null : tvpValue;
    sqlParameter.SqlDbType = SqlDbType.Structured;
    sqlParameter.TypeName = tvpValue.TableTypeName;
    return sqlParameter;
  }

  private SqlDbType GetSqlDbType (IStorageTypeInformation storageTypeInformation)
  {
    var dummy = new SqlParameter();
    dummy.DbType = storageTypeInformation.StorageDbType;
    var sqlDbType = dummy.SqlDbType;
    return sqlDbType;
  }
}
