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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryFactoryTest : StandardMappingTest
  {
    public override void TearDown ()
    {
      ResetCaches();
      base.TearDown();
    }

    [Test]
    public void CreateQuery_FromDefinition ()
    {
      var definition = new QueryDefinition("Test", TestDomainStorageProviderDefinition, "y", QueryType.CollectionReadWrite, typeof(OrderCollection));

      IQuery query = QueryFactory.CreateQuery(definition);
      Assert.That(query.CollectionType, Is.EqualTo(definition.CollectionType));
      Assert.That(query.ID, Is.EqualTo(definition.ID));
      Assert.That(query.Parameters, Is.Empty);
      Assert.That(query.QueryType, Is.EqualTo(definition.QueryType));
      Assert.That(query.Statement, Is.EqualTo(definition.Statement));
      Assert.That(query.StorageProviderDefinition, Is.SameAs(definition.StorageProviderDefinition));
    }

    [Test]
    public void CreateQuery_FromDefinition_WithParameterCollection ()
    {
      var definition = new QueryDefinition("Test", TestDomainStorageProviderDefinition, "y", QueryType.CollectionReadWrite, typeof(OrderCollection));
      var parameterCollection = new QueryParameterCollection();

      IQuery query = QueryFactory.CreateQuery(definition, parameterCollection);
      Assert.That(query.CollectionType, Is.EqualTo(definition.CollectionType));
      Assert.That(query.ID, Is.EqualTo(definition.ID));
      Assert.That(query.Parameters, Is.SameAs(parameterCollection));
      Assert.That(query.QueryType, Is.EqualTo(definition.QueryType));
      Assert.That(query.Statement, Is.EqualTo(definition.Statement));
      Assert.That(query.StorageProviderDefinition, Is.SameAs(definition.StorageProviderDefinition));
    }

    [Test]
    public void CreateQueryFromConfiguration_FromID ()
    {
      var definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions[0];

      IQuery query = QueryFactory.CreateQueryFromConfiguration(definition.ID);
      Assert.That(query.CollectionType, Is.EqualTo(definition.CollectionType));
      Assert.That(query.ID, Is.EqualTo(definition.ID));
      Assert.That(query.Parameters, Is.Empty);
      Assert.That(query.QueryType, Is.EqualTo(definition.QueryType));
      Assert.That(query.Statement, Is.EqualTo(definition.Statement));
      Assert.That(query.StorageProviderDefinition, Is.SameAs(definition.StorageProviderDefinition));
    }

    [Test]
    public void CreateQueryFromConfiguration_FromID_WithParameterCollection ()
    {
      var definition = DomainObjectsConfiguration.Current.Query.QueryDefinitions[0];
      var parameterCollection = new QueryParameterCollection();

      IQuery query = QueryFactory.CreateQueryFromConfiguration(definition.ID, parameterCollection);
      Assert.That(query.CollectionType, Is.EqualTo(definition.CollectionType));
      Assert.That(query.ID, Is.EqualTo(definition.ID));
      Assert.That(query.Parameters, Is.SameAs(parameterCollection));
      Assert.That(query.QueryType, Is.EqualTo(definition.QueryType));
      Assert.That(query.Statement, Is.EqualTo(definition.Statement));
      Assert.That(query.StorageProviderDefinition, Is.EqualTo(definition.StorageProviderDefinition));
    }

    [Test]
    public void CreateScalarQuery ()
    {
      var id = "id";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection();

      IQuery query = QueryFactory.CreateScalarQuery(id, TestDomainStorageProviderDefinition, statement, parameterCollection);
      Assert.That(query.CollectionType, Is.Null);
      Assert.That(query.ID, Is.EqualTo(id));
      Assert.That(query.Parameters, Is.SameAs(parameterCollection));
      Assert.That(query.QueryType, Is.EqualTo(QueryType.ScalarReadOnly));
      Assert.That(query.Statement, Is.EqualTo(statement));
      Assert.That(query.StorageProviderDefinition, Is.SameAs(TestDomainStorageProviderDefinition));
    }

    [Test]
    public void CreateCollectionQuery ()
    {
      var id = "id";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection();
      var collectionType = typeof(OrderCollection);

      IQuery query = QueryFactory.CreateCollectionQuery(id, TestDomainStorageProviderDefinition, statement, parameterCollection, collectionType);
      Assert.That(query.ID, Is.EqualTo(id));
      Assert.That(query.CollectionType, Is.SameAs(collectionType));
      Assert.That(query.Parameters, Is.SameAs(parameterCollection));
      Assert.That(query.QueryType, Is.EqualTo(QueryType.CollectionReadOnly));
      Assert.That(query.Statement, Is.EqualTo(statement));
      Assert.That(query.StorageProviderDefinition, Is.SameAs(TestDomainStorageProviderDefinition));
    }

    [Test]
    public void CreateCustomQuery ()
    {
      var id = "id";
      var statement = "stmt";
      var parameterCollection = new QueryParameterCollection();

      IQuery query = QueryFactory.CreateCustomQuery(id, TestDomainStorageProviderDefinition, statement, parameterCollection);
      Assert.That(query.ID, Is.EqualTo(id));
      Assert.That(query.CollectionType, Is.Null);
      Assert.That(query.Parameters, Is.SameAs(parameterCollection));
      Assert.That(query.QueryType, Is.EqualTo(QueryType.CustomReadOnly));
      Assert.That(query.Statement, Is.EqualTo(statement));
      Assert.That(query.StorageProviderDefinition, Is.SameAs(TestDomainStorageProviderDefinition));
    }

    [Test]
    public void CreateQuery_FromLinqQuery ()
    {
      var queryable = from o in QueryFactory.CreateLinqQuery<Order>()
                      where o.OrderNumber > 1
                      select o;

      IQuery query = QueryFactory.CreateQuery<Order>("<dynamico queryo>", queryable);
      Assert.That(query.Statement, Is.EqualTo(
        "SELECT [t0].[ID],[t0].[ClassID],[t0].[Timestamp],[t0].[OrderNo],[t0].[DeliveryDate],[t0].[OfficialID],[t0].[CustomerID],[t0].[CustomerIDClassID] "
        +"FROM [OrderView] AS [t0] WHERE ([t0].[OrderNo] > @1)"));
      Assert.That(query.Parameters.Count, Is.EqualTo(1));
      Assert.That(query.ID, Is.EqualTo("<dynamico queryo>"));
      Assert.That(query.QueryType, Is.EqualTo(QueryType.CollectionReadOnly));
      Assert.That(query.StorageProviderDefinition, Is.EqualTo(TestDomainStorageProviderDefinition));
    }

    [Test]
    public void CreateQuery_FromLinqQuery_WithEagerFetching ()
    {
      var queryable = (from o in QueryFactory.CreateLinqQuery<Order>()
                       where o.OrderNumber > 1
                       select o).FetchMany(o => o.OrderItems);

      IQuery query = QueryFactory.CreateQuery<Order>("<dynamico queryo>", queryable);
      Assert.That(query.EagerFetchQueries.Count, Is.EqualTo(1));
      Assert.That(query.EagerFetchQueries.Single().Key.PropertyName, Is.EqualTo(typeof(Order).FullName + ".OrderItems"));
    }

    [Test]
    public void CreateQuery_FromLinqQuery_InvalidQueryable ()
    {
      var queryable = new int[0].AsQueryable();
      Assert.That(
          () => QueryFactory.CreateQuery<int>("<dynamic query>", queryable),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given queryable must stem from an instance of DomainObjectQueryable. Instead, "
                  +
                  "it is of type 'EnumerableQuery`1', with a query provider of type 'EnumerableQuery`1'. Be sure to use QueryFactory.CreateLinqQuery to "
                  + "create the queryable instance, and only use standard query methods on it.", "queryable"));
    }

    [Test]
    public void CreateLinqQuery_WithParserAndExecutor ()
    {
      var factoryMock = new Mock<ILinqProviderComponentFactory>(MockBehavior.Strict);
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<ILinqProviderComponentFactory>(() => factoryMock.Object);
      using (new ServiceLocatorScope(serviceLocator))
      {
        var executorStub = new Mock<IQueryExecutor>();
        var queryParserStub = new Mock<IQueryParser>();
        var fakeResult = new Mock<IQueryable<Order>>();

        factoryMock
            .Setup(mock => mock.CreateQueryable<Order>(queryParserStub.Object, executorStub.Object))
            .Returns(fakeResult.Object)
            .Verifiable();

        var result = QueryFactory.CreateLinqQuery<Order>(queryParserStub.Object, executorStub.Object);

        factoryMock.Verify();
        Assert.That(result, Is.SameAs(fakeResult.Object));
      }
    }

    [Test]
    public void CreateLinqQuery_WithoutParserAndExecutor ()
    {
      var factoryMock = new Mock<ILinqProviderComponentFactory>(MockBehavior.Strict);
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<ILinqProviderComponentFactory>(() => factoryMock.Object);
      using (new ServiceLocatorScope(serviceLocator))
      {
        var fakeExecutor = new Mock<IQueryExecutor>();
        var fakeQueryParser = new Mock<IQueryParser>();
        var fakeResult = new Mock<IQueryable<Order>>();

        factoryMock
            .Setup(mock => mock.CreateQueryExecutor(TestDomainStorageProviderDefinition))
            .Returns(fakeExecutor.Object)
            .Verifiable();
        factoryMock
            .Setup(mock => mock.CreateQueryParser())
            .Returns(fakeQueryParser.Object)
            .Verifiable();
        factoryMock
            .Setup(mock => mock.CreateQueryable<Order>(fakeQueryParser.Object, fakeExecutor.Object))
            .Returns(fakeResult.Object)
            .Verifiable();

        var result = QueryFactory.CreateLinqQuery<Order>();

        factoryMock.Verify();
        Assert.That(result, Is.SameAs(fakeResult.Object));
      }
    }

    private void ResetCaches ()
    {
      var linqProviderComponentFactoryCache =
          (DoubleCheckedLockingContainer<ILinqProviderComponentFactory>)PrivateInvoke.GetNonPublicStaticField(typeof(QueryFactory), "s_linqProviderComponentFactory");
      linqProviderComponentFactoryCache.Value = null;

      var queryParserCache =
          (DoubleCheckedLockingContainer<IQueryParser>)PrivateInvoke.GetNonPublicStaticField(typeof(QueryFactory), "s_queryParser");
      queryParserCache.Value = null;
    }
  }
}
