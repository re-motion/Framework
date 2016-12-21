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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  [TestFixture]
  public class SynchronizedRealObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IRealObjectEndPoint _endPointMock;
    private IRelationEndPointProvider _endPointProviderStub;
    private IClientTransactionEventSink _transactionEventSinkStub;

    private SynchronizedRealObjectEndPointSyncState _state;

    private Order _order;
    private Location _location;

    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;
    private IRelationEndPointDefinition _locationClientEndPointDefinition;
    private IRelationEndPointDefinition _orderCustomerEndPointDefinition;

    private Action<DomainObject> _fakeSetter;
    private Action _fakeNullSetter;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      _endPointProviderStub = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _transactionEventSinkStub = MockRepository.GenerateStub<IClientTransactionEventSink>();

      _state = new SynchronizedRealObjectEndPointSyncState (_endPointProviderStub, _transactionEventSinkStub);

      _order = DomainObjectMother.CreateFakeObject<Order> ();
      _location = DomainObjectMother.CreateFakeObject<Location> ();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition(typeof (Order), "OrderTicket");
      _locationClientEndPointDefinition = GetRelationEndPointDefinition (typeof (Location), "Client");
      _orderCustomerEndPointDefinition = GetRelationEndPointDefinition (typeof (Order), "Customer");

      _fakeSetter = domainObject => { };
      _fakeNullSetter = () => { };
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_state.IsSynchronized(_endPointMock), Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      oppositeEndPointMock.Replay();
      
      _state.Synchronize (_endPointMock, oppositeEndPointMock);

      oppositeEndPointMock.AssertWasNotCalled(mock=>mock.SynchronizeOppositeEndPoint (_endPointMock));
    }

    [Test]
    public void CreateDeleteCommand_NonVirtualOpposite ()
    {
      var virtualDefinition = RelationEndPointObjectMother.GetEndPointDefinition (typeof (Order), "OrderTicket");

      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);
      _endPointMock.Stub (stub => stub.Definition).Return (virtualDefinition);
      _endPointMock.Replay();

      var command = (RelationEndPointModificationCommand) _state.CreateDeleteCommand (_endPointMock, _fakeNullSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (GetOppositeObjectNullSetter ((ObjectEndPointDeleteCommand) command), Is.SameAs (_fakeNullSetter));
    }

    [Test]
    public void CreateDeleteCommand_VirtualOpposite ()
    {
      var realDefinition = RelationEndPointObjectMother.GetEndPointDefinition (typeof (OrderTicket), "Order");

      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);
      _endPointMock.Stub (stub => stub.Definition).Return (realDefinition);
      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Order1);
      _endPointMock.Replay ();

      var oldOppositeEndPointStub = MockRepository.GenerateStrictMock<IVirtualEndPoint>();
      var newOppositeEndPointStub = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();

      var oldOppositeEndPointID = RelationEndPointID.CreateOpposite (realDefinition, DomainObjectIDs.Order1);
      var newOppositeEndPointID = RelationEndPointID.CreateOpposite (realDefinition, null);

      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (oldOppositeEndPointID))
          .Return (oldOppositeEndPointStub);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (newOppositeEndPointID))
          .Return (newOppositeEndPointStub);

      var command = _state.CreateDeleteCommand (_endPointMock, _fakeNullSetter);

      Assert.That (command, Is.TypeOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var decorator = (RealObjectEndPointRegistrationCommandDecorator) command;
      Assert.That (decorator.RealObjectEndPoint, Is.SameAs (_endPointMock));
      Assert.That (decorator.OldRelatedEndPoint, Is.SameAs (oldOppositeEndPointStub));
      Assert.That (decorator.NewRelatedEndPoint, Is.SameAs (newOppositeEndPointStub));

      Assert.That (decorator.DecoratedCommand, Is.TypeOf (typeof (ObjectEndPointDeleteCommand)));
      var decoratedCommand = (ObjectEndPointDeleteCommand) decorator.DecoratedCommand;
      Assert.That (decoratedCommand.DomainObject, Is.SameAs (_order));
      Assert.That (decoratedCommand.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (GetOppositeObjectNullSetter (decoratedCommand), Is.SameAs (_fakeNullSetter));
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (relatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject ()).Return (relatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, relatedObject, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.SameAs (relatedObject));
      Assert.That (command.NewRelatedObject, Is.SameAs (relatedObject));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (null);
      _endPointMock.Stub (stub => stub.GetOppositeObject ()).Return (null);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, null, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (ObjectEndPointSetSameCommand)));
      Assert.That (command.DomainObject, Is.SameAs (_order));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.OldRelatedObject, Is.Null);
      Assert.That (command.NewRelatedObject, Is.Null);
    }

    [Test]
    public void CreateSetCommand_Unidirectional ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Client> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Client> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_locationClientEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_location);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject ()).Return (oldRelatedObject);

      var command = (RelationEndPointModificationCommand) _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command.GetType (), Is.EqualTo (typeof (ObjectEndPointSetUnidirectionalCommand)));
      Assert.That (command.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (command.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (command.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (GetOppositeObjectIDSetter ((ObjectEndPointSetCommand) command), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject ()).Return (oldRelatedObject);

      var oldOppositeEndPointStub = MockRepository.GenerateStub<IVirtualEndPoint> ();
      var newOppositeEndPointStub = MockRepository.GenerateStub<IVirtualEndPoint> ();

      var oldOppositeEndPointID = RelationEndPointID.CreateOpposite (_orderOrderTicketEndPointDefinition, oldRelatedObject.ID);
      var newOppositeEndPointID = RelationEndPointID.CreateOpposite (_orderOrderTicketEndPointDefinition, newRelatedObject.ID);

      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (oldOppositeEndPointID))
          .Return (oldOppositeEndPointStub);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (newOppositeEndPointID))
          .Return (newOppositeEndPointStub);

      var command = _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var decorator = (RealObjectEndPointRegistrationCommandDecorator) command;
      Assert.That (decorator.RealObjectEndPoint, Is.SameAs (_endPointMock));
      Assert.That (decorator.OldRelatedEndPoint, Is.SameAs (oldOppositeEndPointStub));
      Assert.That (decorator.NewRelatedEndPoint, Is.SameAs (newOppositeEndPointStub));

      Assert.That (decorator.DecoratedCommand, Is.TypeOf (typeof (ObjectEndPointSetOneOneCommand)));
      var decoratedCommand = (ObjectEndPointSetOneOneCommand) decorator.DecoratedCommand;
      Assert.That (decoratedCommand.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (decoratedCommand.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (decoratedCommand.OldRelatedObject, Is.SameAs (oldRelatedObject));

      Assert.That (GetOppositeObjectIDSetter (decoratedCommand), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneMany ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Customer> ();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Customer> ();

      _endPointMock.Stub (stub => stub.Definition).Return (_orderCustomerEndPointDefinition);
      _endPointMock.Stub (stub => stub.GetDomainObject ()).Return (_order);
      _endPointMock.Stub (stub => stub.IsNull).Return (false);

      _endPointMock.Stub (stub => stub.OppositeObjectID).Return (oldRelatedObject.ID);
      _endPointMock.Stub (stub => stub.GetOppositeObject ()).Return (oldRelatedObject);

      var oldOppositeEndPointStub = MockRepository.GenerateStub<IVirtualEndPoint> ();
      var newOppositeEndPointStub = MockRepository.GenerateStub<IVirtualEndPoint> ();

      var oldOppositeEndPointID = RelationEndPointID.CreateOpposite (_orderCustomerEndPointDefinition, oldRelatedObject.ID);
      var newOppositeEndPointID = RelationEndPointID.CreateOpposite (_orderCustomerEndPointDefinition, newRelatedObject.ID);

      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (oldOppositeEndPointID))
          .Return (oldOppositeEndPointStub);
      _endPointProviderStub
          .Stub (stub => stub.GetRelationEndPointWithLazyLoad (newOppositeEndPointID))
          .Return (newOppositeEndPointStub);

      var command = _state.CreateSetCommand (_endPointMock, newRelatedObject, _fakeSetter);

      Assert.That (command, Is.TypeOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      var decorator = (RealObjectEndPointRegistrationCommandDecorator) command;
      Assert.That (decorator.RealObjectEndPoint, Is.SameAs (_endPointMock));
      Assert.That (decorator.OldRelatedEndPoint, Is.SameAs (oldOppositeEndPointStub));
      Assert.That (decorator.NewRelatedEndPoint, Is.SameAs (newOppositeEndPointStub));

      Assert.That (decorator.DecoratedCommand, Is.TypeOf (typeof (ObjectEndPointSetOneManyCommand)));
      var decoratedCommand = (ObjectEndPointSetOneManyCommand) decorator.DecoratedCommand;

      Assert.That (decoratedCommand, Is.TypeOf (typeof (ObjectEndPointSetOneManyCommand)));
      Assert.That (decoratedCommand.ModifiedEndPoint, Is.SameAs (_endPointMock));
      Assert.That (decoratedCommand.NewRelatedObject, Is.SameAs (newRelatedObject));
      Assert.That (decoratedCommand.OldRelatedObject, Is.SameAs (oldRelatedObject));
      Assert.That (decoratedCommand.EndPointProvider, Is.SameAs (_endPointProviderStub));
      Assert.That (GetOppositeObjectIDSetter (decoratedCommand), Is.SameAs (_fakeSetter));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new SynchronizedRealObjectEndPointSyncState (
          new SerializableRelationEndPointProviderFake(), new SerializableClientTransactionEventSinkFake());

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.EndPointProvider, Is.Not.Null);
      Assert.That (result.TransactionEventSink, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type classType, string shortPropertyName)
    {
      return Configuration.GetTypeDefinition (classType).GetRelationEndPointDefinition (classType.FullName + "." + shortPropertyName);
    }

    private Action<DomainObject> GetOppositeObjectIDSetter (ObjectEndPointSetCommand command)
    {
      return (Action<DomainObject>) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectSetter");
    }

    private Action GetOppositeObjectNullSetter (ObjectEndPointDeleteCommand command)
    {
      return (Action) PrivateInvoke.GetNonPublicField (command, "_oppositeObjectNullSetter");
    }

  }
}