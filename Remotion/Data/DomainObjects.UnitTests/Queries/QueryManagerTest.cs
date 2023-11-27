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
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryManagerTest : ClientTransactionBaseTest
  {
    private QueryManager _queryManager;

    private Mock<IPersistenceStrategy> _persistenceStrategyMock;
    private Mock<IObjectLoader> _objectLoaderMock;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private IQuery _collectionQuery;
    private IQuery _scalarQuery;

    private Order _fakeOrder1;
    private Order _fakeOrder2;
    private Mock<ILoadedObjectData> _loadedObjectDataStub1;
    private Mock<ILoadedObjectData> _loadedObjectDataStub2;

    Func<IQueryResultRow, object> _rowConversion;
    private IQuery _customQuery;

    public override void SetUp ()
    {
      base.SetUp();

      _persistenceStrategyMock = new Mock<IPersistenceStrategy>(MockBehavior.Strict);
      _objectLoaderMock = new Mock<IObjectLoader>(MockBehavior.Strict);
      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _queryManager = new QueryManager(
          _persistenceStrategyMock.Object,
          _objectLoaderMock.Object,
          _transactionEventSinkWithMock.Object);

      _collectionQuery = QueryFactory.CreateQuery(Queries.GetMandatory("OrderQuery"));
      _scalarQuery = QueryFactory.CreateQuery(Queries.GetMandatory("OrderNoSumByCustomerNameQuery"));
      _customQuery = QueryFactory.CreateQuery(Queries.GetMandatory("CustomQuery"));

      _fakeOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _fakeOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _loadedObjectDataStub1 = new Mock<ILoadedObjectData>();
      _loadedObjectDataStub2 = new Mock<ILoadedObjectData>();

      _rowConversion = qrr => qrr.GetRawValue(0);
    }

    [Test]
    public void GetScalar ()
    {
      _persistenceStrategyMock.Setup(mock => mock.ExecuteScalarQuery(_scalarQuery)).Returns(27).Verifiable();

      var result = _queryManager.GetScalar(_scalarQuery);

      _persistenceStrategyMock.Verify();
      Assert.That(result, Is.EqualTo(27));
    }

    [Test]
    public void GetScalar_WithCollectionQuery ()
    {
      Assert.That(
          () => _queryManager.GetScalar(_collectionQuery),
          Throws.ArgumentException);
    }

    [Test]
    public void GetCollection ()
    {
      _loadedObjectDataStub1.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder1);
      _loadedObjectDataStub2.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder2);

      _transactionEventSinkWithMock
          .Setup(stub => stub.RaiseFilterQueryResultEvent(It.IsAny<QueryResult<Order>>()))
          .Returns((QueryResult<Order> queryResult) => queryResult);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadCollectionQueryResult(_collectionQuery))
          .Returns(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object })
          .Verifiable();

      var result = _queryManager.GetCollection<Order>(_collectionQuery);

      _objectLoaderMock.Verify();
      Assert.That(result.AsEnumerable(), Is.EqualTo(new[] { _fakeOrder1, _fakeOrder2 }));
    }

    [Test]
    public void GetCollection_WithNull ()
    {
      _loadedObjectDataStub1.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder1);
      _loadedObjectDataStub2.Setup(stub => stub.GetDomainObjectReference()).Returns((DomainObject)null);

      _transactionEventSinkWithMock
          .Setup(stub => stub.RaiseFilterQueryResultEvent(It.IsAny<QueryResult<Order>>()))
          .Returns((QueryResult<Order> queryResult) => queryResult);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadCollectionQueryResult(_collectionQuery))
          .Returns(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object })
          .Verifiable();

      var result = _queryManager.GetCollection<Order>(_collectionQuery);

      _objectLoaderMock.Verify();
      Assert.That(result.AsEnumerable(), Is.EqualTo(new[] { _fakeOrder1, null }));
    }

    [Test]
    public void GetCollection_WithCastProblem ()
    {
      _loadedObjectDataStub1.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder1);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadCollectionQueryResult(_collectionQuery))
          .Returns(new[] { _loadedObjectDataStub1.Object })
          .Verifiable();

      Assert.That(
          () => _queryManager.GetCollection<Customer>(_collectionQuery),
          Throws.TypeOf<UnexpectedQueryResultException>().With.Message.EqualTo(
            "The query returned an object of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order', but a query result of type "
            + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer' was expected."));
    }

    [Test]
    public void GetCollection_CallsFilterQueryResult ()
    {
      _loadedObjectDataStub1.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder1);
      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadCollectionQueryResult(_collectionQuery))
          .Returns(new[] { _loadedObjectDataStub1.Object });

      var filteredResult = new QueryResult<Order>(_collectionQuery, new[] { _fakeOrder2 });
      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseFilterQueryResultEvent(It.Is<QueryResult<Order>>(qr => qr.ToArray().SequenceEqual(new[] { _fakeOrder1 }))))
          .Returns(filteredResult)
          .Verifiable();

      var result = _queryManager.GetCollection<Order>(_collectionQuery);

      _transactionEventSinkWithMock.Verify();
      Assert.That(result, Is.SameAs(filteredResult));
    }

    [Test]
    public void GetCollection_WithScalarQuery ()
    {
      Assert.That(
          () => _queryManager.GetCollection(_scalarQuery),
          Throws.ArgumentException);
    }

    [Test]
    public void GetCollection_NonGeneric ()
    {
      _loadedObjectDataStub1.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder1);
      _loadedObjectDataStub2.Setup(stub => stub.GetDomainObjectReference()).Returns(_fakeOrder2);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadCollectionQueryResult(_collectionQuery))
          .Returns(new[] { _loadedObjectDataStub1.Object, _loadedObjectDataStub2.Object })
          .Verifiable();

      var filteredResult = new QueryResult<DomainObject>(_collectionQuery, new[] { _fakeOrder2 });
      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseFilterQueryResultEvent(It.Is<QueryResult<DomainObject>>(qr => qr.ToArray().SequenceEqual(new[] { _fakeOrder1, _fakeOrder2 }))))
          .Returns(filteredResult)
          .Verifiable();

      var result = _queryManager.GetCollection(_collectionQuery);

      _objectLoaderMock.Verify();
      _transactionEventSinkWithMock.Verify();

      Assert.That(result, Is.SameAs(filteredResult));
    }

    [Test]
    public void GetCustom_WithNonCustomQuery ()
    {
      Assert.That(
          () => _queryManager.GetCustom(_collectionQuery, _rowConversion),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("A collection or scalar query cannot be used with GetCustom.", "query"));
    }

    [Test]
    public void GetCustom_WithEagerFetchQueries ()
    {
      var relationEndPointDefinitionStub = new Mock<IRelationEndPointDefinition>();
      _customQuery.EagerFetchQueries.Add(relationEndPointDefinitionStub.Object, _scalarQuery);
      Assert.That(
          () => _queryManager.GetCustom(_customQuery, _rowConversion),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("A custom query cannot have eager fetch queries defined.", "query"));
    }

    [Test]
    public void GetCustom ()
    {
      var fakeRow1 = new Mock<IQueryResultRow>();
      fakeRow1.Setup(stub => stub.GetRawValue(0)).Returns("Fake1");
      var fakeRow2 = new Mock<IQueryResultRow>();
      fakeRow2.Setup(stub => stub.GetRawValue(0)).Returns("Fake2");

      var fakeResult = new[] { fakeRow1.Object, fakeRow2.Object };

      _persistenceStrategyMock
          .Setup(mock => mock.ExecuteCustomQuery(_customQuery))
          .Returns(fakeResult)
          .Verifiable();

      _transactionEventSinkWithMock
          .Setup(stub => stub.RaiseFilterCustomQueryResultEvent(_customQuery, new object[] { "Fake1", "Fake2" }))
          .Returns((IQuery _, IEnumerable<object> results) => results)
          .Verifiable();

      var result = _queryManager.GetCustom(_customQuery, _rowConversion);

      _persistenceStrategyMock.Verify();
      _transactionEventSinkWithMock.Verify();
      Assert.That(result.ToArray(), Is.EqualTo(new[] { "Fake1", "Fake2" }));
    }

    [Test]
    public void Serialization ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var queryManager = ClientTransactionScope.CurrentTransaction.QueryManager;

      var deserializedQueryManager = Serializer.SerializeAndDeserialize(queryManager);

      Assert.That(deserializedQueryManager, Is.Not.Null);
    }
  }
}
