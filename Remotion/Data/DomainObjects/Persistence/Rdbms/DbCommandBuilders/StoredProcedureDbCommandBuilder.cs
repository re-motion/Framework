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
using System.Data.Common;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

/// <summary>
/// Builds an <see cref="IDbCommand"/> for a given <see cref="QueryStatementType.StoredProcedure"/> call.
/// </summary>
public class StoredProcedureDbCommandBuilder : DbCommandBuilder
{
  public string ProcedureName { get; }
  public IEnumerable<QueryParameterWithDataParameterDefinition> Parameters { get; }

  public StoredProcedureDbCommandBuilder (string procedureName, IEnumerable<QueryParameterWithDataParameterDefinition> parameters, ISqlDialect sqlDialect)
      : base(sqlDialect)
  {
    ArgumentException.ThrowIfNullOrEmpty(procedureName);
    ArgumentNullException.ThrowIfNull(parameters);

    ProcedureName = procedureName;
    Parameters = parameters;
  }

  public override DbCommand Create (IDbCommandFactory dbCommandFactory)
  {
    ArgumentNullException.ThrowIfNull(dbCommandFactory);

    var command = dbCommandFactory.CreateDbCommand();
    command.CommandText = ProcedureName;
    command.CommandType = CommandType.StoredProcedure;

    var valueParameters = Parameters;
    foreach (var parameterWithDefinition in valueParameters)
    {
      if (parameterWithDefinition.QueryParameter.ParameterType == QueryParameterType.Text)
        throw new NotSupportedException($"{nameof(StoredProcedureDbCommandBuilder)} does not support text parameters.");

      var dataParameterName = parameterWithDefinition.QueryParameter.Name;
      var dataParameterValue = parameterWithDefinition.DataParameterDefinition.GetParameterValue(parameterWithDefinition.QueryParameter.Value);
      var parameter = parameterWithDefinition.DataParameterDefinition.CreateDataParameter(command, dataParameterName, dataParameterValue);

      command.Parameters.Add(parameter);
    }

    return command;
  }
}
