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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private TestableObjectEndPoint _endPointPartialMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      _endPointPartialMock = MockRepository.GeneratePartialMock<TestableObjectEndPoint> (TestableClientTransaction, _endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "End point ID must refer to an end point with cardinality 'One'.\r\nParameter name: id")]
    public void Initialize_WithNonOneEndPointID_Throws ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      new TestableObjectEndPoint (
          TestableClientTransaction,
          endPointID);
    }

    [Test]
    public void SetDataFromSubTransaction_CallsSubclass_WhenIDsDiffer ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket2);
      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Expect (mock => mock.CallSetOppositeObjectDataFromSubTransaction (source));
      _endPointPartialMock.Stub (stub => stub.Touch ());
      _endPointPartialMock.Stub (stub => stub.HasChanged).Return (false);
      _endPointPartialMock.Replay ();

      _endPointPartialMock.SetDataFromSubTransaction (source);

      _endPointPartialMock.VerifyAllExpectations();
    }

    [Test]
    public void SetDataFromSubTransaction_CallsSubclass_WhenIDsEqual ()
    {
      var sourceID = RelationEndPointID.Create (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      var source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Expect (mock => mock.CallSetOppositeObjectDataFromSubTransaction (source));
      _endPointPartialMock.Stub (stub => stub.Touch ());
      _endPointPartialMock.Stub (stub => stub.HasChanged).Return (false);
      _endPointPartialMock.Replay ();
      
      _endPointPartialMock.SetDataFromSubTransaction (source);

      _endPointPartialMock.VerifyAllExpectations();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_IfSourceWasTouched ()
    {
      var sourceID = RelationEndPointID.Create (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket2);
      source.Touch();
      Assert.That (source.HasBeenTouched, Is.True);

      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Stub (stub => stub.HasBeenTouched).Return (false);
      _endPointPartialMock.Stub (stub => stub.CallSetOppositeObjectDataFromSubTransaction (source));
      _endPointPartialMock.Expect (mock => mock.Touch ());
      _endPointPartialMock.Replay ();

      _endPointPartialMock.SetDataFromSubTransaction (source);

      _endPointPartialMock.VerifyAllExpectations ();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_IfDataWasChanged ()
    {
      var sourceID = RelationEndPointID.Create (DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket2);
      Assert.That (source.HasBeenTouched, Is.False);

      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Stub (stub => stub.HasBeenTouched).Return (false);
      _endPointPartialMock.Stub (stub => stub.HasChanged).Return (true);
      _endPointPartialMock.Stub (stub => stub.CallSetOppositeObjectDataFromSubTransaction (source));
      _endPointPartialMock.Expect (mock => mock.Touch ());
      _endPointPartialMock.Replay ();

      _endPointPartialMock.SetDataFromSubTransaction (source);

      _endPointPartialMock.VerifyAllExpectations();
    }

    [Test]
    public void SetDataFromSubTransaction_NoTouching_IfNothingHappened ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint (sourceID, DomainObjectIDs.OrderTicket1);
      Assert.That (source.HasBeenTouched, Is.False);

      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Stub (stub => stub.HasBeenTouched).Return (false);
      _endPointPartialMock.Stub (stub => stub.HasChanged).Return (false);
      _endPointPartialMock.Stub (stub => stub.CallSetOppositeObjectDataFromSubTransaction (source));
      _endPointPartialMock.Replay ();

      _endPointPartialMock.SetDataFromSubTransaction (source);

      _endPointPartialMock.AssertWasNotCalled (mock => mock.Touch ());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot set this end point's value from "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'; "
        + "the end points do not have the same end point definition.\r\nParameter name: source")]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      ObjectEndPoint source = RelationEndPointObjectMother.CreateRealObjectEndPoint (otherID);

      _endPointPartialMock.SetDataFromSubTransaction (source);
    }
    
    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeSetCommand = MockRepository.GenerateStub<IDataManagementCommand>();
      _endPointPartialMock.Expect (mock => mock.CreateSetCommand (null)).Return (fakeSetCommand);
      _endPointPartialMock.Stub (mock => mock.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Replay();
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> (_endPointPartialMock.OppositeObjectID);
      
      var result = _endPointPartialMock.CreateRemoveCommand (relatedObject);

      _endPointPartialMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeSetCommand));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "Cannot remove object 'OrderTicket|6768db2b-9c66-4e2f-bba2-89c56718ff2b|System.Guid' from object end point "
        + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' - it currently holds object "
        + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'.")]
    public void CreateRemoveCommand_InvalidID ()
    {
      _endPointPartialMock.Stub (mock => mock.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);

      var orderTicket = DomainObjectIDs.OrderTicket4.GetObject<OrderTicket> ();
      _endPointPartialMock.CreateRemoveCommand (orderTicket);
    }

    [Test]
    public void GetOppositeRelationEndPointID_NullEndPoint ()
    {
      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (null);

      var oppositeEndPointID = _endPointPartialMock.GetOppositeRelationEndPointID ();

      var expectedID = RelationEndPointID.Create (null, _endPointPartialMock.Definition.GetOppositeEndPointDefinition ());
      Assert.That (oppositeEndPointID, Is.EqualTo (expectedID));
    }

    [Test]
    public void GetOppositeRelationEndPointID_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (endPointID);
      Assert.That (endPoint.Definition.GetOppositeEndPointDefinition ().IsAnonymous, Is.True);

      var oppositeEndPointID = endPoint.GetOppositeRelationEndPointID ();

      Assert.That (oppositeEndPointID, Is.Null);
    }

    [Test]
    public void GetOppositeRelationEndPointID_NonNullEndPoint ()
    {
      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);

      var oppositeEndPointID = _endPointPartialMock.GetOppositeRelationEndPointID ();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (oppositeEndPointID, Is.EqualTo (expectedID));
    }

    [Test]
    public void GetOppositeRelationEndPointIDs_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (endPointID);

      Assert.That (endPoint.Definition.GetOppositeEndPointDefinition ().IsAnonymous, Is.True);

      var oppositeEndPointIDs = endPoint.GetOppositeRelationEndPointIDs ().ToArray ();

      Assert.That (oppositeEndPointIDs, Is.Empty);
    }

    [Test]
    public void GetOppositeRelationEndPointIDs_BidirectionalEndPoint ()
    {
      _endPointPartialMock.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);
      var oppositeEndPointIDs = _endPointPartialMock.GetOppositeRelationEndPointIDs ().ToArray ();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      Assert.That (oppositeEndPointIDs, Is.EqualTo (new[] { expectedID }));
    }
    
  }
}
