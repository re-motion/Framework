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
using System.Data;
using System.Data.Common;
using System.Reflection;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbConnectionTest
  {
    private DbConnection _connection;
    private Mock<DbConnection> _innerConnectionMock;
    private Mock<IPersistenceExtension> _extensionMock;

    [SetUp]
    public void SetUp ()
    {
      _innerConnectionMock = new Mock<DbConnection>(MockBehavior.Strict);
      _innerConnectionMock.Protected().Setup("Dispose", [false]); // for Finalizer
      _extensionMock = new Mock<IPersistenceExtension>(MockBehavior.Strict);

      _connection = new TracingDbConnection(_innerConnectionMock.Object, _extensionMock.Object);
    }

    [Test]
    public void ChangeDatabase ()
    {
      string databaseName = "databaseName";
      _innerConnectionMock.Setup(mock => mock.ChangeDatabase(databaseName)).Verifiable();

      _connection.ChangeDatabase(databaseName);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetConnectionString ()
    {
      var connectionString = "connectionString";
      _innerConnectionMock.Setup(mock => mock.ConnectionString).Returns(connectionString).Verifiable();

      var result = _connection.ConnectionString;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(connectionString));
    }

    [Test]
    public void SetConnectionString ()
    {
      var connectionString = "connectionString";
      _innerConnectionMock.SetupSet(_ => _.ConnectionString = connectionString).Verifiable();

      _connection.ConnectionString = connectionString;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetConnectionTimeout ()
    {
      var assumedResult = 10;
      _innerConnectionMock.Setup(mock => mock.ConnectionTimeout).Returns(assumedResult).Verifiable();

      var result = _connection.ConnectionTimeout;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetDatabase ()
    {
      var assumedResult = "database";
      _innerConnectionMock.Setup(mock => mock.Database).Returns(assumedResult).Verifiable();

      var result = _connection.Database;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetState ()
    {
      var assumedResult = new ConnectionState();
      _innerConnectionMock.Setup(mock => mock.State).Returns(assumedResult).Verifiable();

      var result = _connection.State;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var wrappedInstance = ((TracingDbConnection)_connection).WrappedInstance;
      Assert.That(wrappedInstance, Is.EqualTo(_innerConnectionMock.Object));
    }

    [Test]
    public void Open ()
    {
      var sequence = new VerifiableSequence();
      _innerConnectionMock.InVerifiableSequence(sequence).Setup(mock => mock.Open()).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ConnectionOpened(((TracingDbConnection)_connection).ConnectionID)).Verifiable();

      _connection.Open();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Dispose_ConnectionOpen ()
    {
      var sequence = new VerifiableSequence();
      _innerConnectionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID)).Verifiable();

      _connection.Dispose();
      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Close ()
    {
      var sequence = new VerifiableSequence();
      _innerConnectionMock.InVerifiableSequence(sequence).Setup(mock => mock.Close()).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID)).Verifiable();

      _connection.Close();
      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void BeginTransaction ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, isolationLevel)).Verifiable();

      var tracingDbTransaction = ((TracingDbConnection)_connection).BeginTransaction();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransaction_WithIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;

      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", isolationLevel).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, isolationLevel)).Verifiable();

      var tracingDbTransaction = ((TracingDbConnection)_connection).BeginTransaction(isolationLevel);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();

      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void CreateCommand ()
    {
      var commandMock = new Mock<DbCommand>(MockBehavior.Strict);
      commandMock.Protected().Setup("Dispose", [false]); // for Finalizer
      _innerConnectionMock.Protected().Setup<DbCommand>("CreateDbCommand").Returns(commandMock.Object).Verifiable();

      var tracingDbCommand = ((TracingDbConnection)_connection).CreateCommand();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      commandMock.Verify();

      Assert.That(tracingDbCommand.WrappedInstance, Is.EqualTo(commandMock.Object));
      Assert.That(tracingDbCommand.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbCommand.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransaction_CallerIsAbstractBaseClass ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, isolationLevel)).Verifiable();

      var tracingDbTransaction = _connection.BeginTransaction() as TracingDbTransaction ?? throw new InvalidCastException("BeginTransaction did not return TracingDbTransaction");

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransaction_WithIsolationLevel_CallerIsAbstractBaseClass ()
    {
      var isolationLevel = IsolationLevel.Chaos;

      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", isolationLevel).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, isolationLevel)).Verifiable();

      var tracingDbTransaction = _connection.BeginTransaction(isolationLevel) as TracingDbTransaction ?? throw new InvalidCastException("BeginTransaction did not return TracingDbTransaction");

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();

      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void CreateCommand_CallerIsAbstractBaseClass ()
    {
      var commandMock = new Mock<DbCommand>(MockBehavior.Strict);
      commandMock.Protected().Setup("Dispose", [false]); // for Finalizer
      _innerConnectionMock.Protected().Setup<DbCommand>("CreateDbCommand").Returns(commandMock.Object).Verifiable();

      var tracingDbCommand = _connection.CreateCommand() as TracingDbCommand ?? throw new InvalidCastException("CreateCommand did not return TracingDbCommand");

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      commandMock.Verify();

      Assert.That(tracingDbCommand.WrappedInstance, Is.EqualTo(commandMock.Object));
      Assert.That(tracingDbCommand.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbCommand.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransaction_CallerIsInterface ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, isolationLevel)).Verifiable();

      ((IDbConnection)_connection).BeginTransaction();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
    }

    [Test]
    public void BeginTransaction_WithIsolationLevel_CallerIsInterface ()
    {
      var isolationLevel = IsolationLevel.Chaos;

      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", isolationLevel).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, isolationLevel)).Verifiable();

      ((IDbConnection)_connection).BeginTransaction(isolationLevel);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
    }

    [Test]
    public void CreateCommand_CallerIsInterface ()
    {
      var commandMock = new Mock<DbCommand>(MockBehavior.Strict);
      commandMock.Protected().Setup("Dispose", [false]); // for Finalizer
      _innerConnectionMock.Protected().Setup<DbCommand>("CreateDbCommand").Returns(commandMock.Object).Verifiable();

      ((IDbConnection)_connection).CreateCommand();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      commandMock.Verify();
    }

    [Test]
    public void NoFinalizerImplemented ()
    {
      var type = typeof(TracingDbConnection);
      var finalizer = type.GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
      Assert.That(finalizer?.DeclaringType, Is.Not.EqualTo(typeof(object)));
    }
  }
}
