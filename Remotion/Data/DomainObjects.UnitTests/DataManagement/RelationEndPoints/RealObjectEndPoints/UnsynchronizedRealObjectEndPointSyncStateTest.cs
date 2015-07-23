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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Serialization;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  [TestFixture]
  public class UnsynchronizedRealObjectEndPointSyncStateTest : StandardMappingTest
  {
    private IRealObjectEndPoint _endPointStub;
    private UnsynchronizedRealObjectEndPointSyncState _state;
    private IRelationEndPointDefinition _orderOrderTicketEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _orderOrderTicketEndPointDefinition = GetRelationEndPointDefinition (typeof (Order), "OrderTicket");
      
      _endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      _endPointStub.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      _endPointStub.Stub (stub => stub.Definition).Return (_orderOrderTicketEndPointDefinition);
      
      _state = new UnsynchronizedRealObjectEndPointSyncState ();
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_state.IsSynchronized(_endPointStub), Is.False);
    }

    [Test]
    public void Synchronize ()
    {
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualEndPoint> ();
      oppositeEndPointMock.Expect (mock => mock.SynchronizeOppositeEndPoint (_endPointStub));
      oppositeEndPointMock.Replay ();

      _state.Synchronize (_endPointStub, oppositeEndPointMock);

      oppositeEndPointMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "The domain object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be deleted because its "
      + "relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' is "
      + "out of sync with the opposite property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, "
      + "synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
      + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property.")]
    public void CreateDeleteCommand ()
    {
      _state.CreateDeleteCommand(_endPointStub, () => { });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "The relation property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' of object "
      + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be changed because it is "
      + "out of sync with the opposite property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order'. To make this change, "
      + "synchronize the two properties by calling the 'BidirectionalRelationSyncService.Synchronize' method on the "
      + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' property.")]
    public void CreateSetCommand ()
    {
      var relatedObject = DomainObjectMother.CreateFakeObject<OrderTicket> ();

      _state.CreateSetCommand (_endPointStub, relatedObject, domainObject => { });
    }

    [Test]
    public void FlattenedSerializable ()
    {
      var state = new UnsynchronizedRealObjectEndPointSyncState ();

      var result = FlattenedSerializer.SerializeAndDeserialize (state);

      Assert.That (result, Is.Not.Null);
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition (Type classType, string shortPropertyName)
    {
      return Configuration.GetTypeDefinition (classType).GetRelationEndPointDefinition (classType.FullName + "." + shortPropertyName);
    }
  }
}