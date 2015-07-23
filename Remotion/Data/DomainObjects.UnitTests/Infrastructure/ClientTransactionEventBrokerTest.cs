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
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.FunctionalProgramming;
using Rhino.Mocks;

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

    private MockRepository _mockRepository;

    private DomainObjectMockEventReceiver _order1EventReceiverMock;
    private DomainObjectMockEventReceiver _order2EventReceiverMock;
    private DomainObjectMockEventReceiver _invalidObjectEventReceiverMock;

    private IUnloadEventReceiver _unloadEventReceiverMock;
    private ILoadEventReceiver _loadEventReceiverMock;
    
    private ClientTransactionMockEventReceiver _transactionEventReceiverMock;
    
    private IClientTransactionExtension _extensionMock;
    private IClientTransactionListener _listenerMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();

      _eventBroker = new ClientTransactionEventBroker (_clientTransaction);

      _domainObject1 = _clientTransaction.ExecuteInScope (() => DomainObjectIDs.Order1.GetObject<Order> ());
      _domainObject2 = _clientTransaction.ExecuteInScope (() => DomainObjectIDs.Order3.GetObject<Order> ());
      _invalidDomainObject = _clientTransaction.ExecuteInScope (
          () =>
          {
            var order = Order.NewObject ();
            order.Delete ();
            return order;
          });

      _mockRepository = new MockRepository ();
      _order1EventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_domainObject1);
      _order2EventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_domainObject2);
      _invalidObjectEventReceiverMock = _mockRepository.StrictMock<DomainObjectMockEventReceiver> (_invalidDomainObject);

      _unloadEventReceiverMock = _mockRepository.StrictMock<IUnloadEventReceiver> ();
      ((TestDomainBase) _domainObject1).SetUnloadEventReceiver (_unloadEventReceiverMock);
      ((TestDomainBase) _domainObject2).SetUnloadEventReceiver (_unloadEventReceiverMock);

      _transactionEventReceiverMock = _mockRepository.StrictMock<ClientTransactionMockEventReceiver> (_clientTransaction);

      _loadEventReceiverMock = _mockRepository.StrictMock<ILoadEventReceiver> ();
      ((TestDomainBase) _domainObject1).SetLoadEventReceiver (_loadEventReceiverMock);
      ((TestDomainBase) _domainObject2).SetLoadEventReceiver (_loadEventReceiverMock);

      _extensionMock = _mockRepository.StrictMock<IClientTransactionExtension> ();
      _extensionMock.Stub (stub => stub.Key).Return ("extension");
      _extensionMock.Replay ();
      _eventBroker.Extensions.Add (_extensionMock);
      _extensionMock.BackToRecord ();

      _listenerMock = _mockRepository.StrictMock<IClientTransactionListener> ();
      _eventBroker.AddListener (_listenerMock);
    }

    public override void TearDown ()
    {
      _clientTransaction.Discard();

      base.TearDown ();
    }

    [Test]
    public void Extensions ()
    {
      Assert.That (_eventBroker.Extensions, Has.Member (_extensionMock));

      var fakeExtension = ClientTransactionExtensionObjectMother.Create();

      Assert.That (_eventBroker.Extensions, Has.No.Member (fakeExtension));

      _eventBroker.Extensions.Add (fakeExtension);

      Assert.That (_eventBroker.Extensions, Has.Member (fakeExtension));
    }

    [Test]
    public void AddListener()
    {
      var fakeListener = ClientTransactionListenerObjectMother.Create();
      Assert.That (_eventBroker.Listeners, Has.No.Member (fakeListener));

      _eventBroker.AddListener (fakeListener);

      Assert.That (_eventBroker.Listeners, Has.Member (fakeListener));
    }

    [Test]
    public void RemoveListener ()
    {
      Assert.That (_eventBroker.Listeners, Has.Member (_listenerMock));

      _eventBroker.RemoveListener (_listenerMock);

      Assert.That (_eventBroker.Listeners, Has.No.Member (_listenerMock));
    }

    [Test]
    public void RaiseTransactionInitializeEvent ()
    {
      CheckEventWithListenersFirst (
        s => s.RaiseTransactionInitializeEvent (),
        l => l.TransactionInitialize (_clientTransaction),
        x => x.TransactionInitialize (_clientTransaction));
    }

    [Test]
    public void RaiseTransactionDiscardEvent ()
    {
      CheckEventWithListenersFirst (
          s => s.RaiseTransactionDiscardEvent (),
          l => l.TransactionDiscard (_clientTransaction),
          x => x.TransactionDiscard (_clientTransaction));
    }

    [Test]
    public void RaiseSubTransactionCreatingEvent ()
    {
      CheckEventWithListenersFirst (
          s => s.RaiseSubTransactionCreatingEvent (),
          l => l.SubTransactionCreating (_clientTransaction),
          x => x.SubTransactionCreating (_clientTransaction));
    }

    [Test]
    public void RaiseSubTransactionInitializeEvent ()
    {
      var subTransaction = ClientTransactionObjectMother.Create ();
      CheckEventWithListenersFirst (
          s => s.RaiseSubTransactionInitializeEvent (subTransaction),
          l => l.SubTransactionInitialize (_clientTransaction, subTransaction),
          x => x.SubTransactionInitialize (_clientTransaction, subTransaction));
    }

    [Test]
    public void RaiseSubTransactionCreatedEvent ()
    {
      var subTransaction = ClientTransactionObjectMother.Create ();

      CheckEventWithListenersLast (
          s => s.RaiseSubTransactionCreatedEvent (subTransaction),
          l => l.SubTransactionCreated (_clientTransaction, subTransaction),
          x => x.SubTransactionCreated (_clientTransaction, subTransaction),
          () =>
          _transactionEventReceiverMock
              .Expect (
                  mock => mock.SubTransactionCreated (
                      Arg.Is (_clientTransaction),
                      Arg<SubTransactionCreatedEventArgs>.Matches (args => args.SubTransaction == subTransaction)))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseNewObjectCreatingEvent ()
    {
      CheckEventWithListenersFirst (
          s => s.RaiseNewObjectCreatingEvent (typeof (Order)),
          l => l.NewObjectCreating (_clientTransaction, typeof (Order)),
          x => x.NewObjectCreating (_clientTransaction, typeof (Order)));
    }

    [Test]
    public void RaiseObjectsLoadingEvent ()
    {
      var objectIDs = Array.AsReadOnly (new[] { DomainObjectIDs.Order1 });
      CheckEventWithListenersFirst (
          s => s.RaiseObjectsLoadingEvent (objectIDs),
          l => l.ObjectsLoading (_clientTransaction, objectIDs),
          x => x.ObjectsLoading (_clientTransaction, objectIDs));
    }

    [Test]
    public void RaiseObjectsLoadedEvent ()
    {
      var domainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });
      CheckEventWithListenersLast (
          s => s.RaiseObjectsLoadedEvent (domainObjects),
          l => l.ObjectsLoaded (_clientTransaction, domainObjects),
          x => x.ObjectsLoaded (_clientTransaction, domainObjects),
          () =>
          {
            _loadEventReceiverMock.Expect (mock => mock.OnLoaded (_domainObject1)).WithCurrentTransaction (_clientTransaction);
            _loadEventReceiverMock.Expect (mock => mock.OnLoaded (_domainObject2)).WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (
                    mock => mock.Loaded (
                        Arg.Is (_clientTransaction),
                        Arg<ClientTransactionEventArgs>.Matches (args => args.DomainObjects.SequenceEqual (domainObjects))))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void RaiseObjectsNotFoundEvent ()
    {
      var domainObjects = Array.AsReadOnly (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });
      CheckEventWithListenersOnly (
          s => s.RaiseObjectsNotFoundEvent (domainObjects),
          l => l.ObjectsNotFound (_clientTransaction, domainObjects));
    }

    [Test]
    public void RaiseObjectsUnloadingEvent ()
    {
      var unloadedDomainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });

      CheckEventWithListenersFirst (
          s => s.RaiseObjectsUnloadingEvent (unloadedDomainObjects),
          l => l.ObjectsUnloading (_clientTransaction, unloadedDomainObjects),
          x => x.ObjectsUnloading (_clientTransaction, unloadedDomainObjects),
          () =>
          {
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloading (_domainObject1))
                .WithCurrentTransaction (_clientTransaction);
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloading (_domainObject2))
                .WithCurrentTransaction (_clientTransaction);
          });

    }

    [Test]
    public void RaiseObjectsUnloadedEvent ()
    {
      var unloadedDomainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });

      CheckEventWithListenersLast (
          s => s.RaiseObjectsUnloadedEvent (unloadedDomainObjects),
          l => l.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects),
          x => x.ObjectsUnloaded (_clientTransaction, unloadedDomainObjects),
          () =>
          {
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloaded (_domainObject2))
                .WithCurrentTransaction (_clientTransaction);
            _unloadEventReceiverMock
                .Expect (mock => mock.OnUnloaded (_domainObject1))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void RaiseObjectDeletingEvent ()
    {
      CheckEventWithListenersFirst (
          s => s.RaiseObjectDeletingEvent (_domainObject1),
          l => l.ObjectDeleting (_clientTransaction, _domainObject1),
          x => x.ObjectDeleting (_clientTransaction, _domainObject1),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.Deleting (_domainObject1, EventArgs.Empty))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseObjectDeletedEvent ()
    {
      CheckEventWithListenersLast (
          s => s.RaiseObjectDeletedEvent (_domainObject1),
          l => l.ObjectDeleted (_clientTransaction, _domainObject1),
          x => x.ObjectDeleted (_clientTransaction, _domainObject1),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.Deleted (_domainObject1, EventArgs.Empty))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaisePropertyValueReadingEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      CheckEventWithListenersFirst (
          s => s.RaisePropertyValueReadingEvent (_domainObject1, propertyDefinition, ValueAccess.Current),
          l => l.PropertyValueReading (_clientTransaction, _domainObject1, propertyDefinition, ValueAccess.Current),
          x => x.PropertyValueReading (_clientTransaction, _domainObject1, propertyDefinition, ValueAccess.Current));
    }

    [Test]
    public void RaisePropertyValueReadEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      CheckEventWithListenersLast (
          s => s.RaisePropertyValueReadEvent (_domainObject1, propertyDefinition, 17, ValueAccess.Current),
          l => l.PropertyValueRead (_clientTransaction, _domainObject1, propertyDefinition, 17, ValueAccess.Current),
          x => x.PropertyValueRead (_clientTransaction, _domainObject1, propertyDefinition, 17, ValueAccess.Current));
    }

    [Test]
    public void RaisePropertyValueRead_EventWithNull ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      CheckEventWithListenersLast (
          s => s.RaisePropertyValueReadEvent (_domainObject1, propertyDefinition, null, ValueAccess.Current),
          l => l.PropertyValueRead (_clientTransaction, _domainObject1, propertyDefinition, null, ValueAccess.Current),
          x => x.PropertyValueRead (_clientTransaction, _domainObject1, propertyDefinition, null, ValueAccess.Current));
    }

    [Test]
    public void RaisePropertyValueChangingEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersFirst (
          s => s.RaisePropertyValueChangingEvent (_domainObject1, propertyDefinition, oldValue, newValue),
          l => l.PropertyValueChanging (_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          x => x.PropertyValueChanging (_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanging (propertyDefinition, oldValue, newValue))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaisePropertyValueChangingEvent_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();

      CheckEventWithListenersFirst (
          s => s.RaisePropertyValueChangingEvent (_domainObject1, propertyDefinition, null, null),
          l => l.PropertyValueChanging (_clientTransaction, _domainObject1, propertyDefinition, null, null),
          x => x.PropertyValueChanging (_clientTransaction, _domainObject1, propertyDefinition, null, null),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanging (propertyDefinition, null, null))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaisePropertyValueChangedEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
      object oldValue = "old";
      object newValue = "new";

      CheckEventWithListenersLast (
          s => s.RaisePropertyValueChangedEvent (_domainObject1, propertyDefinition, oldValue, newValue),
          l => l.PropertyValueChanged (_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          x => x.PropertyValueChanged (_clientTransaction, _domainObject1, propertyDefinition, oldValue, newValue),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanged (propertyDefinition, oldValue, newValue))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaisePropertyValueChangedEvent_WithNulls ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();

      CheckEventWithListenersLast (
          s => s.RaisePropertyValueChangedEvent (_domainObject1, propertyDefinition, null, null),
          l => l.PropertyValueChanged (_clientTransaction, _domainObject1, propertyDefinition, null, null),
          x => x.PropertyValueChanged (_clientTransaction, _domainObject1, propertyDefinition, null, null),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.PropertyChanged (propertyDefinition, null, null))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseRelationReadingEvent ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersFirst (
          s => s.RaiseRelationReadingEvent (_domainObject1, endPointDefinition, ValueAccess.Current),
          l => l.RelationReading (_clientTransaction, _domainObject1, endPointDefinition, ValueAccess.Current),
          x => x.RelationReading (_clientTransaction, _domainObject1, endPointDefinition, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationReadEvent_Object ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersLast (
          s => s.RaiseRelationReadEvent (_domainObject1, endPointDefinition, _domainObject2, ValueAccess.Current),
          l => l.RelationRead (_clientTransaction, _domainObject1, endPointDefinition, _domainObject2, ValueAccess.Current),
          x => x.RelationRead (_clientTransaction, _domainObject1, endPointDefinition, _domainObject2, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationReadEvent_Object_WithNull ()
    {
      var endPointDefinition = GetSomeEndPointDefinition();

      CheckEventWithListenersLast (
          s => s.RaiseRelationReadEvent (_domainObject1, endPointDefinition, (DomainObject) null, ValueAccess.Current),
          l => l.RelationRead (_clientTransaction, _domainObject1, endPointDefinition, (DomainObject) null, ValueAccess.Current),
          x => x.RelationRead (_clientTransaction, _domainObject1, endPointDefinition, (DomainObject) null, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationReadEvent_Collection ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      var relatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (new DomainObjectCollection ());
      CheckEventWithListenersLast (
          s => s.RaiseRelationReadEvent (_domainObject1, endPointDefinition, relatedObjects, ValueAccess.Current),
          l => l.RelationRead (_clientTransaction, _domainObject1, endPointDefinition, relatedObjects, ValueAccess.Current),
          x => x.RelationRead (_clientTransaction, _domainObject1, endPointDefinition, relatedObjects, ValueAccess.Current));
    }

    [Test]
    public void RaiseRelationChangingEvent ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();
      var oldValue = DomainObjectMother.CreateFakeObject ();
      var newValue = DomainObjectMother.CreateFakeObject ();

      CheckEventWithListenersFirst (
          s => s.RaiseRelationChangingEvent (_domainObject1, endPointDefinition, oldValue, newValue),
          l => l.RelationChanging (_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          x => x.RelationChanging (_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanging (endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseRelationChangingEvent_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersFirst (
          s => s.RaiseRelationChangingEvent (_domainObject1, endPointDefinition, null, null),
          l => l.RelationChanging (_clientTransaction, _domainObject1, endPointDefinition, null, null),
          x => x.RelationChanging (_clientTransaction, _domainObject1, endPointDefinition, null, null),
          () => _order1EventReceiverMock
                    .Expect (mock => mock.RelationChanging (endPointDefinition, null, null))
                    .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseRelationChangedEvent ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();
      var oldValue = DomainObjectMother.CreateFakeObject ();
      var newValue = DomainObjectMother.CreateFakeObject ();

      CheckEventWithListenersLast (
          s => s.RaiseRelationChangedEvent (_domainObject1, endPointDefinition, oldValue, newValue),
          l => l.RelationChanged (_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          x => x.RelationChanged (_clientTransaction, _domainObject1, endPointDefinition, oldValue, newValue),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanged (endPointDefinition, oldValue, newValue))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseRelationChangedEvent_WithNulls ()
    {
      var endPointDefinition = GetSomeEndPointDefinition ();

      CheckEventWithListenersLast (
          s => s.RaiseRelationChangedEvent (_domainObject1, endPointDefinition, null, null),
          l => l.RelationChanged (_clientTransaction, _domainObject1, endPointDefinition, null, null),
          x => x.RelationChanged (_clientTransaction, _domainObject1, endPointDefinition, null, null),
          () =>
          _order1EventReceiverMock
              .Expect (mock => mock.RelationChanged (endPointDefinition, null, null))
              .WithCurrentTransaction (_clientTransaction));
    }

    [Test]
    public void RaiseFilterQueryResultEvent ()
    {
      var queryResult1 = QueryResultObjectMother.CreateQueryResult<Order> ();
      var queryResult2 = QueryResultObjectMother.CreateQueryResult<Order> ();
      var queryResult3 = QueryResultObjectMother.CreateQueryResult<Order> ();

      using (_mockRepository.Ordered ())
      {
        _listenerMock.Expect (l => l.FilterQueryResult (_clientTransaction, queryResult1)).Return (queryResult2);
        _extensionMock.Expect (x => x.FilterQueryResult (_clientTransaction, queryResult2)).Return (queryResult3);
      }
      _mockRepository.ReplayAll ();

      var result = _eventBroker.RaiseFilterQueryResultEvent (queryResult1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (queryResult3));
    }

    [Test]
    public void RaiseFilterCustomQueryResultEvent ()
    {
      var query = QueryObjectMother.Create();
      var queryResult1 = new[]{ "one" };
      var queryResult2 = new[] { "two" };

      _listenerMock.Expect (l => l.FilterCustomQueryResult<object> (_clientTransaction, query, queryResult1)).Return (queryResult2);
      _mockRepository.ReplayAll ();

      var result = _eventBroker.RaiseFilterCustomQueryResultEvent<object> (query, queryResult1);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.SameAs (queryResult2));
    }

    [Test]
    public void RaiseTransactionCommittingEvent ()
    {
      var eventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar>();
      var changedDomainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });

      CheckEventWithListenersFirst (
          s => s.RaiseTransactionCommittingEvent (changedDomainObjects, eventRegistrar),
          l => l.TransactionCommitting (_clientTransaction, changedDomainObjects, eventRegistrar),
          x => x.Committing (_clientTransaction, changedDomainObjects, eventRegistrar),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (
                    mock =>
                    mock.Committing (
                        Arg.Is (_clientTransaction),
                        Arg<ClientTransactionCommittingEventArgs>.Matches (
                            args => args.DomainObjects.SetEquals (changedDomainObjects) && args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (
                    mock => mock.Committing (
                        Arg.Is (_domainObject1),
                        Arg<DomainObjectCommittingEventArgs>.Matches (args => args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction (_clientTransaction);
            _order2EventReceiverMock
                .Expect (
                    mock => mock.Committing (
                        Arg.Is (_domainObject2),
                        Arg<DomainObjectCommittingEventArgs>.Matches (args => args.EventRegistrar == eventRegistrar)))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void RaiseTransactionCommittingEvent_InvalidObject ()
    {
      var eventRegistrar = MockRepository.GenerateStub<ICommittingEventRegistrar>();
      var changedDomainObjects = Array.AsReadOnly (new[] { _invalidDomainObject });
      CheckEventWithListenersFirst (
          s => s.RaiseTransactionCommittingEvent (changedDomainObjects, eventRegistrar),
          l => l.TransactionCommitting (_clientTransaction, changedDomainObjects, eventRegistrar),
          x => x.Committing (_clientTransaction, changedDomainObjects, eventRegistrar),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.Committing (new[] { _invalidDomainObject }))
                .WithCurrentTransaction (_clientTransaction);
            _invalidObjectEventReceiverMock
                .Expect (mock => mock.Committing (Arg<object>.Is.Anything, Arg<DomainObjectCommittingEventArgs>.Is.Anything))
                .Repeat.Never()
                .Message ("DomainObject event should not be RaisedEvent if object is made invalid.");
          });
    }


    [Test]
    public void RaiseTransactionCommitValidateEvent ()
    {
      var data1 = PersistableDataObjectMother.Create();
      var data2 = PersistableDataObjectMother.Create();
      var committedData = Array.AsReadOnly (new[] { data1, data2 });
      CheckEventWithListenersFirst (
          s => s.RaiseTransactionCommitValidateEvent (committedData),
          l => l.TransactionCommitValidate (_clientTransaction, committedData),
          x => x.CommitValidate (_clientTransaction, committedData));
    }

    [Test]
    public void RaiseTransactionCommittedEvent ()
    {
      var changedDomainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });
      CheckEventWithListenersLast (
          s => s.RaiseTransactionCommittedEvent (changedDomainObjects),
          l => l.TransactionCommitted (_clientTransaction, changedDomainObjects),
          x => x.Committed (_clientTransaction, changedDomainObjects),
          () =>
          {
            _order2EventReceiverMock
                .Expect (mock => mock.Committed (_domainObject2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.Committed (_domainObject1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _transactionEventReceiverMock
                .Expect (mock => mock.Committed (_domainObject1, _domainObject2))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void RaiseTransactionRollingBackEvent ()
    {
      var changedDomainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });
      CheckEventWithListenersFirst (
          s => s.RaiseTransactionRollingBackEvent (changedDomainObjects),
          l => l.TransactionRollingBack (_clientTransaction, changedDomainObjects),
          x => x.RollingBack (_clientTransaction, changedDomainObjects),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.RollingBack (_domainObject1, _domainObject2))
                .WithCurrentTransaction (_clientTransaction);
            _order1EventReceiverMock
                .Expect (mock => mock.RollingBack (_domainObject1, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
            _order2EventReceiverMock
                .Expect (mock => mock.RollingBack (_domainObject2, EventArgs.Empty))
                .WithCurrentTransaction (_clientTransaction);
          });
    }

    [Test]
    public void RaiseTransactionRollingBackEvent_InvalidObjects ()
    {
      CheckEventWithListenersFirst (
          s => s.RaiseTransactionRollingBackEvent (Array.AsReadOnly (new[] { _invalidDomainObject })),
          l => l.TransactionRollingBack (_clientTransaction, Array.AsReadOnly (new[] { _invalidDomainObject })),
          x => x.RollingBack (_clientTransaction, Array.AsReadOnly (new[] { _invalidDomainObject })),
          () =>
          {
            _transactionEventReceiverMock
                .Expect (mock => mock.RollingBack (new[] { _invalidDomainObject }))
                .WithCurrentTransaction (_clientTransaction);
            _invalidObjectEventReceiverMock
                .Expect (mock => mock.RollingBack (Arg<DomainObject>.Is.Anything, Arg<EventArgs>.Is.Anything))
                .Repeat.Never ()
                .Message ("DomainObject event should not be RaisedEvent if object is made invalid.");
          });
    }

    [Test]
    public void RaiseTransactionRolledBackEvent ()
    {
      var changedDomainObjects = Array.AsReadOnly (new[] { _domainObject1, _domainObject2 });
      CheckEventWithListenersLast (
          s => s.RaiseTransactionRolledBackEvent (changedDomainObjects),
              l => l.TransactionRolledBack (_clientTransaction, changedDomainObjects),
              x => x.RolledBack (_clientTransaction, changedDomainObjects),
              () =>
              {
                _order2EventReceiverMock
                    .Expect (mock => mock.RolledBack (_domainObject2, EventArgs.Empty))
                    .WithCurrentTransaction (_clientTransaction);
                _order1EventReceiverMock
                    .Expect (mock => mock.RolledBack (_domainObject1, EventArgs.Empty))
                    .WithCurrentTransaction (_clientTransaction);
                _transactionEventReceiverMock
                    .Expect (mock => mock.RolledBack (_domainObject1, _domainObject2))
                    .WithCurrentTransaction (_clientTransaction);
              });
    }

    [Test]
    public void RaiseRelationEndPointMapRegisteringEvent ()
    {
      var relationEndPoint = MockRepository.GenerateStub<IRelationEndPoint> ();
      CheckEventWithListenersOnly (
          s => s.RaiseRelationEndPointMapRegisteringEvent (relationEndPoint), 
          l => l.RelationEndPointMapRegistering (_clientTransaction, relationEndPoint));
    }

    [Test]
    public void RaiseRelationEndPointMapUnregisteringEvent ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      CheckEventWithListenersOnly (
          s => s.RaiseRelationEndPointMapUnregisteringEvent (endPointID),
          l => l.RelationEndPointMapUnregistering (_clientTransaction, endPointID));
    }

    [Test]
    public void RaiseRelationEndPointBecomingIncompleteEvent ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      CheckEventWithListenersOnly (
          s => s.RaiseRelationEndPointBecomingIncompleteEvent (endPointID),
          l => l.RelationEndPointBecomingIncomplete (_clientTransaction, endPointID));
    }

    [Test]
    public void RaiseObjectMarkedInvalidEvent ()
    {
      var domainObject = Order.NewObject ();
      CheckEventWithListenersOnly (
          s => s.RaiseObjectMarkedInvalidEvent (domainObject),
          l => l.ObjectMarkedInvalid (_clientTransaction, domainObject));
    }

    [Test]
    public void RaiseObjectMarkedNotInvalidEvent ()
    {
      var domainObject = Order.NewObject ();
      CheckEventWithListenersOnly (
          s => s.RaiseObjectMarkedNotInvalidEvent (domainObject),
          l => l.ObjectMarkedNotInvalid (_clientTransaction, domainObject));
    }

    [Test]
    public void RaiseDataContainerMapRegisteringEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create (Order.NewObject ());
      CheckEventWithListenersOnly (
          s => s.RaiseDataContainerMapRegisteringEvent (dataContainer),
          l => l.DataContainerMapRegistering (_clientTransaction, dataContainer));
    }

    [Test]
    public void RaiseDataContainerMapUnregisteringEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create (Order.NewObject ());
      CheckEventWithListenersOnly (
          s => s.RaiseDataContainerMapUnregisteringEvent (dataContainer),
          l => l.DataContainerMapUnregistering (_clientTransaction, dataContainer));
    }

    [Test]
    public void RaiseDataContainerStateUpdatedEvent ()
    {
      var dataContainer = DataContainerObjectMother.Create (Order.NewObject());
      var newDataContainerState = StateType.New;
      CheckEventWithListenersOnly (
          s => s.RaiseDataContainerStateUpdatedEvent (dataContainer, newDataContainerState),
          l => l.DataContainerStateUpdated (_clientTransaction, dataContainer, newDataContainerState));
    }

    [Test]
    public void RaiseVirtualRelationEndPointStateUpdatedEvent ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderItems"));
      var newEndPointChangeState = BooleanObjectMother.GetRandomBoolean();
      CheckEventWithListenersOnly (
          s => s.RaiseVirtualRelationEndPointStateUpdatedEvent (endPointID, newEndPointChangeState),
          l => l.VirtualRelationEndPointStateUpdated (_clientTransaction, endPointID, newEndPointChangeState));
    }

    [Test]
    public void Serializable ()
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      var instance = new ClientTransactionEventBroker (clientTransaction);
      instance.AddListener (new SerializableClientTransactionListenerFake ());
      instance.Extensions.Add (new SerializableClientTransactionExtensionFake ("bla"));

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);

      Assert.That (deserializedInstance.Listeners, Is.Not.Empty);
      Assert.That (deserializedInstance.Extensions, Is.Not.Empty);
    }

    [Test]
    public void InactiveTransactionIsActivated_ForEvents ()
    {
      var inactiveClientTransaction = ClientTransaction.CreateRootTransaction ();
      using (ClientTransactionTestHelper.MakeInactive (inactiveClientTransaction))
      {
        Assert.That (inactiveClientTransaction.ActiveTransaction, Is.Not.SameAs (inactiveClientTransaction));

        var transactionEventReceiverMock = MockRepository.GenerateStrictMock<ClientTransactionMockEventReceiver> (inactiveClientTransaction);
        transactionEventReceiverMock
            .Expect (mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<SubTransactionCreatedEventArgs>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  Assert.That (ClientTransaction.Current, Is.SameAs (inactiveClientTransaction));
                  Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (inactiveClientTransaction));
                });

        var eventBroker = new ClientTransactionEventBroker (inactiveClientTransaction);
        eventBroker.RaiseSubTransactionCreatedEvent (ClientTransactionObjectMother.Create());

        transactionEventReceiverMock.VerifyAllExpectations();
      }
    }

    [Test]
    public void InactiveTransactionIsActivated_ForEvents_EvenWhenAlreadyCurrent ()
    {
      var inactiveClientTransaction = ClientTransaction.CreateRootTransaction ();

      using (inactiveClientTransaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive (inactiveClientTransaction))
        {
          var transactionEventReceiverMock = MockRepository.GenerateStrictMock<ClientTransactionMockEventReceiver> (inactiveClientTransaction);
          transactionEventReceiverMock
              .Expect (mock => mock.SubTransactionCreated (Arg<ClientTransaction>.Is.Anything, Arg<SubTransactionCreatedEventArgs>.Is.Anything))
              .WhenCalled (
                  mi =>
                  {
                    Assert.That (ClientTransaction.Current, Is.SameAs (inactiveClientTransaction));
                    Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (inactiveClientTransaction));
                  });

          var eventBroker = new ClientTransactionEventBroker (inactiveClientTransaction);
          eventBroker.RaiseSubTransactionCreatedEvent (ClientTransactionObjectMother.Create());

          transactionEventReceiverMock.VerifyAllExpectations();
        }
      }
    }

    private void CheckEventWithListenersLast (
        Action<IClientTransactionEventSink> raiseAction,
        Action<IClientTransactionListener> listenerEvent,
        Action<IClientTransactionExtension> extensionEvent,
        Action orderedPreListenerExpectations = null)
    {
      using (_mockRepository.Ordered ())
      {
        if (orderedPreListenerExpectations != null)
          orderedPreListenerExpectations ();
        _extensionMock.Expect (extensionEvent);
        _listenerMock.Expect (listenerEvent);
      }
      _mockRepository.ReplayAll ();

      raiseAction (_eventBroker);

      _mockRepository.VerifyAll ();
    }

    private void CheckEventWithListenersFirst (
        Action<IClientTransactionEventSink> raiseAction,
        Action<IClientTransactionListener> listenerEvent,
        Action<IClientTransactionExtension> extensionEvent,
        Action orderedPostListenerExpectations = null)
    {
      using (_mockRepository.Ordered ())
      {
        _listenerMock.Expect (listenerEvent);
        _extensionMock.Expect (extensionEvent);
        if (orderedPostListenerExpectations != null)
          orderedPostListenerExpectations ();
      }
      _mockRepository.ReplayAll ();

      raiseAction (_eventBroker);

      _mockRepository.VerifyAll ();
    }

    private void CheckEventWithListenersOnly (
        Action<IClientTransactionEventSink> raiseAction,
        Action<IClientTransactionListener> listenerEvent)
    {
      _listenerMock.Expect (listenerEvent);
      _mockRepository.ReplayAll ();

      raiseAction (_eventBroker);

      _mockRepository.VerifyAll ();
    }
  }
}