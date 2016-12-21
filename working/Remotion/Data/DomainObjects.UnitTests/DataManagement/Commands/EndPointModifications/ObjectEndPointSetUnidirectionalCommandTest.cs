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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class ObjectEndPointSetUnidirectionalCommandTest : ObjectEndPointSetCommandTestBase
  {
    private Client _domainObject;
    private Client _oldRelatedObject;
    private Client _newRelatedObject;

    private RelationEndPointID _endPointID;
    private ObjectEndPoint _endPoint;
    
    private ObjectEndPointSetCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _domainObject = DomainObjectIDs.Client3.GetObject<Client> ();
      _oldRelatedObject = DomainObjectIDs.Client1.GetObject<Client> ();
      _newRelatedObject = DomainObjectIDs.Client2.GetObject<Client> ();

      _endPointID = RelationEndPointID.Resolve (_domainObject, c => c.ParentClient);
      _endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);

      _command = new ObjectEndPointSetUnidirectionalCommand (_endPoint, _newRelatedObject, OppositeObjectSetter, TransactionEventSinkWithMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (_endPoint));
      Assert.That (_command.OldRelatedObject, Is.SameAs (_oldRelatedObject));
      Assert.That (_command.NewRelatedObject, Is.SameAs (_newRelatedObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (TestableClientTransaction, _endPointID.Definition);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, _newRelatedObject, OppositeObjectSetter, TransactionEventSinkWithMock);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneMany ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderItem))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderItem).FullName + ".Order");
      var orderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var id = RelationEndPointID.Create(orderItem.ID, definition);

      var endPoint = (IObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (id);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, Order.NewObject (), mi => { }, TransactionEventSinkWithMock);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "EndPoint 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order' "
        + "is from a bidirectional relation - use a ObjectEndPointSetOneOneCommand or ObjectEndPointSetOneManyCommand instead.\r\nParameter name: modifiedEndPoint")]
    public void Initialization_Bidirectional_OneOne ()
    {
      var definition = MappingConfiguration.Current.GetTypeDefinition (typeof (OrderTicket))
          .GetMandatoryRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ().ID, definition);
      var endPoint = (IObjectEndPoint) TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, Order.NewObject (), mi => { }, TransactionEventSinkWithMock);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "New related object for EndPoint "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient' is the same as its old value - use a ObjectEndPointSetSameCommand "
        + "instead.\r\nParameter name: newRelatedObject")]
    public void Initialization_Same ()
    {
      var endPoint = RelationEndPointObjectMother.CreateObjectEndPoint (_endPointID, _oldRelatedObject.ID);
      new ObjectEndPointSetUnidirectionalCommand (endPoint, _oldRelatedObject, mi => { }, TransactionEventSinkWithMock);
    }

    [Test]
    public void Perform_InvokesPerformRelationChange ()
    {
      Assert.That (OppositeObjectSetterCalled, Is.False);

      _command.Perform ();

      Assert.That (OppositeObjectSetterCalled, Is.True);
      Assert.That (OppositeObjectSetterObject, Is.SameAs (_newRelatedObject));
    }

    [Test]
    public void Perform_TouchesEndPoint ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _command.Perform();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public virtual void Begin ()
    {
      TransactionEventSinkWithMock.Expect (mock => mock.RaiseRelationChangingEvent (
          _endPoint.GetDomainObject(),
          _endPoint.Definition,
          _oldRelatedObject,
          _newRelatedObject));
      TransactionEventSinkWithMock.Replay();

      _command.Begin();

      TransactionEventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public virtual void End ()
    {
      TransactionEventSinkWithMock.Expect (mock => mock.RaiseRelationChangedEvent (
          _endPoint.GetDomainObject(),
          _endPoint.Definition,
          _oldRelatedObject,
          _newRelatedObject));
      TransactionEventSinkWithMock.Replay();

      _command.End();

      TransactionEventSinkWithMock.VerifyAllExpectations();
    }
    
    [Test]
    public void ExpandToAllRelatedObjects_SetDifferent_Unidirectional ()
    {
      var bidirectionalModification = _command.ExpandToAllRelatedObjects ();
      Assert.That (bidirectionalModification.GetNestedCommands (), Is.EqualTo (new[] { _command }));
    }
  }
}
