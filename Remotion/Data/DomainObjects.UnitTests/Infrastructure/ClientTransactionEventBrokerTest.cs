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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class ClientTransactionEventBrokerTest : ClientTransactionBaseTest
  {
    private ClientTransaction _clientTransaction;

    private ClientTransactionEventBroker _eventBroker;

    private DomainObject _domainObject1;
    private DomainObject _domainObject2;
    private DomainObject _invalidDomainObject;

    private Mock<IDomainObjectMockEventReceiver> _order1EventReceiverMock;
    private Mock<IDomainObjectMockEventReceiver> _order2EventReceiverMock;
    private Mock<IDomainObjectMockEventReceiver> _invalidObjectEventReceiverMock;

    private Mock<IUnloadEventReceiver> _unloadEventReceiverMock;
    private Mock<ILoadEventReceiver> _loadEventReceiverMock;

    private Mock<IClientTransactionMockEventReceiver> _transactionEventReceiverMock;

    private Mock<IClientTransactionExtension> _extensionMock;
    private Mock<IClientTransactionListener> _listenerMock;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _eventBroker = new ClientTransactionEventBroker(_clientTransaction);

      _domainObject1 = _clientTransaction.ExecuteInScope(() => DomainObjectIDs.Order1.GetObject<Order>());
      _domainObject2 = _clientTransaction.ExecuteInScope(() => DomainObjectIDs.Order3.GetObject<Order>());
      _invalidDomainObject = _clientTransaction.ExecuteInScope(
          () =>
          {
            var order = Order.NewObject();
            order.Delete();
            return order;
          });

      _order1EventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _domainObject1);
      _order2EventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _domainObject2);
      _invalidObjectEventReceiverMock = DomainObjectMockEventReceiver.CreateMock(MockBehavior.Strict, _invalidDomainObject);

      _unloadEventReceiverMock = new Mock<IUnloadEventReceiver>(MockBehavior.Strict);
      ((TestDomainBase)_domainObject1).SetUnloadEventReceiver(_unloadEventReceiverMock.Object);
      ((TestDomainBase)_domainObject2).SetUnloadEventReceiver(_unloadEventReceiverMock.Object);

      _transactionEventReceiverMock = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, _clientTransaction);

      _loadEventReceiverMock = new Mock<ILoadEventReceiver>(MockBehavior.Strict);
      ((TestDomainBase)_domainObject1).SetLoadEventReceiver(_loadEventReceiverMock.Object);
      ((TestDomainBase)_domainObject2).SetLoadEventReceiver(_loadEventReceiverMock.Object);

      _extensionMock = new Mock<IClientTransactionExtension>(MockBehavior.Strict);
      _extensionMock.Setup(stub => stub.Key).Returns("extension");
      _eventBroker.Extensions.Add(_extensionMock.Object);
      _extensionMock.Reset();

      _listenerMock = new Mock<IClientTransactionListener>(MockBehavior.Strict);
      _eventBroker.AddListener(_listenerMock.Object);
    }

    public override void TearDown ()
    {
      _clientTransaction.Discard();

      base.TearDown();
    }

    [Test]
    public void Extensions ()
    {
      Assert.That(_eventBroker.Extensions, Has.Member(_extensionMock.Object));

      var fakeExtension = ClientTransactionExtensionObjectMother.Create();

      Assert.That(_eventBroker.Extensions, Has.No.Member(fakeExtension));

      _eventBroker.Extensions.Add(fakeExtension);

      Assert.That(_eventBroker.Extensions, Has.Member(fakeExtension));
    }

    [Test]
    public void AddListener ()
    {
      var fakeListener = ClientTransactionListenerObjectMother.Create();
      Assert.That(_eventBroker.Listeners, Has.No.Member(fakeListener));

      _eventBroker.AddListener(fakeListener);

      Assert.That(_eventBroker.Listeners, Has.Member(fakeListener));
    }

    [Test]
    public void RemoveListener ()
    {
      Assert.That(_eventBroker.Listeners, Has.Member(_listenerMock.Object));

      _eventBroker.RemoveListener(_listenerMock.Object);

      Assert.That(_eventBroker.Listeners, Has.No.Member(_listenerMock.Object));
    }

    [Test]
    public void RaiseTransactionInitializeEvent ()
    {
      CheckEventWithListenersFirst(
          s => s.RaiseTransactionInitializeEvent(),
          l => l.TransactionInitialize(_clientTransaction),
          x => x.TransactionInitialize(_clientTransaction));
    }

    [Test]
    public void RaiseTransactionDiscardEvent ()
    {
      CheckEventWithListenersFirst(
          s => s.RaiseTransactionDiscardEvent(),
          l => l.TransactionDiscard(_clientTransaction),
          x => x.TransactionDiscard(_clientTransaction));
    }

    [Test]
    public void RaiseSubTransactionCreatingEvent ()
    {
      CheckEventWithListenersFirst(
          s => s.RaiseSubTransactionCreatingEvent(),
          l => l.SubTransactionCreating(_clientTransaction),
          x => x.SubTransactionCreating(_clientTransaction));
    }

    [Test]
    public void RaiseSubTransactionInitializeEvent ()
    {
      var subTransaction = ClientTransactionObjectMother.Create();
      CheckEventWithListenersFirst(
          s => s.RaiseSubTransactionInitializeEvent(subTransaction),
          l => l.SubTransactionInitialize(_clientTransaction, subTransaction),
          x => x.SubTransactionInitialize(_clientTransaction, subTransaction));
    }

    [Test]
    public void RaiseSubTransactionCreatedEvent ()
    {
      var subTransaction = ClientTransactionObjectMother.Create();

      CheckEventWithListenersLast(
          s => s.RaiseSubTransactionCreatedEvent(subTransaction),
          l => l.SubTransactionCreated(_clientTransaction, subTransaction),
          x => x.SubTransactionCreated(_clientTransaction, subTransaction),
          sequence =>
              _transactionEventReceiverMock
                  .InVerifiableSequence(sequence)
                  .Setup(
                      mock => mock.SubTransactionCreated(
                          _clientTransaction,
                          It.Is<SubTransactionCreatedEventArgs>(args => args.SubTransaction == subTransaction)))
                  .WithCurrentTransaction(_clientTransaction)
                  .Verifiable());
    }

    [Test]
    public void RaiseNewObjectCreatingEvent ()
    {
      CheckEventWithListenersFirst(
          s => s.RaiseNewObjectCreatingEvent(typeof(Order)),
          l => l.NewObjectCreating(_clientTransaction, typeof(Order)),
          x => x.NewObjectCreating(_clientTransaction, typeof(Order)));
    }

    [Test]
    public void RaiseObjectsLoadingEvent ()
    {
      var objectIDs = new[] { DomainObjectIDs.Order1 };
      CheckEventWithListenersFirst(
          s => s.RaiseObjectsLoadingEvent(objectIDs),
          l => l.ObjectsLoading(_clientTransaction, objectIDs),
          x => x.ObjectsLoading(_clientTransaction, objectIDs));
    }

    [Test]
    public void RaiseObjectsLoadedEvent ()
    {
      var domainObjects = new[] { _domainObject1, _domainObject2 };
      CheckEventWithListenersLast(
          s => s.RaiseObjectsLoadedEvent(domainObjects),
          l => l.ObjectsLoaded(_clientTransaction, domainObjects),
          x => x.ObjectsLoaded(_clientTransaction, domainObjects),
          sequence =>
          {
            _loadEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.OnLoaded(_domainObject1))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _loadEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.OnLoaded(_domainObject2))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(
                    mock => mock.Loaded(
                        _clientTransaction,
                        It.Is<ClientTransactionEventArgs>(args => args.DomainObjects.SequenceEqual(domainObjects))))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseObjectsNotFoundEvent ()
    {
      var domainObjects = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 };
      CheckEventWithListenersOnly(
          s => s.RaiseObjectsNotFoundEvent(domainObjects),
          l => l.ObjectsNotFound(_clientTransaction, domainObjects));
    }

    [Test]
    public void RaiseObjectsUnloadingEvent ()
    {
      var unloadedDomainObjects = new[] { _domainObject1, _domainObject2 };

      CheckEventWithListenersFirst(
          s => s.RaiseObjectsUnloadingEvent(unloadedDomainObjects),
          l => l.ObjectsUnloading(_clientTransaction, unloadedDomainObjects),
          x => x.ObjectsUnloading(_clientTransaction, unloadedDomainObjects),
          sequence =>
          {
            _unloadEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.OnUnloading(_domainObject1))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _unloadEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.OnUnloading(_domainObject2))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseObjectsUnloadedEvent ()
    {
      var unloadedDomainObjects = new[] { _domainObject1, _domainObject2 };

      CheckEventWithListenersLast(
          s => s.RaiseObjectsUnloadedEvent(unloadedDomainObjects),
          l => l.ObjectsUnloaded(_clientTransaction, unloadedDomainObjects),
          x => x.ObjectsUnloaded(_clientTransaction, unloadedDomainObjects),
          sequence =>
          {
            _unloadEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.OnUnloaded(_domainObject2))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _unloadEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.OnUnloaded(_domainObject1))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseObjectDeletingEvent ()
    {
      CheckEventWithListenersFirst(
          s => s.RaiseObjectDeletingEvent(_domainObject1),
          l => l.ObjectDeleting(_clientTransaction, _domainObject1),
          x => x.ObjectDeleting(_clientTransaction, _domainObject1),
          sequence =>
              _order1EventReceiverMock
                  .InVerifiableSequence(sequence)
                  .Setup(mock => mock.Deleting(_domainObject1, EventArgs.Empty))
                  .WithCurrentTransaction(_clientTransaction)
                  .Verifiable());
    }

    [Test]
    public void RaiseObjectDeletedEvent ()
    {
      CheckEventWithListenersLast(
          s => s.RaiseObjectDeletedEvent(_domainObject1),
          l => l.ObjectDeleted(_clientTransaction, _domainObject1),
          x => x.ObjectDeleted(_clientTransaction, _domainObject1),
          sequence =>
              _order1EventReceiverMock
                  .InVerifiableSequence(sequence)
                  .Setup(mock => mock.Deleted(_domainObject1, EventArgs.Empty))
                  .WithCurrentTransaction(_clientTransaction)
                  .Verifiable());
    }

    [Test]
    public void RaisePropertyValueReadingEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      CheckEventWithListenersFirst(
          s => s.RaisePropertyValueReadingEvent(_domainObject1, propertyDefinition, ValueAccess.Current),
          l => l.PropertyValueReading(_clientTransaction, _domainObject1, propertyDefinition, ValueAccess.Current),
          x => x.PropertyValueReading(_clientTransaction, _domainObject1, propertyDefinition, ValueAccess.Current));
    }

    [Test]
    public void RaisePropertyValueReadEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      CheckEventWithListenersLast(
          s => s.RaisePropertyValueReadEvent(_domainObject1, propertyDefinition, 17, ValueAccess.Current),
          l => l.PropertyValueRead(_clientTransaction, _domainObject1, propertyDefinition, 17, ValueAccess.Current),
          x => x.PropertyValueRead(_clientTransaction, _domainObject1, propertyDefinition, 17, ValueAccess.Current));
    }

    [Test]
    public void RaisePropertyValueRead_EventWithNull ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      CheckEventWithListenersLast(
          s => s.RaisePropertyValueReadEvent(_domainObject1, propertyDefinition, null, ValueAccess.Current),
          l => l.PropertyValueRead(_clientTransaction, _domainObject1, propertyDefinition, null, ValueAccess.Current),
          x => x.PropertyValueRead(_clientTransaction, _domainObject1, propertyDefinition, null, ValueAccess.Current));
    }

    [Test]
    public void RaisePropertyValueChangingEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersFirst(
          s => s.RaisePropertyValueChangingEvent(_domainObject1, propertyDefinition, oldValue, newValue),
          l => l.PropertyValueChanging(_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          x => x.PropertyValueChanging(_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          sequence => _order1EventReceiverMock
              .InVerifiableSequence(sequence)
              .SetupPropertyChanging(_domainObject1, propertyDefinition, oldValue, newValue)
              .WithCurrentTransaction(_clientTransaction)
              .Verifiable());
    }

    [Test]
    public void RaisePropertyValueChangingEvent_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();

      CheckEventWithListenersFirst(
          s => s.RaisePropertyValueChangingEvent(_domainObject1, propertyDefinition, null, null),
          l => l.PropertyValueChanging(_clientTransaction, _domainObject1, propertyDefinition, null, null),
          x => x.PropertyValueChanging(_clientTransaction, _domainObject1, propertyDefinition, null, null),
          sequence => _order1EventReceiverMock
              .InVerifiableSequence(sequence)
              .SetupPropertyChanging(_domainObject1, propertyDefinition, null, null)
              .WithCurrentTransaction(_clientTransaction)
              .Verifiable());
    }

    [Test]
    public void RaisePropertyValueChangedEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersLast(
          s => s.RaisePropertyValueChangedEvent(_domainObject1, propertyDefinition, oldValue, newValue),
          l => l.PropertyValueChanged(_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          x => x.PropertyValueChanged(_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          sequence => _order1EventReceiverMock
              .InVerifiableSequence(sequence)
              .SetupPropertyChanged(_domainObject1, propertyDefinition, oldValue, newValue)
              .WithCurrentTransaction(_clientTransaction)
              .Verifiable());
    }

    [Test]
    public void RaisePropertyValueChangedEvent_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();

      CheckEventWithListenersLast(
          s => s.RaisePropertyValueChangedEvent(_domainObject1, propertyDefinition, null, null),
          l => l.PropertyValueChanged(_clientTransaction, _domainObject1, propertyDefinition, null, null),
          x => x.PropertyValueChanged(_clientTransaction, _domainObject1, propertyDefinition, null, null),
          sequence => _order1EventReceiverMock
              .InVerifiableSequence(sequence)
              .SetupPropertyChanged(_domainObject1, propertyDefinition, null, null)
              .WithCurrentTransaction(_clientTransaction)
              .Verifiable());
    }

    [Test]
    public void RaiseRelationReadingEvent ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      CheckEventWithListenersFirst(
          s => s.RaiseRelationReadingEvent(_domainObject1, endPointDefinition, ValueAccess.Current),
          l => l.RelationReading(_clientTransaction, _domainObject1, endPointDefinition, ValueAccess.Current),
          x => x.RelationReading(_clientTransaction, _domainObject1, endPointDefinition, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationReadEvent_Object ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      CheckEventWithListenersLast(
          s => s.RaiseRelationReadEvent(_domainObject1, endPointDefinition, _domainObject2, ValueAccess.Current),
          l => l.RelationRead(_clientTransaction, _domainObject1, endPointDefinition, _domainObject2, ValueAccess.Current),
          x => x.RelationRead(_clientTransaction, _domainObject1, endPointDefinition, _domainObject2, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationReadEvent_Object_WithNull ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      CheckEventWithListenersLast(
          s => s.RaiseRelationReadEvent(_domainObject1, endPointDefinition, (DomainObject)null, ValueAccess.Current),
          l => l.RelationRead(_clientTransaction, _domainObject1, endPointDefinition, (DomainObject)null, ValueAccess.Current),
          x => x.RelationRead(_clientTransaction, _domainObject1, endPointDefinition, (DomainObject)null, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationReadEvent_Collection ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      var relatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(new DomainObjectCollection());
      CheckEventWithListenersLast(
          s => s.RaiseRelationReadEvent(_domainObject1, endPointDefinition, relatedObjects, ValueAccess.Current),
          l => l.RelationRead(_clientTransaction, _domainObject1, endPointDefinition, relatedObjects, ValueAccess.Current),
          x => x.RelationRead(_clientTransaction, _domainObject1, endPointDefinition, relatedObjects, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationChangingEvent ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();
      var oldValue = DomainObjectMother.CreateFakeObject();
      var newValue = DomainObjectMother.CreateFakeObject();

      CheckEventWithListenersFirst(
          s => s.RaiseRelationChangingEvent(_domainObject1, endPointDefinition, oldValue, newValue),
          l => l.RelationChanging(_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          x => x.RelationChanging(_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          sequence =>
              _order1EventReceiverMock
                  .InVerifiableSequence(sequence)
                  .SetupRelationChanging(_domainObject1, endPointDefinition, oldValue, newValue)
                  .WithCurrentTransaction(_clientTransaction)
                  .Verifiable());
    }

    [Test]
    public void RaiseRelationChangingEvent_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      CheckEventWithListenersFirst(
          s => s.RaiseRelationChangingEvent(_domainObject1, endPointDefinition, null, null),
          l => l.RelationChanging(_clientTransaction, _domainObject1, endPointDefinition, null, null),
          x => x.RelationChanging(_clientTransaction, _domainObject1, endPointDefinition, null, null),
          sequence => _order1EventReceiverMock
              .InVerifiableSequence(sequence)
              .SetupRelationChanging(_domainObject1, endPointDefinition, null, null)
              .WithCurrentTransaction(_clientTransaction)
              .Verifiable());
    }

    [Test]
    public void RaiseRelationChangedEvent ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();
      var oldValue = DomainObjectMother.CreateFakeObject();
      var newValue = DomainObjectMother.CreateFakeObject();

      CheckEventWithListenersLast(
          s => s.RaiseRelationChangedEvent(_domainObject1, endPointDefinition, oldValue, newValue),
          l => l.RelationChanged(_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          x => x.RelationChanged(_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          sequence =>
              _order1EventReceiverMock
                  .InVerifiableSequence(sequence)
                  .SetupRelationChanged(_domainObject1, endPointDefinition, oldValue, newValue)
                  .WithCurrentTransaction(_clientTransaction)
                  .Verifiable());
    }

    [Test]
    public void RaiseRelationChangedEvent_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      CheckEventWithListenersLast(
          s => s.RaiseRelationChangedEvent(_domainObject1, endPointDefinition, null, null),
          l => l.RelationChanged(_clientTransaction, _domainObject1, endPointDefinition, null, null),
          x => x.RelationChanged(_clientTransaction, _domainObject1, endPointDefinition, null, null),
          sequence =>
              _order1EventReceiverMock
                  .InVerifiableSequence(sequence)
                  .SetupRelationChanged(_domainObject1, endPointDefinition, null, null)
                  .WithCurrentTransaction(_clientTransaction)
                  .Verifiable());
    }

    [Test]
    public void RaiseFilterQueryResultEvent ()
    {
      var queryResult1 = QueryResultObjectMother.CreateQueryResult<Order>(StorageSettings);
      var queryResult2 = QueryResultObjectMother.CreateQueryResult<Order>(StorageSettings);
      var queryResult3 = QueryResultObjectMother.CreateQueryResult<Order>(StorageSettings);

      var sequence = new VerifiableSequence();
      _listenerMock.InVerifiableSequence(sequence).Setup(l => l.FilterQueryResult(_clientTransaction, queryResult1)).Returns(queryResult2).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(x => x.FilterQueryResult(_clientTransaction, queryResult2)).Returns(queryResult3).Verifiable();

      var result = _eventBroker.RaiseFilterQueryResultEvent(queryResult1);

      _order1EventReceiverMock.Verify();
      _order2EventReceiverMock.Verify();
      _invalidObjectEventReceiverMock.Verify();
      _unloadEventReceiverMock.Verify();
      _transactionEventReceiverMock.Verify();
      _loadEventReceiverMock.Verify();
      _extensionMock.Verify();
      _listenerMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(queryResult3));
    }

    [Test]
    public void RaiseFilterCustomQueryResultEvent ()
    {
      var query = QueryObjectMother.Create(StorageSettings);
      var queryResult1 = new[] { "one" };
      var queryResult2 = new[] { "two" };

      _listenerMock.Setup(l => l.FilterCustomQueryResult<object>(_clientTransaction, query, queryResult1)).Returns(queryResult2).Verifiable();

      var result = _eventBroker.RaiseFilterCustomQueryResultEvent<object>(query, queryResult1);

      _order1EventReceiverMock.Verify();
      _order2EventReceiverMock.Verify();
      _invalidObjectEventReceiverMock.Verify();
      _unloadEventReceiverMock.Verify();
      _transactionEventReceiverMock.Verify();
      _loadEventReceiverMock.Verify();
      _extensionMock.Verify();
      _listenerMock.Verify();
      Assert.That(result, Is.SameAs(queryResult2));
    }

    [Test]
    public void RaiseTransactionCommittingEvent ()
    {
      var eventRegistrar = Mock.Of<ICommittingEventRegistrar>();
      var changedDomainObjects = new[] { _domainObject1, _domainObject2 };

      CheckEventWithListenersFirst(
          s => s.RaiseTransactionCommittingEvent(changedDomainObjects, eventRegistrar),
          l => l.TransactionCommitting(_clientTransaction, changedDomainObjects, eventRegistrar),
          x => x.Committing(_clientTransaction, changedDomainObjects, eventRegistrar),
          sequence =>
          {
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(
                    mock => mock.Committing(
                        _clientTransaction,
                        It.Is<ClientTransactionCommittingEventArgs>(args => args.DomainObjects.SetEquals(changedDomainObjects) && args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _order1EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(
                    mock => mock.Committing(
                        _domainObject1,
                        It.Is<DomainObjectCommittingEventArgs>(args => args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _order2EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(
                    mock => mock.Committing(
                        _domainObject2,
                        It.Is<DomainObjectCommittingEventArgs>(args => args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseTransactionCommittingEvent_InvalidObject ()
    {
      var eventRegistrar = new Mock<ICommittingEventRegistrar>();
      var changedDomainObjects = new[] { _invalidDomainObject };
      CheckEventWithListenersFirst(
          s => s.RaiseTransactionCommittingEvent(changedDomainObjects, eventRegistrar.Object),
          l => l.TransactionCommitting(_clientTransaction, changedDomainObjects, eventRegistrar.Object),
          x => x.Committing(_clientTransaction, changedDomainObjects, eventRegistrar.Object),
          sequence =>
          {
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .SetupCommitting(_clientTransaction, new[] { _invalidDomainObject })
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });

      // DomainObject event should not be RaisedEvent if object is made invalid.
      _invalidObjectEventReceiverMock.Verify(mock => mock.Committing(It.IsAny<object>(), It.IsAny<DomainObjectCommittingEventArgs>()), Times.Never());
    }


    [Test]
    public void RaiseTransactionCommitValidateEvent ()
    {
      var data1 = PersistableDataObjectMother.Create();
      var data2 = PersistableDataObjectMother.Create();
      var committedData = new[] { data1, data2 };
      CheckEventWithListenersFirst(
          s => s.RaiseTransactionCommitValidateEvent(committedData),
          l => l.TransactionCommitValidate(_clientTransaction, committedData),
          x => x.CommitValidate(_clientTransaction, committedData));
    }

    [Test]
    public void RaiseTransactionCommittedEvent ()
    {
      var changedDomainObjects = new[] { _domainObject1, _domainObject2 };
      CheckEventWithListenersLast(
          s => s.RaiseTransactionCommittedEvent(changedDomainObjects),
          l => l.TransactionCommitted(_clientTransaction, changedDomainObjects),
          x => x.Committed(_clientTransaction, changedDomainObjects),
          sequence =>
          {
            _order2EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.Committed(_domainObject2, EventArgs.Empty))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _order1EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.Committed(_domainObject1, EventArgs.Empty))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .SetupCommitted(_clientTransaction, _domainObject1, _domainObject2)
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseTransactionRollingBackEvent ()
    {
      var changedDomainObjects = new[] { _domainObject1, _domainObject2 };
      CheckEventWithListenersFirst(
          s => s.RaiseTransactionRollingBackEvent(changedDomainObjects),
          l => l.TransactionRollingBack(_clientTransaction, changedDomainObjects),
          x => x.RollingBack(_clientTransaction, changedDomainObjects),
          sequence =>
          {
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .SetupRollingBack(_clientTransaction, _domainObject1, _domainObject2)
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _order1EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.RollingBack(_domainObject1, EventArgs.Empty))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _order2EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.RollingBack(_domainObject2, EventArgs.Empty))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseTransactionRollingBackEvent_InvalidObjects ()
    {
      CheckEventWithListenersFirst(
          s => s.RaiseTransactionRollingBackEvent(new[] { _invalidDomainObject }),
          l => l.TransactionRollingBack(_clientTransaction, new[] { _invalidDomainObject }),
          x => x.RollingBack(_clientTransaction, new[] { _invalidDomainObject }),
          sequence =>
          {
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .SetupRollingBack(_clientTransaction, new[] { _invalidDomainObject })
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });

      // DomainObject event should not be RaisedEvent if object is made invalid.
      _invalidObjectEventReceiverMock.Verify(mock => mock.RollingBack(It.IsAny<DomainObject>(), It.IsAny<EventArgs>()), Times.Never());
    }

    [Test]
    public void RaiseTransactionRolledBackEvent ()
    {
      var changedDomainObjects = new[] { _domainObject1, _domainObject2 };
      CheckEventWithListenersLast(
          s => s.RaiseTransactionRolledBackEvent(changedDomainObjects),
          l => l.TransactionRolledBack(_clientTransaction, changedDomainObjects),
          x => x.RolledBack(_clientTransaction, changedDomainObjects),
          sequence =>
          {
            _order2EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.RolledBack(_domainObject2, EventArgs.Empty))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _order1EventReceiverMock
                .InVerifiableSequence(sequence)
                .Setup(mock => mock.RolledBack(_domainObject1, EventArgs.Empty))
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
            _transactionEventReceiverMock
                .InVerifiableSequence(sequence)
                .SetupRolledBack(_clientTransaction, _domainObject1, _domainObject2)
                .WithCurrentTransaction(_clientTransaction)
                .Verifiable();
          });
    }

    [Test]
    public void RaiseRelationEndPointMapRegisteringEvent ()
    {
      var relationEndPoint = new Mock<IRelationEndPoint>();
      CheckEventWithListenersOnly(
          s => s.RaiseRelationEndPointMapRegisteringEvent(relationEndPoint.Object),
          l => l.RelationEndPointMapRegistering(_clientTransaction, relationEndPoint.Object));
    }

    [Test]
    public void RaiseRelationEndPointMapUnregisteringEvent ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName(typeof(Order), "OrderItems"));
      CheckEventWithListenersOnly(
          s => s.RaiseRelationEndPointMapUnregisteringEvent(endPointID),
          l => l.RelationEndPointMapUnregistering(_clientTransaction, endPointID));
    }

    [Test]
    public void RaiseRelationEndPointBecomingIncompleteEvent ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName(typeof(Order), "OrderItems"));
      CheckEventWithListenersOnly(
          s => s.RaiseRelationEndPointBecomingIncompleteEvent(endPointID),
          l => l.RelationEndPointBecomingIncomplete(_clientTransaction, endPointID));
    }

    [Test]
    public void RaiseObjectMarkedInvalidEvent ()
    {
      var domainObject = Order.NewObject();
      CheckEventWithListenersOnly(
          s => s.RaiseObjectMarkedInvalidEvent(domainObject),
          l => l.ObjectMarkedInvalid(_clientTransaction, domainObject));
    }

    [Test]
    public void RaiseObjectMarkedNotInvalidEvent ()
    {
      var domainObject = Order.NewObject();
      CheckEventWithListenersOnly(
          s => s.RaiseObjectMarkedNotInvalidEvent(domainObject),
          l => l.ObjectMarkedNotInvalid(_clientTransaction, domainObject));
    }

    [Test]
    public void RaiseDataContainerMapRegisteringEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create(Order.NewObject());
      CheckEventWithListenersOnly(
          s => s.RaiseDataContainerMapRegisteringEvent(dataContainer),
          l => l.DataContainerMapRegistering(_clientTransaction, dataContainer));
    }

    [Test]
    public void RaiseDataContainerMapUnregisteringEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create(Order.NewObject());
      CheckEventWithListenersOnly(
          s => s.RaiseDataContainerMapUnregisteringEvent(dataContainer),
          l => l.DataContainerMapUnregistering(_clientTransaction, dataContainer));
    }

    [Test]
    public void RaiseDataContainerStateUpdatedEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create(Order.NewObject());
      var newDataContainerState = new DataContainerState.Builder().SetNew().Value;
      CheckEventWithListenersOnly(
          s => s.RaiseDataContainerStateUpdatedEvent(dataContainer, newDataContainerState),
          l => l.DataContainerStateUpdated(_clientTransaction, dataContainer, newDataContainerState));
    }

    [Test]
    public void RaiseVirtualRelationEndPointStateUpdatedEvent ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName(typeof(Order), "OrderItems"));
      var newEndPointChangeState = BooleanObjectMother.GetRandomBoolean();
      CheckEventWithListenersOnly(
          s => s.RaiseVirtualRelationEndPointStateUpdatedEvent(endPointID, newEndPointChangeState),
          l => l.VirtualRelationEndPointStateUpdated(_clientTransaction, endPointID, newEndPointChangeState));
    }

    [Test]
    public void Serializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var instance = new ClientTransactionEventBroker(clientTransaction);
      instance.AddListener(new SerializableClientTransactionListenerFake());
      instance.Extensions.Add(new SerializableClientTransactionExtensionFake("bla"));

      var deserializedInstance = Serializer.SerializeAndDeserialize(instance);

      Assert.That(deserializedInstance.ClientTransaction, Is.Not.Null);

      Assert.That(deserializedInstance.Listeners, Is.Not.Empty);
      Assert.That(deserializedInstance.Extensions, Is.Not.Empty);
    }

    [Test]
    public void InactiveTransactionIsActivated_ForEvents ()
    {
      var inactiveClientTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionTestHelper.MakeInactive(inactiveClientTransaction))
      {
        Assert.That(inactiveClientTransaction.ActiveTransaction, Is.Not.SameAs(inactiveClientTransaction));

        var transactionEventReceiverMock = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, inactiveClientTransaction);
        transactionEventReceiverMock
            .Setup(mock => mock.SubTransactionCreated(It.IsAny<ClientTransaction>(), It.IsAny<SubTransactionCreatedEventArgs>()))
            .Callback(
                (object sender, SubTransactionCreatedEventArgs args) =>
                {
                  Assert.That(ClientTransaction.Current, Is.SameAs(inactiveClientTransaction));
                  Assert.That(ClientTransaction.Current.ActiveTransaction, Is.SameAs(inactiveClientTransaction));
                })
            .Verifiable();

        var eventBroker = new ClientTransactionEventBroker(inactiveClientTransaction);
        eventBroker.RaiseSubTransactionCreatedEvent(ClientTransactionObjectMother.Create());

        transactionEventReceiverMock.Verify();
      }
    }

    [Test]
    public void InactiveTransactionIsActivated_ForEvents_EvenWhenAlreadyCurrent ()
    {
      var inactiveClientTransaction = ClientTransaction.CreateRootTransaction();

      using (inactiveClientTransaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive(inactiveClientTransaction))
        {
          var transactionEventReceiverMock = ClientTransactionMockEventReceiver.CreateMock(MockBehavior.Strict, inactiveClientTransaction);
          transactionEventReceiverMock
              .Setup(mock => mock.SubTransactionCreated(It.IsAny<ClientTransaction>(), It.IsAny<SubTransactionCreatedEventArgs>()))
              .Callback(
                  (object sender, SubTransactionCreatedEventArgs args) =>
                  {
                    Assert.That(ClientTransaction.Current, Is.SameAs(inactiveClientTransaction));
                    Assert.That(ClientTransaction.Current.ActiveTransaction, Is.SameAs(inactiveClientTransaction));
                  })
              .Verifiable();

          var eventBroker = new ClientTransactionEventBroker(inactiveClientTransaction);
          eventBroker.RaiseSubTransactionCreatedEvent(ClientTransactionObjectMother.Create());

          transactionEventReceiverMock.Verify();
        }
      }
    }

    private void CheckEventWithListenersLast (
        Action<IClientTransactionEventSink> raiseAction,
        Expression<Action<IClientTransactionListener>> listenerEvent,
        Expression<Action<IClientTransactionExtension>> extensionEvent,
        Action<VerifiableSequence> orderedPreListenerExpectations = null)
    {
      var sequence = new VerifiableSequence();
      orderedPreListenerExpectations?.Invoke(sequence);
      _extensionMock.InVerifiableSequence(sequence).Setup(extensionEvent).Verifiable();
      _listenerMock.InVerifiableSequence(sequence).Setup(listenerEvent).Verifiable();

      raiseAction(_eventBroker);

      _order1EventReceiverMock.Verify();
      _order2EventReceiverMock.Verify();
      _invalidObjectEventReceiverMock.Verify();
      _unloadEventReceiverMock.Verify();
      _transactionEventReceiverMock.Verify();
      _loadEventReceiverMock.Verify();
      _extensionMock.Verify();
      _listenerMock.Verify();
      sequence.Verify();
    }

    private void CheckEventWithListenersFirst (
        Action<IClientTransactionEventSink> raiseAction,
        Expression<Action<IClientTransactionListener>> listenerEvent,
        Expression<Action<IClientTransactionExtension>> extensionEvent,
        Action<VerifiableSequence> orderedPostListenerExpectations = null)
    {
      var sequence = new VerifiableSequence();
      _listenerMock.InVerifiableSequence(sequence).Setup(listenerEvent).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(extensionEvent).Verifiable();
      orderedPostListenerExpectations?.Invoke(sequence);

      raiseAction(_eventBroker);

      _order1EventReceiverMock.Verify();
      _order2EventReceiverMock.Verify();
      _invalidObjectEventReceiverMock.Verify();
      _unloadEventReceiverMock.Verify();
      _transactionEventReceiverMock.Verify();
      _loadEventReceiverMock.Verify();
      _extensionMock.Verify();
      _listenerMock.Verify();
      sequence.Verify();
    }

    private void CheckEventWithListenersOnly (
        Action<IClientTransactionEventSink> raiseAction,
        Expression<Action<IClientTransactionListener>> listenerEvent)
    {
      _listenerMock.Setup(listenerEvent).Verifiable();

      raiseAction(_eventBroker);

      _order1EventReceiverMock.Verify();
      _order2EventReceiverMock.Verify();
      _invalidObjectEventReceiverMock.Verify();
      _unloadEventReceiverMock.Verify();
      _transactionEventReceiverMock.Verify();
      _loadEventReceiverMock.Verify();
      _extensionMock.Verify();
      _listenerMock.Verify();
    }
  }
}
