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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  public abstract class DbCommandBuilder : IDbCommandBuilder
  {
    private readonly ISqlDialect _sqlDialect;

    protected DbCommandBuilder (ISqlDialect sqlDialect)
    {
      ArgumentNullException.ThrowIfNull(sqlDialect);

      _sqlDialect = sqlDialect;
    }

    public abstract IDbCommand Create (IDbCommandFactory dbCommandFactory);

    public ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    protected virtual void AppendSelectClause (
        StringBuilder statement,
        IDbCommand command,
        ISelectedColumnsSpecification selectedColumns)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(command);
      ArgumentNullException.ThrowIfNull(selectedColumns);

      statement.Append("SELECT ");
      selectedColumns.AppendProjection(statement, SqlDialect);
    }

    protected virtual void AppendFromClause (
        StringBuilder statement,
        IDbCommand command,
        TableDefinition tableDefinition)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(command);
      ArgumentNullException.ThrowIfNull(tableDefinition);

      statement.Append(" FROM ");
      AppendTableName(statement, command, tableDefinition);
    }

    protected void AppendTableName (
        StringBuilder statement,
        IDbCommand command,
        TableDefinition tableDefinition)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(command);
      ArgumentNullException.ThrowIfNull(tableDefinition);

      if (tableDefinition.TableName.SchemaName != null)
      {
        statement.Append(SqlDialect.DelimitIdentifier(tableDefinition.TableName.SchemaName));
        statement.Append(".");
      }
      statement.Append(SqlDialect.DelimitIdentifier(tableDefinition.TableName.EntityName));
    }

    protected virtual void AppendWhereClause (
        StringBuilder statement,
        IDbCommand command,
        IComparedColumnsSpecification comparedColumns)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(command);
      ArgumentNullException.ThrowIfNull(comparedColumns);

      statement.Append(" WHERE ");
      comparedColumns.AddParameters(command, SqlDialect);
      comparedColumns.AppendComparisons(statement, command, SqlDialect);
    }

    protected virtual void AppendOrderByClause (
        StringBuilder statement,
        IDbCommand command,
        IOrderedColumnsSpecification orderedColumnsSpecification)
    {
      ArgumentNullException.ThrowIfNull(statement);
      ArgumentNullException.ThrowIfNull(command);
      ArgumentNullException.ThrowIfNull(orderedColumnsSpecification);

      if (!orderedColumnsSpecification.IsEmpty)
      {
        statement.Append(" ORDER BY ");
        orderedColumnsSpecification.AppendOrderings(statement, SqlDialect);
      }
    }
  }
}
