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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  public class TestableDbCommandBuilder : DbCommandBuilder
  {
    public TestableDbCommandBuilder (ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
    }

    public override IDbCommand Create (IDbCommandFactory dbCommandFactory)
    {
      throw new NotImplementedException();
    }

    public new void AppendSelectClause (StringBuilder statement, IDbCommand command, ISelectedColumnsSpecification selectedColumns)
    {
      base.AppendSelectClause(statement, command, selectedColumns);
    }

    public new void AppendFromClause (StringBuilder statement, IDbCommand command, TableDefinition tableDefinition)
    {
      base.AppendFromClause(statement, command, tableDefinition);
    }

    public new void AppendTableName (StringBuilder statement, IDbCommand command, TableDefinition tableDefinition)
    {
      base.AppendTableName(statement, command, tableDefinition);
    }

    public new void AppendWhereClause (StringBuilder statement, IDbCommand command, IComparedColumnsSpecification comparedColumns)
    {
      base.AppendWhereClause(statement, command, comparedColumns);
    }

    public new void AppendOrderByClause (StringBuilder statement, IDbCommand command, IOrderedColumnsSpecification orderedColumnsSpecification)
    {
      base.AppendOrderByClause(statement, command, orderedColumnsSpecification);
    }
  }
}
