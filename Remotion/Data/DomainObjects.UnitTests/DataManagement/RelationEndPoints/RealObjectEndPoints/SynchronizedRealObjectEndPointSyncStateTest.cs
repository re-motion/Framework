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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  [TestFixture]
  public class SynchronizedRealObjectEndPointSyncStateTest : StandardMappingTest
  {
    private Mock<IRealObjectEndPoint> _endPointMock;
    private Mock<IRelationEndPointProvider> _endPointProviderStub;
    private Mock<IClientTransactionEventSink> _transactionEventSinkStub;

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
      base.SetUp();

      _endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      _endPointProviderStub = new Mock<IRelationEndPointProvider>();
      _transactionEventSinkStub = new Mock<IClientTransactionEventSink>();

      _state = new SynchronizedRealObjectEndPointSyncState(_endPointProviderStub.Object, _transactionEventSinkStub.Object);

      _order = DomainObjectMother.CreateFakeObject<Order>();
      _location = DomainObjectMother.CreateFakeObject<Location>();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition(typeof(Order), "OrderTicket");
      _locationClientEndPointDefinition = GetRelationEndPointDefinition(typeof(Location), "Client");
      _orderCustomerEndPointDefinition = GetRelationEndPointDefinition(typeof(Order), "Customer");

      _fakeSetter = domainObject => { };
      _fakeNullSetter = () => { };
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That(_state.IsSynchronized(_endPointMock.Object), Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);

      _state.Synchronize(_endPointMock.Object, oppositeEndPointMock.Object);

      oppositeEndPointMock.Verify(mock=>mock.SynchronizeOppositeEndPoint(_endPointMock.Object), Times.Never());
    }

    [Test]
    public void CreateDeleteCommand_NonVirtualOpposite ()
    {
      var virtualDefinition = RelationEndPointObjectMother.GetEndPointDefinition(typeof(Order), "OrderTicket");

      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_order);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);
      _endPointMock.Setup(stub => stub.Definition).Returns(virtualDefinition);

      var command = (RelationEndPointModificationCommand)_state.CreateDeleteCommand(_endPointMock.Object, _fakeNullSetter);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointDeleteCommand)));
      Assert.That(command.DomainObject, Is.SameAs(_order));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(GetOppositeObjectNullSetter((ObjectEndPointDeleteCommand)command), Is.SameAs(_fakeNullSetter));
    }

    [Test]
    public void CreateDeleteCommand_VirtualOpposite ()
    {
      var realDefinition = RelationEndPointObjectMother.GetEndPointDefinition(typeof(OrderTicket), "Order");

      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_order);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);
      _endPointMock.Setup(stub => stub.Definition).Returns(realDefinition);
      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.Order1);

      var oldOppositeEndPointStub = new Mock<IVirtualEndPoint>(MockBehavior.Strict);
      var newOppositeEndPointStub = new Mock<IVirtualEndPoint>(MockBehavior.Strict);

      var oldOppositeEndPointID = RelationEndPointID.CreateOpposite(realDefinition, DomainObjectIDs.Order1);
      var newOppositeEndPointID = RelationEndPointID.CreateOpposite(realDefinition, null);

      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(oldOppositeEndPointID))
          .Returns(oldOppositeEndPointStub.Object);
      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(newOppositeEndPointID))
          .Returns(newOppositeEndPointStub.Object);

      var command = _state.CreateDeleteCommand(_endPointMock.Object, _fakeNullSetter);

      Assert.That(command, Is.TypeOf(typeof(RealObjectEndPointRegistrationCommandDecorator)));
      var decorator = (RealObjectEndPointRegistrationCommandDecorator)command;
      Assert.That(decorator.RealObjectEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(decorator.OldRelatedEndPoint, Is.SameAs(oldOppositeEndPointStub.Object));
      Assert.That(decorator.NewRelatedEndPoint, Is.SameAs(newOppositeEndPointStub.Object));

      Assert.That(decorator.DecoratedCommand, Is.TypeOf(typeof(ObjectEndPointDeleteCommand)));
      var decoratedCommand = (ObjectEndPointDeleteCommand)decorator.DecoratedCommand;
      Assert.That(decoratedCommand.DomainObject, Is.SameAs(_order));
      Assert.That(decoratedCommand.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(GetOppositeObjectNullSetter(decoratedCommand), Is.SameAs(_fakeNullSetter));
    }

    [Test]
    public void CreateSetCommand_Same ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();

      _endPointMock.Setup(stub => stub.Definition).Returns(_orderOrderTicketEndPointDefinition);
      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_order);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);

      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns(relatedObject.ID);
      _endPointMock.Setup(stub => stub.GetOppositeObject()).Returns(relatedObject);

      var command = (RelationEndPointModificationCommand)_state.CreateSetCommand(_endPointMock.Object, relatedObject, _fakeSetter);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointSetSameCommand)));
      Assert.That(command.DomainObject, Is.SameAs(_order));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(command.OldRelatedObject, Is.SameAs(relatedObject));
      Assert.That(command.NewRelatedObject, Is.SameAs(relatedObject));
    }

    [Test]
    public void CreateSetCommand_Same_Null ()
    {
      _endPointMock.Setup(stub => stub.Definition).Returns(_orderOrderTicketEndPointDefinition);
      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_order);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);

      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns((ObjectID)null);
      _endPointMock.Setup(stub => stub.GetOppositeObject()).Returns((DomainObject)null);

      var command = (RelationEndPointModificationCommand)_state.CreateSetCommand(_endPointMock.Object, null, _fakeSetter);

      Assert.That(command, Is.TypeOf(typeof(ObjectEndPointSetSameCommand)));
      Assert.That(command.DomainObject, Is.SameAs(_order));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(command.OldRelatedObject, Is.Null);
      Assert.That(command.NewRelatedObject, Is.Null);
    }

    [Test]
    public void CreateSetCommand_Unidirectional ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Client>();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Client>();

      _endPointMock.Setup(stub => stub.Definition).Returns(_locationClientEndPointDefinition);
      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_location);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);

      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns(oldRelatedObject.ID);
      _endPointMock.Setup(stub => stub.GetOppositeObject()).Returns(oldRelatedObject);

      var command = (RelationEndPointModificationCommand)_state.CreateSetCommand(_endPointMock.Object, newRelatedObject, _fakeSetter);

      Assert.That(command.GetType(), Is.EqualTo(typeof(ObjectEndPointSetUnidirectionalCommand)));
      Assert.That(command.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(command.NewRelatedObject, Is.SameAs(newRelatedObject));
      Assert.That(command.OldRelatedObject, Is.SameAs(oldRelatedObject));
      Assert.That(GetOppositeObjectIDSetter((ObjectEndPointSetCommand)command), Is.SameAs(_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneOne ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();

      _endPointMock.Setup(stub => stub.Definition).Returns(_orderOrderTicketEndPointDefinition);
      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_order);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);

      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns(oldRelatedObject.ID);
      _endPointMock.Setup(stub => stub.GetOppositeObject()).Returns(oldRelatedObject);

      var oldOppositeEndPointStub = new Mock<IVirtualEndPoint>();
      var newOppositeEndPointStub = new Mock<IVirtualEndPoint>();

      var oldOppositeEndPointID = RelationEndPointID.CreateOpposite(_orderOrderTicketEndPointDefinition, oldRelatedObject.ID);
      var newOppositeEndPointID = RelationEndPointID.CreateOpposite(_orderOrderTicketEndPointDefinition, newRelatedObject.ID);

      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(oldOppositeEndPointID))
          .Returns(oldOppositeEndPointStub.Object);
      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(newOppositeEndPointID))
          .Returns(newOppositeEndPointStub.Object);

      var command = _state.CreateSetCommand(_endPointMock.Object, newRelatedObject, _fakeSetter);

      Assert.That(command, Is.TypeOf(typeof(RealObjectEndPointRegistrationCommandDecorator)));
      var decorator = (RealObjectEndPointRegistrationCommandDecorator)command;
      Assert.That(decorator.RealObjectEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(decorator.OldRelatedEndPoint, Is.SameAs(oldOppositeEndPointStub.Object));
      Assert.That(decorator.NewRelatedEndPoint, Is.SameAs(newOppositeEndPointStub.Object));

      Assert.That(decorator.DecoratedCommand, Is.TypeOf(typeof(ObjectEndPointSetOneOneCommand)));
      var decoratedCommand = (ObjectEndPointSetOneOneCommand)decorator.DecoratedCommand;
      Assert.That(decoratedCommand.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(decoratedCommand.NewRelatedObject, Is.SameAs(newRelatedObject));
      Assert.That(decoratedCommand.OldRelatedObject, Is.SameAs(oldRelatedObject));

      Assert.That(GetOppositeObjectIDSetter(decoratedCommand), Is.SameAs(_fakeSetter));
    }

    [Test]
    public void CreateSetCommand_OneMany ()
    {
      var oldRelatedObject = DomainObjectMother.CreateFakeObject<Customer>();
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Customer>();

      _endPointMock.Setup(stub => stub.Definition).Returns(_orderCustomerEndPointDefinition);
      _endPointMock.Setup(stub => stub.GetDomainObject()).Returns(_order);
      _endPointMock.Setup(stub => stub.IsNull).Returns(false);

      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns(oldRelatedObject.ID);
      _endPointMock.Setup(stub => stub.GetOppositeObject()).Returns(oldRelatedObject);

      var oldOppositeEndPointStub = new Mock<IVirtualEndPoint>();
      var newOppositeEndPointStub = new Mock<IVirtualEndPoint>();

      var oldOppositeEndPointID = RelationEndPointID.CreateOpposite(_orderCustomerEndPointDefinition, oldRelatedObject.ID);
      var newOppositeEndPointID = RelationEndPointID.CreateOpposite(_orderCustomerEndPointDefinition, newRelatedObject.ID);

      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(oldOppositeEndPointID))
          .Returns(oldOppositeEndPointStub.Object);
      _endPointProviderStub
          .Setup(stub => stub.GetRelationEndPointWithLazyLoad(newOppositeEndPointID))
          .Returns(newOppositeEndPointStub.Object);

      var command = _state.CreateSetCommand(_endPointMock.Object, newRelatedObject, _fakeSetter);

      Assert.That(command, Is.TypeOf(typeof(RealObjectEndPointRegistrationCommandDecorator)));
      var decorator = (RealObjectEndPointRegistrationCommandDecorator)command;
      Assert.That(decorator.RealObjectEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(decorator.OldRelatedEndPoint, Is.SameAs(oldOppositeEndPointStub.Object));
      Assert.That(decorator.NewRelatedEndPoint, Is.SameAs(newOppositeEndPointStub.Object));

      Assert.That(decorator.DecoratedCommand, Is.TypeOf(typeof(ObjectEndPointSetOneManyCommand)));
      var decoratedCommand = (ObjectEndPointSetOneManyCommand)decorator.DecoratedCommand;

      Assert.That(decoratedCommand, Is.TypeOf(typeof(ObjectEndPointSetOneManyCommand)));
      Assert.That(decoratedCommand.ModifiedEndPoint, Is.SameAs(_endPointMock.Object));
      Assert.That(decoratedCommand.NewRelatedObject, Is.SameAs(newRelatedObject));
      Assert.That(decoratedCommand.OldRelatedObject, Is.SameAs(oldRelatedObject));
      Assert.That(decoratedCommand.EndPointProvider, Is.SameAs(_endPointProviderStub.Object));
      Assert.That(GetOppositeObjectIDSetter(decoratedCommand), Is.SameAs(_fakeSetter));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new SynchronizedRealObjectEndPointSyncState(
          new SerializableRelationEndPointProviderFake(), new SerializableClientTransactionEventSinkFake());

      var result = FlattenedSerializer.SerializeAndDeserialize(state);

      Assert.That(result, Is.Not.Null);
      Assert.That(result.EndPointProvider, Is.Not.Null);
      Assert.That(result.TransactionEventSink, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type type, string shortPropertyName)
    {
      return Configuration.GetTypeDefinition(type).GetRelationEndPointDefinition(type.FullName + "." + shortPropertyName);
    }

    private Action<DomainObject> GetOppositeObjectIDSetter (ObjectEndPointSetCommand command)
    {
      return (Action<DomainObject>)PrivateInvoke.GetNonPublicField(command, "_oppositeObjectSetter");
    }

    private Action GetOppositeObjectNullSetter (ObjectEndPointDeleteCommand command)
    {
      return (Action)PrivateInvoke.GetNonPublicField(command, "_oppositeObjectNullSetter");
    }

  }
}
