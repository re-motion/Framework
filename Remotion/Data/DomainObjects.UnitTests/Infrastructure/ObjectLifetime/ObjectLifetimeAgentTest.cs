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
using Moq;
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
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectLifetimeAgentTest : StandardMappingTest
  {
    private ClientTransaction _transaction;
    private Mock<IClientTransactionEventSink> _eventSinkWithMock;
    private Mock<IInvalidDomainObjectManager> _invalidDomainObjectManagerMock;
    private Mock<IDataManager> _dataManagerMock;
    private Mock<IEnlistedDomainObjectManager> _enlistedDomainObjectManagerMock;
    private Mock<IPersistenceStrategy> _persistenceStrategyMock;

    private ObjectLifetimeAgent _agent;

    private ObjectID _objectID1;
    private DomainObject _domainObject1;
    private DataContainer _dataContainer1;

    private ObjectID _objectID2;
    private DomainObject _domainObject2;
    private DataContainer _dataContainer2;
    private Mock<IDomainObjectCreator> _domainObjectCreatorMock;
    private ClassDefinition _classDefinitionWithCreatorMock;
    private ObjectID _objectIDWithCreatorMock;

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransactionObjectMother.Create();
      _eventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);
      _invalidDomainObjectManagerMock = new Mock<IInvalidDomainObjectManager>(MockBehavior.Strict);
      _dataManagerMock = new Mock<IDataManager>(MockBehavior.Strict);
      _enlistedDomainObjectManagerMock = new Mock<IEnlistedDomainObjectManager>(MockBehavior.Strict);
      _persistenceStrategyMock = new Mock<IPersistenceStrategy>(MockBehavior.Strict);

      _agent = new ObjectLifetimeAgent(
          _transaction,
          _eventSinkWithMock.Object,
          _invalidDomainObjectManagerMock.Object,
          _dataManagerMock.Object,
          _enlistedDomainObjectManagerMock.Object,
          _persistenceStrategyMock.Object);

      _objectID1 = DomainObjectIDs.Order1;
      _domainObject1 = DomainObjectMother.CreateFakeObject(_objectID1);
      _dataContainer1 = DataContainerObjectMother.CreateExisting(_domainObject1);

      _objectID2 = DomainObjectIDs.Order3;
      _domainObject2 = DomainObjectMother.CreateFakeObject(_objectID2);
      _dataContainer2 = DataContainerObjectMother.CreateExisting(_domainObject2);

      _domainObjectCreatorMock = new Mock<IDomainObjectCreator>(MockBehavior.Strict);
      _classDefinitionWithCreatorMock = ClassDefinitionObjectMother.CreateClassDefinitionWithTable(
          TestDomainStorageProviderDefinition,
          classType: typeof(OrderItem),
          instanceCreator: _domainObjectCreatorMock.Object);

      _objectIDWithCreatorMock = new ObjectID(_classDefinitionWithCreatorMock, Guid.NewGuid());
    }

    [Test]
    public void NewObject ()
    {
      var constructorParameters = ParamList.Create("Some Product");

      _eventSinkWithMock.Setup(mock => mock.RaiseNewObjectCreatingEvent(_classDefinitionWithCreatorMock.Type)).Verifiable();
      _persistenceStrategyMock.Setup(mock => mock.CreateNewObjectID(_classDefinitionWithCreatorMock)).Returns(_objectID1).Verifiable();

      _domainObjectCreatorMock
          .Setup(
              mock => mock.CreateNewObject(It.IsAny<IObjectInitializationContext>(), constructorParameters, _transaction))
          .Callback((IObjectInitializationContext objectInitializationContext, ParamList _, ClientTransaction _) =>
              CheckInitializationContext<NewObjectInitializationContext>(objectInitializationContext, _objectID1, _transaction))
          .Returns(_domainObject1)
          .Verifiable();

      var result = _agent.NewObject(_classDefinitionWithCreatorMock, constructorParameters);

      _eventSinkWithMock.Verify();
      _persistenceStrategyMock.Verify();
      _domainObjectCreatorMock.Verify();

      Assert.That(result, Is.SameAs(_domainObject1));
    }

    [Test]
    public void NewObject_WithSubTransaction ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var agent = new ObjectLifetimeAgent(
          subTransaction,
          _eventSinkWithMock.Object,
          _invalidDomainObjectManagerMock.Object,
          _dataManagerMock.Object,
          _enlistedDomainObjectManagerMock.Object,
          _persistenceStrategyMock.Object);

      _eventSinkWithMock.Setup(mock => mock.RaiseNewObjectCreatingEvent(It.IsAny<Type>()));
      _persistenceStrategyMock.Setup(mock => mock.CreateNewObjectID(It.IsAny<ClassDefinition>())).Returns(_objectID1);
      _domainObjectCreatorMock
          .Setup(mock => mock.CreateNewObject(It.IsAny<IObjectInitializationContext>(), ParamList.Empty, subTransaction))
          .Callback((IObjectInitializationContext objectInitializationContext, ParamList _, ClientTransaction _) =>
              CheckInitializationContext<NewObjectInitializationContext>(objectInitializationContext, _objectID1, _transaction))
          .Returns(_domainObject1)
          .Verifiable();

      agent.NewObject(_classDefinitionWithCreatorMock, ParamList.Empty);

      _domainObjectCreatorMock.Verify();
    }

    [Test]
    public void NewObject_AbstractClass ()
    {
      var classDefinition = GetClassDefinition(typeof(AbstractClass));
      Assert.That(
          () => _agent.NewObject(classDefinition, ParamList.Empty),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot instantiate type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.AbstractClass' because it is abstract. "
              + "For classes with automatic properties, InstantiableAttribute must be used."));
    }

    [Test]
    public void NewObject_CreationException_CausesObjectToBeDeleted ()
    {
      var sequence = new VerifiableSequence();

      var constructorParameters = ParamList.Empty;

      _eventSinkWithMock.Setup(stub => stub.RaiseNewObjectCreatingEvent(_classDefinitionWithCreatorMock.Type));
      _persistenceStrategyMock.Setup(stub => stub.CreateNewObjectID(_classDefinitionWithCreatorMock)).Returns(_objectID1);

      var exception = new Exception("Test");
      _domainObjectCreatorMock
          .Setup(mock => mock.CreateNewObject(It.IsAny<IObjectInitializationContext>(), constructorParameters, _transaction))
          .Callback(
              (IObjectInitializationContext objectInitializationContext, ParamList _, ClientTransaction _) =>
              {
                // Pretend an object was registered, then throw an exception - that way, the registered object needs to be cleaned up
                FakeRegisteredObject((NewObjectInitializationContext)objectInitializationContext, _domainObject1);
                throw exception;
              })
          .Returns((DomainObject)null)
          .Verifiable();

      var deleteCommandMock = SetupDeleteExpectations(sequence, _dataManagerMock, _domainObject1);
      _enlistedDomainObjectManagerMock.Setup(mock => mock.DisenlistDomainObject(_domainObject1)).Verifiable();

      Assert.That(() => _agent.NewObject(_classDefinitionWithCreatorMock, constructorParameters), Throws.Exception.SameAs(exception));

      _domainObjectCreatorMock.Verify();
      _dataManagerMock.Verify();
      deleteCommandMock.Verify();
      _enlistedDomainObjectManagerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void NewObject_CreationException_WithExceptionInDelete ()
    {
      var sequence = new VerifiableSequence();

      var constructorParameters = ParamList.Empty;

      _eventSinkWithMock.Setup(stub => stub.RaiseNewObjectCreatingEvent(_classDefinitionWithCreatorMock.Type));
      _persistenceStrategyMock.Setup(stub => stub.CreateNewObjectID(_classDefinitionWithCreatorMock)).Returns(_objectID1);

      var exceptionInCreate = new Exception("Test");
      _domainObjectCreatorMock
          .Setup(mock => mock.CreateNewObject(It.IsNotNull<IObjectInitializationContext>(), constructorParameters, _transaction))
          .Callback((IObjectInitializationContext objectInitializationContext, ParamList _, ClientTransaction _) =>
{
            // Pretend an object was registered, then throw an exception - that way, the registered object needs to be cleaned up
            FakeRegisteredObject((NewObjectInitializationContext)objectInitializationContext, _domainObject1);
            throw exceptionInCreate;
          })
          .Returns((DomainObject)null)
          .Verifiable();

      var exceptionInDelete = new InvalidOperationException("Cancelled");
      var deleteCommandMock = SetupDeleteExpectationsWithException(sequence, _dataManagerMock, _domainObject1, exceptionInDelete);

      Assert.That(
          () => _agent.NewObject(_classDefinitionWithCreatorMock, constructorParameters),
          Throws.TypeOf<ObjectCleanupException>()
              .With.Message.EqualTo(
                  "While cleaning up an object of type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' that threw an exception of type "
                  + "'System.Exception' from its constructor, another exception of type 'System.InvalidOperationException' was encountered. "
                  + "Cleanup was therefore aborted, and a partially constructed object with ID '" + _objectID1 + "' remains "
                  + "within the ClientTransaction '" + _transaction
                  + "'. Rollback the transaction to get rid of the partially constructed instance." + Environment.NewLine
                  + "Message of original exception: Test" + Environment.NewLine
                  + "Message of exception occurring during cleanup: Cancelled")
              .And.InnerException.SameAs(exceptionInCreate)
              .And.Property("CleanupException").SameAs(exceptionInDelete));

      _domainObjectCreatorMock.Verify();
      _dataManagerMock.Verify();
      deleteCommandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void NewObject_CreationException_WithoutRegisteredObject ()
    {
      var constructorParameters = ParamList.Empty;

      _eventSinkWithMock.Setup(stub => stub.RaiseNewObjectCreatingEvent(_classDefinitionWithCreatorMock.Type));
      _persistenceStrategyMock.Setup(stub => stub.CreateNewObjectID(_classDefinitionWithCreatorMock)).Returns(_objectID1);

      var exception = new Exception("Test");
      // No object is registered before the exception is thrown
      _domainObjectCreatorMock
          .Setup(mock => mock.CreateNewObject(It.IsAny<IObjectInitializationContext>(), constructorParameters, _transaction))
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _agent.NewObject(_classDefinitionWithCreatorMock, constructorParameters), Throws.Exception.SameAs(exception));

      _domainObjectCreatorMock.Verify();
    }

    [Test]
    public void GetObjectReference_KnownObject_Invalid_Works ()
    {
      _invalidDomainObjectManagerMock.Setup(mock => mock.IsInvalid(_objectIDWithCreatorMock)).Returns(true).Verifiable();
      _invalidDomainObjectManagerMock.Setup(mock => mock.GetInvalidObjectReference(_objectIDWithCreatorMock)).Returns(_domainObject1).Verifiable();

      var result = _agent.GetObjectReference(_objectIDWithCreatorMock);

      _invalidDomainObjectManagerMock.Verify();
      _domainObjectCreatorMock.Verify(mock => mock.CreateObjectReference(It.IsAny<IObjectInitializationContext>(), It.IsAny<ClientTransaction>()), Times.Never());

      Assert.That(result, Is.SameAs(_domainObject1));
    }

    [Test]
    public void GetObjectReference_KnownObject_ReturnedWithoutLoading ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectIDWithCreatorMock)).Returns(false);
      _enlistedDomainObjectManagerMock.Setup(mock => mock.GetEnlistedDomainObject(_objectIDWithCreatorMock)).Returns(_domainObject1).Verifiable();

      var result = _agent.GetObjectReference(_objectIDWithCreatorMock);

      _enlistedDomainObjectManagerMock.Verify();
      _domainObjectCreatorMock.Verify(mock => mock.CreateObjectReference(It.IsAny<IObjectInitializationContext>(), It.IsAny<ClientTransaction>()), Times.Never());

      Assert.That(result, Is.SameAs(_domainObject1));
    }

    [Test]
    public void GetObjectReference_UnknownObject_ReturnsUnloadedObject ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectIDWithCreatorMock)).Returns(false);
      _enlistedDomainObjectManagerMock.Setup(stub => stub.GetEnlistedDomainObject(_objectIDWithCreatorMock)).Returns((DomainObject)null);

      _domainObjectCreatorMock
          .Setup(mock => mock.CreateObjectReference(It.IsAny<IObjectInitializationContext>(), _transaction))
          .Returns(_domainObject1)
          .Callback(
              (IObjectInitializationContext objectInitializationContext, ClientTransaction _) =>
                  CheckInitializationContext<ObjectReferenceInitializationContext>(objectInitializationContext, _objectIDWithCreatorMock, _transaction))
          .Verifiable();

      var result = _agent.GetObjectReference(_objectIDWithCreatorMock);

      _domainObjectCreatorMock.Verify();
      Assert.That(result, Is.SameAs(_domainObject1));
    }

    [Test]
    public void GetObjectReference_UnknownObject_WithSubTransaction_PutsRootTransactionIntoContext ()
    {
      var subTransaction = _transaction.CreateSubTransaction();
      var agent = new ObjectLifetimeAgent(
          subTransaction,
          _eventSinkWithMock.Object,
          _invalidDomainObjectManagerMock.Object,
          _dataManagerMock.Object,
          _enlistedDomainObjectManagerMock.Object,
          _persistenceStrategyMock.Object);

      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectIDWithCreatorMock)).Returns(false);
      _enlistedDomainObjectManagerMock.Setup(stub => stub.GetEnlistedDomainObject(_objectIDWithCreatorMock)).Returns((DomainObject)null);

      _domainObjectCreatorMock
          .Setup(mock => mock.CreateObjectReference(It.IsAny<ObjectReferenceInitializationContext>(), subTransaction))
          .Returns(_domainObject1)
          .Callback(
              (IObjectInitializationContext objectInitializationContext, ClientTransaction _) =>
                  CheckInitializationContext<ObjectReferenceInitializationContext>(objectInitializationContext, _objectIDWithCreatorMock, _transaction))
          .Verifiable();

      var result = agent.GetObjectReference(_objectIDWithCreatorMock);

      _domainObjectCreatorMock.Verify();
      Assert.That(result, Is.SameAs(_domainObject1));
    }

    [Test]
    public void GetObject ()
    {
      Assert.That(_dataContainer1.State.IsDeleted, Is.False);

      _dataManagerMock.Setup(mock => mock.GetDataContainerWithLazyLoad(_objectID1, true)).Returns(_dataContainer1).Verifiable();

      var result = _agent.GetObject(_objectID1, BooleanObjectMother.GetRandomBoolean());

      _dataManagerMock.Verify();
      Assert.That(result, Is.SameAs(_dataContainer1.DomainObject));
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedTrue ()
    {
      var dataContainer = DataContainerObjectMother.CreateDeleted(_domainObject1);
      Assert.That(dataContainer.State.IsDeleted, Is.True);

      _dataManagerMock.Setup(stub => stub.GetDataContainerWithLazyLoad(_objectID1, true)).Returns(dataContainer);

      var result = _agent.GetObject(_objectID1, true);

      Assert.That(result, Is.SameAs(dataContainer.DomainObject));
    }

    [Test]
    public void GetObject_DeletedObject_IncludeDeletedFalse ()
    {
      var dataContainer = DataContainerObjectMother.CreateDeleted(_domainObject1);
      Assert.That(dataContainer.State.IsDeleted, Is.True);

      _dataManagerMock.Setup(stub => stub.GetDataContainerWithLazyLoad(_objectID1, true)).Returns(dataContainer);

      Assert.That(
          () => _agent.GetObject(_objectID1, false),
          Throws.TypeOf<ObjectDeletedException>().With.Property<ObjectDeletedException>(e => e.ID).EqualTo(_objectID1));
    }

    [Test]
    public void TryGetObject_InvalidObject ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(true).Verifiable();
      _invalidDomainObjectManagerMock.Setup(stub => stub.GetInvalidObjectReference(_objectID1)).Returns(_domainObject1).Verifiable();

      var result = _agent.TryGetObject(_objectID1);

      _dataManagerMock.Verify(mock => mock.GetDataContainerWithLazyLoad(It.IsAny<ObjectID>(), It.IsAny<bool>()), Times.Never());
      Assert.That(result, Is.SameAs(_domainObject1));
    }

    [Test]
    public void TryGetObject_LoadsViaDataManager_Found ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(false);
      _dataManagerMock.Setup(mock => mock.GetDataContainerWithLazyLoad(_objectID1, false)).Returns(_dataContainer1).Verifiable();

      var result = _agent.TryGetObject(_objectID1);

      _dataManagerMock.Verify();
      Assert.That(result, Is.SameAs(_dataContainer1.DomainObject));
    }

    [Test]
    public void TryGetObject_LoadsViaDataManager_NotFound ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(false);
      _dataManagerMock.Setup(mock => mock.GetDataContainerWithLazyLoad(_objectID1, false)).Returns((DataContainer)null).Verifiable();

      var result = _agent.TryGetObject(_objectID1);

      _dataManagerMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetObjects ()
    {
      _dataManagerMock
          .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, true))
          .Returns(new[] { _dataContainer1, _dataContainer2 })
          .Verifiable();

      var result = _agent.GetObjects<Order>(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      _dataManagerMock.Verify();
      Assert.That(result, Is.TypeOf<Order[]>().And.EqualTo(new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void GetObjects_InvalidType ()
    {
      _dataManagerMock
          .Setup(stub => stub.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, true))
          .Returns(new[] { _dataContainer1, _dataContainer2 });

      Assert.That(
          () => _agent.GetObjects<ClassWithAllDataTypes>(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }),
          Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void TryGetObjects ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(false);
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID2)).Returns(false);

      _dataManagerMock
          .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, false))
          .Returns(new[] { _dataContainer1, _dataContainer2 })
          .Verifiable();

      var result = _agent.TryGetObjects<Order>(new[] { _objectID1, _objectID2 });

      _dataManagerMock.Verify();
      Assert.That(result, Is.TypeOf<Order[]>().And.EqualTo(new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_WithNotFoundObjects ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(false);
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID2)).Returns(false);

      _dataManagerMock
          .Setup(stub => stub.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, false))
          .Returns(new[] { null, _dataContainer2 });

      var result = _agent.TryGetObjects<Order>(new[] { _objectID1, _objectID2 });

      Assert.That(result, Is.EqualTo(new[] { null, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_WithInvalidObjects ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(true);
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID2)).Returns(false);
      _invalidDomainObjectManagerMock.Setup(stub => stub.GetInvalidObjectReference(_objectID1)).Returns(_domainObject1);

      _dataManagerMock
            .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID2 }, false))
            .Returns(new[] { _dataContainer2 });

      var result = _agent.TryGetObjects<Order>(new[] { _objectID1, _objectID2 });
      Assert.That(result, Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void TryGetObjects_InvalidType ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID1)).Returns(false);
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(_objectID2)).Returns(false);

      _dataManagerMock
          .Setup(mock => mock.GetDataContainersWithLazyLoad(new[] { _objectID1, _objectID2 }, false))
          .Returns(new[] { _dataContainer1, _dataContainer2 })
          .Verifiable();

      Assert.That(
          () => _agent.TryGetObjects<ClassWithAllDataTypes>(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }),
          Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Delete ()
    {
      var sequence = new VerifiableSequence();

      var deleteCommandMock = SetupDeleteExpectations(sequence, _dataManagerMock, _domainObject1);

      _agent.Delete(_domainObject1);

      _dataManagerMock.Verify();
      deleteCommandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Serialization ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var instance = new ObjectLifetimeAgent(
          _transaction,
          new SerializableClientTransactionEventSinkFake(),
          new SerializableInvalidDomainObjectManagerFake(),
          new SerializableDataManagerFake(),
          new SerializableEnlistedDomainObjectManagerFake(),
          new SerializablePersistenceStrategyFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize(instance);

      Assert.That(deserializedInstance.ClientTransaction, Is.Not.Null);
      Assert.That(deserializedInstance.EventSink, Is.Not.Null);
      Assert.That(deserializedInstance.InvalidDomainObjectManager, Is.Not.Null);
      Assert.That(deserializedInstance.DataManager, Is.Not.Null);
      Assert.That(deserializedInstance.EnlistedDomainObjectManager, Is.Not.Null);
      Assert.That(deserializedInstance.PersistenceStrategy, Is.Not.Null);
    }

    private Mock<IDataManagementCommand> SetupDeleteExpectations (VerifiableSequence sequence, Mock<IDataManager> dataManagerMock, DomainObject deletedObject)
    {
      var initialCommandStub = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      var actualCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);

      actualCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(Enumerable.Empty<Exception>());
      initialCommandStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ExpandToAllRelatedObjects())
          .Returns(new ExpandedCommand(actualCommandMock.Object))
          .Verifiable();
      actualCommandMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Begin())
          .Verifiable();
      actualCommandMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Perform())
          .Verifiable();
      actualCommandMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.End())
          .Verifiable();
      dataManagerMock
          .Setup(mock => mock.CreateDeleteCommand(deletedObject))
          .Returns(initialCommandStub.Object)
          .Verifiable();
      return actualCommandMock;
    }

    private Mock<IDataManagementCommand> SetupDeleteExpectationsWithException (
        VerifiableSequence sequence,
        Mock<IDataManager> dataManagerMock,
        DomainObject deletedObject,
        Exception exception)
    {
      var initialCommandStub = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      var actualCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);

      actualCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(Enumerable.Empty<Exception>());
      initialCommandStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ExpandToAllRelatedObjects())
          .Returns(new ExpandedCommand(actualCommandMock.Object))
          .Verifiable();
      actualCommandMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Begin())
          .Throws(exception)
          .Verifiable();
      dataManagerMock
          .Setup(mock => mock.CreateDeleteCommand(deletedObject))
          .Returns(initialCommandStub.Object)
          .Verifiable();
      return actualCommandMock;
    }

    private static void CheckInitializationContext<TExpectedContextType> (
        object initializationContext, ObjectID expectedObjectID, ClientTransaction expectedRootTransaction)
      where TExpectedContextType : IObjectInitializationContext
    {
      Assert.That(
          initializationContext,
          Is.Not.Null
            .And.TypeOf<TExpectedContextType>()
            .And.Property<IObjectInitializationContext>(c => c.ObjectID).EqualTo(expectedObjectID)
            .And.Property<IObjectInitializationContext>(c => c.RootTransaction).SameAs(expectedRootTransaction));
    }

    private void FakeRegisteredObject (NewObjectInitializationContext objectInitializationContext, DomainObject registeredObject)
    {
      PrivateInvoke.SetNonPublicField(objectInitializationContext, "_registeredObject", registeredObject);
    }
  }
}
