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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class DataContainerEndPointsRegistrationAgentBaseTest : StandardMappingTest
  {
    private Mock<IRelationEndPointFactory> _endPointFactoryMock;
    private Mock<IRelationEndPointRegistrationAgent> _registrationAgentMock;

    private Mock<TestableDataContainerEndPointsRegistrationAgentBase> _agentPartialMock;

    private RelationEndPointMap _map;

    private RelationEndPointID _orderTicketEndPointID;
    private RelationEndPointID _customerEndPointID;

    private Mock<IVirtualObjectEndPoint> _orderTicketEndPointMock;
    private Mock<IRealObjectEndPoint> _customerEndPointStub;
    private DataContainer _orderDataContainer;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryMock = new Mock<IRelationEndPointFactory>(MockBehavior.Strict);
      _registrationAgentMock = new Mock<IRelationEndPointRegistrationAgent>(MockBehavior.Strict);

      _agentPartialMock = new Mock<TestableDataContainerEndPointsRegistrationAgentBase>(
          _endPointFactoryMock.Object, _registrationAgentMock.Object)          { CallBase = true };

      _map = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _orderTicketEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      _customerEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      _orderTicketEndPointMock = new Mock<IVirtualObjectEndPoint>(MockBehavior.Strict);
      _orderTicketEndPointMock.Setup(stub => stub.ID).Returns(_orderTicketEndPointID);

      _customerEndPointStub = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      _customerEndPointStub.Setup(stub => stub.ID).Returns(_customerEndPointID);

      _orderDataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);
    }

    [Test]
    public void RegisterEndPoints ()
    {
      _agentPartialMock
          .Setup(stub => stub.MockableGetOwnedEndPointIDs(_orderDataContainer))
          .Returns(new[] { _orderTicketEndPointID, _customerEndPointID });

      _endPointFactoryMock.Setup(mock => mock.CreateVirtualObjectEndPoint(_orderTicketEndPointID)).Returns(_orderTicketEndPointMock.Object).Verifiable();
      _endPointFactoryMock.Setup(mock => mock.CreateRealObjectEndPoint(_customerEndPointID, _orderDataContainer)).Returns(_customerEndPointStub.Object).Verifiable();

      _orderTicketEndPointMock.Setup(mock => mock.MarkDataComplete(null)).Verifiable();

      _registrationAgentMock.Setup(mock => mock.RegisterEndPoint(_orderTicketEndPointMock.Object, _map)).Verifiable();
      _registrationAgentMock.Setup(mock => mock.RegisterEndPoint(_customerEndPointStub.Object, _map)).Verifiable();

      _agentPartialMock.Object.RegisterEndPoints(_orderDataContainer, _map);

      _endPointFactoryMock.Verify();
      _registrationAgentMock.Verify();
      _orderTicketEndPointMock.Verify();
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_EndPointsNotLoaded ()
    {
      _agentPartialMock.Setup(stub => stub.MockableGetOwnedEndPointIDs(_orderDataContainer)).Returns(new[] { _orderTicketEndPointID, _customerEndPointID });

      var result = _agentPartialMock.Object.CreateUnregisterEndPointsCommand(_orderDataContainer, _map);

      Assert.That(result, Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That(((UnregisterEndPointsCommand)result).EndPoints, Is.Empty);
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_Loaded_NoProblems ()
    {
      _map.AddEndPoint(_orderTicketEndPointMock.Object);
      _map.AddEndPoint(_customerEndPointStub.Object);

      _agentPartialMock.Setup(stub => stub.MockableGetOwnedEndPointIDs(_orderDataContainer)).Returns(new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Setup(mock => mock.MockableGetUnregisterProblem(_orderTicketEndPointMock.Object, _map)).Returns((string)null).Verifiable();
      _agentPartialMock.Setup(mock => mock.MockableGetUnregisterProblem(_customerEndPointStub.Object, _map)).Returns((string)null).Verifiable();

      var result = _agentPartialMock.Object.CreateUnregisterEndPointsCommand(_orderDataContainer, _map);

      _agentPartialMock.Verify();
      Assert.That(result, Is.TypeOf<UnregisterEndPointsCommand>());
      Assert.That(((UnregisterEndPointsCommand)result).EndPoints, Is.EqualTo(new IRelationEndPoint[] { _orderTicketEndPointMock.Object, _customerEndPointStub.Object }));
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_Loaded_Problems ()
    {
      _map.AddEndPoint(_orderTicketEndPointMock.Object);
      _map.AddEndPoint(_customerEndPointStub.Object);

      _agentPartialMock.Setup(stub => stub.MockableGetOwnedEndPointIDs(_orderDataContainer)).Returns(new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Setup(mock => mock.MockableGetUnregisterProblem(_orderTicketEndPointMock.Object, _map)).Returns("Oh no!").Verifiable();
      _agentPartialMock.Setup(mock => mock.MockableGetUnregisterProblem(_customerEndPointStub.Object, _map)).Returns("Oh no 2!").Verifiable();

      var result = _agentPartialMock.Object.CreateUnregisterEndPointsCommand(_orderDataContainer, _map);

      _agentPartialMock.Verify();
      Assert.That(result, Is.TypeOf<ExceptionCommand>());
      Assert.That(
          ((ExceptionCommand)result).Exception.Message,
          Is.EqualTo("The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\nOh no!\r\nOh no 2!"));
    }

    [Test]
    public void CreateUnregisterEndPointsCommand_Loaded_SomeProblems ()
    {
      _map.AddEndPoint(_orderTicketEndPointMock.Object);
      _map.AddEndPoint(_customerEndPointStub.Object);

      _agentPartialMock.Setup(stub => stub.MockableGetOwnedEndPointIDs(_orderDataContainer)).Returns(new[] { _orderTicketEndPointID, _customerEndPointID });
      _agentPartialMock.Setup(mock => mock.MockableGetUnregisterProblem(_orderTicketEndPointMock.Object, _map)).Returns("Oh no!").Verifiable();
      _agentPartialMock.Setup(mock => mock.MockableGetUnregisterProblem(_customerEndPointStub.Object, _map)).Returns((string)null).Verifiable();

      var result = _agentPartialMock.Object.CreateUnregisterEndPointsCommand(_orderDataContainer, _map);

      _agentPartialMock.Verify();
      Assert.That(result, Is.TypeOf<ExceptionCommand>());
      Assert.That(
          ((ExceptionCommand)result).Exception.Message,
          Is.EqualTo("The relations of object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' cannot be unloaded.\r\nOh no!"));
    }
  }
}
