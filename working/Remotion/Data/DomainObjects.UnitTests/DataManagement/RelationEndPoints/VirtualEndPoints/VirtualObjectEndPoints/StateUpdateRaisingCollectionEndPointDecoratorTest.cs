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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.UnitTests.UnitTesting;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints
{
  [TestFixture]
  public class StateUpdateRaisingVirtualObjectEndPointDecoratorTest : StandardMappingTest
  {
    private RelationEndPointID _endPointID;
    private IVirtualEndPointStateUpdateListener _listenerMock;
    private IVirtualObjectEndPoint _innerEndPointMock;

    private StateUpdateRaisingVirtualObjectEndPointDecorator _decorator;
    private DecoratorTestHelper<IVirtualObjectEndPoint> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      _listenerMock = MockRepository.GenerateStrictMock<IVirtualEndPointStateUpdateListener> ();
      _innerEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);

      _decorator = new StateUpdateRaisingVirtualObjectEndPointDecorator (_innerEndPointMock, _listenerMock);
      _decoratorTestHelper = new DecoratorTestHelper<IVirtualObjectEndPoint> (_decorator, _innerEndPointMock);
    }

    [Test]
    public void SetDataFromSubTransaction_UnwrapsSourceEndPoint ()
    {
      var sourceInnerEndPoint = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      var sourceEndPoint = new StateUpdateRaisingVirtualObjectEndPointDecorator (sourceInnerEndPoint, _listenerMock);

      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
      _listenerMock.Replay ();

      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (true).Repeat.Once ();
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _innerEndPointMock
          .Expect (ep => ep.SetDataFromSubTransaction (sourceInnerEndPoint))
          .WhenCalled (mi => _listenerMock.AssertWasNotCalled (mock => mock.VirtualEndPointStateUpdated (Arg<RelationEndPointID>.Is.Anything, Arg<bool?>.Is.Anything)));
      _innerEndPointMock.Replay ();

      _decorator.SetDataFromSubTransaction (sourceEndPoint);

      _innerEndPointMock.VerifyAllExpectations ();
      _listenerMock.AssertWasCalled (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
    }

    [Test]
    public void SetDataFromSubTransaction_WithException ()
    {
      var sourceInnerEndPoint = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      var sourceEndPoint = new StateUpdateRaisingVirtualObjectEndPointDecorator (sourceInnerEndPoint, _listenerMock);

      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
      _listenerMock.Replay ();

      var exception = new Exception ();
      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (true).Repeat.Once ();
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _innerEndPointMock
          .Expect (ep => ep.SetDataFromSubTransaction (sourceInnerEndPoint))
          .Throw (exception);
      _innerEndPointMock.Replay ();

      Assert.That (() => _decorator.SetDataFromSubTransaction (sourceEndPoint), Throws.Exception.SameAs (exception));

      _innerEndPointMock.VerifyAllExpectations ();
      _listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void Synchronize ()
    {
      CheckDelegationWithStateUpdate (ep => ep.Synchronize ());
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
      CheckDelegationWithStateUpdate (ep => ep.Commit ());
    }

    [Test]
    public void Rollback ()
    {
      CheckDelegationWithStateUpdate (ep => ep.Rollback ());
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var orderTicket = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateRemoveCommand (orderTicket));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateDeleteCommand ());
    }

    [Test]
    public void CreateSetCommand ()
    {
      var orderTicket = DomainObjectMother.CreateFakeObject<OrderTicket> ();
      CheckCreateStateUpdateRaisingCommand (ep => ep.CreateSetCommand (orderTicket));
    }

    [Test]
    public void ID ()
    {
      Assert.That (_decorator.ID, Is.EqualTo (_endPointID));
    }

    [Test]
    public void DelegatedMembers ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      var endPoint = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      var orderTicket = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _listenerMock.Replay ();

      _decoratorTestHelper.CheckDelegation (ep => ep.IsNull, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsNull, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.ClientTransaction, ClientTransaction.CreateRootTransaction ());
      _decoratorTestHelper.CheckDelegation (ep => ep.ObjectID, DomainObjectIDs.Order1);
      _decoratorTestHelper.CheckDelegation (ep => ep.Definition, GetEndPointDefinition (typeof (Order), "OrderTicket"));
      _decoratorTestHelper.CheckDelegation (ep => ep.RelationDefinition, GetRelationDefinition (typeof (Order), "OrderTicket"));
      _decoratorTestHelper.CheckDelegation (ep => ep.HasBeenTouched, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasBeenTouched, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetDomainObject (), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetDomainObjectReference (), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsDataComplete, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsDataComplete, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.EnsureDataComplete ());
      _decoratorTestHelper.CheckDelegation (ep => ep.IsSynchronized, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsSynchronized, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.IsSynchronized, null);
      _decoratorTestHelper.CheckDelegation (ep => ep.Touch ());
      _decoratorTestHelper.CheckDelegation (ep => ep.ValidateMandatory ());
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOppositeRelationEndPointIDs (), new[] { endPointID });
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeCollected, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeCollected, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeMarkedIncomplete, false);
      _decoratorTestHelper.CheckDelegation (ep => ep.CanBeMarkedIncomplete, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.MarkDataIncomplete ());
      _decoratorTestHelper.CheckDelegation (ep => ep.RegisterOriginalOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.UnregisterOriginalOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.RegisterCurrentOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.UnregisterCurrentOppositeEndPoint (endPoint));
      _decoratorTestHelper.CheckDelegation (ep => ep.GetData (), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOriginalData (), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.OppositeObjectID, DomainObjectIDs.Order1);
      _decoratorTestHelper.CheckDelegation (ep => ep.OriginalOppositeObjectID, DomainObjectIDs.Order1);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOppositeObject (), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOppositeObject (), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOriginalOppositeObject(), orderTicket);
      _decoratorTestHelper.CheckDelegation (ep => ep.GetOppositeRelationEndPointID(), endPointID);
      _decoratorTestHelper.CheckDelegation (ep => ep.MarkDataComplete (orderTicket));

      _innerEndPointMock.BackToRecord ();
      
      _decoratorTestHelper.CheckDelegation (ep => ep.HasChanged, true);
      _decoratorTestHelper.CheckDelegation (ep => ep.HasChanged, false);
    }

    [Test]
    public void TestToString ()
    {
      Assert.That (_decorator.ToString(), Is.EqualTo ("StateUpdateRaisingVirtualObjectEndPointDecorator { " + _innerEndPointMock + " }"));
    }

    [Test]
    public void Serialization ()
    {
      var innerEndPoint = new SerializableVirtualObjectEndPointFake ();
      var listener = new SerializableVirtualEndPointStateUpdateListenerFake ();
      var instance = new StateUpdateRaisingVirtualObjectEndPointDecorator (innerEndPoint, listener);

      var deserializedInstance = FlattenedSerializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.InnerEndPoint, Is.Not.Null);
      Assert.That (deserializedInstance.Listener, Is.Not.Null);
    }

    private void CheckDelegationWithStateUpdate (Action<IVirtualObjectEndPoint> action)
    {
      // Check with HasChanged returning the same value before and after the operation - no state update should be raised then

      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (true).Repeat.Any ();
      _innerEndPointMock.Replay ();

      _listenerMock.BackToRecord ();
      _listenerMock.Replay ();

      _decoratorTestHelper.CheckDelegation (action);

      _listenerMock.AssertWasNotCalled (mock => mock.VirtualEndPointStateUpdated (Arg<RelationEndPointID>.Is.Anything, Arg<bool?>.Is.Anything));
      
      // Check with HasChanged returning true, then false; also check that listener is called _after_ delegation
      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (true).Repeat.Once ();
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _innerEndPointMock.Replay ();

      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
      _listenerMock.Replay ();

      _decoratorTestHelper.CheckDelegationWithContinuation (
          action,
          mi => _listenerMock.AssertWasNotCalled (mock => mock.VirtualEndPointStateUpdated (Arg<RelationEndPointID>.Is.Anything, Arg<bool?>.Is.Anything)));

      _listenerMock.AssertWasCalled (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));

      // Check with exception

      _innerEndPointMock.BackToRecord ();
      _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (true).Repeat.Once ();
      _innerEndPointMock.Stub (stub => stub.HasChanged).Return (false);
      _innerEndPointMock.Replay ();

      _listenerMock.BackToRecord ();
      _listenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_endPointID, false));
      _listenerMock.Replay ();

      var exception = new Exception ();
      Assert.That (
          () => _decoratorTestHelper.CheckDelegationWithContinuation (action, mi => { throw exception; }),
          Throws.Exception.SameAs (exception));

      _listenerMock.VerifyAllExpectations ();
    }

    private void CheckCreateStateUpdateRaisingCommand (Func<IVirtualObjectEndPoint, IDataManagementCommand> action)
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
            _innerEndPointMock.Stub (stub => stub.HasChanged).Return (true);
            _innerEndPointMock.Replay ();
            Assert.That (changeStateProvider (), Is.True);

            _innerEndPointMock.BackToRecord ();
            _innerEndPointMock.Stub (stub => stub.ID).Return (_endPointID);
            _innerEndPointMock.Stub (stub => stub.HasChanged).Return (false);
            _innerEndPointMock.Replay ();
            Assert.That (changeStateProvider (), Is.False);
          });
    }
  }
}