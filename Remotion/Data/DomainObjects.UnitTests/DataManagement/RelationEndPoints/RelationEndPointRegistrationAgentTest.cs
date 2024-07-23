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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointRegistrationAgentTest : StandardMappingTest
  {
    private Mock<IVirtualEndPointProvider> _endPointProviderMock;
    private RelationEndPointMap _map;

    private RelationEndPointRegistrationAgent _agent;
    private RelationEndPointID _realOneManyEndPointID;
    private RelationEndPointID _virtualEndPointID;
    private RelationEndPointID _unidirectionalEndPointID;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointProviderMock = new Mock<IVirtualEndPointProvider>(MockBehavior.Strict);
      _map = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _realOneManyEndPointID = RelationEndPointID.Create(DomainObjectIDs.OrderItem1, typeof(OrderItem), "Order");
      _virtualEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      _unidirectionalEndPointID = RelationEndPointID.Create(DomainObjectIDs.Location1, typeof(Location), "Client");

      _agent = new RelationEndPointRegistrationAgent(_endPointProviderMock.Object);
    }

    [Test]
    public void RegisterEndPoint_NonRealEndPoint ()
    {
      var endPointMock = CreateVirtualEndPointMock();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNull ()
    {
      var endPointMock = CreateRealObjectEndPointMock(null);

      var oppositeEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(RelationEndPointID.Create(null, _virtualEndPointID.Definition)))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _endPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_Unidirectional ()
    {
      var endPointMock = CreateUnidirectionalEndPointMock();
      endPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_NonUnidirectional ()
    {
      var endPointMock = CreateRealObjectEndPointMock(_virtualEndPointID.ObjectID);

      var oppositeEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(_virtualEndPointID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _endPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem>();
      var endPointMock = CreateRealObjectEndPointMock(_virtualEndPointID.ObjectID);
      endPointMock.Setup(stub => stub.GetDomainObjectReference()).Returns(objectReference);

      var oppositeEndPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(mock => mock.RegisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(_virtualEndPointID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.RegisterEndPoint(endPointMock.Object, _map);

      oppositeEndPointMock.Verify(mock => mock.MarkDataComplete(objectReference), Times.Never());
      endPointMock.Verify();
      _endPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void RegisterEndPoint_IDAlreadyRegistered ()
    {
      var existingEndPointStub = new Mock<IRelationEndPoint>();
      existingEndPointStub.Setup(stub => stub.ID).Returns(_realOneManyEndPointID);
      _map.AddEndPoint(existingEndPointStub.Object);

      Assert.That(() => _agent.RegisterEndPoint(existingEndPointStub.Object, _map), Throws.InvalidOperationException.With.Message.EqualTo(
          "A relation end-point with ID "
          + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' has "
          + "already been registered."));
    }

    [Test]
    public void UnregisterEndPoint_NonRealEndPoint ()
    {
      var endPointMock = CreateVirtualEndPointMock();
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);

      _map.AddEndPoint(endPointMock.Object);

      _agent.UnregisterEndPoint(endPointMock.Object, _map);

      Assert.That(_map, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNull_DomainObjectCollection ()
    {
      var endPointMock = CreateRealObjectEndPointMock(null);
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);
      endPointMock.Setup(mock => mock.ResetSyncState()).Verifiable();

      _map.AddEndPoint(endPointMock.Object);

      var oppositeEndPointID = RelationEndPointID.CreateOpposite(endPointMock.Object.Definition, null);
      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointID))
          .Returns(new NullDomainObjectCollectionEndPoint(ClientTransaction.CreateRootTransaction(), oppositeEndPointID.Definition))
          .Verifiable();

      _agent.UnregisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _endPointProviderMock.Verify();
      Assert.That(_map, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNull_VirtualCollection ()
    {
      var endPointMock = CreateRealObjectEndPointMock(null);
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);
      endPointMock.Setup(mock => mock.ResetSyncState()).Verifiable();

      _map.AddEndPoint(endPointMock.Object);

      var oppositeEndPointID = RelationEndPointID.CreateOpposite(endPointMock.Object.Definition, null);
      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointID))
          .Returns(new NullVirtualCollectionEndPoint(ClientTransaction.CreateRootTransaction(), oppositeEndPointID.Definition))
          .Verifiable();

      _agent.UnregisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _endPointProviderMock.Verify();
      Assert.That(_map, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNonNull_Unidirectional ()
    {
      var endPointMock = CreateUnidirectionalEndPointMock();
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);
      endPointMock.Setup(mock => mock.ResetSyncState()).Verifiable();

      _map.AddEndPoint(endPointMock.Object);

      _agent.UnregisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      Assert.That(_map, Has.No.Member(endPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNonNull_NonUnidirectional_CanBeCollectedTrue ()
    {
      var endPointMock = CreateRealObjectEndPointMock(_virtualEndPointID.ObjectID);
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);

      var oppositeEndPointMock = CreateVirtualEndPointMock();
      oppositeEndPointMock.Setup(stub => stub.CanBeCollected).Returns(true);
      oppositeEndPointMock.Setup(mock => mock.UnregisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _map.AddEndPoint(endPointMock.Object);
      _map.AddEndPoint(oppositeEndPointMock.Object);

      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointMock.Object.ID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.UnregisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _endPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.No.Member(endPointMock.Object));
      Assert.That(_map, Has.No.Member(oppositeEndPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNonNull_NonUnidirectional_CanBeCollectedFalse ()
    {
      var endPointMock = CreateRealObjectEndPointMock(_virtualEndPointID.ObjectID);
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);

      var oppositeEndPointMock = CreateVirtualEndPointMock();
      oppositeEndPointMock.Setup(stub => stub.CanBeCollected).Returns(false);
      oppositeEndPointMock.Setup(mock => mock.UnregisterOriginalOppositeEndPoint(endPointMock.Object)).Verifiable();

      _map.AddEndPoint(endPointMock.Object);
      _map.AddEndPoint(oppositeEndPointMock.Object);

      _endPointProviderMock
          .Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeEndPointMock.Object.ID))
          .Returns(oppositeEndPointMock.Object)
          .Verifiable();

      _agent.UnregisterEndPoint(endPointMock.Object, _map);

      endPointMock.Verify();
      _endPointProviderMock.Verify();
      oppositeEndPointMock.Verify();
      Assert.That(_map, Has.No.Member(endPointMock.Object));
      Assert.That(_map, Has.Member(oppositeEndPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_RealEndPoint_PointingToNonNull_NonUnidirectional_OppositeEndPointNotFound ()
    {
      var endPointMock = CreateRealObjectEndPointMock(_virtualEndPointID.ObjectID);
      endPointMock.Setup(stub => stub.HasChanged).Returns(false);

      _map.AddEndPoint(endPointMock.Object);

      _endPointProviderMock.Setup(mock => mock.GetOrCreateVirtualEndPoint(_virtualEndPointID)).Returns((IVirtualEndPoint)null).Verifiable();

      Assert.That(() => _agent.UnregisterEndPoint(endPointMock.Object, _map), Throws.InvalidOperationException.With.Message.EqualTo(
          "Opposite end-point of "
          + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' "
          + "not found. When unregistering a non-virtual bidirectional end-point, the opposite end-point must exist."));
      Assert.That(_map, Has.Member(endPointMock.Object));
    }

    [Test]
    public void UnregisterEndPoint_NotRegistered ()
    {
      var existingEndPoint = new Mock<IRelationEndPoint>();
      existingEndPoint.Setup(stub => stub.ID).Returns(_realOneManyEndPointID);

      Assert.That(() => _agent.UnregisterEndPoint(existingEndPoint.Object, _map), Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
          "End-point 'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order' "
          + "is not part of this map.",
          "endPoint"));
    }

    private Mock<IVirtualEndPoint> CreateVirtualEndPointMock ()
    {
      var endPointMock = new Mock<IVirtualEndPoint>();
      endPointMock.Setup(stub => stub.ID).Returns(_virtualEndPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(_virtualEndPointID.Definition);
      return endPointMock;
    }

    private Mock<IRealObjectEndPoint> CreateRealObjectEndPointMock (ObjectID oppositeObjectID)
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.ID).Returns(_realOneManyEndPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(_realOneManyEndPointID.Definition);
      endPointMock.Setup(stub => stub.OriginalOppositeObjectID).Returns(oppositeObjectID);
      return endPointMock;
    }

    private Mock<IRealObjectEndPoint> CreateUnidirectionalEndPointMock ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.ID).Returns(_unidirectionalEndPointID);
      endPointMock.Setup(stub => stub.Definition).Returns(_unidirectionalEndPointID.Definition);
      endPointMock.Setup(stub => stub.OriginalOppositeObjectID).Returns(DomainObjectIDs.Client1);
      return endPointMock;
    }

  }
}
