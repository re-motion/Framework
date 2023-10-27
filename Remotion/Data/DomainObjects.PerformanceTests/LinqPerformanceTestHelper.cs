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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class LinqPerformanceTestHelper<T>
  {
    private readonly IQueryParser _queryParser = QueryParser.CreateDefault();
    private readonly CompoundMethodCallTransformerProvider _methodCallTransformerProvider = CompoundMethodCallTransformerProvider.CreateDefault();
    private readonly ResultOperatorHandlerRegistry _resultOperatorHandlerRegistry = ResultOperatorHandlerRegistry.CreateDefault();

    private readonly Func<IQueryable<T>> _queryGenerator;

    public LinqPerformanceTestHelper (Func<IQueryable<T>> queryGenerator)
    {
      ArgumentUtility.CheckNotNull("queryGenerator", queryGenerator);

      _queryGenerator = queryGenerator;
    }

    public bool GenerateQueryModel ()
    {
      var queryable = _queryGenerator();
      return _queryParser.GetParsedQuery(queryable.Expression) != null;
    }

    public bool GenerateQueryModelAndSQL ()
    {
      var storageProviderDefinition =
          (RdbmsProviderDefinition)MappingConfiguration.Current.GetTypeDefinition(typeof(Client)).StorageEntityDefinition.StorageProviderDefinition;
      var storageObjectFactory = (SqlStorageObjectFactory)storageProviderDefinition.Factory;
      var sqlQueryGenerator =
          storageObjectFactory.CreateSqlQueryGenerator(storageProviderDefinition, _methodCallTransformerProvider, _resultOperatorHandlerRegistry);

      var queryable = _queryGenerator();
      var queryModel = _queryParser.GetParsedQuery(queryable.Expression);

      var result = sqlQueryGenerator.CreateSqlQuery(queryModel);

      var sqlStatement = result.SqlCommand.CommandText;
      return !string.IsNullOrEmpty(sqlStatement);
    }

    public bool GenerateQueryModelAndSQLAndIQuery ()
    {
      var query = _queryGenerator();
      return QueryFactory.CreateQuery<T>("perftest", query) != null;
    }

    public bool GenerateAndExecuteQueryDBOnly ()
    {
      var query = _queryGenerator();
      var restoreQuery = QueryFactory.CreateQuery<T>("perftest", query);

      using (var manager = new StorageProviderManager(NullPersistenceExtension.Instance))
      {
        return manager.GetMandatory("PerformanceTestDomain").ExecuteCollectionQuery(restoreQuery).ToArray().Length > 100;
      }
    }

    public bool GenerateAndExecuteQuery ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var query = _queryGenerator();
        var recordCount = query.ToList().Count;
        return recordCount > 100;
      }
    }
  }
}
