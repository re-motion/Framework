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
using System.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// Builds an <see cref="IDbCommand"/> for a given <see cref="IQuery"/>.
  /// </summary>
  public class QueryDbCommandBuilder : DbCommandBuilder
  {
    private readonly string _statement;
    private readonly QueryParameterWithType[] _parametersWithType;
    
    public QueryDbCommandBuilder (string statement, IEnumerable<QueryParameterWithType> parameters, ISqlDialect sqlDialect)
        : base (sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      _statement = statement;
      _parametersWithType = parameters.ToArray();
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      var command = commandExecutionContext.CreateDbCommand ();

      var statement = _statement;
      foreach (var parameterWithType in _parametersWithType)
      {
        if (parameterWithType.QueryParameter.ParameterType == QueryParameterType.Text)
          statement = statement.Replace (parameterWithType.QueryParameter.Name, parameterWithType.QueryParameter.Value.ToString ());
        else
        {
          var parameter = parameterWithType.StorageTypeInformation.CreateDataParameter (command, parameterWithType.QueryParameter.Value);
          parameter.ParameterName = parameterWithType.QueryParameter.Name;
          command.Parameters.Add (parameter);
        }
      }

      command.CommandText = statement;
      return command;
    }
  }
}