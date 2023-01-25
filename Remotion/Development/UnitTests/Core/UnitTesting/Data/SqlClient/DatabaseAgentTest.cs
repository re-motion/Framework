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
using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Data.SqlClient
{
  [TestFixture]
  public class DatabaseAgentTest
  {
    private Mock<IDbCommand> _commandMock;
    private Mock<IDbConnection> _connectionStub;

    [SetUp]
    public void SetUp ()
    {
      _connectionStub = new Mock<IDbConnection>();
      _commandMock = new Mock<IDbCommand>(MockBehavior.Strict);

      _connectionStub.Setup(_ => _.CreateCommand()).Returns(_commandMock.Object);
    }

    [Test]
    public void ExecuteScalarCommand ()
    {
      var sequence = new VerifiableSequence();

      SetupCommandExpectations(
          sequence,
          "my command",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteScalar()).Returns("foo").Verifiable());

      TestableDatabaseAgent agent = new TestableDatabaseAgent(_connectionStub.Object);
      object result = agent.ExecuteScalarCommand("my command");
      Assert.That(result, Is.EqualTo("foo"));

      _commandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteCommand_ReturnsCount ()
    {
      var sequence = new VerifiableSequence();

      SetupCommandExpectations(
          sequence,
          "my command",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteNonQuery()).Returns(15).Verifiable());

      TestableDatabaseAgent agent = new TestableDatabaseAgent(_connectionStub.Object);
      int result = agent.ExecuteCommand("my command");
      Assert.That(result, Is.EqualTo(15));

      _commandMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteBatchString ()
    {
      var sequence = new VerifiableSequence();

      SetupCommandExpectations(
          sequence,
          "ABCDEFG",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteNonQuery()).Returns(5).Verifiable());

      TestableDatabaseAgent agent = new TestableDatabaseAgent(_connectionStub.Object);
      int result = agent.ExecuteBatchString(_connectionStub.Object, "ABCDEFG", null);

      _commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void ExecuteBatchString_StringIsSplitted ()
    {
      var sequence = new VerifiableSequence();
      var executeNonQueryResults = new Queue<int>(new[] { 10, 20 });

      SetupCommandExpectations(
          sequence,
          "ABC",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteNonQuery()).Returns(() => executeNonQueryResults.Dequeue()).Verifiable());
      SetupCommandExpectations(
          sequence,
          "GFE",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteNonQuery()).Returns(() => executeNonQueryResults.Dequeue()).Verifiable());

      TestableDatabaseAgent agent = new TestableDatabaseAgent(_connectionStub.Object);
      int result = agent.ExecuteBatchString(_connectionStub.Object, "ABC\nGO\nGFE", null);

      _commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(30));
    }

    [Test]
    public void ExecuteBatchString_EmptyLines ()
    {
      var sequence = new VerifiableSequence();
      var executeNonQueryResults = new Queue<int>(new[] { 10, 20 });

      SetupCommandExpectations(
          sequence,
          "ABC",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteNonQuery()).Returns(() => executeNonQueryResults.Dequeue()).Verifiable());
      SetupCommandExpectations(
          sequence,
          "GFE",
          null,
          () => _commandMock.InVerifiableSequence(sequence).Setup(_ => _.ExecuteNonQuery()).Returns(() => executeNonQueryResults.Dequeue()).Verifiable());

      TestableDatabaseAgent agent = new TestableDatabaseAgent(_connectionStub.Object);
      int result = agent.ExecuteBatchString(_connectionStub.Object, "ABC\n\nGO\n\n\nGFE\n\n", null);

      _commandMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(30));
    }

    private void SetupCommandExpectations (VerifiableSequence sequence, string commandText, IDbTransaction transaction, Action actualCommandExpectation)
    {
      _commandMock.SetupSet(_ => _.CommandType = CommandType.Text).Verifiable();
      _commandMock.SetupSet(_ => _.CommandText = commandText).Verifiable();
      _commandMock.SetupSet(_ => _.Transaction = transaction).Verifiable();

      actualCommandExpectation();
      _commandMock.InVerifiableSequence(sequence).Setup(_ => _.Dispose()).Verifiable();
    }
  }
}
