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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetOneOneCommandTest : ObjectEndPointSetCommandTestBase
  {
    private Order _domainObject;
    private OrderTicket _oldRelatedObject;
    private OrderTicket _newRelatedObject;

    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;

    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _domainObject = DomainObjectIDs.Order1.GetObject<Order>();
      _oldRelatedObject = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>();
      _newRelatedObject = DomainObjectIDs.OrderTicket2.GetObject<OrderTicket>();

      _endPointID = RelationEndPointID.Resolve(_domainObject, o => o.OrderTicket);
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint(_endPointID, _oldRelatedObject.ID);

      _command = new ObjectEndPointSetOneOneCommand(_endPoint, _newRelatedObject, OppositeObjectSetter, TransactionEventSinkWithMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_command.ModifiedEndPoint, Is.SameAs(_endPoint));
      Assert.That(_command.OldRelatedObject, Is.SameAs(_oldRelatedObject));
      Assert.That(_command.NewRelatedObject, Is.SameAs(_newRelatedObject));
    }

    [Test]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint(TestableClientTransaction, _endPointID.Definition);
      Assert.That(
          () => new ObjectEndPointSetOneOneCommand(endPoint, _newRelatedObject, OppositeObjectSetter, TransactionEventSinkWithMock.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Modified end point is null, a NullEndPointModificationCommand is needed.",
                  "modifiedEndPoint"));
    }

    [Test]
    public void Initialization_Unidirectional ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition(typeof(Client))
          .GetMandatoryRelationEndPointDefinition(typeof(Client).FullName + ".ParentClient");
      var client = DomainObjectIDs.Client1.GetObject<Client>();
      var id = RelationEndPointID.Create(client.ID, definition);
      var endPoint = (IObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(id);
      Assert.That(
          () => new ObjectEndPointSetOneOneCommand(endPoint, Client.NewObject(), mi => { }, TransactionEventSinkWithMock.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "EndPoint 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient' "
                  + "is from a unidirectional relation - use a ObjectEndPointSetUnidirectionalCommand instead.", "modifiedEndPoint"));
    }

    [Test]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition(typeof(OrderItem))
          .GetMandatoryRelationEndPointDefinition(typeof(OrderItem).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.OrderItem1.GetObject<OrderItem>().ID, definition);
      var endPoint =
          (IObjectEndPoint)TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
      Assert.That(
          () => new ObjectEndPointSetOneOneCommand(endPoint, Order.NewObject(), mi => { }, TransactionEventSinkWithMock.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "EndPoint 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' "
                  + "is from a 1:n relation - use a ObjectEndPointSetOneManyCommand instead.", "modifiedEndPoint"));
    }

    [Test]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint(_endPointID, _oldRelatedObject.ID);
      Assert.That(
          () => new ObjectEndPointSetOneOneCommand(endPoint, _oldRelatedObject, mi => { }, TransactionEventSinkWithMock.Object),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "New related object for EndPoint "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' is the same as its old value - use a ObjectEndPointSetSameCommand "
                  + "instead.", "newRelatedObject"));
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That(OppositeObjectSetterCalled, Is.False);

      _command.Perform();

      Assert.That(OppositeObjectSetterCalled, Is.True);
      Assert.That(OppositeObjectSetterObject, Is.SameAs(_newRelatedObject));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      Assert.That(_endPoint.HasBeenTouched, Is.False);

      _command.Perform();

      Assert.That(_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void Begin ()
    {
      TransactionEventSinkWithMock
          .Setup(mock => mock.RaiseRelationChangingEvent(_endPoint.GetDomainObject(), _endPoint.Definition, _oldRelatedObject, _newRelatedObject))
          .Verifiable();

      _command.Begin();

      TransactionEventSinkWithMock.Verify();
    }

    [Test]
    public virtual void End ()
    {
      TransactionEventSinkWithMock
          .Setup(mock => mock.RaiseRelationChangedEvent(_endPoint.GetDomainObject(), _endPoint.Definition, _oldRelatedObject, _newRelatedObject))
          .Verifiable();

      _command.End();

      TransactionEventSinkWithMock.Verify();
    }

    [Test]
    public void ExpandToAllRelatedObjects_SetDifferent_BidirectionalOneOne ()
    {
      // order.OrderTicket = newOrderTicket;

      var bidirectionalModification = _command.ExpandToAllRelatedObjects();

      var steps = bidirectionalModification.GetNestedCommands();
      Assert.That(steps.Count, Is.EqualTo(4));

      // order.OrderTicket = newOrderTicket;
      Assert.That(steps[0], Is.SameAs(_command));

      // oldOrderTicket.Order = null;

      var orderOfOldOrderTicketEndPointID = RelationEndPointID.Resolve(_oldRelatedObject, ot => ot.Order);
      var orderOfOldOrderTicketEndPoint =
          TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(orderOfOldOrderTicketEndPointID);

      Assert.That(steps[1], Is.InstanceOf(typeof(RealObjectEndPointRegistrationCommandDecorator)));
      var setOrderOfOldOrderTicketCommand = (ObjectEndPointSetCommand)((RealObjectEndPointRegistrationCommandDecorator)steps[1]).DecoratedCommand;
      Assert.That(setOrderOfOldOrderTicketCommand.ModifiedEndPoint, Is.SameAs(orderOfOldOrderTicketEndPoint));
      Assert.That(setOrderOfOldOrderTicketCommand.OldRelatedObject, Is.SameAs(_domainObject));
      Assert.That(setOrderOfOldOrderTicketCommand.NewRelatedObject, Is.Null);

      // newOrderTicket.Order = order;

      var orderOfNewOrderTicketEndPointID = RelationEndPointID.Resolve(_newRelatedObject, ot => ot.Order);
      var orderOfNewOrderTicketEndPoint =
          TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(orderOfNewOrderTicketEndPointID);

      Assert.That(steps[2], Is.InstanceOf(typeof(RealObjectEndPointRegistrationCommandDecorator)));
      var setOrderOfNewOrderTicketCommand = (ObjectEndPointSetCommand)((RealObjectEndPointRegistrationCommandDecorator)steps[2]).DecoratedCommand;
      Assert.That(setOrderOfNewOrderTicketCommand.ModifiedEndPoint, Is.SameAs(orderOfNewOrderTicketEndPoint));
      Assert.That(setOrderOfNewOrderTicketCommand.OldRelatedObject, Is.SameAs(_newRelatedObject.Order));
      Assert.That(setOrderOfNewOrderTicketCommand.NewRelatedObject, Is.SameAs(_domainObject));

      // oldOrderOfNewOrderTicket.OrderTicket = null

      var orderTicketOfOldOrderOfNewOrderTicketEndPointID = RelationEndPointID.Create(_newRelatedObject.Order.ID, _endPoint.Definition);
      var orderTicketOfOldOrderOfNewOrderTicketEndPoint =
          TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad(orderTicketOfOldOrderOfNewOrderTicketEndPointID);

      Assert.That(steps[3], Is.InstanceOf(typeof(VirtualEndPointStateUpdatedRaisingCommandDecorator)));
      var setOrderTicketOfOldOrderOfNewOrderTicketCommand = ((ObjectEndPointSetCommand)((VirtualEndPointStateUpdatedRaisingCommandDecorator)steps[3]).DecoratedCommand);
      Assert.That(
          setOrderTicketOfOldOrderOfNewOrderTicketCommand.ModifiedEndPoint,
          Is.SameAs(((StateUpdateRaisingVirtualObjectEndPointDecorator)orderTicketOfOldOrderOfNewOrderTicketEndPoint).InnerEndPoint));
      Assert.That(setOrderTicketOfOldOrderOfNewOrderTicketCommand.OldRelatedObject, Is.SameAs(_newRelatedObject));
      Assert.That(setOrderTicketOfOldOrderOfNewOrderTicketCommand.NewRelatedObject, Is.SameAs(null));
    }
  }
}
