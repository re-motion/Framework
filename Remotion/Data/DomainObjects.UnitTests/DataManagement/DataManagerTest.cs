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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataManagerTest : ClientTransactionBaseTest
  {
    private DataManager _dataManager;

    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;
    private Mock<IDataContainerEventListener> _dataContainerEventListenerStub;
    private Mock<IObjectLoader> _objectLoaderMock;
    private Mock<IRelationEndPointManager> _endPointManagerMock;
    private Mock<IInvalidDomainObjectManager> _invalidDomainObjectManagerMock;

    private DataManager _dataManagerWithMocks;

    public override void SetUp ()
    {
      base.SetUp();

      _dataManager = TestableClientTransaction.DataManager;

      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();
      _dataContainerEventListenerStub = new Mock<IDataContainerEventListener>();
      _objectLoaderMock = new Mock<IObjectLoader>(MockBehavior.Strict);
      _endPointManagerMock = new Mock<IRelationEndPointManager>(MockBehavior.Strict);
      _invalidDomainObjectManagerMock = new Mock<IInvalidDomainObjectManager>();

      _dataManagerWithMocks = new DataManager(
          TestableClientTransaction,
          _transactionEventSinkStub.Object,
          _dataContainerEventListenerStub.Object,
          _invalidDomainObjectManagerMock.Object,
          _objectLoaderMock.Object,
          _endPointManagerMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_dataManagerWithMocks.ClientTransaction, Is.SameAs(TestableClientTransaction));
      Assert.That(_dataManagerWithMocks.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(_dataManagerWithMocks.DataContainerEventListener, Is.SameAs(_dataContainerEventListenerStub.Object));
      Assert.That(DataManagerTestHelper.GetRelationEndPointManager(_dataManagerWithMocks), Is.SameAs(_endPointManagerMock.Object));

      var dataContainerMap = DataManagerTestHelper.GetDataContainerMap(_dataManagerWithMocks);
      Assert.That(dataContainerMap.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
    }

    [Test]
    public void GetState ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(_dataManager.GetState(order1.ID).IsUnchanged, Is.True);

      var propertyName = GetPropertyDefinition(typeof(Order), "OrderNumber");
      _dataManager.DataContainers[order1.ID].SetValue(propertyName, 100);

      Assert.That(_dataManager.GetState(order1.ID).IsChanged, Is.True);
    }

    [Test]
    public void GetLoadedDataByObjectState ()
    {
      var unchangedInstance = DomainObjectMother.GetUnchangedObject(TestableClientTransaction, DomainObjectIDs.Order1);
      var changedInstance = DomainObjectMother.GetChangedObject(TestableClientTransaction, DomainObjectIDs.OrderItem1);
      var newInstance = DomainObjectMother.GetNewObject();
      var deletedInstance = DomainObjectMother.GetDeletedObject(TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);

      var unchangedObjects = _dataManager.GetLoadedDataByObjectState(state => state.IsUnchanged);
      var changedOrNewObjects = _dataManager.GetLoadedDataByObjectState(state => state.IsChanged || state.IsNew);
      var deletedOrUnchangedObjects = _dataManager.GetLoadedDataByObjectState(state => state.IsDeleted || state.IsUnchanged);

      CheckPersistableDataSequence(new[] { CreatePersistableData(unchangedInstance) }, unchangedObjects);
      CheckPersistableDataSequence(new[] { CreatePersistableData(changedInstance), CreatePersistableData(newInstance) }, changedOrNewObjects);
      CheckPersistableDataSequence(new[] { CreatePersistableData(deletedInstance), CreatePersistableData(unchangedInstance) }, deletedOrUnchangedObjects);
    }

    [Test]
    public void RegisterDataContainer_RegistersDataContainerInMap ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      SetDomainObject(dataContainer);
      Assert.That(_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);

      _dataManager.RegisterDataContainer(dataContainer);

      Assert.That(_dataManager.DataContainers[DomainObjectIDs.Order1], Is.SameAs(dataContainer));
    }

    [Test]
    public void RegisterDataContainer_RegistersEndPointsInMap ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      SetDomainObject(dataContainer);

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);

      _dataManager.RegisterDataContainer(dataContainer);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);
    }

    [Test]
    public void RegisterDataContainer_New ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      SetDomainObject(dataContainer);

      Assert.That(_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order).FullName + ".OrderItems");
      var realEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order).FullName + ".Customer");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(collectionEndPointID), Is.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(realEndPointID), Is.Null);

      _dataManager.RegisterDataContainer(dataContainer);

      Assert.That(dataContainer.ClientTransaction, Is.SameAs(TestableClientTransaction));
      Assert.That(dataContainer.EventListener, Is.SameAs(_dataManager.DataContainerEventListener));
      Assert.That(_dataManager.DataContainers[DomainObjectIDs.Order1], Is.SameAs(dataContainer));

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(collectionEndPointID).ObjectID, Is.EqualTo(dataContainer.ID));
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(realEndPointID).ObjectID, Is.EqualTo(dataContainer.ID));
    }

    [Test]
    public void RegisterDataContainer_Existing ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      SetDomainObject(dataContainer);

      Assert.That(_dataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order).FullName + ".OrderItems");
      var realEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order).FullName + ".Customer");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(collectionEndPointID), Is.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(realEndPointID), Is.Null);

      _dataManager.RegisterDataContainer(dataContainer);

      Assert.That(dataContainer.ClientTransaction, Is.SameAs(TestableClientTransaction));
      Assert.That(dataContainer.EventListener, Is.SameAs(_dataManager.DataContainerEventListener));
      Assert.That(_dataManager.DataContainers[DomainObjectIDs.Order1], Is.SameAs(dataContainer));

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(collectionEndPointID), Is.Null);
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(realEndPointID).ObjectID, Is.EqualTo(dataContainer.ID));
    }

    [Test]
    public void RegisterDataContainer_ContainerHasNoDomainObject ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => _dataManager.RegisterDataContainer(dataContainer),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The DomainObject of a DataContainer must be set before it can be registered with a transaction."));
    }

    [Test]
    public void RegisterDataContainer_ContainerAlreadyHasTransaction ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      SetDomainObject(dataContainer);

      var otherTransaction = new TestableClientTransaction();
      otherTransaction.DataManager.RegisterDataContainer(dataContainer);
      Assert.That(dataContainer.IsRegistered, Is.True);
      var previousEventListener = dataContainer.EventListener;

      Assert.That(
          () => _dataManager.RegisterDataContainer(dataContainer),
          Throws.InvalidOperationException.With.Message.EqualTo("This DataContainer has already been registered with a ClientTransaction."));

      Assert.That(dataContainer.ClientTransaction, Is.SameAs(otherTransaction));
      Assert.That(dataContainer.EventListener, Is.SameAs(previousEventListener));
      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.Null);
    }

    [Test]
    public void RegisterDataContainer_AlreadyRegistered ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      SetDomainObject(dataContainer);

      var otherDataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      SetDomainObject(otherDataContainer);

      _dataManager.RegisterDataContainer(otherDataContainer);
      Assert.That(otherDataContainer.IsRegistered, Is.True);

      Assert.That(
          () => _dataManager.RegisterDataContainer(dataContainer),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "A DataContainer with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' already exists in this transaction."));

      Assert.That(dataContainer.IsRegistered, Is.False);
      Assert.That(otherDataContainer.IsRegistered, Is.True);
      Assert.That(otherDataContainer.ClientTransaction, Is.SameAs(_dataManager.ClientTransaction));
      Assert.That(otherDataContainer.EventListener, Is.SameAs(_dataManager.DataContainerEventListener));
      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.SameAs(otherDataContainer));
    }

    [Test]
    public void Discard_RemovesEndPoints ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof(OrderTicket).FullName + ".Order");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      _dataManager.Discard(dataContainer);

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
    }

    [Test]
    public void Discard_RemovesDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.SameAs(dataContainer));

      _dataManager.Discard(dataContainer);

      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Discard_DiscardsDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      _dataManager.Discard(dataContainer);

      Assert.That(dataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_MarksObjectInvalid ()
    {
      var dataContainer = PrepareNewDataContainer(_dataManager, DomainObjectIDs.OrderTicket1);

      _dataManager.Discard(dataContainer);

      Assert.That(_dataManager.ClientTransaction.IsInvalid(dataContainer.ID), Is.True);
    }

    [Test]
    public void Discard_ThrowsOnDanglingReferences ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof(OrderTicket).FullName + ".Order");
      var endPoint = (RealObjectEndPoint)_dataManager.GetRelationEndPointWithoutLoading(endPointID);
      RealObjectEndPointTestHelper.SetOppositeObjectID(endPoint, DomainObjectIDs.Order1);
      Assert.That(
          () => _dataManager.Discard(dataContainer),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot discard data for object 'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid': The relations of object "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid' cannot be unloaded.\r\n"
                  + "Relation end-point "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' would "
                  + "leave a dangling reference."));
    }

    [Test]
    public void MarkInvalid ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order>(_dataManagerWithMocks.ClientTransaction);
      _invalidDomainObjectManagerMock.Setup(mock => mock.MarkInvalid(domainObject)).Returns(true).Verifiable();

      Assert.That(_dataManagerWithMocks.ClientTransaction.IsEnlisted(domainObject), Is.True);
      Assert.That(_dataManagerWithMocks.GetDataContainerWithoutLoading(domainObject.ID), Is.Null);
      _endPointManagerMock
          .Setup(stub => stub.GetRelationEndPointWithoutLoading(It.IsAny<RelationEndPointID>()))
          .Returns((IRelationEndPoint)null);

      _dataManagerWithMocks.MarkInvalid(domainObject);

      _invalidDomainObjectManagerMock.Verify();
    }

    [Test]
    public void MarkInvalid_NotEnlisted_Throws ()
    {
      var domainObject = DomainObjectMother.CreateObjectInOtherTransaction<Order>();
      Assert.That(_dataManagerWithMocks.ClientTransaction.IsEnlisted(domainObject), Is.False);

      Assert.That(() => _dataManagerWithMocks.MarkInvalid(domainObject), Throws.TypeOf<ClientTransactionsDifferException>());

      _invalidDomainObjectManagerMock.Verify(mock => mock.MarkInvalid(It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void MarkInvalid_DataContainerRegistered_Throws ()
    {
      var domainObject = DomainObjectMother.CreateObjectInTransaction<Order>(_dataManagerWithMocks.ClientTransaction);
      PrepareLoadedDataContainer(_dataManagerWithMocks, domainObject.ID);
      Assert.That(_dataManagerWithMocks.ClientTransaction.IsEnlisted(domainObject), Is.True);
      Assert.That(_dataManagerWithMocks.GetDataContainerWithoutLoading(domainObject.ID), Is.Not.Null);

      Assert.That(() => _dataManagerWithMocks.MarkInvalid(domainObject), Throws.InvalidOperationException.With.Message.EqualTo(
          "Cannot mark DomainObject '" + domainObject.ID + "' invalid because there is data registered for the object."));

      _invalidDomainObjectManagerMock.Verify(mock => mock.MarkInvalid(It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void MarkNotInvalid ()
    {
      _invalidDomainObjectManagerMock.Setup(mock => mock.MarkNotInvalid(DomainObjectIDs.Order1)).Returns(true).Verifiable();

      _dataManagerWithMocks.MarkNotInvalid(DomainObjectIDs.Order1);

      _invalidDomainObjectManagerMock.Verify();
    }

    [Test]
    public void MarkNotInvalid_NotInvalid ()
    {
      _invalidDomainObjectManagerMock.Setup(mock => mock.MarkNotInvalid(DomainObjectIDs.Order1)).Returns(false).Verifiable();

      Assert.That(
          () => _dataManagerWithMocks.MarkNotInvalid(DomainObjectIDs.Order1),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "Cannot clear the invalid state from object '" + DomainObjectIDs.Order1 + "' - it wasn't marked invalid in the first place."));

      _invalidDomainObjectManagerMock.Verify();
    }

    [Test]
    public void Commit_CommitsRelationEndPoints ()
    {
      _endPointManagerMock.Setup(mock => mock.CommitAllEndPoints()).Verifiable();

      _dataManagerWithMocks.Commit();

      _endPointManagerMock.Verify();
    }

    [Test]
    public void Commit_CommitsDataContainerMap ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      Assert.That(dataContainer.State.IsNew, Is.True);

      _dataManager.Commit();

      Assert.That(dataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void Commit_RemovesDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete();

      Assert.That(dataContainer.State.IsDeleted, Is.True);
      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.Not.Null);

      _dataManager.Commit();

      Assert.That(dataContainer.State.IsDiscarded, Is.True);
      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Commit_RemovesDeletedDataContainers_EndPoints ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete();

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof(OrderTicket).FullName + ".Order");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      _dataManager.Commit();

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
    }

    [Test]
    public void Commit_DiscardsDeletedDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete();

      Assert.That(dataContainer.State.IsDeleted, Is.True);
      Assert.That(dataContainer.State.IsDiscarded, Is.False);

      _dataManager.Commit();

      Assert.That(dataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void Commit_MarksDeletedObjectsAsInvalid ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete();

      Assert.That(_dataManager.ClientTransaction.IsInvalid(DomainObjectIDs.Order1), Is.False);

      _dataManager.Commit();

      Assert.That(_dataManager.ClientTransaction.IsInvalid(DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void Rollback_RollsBackRelationEndPoints ()
    {
      _endPointManagerMock.Setup(mock => mock.RollbackAllEndPoints()).Verifiable();

      _dataManagerWithMocks.Rollback();

      _endPointManagerMock.Verify();
    }

    [Test]
    public void Rollback_RollsBackDataContainerStates ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.OrderTicket1, null, pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      dataContainer.Delete();

      Assert.That(dataContainer.State.IsDeleted, Is.True);

      _dataManager.Rollback();

      Assert.That(dataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      Assert.That(dataContainer.State.IsNew, Is.True);

      _dataManager.Rollback();

      Assert.That(_dataManager.DataContainers[dataContainer.ID], Is.Null);
    }

    [Test]
    public void Rollback_RemovesNewDataContainers_EndPoints ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.OrderTicket1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      var endPointID = RelationEndPointID.Create(dataContainer.ID, typeof(OrderTicket).FullName + ".Order");
      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      _dataManager.Rollback();

      Assert.That(_dataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
    }

    [Test]
    public void Rollback_DiscardsNewDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      Assert.That(dataContainer.State.IsDiscarded, Is.False);

      _dataManager.Rollback();

      Assert.That(dataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void Rollback_MarksNewObjectsAsInvalid ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      ClientTransactionTestHelper.RegisterDataContainer(_dataManager.ClientTransaction, dataContainer);

      Assert.That(_dataManager.ClientTransaction.IsInvalid(DomainObjectIDs.Order1), Is.False);

      _dataManager.Rollback();

      Assert.That(_dataManager.ClientTransaction.IsInvalid(DomainObjectIDs.Order1), Is.True);
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var deletedObject = DomainObjectIDs.Order1.GetObject<Order>();

      var command = _dataManager.CreateDeleteCommand(deletedObject);

      Assert.That(command, Is.InstanceOf(typeof(DeleteCommand)));
      Assert.That(((DeleteCommand)command).ClientTransaction, Is.SameAs(_dataManager.ClientTransaction));
      Assert.That(((DeleteCommand)command).DeletedObject, Is.SameAs(deletedObject));
      Assert.That(((DeleteCommand)command).TransactionEventSink, Is.SameAs(_dataManager.TransactionEventSink));
    }

    [Test]
    public void CreateDeleteCommand_OtherClientTransaction ()
    {
      var order1 = DomainObjectMother.CreateObjectInOtherTransaction<Order>();
      Assert.That(
          () => _dataManager.CreateDeleteCommand(order1),
          Throws.InstanceOf<ClientTransactionsDifferException>()
              .With.Message.Matches("Cannot delete DomainObject '.*', because it belongs to a different ClientTransaction."));
    }

    [Test]
    public void CreateDeleteCommand_DeletedObject ()
    {
      var deletedObject = DomainObjectIDs.Order1.GetObject<Order>();
      deletedObject.Delete();
      Assert.That(deletedObject.State.IsDeleted, Is.True);

      var command = _dataManager.CreateDeleteCommand(deletedObject);
      Assert.That(command, Is.InstanceOf(typeof(NopCommand)));
    }

    [Test]
    public void CreateDeleteCommand_InvalidObject ()
    {
      var invalidObject = Order.NewObject();
      invalidObject.Delete();
      Assert.That(invalidObject.State.IsInvalid, Is.True);
      Assert.That(
          () => _dataManager.CreateDeleteCommand(invalidObject),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void CreateUnloadCommand_NoObjects ()
    {
      var result = _dataManager.CreateUnloadCommand();

      Assert.That(result, Is.TypeOf<NopCommand>());
    }

    [Test]
    public void CreateUnloadCommand_NoLoadedObjects ()
    {
      var result = _dataManager.CreateUnloadCommand(DomainObjectIDs.Order1, DomainObjectIDs.Order3);

      Assert.That(result, Is.TypeOf<NopCommand>());
    }

    [Test]
    public void CreateUnloadCommand_WithLoadedObjects ()
    {
      var loadedDataContainer1 = _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order1, true);
      var loadedDataContainer2 = _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order3, true);

      var loadedObject1 = loadedDataContainer1.DomainObject;
      var loadedObject2 = loadedDataContainer2.DomainObject;

      var result = _dataManager.CreateUnloadCommand(DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4);

      Assert.That(result, Is.TypeOf<UnloadCommand>());
      var unloadCommand = (UnloadCommand)result;
      Assert.That(unloadCommand.DomainObjects, Is.EqualTo(new[] { loadedObject1, loadedObject2 }));
      Assert.That(unloadCommand.UnloadDataCommand, Is.TypeOf<CompositeCommand>());
      Assert.That(unloadCommand.TransactionEventSink, Is.SameAs(_dataManager.TransactionEventSink));

      var unloadDataCommandSteps = ((CompositeCommand)unloadCommand.UnloadDataCommand).GetNestedCommands();
      Assert.That(unloadDataCommandSteps, Has.Count.EqualTo(4));

      Assert.That(unloadDataCommandSteps[0], Is.TypeOf<UnregisterDataContainerCommand>());
      Assert.That(((UnregisterDataContainerCommand)unloadDataCommandSteps[0]).Map, Is.SameAs(_dataManager.DataContainers));
      Assert.That(((UnregisterDataContainerCommand)unloadDataCommandSteps[0]).ObjectID, Is.EqualTo(DomainObjectIDs.Order1));

      Assert.That(unloadDataCommandSteps[1], Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That(((UnregisterEndPointsCommand)unloadDataCommandSteps[1]).EndPoints, Has.Count.EqualTo(2));

      Assert.That(unloadDataCommandSteps[2], Is.TypeOf<UnregisterDataContainerCommand>());
      Assert.That(((UnregisterDataContainerCommand)unloadDataCommandSteps[2]).Map, Is.SameAs(_dataManager.DataContainers));
      Assert.That(((UnregisterDataContainerCommand)unloadDataCommandSteps[2]).ObjectID, Is.EqualTo(DomainObjectIDs.Order3));

      Assert.That(unloadDataCommandSteps[3], Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That(((UnregisterEndPointsCommand)unloadDataCommandSteps[3]).EndPoints, Has.Count.EqualTo(2));
    }

    [Test]
    public void CreateUnloadCommand_WithChangedOrDiscardedObjects ()
    {
      _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order1, true);
      var loadedDataContainer2 = _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order3, true);
      var loadedDataContainer3 = _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order4, true);
      var loadedDataContainer4 = _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order5, true);

      loadedDataContainer2.MarkAsChanged();
      loadedDataContainer3.Delete();
      _dataManager.Discard(loadedDataContainer4);

      var result = _dataManager.CreateUnloadCommand(DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.Order4, DomainObjectIDs.Order5);

      Assert.That(result, Is.TypeOf<ExceptionCommand>());
      var exceptionCommand = (ExceptionCommand)result;
      Assert.That(exceptionCommand.Exception.Message, Is.EqualTo(
          "The state of the following DataContainers prohibits that they be unloaded; only unchanged DataContainers can be unloaded: "
          + "'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' (DataContainerState (Changed, PersistentDataChanged)), "
          + "'Order|3c0fb6ed-de1c-4e70-8d80-218e0bf58df3|System.Guid' (DataContainerState (Deleted)), "
          + "'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' (DataContainerState (Discarded))."));
    }

    [Test]
    public void CreateUnloadCommand_WithUnknownObject ()
    {
      var loadedDataContainer = _dataManager.GetDataContainerWithLazyLoad(DomainObjectIDs.Order4, true);

      var loadedObject = loadedDataContainer.DomainObject;

      var result = _dataManager.CreateUnloadCommand(new ObjectID(typeof(Order), Guid.NewGuid()), DomainObjectIDs.Order4);

      Assert.That(result, Is.TypeOf<UnloadCommand>());
      var unloadCommand = (UnloadCommand)result;
      Assert.That(unloadCommand.DomainObjects, Is.EqualTo(new[] { loadedObject }));
      Assert.That(unloadCommand.UnloadDataCommand, Is.TypeOf<CompositeCommand>());
      Assert.That(unloadCommand.TransactionEventSink, Is.SameAs(_dataManager.TransactionEventSink));

      var unloadDataCommandSteps = ((CompositeCommand)unloadCommand.UnloadDataCommand).GetNestedCommands();
      Assert.That(unloadDataCommandSteps, Has.Count.EqualTo(2));

      Assert.That(unloadDataCommandSteps[0], Is.TypeOf<UnregisterDataContainerCommand>());
      Assert.That(((UnregisterDataContainerCommand)unloadDataCommandSteps[0]).Map, Is.SameAs(_dataManager.DataContainers));
      Assert.That(((UnregisterDataContainerCommand)unloadDataCommandSteps[0]).ObjectID, Is.EqualTo(DomainObjectIDs.Order4));

      Assert.That(unloadDataCommandSteps[1], Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That(((UnregisterEndPointsCommand)unloadDataCommandSteps[1]).EndPoints, Has.Count.EqualTo(2));
    }

    [Test]
    public void CreateUnloadVirtualEndPointCommand ()
    {
      var endPointIDOfUnloadedObject = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPointIDOfUnchangedObject = RelationEndPointID.Create(DomainObjectIDs.Order3, typeof(Order), "OrderItems");
      var endPointIDOfChangedObject = RelationEndPointID.Create(DomainObjectIDs.Order4, typeof(Order), "OrderItems");
      var endPointIDOfNullValue = RelationEndPointID.Create(null, endPointIDOfUnchangedObject.Definition);

      PrepareLoadedDataContainer(_dataManagerWithMocks, endPointIDOfUnchangedObject.ObjectID);

      var dataContainerOfChangedObject = PrepareLoadedDataContainer(_dataManagerWithMocks, endPointIDOfChangedObject.ObjectID);
      dataContainerOfChangedObject.MarkAsChanged();

      var fakeCommand = new Mock<IDataManagementCommand>();
      _endPointManagerMock
          .Setup(
              mock => mock.CreateUnloadVirtualEndPointsCommand(new[] { endPointIDOfUnloadedObject, endPointIDOfUnchangedObject, endPointIDOfNullValue, endPointIDOfChangedObject }))
          .Returns(fakeCommand.Object)
          .Verifiable();

      var result = _dataManagerWithMocks.CreateUnloadVirtualEndPointsCommand(
          endPointIDOfUnloadedObject,
          endPointIDOfUnchangedObject,
          endPointIDOfNullValue,
          endPointIDOfChangedObject);

      _endPointManagerMock.Verify();
      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    [Test]
    public void CreateUnloadVirtualEndPointCommand_NewAndDeletedObjects ()
    {
      var endPointIDOfUnloadedObject = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPointIDOfNewObject = RelationEndPointID.Create(DomainObjectIDs.Order3, typeof(Order), "OrderItems");
      var endPointIDOfDeletedObject = RelationEndPointID.Create(DomainObjectIDs.Order4, typeof(Order), "OrderItems");

      PrepareNewDataContainer(_dataManagerWithMocks, endPointIDOfNewObject.ObjectID);
      var dataContainerOfDeletedObject = PrepareLoadedDataContainer(_dataManagerWithMocks, endPointIDOfDeletedObject.ObjectID);
      dataContainerOfDeletedObject.Delete();

      var result = _dataManagerWithMocks.CreateUnloadVirtualEndPointsCommand(endPointIDOfUnloadedObject, endPointIDOfNewObject, endPointIDOfDeletedObject);

      Assert.That(result, Is.TypeOf<ExceptionCommand>());
      var exception = ((ExceptionCommand)result).Exception;
      var expectedMessage = string.Format(
          "Cannot unload the following relation end-points because they belong to new or deleted objects: {0}, {1}.",
          endPointIDOfNewObject,
          endPointIDOfDeletedObject);
      Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void CreateUnloadAllCommand ()
    {
      PrepareLoadedDataContainer(_dataManagerWithMocks);
      PrepareNewDataContainer(_dataManagerWithMocks, DomainObjectIDs.Order1);

      var command = _dataManagerWithMocks.CreateUnloadAllCommand();

      Assert.That(command, Is.TypeOf<UnloadAllCommand>());
      var unloadAllCommand = (UnloadAllCommand)command;
      Assert.That(unloadAllCommand.RelationEndPointManager, Is.SameAs(DataManagerTestHelper.GetRelationEndPointManager(_dataManagerWithMocks)));
      Assert.That(unloadAllCommand.DataContainerMap, Is.SameAs(DataManagerTestHelper.GetDataContainerMap(_dataManagerWithMocks)));
      Assert.That(unloadAllCommand.InvalidDomainObjectManager, Is.SameAs(DataManagerTestHelper.GetInvalidDomainObjectManager(_dataManagerWithMocks)));
      Assert.That(unloadAllCommand.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
    }

    [Test]
    [Ignore("TODO RM-8240: enable after hard cast has been removed")]
    public void CreateUnloadFilteredDomainObjectsCommand ()
    {
      Predicate<DomainObject> domainObjectFilter = obj => true;
      var command = _dataManagerWithMocks.CreateUnloadFilteredDomainObjectsCommand(domainObjectFilter);

      Assert.That(command, Is.TypeOf<UnloadFilteredDomainObjectsCommand>());
      var unloadFilteredDomainObjectsCommand = (UnloadFilteredDomainObjectsCommand)command;
      Assert.That(unloadFilteredDomainObjectsCommand.DataContainerMap, Is.SameAs(DataManagerTestHelper.GetDataContainerMap(_dataManagerWithMocks)));
      Assert.That(unloadFilteredDomainObjectsCommand.RelationEndPointMap, Is.SameAs(DataManagerTestHelper.GetRelationEndPointManager(_dataManagerWithMocks)));
      Assert.That(unloadFilteredDomainObjectsCommand.InvalidDomainObjectManager, Is.SameAs(DataManagerTestHelper.GetInvalidDomainObjectManager(_dataManagerWithMocks)));
      Assert.That(unloadFilteredDomainObjectsCommand.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
      Assert.That(unloadFilteredDomainObjectsCommand.DomainObjectFilter, Is.SameAs(domainObjectFilter));
    }

    [Test]
    public void GetDataContainerWithoutLoading_NotLoaded ()
    {
      var result = _dataManagerWithMocks.GetDataContainerWithoutLoading(DomainObjectIDs.Order1);
      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetDataContainerWithoutLoading_Loaded ()
    {
      var dataContainer = PrepareLoadedDataContainer(_dataManagerWithMocks);

      var result = _dataManagerWithMocks.GetDataContainerWithoutLoading(dataContainer.ID);

      Assert.That(result, Is.SameAs(dataContainer));
    }

    [Test]
    public void GetDataContainerWithoutLoading_Invalid ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(DomainObjectIDs.Order1)).Returns(true);
      Assert.That(
          () => _dataManagerWithMocks.GetDataContainerWithoutLoading(DomainObjectIDs.Order1),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void GetDataContainerWithLazyLoad_Loaded ()
    {
      var dataContainer = PrepareLoadedDataContainer(_dataManagerWithMocks);

      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();
      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad(dataContainer.ID, throwOnNotFound);

      _objectLoaderMock.Verify();
      Assert.That(result, Is.SameAs(dataContainer));
    }

    [Test]
    public void GetDataContainerWithLazyLoad_NotLoaded ()
    {
      var dataContainer = PrepareNonLoadedDataContainer();

      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();
      _objectLoaderMock
          .Setup(mock => mock.LoadObject(dataContainer.ID, throwOnNotFound))
          .Callback((ObjectID id, bool throwOnNotFound) => DataManagerTestHelper.AddDataContainer(_dataManagerWithMocks, dataContainer))
          .Returns(new FreshlyLoadedObjectData(dataContainer))
          .Verifiable();

      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad(dataContainer.ID, throwOnNotFound);

      _objectLoaderMock.Verify();
      Assert.That(result, Is.SameAs(dataContainer));
    }

    [Test]
    public void GetDataContainerWithLazyLoad_NotFound ()
    {
      var notFoundID = new ObjectID(typeof(Order), Guid.NewGuid());

      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();

      _objectLoaderMock
          .Setup(mock => mock.LoadObject(notFoundID, throwOnNotFound))
          .Callback((ObjectID id, bool throwOnNotFound) => _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(notFoundID)).Returns(true))
          .Returns(new NotFoundLoadedObjectData(notFoundID))
          .Verifiable();

      var result = _dataManagerWithMocks.GetDataContainerWithLazyLoad(notFoundID, throwOnNotFound);

      _objectLoaderMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetDataContainerWithLazyLoad_Invalid ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(DomainObjectIDs.Order1)).Returns(true);
      Assert.That(
          () => _dataManagerWithMocks.GetDataContainerWithLazyLoad(DomainObjectIDs.Order1, BooleanObjectMother.GetRandomBoolean()),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void GetDataContainersWithLazyLoad ()
    {
      var loadedDataContainer = PrepareLoadedDataContainer(_dataManagerWithMocks);

      var nonLoadedDataContainer1 = PrepareNonLoadedDataContainer();
      var nonLoadedDataContainer2 = PrepareNonLoadedDataContainer();

      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();

      _objectLoaderMock
          .Setup(mock => mock.LoadObjects(new[] { nonLoadedDataContainer1.ID, nonLoadedDataContainer2.ID }, throwOnNotFound))
          .Callback(
              (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound) =>
              {
                DataManagerTestHelper.AddDataContainer(_dataManagerWithMocks, nonLoadedDataContainer1);
                DataManagerTestHelper.AddDataContainer(_dataManagerWithMocks, nonLoadedDataContainer2);
              })
          .Returns(new[] { new FreshlyLoadedObjectData(nonLoadedDataContainer1), new FreshlyLoadedObjectData(nonLoadedDataContainer2) })
          .Verifiable();

      var result = _dataManagerWithMocks.GetDataContainersWithLazyLoad(
          new[] { nonLoadedDataContainer1.ID, loadedDataContainer.ID, nonLoadedDataContainer2.ID },
          throwOnNotFound);

      _objectLoaderMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { nonLoadedDataContainer1, loadedDataContainer, nonLoadedDataContainer2 }));
    }

    [Test]
    public void GetDataContainersWithLazyLoad_WithInvalidID ()
    {
      _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(DomainObjectIDs.Order1)).Returns(true);

      _objectLoaderMock
          .Setup(mock => mock.LoadObjects(It.IsAny<IEnumerable<ObjectID>>(), It.IsAny<bool>()))
          // evaluate args to trigger exception
          .Callback((IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound) => idsToBeLoaded.ToList())
          .Returns((ICollection<ILoadedObjectData>)null)
          .Verifiable();

      Assert.That(
          () =>
          _dataManagerWithMocks.GetDataContainersWithLazyLoad(
              new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }, BooleanObjectMother.GetRandomBoolean()),
          Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void GetDataContainersWithLazyLoad_NotFound ()
    {
      var loadedDataContainer = PrepareLoadedDataContainer(_dataManagerWithMocks);
      var nonLoadedDataContainer = PrepareNonLoadedDataContainer();

      var notFoundID = new ObjectID(typeof(Order), Guid.NewGuid());

      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();

      _objectLoaderMock
          .Setup(mock => mock.LoadObjects(new[] { nonLoadedDataContainer.ID, notFoundID }, throwOnNotFound))
          .Callback(
              (IEnumerable<ObjectID> idsToBeLoaded, bool throwOnNotFound) =>
              {
                DataManagerTestHelper.AddDataContainer(_dataManagerWithMocks, nonLoadedDataContainer);
                _invalidDomainObjectManagerMock.Setup(stub => stub.IsInvalid(notFoundID)).Returns(true);
              })
          .Returns(new ILoadedObjectData[] { new FreshlyLoadedObjectData(nonLoadedDataContainer), new NotFoundLoadedObjectData(notFoundID) })
          .Verifiable();

      var result = _dataManagerWithMocks.GetDataContainersWithLazyLoad(
          new[] { nonLoadedDataContainer.ID, loadedDataContainer.ID, notFoundID },
          throwOnNotFound);

      _objectLoaderMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { nonLoadedDataContainer, loadedDataContainer, null}));
    }

    [Test]
    public void GetDataContainersWithLazyLoad_AllLoadedObjects ()
    {
      var loadedDataContainer = PrepareLoadedDataContainer(_dataManagerWithMocks);

      var result = _dataManagerWithMocks.GetDataContainersWithLazyLoad(
          new[] { loadedDataContainer.ID },
          true);

      _objectLoaderMock.Verify(mock => mock.LoadObjects(It.IsAny<IEnumerable<ObjectID>>(), It.IsAny<bool>()), Times.Never());
      Assert.That(result, Is.EqualTo(new[] { loadedDataContainer }));
    }

    [Test]
    public void LoadLazyCollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");

      var fakeOrderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      var loadedObjectDataStub = new Mock<ILoadedObjectData>();
      loadedObjectDataStub.Setup(stub => stub.GetDomainObjectReference()).Returns(fakeOrderItem);

      var endPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.ID).Returns(endPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMock.Setup(stub => stub.IsDataComplete).Returns(false);
      endPointMock.Setup(mock => mock.MarkDataComplete(new[] { fakeOrderItem })).Verifiable();

      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns(endPointMock.Object);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadRelatedObjects(endPointID))
          .Returns(new[] { loadedObjectDataStub.Object })
          .Verifiable();

      _dataManagerWithMocks.LoadLazyCollectionEndPoint(endPointID);

      _objectLoaderMock.Verify();
      endPointMock.Verify();
    }

    [Test]
    public void LoadLazyCollectionEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns((IRelationEndPoint)null);
      Assert.That(
          () => _dataManagerWithMocks.LoadLazyCollectionEndPoint(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given ID does not identify an ICollectionEndPoint managed by this DataManager.", "endPointID"));
    }

    [Test]
    public void LoadLazyCollectionEndPoint_NotICollectionEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
      var endPointStub = new Mock<IVirtualObjectEndPoint>();
      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns(endPointStub.Object);
      Assert.That(
          () => _dataManagerWithMocks.LoadLazyCollectionEndPoint(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given ID does not identify an ICollectionEndPoint managed by this DataManager.", "endPointID"));
    }

    [Test]
    public void LoadLazyCollectionEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      var endPointStub = new Mock<ICollectionEndPoint<ICollectionEndPointData>>();
      endPointStub.Setup(stub => stub.ID).Returns(endPointID);
      endPointStub.Setup(stub => stub.IsDataComplete).Returns(true);

      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns(endPointStub.Object);
      Assert.That(
          () => _dataManagerWithMocks.LoadLazyCollectionEndPoint(endPointID),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The given end-point cannot be loaded, its data is already complete."));
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_MarkedCompleteByLoader ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");

      var fakeOrderTicket = DomainObjectMother.CreateFakeObject<OrderTicket>();
      var loadedObjectDataStub = new Mock<ILoadedObjectData>();
      loadedObjectDataStub.Setup(stub => stub.GetDomainObjectReference()).Returns(fakeOrderTicket);

      var endPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.ID).Returns(endPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMock.Setup(stub => stub.IsDataComplete).Returns(false);

      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns(endPointMock.Object);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadRelatedObject(endPointID))
          .Returns(loadedObjectDataStub.Object)
          .Callback((RelationEndPointID relationEndPointID) => endPointMock.Setup(stub => stub.IsDataComplete).Returns(true))
          .Verifiable();

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint(endPointID);

      _objectLoaderMock.Verify();
      endPointMock.Verify();
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_NotMarkedCompleteByLoader ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");

      var fakeOrderTicket = DomainObjectMother.CreateFakeObject<OrderTicket>();
      var loadedObjectDataStub = new Mock<ILoadedObjectData>();
      loadedObjectDataStub.Setup(stub => stub.GetDomainObjectReference()).Returns(fakeOrderTicket);

      var endPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.ID).Returns(endPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(endPointID.Definition);
      endPointMock.Setup(stub => stub.IsDataComplete).Returns(false);
      endPointMock.Setup(mock => mock.MarkDataComplete(fakeOrderTicket)).Verifiable();

      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns(endPointMock.Object);

      _objectLoaderMock
          .Setup(mock => mock.GetOrLoadRelatedObject(endPointID))
          .Returns(loadedObjectDataStub.Object)
          .Verifiable();

      _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint(endPointID);

      _objectLoaderMock.Verify();
      endPointMock.Verify();
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_NotRegistered ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns((IRelationEndPoint)null);
      Assert.That(
          () => _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given ID does not identify an IVirtualObjectEndPoint managed by this DataManager.", "endPointID"));
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_NotIVirtualObjectEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderItems");
      var endPointStub = new Mock<ICollectionEndPoint<ICollectionEndPointData>>();
      _endPointManagerMock.Setup(stub => stub.GetRelationEndPointWithoutLoading(endPointID)).Returns(endPointStub.Object);
      Assert.That(
          () => _dataManagerWithMocks.LoadLazyVirtualObjectEndPoint(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given ID does not identify an IVirtualObjectEndPoint managed by this DataManager.", "endPointID"));
    }

    [Test]
    public void LoadLazyVirtualObjectEndPoint_AlreadyLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
      var endPoint = (IVirtualObjectEndPoint)_dataManager.GetRelationEndPointWithLazyLoad(endPointID);
      endPoint.EnsureDataComplete();
      Assert.That(
          () => _dataManager.LoadLazyVirtualObjectEndPoint(endPointID),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The given end-point cannot be loaded, its data is already complete."));
    }

    [Test]
    public void LoadLazyDataContainer ()
    {
      var fakeDataContainer = PrepareNonLoadedDataContainer();

      _objectLoaderMock
          .Setup(mock => mock.LoadObject(fakeDataContainer.ID, true))
          .Returns(new FreshlyLoadedObjectData(fakeDataContainer))
          .Callback((ObjectID id, bool throwOnNotFound) => DataManagerTestHelper.AddDataContainer(_dataManagerWithMocks, fakeDataContainer))
          .Verifiable();

      var result = _dataManagerWithMocks.LoadLazyDataContainer(fakeDataContainer.ID);

      _objectLoaderMock.Verify();
      Assert.That(result, Is.SameAs(fakeDataContainer));
    }

    [Test]
    public void LoadLazyDataContainer_AlreadyLoaded ()
    {
      var fakeObject = DomainObjectMother.CreateFakeObject<Order>(DomainObjectIDs.Order1);
      var fakeDataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      fakeDataContainer.SetDomainObject(fakeObject);
      DataManagerTestHelper.AddDataContainer(_dataManagerWithMocks, fakeDataContainer);

      Assert.That(
          () => _dataManagerWithMocks.LoadLazyDataContainer(DomainObjectIDs.Order1),
          Throws.InvalidOperationException.With.Message.EquivalentTo(
              "The given DataContainer cannot be loaded, its data is already available."));
    }

    [Test]
    public void GetRelationEndPointWithLazyLoad ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, GetPropertyIdentifier(typeof(Order), "OrderTicket"));
      var fakeEndPoint = new Mock<IRelationEndPoint>();

      _endPointManagerMock.Setup(mock => mock.GetRelationEndPointWithLazyLoad(endPointID)).Returns(fakeEndPoint.Object).Verifiable();

      var result = _dataManagerWithMocks.GetRelationEndPointWithLazyLoad(endPointID);

      _endPointManagerMock.Verify();
      Assert.That(result, Is.SameAs(fakeEndPoint.Object));
    }

    [Test]
    public void GetRelationEndPointWithoutLoading ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, GetPropertyIdentifier(typeof(Order), "OrderTicket"));
      var fakeEndPoint = new Mock<IRelationEndPoint>();

      _endPointManagerMock.Setup(mock => mock.GetRelationEndPointWithoutLoading(endPointID)).Returns(fakeEndPoint.Object).Verifiable();

      var result = _dataManagerWithMocks.GetRelationEndPointWithoutLoading(endPointID);

      _endPointManagerMock.Verify();
      Assert.That(result, Is.SameAs(fakeEndPoint.Object));
    }

    [Test]
    public void GetOrCreateVirtualEndPoint ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, GetPropertyIdentifier(typeof(Order), "OrderTicket"));
      var fakeVirtualEndPoint = new Mock<IVirtualEndPoint>();

      _endPointManagerMock.Setup(mock => mock.GetOrCreateVirtualEndPoint(endPointID)).Returns(fakeVirtualEndPoint.Object).Verifiable();

      var result = _dataManagerWithMocks.GetOrCreateVirtualEndPoint(endPointID);

      _endPointManagerMock.Verify();
      Assert.That(result, Is.SameAs(fakeVirtualEndPoint.Object));
    }

    private PersistableData CreatePersistableData (DomainObject domainObject)
    {
      var dataContainer = TestableClientTransaction.DataManager.DataContainers[domainObject.ID];
      return new PersistableData(domainObject, domainObject.State, dataContainer, _dataManager.RelationEndPoints.Where(ep => ep.ObjectID == domainObject.ID));
    }

    private void SetDomainObject (DataContainer dc)
    {
      dc.SetDomainObject(DomainObjectMother.GetObjectReference<DomainObject>(_dataManager.ClientTransaction, dc.ID));
    }

    private void CheckPersistableDataSequence (IEnumerable<PersistableData> expected, IEnumerable<PersistableData> actual)
    {
      var expectedList = expected.ToList();
      var actualDictionary = actual.ToDictionary(pd => pd.DomainObject);

      Assert.That(actualDictionary.Count, Is.EqualTo(expectedList.Count));
      foreach (var expectedPersistableData in expectedList)
      {
        CheckHasPersistableDataItem(expectedPersistableData, actualDictionary);
      }
    }

    private void CheckHasPersistableDataItem (PersistableData expectedPersistableData, Dictionary<DomainObject, PersistableData> actualDictionary)
    {
      var actualPersistableData = actualDictionary.GetValueOrDefault(expectedPersistableData.DomainObject);
      Assert.That(actualPersistableData, Is.Not.Null, $"Expected persistable item: {expectedPersistableData.DomainObject.ID}");
      Assert.That(actualPersistableData.DomainObjectState, Is.EqualTo(expectedPersistableData.DomainObjectState));
      Assert.That(actualPersistableData.DataContainer, Is.SameAs(expectedPersistableData.DataContainer));
    }

    private DataContainer PrepareNonLoadedDataContainer ()
    {
      var nonLoadedDomainObject = DomainObjectMother.CreateFakeObject<Order>();
      var nonLoadedDataContainer = DataContainer.CreateNew(nonLoadedDomainObject.ID);
      return nonLoadedDataContainer;
    }

    private DataContainer PrepareLoadedDataContainer (DataManager dataManager)
    {
      return PrepareLoadedDataContainer(dataManager, new ObjectID(typeof(Order), Guid.NewGuid()));
    }

    private DataContainer PrepareLoadedDataContainer (DataManager dataManager, ObjectID objectID)
    {
      var loadedDomainObject = DomainObjectMother.CreateFakeObject(objectID);
      var loadedDataContainer = DataContainer.CreateForExisting(objectID, null, pd => pd.DefaultValue);
      loadedDataContainer.SetDomainObject(loadedDomainObject);
      DataManagerTestHelper.AddDataContainer(dataManager, loadedDataContainer);
      return loadedDataContainer;
    }

    private DataContainer PrepareNewDataContainer (DataManager dataManager, ObjectID objectID)
    {
      var loadedDomainObject = DomainObjectMother.CreateFakeObject(objectID);
      var loadedDataContainer = DataContainer.CreateNew(objectID);
      loadedDataContainer.SetDomainObject(loadedDomainObject);
      DataManagerTestHelper.AddDataContainer(dataManager, loadedDataContainer);
      return loadedDataContainer;
    }

  }
}
