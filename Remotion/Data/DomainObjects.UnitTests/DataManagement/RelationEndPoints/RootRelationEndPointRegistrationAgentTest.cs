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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RootRelationEndPointRegistrationAgentTest : StandardMappingTest
  {
    private IVirtualEndPointProvider _virtualEndPointProviderMock;
    private RelationEndPointMap _map;

    private RootRelationEndPointRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp ();

      _virtualEndPointProviderMock = MockRepository.GenerateStrictMock<IVirtualEndPointProvider> ();
      _map = new RelationEndPointMap (MockRepository.GenerateStub<IClientTransactionEventSink> ());

      _agent = new RootRelationEndPointRegistrationAgent (_virtualEndPointProviderMock);
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_Unidirectional ()
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      var unidirectionalEndPointID = RelationEndPointID.Create (DomainObjectIDs.Location1, typeof (Location), "Client");
      endPointMock.Stub (stub => stub.ID).Return (unidirectionalEndPointID);
      endPointMock.Stub (stub => stub.Definition).Return (unidirectionalEndPointID.Definition);
      endPointMock.Stub (stub => stub.OriginalOppositeObjectID).Return (DomainObjectIDs.Client1);

      endPointMock.Expect (mock => mock.MarkSynchronized ());
      endPointMock.Replay ();

      _virtualEndPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock, _map);

      endPointMock.VerifyAllExpectations ();
      Assert.That (_map, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeCollectionEndPoint ()
    {
      var endPointMock = CreateRealObjectEndPointMock (DomainObjectIDs.OrderItem1, "Order", DomainObjectIDs.Order1);

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<ICollectionEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay ();

      var oppositeEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      _virtualEndPointProviderMock
          .Expect (mock => mock.GetOrCreateVirtualEndPoint (oppositeEndPointID))
          .Return (oppositeEndPointMock);
      _virtualEndPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock, _map);

      endPointMock.VerifyAllExpectations ();
      _virtualEndPointProviderMock.VerifyAllExpectations ();

      oppositeEndPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (Arg<DomainObject[]>.Is.Anything));
      oppositeEndPointMock.VerifyAllExpectations ();
      
      Assert.That (_map, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_VirtualEndPointNotYetComplete ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem> ();

      var endPointMock = CreateRealObjectEndPointMock (DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
      endPointMock.Stub (stub => stub.GetDomainObjectReference()).Return (objectReference);
      
      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Expect (mock => mock.MarkDataComplete (objectReference));
      oppositeEndPointMock.Replay ();

      var oppositeEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      _virtualEndPointProviderMock
          .Expect (mock => mock.GetOrCreateVirtualEndPoint (oppositeEndPointID))
          .Return (oppositeEndPointMock);
      _virtualEndPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock, _map);

      endPointMock.VerifyAllExpectations ();
      _virtualEndPointProviderMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (_map, Has.Member (endPointMock));
    }

    [Test]
    public void RegisterEndPoint_RealEndPoint_PointingToNonNull_OppositeObjectEndPoint_VirtualEndPointAlreadyComplete ()
    {
      var objectReference = DomainObjectMother.CreateFakeObject<OrderItem> ();
      var endPointMock = CreateRealObjectEndPointMock (DomainObjectIDs.OrderTicket1, "Order", DomainObjectIDs.Order1);
      endPointMock.Stub (stub => stub.GetDomainObjectReference ()).Return (objectReference);
      endPointMock.Replay ();

      var oppositeEndPointMock = MockRepository.GenerateStrictMock<IVirtualObjectEndPoint> ();
      oppositeEndPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      oppositeEndPointMock.Expect (mock => mock.RegisterOriginalOppositeEndPoint (endPointMock));
      oppositeEndPointMock.Replay ();

      var oppositeEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      _virtualEndPointProviderMock
          .Expect (mock => mock.GetOrCreateVirtualEndPoint (oppositeEndPointID))
          .Return (oppositeEndPointMock);
      _virtualEndPointProviderMock.Replay ();

      _agent.RegisterEndPoint (endPointMock, _map);

      oppositeEndPointMock.AssertWasNotCalled (mock => mock.MarkDataComplete (Arg<DomainObject>.Is.Anything));
      endPointMock.VerifyAllExpectations ();
      _virtualEndPointProviderMock.VerifyAllExpectations ();
      oppositeEndPointMock.VerifyAllExpectations ();
      Assert.That (_map, Has.Member (endPointMock));
    }

    [Test]
    public void Serialization ()
    {
      var agent = new RootRelationEndPointRegistrationAgent (new SerializableVirtualEndPointProviderFake ());

      var deserializedAgent = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedAgent.VirtualEndPointProvider, Is.Not.Null);
    }

    private IRealObjectEndPoint CreateRealObjectEndPointMock (ObjectID originatingObjectID, string shortPropertyName, ObjectID oppositeObjectID)
    {
      var endPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      var relationEndPointID = RelationEndPointID.Create (originatingObjectID, originatingObjectID.ClassDefinition.ClassType, shortPropertyName);
      endPointMock.Stub (stub => stub.ID).Return (relationEndPointID);
      endPointMock.Stub (stub => stub.Definition).Return (relationEndPointID.Definition);
      endPointMock.Stub (stub => stub.OriginalOppositeObjectID).Return (oppositeObjectID);
      endPointMock.Replay ();
      return endPointMock;
    }
  }
}