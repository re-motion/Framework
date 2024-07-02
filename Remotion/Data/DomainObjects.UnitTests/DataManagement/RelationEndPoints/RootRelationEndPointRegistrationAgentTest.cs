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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RootRelationEndPointRegistrationAgentTest : StandardMappingTest
  {
    private Mock<IVirtualEndPointProvider> _virtualEndPointProviderMock;
    private RelationEndPointMap _map;

    private RootRelationEndPointRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointProviderMock = new Mock<IVirtualEndPointProvider>(MockBehavior.Strict);
      _map = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _agent = new RootRelationEndPointRegistrationAgent(_virtualEndPointProviderMock.Object);
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_Unidirectional ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      var unidirectionalEndPointID = RelationEndPointID.Create(DomainObjectIDs.Location1, typeof(Location), "Client");
      endPointMock.Setup(stub => stub.ID).Returns(unidirectionalEndPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(unidirectionalEndPointID.Definition);
      endPointMock.Setup(stub => stub.OriginalOppositeObjectID).Returns(DomainObjectIDs.Client1);

      endPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeCollectionEndPoint ()
    {
      var endPointMock = CreateRealObjectEndPointMock(DomainObjectIDs.OrderItem1, "Order", DomainObjectIDs.Order1);

      var oppositeEndPointMock = new Mock<ICollectionEndPoint<ICollectionEndPointData>>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(stub => stub.IsDataComplete).Returns(false);
      oppositeEndPointMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      var oppositeEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      _virtualEndPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _virtualEndPointProviderMock.Verify();

      oppositeEndPointMock.Verify(mock => mock.MarkDataComplete(It.IsAny<DomainObject[]>()), Times.Never());
      oppositeEndPointMock.Verify();

      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_VirtualEndPointNotYetComplete ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem>();

      var endPointMock = CreateRealObjectEndPointMock(DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
      endPointMock.Setup(stub => stub.GetDomainObjectReference()).Returns(objectReference);

      var oppositeEndPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(stub => stub.IsDataComplete).Returns(false);
      oppositeEndPointMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();
      oppositeEndPointMock.Setup(mock => mock.MarkDataComplete(objectReference)).Verifiable();

      var oppositeEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      _virtualEndPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _virtualEndPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_VirtualEndPointAlreadyComplete ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem>();
      var endPointMock = CreateRealObjectEndPointMock(DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
      endPointMock.Setup(stub => stub.GetDomainObjectReference()).Returns(objectReference);

      var oppositeEndPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(stub => stub.IsDataComplete).Returns(true);
      oppositeEndPointMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      var oppositeEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      _virtualEndPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      oppositeEndPointMock.Verify(mock => mock.MarkDataComplete(It.IsAny<DomainObject>()), Times.Never());
      endPointMock.Verify();
      _virtualEndPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    private Mock<IRealObjectEndPoint> CreateRealObjectEndPointMock (ObjectID originatingObjectID, string shortPropertyName, ObjectID oppositeObjectID)
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      var relationEndPointID = RelationEndPointID.Create(originatingObjectID, originatingObjectID.ClassDefinition.ClassType, shortPropertyName);
      endPointMock.Setup(stub => stub.ID).Returns(relationEndPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(relationEndPointID.Definition);
      endPointMock.Setup(stub => stub.OriginalOppositeObjectID).Returns(oppositeObjectID);
      return endPointMock;
    }
  }
}
