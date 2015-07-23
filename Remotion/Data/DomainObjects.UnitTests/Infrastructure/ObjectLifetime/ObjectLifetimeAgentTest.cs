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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectLifetimeAgentTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private IClientTransactionEventSink _eventSinkWithMock;
    private IInvalidDomainObjectManager _invalidDomainObjectManagerMock;
    private IDataManager _dataManagerMock;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerMock;
    private IPersistenceStrategy _persistenceStrategyMock;

    private ObjectLifetimeAgent _agent;

    private ObjectID _objectID1;
    private DomainObject _domainObject1;
    private DataContainer _dataContainer1;

    private ObjectID _objectID2;
    private DomainObject _domainObject2;
    private DataContainer _dataContainer2;
    private IDomainObjectCreator _domainObjectCreatorMock;
    private ClassDefinition _typeDefinitionWithCreatorMock;
    private ObjectID _objectIDWithCreatorMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransactionObjectMother.Create();
      _eventSinkWithMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();
      _invalidDomainObjectManagerMock = MockRepository.GenerateStrictMock<IInvalidDomainObjectManager> ();
      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager> ();
      _enlistedDomainObjectManagerMock = MockRepository.GenerateStrictMock<IEnlistedDomainObjectManager> ();
      _persistenceStrategyMock = MockRepository.GenerateStrictMock<IPersistenceStrategy>();

      _agent = new ObjectLifetimeAgent (
          _transaction, 
          _eventSinkWithMock, 
          _invalidDomainObjectManagerMock, 
          _dataManagerMock, 
          _enlistedDomainObjectManagerMock,
          _persistenceStrategyMock);

      _objectID1 = DomainObjectIDs.Order1;
      _domainObject1 = DomainObjectMother.CreateFakeObject (_objectID1);
      _dataContainer1 = DataContainerObjectMother.CreateExisting (_domainObject1);

      _objectID2 = DomainObjectIDs.Order3;
      _domainObject2 = DomainObjectMother.CreateFakeObject (_objectID2);
      _dataContainer2 = DataContainerObjectMother.CreateExisting (_domainObject2);

      _domainObjectCreatorMock = MockRepository.GenerateStrictMock<IDomainObjectCreator>();
      _typeDefinitionWithCreatorMock = ClassDefinitionObjectMother.CreateClassDefinitionWithTable (
          TestDomainStorageProviderDefinition,
          classType: typeof (OrderItem),
          instanceCreator: _domainObjectCreatorMock);

      _objectIDWithCreatorMock = new ObjectID (_typeDefinitionWithCreatorMock, Guid.NewGuid());
    }

    [Test]
    public void NewObject ()
    {
      var constructorParameters = ParamList.Create ("Some Product");

      _eventSinkWithMock.Expect (mock => mock.RaiseNewObjectCreatingEvent ( _typeDefinitionWithCreatorMock.ClassType));
      _persistenceStrategyMock.Expect (mock => mock.CreateNewObjectID (_typeDefinitionWithCreatorMock)).Return (_objectID1);

      _domainObjectCreatorMock
          .Expect (
              mock => mock.CreateNewObject (Arg<IObjectInitializationContext>.Is.Anything, Arg.Is (constructorParameters), Arg.Is (_transaction)))
          .WhenCalled (mi => CheckInitializationContext<NewObjectInitializationContext> (mi.Arguments[0], _objectID1, _transaction))
          .Return (_domainObject1);

      var result = _agent.NewObject (_typeDefinitionWithCreatorMock, constructorParameters);

      _eventSinkWithMock.VerifyAllExpectations();
      _persistenceStrategyMock.VerifyAllExpectations();
      _domainObjectCreatorMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void NewObject_WithSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      var agent = new ObjectLifetimeAgent (
          subTransaction,
          _eventSinkWithMock,
          _invalidDomainObjectManagerMock,
          _dataManagerMock,
          _enlistedDomainObjectManagerMock,
          _persistenceStrategyMock);

      _eventSinkWithMock.Stub (mock => mock.RaiseNewObjectCreatingEvent (Arg<Type>.Is.Anything));
      _persistenceStrategyMock.Stub (mock => mock.CreateNewObjectID (Arg<ClassDefinition>.Is.Anything)).Return (_objectID1);
      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (Arg<IObjectInitializationContext>.Is.Anything, Arg.Is (ParamList.Empty), Arg.Is (subTransaction)))
          .WhenCalled (mi => CheckInitializationContext<NewObjectInitializationContext> (mi.Arguments[0], _objectID1, _transaction))
          .Return (_domainObject1);

      agent.NewObject (_typeDefinitionWithCreatorMock, ParamList.Empty);

      _domainObjectCreatorMock.VerifyAllExpectations ();
    }

    [Test]
    public void NewObject_AbstractClass ()
    {
      var typeDefinition = GetTypeDefinition (typeof (AbstractClass));
      Assert.That (
          () => _agent.NewObject (typeDefinition, ParamList.Empty), 
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot instantiate type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.AbstractClass' because it is abstract. "
              + "For classes with automatic properties, InstantiableAttribute must be used."));
    }

    [Test]
    public void NewObject_CreationException_CausesObjectToBeDeleted ()
    {
      var constructorParameters = ParamList.Empty;

      _eventSinkWithMock.Stub (stub => stub.RaiseNewObjectCreatingEvent ( _typeDefinitionWithCreatorMock.ClassType));
      _persistenceStrategyMock.Stub (stub => stub.CreateNewObjectID (_typeDefinitionWithCreatorMock)).Return (_objectID1);

      var exception = new Exception ("Test");
      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (Arg<IObjectInitializationContext>.Is.Anything, Arg.Is (constructorParameters), Arg.Is (_transaction)))
          .WhenCalled (mi => 
          {
            // Pretend an object was registered, then throw an exception - that way, the registered object needs to be cleaned up
            FakeRegisteredObject ((NewObjectInitializationContext) mi.Arguments[0], _domainObject1);
            throw exception;
          })
          .Return (null);

      var deleteCommandMock = SetupDeleteExpectations (_dataManagerMock, _domainObject1);
      _enlistedDomainObjectManagerMock.Expect (mock => mock.DisenlistDomainObject (_domainObject1));

      Assert.That (() => _agent.NewObject (_typeDefinitionWithCreatorMock, constructorParameters), Throws.Exception.SameAs (exception));

      _domainObjectCreatorMock.VerifyAllExpectations ();
      _dataManagerMock.VerifyAllExpectations();
      deleteCommandMock.VerifyAllExpectations();
      _enlistedDomainObjectManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void NewObject_CreationException_WithExceptionInDelete ()
    {
      var constructorParameters = ParamList.Empty;

      _eventSinkWithMock.Stub (stub => stub.RaiseNewObjectCreatingEvent ( _typeDefinitionWithCreatorMock.ClassType));
      _persistenceStrategyMock.Stub (stub => stub.CreateNewObjectID (_typeDefinitionWithCreatorMock)).Return (_objectID1);

      var exceptionInCreate = new Exception ("Test");
      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (Arg<IObjectInitializationContext>.Is.Anything, Arg.Is (constructorParameters), Arg.Is (_transaction)))
          .WhenCalled (mi =>
          {
            // Pretend an object was registered, then throw an exception - that way, the registered object needs to be cleaned up
            FakeRegisteredObject ((NewObjectInitializationContext) mi.Arguments[0], _domainObject1);
            throw exceptionInCreate;
          })
          .Return (null);
      
      var exceptionInDelete = new InvalidOperationException ("Cancelled");
      var deleteCommandMock = SetupDeleteExpectationsWithException (_dataManagerMock, _domainObject1, exceptionInDelete);

      Assert.That (
          () => _agent.NewObject (_typeDefinitionWithCreatorMock, constructorParameters), 
          Throws.TypeOf<ObjectCleanupException>()
              .With.Message.EqualTo (
                  "While cleaning up an object of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' that threw an exception of type "
                  + "'System.Exception' from its constructor, another exception of type 'System.InvalidOperationException' was encountered. "
                  + "Cleanup was therefore aborted, and a partially constructed object with ID '" + _objectID1 + "' remains "
                  + "within the ClientTransaction '" + _transaction
                  + "'. Rollback the transaction to get rid of the partially constructed instance." + Environment.NewLine
                  + "Message of original exception: Test" + Environment.NewLine
                  + "Message of exception occurring during cleanup: Cancelled")
              .And.InnerException.SameAs (exceptionInCreate)
              .And.Property ("CleanupException").SameAs (exceptionInDelete));

      _domainObjectCreatorMock.VerifyAllExpectations ();
      _dataManagerMock.VerifyAllExpectations ();
      deleteCommandMock.VerifyAllExpectations ();
    }

    [Test]
    public void NewObject_CreationException_WithoutRegisteredObject ()
    {
      var constructorParameters = ParamList.Empty;

      _eventSinkWithMock.Stub (stub => stub.RaiseNewObjectCreatingEvent ( _typeDefinitionWithCreatorMock.ClassType));
      _persistenceStrategyMock.Stub (stub => stub.CreateNewObjectID (_typeDefinitionWithCreatorMock)).Return (_objectID1);

      var exception = new Exception ("Test");
      // No object is registered before the exception is thrown
      _domainObjectCreatorMock
          .Expect (mock => mock.CreateNewObject (Arg<IObjectInitializationContext>.Is.Anything, Arg.Is (constructorParameters), Arg.Is (_transaction)))
          .Throw (exception);

      Assert.That (() => _agent.NewObject (_typeDefinitionWithCreatorMock, constructorParameters), Throws.Exception.SameAs (exception));

      _domainObjectCreatorMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void GetObjectReference_KnownObject_Invalid_Works ()
    {
      _invalidDomainObjectManagerMock.Expect (mock => mock.IsInvalid (_objectIDWithCreatorMock)).Return (true);
      _invalidDomainObjectManagerMock.Expect (mock => mock.GetInvalidObjectReference (_objectIDWithCreatorMock)).Return (_domainObject1);

      var result = _agent.GetObjectReference (_objectIDWithCreatorMock);

      _invalidDomainObjectManagerMock.VerifyAllExpectations();
      _domainObjectCreatorMock.AssertWasNotCalled (
          mock => mock.CreateObjectReference (Arg<IObjectInitializationContext>.Is.Anything, Arg<ClientTransaction>.Is.Anything));

      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObjectReference_KnownObject_ReturnedWithoutLoading ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectIDWithCreatorMock)).Return (false);
      _enlistedDomainObjectManagerMock.Expect (mock => mock.GetEnlistedDomainObject (_objectIDWithCreatorMock)).Return (_domainObject1);

      var result = _agent.GetObjectReference (_objectIDWithCreatorMock);

      _enlistedDomainObjectManagerMock.VerifyAllExpectations ();
      _domainObjectCreatorMock.AssertWasNotCalled (
          mock => mock.CreateObjectReference (Arg<IObjectInitializationContext>.Is.Anything, Arg<ClientTransaction>.Is.Anything));

      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObjectReference_UnknownObject_ReturnsUnloadedObject ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectIDWithCreatorMock)).Return (false);
      _enlistedDomainObjectManagerMock.Stub (stub => stub.GetEnlistedDomainObject (_objectIDWithCreatorMock)).Return (null);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateObjectReference (Arg<IObjectInitializationContext>.Is.Anything, Arg.Is (_transaction)))
          .Return (_domainObject1)
          .WhenCalled (
              mi => CheckInitializationContext<ObjectReferenceInitializationContext> (mi.Arguments[0], _objectIDWithCreatorMock, _transaction));

      var result = _agent.GetObjectReference (_objectIDWithCreatorMock);

      _domainObjectCreatorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObjectReference_UnknownObject_WithSubTransaction_PutsRootTransactionIntoContext ()
    {
      var subTransaction = _transaction.CreateSubTransaction ();
      var agent = new ObjectLifetimeAgent (
          subTransaction,
          _eventSinkWithMock,
          _invalidDomainObjectManagerMock,
          _dataManagerMock,
          _enlistedDomainObjectManagerMock,
          _persistenceStrategyMock);

      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectIDWithCreatorMock)).Return (false);
      _enlistedDomainObjectManagerMock.Stub (stub => stub.GetEnlistedDomainObject (_objectIDWithCreatorMock)).Return (null);

      _domainObjectCreatorMock
          .Expect (mock => mock.CreateObjectReference (Arg<ObjectReferenceInitializationContext>.Is.TypeOf, Arg.Is (subTransaction)))
          .Return (_domainObject1)
          .WhenCalled (
              mi => CheckInitializationContext<ObjectReferenceInitializationContext> (mi.Arguments[0], _objectIDWithCreatorMock, _transaction));

      var result = agent.GetObjectReference (_objectIDWithCreatorMock);

      _domainObjectCreatorMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void GetObject ()
    {
      Assert.That (_dataContainer1.State, Is.Not.EqualTo (StateType.Deleted));

      _dataManagerMock.Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, true)).Return (_dataContainer1);

      var result = _agent.GetObject (_objectID1, BooleanObjectMother.GetRandomBoolean());

      _dataManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_dataContainer1.DomainObject));
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedTrue ()
    {
      var dataContainer = DataContainerObjectMother.CreateDeleted (_domainObject1);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithLazyLoad (_objectID1, true)).Return (dataContainer);

      var result = _agent.GetObject (_objectID1, true);

      Assert.That (result, Is.SameAs (dataContainer.DomainObject));
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedFalse ()
    {
      var dataContainer = DataContainerObjectMother.CreateDeleted (_domainObject1);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _dataManagerMock.Stub (stub => stub.GetDataContainerWithLazyLoad (_objectID1, true)).Return (dataContainer);

      Assert.That (
          () => _agent.GetObject (_objectID1, false),
          Throws.TypeOf<ObjectDeletedException>().With.Property<ObjectDeletedException> (e => e.ID).EqualTo (_objectID1));
    }

    [Test]
    public void TryGetObject_InvalidObject ()
    {
      _invalidDomainObjectManagerMock.Expect (stub => stub.IsInvalid (_objectID1)).Return (true);
      _invalidDomainObjectManagerMock.Expect (stub => stub.GetInvalidObjectReference (_objectID1)).Return (_domainObject1);

      var result = _agent.TryGetObject (_objectID1);
      
      _dataManagerMock.AssertWasNotCalled (mock => mock.GetDataContainerWithLazyLoad (Arg<ObjectID>.Is.Anything, Arg<bool>.Is.Anything));
      Assert.That (result, Is.SameAs (_domainObject1));
    }

    [Test]
    public void TryGetObject_LoadsViaDataManager_Found ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _dataManagerMock.Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false)).Return (_dataContainer1);

      var result = _agent.TryGetObject (_objectID1);

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_dataContainer1.DomainObject));
    }

    [Test]
    public void TryGetObject_LoadsViaDataManager_NotFound ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _dataManagerMock.Expect (mock => mock.GetDataContainerWithLazyLoad (_objectID1, false)).Return (null);

      var result = _agent.TryGetObject (_objectID1);

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetObjects ()
    {
      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, true))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      var result = _agent.GetObjects<Order> (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<Order[]>().And.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetObjects_InvalidType ()
    {
      _dataManagerMock
          .Stub (stub => stub.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, true))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      Assert.That (
          () => _agent.GetObjects<ClassWithAllDataTypes> (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }), 
          Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void TryGetObjects ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      var result = _agent.TryGetObjects<Order> (new[] { _objectID1, _objectID2 });

      _dataManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<Order[]>().And.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_WithNotFoundObjects ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);

      _dataManagerMock
          .Stub (stub => stub.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { null, _dataContainer2 });

      var result = _agent.TryGetObjects<Order> (new[] { _objectID1, _objectID2 });

      Assert.That (result, Is.EqualTo (new[] { null, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_WithInvalidObjects ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (true);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.GetInvalidObjectReference (_objectID1)).Return (_domainObject1);

      _dataManagerMock
            .Stub (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID2 }, false))
            .Return (new[] { _dataContainer2 });

      var result = _agent.TryGetObjects<Order> (new[] { _objectID1, _objectID2 });
      Assert.That (result, Is.EqualTo (new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_InvalidType ()
    {
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID1)).Return (false);
      _invalidDomainObjectManagerMock.Stub (stub => stub.IsInvalid (_objectID2)).Return (false);

      _dataManagerMock
          .Expect (mock => mock.GetDataContainersWithLazyLoad (new[] { _objectID1, _objectID2 }, false))
          .Return (new[] { _dataContainer1, _dataContainer2 });

      Assert.That (
          () => _agent.TryGetObjects<ClassWithAllDataTypes> (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }),
          Throws.TypeOf<InvalidCastException> ());
    }

    [Test]
    public void Delete ()
    {
      var deleteCommandMock = SetupDeleteExpectations(_dataManagerMock, _domainObject1);

      _agent.Delete (_domainObject1);

      _dataManagerMock.VerifyAllExpectations();
      deleteCommandMock.VerifyAllExpectations();
    }

    [Test]
    public void Serialization ()
    {
      var instance = new ObjectLifetimeAgent (
          _transaction,
          new SerializableClientTransactionEventSinkFake(),
          new SerializableInvalidDomainObjectManagerFake(),
          new SerializableDataManagerFake(),
          new SerializableEnlistedDomainObjectManagerFake(),
          new SerializablePersistenceStrategyFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
      Assert.That (deserializedInstance.InvalidDomainObjectManager, Is.Not.Null);
      Assert.That (deserializedInstance.DataManager, Is.Not.Null);
      Assert.That (deserializedInstance.EnlistedDomainObjectManager, Is.Not.Null);
      Assert.That (deserializedInstance.PersistenceStrategy, Is.Not.Null);
    }

    private IDataManagementCommand SetupDeleteExpectations (IDataManager dataManagerMock, DomainObject deletedObject)
    {
      var initialCommandStub = MockRepository.GenerateStrictMock<IDataManagementCommand> ();
      var actualCommandMock = MockRepository.GenerateStrictMock<IDataManagementCommand> ();

      var counter = new OrderedExpectationCounter ();
      actualCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (Enumerable.Empty<Exception> ());
      initialCommandStub.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (actualCommandMock)).Ordered (counter);
      actualCommandMock.Expect (mock => mock.Begin ()).Ordered (counter);
      actualCommandMock.Expect (mock => mock.Perform ()).Ordered (counter);
      actualCommandMock.Expect (mock => mock.End ()).Ordered (counter);
      dataManagerMock.Expect (mock => mock.CreateDeleteCommand (deletedObject)).Return (initialCommandStub);
      return actualCommandMock;
    }

    private IDataManagementCommand SetupDeleteExpectationsWithException (IDataManager dataManagerMock, DomainObject deletedObject, Exception exception)
    {
      var initialCommandStub = MockRepository.GenerateStrictMock<IDataManagementCommand> ();
      var actualCommandMock = MockRepository.GenerateStrictMock<IDataManagementCommand> ();

      var counter = new OrderedExpectationCounter ();
      actualCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (Enumerable.Empty<Exception> ());
      initialCommandStub.Stub (stub => stub.ExpandToAllRelatedObjects ()).Return (new ExpandedCommand (actualCommandMock)).Ordered (counter);
      actualCommandMock.Expect (mock => mock.Begin ()).Throw (exception);
      dataManagerMock.Expect (mock => mock.CreateDeleteCommand (deletedObject)).Return (initialCommandStub);
      return actualCommandMock;
    }

    private static void CheckInitializationContext<TExpectedContextType> (
        object initializationContext, ObjectID expectedObjectID, ClientTransaction expectedRootTransaction)
      where TExpectedContextType : IObjectInitializationContext
    {
      Assert.That (
          initializationContext,
          Is.Not.Null
            .And.TypeOf<TExpectedContextType>()
            .And.Property<IObjectInitializationContext> (c => c.ObjectID).EqualTo (expectedObjectID)
            .And.Property<IObjectInitializationContext> (c => c.RootTransaction).SameAs (expectedRootTransaction));
    }

    private void FakeRegisteredObject (NewObjectInitializationContext objectInitializationContext, DomainObject registeredObject)
    {
      PrivateInvoke.SetNonPublicField (objectInitializationContext, "_registeredObject", registeredObject);
    }
  }
}