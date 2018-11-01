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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class UnloadVirtualEndPointsCommandTest
  {
    private MockRepository _mockRepository;
    private IVirtualEndPoint _endPointMock1;
    private IVirtualEndPoint _endPointMock2;

    private IRelationEndPointRegistrationAgent _registrationAgentMock;
    private RelationEndPointMap _relationEndPointMap;

    private UnloadVirtualEndPointsCommand _command;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _endPointMock1 = _mockRepository.StrictMock<IVirtualEndPoint> ();
      _endPointMock2 = _mockRepository.StrictMock<IVirtualEndPoint> ();

      _registrationAgentMock = _mockRepository.StrictMock<IRelationEndPointRegistrationAgent>();
      _relationEndPointMap = new RelationEndPointMap (MockRepository.GenerateStub<IClientTransactionEventSink>());

      _command = new UnloadVirtualEndPointsCommand (new[] { _endPointMock1, _endPointMock2 }, _registrationAgentMock, _relationEndPointMap);
    }

    [Test]
    public void Begin ()
    {
      _mockRepository.ReplayAll();

      _command.Begin();
    }

    [Test]
    public void Perform__NonCollectible ()
    {
      _endPointMock1.Expect (mock => mock.MarkDataIncomplete ());
      _endPointMock1.Stub (stub => stub.CanBeCollected).Return (false);
      
      _endPointMock2.Expect (mock => mock.MarkDataIncomplete ());
      _endPointMock2.Stub (stub => stub.CanBeCollected).Return (false);
      _mockRepository.ReplayAll ();

      _command.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform_Collectible ()
    {
      _endPointMock1.Expect (mock => mock.MarkDataIncomplete ());
      _endPointMock1.Stub (stub => stub.CanBeCollected).Return (true);

      _registrationAgentMock.Expect (mock => mock.UnregisterEndPoint (_endPointMock1, _relationEndPointMap));

      _endPointMock2.Expect (mock => mock.MarkDataIncomplete ());
      _endPointMock2.Stub (stub => stub.CanBeCollected).Return (false);
      _mockRepository.ReplayAll ();

      _command.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End ()
    {
      _mockRepository.ReplayAll ();

      _command.End ();
    }
  }
}