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
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  public static class Revision
  {
    public static IQuery GetGetRevisionQuery (IRevisionKey revisionKey)
    {
      ArgumentUtility.CheckNotNull("revisionKey", revisionKey);

      var storageProviderDefinition = GetStorageProviderDefinition();
      var sqlDialect = storageProviderDefinition.Factory.CreateSqlDialect(storageProviderDefinition);

      var parameters = new QueryParameterCollection();

      var statement = new StringBuilder();
      statement.Append("SELECT ");
      statement.Append(GetRevisionValueColumnIdentifier(sqlDialect));
      statement.Append(" FROM ");
      statement.Append(GetRevisionTableIdentifier(sqlDialect));
      statement.Append(" WHERE (");
      AppendKeyClause(statement, parameters, revisionKey, sqlDialect);
      statement.Append(")");
      statement.Append(sqlDialect.StatementDelimiter);

      return QueryFactory.CreateQuery(
          new QueryDefinition(
              typeof(Revision) + "." + MethodBase.GetCurrentMethod()!.Name,
              storageProviderDefinition,
              statement.ToString(),
              QueryType.ScalarReadOnly),
          parameters);
    }

    public static IQuery GetIncrementRevisionQuery (IRevisionKey revisionKey)
    {
      ArgumentUtility.CheckNotNull("revisionKey", revisionKey);

      var storageProviderDefinition = GetStorageProviderDefinition();
      var sqlDialect = storageProviderDefinition.Factory.CreateSqlDialect(storageProviderDefinition);

      const string incrementrevision = "IncrementRevision";
      string revisionTable = GetRevisionTableIdentifier(sqlDialect);
      string revisionValueColumn = GetRevisionValueColumnIdentifier(sqlDialect);
      string revisionValueParameter = sqlDialect.GetParameterName("value");
      string revisionGlobalKeyParameter = sqlDialect.GetParameterName("globalKey");
      string revisionLocalKeyParameter = sqlDialect.GetParameterName("localKey");

      var parameters = new QueryParameterCollection();

      var statement = new StringBuilder();
      statement.Append("BEGIN TRANSACTION " + incrementrevision);
      statement.Append(sqlDialect.StatementDelimiter);
      statement.AppendLine();
      statement.Append("IF EXISTS (SELECT 0 FROM ");
      statement.Append(revisionTable);
      statement.Append(" WHERE (");
      AppendKeyClause(statement, parameters, revisionKey, sqlDialect);
      statement.Append(")");
      statement.Append(")");
      statement.AppendLine();

      statement.Append("UPDATE ");
      statement.Append(revisionTable);
      statement.Append(" SET ");
      statement.Append(revisionValueColumn);
      statement.Append(" = ");
      statement.Append(revisionValueParameter);
      statement.Append(" WHERE (");
      AppendKeyClause(statement, parameters, revisionKey, sqlDialect);
      statement.Append(")");
      statement.AppendLine();

      statement.Append("ELSE");
      statement.AppendLine();

      statement.Append("INSERT INTO ");
      statement.Append(revisionTable);
      statement.Append("(");
      statement.Append(GetRevisionGlobalKeyColumnIdentifier(sqlDialect));
      statement.Append(",");
      statement.Append(GetRevisionLocalKeyColumnIdentifier(sqlDialect));
      statement.Append(",");
      statement.Append(revisionValueColumn);
      statement.Append(") VALUES (");
      statement.Append(Assertion.IsNotNull(parameters[revisionGlobalKeyParameter], "parameters[revisionGlobalKeyParameter] != null").Name);
      statement.Append(",");
      statement.Append(Assertion.IsNotNull(parameters[revisionLocalKeyParameter], "parameters[revisionLocalKeyParameter] != null").Name);
      statement.Append(",");
      statement.Append(revisionValueParameter);
      statement.Append(")");
      statement.Append(sqlDialect.StatementDelimiter);
      statement.AppendLine();
      statement.Append("COMMIT TRANSACTION " + incrementrevision);
      statement.Append(sqlDialect.StatementDelimiter);
      statement.AppendLine();

      parameters.Add(revisionValueParameter, Guid.NewGuid());

      return QueryFactory.CreateQuery(
          new QueryDefinition(
              typeof(Revision) + "." + MethodBase.GetCurrentMethod()!.Name,
              storageProviderDefinition,
              statement.ToString(),
              QueryType.ScalarReadWrite),
          parameters);
    }

    private static void AppendKeyClause (StringBuilder statement, QueryParameterCollection parameters, IRevisionKey key, ISqlDialect sqlDialect)
    {
      string revisionGlobalKeyColumn = GetRevisionGlobalKeyColumnIdentifier(sqlDialect);
      string revisionLocalKeyColumn = GetRevisionLocalKeyColumnIdentifier(sqlDialect);
      string revisionGlobalKeyParameter = sqlDialect.GetParameterName("globalKey");
      string revisionLocalKeyParameter = sqlDialect.GetParameterName("localKey");

      statement.Append(revisionGlobalKeyColumn);
      statement.Append(" = ");
      statement.Append(revisionGlobalKeyParameter);
      if (!parameters.Contains(revisionGlobalKeyParameter))
        parameters.Add(new QueryParameter(revisionGlobalKeyParameter, key.GlobalKey));

      statement.Append(" AND ");
      statement.Append(revisionLocalKeyColumn);
      if (string.IsNullOrEmpty(key.LocalKey))
      {
        statement.Append(" IS NULL");
        if (!parameters.Contains(revisionLocalKeyParameter))
          parameters.Add(new QueryParameter(revisionLocalKeyParameter, null));
      }
      else
      {
        statement.Append(" = ");
        statement.Append(revisionLocalKeyParameter);
        if (!parameters.Contains(revisionLocalKeyParameter))
          parameters.Add(new QueryParameter(revisionLocalKeyParameter, key.LocalKey));
      }
    }

    private static RdbmsProviderDefinition GetStorageProviderDefinition ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(SecurableClassDefinition));
      return (RdbmsProviderDefinition)classDefinition.StorageEntityDefinition.StorageProviderDefinition;
    }

    private static string GetRevisionTableIdentifier (ISqlDialect sqlDialect)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(SecurableClassDefinition));
      var tableDefinition = (TableDefinition)classDefinition.StorageEntityDefinition;

      if (tableDefinition.TableName.SchemaName == null)
        return sqlDialect.DelimitIdentifier("Revision");
      else
        return sqlDialect.DelimitIdentifier(tableDefinition.TableName.SchemaName) + "." + sqlDialect.DelimitIdentifier("Revision");
    }

    private static string GetRevisionGlobalKeyColumnIdentifier (ISqlDialect sqlDialect)
    {
      return sqlDialect.DelimitIdentifier("GlobalKey");
    }

    private static string GetRevisionLocalKeyColumnIdentifier (ISqlDialect sqlDialect)
    {
      return sqlDialect.DelimitIdentifier("LocalKey");
    }

    private static string GetRevisionValueColumnIdentifier (ISqlDialect sqlDialect)
    {
      return sqlDialect.DelimitIdentifier("Value");
    }
  }
}
