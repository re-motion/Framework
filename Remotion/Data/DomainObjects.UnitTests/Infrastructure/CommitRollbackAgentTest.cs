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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.RhinoMocks.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class CommitRollbackAgentTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private IClientTransactionEventSink _eventSinkWithMock;
    private IPersistenceStrategy _persistenceStrategyMock;
    private IDataManager _dataManagerMock;
    private ClientTransaction _clientTransaction;

    private CommitRollbackAgent _agent;

    private DomainObject _fakeChangedDomainObject;
    private DomainObject _fakeNewDomainObject;
    private DomainObject _fakeDeletedDomainObject;

    private PersistableData _fakeChangedPersistableItem;
    private PersistableData _fakeNewPersistableItem;
    private PersistableData _fakeDeletedPersistableItem;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();

      _eventSinkWithMock = _mockRepository.StrictMock<IClientTransactionEventSink>();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager> ();
      _clientTransaction = ClientTransactionObjectMother.Create();

      _agent = new CommitRollbackAgent (_clientTransaction, _eventSinkWithMock, _persistenceStrategyMock, _dataManagerMock);

      _fakeChangedDomainObject = LifetimeService.NewObject (_clientTransaction, typeof (Order), ParamList.Empty);
      _fakeNewDomainObject = LifetimeService.NewObject (_clientTransaction, typeof (Order), ParamList.Empty);
      _fakeDeletedDomainObject = LifetimeService.NewObject (_clientTransaction, typeof (Order), ParamList.Empty);

      var fakeDataContainer1 = DataContainerObjectMother.Create();
      var fakeDataContainer2 = DataContainerObjectMother.Create();
      var fakeDataContainer3 = DataContainerObjectMother.Create();

      _fakeChangedPersistableItem = new PersistableData (_fakeChangedDomainObject, StateType.Changed, fakeDataContainer1, new IRelationEndPoint[0]);
      _fakeNewPersistableItem = new PersistableData (_fakeNewDomainObject, StateType.New, fakeDataContainer2, new IRelationEndPoint[0]);
      _fakeDeletedPersistableItem = new PersistableData (_fakeDeletedDomainObject, StateType.Deleted, fakeDataContainer3, new IRelationEndPoint[0]);
    }

    [Test]
    public void HasDataChanged_True ()
    {
      var fakeDomainObject = DomainObjectMother.CreateFakeObject<Order>();
      var fakeDataContainer = DataContainer.CreateNew (fakeDomainObject.ID);

      var item = new PersistableData (fakeDomainObject, StateType.Changed, fakeDataContainer, new IRelationEndPoint[0]);

      _dataManagerMock.Stub (stub => stub.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New)).Return (new[] { item });
      _mockRepository.ReplayAll ();

      var result = _agent.HasDataChanged();
      Assert.That (result, Is.True);
    }

    [Test]
    public void HasDataChanged_False ()
    {
      _dataManagerMock
          .Stub (stub => stub.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
          .Return (new PersistableData[0]);
      _mockRepository.ReplayAll ();

      var result = _agent.HasDataChanged ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      using (_mockRepository.Ordered())
      {
        // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        ExpectTransactionCommitting (_fakeChangedDomainObject, _fakeNewDomainObject);

        // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set 
        // Event is raised just for _fakeDeletedPersistableItem - the others have already got their event
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionCommitting (_fakeDeletedDomainObject);

        // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
        ExpectTransactionCommitValidate (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        
        // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
        ExpectPersistData (_fakeNewPersistableItem, _fakeDeletedPersistableItem);
        _dataManagerMock.Expect (mock => mock.Commit());

        // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
        ExpectTransactionCommitted (_fakeNewDomainObject);
      }
      _mockRepository.ReplayAll();

      _agent.CommitData();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Commit_WithRegisterForAdditionalEvents ()
    {
      using (_mockRepository.Ordered ())
      {
        // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
        // fakeChangedDomainObject and _fakeNewDomainObject are both reregistered
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        ExpectTransactionCommitting (_fakeChangedDomainObject, _fakeNewDomainObject)
            .WhenCalled (mi => GetEventRegistrar (mi).RegisterForAdditionalCommittingEvents (_fakeChangedDomainObject, _fakeNewDomainObject));

        // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, fakeDeletedPersistableItem in commit set 
        // Event is raised for all three - two have been reregistered, one is added to the commit set
        // _fakeChangedDomainObject is again reregistered
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionCommitting (_fakeChangedDomainObject, _fakeDeletedDomainObject, _fakeNewDomainObject)
            .WhenCalled (mi => GetEventRegistrar(mi).RegisterForAdditionalCommittingEvents (_fakeChangedDomainObject));

        // Third run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, fakeDeletedPersistableItem in commit set 
        // Event is raised only for _fakeChangedDomainObject, it was reregistered
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionCommitting (_fakeChangedDomainObject);

        // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
        ExpectTransactionCommitValidate (_fakeNewPersistableItem, _fakeDeletedPersistableItem);

        // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
        ExpectPersistData (_fakeNewPersistableItem, _fakeDeletedPersistableItem);
        _dataManagerMock.Expect (mock => mock.Commit ());

        // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
        ExpectTransactionCommitted (_fakeNewDomainObject);
      }
      _mockRepository.ReplayAll ();

      _agent.CommitData ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Rollback ()
    {
      using (_mockRepository.Ordered ())
      {
        // First run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem in rollback set - event raised for both
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        ExpectTransactionRollingBack (_fakeChangedDomainObject, _fakeNewDomainObject);

        // Second run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set 
        // Event is raised just for _fakeDeletedPersistableItem -  the others have alreay got their event
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionRollingBack (_fakeDeletedDomainObject);

        // End of BeginRollback: _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set - events already raised for all of those
        _dataManagerMock
            .Expect (mock => mock.GetLoadedDataByObjectState (StateType.Changed, StateType.Deleted, StateType.New))
            .Return (new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // Rollback
        _dataManagerMock.Expect (mock => mock.Rollback ());

        // Raise event only for _fakeDeletedPersistableItem, _fakeNewPersistableItem was New
        ExpectTransactionRolledBack (_fakeDeletedDomainObject);
      }
      _mockRepository.ReplayAll ();

      _agent.RollbackData ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Serializable ()
    {
      var instance = new CommitRollbackAgent (
          _clientTransaction,
          new SerializableClientTransactionEventSinkFake(),
          new SerializablePersistenceStrategyFake(),
          new SerializableDataManagerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
      Assert.That (deserializedInstance.PersistenceStrategy, Is.Not.Null);
      Assert.That (deserializedInstance.DataManager, Is.Not.Null);
    }

    private IMethodOptions<RhinoMocksExtensions.VoidType> ExpectTransactionCommitting (params DomainObject[] domainObjects)
    {
      return _eventSinkWithMock.Expect (mock => mock.RaiseTransactionCommittingEvent (
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects),
          Arg<CommittingEventRegistrar>.Is.TypeOf))
          .WhenCalled (mi => Assert.That (((CommittingEventRegistrar) mi.Arguments[1]).ClientTransaction, Is.SameAs (_clientTransaction)));
    }

    private void ExpectTransactionCommitValidate (params PersistableData[] persistableData)
    {
      _eventSinkWithMock.Expect (mock => mock.RaiseTransactionCommitValidateEvent (
          Arg<ReadOnlyCollection<PersistableData>>.List.Equivalent (persistableData)));
    }

    private void ExpectPersistData (params PersistableData[] persistableDatas)
    {
      _persistenceStrategyMock.Expect (mock => mock.PersistData (Arg<IEnumerable<PersistableData>>.List.Equivalent (persistableDatas)));
    }

    private void ExpectTransactionCommitted (params DomainObject[] domainObjects)
    {
      _eventSinkWithMock.Expect (mock => mock.RaiseTransactionCommittedEvent (
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects)));
    }

    private void ExpectTransactionRollingBack (params DomainObject[] domainObjects)
    {
      _eventSinkWithMock.Expect (mock => mock.RaiseTransactionRollingBackEvent (
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects)));
    }

    private void ExpectTransactionRolledBack (params DomainObject[] domainObjects)
    {
      _eventSinkWithMock.Expect (mock => mock.RaiseTransactionRolledBackEvent (
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (domainObjects)));
    }


    private ICommittingEventRegistrar GetEventRegistrar (MethodInvocation mi)
    {
      return ((ICommittingEventRegistrar) mi.Arguments[1]);
    }

  }
}