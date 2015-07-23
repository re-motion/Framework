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
  public class UnregisterEndPointsCommandTest : StandardMappingTest
  {
    private IRelationEndPoint _endPoint1;
    private IRelationEndPoint _endPoint2;

    private IRelationEndPointRegistrationAgent _registrationAgentMock;
    private RelationEndPointMap _map;

    private UnregisterEndPointsCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _endPoint1 = MockRepository.GenerateStub<IRelationEndPoint> ();
      _endPoint2 = MockRepository.GenerateStub<IRelationEndPoint> ();

      _registrationAgentMock = MockRepository.GenerateStrictMock<IRelationEndPointRegistrationAgent>();
      _map = new RelationEndPointMap (MockRepository.GenerateStub<IClientTransactionEventSink> ());

      _command = new UnregisterEndPointsCommand (new[] { _endPoint1, _endPoint2 }, _registrationAgentMock, _map);
    }

    [Test]
    public void GetAllExceptions ()
    {
      Assert.That (_command.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void Begin_DoesNothing ()
    {
      _command.Begin();
    }

    [Test]
    public void Perform ()
    {
      _registrationAgentMock.Expect (mock => mock.UnregisterEndPoint (_endPoint1, _map));
      _registrationAgentMock.Expect (mock => mock.UnregisterEndPoint (_endPoint2, _map));
      _registrationAgentMock.Replay();

      _command.Perform();

      _registrationAgentMock.VerifyAllExpectations();
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

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}