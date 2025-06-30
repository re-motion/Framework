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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// Builds an <see cref="IDbCommand"/> for a given <see cref="IQuery"/>.
  /// </summary>
  public class QueryDbCommandBuilder : DbCommandBuilder
  {
    private static readonly ConcurrentDictionary<string, Regex> s_regexDictionary = new();
    private readonly string _statement;
    private readonly IReadOnlyDictionary<string, QueryParameterWithDataParameterDefinition> _parameters;


    public QueryDbCommandBuilder (string statement, IEnumerable<QueryParameterWithDataParameterDefinition> parameters, ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(parameters);

      _statement = statement;
      _parameters = parameters.ToDictionary(param => param.QueryParameter.Name, param => param).AsReadOnly();
    }


    public override IDbCommand Create (IDbCommandFactory dbCommandFactory)
    {
      ArgumentNullException.ThrowIfNull(dbCommandFactory);

      var command = dbCommandFactory.CreateDbCommand();

      var statementBuilder = new StringBuilder(_statement.Length + Math.Min(3 * _parameters.Count, 100));
      var statementIndex = 0;

      var regexKey = SqlDialect.GetParameterName("regex");
      var paramRegex = s_regexDictionary.GetOrAdd(
          regexKey,
          key => new Regex(Regex.Escape(key).Replace("regex", "\\b[\\w\\d_]+\\b"), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.NonBacktracking));

      foreach (Match match in paramRegex.Matches(_statement))
      {
        statementBuilder.Append(_statement.Substring(statementIndex, match.Index - statementIndex));
        statementIndex = match.Index + match.Length;
        if(_parameters.TryGetValue(match.Value, out var parameterDefinition))
          statementBuilder.Append(GetParameterTextRepresentation(parameterDefinition));
        else
          statementBuilder.Append(match.Value);
      }
      statementBuilder.Append(_statement.Substring(statementIndex, _statement.Length - statementIndex));

      var valueParameters = _parameters.Values.Where(p => p.QueryParameter.ParameterType == QueryParameterType.Value);
      foreach (var parameterWithDefinition in valueParameters)
      {
        var dataParameterName = parameterWithDefinition.QueryParameter.Name;
        var dataParameterValue = parameterWithDefinition.DataParameterDefinition.GetParameterValue(parameterWithDefinition.QueryParameter.Value);
        var parameter = parameterWithDefinition.DataParameterDefinition.CreateDataParameter(command, dataParameterName, dataParameterValue);

        command.Parameters.Add(parameter);
      }

      command.CommandText = statementBuilder.ToString();
      return command;
    }

    protected virtual string GetParameterTextRepresentation (QueryParameterWithDataParameterDefinition parameterWithDefinition)
    {
      var queryParameterValue = parameterWithDefinition.QueryParameter.Value;
      switch (parameterWithDefinition.QueryParameter.ParameterType)
      {
        case QueryParameterType.Text:
          Assertion.DebugAssert(
              queryParameterValue is string,
              "parameterWithType.QueryParameter.Value is string when parameterWithType.QueryParameter.ParameterType == Text");
          return (string)queryParameterValue;
        case QueryParameterType.Value:
          return parameterWithDefinition.QueryParameter.Name;
        default:
          throw new NotSupportedException($"Parameter type '{parameterWithDefinition.QueryParameter.ParameterType}' is not supported");
      }
    }
  }
}
