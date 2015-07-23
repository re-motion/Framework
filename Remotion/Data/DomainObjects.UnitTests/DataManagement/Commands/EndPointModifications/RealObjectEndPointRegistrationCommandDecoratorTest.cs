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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RealObjectEndPointRegistrationCommandDecoratorTest
  {
    private MockRepository _mockRepository;

    private IDataManagementCommand _decoratedCommandMock;
    private IRealObjectEndPoint _realObjectEndPointStub;
    private IVirtualEndPoint _oldRelatedEndPointMock;
    private IVirtualEndPoint _newRelatedEndPointMock;
    
    private RealObjectEndPointRegistrationCommandDecorator _decorator;

    private Exception _exception1;
    private Exception _exception2;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _decoratedCommandMock = _mockRepository.StrictMock<IDataManagementCommand> ();
      _realObjectEndPointStub = _mockRepository.Stub<IRealObjectEndPoint> ();
      _oldRelatedEndPointMock = _mockRepository.StrictMock<IVirtualEndPoint> ();
      _newRelatedEndPointMock = _mockRepository.StrictMock<IVirtualEndPoint> ();

      _decorator = new RealObjectEndPointRegistrationCommandDecorator (
          _decoratedCommandMock, 
          _realObjectEndPointStub, 
          _oldRelatedEndPointMock, 
          _newRelatedEndPointMock);

      _exception1 = new Exception ("1");
      _exception2 = new Exception ("2");
    }

    [Test]
    public void GetAllExceptions ()
    {
      _decoratedCommandMock.Stub (stub => stub.GetAllExceptions()).Return (new[] { _exception1, _exception2 });
      _mockRepository.ReplayAll();

      var result = _decorator.GetAllExceptions();

      Assert.That (result, Is.EqualTo (new[] { _exception1, _exception2 }));
    }

    [Test]
    public void Perform ()
    {
      _decoratedCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      using (_mockRepository.Ordered ())
      {
        _oldRelatedEndPointMock.Expect (mock => mock.UnregisterCurrentOppositeEndPoint (_realObjectEndPointStub));
        _decoratedCommandMock.Expect (mock => mock.Perform());
        _newRelatedEndPointMock.Expect (mock => mock.RegisterCurrentOppositeEndPoint (_realObjectEndPointStub));
      }

      _mockRepository.ReplayAll();

      _decorator.Perform ();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      _decoratedCommandMock.Stub (stub => stub.GetAllExceptions()).Return (new[] { _exception1 });
      _mockRepository.ReplayAll ();

      var exception = Assert.Throws<Exception> (_decorator.Perform);
      Assert.That (exception, Is.SameAs (_exception1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var fakeExpandedCommand = new ExpandedCommand();
      _decoratedCommandMock
          .Stub (stub => stub.ExpandToAllRelatedObjects())
          .Return (fakeExpandedCommand);
      _mockRepository.ReplayAll();

      var result = _decorator.ExpandToAllRelatedObjects();

      Assert.That (result, Is.Not.Null);
      
      var nestedCommands = result.GetNestedCommands ();
      Assert.That (nestedCommands.Count, Is.EqualTo (1));
      Assert.That (nestedCommands[0], Is.TypeOf (typeof (RealObjectEndPointRegistrationCommandDecorator)));
      
      var innerExpandedCommand = (RealObjectEndPointRegistrationCommandDecorator) nestedCommands[0];
      Assert.That (innerExpandedCommand.DecoratedCommand, Is.SameAs (fakeExpandedCommand));
      Assert.That (innerExpandedCommand.RealObjectEndPoint, Is.SameAs (_realObjectEndPointStub));
      Assert.That (innerExpandedCommand.OldRelatedEndPoint, Is.SameAs (_oldRelatedEndPointMock));
      Assert.That (innerExpandedCommand.NewRelatedEndPoint, Is.SameAs (_newRelatedEndPointMock));
    }

    [Test]
    public void DelegatingMembers ()
    {
      CheckOperationIsDelegated (c => c.Begin ());
      CheckOperationIsDelegated (c => c.End ());
      CheckOperationIsDelegated (c => c.Begin ());
      CheckOperationIsDelegated (c => c.End ());
    }

    private void CheckOperationIsDelegated (Action<IDataManagementCommand> action)
    {
      _decoratedCommandMock.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);
      _decoratedCommandMock.Expect (action);
      _mockRepository.ReplayAll();

      action (_decorator);

      _mockRepository.VerifyAll ();
    }
  }
}