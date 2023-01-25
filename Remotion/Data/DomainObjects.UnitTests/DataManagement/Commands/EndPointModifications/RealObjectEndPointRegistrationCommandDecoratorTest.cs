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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RealObjectEndPointRegistrationCommandDecoratorTest
  {

    private Mock<IDataManagementCommand> _decoratedCommandMock;
    private Mock<IRealObjectEndPoint> _realObjectEndPointStub;
    private Mock<IVirtualEndPoint> _oldRelatedEndPointMock;
    private Mock<IVirtualEndPoint> _newRelatedEndPointMock;

    private RealObjectEndPointRegistrationCommandDecorator _decorator;

    private Exception _exception1;
    private Exception _exception2;

    [SetUp]
    public void SetUp ()
    {
      _decoratedCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _realObjectEndPointStub = new Mock<IRealObjectEndPoint>();
      _oldRelatedEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);
      _newRelatedEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);

      _decorator = new RealObjectEndPointRegistrationCommandDecorator(
          _decoratedCommandMock.Object,
          _realObjectEndPointStub.Object,
          _oldRelatedEndPointMock.Object,
          _newRelatedEndPointMock.Object);

      _exception1 = new Exception("1");
      _exception2 = new Exception("2");
    }

    [Test]
    public void GetAllExceptions ()
    {
      _decoratedCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1, _exception2 });

      var result = _decorator.GetAllExceptions();

      Assert.That(result, Is.EqualTo(new[] { _exception1, _exception2 }));
    }

    [Test]
    public void Perform ()
    {
      _decoratedCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      var sequence = new VerifiableSequence();
      _oldRelatedEndPointMock.InVerifiableSequence(sequence).Setup(mock => mock.UnregisterCurrentOppositeEndPoint(_realObjectEndPointStub.Object)).Verifiable();
      _decoratedCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _newRelatedEndPointMock.InVerifiableSequence(sequence).Setup(mock => mock.RegisterCurrentOppositeEndPoint(_realObjectEndPointStub.Object)).Verifiable();

      _decorator.Perform();

      _decoratedCommandMock.Verify();
      _oldRelatedEndPointMock.Verify();
      _newRelatedEndPointMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      _decoratedCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1 });

      var exception = Assert.Throws<Exception>(_decorator.Perform);
      Assert.That(exception, Is.SameAs(_exception1));

      _decoratedCommandMock.Verify();
      _oldRelatedEndPointMock.Verify();
      _newRelatedEndPointMock.Verify();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var fakeExpandedCommand = new ExpandedCommand();
      _decoratedCommandMock
          .Setup(stub => stub.ExpandToAllRelatedObjects())
          .Returns(fakeExpandedCommand);

      var result = _decorator.ExpandToAllRelatedObjects();

      Assert.That(result, Is.Not.Null);

      var nestedCommands = result.GetNestedCommands();
      Assert.That(nestedCommands.Count, Is.EqualTo(1));
      Assert.That(nestedCommands[0], Is.TypeOf(typeof(RealObjectEndPointRegistrationCommandDecorator)));

      var innerExpandedCommand = (RealObjectEndPointRegistrationCommandDecorator)nestedCommands[0];
      Assert.That(innerExpandedCommand.DecoratedCommand, Is.SameAs(fakeExpandedCommand));
      Assert.That(innerExpandedCommand.RealObjectEndPoint, Is.SameAs(_realObjectEndPointStub.Object));
      Assert.That(innerExpandedCommand.OldRelatedEndPoint, Is.SameAs(_oldRelatedEndPointMock.Object));
      Assert.That(innerExpandedCommand.NewRelatedEndPoint, Is.SameAs(_newRelatedEndPointMock.Object));
    }

    [Test]
    public void DelegatingMembers ()
    {
      CheckOperationIsDelegated(c => c.Begin());
      CheckOperationIsDelegated(c => c.End());
      CheckOperationIsDelegated(c => c.Begin());
      CheckOperationIsDelegated(c => c.End());
    }

    private void CheckOperationIsDelegated (Expression<Action<IDataManagementCommand>> action)
    {
      _decoratedCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      _decoratedCommandMock.Setup(action).Verifiable();

      var compiledAction = action.Compile();
      compiledAction(_decorator);

      _decoratedCommandMock.Verify();
      _oldRelatedEndPointMock.Verify();
      _newRelatedEndPointMock.Verify();
    }
  }
}
