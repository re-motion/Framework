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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RealObjectEndPointTest : ClientTransactionBaseTest
  {
    private DataContainer _foreignKeyDataContainer;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkStub;
    private IRealObjectEndPointSyncState _syncStateMock;

    private RealObjectEndPoint _endPoint;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _foreignKeyDataContainer = DataContainer.CreateForExisting (_endPointID.ObjectID, null, pd => pd.DefaultValue);
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink>();
      _syncStateMock = MockRepository.GenerateStrictMock<IRealObjectEndPointSyncState> ();

      _endPoint = new RealObjectEndPoint (
          TestableClientTransaction, _endPointID, _foreignKeyDataContainer, _endPointProviderStub, _transactionEventSinkStub);
      PrivateInvoke.SetNonPublicField (_endPoint, "_syncState", _syncStateMock);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a non-virtual end point.\r\nParameter name: id")]
    public void Initialize_VirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var foreignKeyDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Dev.Null = new RealObjectEndPoint (TestableClientTransaction, id, foreignKeyDataContainer, _endPointProviderStub, _transactionEventSinkStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The foreign key data container must be from the same object as the end point definition.\r\nParameter name: foreignKeyDataContainer")]
    public void Initialize_InvalidDataContainer ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Dev.Null = new RealObjectEndPoint (TestableClientTransaction, id, foreignKeyDataContainer, _endPointProviderStub, _transactionEventSinkStub);
    }

    [Test]
    public void Initialization_SyncState ()
    {
      var endPoint = new RealObjectEndPoint (TestableClientTransaction, _endPointID, _foreignKeyDataContainer, _endPointProviderStub, _transactionEventSinkStub);

      var syncState = RealObjectEndPointTestHelper.GetSyncState (endPoint);
      Assert.That (syncState, Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
      Assert.That (((UnknownRealObjectEndPointSyncState) syncState).VirtualEndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void IsDataComplete_True ()
    {
      Assert.That (_endPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void IsSynchronized ()
    {
      _syncStateMock
          .Expect (mock => mock.IsSynchronized (_endPoint))
          .Return (true);
      _syncStateMock.Replay ();

      var result = _endPoint.IsSynchronized;

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void OppositeObjectID_Get_FromProperty ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order3));
      RealObjectEndPointTestHelper.SetValueViaDataContainer(_endPoint, DomainObjectIDs.Order3);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void OppositeObjectID_Get_DoesNotRaisePropertyReadEvents ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      TestableClientTransaction.AddListener (listenerMock);

      Dev.Null = _endPoint.OppositeObjectID;

      listenerMock.AssertWasNotCalled (mock => mock.PropertyValueReading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DomainObject>.Is.Anything, 
          Arg<PropertyDefinition>.Is.Anything, 
          Arg<ValueAccess>.Is.Anything));
      listenerMock.AssertWasNotCalled (mock => mock.PropertyValueRead (
          Arg<ClientTransaction>.Is.Anything,
          Arg<DomainObject>.Is.Anything,
          Arg<PropertyDefinition>.Is.Anything,
          Arg<object>.Is.Anything,
          Arg<ValueAccess>.Is.Anything));
    }

    [Test]
    public void OppositeObjectID_Set_ToProperty ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.Not.EqualTo (DomainObjectIDs.Order3));

      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order3);

      Assert.That (RealObjectEndPointTestHelper.GetValueViaDataContainer(_endPoint), Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void OriginalOppositeObjectID_Get_FromProperty ()
    {
      RealObjectEndPointTestHelper.SetValueViaDataContainer (_endPoint, DomainObjectIDs.Order3);
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order3));

      _endPoint.ForeignKeyDataContainer.CommitState();

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void HasChanged_FromProperty ()
    {
      Assert.That (_endPoint.HasChanged, Is.False);

      RealObjectEndPointTestHelper.SetValueViaDataContainer (_endPoint, DomainObjectIDs.Order3);

      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void HasBeenTouched_FromProperty ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.ForeignKeyDataContainer.TouchValue (_endPoint.PropertyDefinition);

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void GetOppositeObject ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order1);

      DomainObject oppositeObject;
      using (ClientTransactionScope.EnterNullScope ())
      {
        oppositeObject = _endPoint.GetOppositeObject ();
      }

      Assert.That (oppositeObject, Is.SameAs (LifetimeService.GetObjectReference (TestableClientTransaction, DomainObjectIDs.Order1)));
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.NotLoadedYet), "Data has not been loaded");
    }

    [Test]
    public void GetOppositeObject_Null ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, null);

      var oppositeObject = _endPoint.GetOppositeObject ();
      Assert.That (oppositeObject, Is.Null);
    }

    [Test]
    public void GetOppositeObject_Deleted ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      order1.Delete ();
      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, order1.ID);

      Assert.That (_endPoint.GetOppositeObject (), Is.SameAs (order1));
    }

    [Test]
    public void GetOppositeObject_Invalid ()
    {
      var oppositeObject = Order.NewObject ();

      oppositeObject.Delete ();
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.Invalid));

      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, oppositeObject.ID);

      Assert.That (_endPoint.GetOppositeObject (), Is.SameAs (oppositeObject));
    }

    [Test]
    public void GetOppositeObject_NotFound ()
    {
      var objectID = new ObjectID(typeof (Order), Guid.NewGuid());
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, objectID);

      var oppositeObject = _endPoint.GetOppositeObject ();
      Assert.That (oppositeObject.ID, Is.EqualTo (objectID));
      Assert.That (oppositeObject.State, Is.EqualTo (StateType.NotLoadedYet), "Data has not been loaded");

      Assert.That (() => oppositeObject.EnsureDataAvailable(), Throws.TypeOf<ObjectsNotFoundException>());

      var oppositeObject2 = _endPoint.GetOppositeObject ();
      Assert.That (oppositeObject2.ID, Is.EqualTo (objectID));
      Assert.That (oppositeObject2.State, Is.EqualTo (StateType.Invalid), "Data has not been found");
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order1);
      _foreignKeyDataContainer.CommitState();

      var originalOppositeObject = _endPoint.GetOriginalOppositeObject();

      Assert.That (originalOppositeObject, Is.SameAs (DomainObjectIDs.Order1.GetObjectReference<Order> ()));
      Assert.That (originalOppositeObject.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void GetOriginalOppositeObject_Null ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order5);

      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.Null);
    }

    [Test]
    public void GetOriginalOppositeObject_Deleted ()
    {
      RealObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order1);
      _foreignKeyDataContainer.CommitState();
      var originalOppositeObject = (Order) _endPoint.GetOppositeObject ();
      originalOppositeObject.Delete ();

      Assert.That (originalOppositeObject.State, Is.EqualTo (StateType.Deleted));
      Assert.That (_endPoint.GetOriginalOppositeObject (), Is.SameAs (originalOppositeObject));
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      _endPoint.EnsureDataComplete ();
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointStub = MockRepository.GenerateStub<IVirtualEndPoint> ();
      var oppositeEndPointID = RelationEndPointID.Create (_endPoint.OppositeObjectID, _endPointID.Definition.GetOppositeEndPointDefinition());
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (oppositeEndPointID))
          .Return (oppositeEndPointStub);

      _syncStateMock
          .Expect (mock => mock.Synchronize (_endPoint, oppositeEndPointStub));
      _syncStateMock.Replay ();

      _endPoint.Synchronize ();

      _syncStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkSynchronized ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.SameAs (_syncStateMock));

      _endPoint.MarkSynchronized ();

      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.TypeOf (typeof (SynchronizedRealObjectEndPointSyncState)));
      Assert.That (_endPoint.EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (_endPoint.TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
    }

    [Test]
    public void MarkUnsynchronized ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.SameAs (_syncStateMock));

      _endPoint.MarkUnsynchronized ();
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.TypeOf (typeof (UnsynchronizedRealObjectEndPointSyncState)));
    }

    [Test]
    public void ResetSyncState ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetSyncState (_endPoint), Is.SameAs (_syncStateMock));

      _endPoint.ResetSyncState ();

      var syncState = RealObjectEndPointTestHelper.GetSyncState (_endPoint);
      Assert.That (syncState, Is.TypeOf (typeof (UnknownRealObjectEndPointSyncState)));
      Assert.That (((UnknownRealObjectEndPointSyncState) syncState).VirtualEndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();
      var relatedObject = DomainObjectMother.CreateFakeObject<Order> ();

      Action<DomainObject> oppositeObjectSetter = null;

      _syncStateMock
          .Expect (mock => mock.CreateSetCommand (Arg.Is (_endPoint), Arg.Is (relatedObject), Arg<Action<DomainObject>>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { oppositeObjectSetter = (Action<DomainObject>) mi.Arguments[2]; });
      _syncStateMock.Replay ();

      var result = _endPoint.CreateSetCommand (relatedObject);

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order3));
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order> ();
      oppositeObjectSetter (newRelatedObject);
      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (newRelatedObject.ID));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeResult = MockRepository.GenerateStub<IDataManagementCommand> ();

      Action oppositeObjectSetter = null;
      _syncStateMock
          .Expect (mock => mock.CreateDeleteCommand (Arg.Is (_endPoint), Arg<Action>.Is.Anything))
          .Return (fakeResult)
          .WhenCalled (mi => { oppositeObjectSetter = (Action) mi.Arguments[1]; });
      _syncStateMock.Replay ();

      var result = _endPoint.CreateDeleteCommand ();

      _syncStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));

      RealObjectEndPointTestHelper.SetValueViaDataContainer (_endPoint, DomainObjectIDs.Order1);

      Assert.That (_endPoint.OppositeObjectID, Is.Not.Null);
      oppositeObjectSetter ();
      Assert.That (_endPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void Touch_ToProperty ()
    {
      Assert.That (RealObjectEndPointTestHelper.HasBeenTouchedViaDataContainer (_endPoint), Is.False);

      _endPoint.Touch ();

      Assert.That (RealObjectEndPointTestHelper.HasBeenTouchedViaDataContainer (_endPoint), Is.True);
    }

    [Test]
    public void Commit_ToProperty ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetValueViaDataContainer (_endPoint), Is.Null);

      RealObjectEndPointTestHelper.SetValueViaDataContainer (_endPoint, DomainObjectIDs.Order3);
      Assert.That (RealObjectEndPointTestHelper.HasChangedViaDataContainer(_endPoint), Is.True);

      _endPoint.Commit ();

      Assert.That (RealObjectEndPointTestHelper.HasChangedViaDataContainer (_endPoint), Is.False);
      Assert.That (RealObjectEndPointTestHelper.GetValueViaDataContainer (_endPoint), Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void Rollback_ToProperty ()
    {
      Assert.That (RealObjectEndPointTestHelper.GetValueViaDataContainer (_endPoint), Is.Null);

      RealObjectEndPointTestHelper.SetValueViaDataContainer (_endPoint, DomainObjectIDs.Order3);
      Assert.That (RealObjectEndPointTestHelper.HasChangedViaDataContainer (_endPoint), Is.True);

      _endPoint.Rollback ();

      Assert.That (RealObjectEndPointTestHelper.HasChangedViaDataContainer (_endPoint), Is.False);
      Assert.That (RealObjectEndPointTestHelper.GetValueViaDataContainer (_endPoint), Is.Null);
    }

    [Test]
    public void SetOppositeObjectDataFromSubTransaction ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order3));
      var sourceDataContainer = DataContainer.CreateForExisting (_endPointID.ObjectID, null, pd => pd.DefaultValue);
      
      var source = new RealObjectEndPoint (TestableClientTransaction, _endPointID, sourceDataContainer, _endPointProviderStub, _transactionEventSinkStub);
      RealObjectEndPointTestHelper.SetValueViaDataContainer (source, DomainObjectIDs.Order3);

      PrivateInvoke.InvokeNonPublicMethod (_endPoint, "SetOppositeObjectDataFromSubTransaction", source);

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order3));
      Assert.That (_endPoint.HasChanged, Is.True);
    }
  }
}