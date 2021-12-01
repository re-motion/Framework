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
  public class UnloadVirtualEndPointsCommandTest
  {
    private Mock<IVirtualEndPoint> _endPointMock1;
    private Mock<IVirtualEndPoint> _endPointMock2;

    private Mock<IRelationEndPointRegistrationAgent> _registrationAgentMock;
    private RelationEndPointMap _relationEndPointMap;

    private UnloadVirtualEndPointsCommand _command;

    [SetUp]
    public void SetUp ()
    {
      _endPointMock1 = new Mock<IVirtualEndPoint> (MockBehavior.Strict);
      _endPointMock2 = new Mock<IVirtualEndPoint> (MockBehavior.Strict);

      _registrationAgentMock = new Mock<IRelationEndPointRegistrationAgent> (MockBehavior.Strict);
      _relationEndPointMap = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _command = new UnloadVirtualEndPointsCommand(new[] { _endPointMock1.Object, _endPointMock2.Object }, _registrationAgentMock.Object, _relationEndPointMap);
    }

    [Test]
    public void Begin ()
    {
      _command.Begin();
    }

    [Test]
    public void Perform__NonCollectible ()
    {
      _endPointMock1.Setup (mock => mock.MarkDataIncomplete()).Verifiable();
      _endPointMock1.Setup (stub => stub.CanBeCollected).Returns (false);

      _endPointMock2.Setup (mock => mock.MarkDataIncomplete()).Verifiable();
      _endPointMock2.Setup (stub => stub.CanBeCollected).Returns (false);

      _command.Perform();

      _endPointMock1.Verify();
      _endPointMock2.Verify();
      _registrationAgentMock.Verify();
    }

    [Test]
    public void Perform_Collectible ()
    {
      _endPointMock1.Setup (mock => mock.MarkDataIncomplete()).Verifiable();
      _endPointMock1.Setup (stub => stub.CanBeCollected).Returns (true);

      _registrationAgentMock.Setup (mock => mock.UnregisterEndPoint (_endPointMock1.Object, _relationEndPointMap)).Verifiable();

      _endPointMock2.Setup (mock => mock.MarkDataIncomplete()).Verifiable();
      _endPointMock2.Setup (stub => stub.CanBeCollected).Returns (false);

      _command.Perform();

      _endPointMock1.Verify();
      _endPointMock2.Verify();
      _registrationAgentMock.Verify();
    }

    [Test]
    public void End ()
    {
      _command.End();
    }
  }
}
