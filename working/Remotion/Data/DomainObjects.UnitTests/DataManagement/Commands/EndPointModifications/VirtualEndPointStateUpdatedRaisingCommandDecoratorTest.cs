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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class VirtualEndPointStateUpdatedRaisingCommandDecoratorTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    private IDataManagementCommand _decoratedCommandMock;
    private RelationEndPointID _modifiedEndPointID;
    private IVirtualEndPointStateUpdateListener _stateUpdateListenerMock;
    private bool? _fakeChangeState;

    private VirtualEndPointStateUpdatedRaisingCommandDecorator _commandDecorator;
    private DecoratorTestHelper<IDataManagementCommand> _decoratorTestHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _decoratedCommandMock = _mockRepository.StrictMock<IDataManagementCommand> ();
      _modifiedEndPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      _stateUpdateListenerMock = _mockRepository.StrictMock<IVirtualEndPointStateUpdateListener> ();
      _fakeChangeState = null;

      _commandDecorator = new VirtualEndPointStateUpdatedRaisingCommandDecorator (
          _decoratedCommandMock,
          _modifiedEndPointID,
          _stateUpdateListenerMock,
          () => _fakeChangeState);
      _decoratorTestHelper = new DecoratorTestHelper<IDataManagementCommand> (_commandDecorator, _decoratedCommandMock);
    }

    [Test]
    public void DelegatedMembers ()
    {
      _decoratorTestHelper.CheckDelegation (command => command.GetAllExceptions(), new[] { new Exception() });
      _decoratorTestHelper.CheckDelegation (command => command.Begin());
      _decoratorTestHelper.CheckDelegation (command => command.Begin ());
      _decoratorTestHelper.CheckDelegation (command => command.End ());
      _decoratorTestHelper.CheckDelegation (command => command.End ());
    }

    [Test]
    public void Perform ()
    {
      using (_mockRepository.Ordered ())
      {
        _decoratedCommandMock.Expect (mock => mock.Perform());
        _stateUpdateListenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_modifiedEndPointID, null));
      }
      _mockRepository.ReplayAll();

      _commandDecorator.Perform ();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform_WithException ()
    {
      var exception = new Exception();
      _decoratedCommandMock.Expect (mock => mock.Perform ()).Throw (exception);
      _stateUpdateListenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_modifiedEndPointID, null));

      _mockRepository.ReplayAll ();

      Assert.That (() => _commandDecorator.Perform(), Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Perform_WithDifferentStates ()
    {
      using (_mockRepository.Ordered ())
      {
        _decoratedCommandMock.Expect (mock => mock.Perform ());
        _stateUpdateListenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_modifiedEndPointID, true));
        _decoratedCommandMock.Expect (mock => mock.Perform ());
        _stateUpdateListenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_modifiedEndPointID, false));
        _decoratedCommandMock.Expect (mock => mock.Perform ());
        _stateUpdateListenerMock.Expect (mock => mock.VirtualEndPointStateUpdated (_modifiedEndPointID, null));
      }
      _mockRepository.ReplayAll ();

      _fakeChangeState = true;
      _commandDecorator.Perform ();
      _fakeChangeState = false;
      _commandDecorator.Perform ();
      _fakeChangeState = null;
      _commandDecorator.Perform ();

      _mockRepository.VerifyAll ();
    }
    
    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var fakeExpandedCommand = new ExpandedCommand();
      _decoratedCommandMock.Expect (mock => mock.ExpandToAllRelatedObjects()).Return (fakeExpandedCommand);
      _decoratedCommandMock.Replay();

      var result = _commandDecorator.ExpandToAllRelatedObjects();

      _decoratedCommandMock.VerifyAllExpectations();
      var nestedCommands = result.GetNestedCommands ();
      Assert.That (nestedCommands.Count, Is.EqualTo (1));
      Assert.That (nestedCommands[0], Is.TypeOf (typeof (VirtualEndPointStateUpdatedRaisingCommandDecorator)));

      var innerExpandedCommand = (VirtualEndPointStateUpdatedRaisingCommandDecorator) nestedCommands[0];
      Assert.That (innerExpandedCommand.DecoratedCommand, Is.SameAs (fakeExpandedCommand));
      Assert.That (innerExpandedCommand.ModifiedEndPointID, Is.EqualTo (_modifiedEndPointID));
      Assert.That (innerExpandedCommand.Listener, Is.SameAs (_stateUpdateListenerMock));
      Assert.That (innerExpandedCommand.ChangeStateProvider, Is.SameAs (_commandDecorator.ChangeStateProvider));
    }
  }
}