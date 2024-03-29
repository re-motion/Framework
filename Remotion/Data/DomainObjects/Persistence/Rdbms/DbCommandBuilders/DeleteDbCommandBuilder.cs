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
  /// The <see cref="DeleteDbCommandBuilder"/> builds a command that allows deleting a set of records as specified by a given 
  /// <see cref="IComparedColumnsSpecification"/>.
  /// </summary>
  public class DeleteDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _tableDefinition;
    private readonly IComparedColumnsSpecification _comparedColumnsSpecification;

    public DeleteDbCommandBuilder (
        TableDefinition tableDefinition,
        IComparedColumnsSpecification comparedColumnsSpecification,
        ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
      ArgumentUtility.CheckNotNull("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull("comparedColumnsSpecification", comparedColumnsSpecification);

      _tableDefinition = tableDefinition;
      _comparedColumnsSpecification = comparedColumnsSpecification;
    }

    public TableDefinition TableDefinition
    {
      get { return _tableDefinition; }
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

      statement.Append("DELETE FROM ");
      AppendTableName(statement, command, _tableDefinition);
      AppendWhereClause(statement, command, _comparedColumnsSpecification);
      statement.Append(SqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString();
      return command;
    }

  }
}
