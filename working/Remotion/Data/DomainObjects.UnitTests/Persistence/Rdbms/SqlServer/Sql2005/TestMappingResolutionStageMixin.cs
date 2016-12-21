﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Sql2005
{
  public class TestMappingResolutionStageMixin
  {
    [OverrideTarget]
    public virtual SqlStatement ResolveSqlStatement (SqlStatement sqlStatement, IMappingResolutionContext context)
    {
      Assert.That (sqlStatement.SelectProjection, Is.TypeOf (typeof (ConstantExpression)));
      Assert.That (((ConstantExpression) sqlStatement.SelectProjection).Value, Is.EqualTo ("Value added by preparation mixin"));

      var builder = new SqlStatementBuilder
                    {
                        DataInfo = new StreamedScalarValueInfo (typeof (string)),
                        SelectProjection =
                            new SqlEntityDefinitionExpression (typeof (int), "c", "CookTable", e => e.GetColumn (typeof (int), "ID", false))
                    };
      return builder.GetSqlStatement();
    }
  }
}