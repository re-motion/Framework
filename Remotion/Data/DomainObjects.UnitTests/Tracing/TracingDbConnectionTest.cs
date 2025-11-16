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
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using IsolationLevel = System.Data.IsolationLevel;

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
    public void ChangeDatabaseAsync ()
    {
      var databaseName = "databaseName";
      var token = CancellationToken.None;
      var assumedResult = Task.FromException(new InvalidOperationException("Should not get called."));
      _innerConnectionMock.Setup(mock => mock.ChangeDatabaseAsync(databaseName, token)).Returns(assumedResult).Verifiable();

      var result = _connection.ChangeDatabaseAsync(databaseName, token);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
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
    public void StateChange_Add ()
    {
      var handlerMock = new Mock<StateChangeEventHandler>(MockBehavior.Strict);
      _innerConnectionMock.SetupAdd(mock => mock.StateChange += It.IsAny<StateChangeEventHandler>()).Verifiable();

      _connection.StateChange += handlerMock.Object;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void StateChange_Remove ()
    {
      var stateChangeEventHandlerMock = new Mock<StateChangeEventHandler>(MockBehavior.Strict);
      _innerConnectionMock.SetupRemove(mock => mock.StateChange -= It.IsAny<StateChangeEventHandler>()).Verifiable();

      _connection.StateChange -= stateChangeEventHandlerMock.Object;

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
    public void GetDataSource ()
    {
      var assumedResult = "data source";
      _innerConnectionMock.Setup(mock => mock.DataSource).Returns(assumedResult).Verifiable();

      var result = _connection.DataSource;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetServerVersion ()
    {
      var assumedResult = "server version";
      _innerConnectionMock.Setup(mock => mock.ServerVersion).Returns(assumedResult).Verifiable();

      var result = _connection.ServerVersion;

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
    public void GetCanCreateBatch ()
    {
      var assumedResult = true;
      _innerConnectionMock.Setup(mock => mock.CanCreateBatch).Returns(assumedResult).Verifiable();

      var result = _connection.CanCreateBatch;

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
    public async Task OpenAsync ()
    {
      var token = CancellationToken.None;
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = taskCompletionSource.Task;
      var sequence = new VerifiableSequence();

      _innerConnectionMock.InVerifiableSequence(sequence).Setup(mock => mock.OpenAsync(token)).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ConnectionOpened(((TracingDbConnection)_connection).ConnectionID)).Verifiable();

      var task = _connection.OpenAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.ConnectionOpened(((TracingDbConnection)_connection).ConnectionID), Times.Never());

      taskCompletionSource.SetResult();
      await task;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
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
    public void Dispose_NotDisposing ()
    {
      PrivateInvoke.InvokeNonPublicMethod(_connection, "Dispose", false);

      _innerConnectionMock.Protected().Verify("Dispose", Times.Never(), [It.IsAny<bool>()]);
      _extensionMock.Verify(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID), Times.Never);
    }

    [Test]
    public async Task DisposeAsync ()
    {
      var sequence = new VerifiableSequence();
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = new ValueTask(taskCompletionSource.Task);
      _innerConnectionMock.InVerifiableSequence(sequence).Setup(mock => mock.DisposeAsync()).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID)).Verifiable();

      var task = _connection.DisposeAsync();
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID), Times.Never());
      taskCompletionSource.SetResult();
      await task;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
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
    public async Task CloseAsync ()
    {
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = taskCompletionSource.Task;
      var sequence = new VerifiableSequence();

      _innerConnectionMock.InVerifiableSequence(sequence).Setup(mock => mock.CloseAsync()).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID)).Verifiable();

      var task = _connection.CloseAsync();
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.ConnectionClosed(((TracingDbConnection)_connection).ConnectionID), Times.Never());
      taskCompletionSource.SetResult();
      await task;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void EnlistTransaction ()
    {
      var param = new CommittableTransaction();
      _innerConnectionMock.Setup(mock => mock.EnlistTransaction(param)).Verifiable();

      _connection.EnlistTransaction(param);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetSchema ()
    {
      var assumedResult = new DataTable();

      _innerConnectionMock.Setup(mock => mock.GetSchema()).Returns(assumedResult).Verifiable();

      var result = _connection.GetSchema();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetSchema_WithCollectionName ()
    {
      var param = "collection name";
      var assumedResult = new DataTable();

      _innerConnectionMock.Setup(mock => mock.GetSchema(param)).Returns(assumedResult).Verifiable();

      var result = _connection.GetSchema(param);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetSchema_WithCollectionName_AndRestrictionValues ()
    {
      var param = "collection name";
      var param2 = new[] { "asdf", "qwerty" };

      var assumedResult = new DataTable();

      _innerConnectionMock.Setup(mock => mock.GetSchema(param, param2)).Returns(assumedResult).Verifiable();

      var result = _connection.GetSchema(param, param2);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetSchemaAsync ()
    {
      var token = CancellationToken.None;
      var assumedResult = Task.FromException<DataTable>(new InvalidOperationException("Should not get called."));

      _innerConnectionMock.Setup(mock => mock.GetSchemaAsync(token)).Returns(assumedResult).Verifiable();

      var result = _connection.GetSchemaAsync(token);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetSchemaAsync_WithCollectionName ()
    {
      var token = CancellationToken.None;
      var param = "collection name";
      var assumedResult = Task.FromException<DataTable>(new InvalidOperationException("Should not get called."));

      _innerConnectionMock.Setup(mock => mock.GetSchemaAsync(param, token)).Returns(assumedResult).Verifiable();

      var result = _connection.GetSchemaAsync(param, token);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetSchemaAsync_WithCollectionName_AndRestrictionValues ()
    {
      var token = CancellationToken.None;
      var param = "collection name";
      var param2 = new[] { "asdf", "qwerty" };

      var assumedResult = Task.FromException<DataTable>(new InvalidOperationException("Should not get called."));

      _innerConnectionMock.Setup(mock => mock.GetSchemaAsync(param, param2, token)).Returns(assumedResult).Verifiable();

      var result = _connection.GetSchemaAsync(param, param2, token);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void CreateCommandWithNewImplementation ()
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
    public void CreateCommand ()
    {
      var commandMock = new Mock<DbCommand>(MockBehavior.Strict);
      commandMock.Protected().Setup("Dispose", [false]); // for Finalizer
      _innerConnectionMock.Protected().Setup<DbCommand>("CreateDbCommand").Returns(commandMock.Object).Verifiable();

      var dbCommand = _connection.CreateCommand();
      Assert.That(dbCommand, Is.InstanceOf<TracingDbCommand>());
      var tracingDbCommand = (TracingDbCommand)dbCommand;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      commandMock.Verify();

      Assert.That(tracingDbCommand.WrappedInstance, Is.EqualTo(commandMock.Object));
      Assert.That(tracingDbCommand.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbCommand.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void CreateCommandWithInterfaceImplementation ()
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
    public void BeginTransactionWithNewImplementation ()
    {
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.Chaos).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.Chaos)).Verifiable();

      var tracingDbTransaction = ((TracingDbConnection)_connection).BeginTransaction();

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransaction_WithNewImplementationAndIsolationLevel ()
    {

      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.ReadCommitted).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Serializable).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.ReadCommitted)).Verifiable();

      var tracingDbTransaction = ((TracingDbConnection)_connection).BeginTransaction(IsolationLevel.Serializable);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();

      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginDbTransaction ()
    {
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.ReadUncommitted).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.ReadUncommitted)).Verifiable();

      var dbTransaction = _connection.BeginTransaction();
      Assert.That(dbTransaction, Is.InstanceOf<TracingDbTransaction>());
      var tracingDbTransaction = (TracingDbTransaction)dbTransaction;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginDbTransaction_WithIsolationLevel ()
    {
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.Snapshot).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.RepeatableRead).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.Snapshot)).Verifiable();

      var dbTransaction = _connection.BeginTransaction(IsolationLevel.RepeatableRead);
      Assert.That(dbTransaction, Is.InstanceOf<TracingDbTransaction>());
      var tracingDbTransaction = (TracingDbTransaction)dbTransaction;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();

      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginDbTransactionExplicitIinterfaceImplementation ()
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
    public void BeginDbTransactionExplicitInterfaceImplementationWithIsolationLevel ()
    {
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.Chaos).Verifiable();

      _innerConnectionMock.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Serializable).Returns(dbTransactionMock.Object).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.Chaos)).Verifiable();

      ((IDbConnection)_connection).BeginTransaction(IsolationLevel.Serializable);

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
    }

    [Test]
    public async Task BeginTransactionAsync ()
    {
      var token = CancellationToken.None;
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      var taskCompletionSource = new TaskCompletionSource<DbTransaction>();
      var assumedResult = new ValueTask<DbTransaction>(taskCompletionSource.Task);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.Chaos).Verifiable();
      _innerConnectionMock.Protected().Setup<ValueTask<DbTransaction>>("BeginDbTransactionAsync", IsolationLevel.Unspecified, token).Returns(assumedResult).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.Chaos)).Verifiable();

      var task = _connection.BeginTransactionAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.Chaos), Times.Never);
      taskCompletionSource.SetResult(dbTransactionMock.Object);
      var dbTransaction = await task;
      Assert.That(dbTransaction, Is.InstanceOf<TracingDbTransaction>());
      var tracingDbTransaction = (TracingDbTransaction)dbTransaction;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();
      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public async Task BeginTransactionAsync_WithIsolationLevel ()
    {
      var token = CancellationToken.None;
      var dbTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      var taskCompletionSource = new TaskCompletionSource<DbTransaction>();
      var assumedResult = new ValueTask<DbTransaction>(taskCompletionSource.Task);
      dbTransactionMock.Setup(mock => mock.IsolationLevel).Returns(IsolationLevel.ReadCommitted).Verifiable();

      _innerConnectionMock.Protected().Setup<ValueTask<DbTransaction>>("BeginDbTransactionAsync", IsolationLevel.Serializable, token).Returns(assumedResult).Verifiable();
      _extensionMock.Setup(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.ReadCommitted)).Verifiable();

      var task = _connection.BeginTransactionAsync(IsolationLevel.Serializable, token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.TransactionBegan(((TracingDbConnection)_connection).ConnectionID, IsolationLevel.ReadCommitted), Times.Never);
      taskCompletionSource.SetResult(dbTransactionMock.Object);
      var dbTransaction = await task;
      Assert.That(dbTransaction, Is.InstanceOf<TracingDbTransaction>());
      var tracingDbTransaction = (TracingDbTransaction)dbTransaction;

      _innerConnectionMock.Verify();
      _extensionMock.Verify();
      dbTransactionMock.Verify();

      Assert.That(tracingDbTransaction.WrappedInstance, Is.EqualTo(dbTransactionMock.Object));
      Assert.That(tracingDbTransaction.PersistenceExtension, Is.EqualTo(_extensionMock.Object));
      Assert.That(tracingDbTransaction.ConnectionID, Is.EqualTo(((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void DbProviderFactory ()
    {
      var looseInnerConnectionMock = new Mock<DbConnection>(MockBehavior.Loose);
      looseInnerConnectionMock.Protected().Setup("Dispose", [false]); // for Finalizer
      var looseExtensionMock = new Mock<IPersistenceExtension>(MockBehavior.Loose);
      var looseConnection = new TracingDbConnection(looseInnerConnectionMock.Object, looseExtensionMock.Object);

      Assert.DoesNotThrow(() => DbProviderFactories.GetFactory(looseConnection));
    }

    [Test]
    public void TestFinalizerImplemented ()
    {
      TracingTestHelper.AssertFinalizerImplemented(typeof(TracingDbConnection));
    }

    [Test]
    public void TestAllVirtualMethodsOverridden ()
    {
      TracingTestHelper.AssertAllVirtualMethodsOverridden(typeof(TracingDbConnection));
    }
  }
}
