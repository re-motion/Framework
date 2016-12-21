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
  public abstract class DbCommandBuilder : IDbCommandBuilder
  {
    private readonly ISqlDialect _sqlDialect;

    protected DbCommandBuilder (ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      _sqlDialect = sqlDialect;
    }

    public abstract IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext);

    public ISqlDialect SqlDialect
    {
      get { return _sqlDialect; }
    }

    protected void AppendSelectClause (StringBuilder statement, ISelectedColumnsSpecification selectedColumns)
    {
      statement.Append ("SELECT ");
      selectedColumns.AppendProjection (statement, SqlDialect);
    }

    protected void AppendFromClause (StringBuilder statement, TableDefinition tableDefinition)
    {
      statement.Append (" FROM ");
      AppendTableName (statement, tableDefinition);
    }

    protected void AppendTableName (StringBuilder statement, TableDefinition tableDefinition)
    {
      if (tableDefinition.TableName.SchemaName != null)
      {
        statement.Append (SqlDialect.DelimitIdentifier (tableDefinition.TableName.SchemaName));
        statement.Append (".");
      }
      statement.Append (SqlDialect.DelimitIdentifier (tableDefinition.TableName.EntityName));
    }

    protected void AppendWhereClause (
        StringBuilder statement,
        IComparedColumnsSpecification comparedColumns,
        IDbCommand command)
    {
      statement.Append (" WHERE ");
      comparedColumns.AddParameters (command, SqlDialect);
      comparedColumns.AppendComparisons (statement, command, SqlDialect);
    }

    protected void AppendOrderByClause (StringBuilder statement, IOrderedColumnsSpecification orderedColumnsSpecification)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("orderedColumnsSpecification", orderedColumnsSpecification);

      if (!orderedColumnsSpecification.IsEmpty)
      {
        statement.Append (" ORDER BY ");
        orderedColumnsSpecification.AppendOrderings (statement, SqlDialect);
      }
    }
  }
}