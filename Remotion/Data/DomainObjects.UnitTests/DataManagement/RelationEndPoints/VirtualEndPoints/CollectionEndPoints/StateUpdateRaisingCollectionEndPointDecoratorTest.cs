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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class StateUpdateRaisingCollectionEndPointDecoratorTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private IVirtualEndPointStateUpdateListener _listenerMock;
    private ICollectionEndPoint _innerEndPointMock;

    private StateUpdateRaisingCollectionEndPointDecorator _decorator;
    private DecoratorTestHelper<ICollectionEndPoint> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      _listenerMock = MockRepository.GenerateStrictMock<IVirtualEndPointStateUpdateListener> ();
      _innerEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint>();
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (false);
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);

      _decorator = new StateUpdateRaisingCollectionEndPointDecorator (_innerEndPointMock, _listenerMock);
      _decoratorTestHelper = new DecoratorTestHelper<ICollectionEndPoint> (_decorator, _innerEndPointMock);
    }

    [Test]
    public void SetDataFromSubTransaction_UnwrapsSourceEndPoint ()
    {
      var sourceInnerEndPoint = MockRepository.GenerateStub<ICollectionEndPoint> ();
      var sourceEndPoint = new StateUpdateRaisingCollectionEndPointDecorator (sourceInnerEndPoint, _listenerMock);

      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, null));
      _listenerMock.Replay();

      _innerEndPointMock.BackToRecord();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (null);
      _innerEndPointMock
          .Expect (ep => ep.SetDataFromSubTransaction (sourceInnerEndPoint))
          .WhenCalled (mi => _listenerMock.AssertWasNotCalled (mock => mock.VirtualEndPointStateUpdated (Arg<RelationEndPointID>.Is.Anything, Arg<bool?>.Is.Anything)));
      _innerEndPointMock.Replay ();

      _decorator.SetDataFromSubTransaction (sourceEndPoint);

      _innerEndPointMock.VerifyAllExpectations ();
      _listenerMock.AssertWasCalled (mock => mock.VirtualEndPointStateUpdated (_endPointID, null));
    }

    [Test]
    public void SetDataFromSubTransaction_WithException ()
    {
      var sourceInnerEndPoint = MockRepository.GenerateStub<ICollectionEndPoint> ();
      var sourceEndPoint = new StateUpdateRaisingCollectionEndPointDecorator (sourceInnerEndPoint, _listenerMock);

      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, null));
      _listenerMock.Replay ();

      var exception = new Exception();
      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (null);
      _innerEndPointMock
          .Expect (ep => ep.SetDataFromSubTransaction (sourceInnerEndPoint))
          .Throw (exception);
      _innerEndPointMock.Replay ();

      Assert.That (() => _decorator.SetDataFromSubTransaction (sourceEndPoint), Throws.Exception.SameAs (exception));

      _innerEndPointMock.VerifyAllExpectations ();
      _listenerMock.VerifyAllExpectations();
    }

    [Test]
    public void Synchronize ()
    {
      CheckDelegationWithStateUpdate (ep => ep.Synchronize());
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      CheckDelegationWithStateUpdate (ep => ep.SynchronizeOppositeEndPoint (endPoint));
    }

    [Test]
    public void Commit ()
    {
      CheckDelegationWithStateUpdate (ep => ep.Commit());
    }

    [Test]
    public void Rollback ()
    {
      CheckDelegationWithStateUpdate (ep => ep.Rollback());
    }

    [Test]
    public void SortCurrentData ()
    {
      CheckDelegationWithStateUpdate (ep => ep.SortCurrentData ((one, two) => 0));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem>();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateRemoveCommand (orderItem));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateDeleteCommand ());
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      var collection = new DomainObjectCollection();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateSetCollectionCommand (collection));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem> ();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateInsertCommand (orderItem, 0));
    }

    [Test]
    public void CreateAddCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem> ();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateAddCommand (orderItem));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem> ();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateReplaceCommand (0, orderItem));
    }

    [Test]
    public void ID ()
    {
      Assert.That (_decorator.ID, Is.EqualTo (_endPointID));
    }

    [Test]
    public void DelegatedMembers ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint>();
      var readOnlyCollectionDataDecorator = new ReadOnlyCollectionDataDecorator (new DomainObjectCollectionData ());
      var domainObjectCollection = new DomainObjectCollection ();
      var eventRaiser = MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser>();
      var orderItem = DomainObjectMother.CreateFakeObject<OrderItem> ();

      _listenerMock.Replay();

      _decoratorTestHelper.CheckDelegation (ep => ep.IsNull, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsNull, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.ClientTransaction, ClientTransaction.CreateRootTransaction());
      _decoratorTestHelper.CheckDelegation (ep => ep.ObjectID, DomainObjectIDs.Order1);
      _decoratorTestHelper.CheckDelegation (ep => ep.Definition, GetEndPointDefinition (typeof (Order), "OrderItems"));
      _decoratorTestHelper.CheckDelegation (ep => ep.RelationDefinition, GetRelationDefinition (typeof (Order), "OrderItems"));
      _decoratorTestHelper.CheckDelegation (ep => ep.HasChanged, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasChanged, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasBeenTouched, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasBeenTouched, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetDomainObject(), orderItem);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetDomainObjectReference(), orderItem);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsDataComplete, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsDataComplete, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.EnsureDataComplete());
      _decoratorTestHelper.CheckDelegation (ep => ep.IsSynchronized, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsSynchronized, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsSynchronized, null);
      _decoratorTestHelper.CheckDelegation (ep => ep.Touch());
      _decoratorTestHelper.CheckDelegation (ep => ep.ValidateMandatory());
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOppositeRelationEndPointIDs(), new[] { endPointID });
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeCollected, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeCollected, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeMarkedIncomplete, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeMarkedIncomplete, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.MarkDataIncomplete ());
      _decoratorTestHelper.CheckDelegation (ep => ep.RegisterOriginalOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.UnregisterOriginalOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.RegisterCurrentOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.UnregisterCurrentOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.GetData (), readOnlyCollectionDataDecorator);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOriginalData (), readOnlyCollectionDataDecorator);
      _decoratorTestHelper.CheckDelegation (ep => ep.Collection, domainObjectCollection);
      _decoratorTestHelper.CheckDelegation (ep => ep.OriginalCollection, domainObjectCollection);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetCollectionEventRaiser (), eventRaiser);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetCollectionWithOriginalData (), domainObjectCollection);
      _decoratorTestHelper.CheckDelegation (ep => ep.MarkDataComplete (new[] { orderItem }));

      _innerEndPointMock.BackToRecord();

      _decoratorTestHelper.CheckDelegation (ep => ep.HasChangedFast, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasChangedFast, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasChangedFast, null);
    }

    [Test]
    public void TestToString ()
    {
      Assert.That (_decorator.ToString(), Is.EqualTo ("StateUpdateRaisingCollectionEndPointDecorator { " + _innerEndPointMock + " }"));
    }

    [Test]
    public void Serialization ()
    {
      var innerEndPoint = new SerializableCollectionEndPointFake ();
      var listener = new SerializableVirtualEndPointStateUpdateListenerFake();
      var instance = new StateUpdateRaisingCollectionEndPointDecorator (innerEndPoint, listener);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.InnerEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.Listener, Is.Not.Null);
    }

    private void CheckDelegationWithStateUpdate (Action<ICollectionEndPoint> action)
    {
      // Check with HasChangedFast returning the same value before and after the operation - no state update should be raised then

      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (true).Repeat.Any ();
      _innerEndPointMock.Replay ();

      _listenerMock.BackToRecord ();
      _listenerMock.Replay ();

      _decoratorTestHelper.CheckDelegation (action);

      _listenerMock.AssertWasNotCalled (mock => mock.VirtualEndPointStateUpdated (Arg<RelationEndPointID>.Is.Anything, Arg<bool?>.Is.Anything));

      // Check with HasChangedFast returning null, also check that listener is called _after_ delegation
      _innerEndPointMock.BackToRecord();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (null);
      _innerEndPointMock.Replay ();

      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, null));
      _listenerMock.Replay();

      _decoratorTestHelper.CheckDelegationWithContinuation (
          action,
          mi => _listenerMock.AssertWasNotCalled (mock => mock.VirtualEndPointStateUpdated (Arg<RelationEndPointID>.Is.Anything, Arg<bool?>.Is.Anything)));

      _listenerMock.AssertWasCalled (mock => mock.VirtualEndPointStateUpdated (_endPointID, null));

      // Check with HasChangedFast returning true, then false

      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (true).Repeat.Once ();
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (false);
      _innerEndPointMock.Replay();

      _listenerMock.BackToRecord();
      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
      _listenerMock.Replay();

      _decoratorTestHelper.CheckDelegation (action);

      _listenerMock.VerifyAllExpectations();

      // Check with exception

      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (true).Repeat.Once ();
      _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (false);
      _innerEndPointMock.Replay ();

      _listenerMock.BackToRecord();
      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
      _listenerMock.Replay ();

      var exception = new Exception ();
      Assert.That (
          () => _decoratorTestHelper.CheckDelegationWithContinuation (action, mi => { throw exception; }), 
          Throws.Exception.SameAs (exception));

      _listenerMock.VerifyAllExpectations();
    }
    
    private void CheckCreateStateUpdateRaisingCommand (Func<ICollectionEndPoint, IDataManagementCommand> action)
    {
      var fakeCommand = MockRepository.GenerateStub<IDataManagementCommand> ();
      _decoratorTestHelper.CheckDelegation (
          action,
          fakeCommand,
          result =>
          {
            Assert.That (
                result,
                Is.TypeOf<VirtualEndPointStateUpdatedRaisingCommandDecorator> ()
                    .With.Property<VirtualEndPointStateUpdatedRaisingCommandDecorator> (d => d.DecoratedCommand).SameAs (fakeCommand)
                    .With.Property<VirtualEndPointStateUpdatedRaisingCommandDecorator> (d => d.ModifiedEndPointID).EqualTo (_endPointID)
                    .And.Property<VirtualEndPointStateUpdatedRaisingCommandDecorator> (d => d.Listener).SameAs (_listenerMock));
            var changeStateProvider = ((VirtualEndPointStateUpdatedRaisingCommandDecorator) result).ChangeStateProvider;

            _innerEndPointMock.BackToRecord ();
            _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
            _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (true);
            _innerEndPointMock.Replay ();
            Assert.That (changeStateProvider (), Is.True);

            _innerEndPointMock.BackToRecord ();
            _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
            _innerEndPointMock.Stub (stub => stub.HasChangedFast).Return (null);
            _innerEndPointMock.Replay ();
            Assert.That (changeStateProvider (), Is.Null);
          });
    }
  }
}