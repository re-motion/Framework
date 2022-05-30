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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.NUnit;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class ObjectEndPointTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _endPointID;
    private Mock<TestableObjectEndPoint> _endPointPartialMock;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket");
      _endPointPartialMock = new Mock<TestableObjectEndPoint>(TestableClientTransaction, _endPointID) { CallBase = true };
    }

    [Test]
    public void Initialize_WithNonOneEndPointID_Throws ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      Assert.That(
          () => new TestableObjectEndPoint(
          TestableClientTransaction,
          endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "End point ID must refer to an end point with cardinality 'One'.", "id"));
    }

    [Test]
    public void SetDataFromSubTransaction_CallsSubclass_WhenIDsDiffer ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      var source = RelationEndPointObjectMother.CreateObjectEndPoint(sourceID, DomainObjectIDs.OrderTicket2);
      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Object.SetupSetOppositeObjectDataFromSubTransaction(_endPointPartialMock, source).Verifiable();
      _endPointPartialMock.Setup(stub => stub.Touch());
      _endPointPartialMock.Setup(stub => stub.HasChanged).Returns(false);

      _endPointPartialMock.Object.SetDataFromSubTransaction(source);

      _endPointPartialMock.Verify();
    }

    [Test]
    public void SetDataFromSubTransaction_CallsSubclass_WhenIDsEqual ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      var source = RelationEndPointObjectMother.CreateObjectEndPoint(sourceID, DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Object.SetupSetOppositeObjectDataFromSubTransaction(_endPointPartialMock, source).Verifiable();
      _endPointPartialMock.Setup(stub => stub.Touch());
      _endPointPartialMock.Setup(stub => stub.HasChanged).Returns(false);

      _endPointPartialMock.Object.SetDataFromSubTransaction(source);

      _endPointPartialMock.Verify();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_IfSourceWasTouched ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint(sourceID, DomainObjectIDs.OrderTicket2);
      source.Touch();
      Assert.That(source.HasBeenTouched, Is.True);

      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Setup(stub => stub.HasBeenTouched).Returns(false);
      _endPointPartialMock.Object.SetupSetOppositeObjectDataFromSubTransaction(_endPointPartialMock, source);
      _endPointPartialMock.Setup(mock => mock.Touch()).Verifiable();

      _endPointPartialMock.Object.SetDataFromSubTransaction(source);

      _endPointPartialMock.Verify();
    }

    [Test]
    public void SetDataFromSubTransaction_TouchesEndPoint_IfDataWasChanged ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint(sourceID, DomainObjectIDs.OrderTicket2);
      Assert.That(source.HasBeenTouched, Is.False);

      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Setup(stub => stub.HasBeenTouched).Returns(false);
      _endPointPartialMock.Setup(stub => stub.HasChanged).Returns(true);
      _endPointPartialMock.Object.SetupSetOppositeObjectDataFromSubTransaction(_endPointPartialMock, source);
      _endPointPartialMock.Setup(mock => mock.Touch()).Verifiable();

      _endPointPartialMock.Object.SetDataFromSubTransaction(source);

      _endPointPartialMock.Verify();
    }

    [Test]
    public void SetDataFromSubTransaction_NoTouching_IfNothingHappened ()
    {
      var sourceID = RelationEndPointID.Create(DomainObjectIDs.OrderItem2, _endPointID.Definition);
      ObjectEndPoint source = RelationEndPointObjectMother.CreateObjectEndPoint(sourceID, DomainObjectIDs.OrderTicket1);
      Assert.That(source.HasBeenTouched, Is.False);

      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      _endPointPartialMock.Setup(stub => stub.HasBeenTouched).Returns(false);
      _endPointPartialMock.Setup(stub => stub.HasChanged).Returns(false);
      _endPointPartialMock.Object.SetupSetOppositeObjectDataFromSubTransaction(_endPointPartialMock, source);

      _endPointPartialMock.Object.SetDataFromSubTransaction(source);

      _endPointPartialMock.Verify(mock => mock.Touch(), Times.Never());
    }

    [Test]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var otherID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      ObjectEndPoint source = RelationEndPointObjectMother.CreateRealObjectEndPoint(otherID);
      Assert.That(
          () => _endPointPartialMock.Object.SetDataFromSubTransaction(source),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Cannot set this end point's value from "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'; "
                  + "the end points do not have the same end point definition.", "source"));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var fakeSetCommand = new Mock<IDataManagementCommand>();
      _endPointPartialMock.Setup(mock => mock.CreateSetCommand(null)).Returns(fakeSetCommand.Object).Verifiable();
      _endPointPartialMock.Setup(mock => mock.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>(_endPointPartialMock.Object.OppositeObjectID);

      var result = _endPointPartialMock.Object.CreateRemoveCommand(relatedObject);

      _endPointPartialMock.Verify();
      Assert.That(result, Is.SameAs(fakeSetCommand.Object));
    }

    [Test]
    public void CreateRemoveCommand_InvalidID ()
    {
      _endPointPartialMock.Setup(mock => mock.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);

      var orderTicket = DomainObjectIDs.OrderTicket4.GetObject<OrderTicket>();
      Assert.That(
          () => _endPointPartialMock.Object.CreateRemoveCommand(orderTicket),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot remove object 'OrderTicket|6768db2b-9c66-4e2f-bba2-89c56718ff2b|System.Guid' from object end point "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' - it currently holds object "
                  + "'OrderTicket|058ef259-f9cd-4cb1-85e5-5c05119ab596|System.Guid'."));
    }

    [Test]
    public void GetOppositeRelationEndPointID_NullEndPoint ()
    {
      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns((ObjectID)null);

      var oppositeEndPointID = _endPointPartialMock.Object.GetOppositeRelationEndPointID();

      var expectedID = RelationEndPointID.Create(null, _endPointPartialMock.Object.Definition.GetOppositeEndPointDefinition());
      Assert.That(oppositeEndPointID, Is.EqualTo(expectedID));
    }

    [Test]
    public void GetOppositeRelationEndPointID_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint(endPointID);
      Assert.That(endPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);

      var oppositeEndPointID = endPoint.GetOppositeRelationEndPointID();

      Assert.That(oppositeEndPointID, Is.Null);
    }

    [Test]
    public void GetOppositeRelationEndPointID_NonNullEndPoint ()
    {
      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);

      var oppositeEndPointID = _endPointPartialMock.Object.GetOppositeRelationEndPointID();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      Assert.That(oppositeEndPointID, Is.EqualTo(expectedID));
    }

    [Test]
    public void GetOppositeRelationEndPointIDs_UnidirectionalEndPoint ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Location1, "Client");
      var endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint(endPointID);

      Assert.That(endPoint.Definition.GetOppositeEndPointDefinition().IsAnonymous, Is.True);

      var oppositeEndPointIDs = endPoint.GetOppositeRelationEndPointIDs().ToArray();

      Assert.That(oppositeEndPointIDs, Is.Empty);
    }

    [Test]
    public void GetOppositeRelationEndPointIDs_BidirectionalEndPoint ()
    {
      _endPointPartialMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);
      var oppositeEndPointIDs = _endPointPartialMock.Object.GetOppositeRelationEndPointIDs().ToArray();

      var expectedID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.OrderTicket1, "Order");
      Assert.That(oppositeEndPointIDs, Is.EqualTo(new[] { expectedID }));
    }

  }
}
