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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  [TestFixture]
  public class UnsynchronizedRealObjectEndPointSyncStateTest : StandardMappingTest
  {
    private Mock<IRealObjectEndPoint> _endPointStub;
    private UnsynchronizedRealObjectEndPointSyncState _state;
    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition(typeof(Order), "OrderTicket");

      _endPointStub = new Mock<IRealObjectEndPoint>();
      _endPointStub.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      _endPointStub.Setup(stub => stub.Definition).Returns(_orderOrderTicketEndPointDefinition);

      _state = new UnsynchronizedRealObjectEndPointSyncState();
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That(_state.IsSynchronized(_endPointStub.Object), Is.False);
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);
      oppositeEndPointMock.Setup(mock => mock.SynchronizeOppositeEndPoint(_endPointStub.Object)).Verifiable();

      _state.Synchronize(_endPointStub.Object, oppositeEndPointMock.Object);

      oppositeEndPointMock.Verify();
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      Assert.That(
          () => _state.CreateDeleteCommand(_endPointStub.Object, () => { }),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The domain object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be deleted because its "
                  + "relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' is "
                  + "out of sync with the opposite property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, "
                  + "synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property."));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket>();
      Assert.That(
          () => _state.CreateSetCommand(_endPointStub.Object, relatedObject, domainObject => { }),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
                  + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be changed because it is "
                  + "out of sync with the opposite property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, "
                  + "synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property."));
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new UnsynchronizedRealObjectEndPointSyncState();

      var result = FlattenedSerializer.SerializeAndDeserialize(state);

      Assert.That(result, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type type, string shortPropertyName)
    {
      return Configuration.GetTypeDefinition(type).GetRelationEndPointDefinition(type.FullName + "." + shortPropertyName);
    }
  }
}
