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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class EagerFetcherTest : StandardMappingTest
  {
    private Mock<IFetchedRelationDataRegistrationAgent> _registrationAgentMock;
    private Mock<IFetchEnabledObjectLoader> _fetchResultLoaderMock;

    private EagerFetcher _eagerFetcher;

    private Mock<IQuery> _fetchQueryStub1;
    private Mock<IQuery> _fetchQueryStub2;

    private IRelationEndPointDefinition _orderTicketEndPointDefinition;
    private IRelationEndPointDefinition _customerEndPointDefinition;
    private IRelationEndPointDefinition _industrialSectorEndPointDefinition;

    private Order _originatingOrder1;
    private Order _originatingOrder2;
    private Mock<ILoadedObjectData> _originatingOrderData1;
    private Mock<ILoadedObjectData> _originatingOrderData2;

    private OrderItem _fetchedOrderItem1;
    private OrderItem _fetchedOrderItem2;
    private OrderItem _fetchedOrderItem3;
    private LoadedObjectDataWithDataSourceData _fetchedOrderItemData1;
    private LoadedObjectDataWithDataSourceData _fetchedOrderItemData2;
    private LoadedObjectDataWithDataSourceData _fetchedOrderItemData3;

    private Customer _fetchedCustomer;
    private LoadedObjectDataWithDataSourceData _fetchedCustomerData;

    private LoadedObjectDataPendingRegistrationCollector _pendingRegistrationCollector;

    public override void SetUp ()
    {
      base.SetUp();

      _registrationAgentMock = new Mock<IFetchedRelationDataRegistrationAgent>(MockBehavior.Strict);
      _fetchResultLoaderMock = new Mock<IFetchEnabledObjectLoader>(MockBehavior.Strict);

      _eagerFetcher = new EagerFetcher(_registrationAgentMock.Object);

      _fetchQueryStub1 = new Mock<IQuery>();
      _fetchQueryStub2 = new Mock<IQuery>();

      _orderTicketEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");
      _industrialSectorEndPointDefinition = GetEndPointDefinition(typeof(Company), "IndustrialSector");

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _originatingOrderData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(_originatingOrder1);
      _originatingOrderData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(_originatingOrder2);

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem>();

      _fetchedOrderItemData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(_fetchedOrderItem1);
      _fetchedOrderItemData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(_fetchedOrderItem2);
      _fetchedOrderItemData3 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(_fetchedOrderItem3);

      _fetchedCustomer = DomainObjectMother.CreateFakeObject<Customer>();
      _fetchedCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(_fetchedCustomer);

      _pendingRegistrationCollector = new LoadedObjectDataPendingRegistrationCollector();
    }

    [Test]
    public void PerformEagerFetching ()
    {
      var fetchQueries = new EagerFetchQueryCollection
                          {
                              { _orderTicketEndPointDefinition, _fetchQueryStub1.Object },
                              { _customerEndPointDefinition, _fetchQueryStub2.Object }
                          };

      var originatingObjectsData = new[] { _originatingOrderData1.Object, _originatingOrderData2.Object };
      var relatedObjectsData1 = new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 };
      var relatedObjectsData2 = new[] { _fetchedCustomerData };

      _fetchResultLoaderMock
          .Setup(mock => mock.GetOrLoadFetchQueryResult(_fetchQueryStub1.Object, _pendingRegistrationCollector))
          .Returns(relatedObjectsData1)
          .Verifiable();
      _fetchResultLoaderMock
          .Setup(mock => mock.GetOrLoadFetchQueryResult(_fetchQueryStub2.Object, _pendingRegistrationCollector))
          .Returns(relatedObjectsData2)
          .Verifiable();

      _registrationAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(_orderTicketEndPointDefinition, originatingObjectsData, relatedObjectsData1))
          .Verifiable();
      _registrationAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(_customerEndPointDefinition, originatingObjectsData, relatedObjectsData2))
          .Verifiable();

      _eagerFetcher.PerformEagerFetching(originatingObjectsData, fetchQueries, _fetchResultLoaderMock.Object, _pendingRegistrationCollector);

      _fetchResultLoaderMock.Verify();
      _registrationAgentMock.Verify();
    }

    [Test]
    public void PerformEagerFetching_NestedEagerFetching_NotHandled ()
    {
      var fetchQueries = new EagerFetchQueryCollection { { _customerEndPointDefinition, _fetchQueryStub1.Object } };
      _fetchQueryStub1
          .Setup(stub => stub.EagerFetchQueries)
          .Returns(new EagerFetchQueryCollection { { _industrialSectorEndPointDefinition, _fetchQueryStub2.Object } });

      var originatingObjectsData = new[] { _originatingOrderData1.Object, _originatingOrderData2.Object };
      var relatedObjectsData = new[] { _fetchedCustomerData };

      _fetchResultLoaderMock
          .Setup(mock => mock.GetOrLoadFetchQueryResult(_fetchQueryStub1.Object, _pendingRegistrationCollector))
          .Returns(relatedObjectsData)
          .Verifiable();

      _registrationAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(_customerEndPointDefinition, originatingObjectsData, relatedObjectsData))
          .Verifiable();

      _eagerFetcher.PerformEagerFetching(originatingObjectsData, fetchQueries, _fetchResultLoaderMock.Object, _pendingRegistrationCollector);

      _fetchResultLoaderMock.Verify();
      _registrationAgentMock.Verify();
    }

    [Test]
    public void PerformEagerFetching_WithExceptionInAgent ()
    {
      var fetchQueries = new EagerFetchQueryCollection { { _orderTicketEndPointDefinition, _fetchQueryStub1.Object } };

      var originatingObjectsData = new[] { _originatingOrderData1.Object, _originatingOrderData2.Object };
      var relatedObjectsData = new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 };

      _fetchResultLoaderMock
          .Setup(mock => mock.GetOrLoadFetchQueryResult(_fetchQueryStub1.Object, _pendingRegistrationCollector))
          .Returns(relatedObjectsData)
          .Verifiable();

      var invalidOperationException = new InvalidOperationException("There was a problem registering stuff.");
      _registrationAgentMock
          .Setup(mock => mock.GroupAndRegisterRelatedObjects(_orderTicketEndPointDefinition, originatingObjectsData, relatedObjectsData))
          .Throws(invalidOperationException)
          .Verifiable();

      Assert.That(
          () =>
          _eagerFetcher.PerformEagerFetching(originatingObjectsData, fetchQueries, _fetchResultLoaderMock.Object, _pendingRegistrationCollector),
          Throws.Exception.TypeOf<UnexpectedQueryResultException>()
              .And.With.InnerException.SameAs(invalidOperationException)
              .And.With.Message.EqualTo("Eager fetching encountered an unexpected query result: There was a problem registering stuff."));

      _fetchResultLoaderMock.Verify();
      _registrationAgentMock.Verify();
    }

    [Test]
    public void PerformEagerFetching_NoOriginatingObjects ()
    {
      var fetchQueries = new EagerFetchQueryCollection { { _orderTicketEndPointDefinition, _fetchQueryStub1.Object } };

      var originatingObjectsData = new ILoadedObjectData[0];

      _eagerFetcher.PerformEagerFetching(originatingObjectsData, fetchQueries, _fetchResultLoaderMock.Object, _pendingRegistrationCollector);

      _fetchResultLoaderMock.Verify(mock => mock.GetOrLoadFetchQueryResult(_fetchQueryStub1.Object, _pendingRegistrationCollector), Times.Never());

      _fetchResultLoaderMock.Verify();
      _registrationAgentMock.Verify();
    }
  }
}
