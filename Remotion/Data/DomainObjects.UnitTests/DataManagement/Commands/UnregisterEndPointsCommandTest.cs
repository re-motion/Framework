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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class UnregisterEndPointsCommandTest : StandardMappingTest
  {
    private Mock<IRelationEndPoint> _endPoint1;
    private Mock<IRelationEndPoint> _endPoint2;

    private Mock<IRelationEndPointRegistrationAgent> _registrationAgentMock;
    private RelationEndPointMap _map;

    private UnregisterEndPointsCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _endPoint1 = new Mock<IRelationEndPoint>();
      _endPoint2 = new Mock<IRelationEndPoint>();

      _registrationAgentMock = new Mock<IRelationEndPointRegistrationAgent> (MockBehavior.Strict);
      _map = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _command = new UnregisterEndPointsCommand(new[] { _endPoint1.Object, _endPoint2.Object }, _registrationAgentMock.Object, _map);
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That(_command.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void Begin_DoesNothing ()
    {
      _command.Begin();
    }

    [Test]
    public void Perform ()
    {
      _registrationAgentMock.Setup (mock => mock.UnregisterEndPoint (_endPoint1.Object, _map)).Verifiable();
      _registrationAgentMock.Setup (mock => mock.UnregisterEndPoint (_endPoint2.Object, _map)).Verifiable();

      _command.Perform();

      _registrationAgentMock.Verify();
    }

    [Test]
    public void End_DoesNothing ()
    {
      _command.End();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = _command.ExpandToAllRelatedObjects();

      Assert.That(result.GetNestedCommands(), Is.EqualTo(new[] { _command }));
    }
  }
}
