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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class FetchEnabledObjectLoaderTest : StandardMappingTest
  {

    private Mock<IFetchEnabledPersistenceStrategy> _persistenceStrategyMock;
    private Mock<ILoadedObjectDataRegistrationAgent> _loadedObjectDataRegistrationAgentMock;
    private Mock<ILoadedObjectDataProvider> _loadedObjectDataProviderStub;
    private Mock<IEagerFetcher> _eagerFetcherMock;

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
      base.SetUp();

      _persistenceStrategyMock = new Mock<IFetchEnabledPersistenceStrategy>(MockBehavior.Strict);
      _loadedObjectDataRegistrationAgentMock = new Mock<ILoadedObjectDataRegistrationAgent>(MockBehavior.Strict);
      _loadedObjectDataProviderStub = new Mock<ILoadedObjectDataProvider>();
      _eagerFetcherMock = new Mock<IEagerFetcher>(MockBehavior.Strict);

      _fetchEnabledObjectLoader = new FetchEnabledObjectLoader(
          _persistenceStrategyMock.Object,
          _loadedObjectDataRegistrationAgentMock.Object,
          _loadedObjectDataProviderStub.Object,
          _eagerFetcherMock.Object);

      _resultItem1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(DomainObjectIDs.Order1).Object;
      _resultItem2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(DomainObjectIDs.Order3).Object;
      _resultItemWithSourceData1 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(DomainObjectIDs.Order1);
      _resultItemWithSourceData2 = LoadedObjectDataObjectMother.CreateLoadedObjectDataWithDataSourceData(DomainObjectIDs.Order3);

      _orderTicketEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
      _customerEndPointDefinition = GetEndPointDefinition(typeof(Order), "Customer");

      var fetchQuery1 = CreateFakeQuery();
      var fetchQuery2 = CreateFakeQuery();
      _queryWithFetchQueries = CreateFakeQuery(
          Tuple.Create(_orderTicketEndPointDefinition, fetchQuery1),
          Tuple.Create(_customerEndPointDefinition, fetchQuery2));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_PerformsEagerFetching_AndRegistersLoadedObjects ()
    {
      LoadedObjectDataPendingRegistrationCollector collector = null;

      var consolidatedResultItems = new[] { CreateEquivalentData(_resultItem1), CreateEquivalentData(_resultItem2) };

      var sequence = new VerifiableSequence();

      _persistenceStrategyMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecuteCollectionQuery(_queryWithFetchQueries, _loadedObjectDataProviderStub.Object))
          .Returns(new[] { _resultItem1, _resultItem2 })
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .InVerifiableSequence(sequence)
          .Setup(
              mock => mock.BeginRegisterIfRequired(
                  new[] { _resultItem1, _resultItem2 },
                  true,
                  It.IsNotNull<LoadedObjectDataPendingRegistrationCollector>()))
          .Callback(
              (IEnumerable<ILoadedObjectData> _, bool throwOnNotFound, LoadedObjectDataPendingRegistrationCollector pendingLoadedObjectDataCollector) =>
                  collector = pendingLoadedObjectDataCollector)
          .Returns(consolidatedResultItems)
          .Verifiable();
      _eagerFetcherMock
          .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.PerformEagerFetching(
                    consolidatedResultItems,
                    _queryWithFetchQueries.EagerFetchQueries,
                    _fetchEnabledObjectLoader,
                    It.Is<LoadedObjectDataPendingRegistrationCollector>(c => c == collector)))
            .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .InVerifiableSequence(sequence)
            .Setup(mock => mock.EndRegisterIfRequired(It.Is<LoadedObjectDataPendingRegistrationCollector>(c => c == collector)))
            .Verifiable();

      var result = _fetchEnabledObjectLoader.GetOrLoadCollectionQueryResult(_queryWithFetchQueries);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      _eagerFetcherMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(consolidatedResultItems));
    }

    [Test]
    public void GetOrLoadCollectionQueryResult_EndsRegistration_EvenWhenAnExceptionIsThrown ()
    {
      var exception = new Exception("Test");

      LoadedObjectDataPendingRegistrationCollector collector = null;

      _persistenceStrategyMock
          .Setup(mock => mock.ExecuteCollectionQuery(_queryWithFetchQueries, _loadedObjectDataProviderStub.Object))
          .Returns(new[] { _resultItem1, _resultItem2 })
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .Setup(
              mock => mock.BeginRegisterIfRequired(
                  new[] { _resultItem1, _resultItem2 },
                  true,
                  It.IsNotNull<LoadedObjectDataPendingRegistrationCollector>()))
          .Callback(
              (IEnumerable<ILoadedObjectData> _, bool throwOnNotFound, LoadedObjectDataPendingRegistrationCollector pendingLoadedObjectDataCollector) =>
                  collector = pendingLoadedObjectDataCollector)
          .Returns(new[] { _resultItem1, _resultItem2 })
          .Verifiable();
      _eagerFetcherMock
          .Setup(
              mock => mock.PerformEagerFetching(
                  new[] { _resultItem1, _resultItem2 },
                  _queryWithFetchQueries.EagerFetchQueries,
                  _fetchEnabledObjectLoader,
                  It.Is<LoadedObjectDataPendingRegistrationCollector>(c => c == collector)))
          .Throws(exception)
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
          .Setup(mock => mock.EndRegisterIfRequired(It.Is<LoadedObjectDataPendingRegistrationCollector>(c => c == collector)))
          .Verifiable();

      Assert.That(() => _fetchEnabledObjectLoader.GetOrLoadCollectionQueryResult(_queryWithFetchQueries), Throws.Exception.SameAs(exception));

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      _eagerFetcherMock.Verify();
    }

    [Test]
    public void GetOrLoadFetchQueryResult ()
    {
      var pendingRegistrationCollector = new LoadedObjectDataPendingRegistrationCollector();
      var consolidatedResultItems =
          new[]
          {
              CreateEquivalentData(_resultItemWithSourceData1.LoadedObjectData),
              CreateEquivalentData(_resultItemWithSourceData2.LoadedObjectData)
          };

      var sequence = new VerifiableSequence();

      _persistenceStrategyMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecuteFetchQuery(_queryWithFetchQueries, _loadedObjectDataProviderStub.Object))
          .Returns(new[] { _resultItemWithSourceData1, _resultItemWithSourceData2 })
          .Verifiable();
      _loadedObjectDataRegistrationAgentMock
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.BeginRegisterIfRequired(
                    new[] { _resultItemWithSourceData1.LoadedObjectData, _resultItemWithSourceData2.LoadedObjectData },
                    true,
                    pendingRegistrationCollector))
            .Returns(consolidatedResultItems)
            .Verifiable();
      _eagerFetcherMock
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.PerformEagerFetching(
                    consolidatedResultItems,
                    _queryWithFetchQueries.EagerFetchQueries,
                    _fetchEnabledObjectLoader,
                    pendingRegistrationCollector))
            .Verifiable();

      var result = _fetchEnabledObjectLoader.GetOrLoadFetchQueryResult(_queryWithFetchQueries, pendingRegistrationCollector);

      _persistenceStrategyMock.Verify();
      _loadedObjectDataRegistrationAgentMock.Verify();
      _eagerFetcherMock.Verify();
      sequence.Verify();
      Assert.That(
          result,
          Is.EqualTo(
              new[]
              {
                  new LoadedObjectDataWithDataSourceData(consolidatedResultItems[0], _resultItemWithSourceData1.DataSourceData),
                  new LoadedObjectDataWithDataSourceData(consolidatedResultItems[1], _resultItemWithSourceData2.DataSourceData)
              }));
    }

    private ILoadedObjectData CreateEquivalentData (ILoadedObjectData loadedObjectData)
    {
      return LoadedObjectDataObjectMother.CreateLoadedObjectDataStub(loadedObjectData.ObjectID).Object;
    }

    private IQuery CreateFakeQuery (params Tuple<IRelationEndPointDefinition, IQuery>[] fetchQueries)
    {
      var query = QueryFactory.CreateCollectionQuery(
          "test", TestDomainStorageProviderDefinition, "TEST", new QueryParameterCollection(), typeof(DomainObjectCollection));
      foreach (var fetchQuery in fetchQueries)
        query.EagerFetchQueries.Add(fetchQuery.Item1, fetchQuery.Item2);

      return query;
    }
  }
}
