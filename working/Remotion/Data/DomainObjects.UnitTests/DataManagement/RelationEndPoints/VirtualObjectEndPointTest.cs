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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class VirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;

    private ILazyLoader _lazyLoaderMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkStub;
    private IVirtualObjectEndPointDataManagerFactory _dataManagerFactory;
    private IVirtualObjectEndPointLoadState _loadStateMock;
    
    private VirtualObjectEndPoint _endPoint;

    private IRealObjectEndPoint _oppositeEndPointStub;
    private OrderTicket _oppositeObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
    
      _lazyLoaderMock = MockRepository.GenerateStrictMock<ILazyLoader>();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider> ();
      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink>();
      _dataManagerFactory = new VirtualObjectEndPointDataManagerFactory();
      _loadStateMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPointLoadState> ();

      _endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current, 
          _endPointID, 
          _lazyLoaderMock, 
          _endPointProviderStub,
          _transactionEventSinkStub,
          _dataManagerFactory);
      PrivateInvoke.SetNonPublicField (_endPoint, "_loadState", _loadStateMock);

      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
      _oppositeObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
    }

    [Test]
    public void Initialization ()
    {
      var endPoint = new VirtualObjectEndPoint (
          ClientTransaction.Current,
          _endPointID,
          _lazyLoaderMock,
          _endPointProviderStub,
          _transactionEventSinkStub,
          _dataManagerFactory);

      Assert.That (endPoint.ID, Is.EqualTo (_endPointID));
      Assert.That (endPoint.ClientTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (endPoint.LazyLoader, Is.SameAs (_lazyLoaderMock));
      Assert.That (endPoint.EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (endPoint.DataManagerFactory, Is.SameAs (_dataManagerFactory));
      Assert.That (endPoint.HasBeenTouched, Is.False);
      Assert.That (endPoint.IsDataComplete, Is.False);

      var loadState = VirtualObjectEndPointTestHelper.GetLoadState (endPoint);
      Assert.That (loadState, Is.TypeOf (typeof (IncompleteVirtualObjectEndPointLoadState)));
      Assert.That (((IncompleteVirtualObjectEndPointLoadState) loadState).DataManagerFactory, Is.SameAs (_dataManagerFactory));
      Assert.That (
          ((IncompleteVirtualObjectEndPointLoadState) loadState).EndPointLoader,
          Is.TypeOf<VirtualObjectEndPoint.EndPointLoader> ().With.Property<VirtualObjectEndPoint.EndPointLoader> (l => l.LazyLoader).SameAs (_lazyLoaderMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a virtual end point.\r\nParameter name: id")]
    public void Initialization_NonVirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      new VirtualObjectEndPoint (TestableClientTransaction, id, _lazyLoaderMock, _endPointProviderStub, _transactionEventSinkStub, _dataManagerFactory);
    }

    [Test]
    public void OppositeObjectID ()
    {
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (_oppositeObject);
      _loadStateMock.Replay();

      var result = _endPoint.OppositeObjectID;
      _loadStateMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (_oppositeObject.ID));
    }

    [Test]
    public void GetData ()
    {
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (_oppositeObject);
      _loadStateMock.Replay ();

      var result = ((IVirtualObjectEndPoint) _endPoint).GetData ();
      Assert.That (result, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void OriginalOppositeObjectID ()
    {
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (_oppositeObject);
      _loadStateMock.Replay ();

      var result = _endPoint.OriginalOppositeObjectID;
      _loadStateMock.VerifyAllExpectations ();

      Assert.That (result, Is.EqualTo (_oppositeObject.ID));
    }

    [Test]
    public void GetOriginalData ()
    {
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (_oppositeObject);
      _loadStateMock.Replay ();

      var result = ((IVirtualObjectEndPoint) _endPoint).GetOriginalData ();
      Assert.That (result, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void HasChanged ()
    {
      _loadStateMock.Expect (mock => mock.HasChanged()).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.HasChanged;

      _loadStateMock.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void IsDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.IsDataComplete()).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.IsDataComplete;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }
    
    [Test]
    public void IsSynchronized ()
    {
      _loadStateMock.Expect (mock => mock.IsSynchronized (_endPoint)).Return (true);
      _loadStateMock.Replay ();

      var result = _endPoint.IsSynchronized;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void CanBeCollected ()
    {
      _loadStateMock.Expect (mock => mock.CanEndPointBeCollected (_endPoint)).Return (true).Repeat.Once ();
      _loadStateMock.Expect (mock => mock.CanEndPointBeCollected (_endPoint)).Return (false).Repeat.Once ();
      _loadStateMock.Replay ();

      var result1 = _endPoint.CanBeCollected;
      var result2 = _endPoint.CanBeCollected;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete ()
    {
      _loadStateMock.Expect (mock => mock.CanDataBeMarkedIncomplete (_endPoint)).Return (true).Repeat.Once ();
      _loadStateMock.Expect (mock => mock.CanDataBeMarkedIncomplete (_endPoint)).Return (false).Repeat.Once ();
      _loadStateMock.Replay ();

      var result1 = _endPoint.CanBeMarkedIncomplete;
      var result2 = _endPoint.CanBeMarkedIncomplete;

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result1, Is.True);
      Assert.That (result2, Is.False);
    }

    [Test]
    public void GetOppositeObject ()
    {
      _loadStateMock.Expect (mock => mock.GetData (_endPoint)).Return (_oppositeObject);
      _loadStateMock.Replay();

      var result = _endPoint.GetOppositeObject ();

      _loadStateMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      _loadStateMock.Expect (mock => mock.GetOriginalData (_endPoint)).Return (_oppositeObject);
      _loadStateMock.Replay ();

      var result = _endPoint.GetOriginalOppositeObject ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_oppositeObject));
    }

    [Test]
    public void EnsureDataComplete ()
    {
      _loadStateMock.Expect (mock => mock.EnsureDataComplete (_endPoint));
      _loadStateMock.Replay();

      _endPoint.EnsureDataComplete();

      _loadStateMock.VerifyAllExpectations();
    }

    [Test]
    public void Synchronize ()
    {
      _loadStateMock.Expect (mock => mock.Synchronize (_endPoint));
      _loadStateMock.Stub (mock => mock.HasChanged ()).Return (true);
      _loadStateMock.Replay ();

      _endPoint.Synchronize();

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.SynchronizeOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Stub (mock => mock.HasChanged ()).Return (true);
      _loadStateMock.Replay ();

      _endPoint.SynchronizeOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataComplete ()
    {
      Action<IVirtualObjectEndPointDataManager> stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataComplete (Arg.Is (_endPoint), Arg.Is (_oppositeObject), Arg<Action<IVirtualObjectEndPointDataManager>>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action<IVirtualObjectEndPointDataManager>) mi.Arguments[2]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataComplete (_oppositeObject);

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (VirtualObjectEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      var dataManagerStub = MockRepository.GenerateStub<IVirtualObjectEndPointDataManager>();
      stateSetter (dataManagerStub);
      
      var newLoadState = VirtualObjectEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.Not.SameAs (_loadStateMock));
      Assert.That (newLoadState, Is.TypeOf (typeof (CompleteVirtualObjectEndPointLoadState)));

      Assert.That (((CompleteVirtualObjectEndPointLoadState) newLoadState).DataManager, Is.SameAs (dataManagerStub));
      Assert.That (((CompleteVirtualObjectEndPointLoadState) newLoadState).TransactionEventSink, Is.SameAs (_transactionEventSinkStub));
      Assert.That (((CompleteVirtualObjectEndPointLoadState) newLoadState).EndPointProvider, Is.SameAs (_endPointProviderStub));
    }

    [Test]
    public void MarkDataComplete_Null ()
    {
      _loadStateMock
          .Expect (
              mock => mock.MarkDataComplete (
                  Arg.Is (_endPoint),
                  Arg.Is ((DomainObject) null),
                  Arg<Action<IVirtualObjectEndPointDataManager>>.Is.Anything));
      _loadStateMock.Replay ();

      _endPoint.MarkDataComplete (null);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Action stateSetter = null;

      _loadStateMock
          .Expect (mock => mock.MarkDataIncomplete (Arg.Is (_endPoint), Arg<Action>.Is.Anything))
          .WhenCalled (mi => { stateSetter = (Action) mi.Arguments[1]; });
      _loadStateMock.Replay ();

      _endPoint.MarkDataIncomplete ();

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (VirtualObjectEndPointTestHelper.GetLoadState (_endPoint), Is.SameAs (_loadStateMock));

      stateSetter();

      var newLoadState = VirtualObjectEndPointTestHelper.GetLoadState (_endPoint);
      Assert.That (newLoadState, Is.Not.SameAs (_loadStateMock));
      Assert.That (newLoadState, Is.TypeOf (typeof (IncompleteVirtualObjectEndPointLoadState)));

      Assert.That (((IncompleteVirtualObjectEndPointLoadState) newLoadState).DataManagerFactory, Is.SameAs (_dataManagerFactory));
      Assert.That (
        ((IncompleteVirtualObjectEndPointLoadState) newLoadState).EndPointLoader,
        Is.TypeOf<VirtualObjectEndPoint.EndPointLoader> ()
          .With.Property<VirtualObjectEndPoint.EndPointLoader> (l => l.LazyLoader).SameAs (_lazyLoaderMock));
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay();

      _endPoint.RegisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterOriginalOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterOriginalOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _loadStateMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_endPoint, _oppositeEndPointStub));
      _loadStateMock.Replay ();

      _endPoint.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateSetCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand>();
      _loadStateMock.Expect (mock => mock.CreateSetCommand (_endPoint, _oppositeObject)).Return (fakeCommand);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateSetCommand (_oppositeObject);

      _loadStateMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      _loadStateMock.Expect (mock => mock.CreateSetCommand (_endPoint, null)).Return (fakeCommand);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateSetCommand (null);

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      _loadStateMock.Expect (mock => mock.CreateDeleteCommand (_endPoint)).Return (fakeCommand);
      _loadStateMock.Replay ();

      var result = _endPoint.CreateDeleteCommand ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCommand));
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.Touch ();
      
      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit_Changed ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Stub(mock => mock.HasChanged()).Return (true);
      _loadStateMock.Expect (mock => mock.Commit (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Commit ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_TouchedUnchanged ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Stub (mock => mock.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      _endPoint.Commit ();

      _loadStateMock.AssertWasNotCalled (mock => mock.Commit (_endPoint));
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_Changed ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Stub (mock => mock.HasChanged ()).Return (true);
      _loadStateMock.Expect (mock => mock.Rollback (_endPoint));
      _loadStateMock.Replay ();

      _endPoint.Rollback ();

      _loadStateMock.VerifyAllExpectations ();
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Rollback_TouchedUnchanged ()
    {
      _endPoint.Touch ();
      Assert.That (_endPoint.HasBeenTouched, Is.True);

      _loadStateMock.Stub (mock => mock.HasChanged ()).Return (false);
      _loadStateMock.Replay ();

      _endPoint.Rollback ();

      _loadStateMock.AssertWasNotCalled (mock => mock.Rollback (_endPoint));
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void SetOppositeObjectDataFromSubTransaction ()
    {
      var source = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, TestableClientTransaction);

      _loadStateMock.Expect (mock => mock.SetDataFromSubTransaction (_endPoint, VirtualObjectEndPointTestHelper.GetLoadState (source)));
      _loadStateMock.Stub (mock => mock.HasChanged()).Return (true);
      _loadStateMock.Replay();

      PrivateInvoke.InvokeNonPublicMethod (_endPoint, "SetOppositeObjectDataFromSubTransaction", source);

      _loadStateMock.VerifyAllExpectations ();
    }

    [Test]
    public void EndPointLoader_LoadEndPointAndGetNewState ()
    {
      var endPointLoader = new VirtualObjectEndPoint.EndPointLoader (_lazyLoaderMock);
      var loadStateFake = MockRepository.GenerateStub<IVirtualObjectEndPointLoadState>();
      _lazyLoaderMock
          .Expect (mock => mock.LoadLazyVirtualObjectEndPoint (_endPointID))
          .WhenCalled (mi => VirtualObjectEndPointTestHelper.SetLoadState (_endPoint, loadStateFake));

      _lazyLoaderMock.Replay();

      var result = endPointLoader.LoadEndPointAndGetNewState (_endPoint);

      _lazyLoaderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (loadStateFake));
    }

    [Test]
    public void EndPointLoader_Serializable ()
    {
      var endPointLoader = new VirtualObjectEndPoint.EndPointLoader (new SerializableLazyLoaderFake ());

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (endPointLoader);

      Assert.That (deserializedInstance.LazyLoader, Is.Not.Null);
    }
  }
}