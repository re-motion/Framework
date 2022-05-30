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
  public class CompositeCommandTest
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
      var compositeCommand = CreateComposite();
      Assert.That(compositeCommand.GetNestedCommands(), Is.EqualTo(new[] { _commandMock1.Object, _commandMock2.Object, _commandMock3.Object }));
    }

    [Test]
    public void GetAllExceptions_None ()
    {
      var compositeCommand = CreateComposite();
      Assert.That(compositeCommand.GetAllExceptions(), Is.Empty);
    }

    [Test]
    public void GetAllExceptions_Some ()
    {
      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.That(nonExecutableComposite.GetAllExceptions(), Is.EqualTo(new[] { _exception1, _exception2, _exception3 }));
    }

    [Test]
    public void Perform ()
    {
      var sequence = new MockSequence();
      _commandMock1.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock3.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();

      var compositeCommand = CreateComposite();
      compositeCommand.Perform();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.Throws<Exception>(nonExecutableComposite.Perform);

      _commandMock1.Verify(mock => mock.Perform(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Perform(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Perform(), Times.Never());
    }

    [Test]
    public void Begin ()
    {
      var sequence = new MockSequence();
      _commandMock1.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock2.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock3.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();

      var compositeCommand = CreateComposite();
      compositeCommand.Begin();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
    }

    [Test]
    public void Begin_NonExecutable ()
    {
      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.Throws<Exception>(nonExecutableComposite.Begin);

      _commandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Begin(), Times.Never());
    }


    [Test]
    public void End ()
    {
      var sequence = new MockSequence();
      _commandMock3.InSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _commandMock2.InSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _commandMock1.InSequence(sequence).Setup(mock => mock.End()).Verifiable();

      var compositeCommand = CreateComposite();
      compositeCommand.End();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
    }

    [Test]
    public void End_NonExecutable ()
    {
      var nonExecutableComposite = CreateNonExecutableComposite();
      Assert.Throws<Exception>(nonExecutableComposite.End);

      _commandMock1.Verify(mock => mock.End(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.End(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.End(), Times.Never());
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var expandedStub1A = new Mock<IDataManagementCommand>();
      expandedStub1A.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      var expandedStub1B = new Mock<IDataManagementCommand>();
      expandedStub1B.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      var expandedStub2A = new Mock<IDataManagementCommand>();
      expandedStub2A.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      var expandedStub2B = new Mock<IDataManagementCommand>();
      expandedStub2B.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      var expandedStub3A = new Mock<IDataManagementCommand>();
      expandedStub3A.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);
      var expandedStub3B = new Mock<IDataManagementCommand>();
      expandedStub3B.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      _commandMock1.Setup(mock => mock.ExpandToAllRelatedObjects()).Returns(new ExpandedCommand(expandedStub1A.Object, expandedStub1B.Object)).Verifiable();
      _commandMock2.Setup(mock => mock.ExpandToAllRelatedObjects()).Returns(new ExpandedCommand(expandedStub2A.Object, expandedStub2B.Object)).Verifiable();
      _commandMock3.Setup(mock => mock.ExpandToAllRelatedObjects()).Returns(new ExpandedCommand(expandedStub3A.Object, expandedStub3B.Object)).Verifiable();

      var compositeCommand = CreateComposite();
      var result = compositeCommand.ExpandToAllRelatedObjects();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
      Assert.That(
          result.GetNestedCommands(),
          Is.EqualTo(new[] { expandedStub1A.Object, expandedStub1B.Object, expandedStub2A.Object, expandedStub2B.Object, expandedStub3A.Object, expandedStub3B.Object }));
    }

    [Test]
    public void CombineWith ()
    {
      var otherCommandStub = new Mock<IDataManagementCommand>();
      otherCommandStub.Setup(stub => stub.GetAllExceptions()).Returns(new Exception[0]);

      var compositeCommand = CreateComposite();
      var result = compositeCommand.CombineWith(otherCommandStub.Object);

      Assert.That(result, Is.Not.SameAs(compositeCommand));
      Assert.That(result.GetNestedCommands(), Is.EqualTo(new[] { _commandMock1.Object, _commandMock2.Object, _commandMock3.Object, otherCommandStub.Object }));
      Assert.That(compositeCommand.GetNestedCommands(), Is.EqualTo(new[] { _commandMock1.Object, _commandMock2.Object, _commandMock3.Object }));
    }

    [Test]
    public void CombineWith_NonExecutable ()
    {
      var compositeCommand = CreateComposite();
      var result = compositeCommand.CombineWith(_commandMock1.Object, _nonExecutableCommandMock1.Object, _nonExecutableCommandMock2.Object);

      Assert.That(result, Is.Not.SameAs(compositeCommand));
      Assert.That(
          result.GetNestedCommands(),
          Is.EqualTo(
              new[]
              {
                  _commandMock1.Object, _commandMock2.Object,
                  _commandMock3.Object, _commandMock1.Object,
                  _nonExecutableCommandMock1.Object, _nonExecutableCommandMock2.Object
              }));
      Assert.That(result.GetAllExceptions(), Is.EqualTo(new[] { _exception1, _exception2, _exception3 }));
    }

    [Test]
    public void NotifyAndPerform_IntegrationTest ()
    {
      var sequence = new MockSequence();
      _commandMock1.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock2.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock3.InSequence(sequence).Setup(mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock3.InSequence(sequence).Setup(mock => mock.Perform()).Verifiable();
      _commandMock3.InSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _commandMock2.InSequence(sequence).Setup(mock => mock.End()).Verifiable();
      _commandMock1.InSequence(sequence).Setup(mock => mock.End()).Verifiable();

      var compositeCommand = CreateComposite();
      compositeCommand.NotifyAndPerform();

      _commandMock1.Verify();
      _commandMock2.Verify();
      _commandMock3.Verify();
      _nonExecutableCommandMock1.Verify();
      _nonExecutableCommandMock2.Verify();
    }

    [Test]
    public void NotifyAndPerform_NonExecutable_IntegrationTest ()
    {
      var nonExecutableComposite = CreateNonExecutableComposite();
      var exception = Assert.Throws<Exception>(nonExecutableComposite.NotifyAndPerform);

      Assert.That(exception, Is.SameAs(_exception1));

      _commandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Begin(), Times.Never());

      _commandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Begin(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Begin(), Times.Never());

      _commandMock1.Verify(mock => mock.Perform(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.Perform(), Times.Never());
      _nonExecutableCommandMock2.Verify(mock => mock.Perform(), Times.Never());

      _nonExecutableCommandMock2.Verify(mock => mock.End(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.End(), Times.Never());
      _commandMock1.Verify(mock => mock.End(), Times.Never());

      _nonExecutableCommandMock2.Verify(mock => mock.End(), Times.Never());
      _nonExecutableCommandMock1.Verify(mock => mock.End(), Times.Never());
      _commandMock1.Verify(mock => mock.End(), Times.Never());
    }

    private CompositeCommand CreateComposite ()
    {
      return new CompositeCommand(_commandMock1.Object, _commandMock2.Object, _commandMock3.Object);
    }

    private CompositeCommand CreateNonExecutableComposite ()
    {
      return new CompositeCommand(_commandMock1.Object, _nonExecutableCommandMock1.Object, _nonExecutableCommandMock2.Object);
    }
  }
}
