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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class FetchEnabledObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IFetchEnabledPersistenceStrategy _persistenceStrategyMock;
    private ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgentMock;
    private ILoadedObjectDataProvider _loadedObjectDataProviderStub;
    private IEagerFetcher _eagerFetcherMock;

    private FetchEnabledObjectLoader _fetchEnabledObjectLoader;
    
    private ILoadedObjectData _resultItem1;
    private ILoadedObjectData _resultItem2;
    private LoadedObjectDataWithDataSourceData _resultItemWithSourceData1;
    private LoadedObjectDataWithDataSourceData _resultItemWithSourceData2;
    private IRelationEndPointDefinition _orderTicketEndPointDefinition;
    private IRelationEndPointDefinition _customerEndPointDefinition;

    private IQuery _queryWithFetchQueries;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IFetchEnabledPersistenceStrategy> ();
      _loadedObjectDataRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectDataRegistrationAgent> ();
      _loadedObjectDataProviderStub = _mockRepository.Stub<ILoadedObjectDataProvider> ();
      _eagerFetcherMock = _mockRepository.StrictMock<IEagerFetcher> ();

      _fetchEnabledObjectLoader = new FetchEnabledObjectLoader (
          _persistenceStrategyMock,
          _loadedObjectDataRegistrationAgentMock,
          _loadedObjectDataProviderStub,
          _eagerFetcherMock);

      _resultItem1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order1);
      _resultItem2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order3);
      _resultItemWithSourceData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order1);
      _resultItemWithSourceData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData (DomainObjectIDs.Order3);

      _orderTicketEndPointDefinition = GetEndPointDefinition (typeof (Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition (typeof (Order), "Customer");

      var fetchQuery1 = CreateFakeQuery ();
      var fetchQuery2 = CreateFakeQuery ();
      _queryWithFetchQueries = CreateFakeQuery (
          Tuple.Create (_orderTicketEndPointDefinition, fetchQuery1),
          Tuple.Create (_customerEndPointDefinition, fetchQuery2));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_PerformsEagerFetching_AndRegistersLoadedObjects ()
    {
      LoadedObjectDataPendingRegistrationCollector collector = null;

      var consolidatedResultItems = new[] { CreateEquivalentData (_resultItem1), CreateEquivalentData (_resultItem2) };

      using (_mockRepository.Ordered())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.ExecuteCollectionQuery (_queryWithFetchQueries, _loadedObjectDataProviderStub))
            .Return (new[] { _resultItem1, _resultItem2 });
        _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock => mock.BeginRegisterIfRequired (
                  Arg.Is (new[] { _resultItem1, _resultItem2 }), Arg.Is (true), Arg<LoadedObjectDataPendingRegistrationCollector>.Is.NotNull))
          .WhenCalled (mi => collector = (LoadedObjectDataPendingRegistrationCollector) mi.Arguments[2])
          .Return (consolidatedResultItems);
        _eagerFetcherMock
            .Expect (
                mock => mock.PerformEagerFetching (
                    Arg.Is (consolidatedResultItems),
                    Arg.Is (_queryWithFetchQueries.EagerFetchQueries),
                    Arg.Is (_fetchEnabledObjectLoader),
                    Arg<LoadedObjectDataPendingRegistrationCollector>.Matches (c => c == collector)));
        _loadedObjectDataRegistrationAgentMock
            .Expect (mock => mock.EndRegisterIfRequired (Arg<LoadedObjectDataPendingRegistrationCollector>.Matches (c => c == collector)));
      }
      _mockRepository.ReplayAll();

      var result = _fetchEnabledObjectLoader.GetOrLoadCollectionQueryResult (_queryWithFetchQueries);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (consolidatedResultItems));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_EndsRegistration_EvenWhenAnExceptionIsThrown ()
    {
      var exception = new Exception ("Test");

      LoadedObjectDataPendingRegistrationCollector collector = null;

      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (_queryWithFetchQueries, _loadedObjectDataProviderStub))
          .Return (new[] { _resultItem1, _resultItem2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock => mock.BeginRegisterIfRequired (
                  Arg.Is (new[] { _resultItem1, _resultItem2 }), Arg.Is (true), Arg<LoadedObjectDataPendingRegistrationCollector>.Is.NotNull))
          .WhenCalled (mi => collector = (LoadedObjectDataPendingRegistrationCollector) mi.Arguments[2])
          .Return (new[] { _resultItem1, _resultItem2 });
      _eagerFetcherMock
          .Expect (
              mock => mock.PerformEagerFetching (
                  Arg.Is (new[] { _resultItem1, _resultItem2 }),
                  Arg.Is (_queryWithFetchQueries.EagerFetchQueries),
                  Arg.Is (_fetchEnabledObjectLoader),
                  Arg<LoadedObjectDataPendingRegistrationCollector>.Matches (c => c == collector)))
          .Throw (exception);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.EndRegisterIfRequired (Arg<LoadedObjectDataPendingRegistrationCollector>.Matches (c => c == collector)));
      _mockRepository.ReplayAll ();

      Assert.That (() => _fetchEnabledObjectLoader.GetOrLoadCollectionQueryResult (_queryWithFetchQueries), Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOrLoadFetchQueryResult ()
    {
      var pendingRegistrationCollector = new LoadedObjectDataPendingRegistrationCollector();
      var consolidatedResultItems =
          new[]
          {
              CreateEquivalentData (_resultItemWithSourceData1.LoadedObjectData),
              CreateEquivalentData (_resultItemWithSourceData2.LoadedObjectData)
          };

      using (_mockRepository.Ordered())
      {
        _persistenceStrategyMock
            .Expect (mock => mock.ExecuteFetchQuery (_queryWithFetchQueries, _loadedObjectDataProviderStub))
            .Return (new[] { _resultItemWithSourceData1, _resultItemWithSourceData2 });
        _loadedObjectDataRegistrationAgentMock
            .Expect (
                mock => mock.BeginRegisterIfRequired (
                    Arg<IEnumerable<ILoadedObjectData>>.List.Equal (
                        new[] { _resultItemWithSourceData1.LoadedObjectData, _resultItemWithSourceData2.LoadedObjectData }),
                    Arg.Is (true),
                    Arg.Is (pendingRegistrationCollector)))
            .Return (consolidatedResultItems);
        _eagerFetcherMock
            .Expect (
                mock => mock.PerformEagerFetching (
                    Arg<ICollection<ILoadedObjectData>>.List.Equal (consolidatedResultItems),
                    Arg.Is (_queryWithFetchQueries.EagerFetchQueries),
                    Arg.Is (_fetchEnabledObjectLoader),
                    Arg.Is (pendingRegistrationCollector)));
      }

      _mockRepository.ReplayAll ();

      var result = _fetchEnabledObjectLoader.GetOrLoadFetchQueryResult (_queryWithFetchQueries, pendingRegistrationCollector);

      _mockRepository.VerifyAll ();
      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  new LoadedObjectDataWithDataSourceData (consolidatedResultItems[0], _resultItemWithSourceData1.DataSourceData),
                  new LoadedObjectDataWithDataSourceData (consolidatedResultItems[1], _resultItemWithSourceData2.DataSourceData)
              }));
    }

    private ILoadedObjectData CreateEquivalentData (ILoadedObjectData loadedObjectData)
    {
      return LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (loadedObjectData.ObjectID);
    }

    [Test]
    public void Serializable ()
    {
      var instance = new FetchEnabledObjectLoader (
          new SerializableFetchEnabledPersistenceStrategyFake(),
          new SerializableLoadedObjectDataRegistrationAgentFake(),
          new SerializableLoadedObjectDataProviderFake(),
          new SerializableEagerFetcherFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.PersistenceStrategy, Is.Not.Null);
      Assert.That (deserializedInstance.LoadedObjectDataRegistrationAgent, Is.Not.Null);
      Assert.That (deserializedInstance.LoadedObjectDataProvider, Is.Not.Null);
      Assert.That (deserializedInstance.EagerFetcher, Is.Not.Null);
    }

    private IQuery CreateFakeQuery (params Tuple<IRelationEndPointDefinition, IQuery>[] fetchQueries)
    {
      var query = QueryFactory.CreateCollectionQuery (
          "test", TestDomainStorageProviderDefinition, "TEST", new QueryParameterCollection(), typeof (DomainObjectCollection));
      foreach (var fetchQuery in fetchQueries)
        query.EagerFetchQueries.Add (fetchQuery.Item1, fetchQuery.Item2);

      return query;
    }
  }
}