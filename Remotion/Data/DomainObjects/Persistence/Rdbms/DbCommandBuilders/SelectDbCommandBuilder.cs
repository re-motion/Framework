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
  /// The <see cref="SelectDbCommandBuilder"/> builds a command that allows retrieving a set of records as specified by a given 
  /// <see cref="IComparedColumnsSpecification"/>.
  /// </summary>
  public class SelectDbCommandBuilder : DbCommandBuilder
  {
    private readonly TableDefinition _table;
    private readonly ISelectedColumnsSpecification _selectedColumns;
    private readonly IComparedColumnsSpecification _comparedColumns;
    private readonly IOrderedColumnsSpecification _orderedColumns;

    public SelectDbCommandBuilder (
        TableDefinition table,
        ISelectedColumnsSpecification selectedColumns,
        IComparedColumnsSpecification comparedColumns,
        IOrderedColumnsSpecification  orderedColumns,
        ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
      ArgumentUtility.CheckNotNull("table", table);
      ArgumentUtility.CheckNotNull("selectedColumns", selectedColumns);
      ArgumentUtility.CheckNotNull("comparedColumns", comparedColumns);
      ArgumentUtility.CheckNotNull("orderedColumns", orderedColumns);

      _table = table;
      _selectedColumns = selectedColumns;
      _comparedColumns = comparedColumns;
      _orderedColumns = orderedColumns;
    }

    public TableDefinition Table
    {
      get { return _table; }
    }

    public ISelectedColumnsSpecification SelectedColumns
    {
      get { return _selectedColumns; }
    }

    public IComparedColumnsSpecification ComparedColumns
    {
      get { return _comparedColumns; }
    }

    public IOrderedColumnsSpecification OrderedColumns
    {
      get { return _orderedColumns; }
    }

    public override IDbCommand Create (IDbCommandFactory dbCommandFactory)
    {
      ArgumentUtility.CheckNotNull("dbCommandFactory", dbCommandFactory);

      var command = dbCommandFactory.CreateDbCommand();

      var statement = new StringBuilder();
      AppendSelectClause(statement, command, _selectedColumns);
      AppendFromClause(statement, command, _table);
      AppendWhereClause(statement, command, _comparedColumns);
      AppendOrderByClause(statement, command, _orderedColumns);
      statement.Append(SqlDialect.StatementDelimiter);

      command.CommandText = statement.ToString();

      return command;
    }
  }
}
