using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;

public class SqlTableValuedParameterComparedColumnSpecification : IComparedColumnsSpecification
{
  public ColumnDefinition ColumnDefinition { get; }
  public IEnumerable<object?> ObjectValues { get; }

  public SqlTableValuedParameterComparedColumnSpecification (ColumnDefinition columnDefinition, IEnumerable<object?> objectValues)
  {
    ArgumentUtility.CheckNotNull(nameof(columnDefinition), columnDefinition);
    ArgumentUtility.CheckNotNull(nameof(objectValues), objectValues);

    ColumnDefinition = columnDefinition;
    ObjectValues = objectValues;
  }

  public void AddParameters (IDbCommand command, ISqlDialect sqlDialect)
  {
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNull(nameof(sqlDialect), sqlDialect);

    var storageTypeInfo = ColumnDefinition.StorageTypeInfo;

    var parameterName = GetParameterName(sqlDialect);
    var parameter = (SqlParameter)sqlDialect.CreateDataParameter(command, storageTypeInfo, parameterName, null);

    // use the parameter we need anyway to find out the SqlDbType for the column
    parameter.DbType = storageTypeInfo.StorageDbType;
    var sqlMetaData = new SqlMetaData("Value", parameter.SqlDbType);

    var typeName = $"TVP_{storageTypeInfo.StorageDbType}";

    parameter.SqlDbType = SqlDbType.Structured;
    parameter.TypeName = typeName;

    if (ObjectValues.Any())
    {
      var tvpValue = new SqlTableValuedParameterValue(typeName, new[] { sqlMetaData });
      foreach (var value in ObjectValues)
      {
        var convertedValue = storageTypeInfo.ConvertToStorageType(value);
        tvpValue.AddRecord(convertedValue);
      }

      parameter.Value = tvpValue;
    }
    else
    {
      parameter.Value = DBNull.Value;
    }

    command.Parameters.Add(parameter);
  }

  public void AppendComparisons (StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect)
  {
    ArgumentUtility.CheckNotNull(nameof(statement), statement);
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNull(nameof(sqlDialect), sqlDialect);

    var delimitedColumnName = sqlDialect.DelimitIdentifier(ColumnDefinition.Name);
    var delimitedValue = sqlDialect.DelimitIdentifier("Value");
    var parameterName = GetParameterName(sqlDialect);
    statement.Append($"{delimitedColumnName} IN (SELECT {delimitedValue} FROM {parameterName})");
  }

  private string GetParameterName (ISqlDialect sqlDialect)
  {
    return sqlDialect.GetParameterName(ColumnDefinition.Name);
  }
}
