// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Data.SqlClient;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// Handles parameter values that have to be represented as a table-valued parameter for different classes that are stored in the same table.
/// </summary>
public class MultiClassTableValuedDataParameterDefinition : IDataParameterDefinition
{
  private readonly Func<ITableManipulationRecordDefinitionProvider, ClassDefinition, RecordDefinition> _getRecordDefinitionFunc;

  private class TableManipulationDataContainerAccessor : ITableManipulationDataContainerAccessor
  {
    private readonly DataContainer _dataContainer;

    public TableManipulationDataContainerAccessor (DataContainer dataContainer)
    {
      ArgumentNullException.ThrowIfNull(dataContainer);

      _dataContainer = dataContainer;
    }

    public ObjectID GetID () => _dataContainer.ID;

    public object GetTimestamp () => _dataContainer.Timestamp!;

    public object? GetValue (PropertyDefinition propertyDefinition) => _dataContainer.GetValue(propertyDefinition);

    public object? GetOptionalValue (PropertyDefinition propertyDefinition)
    {
      throw new NotImplementedException();
    }

    public bool IsOptionalValueSet (PropertyDefinition propertyDefinition)
    {
      throw new NotImplementedException();
    }
  }

  protected ITableManipulationRecordDefinitionProvider TableManipulationRecordDefinitionProvider { get; }

  public MultiClassTableValuedDataParameterDefinition (ITableManipulationRecordDefinitionProvider tableManipulationRecordDefinitionProvider, Func<ITableManipulationRecordDefinitionProvider, ClassDefinition, RecordDefinition> getRecordDefinitionFunc)
  {
    ArgumentNullException.ThrowIfNull(tableManipulationRecordDefinitionProvider);
    ArgumentNullException.ThrowIfNull(getRecordDefinitionFunc);

    TableManipulationRecordDefinitionProvider = tableManipulationRecordDefinitionProvider;
    _getRecordDefinitionFunc = getRecordDefinitionFunc;
  }

  public object GetParameterValue (object? value)
  {
    var dataContainers = ArgumentUtility.CheckNotNullAndType<IEnumerable<DataContainer>>(nameof(value), value!);

    TableTypeDefinition? tableTypeDefinition = null;
    SqlTableValuedParameterValue parameterValue = null!;

    var groupedByClassDefinition = dataContainers.GroupBy(d => d.ClassDefinition).ToList();
    // in case we do not have any data containers we cannot create TVP because we do not know which one.
    if (groupedByClassDefinition.Count == 0)
      throw ArgumentUtility.CreateArgumentEmptyExceptionForCollection(nameof(value));

    foreach (var group in groupedByClassDefinition)
    {
      var recordDefinition = _getRecordDefinitionFunc(TableManipulationRecordDefinitionProvider, group.Key);
      if (tableTypeDefinition == null)
      {
        tableTypeDefinition = (TableTypeDefinition)recordDefinition.StructuredTypeDefinition;
        parameterValue = tableTypeDefinition.CreateTableValuedParameterValue();
      }
      else if (tableTypeDefinition != recordDefinition.StructuredTypeDefinition)
      {
        throw new InvalidOperationException($"Found different {nameof(TableTypeDefinition)}s for one {nameof(MultiClassTableValuedDataParameterDefinition)}.");
      }

      foreach (var columnValues in group.Select(dataContainer => recordDefinition.GetColumnValues(new TableManipulationDataContainerAccessor(dataContainer))))
      {
        parameterValue!.AddRecord(columnValues);
      }
    }

    return parameterValue;
  }

  public DbParameter CreateDataParameter (DbCommand command, string parameterName, object parameterValue)
  {
    ArgumentNullException.ThrowIfNull(command);
    ArgumentException.ThrowIfNullOrEmpty(parameterName);
    var tvpValue = ArgumentUtility.CheckNotNullAndType<SqlTableValuedParameterValue>(nameof(parameterValue), parameterValue);

    var sqlParameter = (SqlParameter)command.CreateParameter();
    sqlParameter.ParameterName = parameterName;
    sqlParameter.Value = tvpValue.IsEmpty ? null : tvpValue;
    sqlParameter.SqlDbType = SqlDbType.Structured;
    sqlParameter.TypeName = tvpValue.TableTypeName;
    return sqlParameter;
  }
}
