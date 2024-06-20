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
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Linq;
using System.Threading;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Represents an SQL Server table type.
  /// </summary>
  public class TableTypeDefinition : IRdbmsStructuredTypeDefinition
  {
    /// <inheritdoc/>
    public EntityNameDefinition TypeName { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IRdbmsStoragePropertyDefinition> Properties { get; }

    /// <summary>
    /// The constraints that are defined on the table type. 
    /// </summary>
    public IReadOnlyCollection<ITableConstraintDefinition> Constraints { get; }

    private readonly Lazy<IReadOnlyCollection<SqlMetaData>> _sqlMetaData;

    public TableTypeDefinition (
        EntityNameDefinition typeName,
        IReadOnlyCollection<IRdbmsStoragePropertyDefinition> properties,
        IReadOnlyCollection<ITableConstraintDefinition> constraints
    )
    {
      ArgumentUtility.CheckNotNull(nameof(typeName), typeName);
      ArgumentUtility.CheckNotNullOrEmpty(nameof(properties), properties);
      ArgumentUtility.CheckNotNull(nameof(constraints), constraints);

      TypeName = typeName;
      Properties = properties.AsReadOnly();
      Constraints = constraints.AsReadOnly();
      _sqlMetaData = new Lazy<IReadOnlyCollection<SqlMetaData>>(CreateSqlMetaData, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Gets a <see cref="ColumnDefinition"/> for each column that is defined on the table type.
    /// </summary>
    public IEnumerable<ColumnDefinition> GetAllColumns ()
    {
      return Properties.SelectMany(p => p.GetColumns());
    }

    /// <summary>
    /// Creates an empty <see cref="SqlTableValuedParameterValue"/> based on <see cref="IRdbmsStructuredTypeDefinition.TypeName"/>,
    /// <see cref="IRdbmsStructuredTypeDefinition.Properties"/>, and <see cref="Constraints"/>. 
    /// </summary>
    public SqlTableValuedParameterValue CreateTableValuedParameterValue ()
    {
      var tableTypeName = string.IsNullOrEmpty(TypeName.SchemaName) ? TypeName.EntityName : $"{TypeName.SchemaName}.{TypeName.EntityName}";
      return new SqlTableValuedParameterValue(tableTypeName, _sqlMetaData.Value);
    }

    private IReadOnlyCollection<SqlMetaData> CreateSqlMetaData ()
    {
      var columnMetaData = new List<SqlMetaData>();
      foreach (var columnDefinition in GetAllColumns())
      {
        var sqlDbType = GetSqlDbType(columnDefinition.StorageTypeInfo);
        var sqlMetaData = columnDefinition.StorageTypeInfo.StorageTypeLength.HasValue
            ? new SqlMetaData(columnDefinition.Name, sqlDbType, columnDefinition.StorageTypeInfo.StorageTypeLength.Value)
            : new SqlMetaData(columnDefinition.Name, sqlDbType);

        columnMetaData.Add(sqlMetaData);
      }

      return columnMetaData.AsReadOnly();
    }

    private static SqlDbType GetSqlDbType (IStorageTypeInformation storageTypeInformation)
    {
      var dummy = new SqlParameter();
      dummy.DbType = storageTypeInformation.StorageDbType;
      var sqlDbType = dummy.SqlDbType;
      return sqlDbType;
    }

    /// <summary>
    /// Calls <see cref="IRdbmsStructuredTypeDefinitionVisitor.VisitTableTypeDefinition"/> on the given <paramref name="visitor"/>.
    /// </summary>
    void IRdbmsStructuredTypeDefinition.Accept (IRdbmsStructuredTypeDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull(nameof(visitor), visitor);

      visitor.VisitTableTypeDefinition(this);
    }
  }
}
