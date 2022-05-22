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
using System.Linq.Expressions;
using Moq;
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
using Remotion.Development.UnitTesting;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class CommitRollbackAgentTest : StandardMappingTest
  {

    private Mock<IClientTransactionEventSink> _eventSinkWithMock;
    private Mock<IPersistenceStrategy> _persistenceStrategyMock;
    private Mock<IDataManager> _dataManagerMock;
    private ClientTransaction _clientTransaction;

    private CommitRollbackAgent _agent;

    private DomainObject _fakeChangedDomainObject;
    private DomainObject _fakeNewDomainObject;
    private DomainObject _fakeDeletedDomainObject;

    private PersistableData _fakeChangedPersistableItem;
    private PersistableData _fakeNewPersistableItem;
    private PersistableData _fakeDeletedPersistableItem;
    private Expression<Func<Predicate<DomainObjectState>,bool>> _predicateThatMatchesOnlyChangedDataContainers;

    public override void SetUp ()
    {
      base.SetUp();

      _eventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
      _persistenceStrategyMock = new Mock<IPersistenceStrategy>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IDataManager>(MockBehavior.Strict);
      _clientTransaction = ClientTransactionObjectMother.Create();

      _agent = new CommitRollbackAgent(_clientTransaction, _eventSinkWithMock.Object, _persistenceStrategyMock.Object, _dataManagerMock.Object);

      _fakeChangedDomainObject = (DomainObject)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      _fakeNewDomainObject = (DomainObject)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);
      _fakeDeletedDomainObject = (DomainObject)LifetimeService.NewObject(_clientTransaction, typeof(Order), ParamList.Empty);

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

      _predicateThatMatchesOnlyChangedDataContainers =
          predicate => predicate(new DomainObjectState.Builder().SetNew().Value)
                       && predicate(new DomainObjectState.Builder().SetChanged().Value)
                       && predicate(new DomainObjectState.Builder().SetDeleted().Value)
                       && predicate(new DomainObjectState.Builder().SetChanged().SetDeleted().Value)
                       && predicate(new DomainObjectState.Builder().SetChanged().SetNotLoadedYet().Value)
                       && !predicate(new DomainObjectState.Builder().SetUnchanged().Value)
                       && !predicate(new DomainObjectState.Builder().SetInvalid().Value)
                       && !predicate(new DomainObjectState.Builder().SetNotLoadedYet().Value);
    }

    [Test]
    public void HasData_WithNonEmptyResult_ReturnsTrue ()
    {
      var fakeDomainObject = DomainObjectMother.CreateFakeObject<Order>();
      var fakeDataContainer = DataContainer.CreateNew(fakeDomainObject.ID);

      var item = new PersistableData(
          fakeDomainObject,
          new DomainObjectState.Builder().Value,
          fakeDataContainer,
          new IRelationEndPoint[0]);

      var data = new[] { item };
      Predicate<DomainObjectState> predicate = _ => true;
      _dataManagerMock.Setup(stub => stub.GetLoadedDataByObjectState(predicate)).Returns(data);

      var result = _agent.HasData(predicate);
      Assert.That(result, Is.True);
    }

    [Test]
    public void HasData_WithEmptyResult_ReturnsFalse ()
    {
      var data = new PersistableData[0];
      Predicate<DomainObjectState> predicate = _ => true;
      _dataManagerMock.Setup(stub => stub.GetLoadedDataByObjectState(predicate)).Returns(data);

      var result = _agent.HasData(predicate);
      Assert.That(result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      var sequence = new MockSequence();

        // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
      _dataManagerMock
            .InSequence(sequence)
            .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
            .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem })
            .Verifiable();
      ExpectTransactionCommitting(sequence, new[] { _fakeChangedDomainObject, _fakeNewDomainObject });

        // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set
        // Event is raised just for _fakeDeletedPersistableItem - the others have already got their event
      _dataManagerMock
            .InSequence(sequence)
            .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
            .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem })
            .Verifiable();
      ExpectTransactionCommitting(sequence, new[] { _fakeDeletedDomainObject });

        // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
      _dataManagerMock
            .InSequence(sequence)
            .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
            .Returns(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem })
            .Verifiable();

        // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
      ExpectTransactionCommitValidate(sequence, new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

        // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
      ExpectPersistData(sequence, new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });
      _dataManagerMock.InSequence(sequence).Setup(mock => mock.Commit()).Verifiable();

        // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
        ExpectTransactionCommitted(sequence, new[] { _fakeNewDomainObject });

      _agent.CommitData();

      _eventSinkWithMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
    }

    [Test]
    public void Commit_WithRegisterForAdditionalEvents ()
    {
      var sequence = new MockSequence();

      // First run of BeginCommit: fakeChangedPersistableItem, _fakeNewPersistableItem in commit set - event raised for both
      // fakeChangedDomainObject and _fakeNewDomainObject are both reregistered
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem })
          .Verifiable();
      ExpectTransactionCommitting(
          sequence,
          new[] { _fakeChangedDomainObject, _fakeNewDomainObject },
          (IReadOnlyList<IDomainObject> _, ICommittingEventRegistrar eventRegistrar) =>
              eventRegistrar.RegisterForAdditionalCommittingEvents(_fakeChangedDomainObject, _fakeNewDomainObject));

      // Second run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, fakeDeletedPersistableItem in commit set
      // Event is raised for all three - two have been reregistered, one is added to the commit set
      // _fakeChangedDomainObject is again reregistered
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem })
          .Verifiable();
      ExpectTransactionCommitting(
          sequence,
          new[] { _fakeChangedDomainObject, _fakeDeletedDomainObject, _fakeNewDomainObject },
          (IReadOnlyList<IDomainObject> _, ICommittingEventRegistrar eventRegistrar) =>
              eventRegistrar.RegisterForAdditionalCommittingEvents(_fakeChangedDomainObject));

      // Third run of BeginCommit: _fakeChangedPersistableItem, _fakeNewPersistableItem, fakeDeletedPersistableItem in commit set
      // Event is raised only for _fakeChangedDomainObject, it was reregistered
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem })
          .Verifiable();
      ExpectTransactionCommitting(sequence, new[] { _fakeChangedDomainObject });

      // End of BeginCommit: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - events already raised for all of those
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem })
          .Verifiable();

      // CommitValidate: _fakeNewPersistableItem, _fakeDeletedPersistableItem in commit set - this is what actually gets committed
      ExpectTransactionCommitValidate(sequence, new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });

      // Commit _fakeNewPersistableItem, _fakeDeletedPersistableItem found earlier
      ExpectPersistData(sequence, new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem });
      _dataManagerMock.InSequence(sequence).Setup(mock => mock.Commit()).Verifiable();

      // Raise event for _fakeNewPersistableItem only, _fakeDeletedPersistableItem was deleted
      ExpectTransactionCommitted(sequence, new[] { _fakeNewDomainObject });

      _agent.CommitData();

      _eventSinkWithMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
    }

    [Test]
    public void Rollback ()
    {
      var sequence = new MockSequence();

      // First run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem in rollback set - event raised for both
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem })
          .Verifiable();
      ExpectTransactionRollingBack(sequence, new[] { _fakeChangedDomainObject, _fakeNewDomainObject });

      // Second run of BeginRollback: fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set
      // Event is raised just for _fakeDeletedPersistableItem -  the others have alreay got their event
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeChangedPersistableItem, _fakeNewPersistableItem, _fakeDeletedPersistableItem })
          .Verifiable();
      ExpectTransactionRollingBack(sequence, new[] { _fakeDeletedDomainObject });

      // End of BeginRollback: _fakeNewPersistableItem, _fakeDeletedPersistableItem in rollback set - events already raised for all of those
      _dataManagerMock
          .InSequence(sequence)
          .Setup(mock => mock.GetLoadedDataByObjectState(It.Is(_predicateThatMatchesOnlyChangedDataContainers)))
          .Returns(new[] { _fakeNewPersistableItem, _fakeDeletedPersistableItem })
          .Verifiable();

      // Rollback
      _dataManagerMock.InSequence(sequence).Setup(mock => mock.Rollback()).Verifiable();

      // Raise event only for _fakeDeletedPersistableItem, _fakeNewPersistableItem was New
      ExpectTransactionRolledBack(sequence, new[] { _fakeDeletedDomainObject });

      _agent.RollbackData();

      _eventSinkWithMock.Verify();
      _persistenceStrategyMock.Verify();
      _dataManagerMock.Verify();
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

    private void ExpectTransactionCommitting (
        MockSequence sequence,
        DomainObject[] domainObjects,
        Action<IReadOnlyList<IDomainObject>, ICommittingEventRegistrar> callback = null)
    {
      _eventSinkWithMock
          .InSequence(sequence)
          .Setup(
              mock => mock.RaiseTransactionCommittingEvent(
                  It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects)),
                  It.IsNotNull<CommittingEventRegistrar>()))
          .Callback(
              (IReadOnlyList<IDomainObject> domainObjectsParameter, ICommittingEventRegistrar eventRegistrarParameter) =>
              {
                Assert.That(((CommittingEventRegistrar)eventRegistrarParameter).ClientTransaction, Is.SameAs(_clientTransaction));
                if (callback != null)
                  callback(domainObjectsParameter, eventRegistrarParameter);
              })
          .Verifiable();
    }

    private void ExpectTransactionCommitValidate (MockSequence sequence, PersistableData[] persistableData)
    {
      _eventSinkWithMock
          .InSequence(sequence)
          .Setup(mock => mock.RaiseTransactionCommitValidateEvent(It.Is<ReadOnlyCollection<PersistableData>>(p => p.SetEquals(persistableData))))
          .Verifiable();
    }

    private void ExpectPersistData (MockSequence sequence, PersistableData[] persistableDatas)
    {
      _persistenceStrategyMock
          .InSequence(sequence)
          .Setup(mock => mock.PersistData(It.Is<IEnumerable<PersistableData>>(p => p.SetEquals(persistableDatas))))
          .Verifiable();
    }

    private void ExpectTransactionCommitted (MockSequence sequence, DomainObject[] domainObjects)
    {
      _eventSinkWithMock
          .InSequence(sequence)
          .Setup(mock => mock.RaiseTransactionCommittedEvent(It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Verifiable();
    }

    private void ExpectTransactionRollingBack (MockSequence sequence, DomainObject[] domainObjects)
    {
      _eventSinkWithMock
          .InSequence(sequence)
          .Setup(mock => mock.RaiseTransactionRollingBackEvent(It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Verifiable();
    }

    private void ExpectTransactionRolledBack (MockSequence sequence, DomainObject[] domainObjects)
    {
      _eventSinkWithMock
          .InSequence(sequence)
          .Setup(mock => mock.RaiseTransactionRolledBackEvent(It.Is<ReadOnlyCollection<IDomainObject>>(p => p.SetEquals(domainObjects))))
          .Verifiable();
    }
  }
}
