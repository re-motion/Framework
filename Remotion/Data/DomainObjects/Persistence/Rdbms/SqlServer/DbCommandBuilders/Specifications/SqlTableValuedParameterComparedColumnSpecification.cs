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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;

/// <summary>
/// Specifies a set of <see cref="ObjectValues"/> that are transmitted as a table-valued parameter, to be used in a WHERE-IN-clause.
/// </summary>
public class SqlTableValuedParameterComparedColumnSpecification : IComparedColumnsSpecification
{
  /// <summary>
  /// The column whose values are looked up in the <see cref="ObjectValues"/>.
  /// </summary>
  public ColumnDefinition ComparedColumnDefinition { get; }

  public SqlTableValuedDataParameterDefinition DataParameterDefinition { get; }

  /// <summary>
  /// The values that fulfill the filter criterion.  
  /// </summary>
  public IEnumerable<object?> ObjectValues { get; }

  public SqlTableValuedParameterComparedColumnSpecification (
      ColumnDefinition comparedColumn,
      IEnumerable<object?> objectValues,
      SqlTableValuedDataParameterDefinition dataParameterDefinition)
  {
    ArgumentUtility.CheckNotNull(nameof(comparedColumn), comparedColumn);
    ArgumentUtility.CheckNotNull(nameof(objectValues), objectValues);
    ArgumentUtility.CheckNotNull(nameof(dataParameterDefinition), dataParameterDefinition);

    ComparedColumnDefinition = comparedColumn;
    ObjectValues = objectValues;
    DataParameterDefinition = dataParameterDefinition;
  }

  /// <inheritdoc />
  public void AddParameters (IDbCommand command, ISqlDialect sqlDialect)
  {
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNull(nameof(sqlDialect), sqlDialect);

    var parameterName = GetParameterName(sqlDialect);
    var parameterValue = DataParameterDefinition.GetParameterValue(ObjectValues);
    var parameter = DataParameterDefinition.CreateDataParameter(command, parameterName, parameterValue);
    command.Parameters.Add(parameter);
  }

  /// <inheritdoc />
  public void AppendComparisons (StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect)
  {
    ArgumentUtility.CheckNotNull(nameof(statement), statement);
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNull(nameof(sqlDialect), sqlDialect);

    var delimitedColumnName = sqlDialect.DelimitIdentifier(ComparedColumnDefinition.Name);
    var delimitedValue = sqlDialect.DelimitIdentifier("Value");
    var parameterName = GetParameterName(sqlDialect);
    statement.Append($"{delimitedColumnName} IN (SELECT {delimitedValue} FROM {parameterName})");
  }

  private string GetParameterName (ISqlDialect sqlDialect)
  {
    return sqlDialect.GetParameterName(ComparedColumnDefinition.Name);
  }
}
