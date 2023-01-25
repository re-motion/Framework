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

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class ExpandedCommandTest
  {

    private Mock<IDataManagementCommand> _commandMock1;
    private Mock<IDataManagementCommand> _commandMock2;
    private Mock<IDataManagementCommand> _commandMock3;

    private Mock<IDataManagementCommand> _nonExecutableCommandMock1;
    private Mock<IDataManagementCommand> _nonExecutableCommandMock2;

    private Exception _exception1;
    private Exception _exception2;
    private Exception _exception3;

    [SetUp]
    public void SetUp ()
    {
      _commandMock1 = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _commandMock1.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      _commandMock2 = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _commandMock2.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      _commandMock3 = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _commandMock3.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      _nonExecutableCommandMock1 = new Mock<IDataManagementCommand>(MockBehavior.Strict);
      _nonExecutableCommandMock2 = new Mock<IDataManagementCommand>(MockBehavior.Strict);

      _exception1 = new Exception("1");
      _exception2 = new Exception("2");
      _exception3 = new Exception("3");

      _nonExecutableCommandMock1.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception1, _exception2 });
      _nonExecutableCommandMock2.Setup(stub => stub.GetAllExceptions()).Returns(new[] { _exception3 });
    }

    [Test]
    public void GetNestedCommands ()
    {
      var expandedCommand = CreateExpandedCommand();
      Assert.That(expandedCommand.GetNestedCommands(), Is.EqualTo(new[] { _commandMock1.Object, _commandMock2.Object, _commandMock3.Object }));
    }

    [Test]
    public void GetAllExceptions_None ()
    {
      var expandedCommand = CreateExpandedCommand();
      Assert.That(expandedCommand.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void GetAllExceptions_Some ()
    {
      var expandedCommand = CreateNonExecutableExpandedCommand();
      Assert.That(expandedCommand.GetAllExceptions(), Is.EqualTo(new[] { _exception1, _exception2, _exception3 }));
    }

    [Test]
    public void Perform ()
    {
      var sequence = new VerifiableSequence();
      _commandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock2.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock3.InVerifiableSequence(sequence).Setup(mock => mock.Perform()).Verifiable();

      var compositeCommand = CreateExpandedCommand();
      compositeCommand.Perform();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
      sequence.Verify();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      var nonExecutableComposite = CreateNonExecutableExpandedCommand();
      Assert.Throws<Exception>(nonExecutableComposite.Perform);

      _commandMock1.Verify(mock => mock.Perform(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Perform(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Perform(), Times.Never());
    }

    [Test]
    public void Begin ()
    {
      var sequence = new VerifiableSequence();
      _commandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock2.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock3.InVerifiableSequence(sequence).Setup(mock => mock.Begin()).Verifiable();

      var compositeCommand = CreateExpandedCommand();
      compositeCommand.Begin();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
      sequence.Verify();
    }

    [Test]
    public void Begin_NonExecutable ()
    {
      var nonExecutableComposite = CreateNonExecutableExpandedCommand();
      Assert.Throws<Exception>(nonExecutableComposite.Begin);

      _commandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Begin(), Times.Never());
    }


    [Test]
    public void End ()
    {
      var sequence = new VerifiableSequence();
      _commandMock3.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _commandMock2.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _commandMock1.InVerifiableSequence(sequence).Setup(mock => mock.End()).Verifiable();

      var compositeCommand = CreateExpandedCommand();
      compositeCommand.End();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
      sequence.Verify();
    }

    [Test]
    public void End_NonExecutable ()
    {
      var nonExecutableComposite = CreateNonExecutableExpandedCommand();
      Assert.Throws<Exception>(nonExecutableComposite.End);

      _commandMock1.Verify(mock => mock.End(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.End(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.End(), Times.Never());
    }

    private ExpandedCommand CreateExpandedCommand ()
    {
      return new ExpandedCommand(_commandMock1.Object, _commandMock2.Object, _commandMock3.Object);
    }

    private ExpandedCommand CreateNonExecutableExpandedCommand ()
    {
      return new ExpandedCommand(_commandMock1.Object, _nonExecutableCommandMock1.Object, _nonExecutableCommandMock2.Object);
    }
  }
}
