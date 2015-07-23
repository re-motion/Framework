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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.EagerFetching
{
  [TestFixture]
  public class EagerFetcherTest : StandardMappingTest
  {
    private IFetchedRelationDataRegistrationAgent _registrationAgentMock;
    private IFetchEnabledObjectLoader _fetchResultLoaderMock;

    private EagerFetcher _eagerFetcher;

    private IQuery _fetchQueryStub1;
    private IQuery _fetchQueryStub2;

    private IRelationEndPointDefinition _orderTicketEndPointDefinition;
    private IRelationEndPointDefinition _customerEndPointDefinition;
    private IRelationEndPointDefinition _industrialSectorEndPointDefinition;

    private Order _originatingOrder1;
    private Order _originatingOrder2;
    private ILoadedObjectData _originatingOrderData1;
    private ILoadedObjectData _originatingOrderData2;

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

      _registrationAgentMock = MockRepository.GenerateStrictMock<IFetchedRelationDataRegistrationAgent>();
      _fetchResultLoaderMock = MockRepository.GenerateStrictMock<IFetchEnabledObjectLoader>();

      _eagerFetcher = new EagerFetcher (_registrationAgentMock);

      _fetchQueryStub1 = MockRepository.GenerateStub<IQuery> ();
      _fetchQueryStub2 = MockRepository.GenerateStub<IQuery> ();

      _orderTicketEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");
      _industrialSectorEndPointDefinition = GetEndPointDefinition (typeof (Company), "IndustrialSector");

      _originatingOrder1 = DomainObjectMother.CreateFakeObject<Order>();
      _originatingOrder2 = DomainObjectMother.CreateFakeObject<Order>();

      _originatingOrderData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder1);
      _originatingOrderData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (_originatingOrder2);

      _fetchedOrderItem1 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem2 = DomainObjectMother.CreateFakeObject<OrderItem>();
      _fetchedOrderItem3 = DomainObjectMother.CreateFakeObject<OrderItem>();

      _fetchedOrderItemData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (_fetchedOrderItem1);
      _fetchedOrderItemData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (_fetchedOrderItem2);
      _fetchedOrderItemData3 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (_fetchedOrderItem3);

      _fetchedCustomer = DomainObjectMother.CreateFakeObject<Customer>();
      _fetchedCustomerData = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (_fetchedCustomer);

      _pendingRegistrationCollector = new LoadedObjectDataPendingRegistrationCollector();
    }

    [Test]
    public void PerformEagerFetching ()
    {
      var fetchQueries = new EagerFetchQueryCollection
                          {
                              { _orderTicketEndPointDefinition, _fetchQueryStub1 },
                              { _customerEndPointDefinition, _fetchQueryStub2 }
                          };
      
      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };
      var relatedObjectsData1 = new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 };
      var relatedObjectsData2 = new[] { _fetchedCustomerData };

      _fetchResultLoaderMock
          .Expect (mock => mock.GetOrLoadFetchQueryResult (_fetchQueryStub1, _pendingRegistrationCollector))
          .Return (relatedObjectsData1);
      _fetchResultLoaderMock
          .Expect (mock => mock.GetOrLoadFetchQueryResult (_fetchQueryStub2, _pendingRegistrationCollector))
          .Return (relatedObjectsData2);
      _fetchResultLoaderMock.Replay();

      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_orderTicketEndPointDefinition, originatingObjectsData, relatedObjectsData1));
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_customerEndPointDefinition, originatingObjectsData, relatedObjectsData2));
      _registrationAgentMock.Replay();

      _eagerFetcher.PerformEagerFetching (originatingObjectsData, fetchQueries, _fetchResultLoaderMock, _pendingRegistrationCollector);

      _fetchResultLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_NestedEagerFetching_NotHandled ()
    {
      var fetchQueries = new EagerFetchQueryCollection { { _customerEndPointDefinition, _fetchQueryStub1 } };
      _fetchQueryStub1
          .Stub (stub => stub.EagerFetchQueries)
          .Return (new EagerFetchQueryCollection { { _industrialSectorEndPointDefinition, _fetchQueryStub2 } });

      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };
      var relatedObjectsData = new[] { _fetchedCustomerData };

      _fetchResultLoaderMock
          .Expect (mock => mock.GetOrLoadFetchQueryResult (_fetchQueryStub1, _pendingRegistrationCollector))
          .Return (relatedObjectsData);
      _fetchResultLoaderMock.Replay ();

      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_customerEndPointDefinition, originatingObjectsData, relatedObjectsData));
      _registrationAgentMock.Replay ();

      _eagerFetcher.PerformEagerFetching (originatingObjectsData, fetchQueries, _fetchResultLoaderMock, _pendingRegistrationCollector);

      _fetchResultLoaderMock.VerifyAllExpectations ();
      _registrationAgentMock.VerifyAllExpectations ();
    }

    [Test]
    public void PerformEagerFetching_WithExceptionInAgent ()
    {
      var fetchQueries = new EagerFetchQueryCollection { { _orderTicketEndPointDefinition, _fetchQueryStub1 } };

      var originatingObjectsData = new[] { _originatingOrderData1, _originatingOrderData2 };
      var relatedObjectsData = new[] { _fetchedOrderItemData1, _fetchedOrderItemData2, _fetchedOrderItemData3 };

      _fetchResultLoaderMock
          .Expect (mock => mock.GetOrLoadFetchQueryResult (_fetchQueryStub1, _pendingRegistrationCollector))
          .Return (relatedObjectsData);
      _fetchResultLoaderMock.Replay();

      var invalidOperationException = new InvalidOperationException ("There was a problem registering stuff.");
      _registrationAgentMock
          .Expect (mock => mock.GroupAndRegisterRelatedObjects (_orderTicketEndPointDefinition, originatingObjectsData, relatedObjectsData))
          .Throw (invalidOperationException);
      _registrationAgentMock.Replay();

      Assert.That (
          () =>
          _eagerFetcher.PerformEagerFetching (originatingObjectsData, fetchQueries, _fetchResultLoaderMock, _pendingRegistrationCollector),
          Throws.Exception.TypeOf<UnexpectedQueryResultException> ()
              .And.With.InnerException.SameAs (invalidOperationException)
              .And.With.Message.EqualTo ("Eager fetching encountered an unexpected query result: There was a problem registering stuff."));

      _fetchResultLoaderMock.VerifyAllExpectations();
      _registrationAgentMock.VerifyAllExpectations();
    }

    [Test]
    public void PerformEagerFetching_NoOriginatingObjects ()
    {
      var fetchQueries = new EagerFetchQueryCollection { { _orderTicketEndPointDefinition, _fetchQueryStub1 } };

      var originatingObjectsData = new ILoadedObjectData[0];

      _fetchResultLoaderMock.Replay ();
      _registrationAgentMock.Replay ();

      _eagerFetcher.PerformEagerFetching (originatingObjectsData, fetchQueries, _fetchResultLoaderMock, _pendingRegistrationCollector);

      _fetchResultLoaderMock.AssertWasNotCalled (mock => mock.GetOrLoadFetchQueryResult (_fetchQueryStub1, _pendingRegistrationCollector));

      _fetchResultLoaderMock.VerifyAllExpectations ();
      _registrationAgentMock.VerifyAllExpectations ();
    }

    [Test]
    public void Serialization ()
    {
      var instance = new EagerFetcher (new SerializableFetchedRelationDataRegistrationAgentFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.RegistrationAgent, Is.Not.Null);
    }
  }
}