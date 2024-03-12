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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class TestQueryFactory
  {
    public static QueryDefinition CreateOrderQueryWithCustomCollectionType (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "OrderQueryWithCustomCollectionType",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.CollectionReadOnly,
          typeof(OrderCollection));
    }

    public static QueryDefinition CreateOrderQueryDefinitionWithObjectListOfOrder (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "OrderQueryWithObjectListOfOrder",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.CollectionReadOnly,
          typeof(ObjectList<Order>));
    }

    public static QueryDefinition CreateCustomerTypeQueryDefinition (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "CustomerTypeQuery",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select [Company].* from [Company] where [CustomerType] = @customerType order by [Name] asc;",
          QueryType.CollectionReadOnly,
          typeof(DomainObjectCollection));
    }

    public static QueryDefinition CreateOrderSumQueryDefinitionWithQueryTypeScalarReadOnly (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "OrderSumQueryWithQueryTypeScalarReadOnly",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.ScalarReadOnly);
    }

    public static QueryDefinition CreateOrderSumQueryDefinitionWithQueryTypeCustomReadOnly (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "OrderSumQueryWithQueryTypeCustomReadOnly",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.CustomReadOnly);
    }

    public static QueryDefinition CreateTestQueryWithQueryTypeCollectionReadWrite (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "TestQueryDefinitionWithQueryTypeCollectionReadWrite",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select [Company].* from [Company] where [CustomerType] = @customerType order by [Name] asc;",
          QueryType.CollectionReadWrite,
          typeof(DomainObjectCollection));
    }

    public static QueryDefinition CreateTestQueryWithQueryTypeCustomReadWrite (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "TestQueryDefinitionWithQueryTypeCustomReadWrite",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select 0;",
          QueryType.CustomReadWrite);
    }

    public static QueryDefinition CreateTestQueryDefinitionWithQueryTypeScalarReadWrite (IStorageSettings storageSettings)
    {
      return new QueryDefinition(
          "TestQueryDefinitionWithQueryTypeScalarReadWrite",
          storageSettings.GetStorageProviderDefinition(DatabaseTest.c_testDomainProviderID),
          "select 0;",
          QueryType.ScalarReadWrite);
    }

    public static QueryResult<DomainObject> CreateTestQueryResult (IStorageSettings storageSettings)
    {
      var collection = new DomainObject[0];
      return CreateTestQueryResult(storageSettings, collection);
    }

    public static QueryResult<DomainObject> CreateTestQueryResult (IStorageSettings storageSettings, DomainObject[] collection)
    {
      var storageProviderDefinition = storageSettings.GetDefaultStorageProviderDefinition();
      var query = QueryFactory.CreateCollectionQuery(
          "test", storageProviderDefinition, "TEST", new QueryParameterCollection(), typeof(DomainObjectCollection));
      return CreateTestQueryResult(storageSettings, query, collection);
    }

    public static QueryResult<DomainObject> CreateTestQueryResult (IStorageSettings storageSettings, IQuery query)
    {
      var collection = new DomainObject[0];
      return CreateTestQueryResult(storageSettings, query, collection);
    }

    public static QueryResult<DomainObject> CreateTestQueryResult (IStorageSettings storageSettings, IQuery query, DomainObject[] collection)
    {
      return new QueryResult<DomainObject>(query, collection);
    }
  }
}
