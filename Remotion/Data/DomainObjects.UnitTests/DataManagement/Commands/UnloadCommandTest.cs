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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class UnloadCommandTest : StandardMappingTest
  {

    private Order _domainObject1;
    private Order _domainObject2;

    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;

    private Mock<IDataManagementCommand> _unloadDataCommandMock;

    private UnloadCommand _unloadCommand;
    private Exception _exception1;
    private Exception _exception2;

    public override void SetUp ()
    {
      base.SetUp();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order>();

      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>(MockBehavior.Strict);

      _unloadDataCommandMock = new Mock<IDataManagementCommand>(MockBehavior.Strict);

      _unloadCommand = new UnloadCommand(
          new[] { _domainObject1, _domainObject2 },
          _unloadDataCommandMock.Object,
          _transactionEventSinkWithMock.Object);

      _exception1 = new Exception("1");
      _exception2 = new Exception("2");
    }

    [Test]
    public void GetAllExceptions ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1, _exception2 });

      Assert.That(_unloadCommand.GetAllExceptions(), Is.EqualTo(new[] { _exception1, _exception2 }));
    }

    [Test]
    public void Begin ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      var sequence = new VerifiableSequence();
      _transactionEventSinkWithMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RaiseObjectsUnloadingEvent(new[] { _domainObject1, _domainObject2 }))
          .Verifiable();
      _unloadDataCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable();

      _unloadCommand.Begin();

      _unloadDataCommandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Begin_NonExecutable ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1 });

      var exception = Assert.Throws<Exception>(_unloadCommand.Begin);
      Assert.That(exception, Is.SameAs(_exception1));

      _unloadDataCommandMock.Verify();
    }

    [Test]
    public void Perform ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      _unloadDataCommandMock.Setup(mock => mock.Perform()).Verifiable();

      _unloadCommand.Perform();

      _unloadDataCommandMock.Verify();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1 });

      var exception = Assert.Throws<Exception>(_unloadCommand.Perform);
      Assert.That(exception, Is.SameAs(_exception1));

      _unloadDataCommandMock.Verify();
    }

    [Test]
    public void End ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      var sequence = new VerifiableSequence();
      _unloadDataCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _transactionEventSinkWithMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.RaiseObjectsUnloadedEvent(new[] { _domainObject1, _domainObject2 }))
          .Verifiable();

      _unloadCommand.End();

      _unloadDataCommandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void End_NonExecutable ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1 });

      var exception = Assert.Throws<Exception>(_unloadCommand.End);
      Assert.That(exception, Is.SameAs(_exception1));

      _unloadDataCommandMock.Verify();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      _unloadDataCommandMock.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      var result = _unloadCommand.ExpandToAllRelatedObjects();

      Assert.That(result.GetNestedCommands(), Is.EqualTo(new[] { _unloadCommand }));
    }
  }
}
