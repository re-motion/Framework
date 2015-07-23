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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class ExistingDataContainerEndPointsRegistrationAgentTest : StandardMappingTest
  {
    private IRelationEndPointFactory _endPointFactoryStub;
    private IRelationEndPointRegistrationAgent _registrationAgentStub;
    private RelationEndPointMap _map;

    private ExistingDataContainerEndPointsRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryStub = MockRepository.GenerateStub<IRelationEndPointFactory> ();
      _registrationAgentStub = MockRepository.GenerateStub<IRelationEndPointRegistrationAgent> ();
      _map = new RelationEndPointMap (MockRepository.GenerateStub<IClientTransactionEventSink> ());

      _agent = new ExistingDataContainerEndPointsRegistrationAgent (_endPointFactoryStub, _registrationAgentStub);
    }

    [Test]
    public void GetOwnedEndPointIDs ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);

      var result = (IEnumerable<RelationEndPointID>) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetOwnedEndPointIDs", dataContainer);

      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              {
                  RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Official"),
                  RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer")
              }));
    }

    [Test]
    public void GetUnregisterProblem_EndPointHasChanged ()
    {
      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint>();
      endPointStub.Stub (stub => stub.HasChanged).Return (true);
      endPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer"));

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.EqualTo (
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "has changed. Only unchanged relation end-points can be unregistered."));
    }

    [Test]
    public void GetUnregisterProblem_OppositeLoadedEndPointHasChanged ()
    {
      var relationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.ID).Return (relationEndPointID);
      endPointStub.Stub (stub => stub.Definition).Return (relationEndPointID.Definition);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);

      var oppositeEndPointID = RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders");
      var oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (oppositeEndPointID);
      oppositeEndPointStub.Stub (stub => stub.Definition).Return (oppositeEndPointID.Definition);
      oppositeEndPointStub.Stub (stub => stub.HasChanged).Return (true);
      _map.AddEndPoint (oppositeEndPointStub);

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.EqualTo (
          "The opposite relation property "
          + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of relation end-point "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' has changed. "
          + "Non-virtual end-points that are part of changed relations cannot be unloaded."));
    }

    [Test]
    public void GetUnregisterProblem_None_WithOppositeLoadedEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.ID).Return (relationEndPointID);
      endPointStub.Stub (stub => stub.Definition).Return (relationEndPointID.Definition);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);

      var oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      oppositeEndPointStub.Stub (stub => stub.ID).Return (RelationEndPointID.Create (DomainObjectIDs.Customer1, typeof (Customer), "Orders"));
      oppositeEndPointStub.Stub (stub => stub.HasChanged).Return (false);
      _map.AddEndPoint (oppositeEndPointStub);

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_None_WithNonLoadedOppositeEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      var endPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.ID).Return (relationEndPointID);
      endPointStub.Stub (stub => stub.Definition).Return (relationEndPointID.Definition);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.Customer1);

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_None_WithVirtualEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");

      var endPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();
      endPointStub.Stub (stub => stub.HasChanged).Return (false);
      endPointStub.Stub (stub => stub.ID).Return (relationEndPointID);
      endPointStub.Stub (stub => stub.Definition).Return (relationEndPointID.Definition);
      endPointStub.Stub (stub => stub.OppositeObjectID).Return (DomainObjectIDs.OrderTicket1);

      var result = (string) PrivateInvoke.InvokeNonPublicMethod (_agent, "GetUnregisterProblem", endPointStub, _map);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void Serialization ()
    {
      var agent = new ExistingDataContainerEndPointsRegistrationAgent (
          new SerializableRelationEndPointFactoryFake(), 
          new SerializableRelationEndPointRegistrationAgentFake());

      var deserializedAgent = Serializer.SerializeAndDeserialize (agent);

      Assert.That (deserializedAgent.EndPointFactory, Is.Not.Null);
      Assert.That (deserializedAgent.RegistrationAgent, Is.Not.Null);
    }
  }
}