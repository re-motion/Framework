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
using Remotion.Development.NUnit.UnitTesting;
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
    private Func<DomainObjectState, bool> _isPersistenceRelevantStatePredicate;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();

      _eventSinkWithMock = _mockRepository.StrictMock<IClientTransactionEventSink>();
      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy>();
      _dataManagerMock = _mockRepository.StrictMock<IDataManager>();
      _clientTransaction = ClientTransactionObjectMother.Create();

      _agent = new CommitRollbackAgent(_clientTransaction, _eventSinkWithMock, _persistenceStrategyMock, _dataManagerMock);

      _fakeChangedDomainObject = LifetimeService.NewObject(_clientTransaction, typeof (Order), ParamList.Empty);
      _fakeNewDomainObject = LifetimeService.NewObject(_clientTransaction, typeof (Order), ParamList.Empty);
      _fakeDeletedDomainObject = LifetimeService.NewObject(_clientTransaction, typeof (Order), ParamList.Empty);

      var fakeDataContainer1 = DataContainerObjectMother.Create();
      var fakeDataContainer2 = DataContainerObjectMother.Create();
      var fakeDataContainer3 = DataContainerObjectMother.Create();

      _fakeChangedPersistableItem = new PersistableData(
          _fakeChangedDomainObject,
          new DomainObjectState.Builder().SetChanged().Value,
          fakeDataContainer1,
          new IRelationEndPoint[0]);

      _fakeNewPersistableItem = new PersistableData(
          _fakeNewDomainObject,
          new DomainObjectState.Builder().SetNew().Value,
          fakeDataContainer2,
          new IRelationEndPoint[0]);

      _fakeDeletedPersistableItem = new PersistableData(
          _fakeDeletedDomainObject,
          new DomainObjectState.Builder().SetDeleted().Value,
          fakeDataContainer3,
          new IRelationEndPoint[0]);

      Func<DomainObjectState, bool> isPersistenceRelevantStatePredicate = null;
      _dataManagerMock.Stub(stub => stub.GetLoadedDataByObjectState(null)).IgnoreArguments().Return(new PersistableData[0])
          .WhenCalled(mi => isPersistenceRelevantStatePredicate = (Func<DomainObjectState, bool>) mi.Arguments[0]);
      _mockRepository.ReplayAll();

      _agent.HasDataChanged();
      Assert.That(isPersistenceRelevantStatePredicate, Is.Not.Null);
      _isPersistenceRelevantStatePredicate = isPersistenceRelevantStatePredicate;
      _mockRepository.BackToRecordAll();
    }

    [Test]
    public void IsPersistenceRelevantState_WithNew_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetNew().Value), Is.True);
    }

    [Test]
    public void IsPersistenceRelevantState_WithChanged_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetChanged().Value), Is.True);
    }

    [Test]
    public void IsPersistenceRelevantState_WithDeleted_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetDeleted().Value), Is.True);
    }

    [Test]
    public void IsPersistenceRelevantState_WithNewAndChangedAndDeleted_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetNew().SetChanged().SetDeleted().Value), Is.True);
    }

    [Test]
    public void IsPersistenceRelevantState_WithChangedAndNotLoadedYet_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetChanged().SetNotLoadedYet().Value), Is.True);
    }

    [Test]
    public void IsPersistenceRelevantState_WithUnchanged_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetUnchanged().Value), Is.False);
    }

    [Test]
    public void IsPersistenceRelevantState_WithInvalid_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetInvalid().Value), Is.False);
    }

    [Test]
    public void IsPersistenceRelevantState_WithNotLoadedYet_ReturnsTrue ()
    {
      Assert.That(_isPersistenceRelevantStatePredicate(new DomainObjectState.Builder().SetNotLoadedYet().Value), Is.False);
    }

    [Test]
    public void HasDataChanged_True ()
    {
      var fakeDomainObject = DomainObjectMother.CreateFakeObject<Order>();
      var fakeDataContainer = DataContainer.CreateNew(fakeDomainObject.ID);

      var item = new PersistableData(
          fakeDomainObject,
          new DomainObjectState.Builder().SetChanged().Value,
          fakeDataContainer,
          new IRelationEndPoint[0]);

      _dataManagerMock.Stub(stub => stub.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate)).Return(new[] { item });
      _mockRepository.ReplayAll();

      var result = _agent.HasDataChanged();
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasDataChanged_False ()
    {
      _dataManagerMock
          .Stub(stub => stub.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
          .Return(new PersistableData[0]);
      _mockRepository.ReplayAll();

      var result = _agent.HasDataChanged();
      Assert.That(result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      using (_mockRepository.Ordered())
      {
        // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        ExpectTransactionCommitting(_fakeChangedDomainObject, _fakeNewDomainObject);

        // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set 
        // Event is raised just for _fakeDeletedPersistableItem - the others have already got their event
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionCommitting(_fakeDeletedDomainObject);

        // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
        ExpectTransactionCommitValidate(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        
        // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
        ExpectPersistData(_fakeNewPersistableItem, _fakeDeletedPersistableItem);
        _dataManagerMock.Expect(mock => mock.Commit());

        // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
        ExpectTransactionCommitted(_fakeNewDomainObject);
      }
      _mockRepository.ReplayAll();

      _agent.CommitData();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Commit_WithRegisterForAdditionalEvents ()
    {
      using (_mockRepository.Ordered())
      {
        // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
        // fakeChangedDomainObject and _fakeNewDomainObject are both reregistered
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        ExpectTransactionCommitting(_fakeChangedDomainObject, _fakeNewDomainObject)
            .WhenCalled(mi => GetEventRegistrar(mi).RegisterForAdditionalCommittingEvents(_fakeChangedDomainObject, _fakeNewDomainObject));

        // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, fakeDeletedPersistableItem in commit set 
        // Event is raised for all three - two have been reregistered, one is added to the commit set
        // _fakeChangedDomainObject is again reregistered
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionCommitting(_fakeChangedDomainObject, _fakeDeletedDomainObject, _fakeNewDomainObject)
            .WhenCalled(mi => GetEventRegistrar(mi).RegisterForAdditionalCommittingEvents(_fakeChangedDomainObject));

        // Third run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, fakeDeletedPersistableItem in commit set 
        // Event is raised only for _fakeChangedDomainObject, it was reregistered
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionCommitting(_fakeChangedDomainObject);

        // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
        ExpectTransactionCommitValidate(_fakeNewPersistableItem, _fakeDeletedPersistableItem);

        // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
        ExpectPersistData(_fakeNewPersistableItem, _fakeDeletedPersistableItem);
        _dataManagerMock.Expect(mock => mock.Commit());

        // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
        ExpectTransactionCommitted(_fakeNewDomainObject);
      }
      _mockRepository.ReplayAll();

      _agent.CommitData();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      using (_mockRepository.Ordered())
      {
        // First run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem in rollback set - event raised for both
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem });
        ExpectTransactionRollingBack(_fakeChangedDomainObject, _fakeNewDomainObject);

        // Second run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set 
        // Event is raised just for _fakeDeletedPersistableItem -  the others have alreay got their event
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem });
        ExpectTransactionRollingBack(_fakeDeletedDomainObject);

        // End of BeginRollback: _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set - events already raised for all of those
        _dataManagerMock
            .Expect(mock => mock.GetLoadedDataByObjectState(_isPersistenceRelevantStatePredicate))
            .Return(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // Rollback
        _dataManagerMock.Expect(mock => mock.Rollback());

        // Raise event only for _fakeDeletedPersistableItem, _fakeNewPersistableItem was New
        ExpectTransactionRolledBack(_fakeDeletedDomainObject);
      }
      _mockRepository.ReplayAll();

      _agent.RollbackData();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Serializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var instance = new CommitRollbackAgent(
          _clientTransaction,
          new SerializableClientTransactionEventSinkFake(),
          new SerializablePersistenceStrategyFake(),
          new SerializableDataManagerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize(instance);

      Assert.That(deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That(deserializedInstance.EventSink, Is.Not.Null);
      Assert.That(deserializedInstance.PersistenceStrategy, Is.Not.Null);
      Assert.That(deserializedInstance.DataManager, Is.Not.Null);
    }

    private IMethodOptions<RhinoMocksExtensions.VoidType> ExpectTransactionCommitting (params DomainObject[] domainObjects)
    {
      return _eventSinkWithMock.Expect(mock => mock.RaiseTransactionCommittingEvent(
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent(domainObjects),
          Arg<CommittingEventRegistrar>.Is.TypeOf))
          .WhenCalled(mi => Assert.That(((CommittingEventRegistrar) mi.Arguments[1]).ClientTransaction, Is.SameAs(_clientTransaction)));
    }

    private void ExpectTransactionCommitValidate (params PersistableData[] persistableData)
    {
      _eventSinkWithMock.Expect(mock => mock.RaiseTransactionCommitValidateEvent(
          Arg<ReadOnlyCollection<PersistableData>>.List.Equivalent(persistableData)));
    }

    private void ExpectPersistData (params PersistableData[] persistableDatas)
    {
      _persistenceStrategyMock.Expect(mock => mock.PersistData(Arg<IEnumerable<PersistableData>>.List.Equivalent(persistableDatas)));
    }

    private void ExpectTransactionCommitted (params DomainObject[] domainObjects)
    {
      _eventSinkWithMock.Expect(mock => mock.RaiseTransactionCommittedEvent(
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent(domainObjects)));
    }

    private void ExpectTransactionRollingBack (params DomainObject[] domainObjects)
    {
      _eventSinkWithMock.Expect(mock => mock.RaiseTransactionRollingBackEvent(
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent(domainObjects)));
    }

    private void ExpectTransactionRolledBack (params DomainObject[] domainObjects)
    {
      _eventSinkWithMock.Expect(mock => mock.RaiseTransactionRolledBackEvent(
          Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent(domainObjects)));
    }


    private ICommittingEventRegistrar GetEventRegistrar (MethodInvocation mi)
    {
      return ((ICommittingEventRegistrar) mi.Arguments[1]);
    }

  }
}