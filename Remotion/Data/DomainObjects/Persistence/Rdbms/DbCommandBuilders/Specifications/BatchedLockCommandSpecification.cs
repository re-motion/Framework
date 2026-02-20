// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;

/// <inheritdoc/>
public class BatchedLockCommandSpecification : IBatchedLockCommandSpecification
{
  private readonly MultiClassTableValuedDataParameterDefinition _parameterDefinition;
  private readonly SqlTableValuedParameterValue _parameterValue;

  public BatchedLockCommandSpecification (TableDefinition tableDefinition, MultiClassTableValuedDataParameterDefinition parameterDefinition, IEnumerable<DataContainer> dataContainers)
  {
    ArgumentNullException.ThrowIfNull(tableDefinition);
    ArgumentNullException.ThrowIfNull(parameterDefinition);
    ArgumentNullException.ThrowIfNull(dataContainers);

    TableDefinition = tableDefinition;
    _parameterDefinition = parameterDefinition;
    _parameterValue = (SqlTableValuedParameterValue)_parameterDefinition.GetParameterValue(dataContainers);
    Columns = _parameterValue.ColumnMetaData.Select(c => c.Name).ToList();
  }

  public TableDefinition TableDefinition { get; }

  public IReadOnlyList<string> Columns { get; }

  public DbParameter CreateDbParameter (DbCommand command, string parameterName)
  {
    ArgumentNullException.ThrowIfNull(command);
    ArgumentNullException.ThrowIfNull(parameterName);

    return _parameterDefinition.CreateDataParameter(command, parameterName, _parameterValue);
  }
}
