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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Moq.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class VirtualEndPointStateUpdatedRaisingCommandDecoratorTest : StandardMappingTest
  {
    private Mock<IDataManagementCommand> _decoratedCommandMock;
    private RelationEndPointID _modifiedEndPointID;
    private Mock<IVirtualEndPointStateUpdateListener> _stateUpdateListenerMock;
    private bool? _fakeChangeState;

    private VirtualEndPointStateUpdatedRaisingCommandDecorator _commandDecorator;
    private DecoratorTestHelper<IDataManagementCommand> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _decoratedCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _modifiedEndPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      _stateUpdateListenerMock = new Mock<IVirtualEndPointStateUpdateListener>(MockBehavior.Strict);
      _fakeChangeState = null;

      _commandDecorator = new VirtualEndPointStateUpdatedRaisingCommandDecorator(
          _decoratedCommandMock.Object,
          _modifiedEndPointID,
          _stateUpdateListenerMock.Object,
          () => _fakeChangeState);
      _decoratorTestHelper = new DecoratorTestHelper<IDataManagementCommand>(_commandDecorator, _decoratedCommandMock);
    }

    [Test]
    public void DelegatedMembers ()
    {
      _decoratorTestHelper.CheckDelegation(command => command.GetAllExceptions(), new[] { new Exception() });
      _decoratorTestHelper.CheckDelegation(command => command.Begin());
      _decoratorTestHelper.CheckDelegation(command => command.Begin());
      _decoratorTestHelper.CheckDelegation(command => command.End());
      _decoratorTestHelper.CheckDelegation(command => command.End());
    }

    [Test]
    public void Perform ()
    {
      var sequence = new MockSequence();
      _decoratedCommandMock.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _stateUpdateListenerMock.InSequence(sequence).Setup(mock => mock.VirtualEndPointStateUpdated(_modifiedEndPointID, null)).Verifiable();

      _commandDecorator.Perform();

      _decoratedCommandMock.Verify();
      _stateUpdateListenerMock.Verify();
    }

    [Test]
    public void Perform_WithException ()
    {
      var exception = new Exception();
      _decoratedCommandMock.Setup(mock => mock.Perform()).Throws(exception).Verifiable();
      _stateUpdateListenerMock.Setup(mock => mock.VirtualEndPointStateUpdated(_modifiedEndPointID, null)).Verifiable();

      Assert.That(() => _commandDecorator.Perform(), Throws.Exception.SameAs(exception));

      _decoratedCommandMock.Verify();
      _stateUpdateListenerMock.Verify();
    }

    [Test]
    public void Perform_WithDifferentStates ()
    {
      var sequence = new MockSequence();
      _decoratedCommandMock.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _stateUpdateListenerMock.InSequence(sequence).Setup(mock => mock.VirtualEndPointStateUpdated(_modifiedEndPointID, true)).Verifiable();
      _decoratedCommandMock.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _stateUpdateListenerMock.InSequence(sequence).Setup(mock => mock.VirtualEndPointStateUpdated(_modifiedEndPointID, false)).Verifiable();
      _decoratedCommandMock.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _stateUpdateListenerMock.InSequence(sequence).Setup(mock => mock.VirtualEndPointStateUpdated(_modifiedEndPointID, null)).Verifiable();

      _fakeChangeState = true;
      _commandDecorator.Perform();
      _fakeChangeState = false;
      _commandDecorator.Perform();
      _fakeChangeState = null;
      _commandDecorator.Perform();

      _decoratedCommandMock.Verify();
      _stateUpdateListenerMock.Verify();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var fakeExpandedCommand = new ExpandedCommand();
      _decoratedCommandMock.Setup(mock => mock.ExpandToAllRelatedObjects()).Returns(fakeExpandedCommand).Verifiable();

      var result = _commandDecorator.ExpandToAllRelatedObjects();

      _decoratedCommandMock.Verify();
      var nestedCommands = result.GetNestedCommands();
      Assert.That(nestedCommands.Count, Is.EqualTo(1));
      Assert.That(nestedCommands[0], Is.TypeOf(typeof(VirtualEndPointStateUpdatedRaisingCommandDecorator)));

      var innerExpandedCommand = (VirtualEndPointStateUpdatedRaisingCommandDecorator)nestedCommands[0];
      Assert.That(innerExpandedCommand.DecoratedCommand, Is.SameAs(fakeExpandedCommand));
      Assert.That(innerExpandedCommand.ModifiedEndPointID, Is.EqualTo(_modifiedEndPointID));
      Assert.That(innerExpandedCommand.Listener, Is.SameAs(_stateUpdateListenerMock.Object));
      Assert.That(innerExpandedCommand.ChangeStateProvider, Is.SameAs(_commandDecorator.ChangeStateProvider));
    }
  }
}
