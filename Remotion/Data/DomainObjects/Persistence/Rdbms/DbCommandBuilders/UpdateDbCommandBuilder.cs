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
using System.Data;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// The <see cref="UpdateDbCommandBuilder"/> builds a command that allows updating a set of records as specified by a given 
  /// <see cref="IUpdatedColumnsSpecification"/>.
  /// </summary>
  public class UpdateDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _tableDefinition;
    private readonly IUpdatedColumnsSpecification _updatedColumnsSpecification;
    private readonly IComparedColumnsSpecification _comparedColumnsSpecification;

    public UpdateDbCommandBuilder (
        TableDefinition tableDefinition,
        IUpdatedColumnsSpecification updatedColumnsSpecification,
        IComparedColumnsSpecification comparedColumnsSpecification,
        ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
      ArgumentUtility.CheckNotNull("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull("updatedColumnsSpecification", updatedColumnsSpecification);
      ArgumentUtility.CheckNotNull("comparedColumnsSpecification", comparedColumnsSpecification);

      _tableDefinition = tableDefinition;
      _updatedColumnsSpecification = updatedColumnsSpecification;
      _comparedColumnsSpecification = comparedColumnsSpecification;
    }

    public TableDefinition TableDefinition
    {
      get { return _tableDefinition; }
    }

    public IUpdatedColumnsSpecification UpdatedColumnsSpecification
    {
      get { return _updatedColumnsSpecification; }
    }

    public IComparedColumnsSpecification ComparedColumnsSpecification
    {
      get { return _comparedColumnsSpecification; }
    }

    public override IDbCommand Create (IDbCommandFactory dbCommandFactory)
    {
      ArgumentUtility.CheckNotNull("dbCommandFactory", dbCommandFactory);

      var command = dbCommandFactory.CreateDbCommand();
      var statement = new StringBuilder();

      statement.Append("UPDATE ");
      AppendTableName(statement, command, _tableDefinition);
      AppendUpdateClause(statement, command, _updatedColumnsSpecification);
      AppendWhereClause(statement, command, _comparedColumnsSpecification);
      statement.Append(SqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString();
      return command;
    }

    protected virtual void AppendUpdateClause (StringBuilder statement, IDbCommand command, IUpdatedColumnsSpecification updatedColumnsSpecification)
    {
      ArgumentUtility.CheckNotNull("statement", statement);
      ArgumentUtility.CheckNotNull("updatedColumnsSpecification", updatedColumnsSpecification);
      ArgumentUtility.CheckNotNull("command", command);

      statement.Append(" SET ");
      updatedColumnsSpecification.AppendColumnValueAssignments(statement, command, SqlDialect);
    }
  }
}
