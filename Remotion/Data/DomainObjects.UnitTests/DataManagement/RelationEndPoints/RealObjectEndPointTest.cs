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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RealObjectEndPointTest : ClientTransactionBaseTest
  {
    private DataContainer _foreignKeyDataContainer;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;
    private Mock<IRealObjectEndPointSyncState> _syncStateMock;

    private RealObjectEndPoint _endPoint;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      _foreignKeyDataContainer = DataContainer.CreateForExisting(_endPointID.ObjectID, null, pd => pd.DefaultValue);
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();
      _syncStateMock = new Mock<IRealObjectEndPointSyncState>(MockBehavior.Strict);

      _endPoint = new RealObjectEndPoint(
          TestableClientTransaction, _endPointID, _foreignKeyDataContainer, _endPointProviderStub.Object, _transactionEventSinkStub.Object);
      PrivateInvoke.SetNonPublicField(_endPoint, "_syncState", _syncStateMock.Object);
    }

    [Test]
    public void Initialize_VirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
      var foreignKeyDataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => new RealObjectEndPoint(TestableClientTransaction, id, foreignKeyDataContainer, _endPointProviderStub.Object, _transactionEventSinkStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point ID must refer to a non-virtual end point.", "id"));
    }

    [Test]
    public void Initialize_InvalidDataContainer ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => new RealObjectEndPoint(TestableClientTransaction, id, foreignKeyDataContainer, _endPointProviderStub.Object, _transactionEventSinkStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The foreign key data container must be from the same object as the end point definition.", "foreignKeyDataContainer"));
    }

    [Test]
    public void Initialize_NullEndPointID ()
    {
      var existingEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      var nullEndPointID = RelationEndPointID.Create(null, existingEndPointID.Definition);
      var foreignKeyDataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => new RealObjectEndPoint(TestableClientTransaction, nullEndPointID, foreignKeyDataContainer, _endPointProviderStub.Object, _transactionEventSinkStub.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "End point ID must have a non-null ObjectID.", "id"));
    }

    [Test]
    public void Initialization_SyncState ()
    {
      var endPoint = new RealObjectEndPoint(TestableClientTransaction, _endPointID, _foreignKeyDataContainer, _endPointProviderStub.Object, _transactionEventSinkStub.Object);

      var syncState = RealObjectEndPointTestHelper.GetSyncState(endPoint);
      Assert.That(syncState, Is.TypeOf(typeof(UnknownRealObjectEndPointSyncState)));
      Assert.That(((UnknownRealObjectEndPointSyncState)syncState).VirtualEndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void IsDataComplete_True ()
    {
      Assert.That(_endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void IsSynchronized ()
    {
      _syncStateMock
          .Setup(mock => mock.IsSynchronized(_endPoint))
          .Returns(true)
          .Verifiable();

      var result = _endPoint.IsSynchronized;

      _syncStateMock.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void OppositeObjectID_Get_FromProperty ()
    {
      Assert.That(_endPoint.OppositeObjectID, Is.Not.EqualTo(DomainObjectIDs.Order3));
      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order3);

      Assert.That(_endPoint.OppositeObjectID, Is.EqualTo(DomainObjectIDs.Order3));
    }

    [Test]
    public void OppositeObjectID_Get_DoesNotRaisePropertyReadEvents ()
    {
      var listenerMock = new Mock<IClientTransactionListener>();
      TestableClientTransaction.AddListener(listenerMock.Object);

      Dev.Null = _endPoint.OppositeObjectID;

      listenerMock.Verify(mock => mock.PropertyValueReading(
          It.IsAny<ClientTransaction>(),
          It.IsAny<DomainObject>(),
          It.IsAny<PropertyDefinition>(),
          It.IsAny<ValueAccess>()), Times.Never());
      listenerMock.Verify(mock => mock.PropertyValueRead(
          It.IsAny<ClientTransaction>(),
          It.IsAny<DomainObject>(),
          It.IsAny<PropertyDefinition>(),
          It.IsAny<object>(),
          It.IsAny<ValueAccess>()), Times.Never());
    }

    [Test]
    public void OppositeObjectID_Set_ToProperty ()
    {
      Assert.That(RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.Not.EqualTo(DomainObjectIDs.Order3));

      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, DomainObjectIDs.Order3);

      Assert.That(RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.EqualTo(DomainObjectIDs.Order3));
    }

    [Test]
    public void OriginalOppositeObjectID_Get_FromProperty ()
    {
      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order3);
      Assert.That(_endPoint.OriginalOppositeObjectID, Is.Not.EqualTo(DomainObjectIDs.Order3));

      _endPoint.ForeignKeyDataContainer.CommitState();

      Assert.That(_endPoint.OriginalOppositeObjectID, Is.EqualTo(DomainObjectIDs.Order3));
    }

    [Test]
    public void HasChanged_FromProperty ()
    {
      Assert.That(_endPoint.HasChanged, Is.False);

      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order3);

      Assert.That(_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void HasBeenTouched_FromProperty ()
    {
      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _endPoint.ForeignKeyDataContainer.TouchValue(_endPoint.PropertyDefinition);

      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void GetOppositeObject ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, DomainObjectIDs.Order1);

      IDomainObject oppositeObject;
      using (ClientTransactionScope.EnterNullScope())
      {
        oppositeObject = _endPoint.GetOppositeObject();
      }

      Assert.That(oppositeObject, Is.SameAs(LifetimeService.GetObjectReference(TestableClientTransaction, DomainObjectIDs.Order1)));
      Assert.That(((DomainObject)oppositeObject).State.IsNotLoadedYet, Is.True, "Data has not been loaded");
    }

    [Test]
    public void GetOppositeObject_Null ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, null);

      var oppositeObject = _endPoint.GetOppositeObject();
      Assert.That(oppositeObject, Is.Null);
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order>();
      order1.Delete();
      Assert.That(order1.State.IsDeleted, Is.True);

      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, order1.ID);

      Assert.That(_endPoint.GetOppositeObject(), Is.SameAs(order1));
    }

    [Test]
    public void GetOppositeObject_Invalid ()
    {
      var oppositeObject = Order.NewObject();

      oppositeObject.Delete();
      Assert.That(oppositeObject.State.IsInvalid, Is.True);

      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, oppositeObject.ID);

      Assert.That(_endPoint.GetOppositeObject(), Is.SameAs(oppositeObject));
    }

    [Test]
    public void GetOppositeObject_NotFound ()
    {
      var objectID = new ObjectID(typeof(Order), Guid.NewGuid());
      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, objectID);

      var oppositeObject = _endPoint.GetOppositeObject();
      Assert.That(oppositeObject.ID, Is.EqualTo(objectID));
      Assert.That(((DomainObject)oppositeObject).State.IsNotLoadedYet, Is.True, "Data has not been loaded");

      Assert.That(() => oppositeObject.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException>());

      var oppositeObject2 = _endPoint.GetOppositeObject();
      Assert.That(oppositeObject2.ID, Is.EqualTo(objectID));
      Assert.That(((DomainObject)oppositeObject2).State.IsInvalid, Is.True, "Data has not been found");
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, DomainObjectIDs.Order1);
      _foreignKeyDataContainer.CommitState();

      var originalOppositeObject = _endPoint.GetOriginalOppositeObject();

      Assert.That(originalOppositeObject, Is.SameAs(DomainObjectIDs.Order1.GetObjectReference<Order>()));
      Assert.That(((DomainObject)originalOppositeObject).State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void GetOriginalOppositeObject_Null ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, DomainObjectIDs.Order5);

      Assert.That(_endPoint.GetOriginalOppositeObject(), Is.Null);
    }

    [Test]
    public void GetOriginalOppositeObject_Deleted ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID(_endPoint, DomainObjectIDs.Order1);
      _foreignKeyDataContainer.CommitState();
      var originalOppositeObject = (Order)_endPoint.GetOppositeObject();
      originalOppositeObject.Delete();

      Assert.That(originalOppositeObject.State.IsDeleted, Is.True);
      Assert.That(_endPoint.GetOriginalOppositeObject(), Is.SameAs(originalOppositeObject));
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      _endPoint.EnsureDataComplete();
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointStub = new Mock<IVirtualEndPoint>();
      var oppositeEndPointID = RelationEndPointID.Create(_endPoint.OppositeObjectID, _endPointID.Definition.GetOppositeEndPointDefinition());
      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(oppositeEndPointID))
          .Returns(oppositeEndPointStub.Object);

      _syncStateMock
          .Setup(mock => mock.Synchronize(_endPoint, oppositeEndPointStub.Object))
          .Verifiable();

      _endPoint.Synchronize();

      _syncStateMock.Verify();
    }

    [Test]
    public void MarkSynchronized ()
    {
      Assert.That(RealObjectEndPointTestHelper.GetSyncState(_endPoint), Is.SameAs(_syncStateMock.Object));

      _endPoint.MarkSynchronized();

      Assert.That(RealObjectEndPointTestHelper.GetSyncState(_endPoint), Is.TypeOf(typeof(SynchronizedRealObjectEndPointSyncState)));
      Assert.That(_endPoint.EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(_endPoint.TransactionEventSink, Is.SameAs(_transactionEventSinkStub.Object));
    }

    [Test]
    public void MarkUnsynchronized ()
    {
      Assert.That(RealObjectEndPointTestHelper.GetSyncState(_endPoint), Is.SameAs(_syncStateMock.Object));

      _endPoint.MarkUnsynchronized();
      Assert.That(RealObjectEndPointTestHelper.GetSyncState(_endPoint), Is.TypeOf(typeof(UnsynchronizedRealObjectEndPointSyncState)));
    }

    [Test]
    public void ResetSyncState ()
    {
      Assert.That(RealObjectEndPointTestHelper.GetSyncState(_endPoint), Is.SameAs(_syncStateMock.Object));

      _endPoint.ResetSyncState();

      var syncState = RealObjectEndPointTestHelper.GetSyncState(_endPoint);
      Assert.That(syncState, Is.TypeOf(typeof(UnknownRealObjectEndPointSyncState)));
      Assert.That(((UnknownRealObjectEndPointSyncState)syncState).VirtualEndPointProvider, Is.SameAs(_endPointProviderStub.Object));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();
      var relatedObject = DomainObjectMother.CreateFakeObject<Order>();

      Action<IDomainObject> actualOppositeObjectSetter = null;

      _syncStateMock
          .Setup(mock => mock.CreateSetCommand(_endPoint, relatedObject, It.IsAny<Action<IDomainObject>>()))
          .Returns(fakeResult.Object)
          .Callback((IRealObjectEndPoint _, DomainObject _, Action<IDomainObject> oppositeObjectSetter) => { actualOppositeObjectSetter = oppositeObjectSetter; })
          .Verifiable();

      var result = _endPoint.CreateSetCommand(relatedObject);

      _syncStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));

      Assert.That(_endPoint.OppositeObjectID, Is.Not.EqualTo(DomainObjectIDs.Order3));
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order>();
      actualOppositeObjectSetter(newRelatedObject);
      Assert.That(_endPoint.OppositeObjectID, Is.EqualTo(newRelatedObject.ID));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = new Mock<IDataManagementCommand>();

      Action actualOppositeObjectSetter = null;
      _syncStateMock
          .Setup(mock => mock.CreateDeleteCommand(_endPoint, It.IsAny<Action>()))
          .Returns(fakeResult.Object)
          .Callback((IRealObjectEndPoint _, Action oppositeObjectNullSetter) => { actualOppositeObjectSetter = oppositeObjectNullSetter; })
          .Verifiable();

      var result = _endPoint.CreateDeleteCommand();

      _syncStateMock.Verify();
      Assert.That(result, Is.SameAs(fakeResult.Object));

      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order1);

      Assert.That(_endPoint.OppositeObjectID, Is.Not.Null);
      actualOppositeObjectSetter();
      Assert.That(_endPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void Touch_ToProperty ()
    {
      Assert.That(RealObjectEndPointTestHelper.HasBeenTouchedViaDataContainer(_endPoint), Is.False);

      _endPoint.Touch();

      Assert.That(RealObjectEndPointTestHelper.HasBeenTouchedViaDataContainer(_endPoint), Is.True);
    }

    [Test]
    public void Commit_ToProperty ()
    {
      Assert.That(RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.Null);

      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order3);
      Assert.That(RealObjectEndPointTestHelper.HasChangedViaDataContainer(_endPoint), Is.True);

      _endPoint.Commit();

      Assert.That(RealObjectEndPointTestHelper.HasChangedViaDataContainer(_endPoint), Is.False);
      Assert.That(RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.EqualTo(DomainObjectIDs.Order3));
    }

    [Test]
    public void Rollback_ToProperty ()
    {
      Assert.That(RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.Null);

      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order3);
      Assert.That(RealObjectEndPointTestHelper.HasChangedViaDataContainer(_endPoint), Is.True);

      _endPoint.Rollback();

      Assert.That(RealObjectEndPointTestHelper.HasChangedViaDataContainer(_endPoint), Is.False);
      Assert.That(RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.Null);
    }

    [Test]
    public void SetOppositeObjectDataFromSubTransaction ()
    {
      Assert.That(_endPoint.OppositeObjectID, Is.Not.EqualTo(DomainObjectIDs.Order3));
      var sourceDataContainer = DataContainer.CreateForExisting(_endPointID.ObjectID, null, pd => pd.DefaultValue);

      var source = new RealObjectEndPoint(TestableClientTransaction, _endPointID, sourceDataContainer, _endPointProviderStub.Object, _transactionEventSinkStub.Object);
      RealObjectEndPointTestHelper.SetValueViaDataContainer(source, DomainObjectIDs.Order3);

      PrivateInvoke.InvokeNonPublicMethod(_endPoint, "SetOppositeObjectDataFromSubTransaction", source);

      Assert.That(_endPoint.OppositeObjectID, Is.EqualTo(DomainObjectIDs.Order3));
      Assert.That(_endPoint.HasChanged, Is.True);
    }
  }
}
