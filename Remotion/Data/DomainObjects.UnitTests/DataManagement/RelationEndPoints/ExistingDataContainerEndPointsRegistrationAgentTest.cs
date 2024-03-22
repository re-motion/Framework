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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class ExistingDataContainerEndPointsRegistrationAgentTest : StandardMappingTest
  {
    private Mock<IRelationEndPointFactory> _endPointFactoryStub;
    private Mock<IRelationEndPointRegistrationAgent> _registrationAgentStub;
    private RelationEndPointMap _map;

    private ExistingDataContainerEndPointsRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryStub = new Mock<IRelationEndPointFactory>();
      _registrationAgentStub = new Mock<IRelationEndPointRegistrationAgent>();
      _map = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _agent = new ExistingDataContainerEndPointsRegistrationAgent(_endPointFactoryStub.Object, _registrationAgentStub.Object);
    }

    [Test]
    public void GetOwnedEndPointIDs ()
    {
      var dataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);

      var result = (IEnumerable<RelationEndPointID>)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetOwnedEndPointIDs", dataContainer);

      Assert.That(
          result,
          Is.EquivalentTo(
              new[]
              {
                  RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Official"),
                  RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer")
              }));
    }

    [Test]
    public void GetUnregisterProblem_EndPointHasChanged ()
    {
      var endPointStub = new Mock<IRelationEndPoint>();
      endPointStub.Setup(stub => stub.HasChanged).Returns(true);
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.EqualTo(
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "has changed. Only unchanged relation end-points can be unregistered."));
    }

    [Test]
    public void GetUnregisterProblem_OppositeLoadedEndPointHasChanged ()
    {
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.HasChanged).Returns(false);
      endPointStub.Setup(stub => stub.ID).Returns(relationEndPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(relationEndPointID.Definition);
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.Customer1);

      var oppositeEndPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");
      var oppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      oppositeEndPointStub.Setup(stub => stub.ID).Returns(oppositeEndPointID);
      oppositeEndPointStub.Setup(stub => stub.Definition).Returns(oppositeEndPointID.Definition);
      oppositeEndPointStub.Setup(stub => stub.HasChanged).Returns(true);
      _map.AddEndPoint(oppositeEndPointStub.Object);

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.EqualTo(
          "The opposite relation property "
          + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' of relation end-point "
          + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' has changed. "
          + "Non-virtual end-points that are part of changed relations cannot be unloaded."));
    }

    [Test]
    public void GetUnregisterProblem_None_WithOppositeLoadedEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.HasChanged).Returns(false);
      endPointStub.Setup(stub => stub.ID).Returns(relationEndPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(relationEndPointID.Definition);
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.Customer1);

      var oppositeEndPointStub = new Mock<IRealObjectEndPoint>();
      oppositeEndPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders"));
      oppositeEndPointStub.Setup(stub => stub.HasChanged).Returns(false);
      _map.AddEndPoint(oppositeEndPointStub.Object);

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_None_WithNonLoadedOppositeEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      var endPointStub = new Mock<IRealObjectEndPoint>();
      endPointStub.Setup(stub => stub.HasChanged).Returns(false);
      endPointStub.Setup(stub => stub.ID).Returns(relationEndPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(relationEndPointID.Definition);
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.Customer1);

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_None_WithVirtualEndPoint ()
    {
      var relationEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");

      var endPointStub = new Mock<IVirtualObjectEndPoint>();
      endPointStub.Setup(stub => stub.HasChanged).Returns(false);
      endPointStub.Setup(stub => stub.ID).Returns(relationEndPointID);
      endPointStub.Setup(stub => stub.Definition).Returns(relationEndPointID.Definition);
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.OrderTicket1);

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.Null);
    }
  }
}
