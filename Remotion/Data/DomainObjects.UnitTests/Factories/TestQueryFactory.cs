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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class TestQueryFactory
  {
    public static QueryDefinition CreateOrderQueryWithCustomCollectionType ()
    {
      return new QueryDefinition(
          "OrderQueryWithCustomCollectionType",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.CollectionReadOnly,
          typeof(OrderCollection));
    }

    public static QueryDefinition CreateOrderQueryDefinitionWithObjectListOfOrder ()
    {
      return new QueryDefinition(
          "OrderQueryWithObjectListOfOrder",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.CollectionReadOnly,
          typeof(ObjectList<Order>));
    }

    public static QueryDefinition CreateCustomerTypeQueryDefinition ()
    {
      return new QueryDefinition(
          "CustomerTypeQuery",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select [Company].* from [Company] where [CustomerType] = @customerType order by [Name] asc;",
          QueryType.CollectionReadOnly,
          typeof(DomainObjectCollection));
    }

    public static QueryDefinition CreateOrderSumQueryDefinitionWithQueryTypeScalarReadOnly ()
    {
      return new QueryDefinition(
          "OrderSumQueryWithQueryTypeScalarReadOnly",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.ScalarReadOnly);
    }

    public static QueryDefinition CreateOrderSumQueryDefinitionWithQueryTypeCustomReadOnly ()
    {
      return new QueryDefinition(
          "OrderSumQueryWithQueryTypeCustomReadOnly",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.CustomReadOnly);
    }

    public static QueryDefinition CreateTestQueryDefinitionWithQueryTypeCollectionReadWrite ()
    {
      return new QueryDefinition(
          "TestQueryDefinitionWithQueryTypeCollectionReadWrite",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select [Company].* from [Company] where [CustomerType] = @customerType order by [Name] asc;",
          QueryType.CollectionReadWrite,
          typeof(DomainObjectCollection));
    }

    public static QueryDefinition CreateOrderSumQueryWithQueryTypeCustomReadOnly ()
    {
      return new QueryDefinition(
          "OrderSumQueryWithQueryTypeCustomReadOnly",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.CustomReadOnly);
    }

    public static QueryDefinition CreateTestQueryDefinitionWithQueryTypeScalarReadWrite ()
    {
      return new QueryDefinition(
          "TestQueryDefinitionWithQueryTypeScalarReadWrite",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select 0;",
          QueryType.ScalarReadWrite);
    }

    public static QueryDefinition CreateTestQueryDefinitionWithQueryTypeCustomReadWrite ()
    {
      return new QueryDefinition(
          "TestQueryDefinitionWithQueryTypeCustomReadWrite",
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID],
          "select 0;",
          QueryType.CustomReadWrite);
    }

    public static QueryResult<T> CreateTestQueryResult<T> () where T : DomainObject
    {
      var collection = new T[0];
      return CreateTestQueryResult(collection);
    }

    public static QueryResult<T> CreateTestQueryResult<T> (T[] collection) where T: DomainObject
    {
      var storageProviderDefinition =
          MappingConfiguration.Current.ContainsTypeDefinition(typeof(T))
              ? MappingConfiguration.Current.GetTypeDefinition(typeof(T)).StorageEntityDefinition.StorageProviderDefinition
              : DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      var query = QueryFactory.CreateCollectionQuery(
          "test", storageProviderDefinition, "TEST", new QueryParameterCollection(), typeof(DomainObjectCollection));
      return CreateTestQueryResult(query, collection);
    }

    public static QueryResult<T> CreateTestQueryResult<T> (IQuery query) where T : DomainObject
    {
      var collection = new T[0];
      return CreateTestQueryResult(query, collection);
    }

    public static QueryResult<T> CreateTestQueryResult<T> (IQuery query, T[] collection) where T : DomainObject
    {
      return new QueryResult<T>(query, collection);
    }
  }
}
