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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbTransactionTest
  {
    private Mock<DbTransaction> _innerTransactionMock;
    private Mock<IPersistenceExtension> _extensionMock;
    private Guid _connectionID;
    private DbTransaction _transaction;

    [SetUp]
    public void SetUp ()
    {
      _innerTransactionMock = new Mock<DbTransaction>(MockBehavior.Strict);
      _extensionMock = new Mock<IPersistenceExtension>(MockBehavior.Strict);
      _connectionID = Guid.NewGuid();

      _transaction = new TracingDbTransaction(_innerTransactionMock.Object, _extensionMock.Object, _connectionID);
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var result = ((TracingDbTransaction)_transaction).WrappedInstance;

      Assert.That(result, Is.EqualTo(_innerTransactionMock.Object));
    }

    [Test]
    public void GetConnectionID ()
    {
      var result = ((TracingDbTransaction)_transaction).ConnectionID;

      Assert.That(result, Is.EqualTo(_connectionID));
    }

    [Test]
    public void GetTransactionID ()
    {
      var result = ((TracingDbTransaction)_transaction).TransactionID;

      Assert.That(result, Is.TypeOf(typeof(Guid)));
    }

    [Test]
    public void GetPersistenceExtension ()
    {
      var result = ((TracingDbTransaction)_transaction).PersistenceExtension;

      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(_extensionMock.Object));
    }

    [Test]
    public void Dispose ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();

      _transaction.Dispose();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Dispose_DisposedTransaction ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();

      _transaction.Dispose();
      _transaction.Dispose();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void DisposeAsync ()
    {
      var sequence = new VerifiableSequence();
      var assumedResult = ValueTask.FromException(new InvalidOperationException("Should not get called."));

      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.DisposeAsync()).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();

      var result = _transaction.DisposeAsync();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void DisposeAsync_DisposedTransaction ()
    {
      var sequence = new VerifiableSequence();
      var exception = new InvalidOperationException("Should not get called.");
      var firstAssumedResult = ValueTask.FromException(exception);
      var secondAssumedResult = ValueTask.FromException(exception);

      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.DisposeAsync()).Returns(firstAssumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.DisposeAsync()).Returns(secondAssumedResult).Verifiable();

      var result = _transaction.DisposeAsync();
      var result2 = _transaction.DisposeAsync();

      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Commit ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Commit()).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionCommitted(_connectionID)).Verifiable();

      _transaction.Commit();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Commit_DisposedTransaction ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Commit()).Verifiable();

      _transaction.Dispose();
      _transaction.Commit();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task CommitAsync ()
    {
      var token = CancellationToken.None;
      var sequence = new VerifiableSequence();
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = taskCompletionSource.Task;

      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.CommitAsync(token)).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionCommitted(_connectionID)).Verifiable();

      var task = _transaction.CommitAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.TransactionCommitted(_connectionID), Times.Never);
      taskCompletionSource.SetResult();
      await task;

      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task CommitAsync_DisposedTransaction ()
    {
      var token = CancellationToken.None;
      var sequence = new VerifiableSequence();
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = taskCompletionSource.Task;

      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.CommitAsync(token)).Returns(assumedResult).Verifiable();

      _transaction.Dispose();
      var task = _transaction.CommitAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      taskCompletionSource.SetResult();
      await task;

      _innerTransactionMock.Verify();
      _extensionMock.Verify(mock => mock.TransactionCommitted(_connectionID), Times.Never);
      sequence.Verify();
    }

    [Test]
    public void Rollback ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Rollback()).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionRolledBack(_connectionID)).Verifiable();

      _transaction.Rollback();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Rollback_DisposedTransaction ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Rollback()).Verifiable();

      _transaction.Dispose();
      _transaction.Rollback();
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RollbackOverload ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Rollback("someSavepoint")).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionRolledBack(_connectionID)).Verifiable();

      _transaction.Rollback("someSavepoint");
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RollbackOverload_DisposedTransaction ()
    {
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Rollback("someSavepoint")).Verifiable();

      _transaction.Dispose();
      _transaction.Rollback("someSavepoint");
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task RollbackAsync ()
    {
      var token = CancellationToken.None;
      var sequence = new VerifiableSequence();
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = taskCompletionSource.Task;

      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.RollbackAsync(token)).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionRolledBack(_connectionID)).Verifiable();

      var task = _transaction.RollbackAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.TransactionRolledBack(_connectionID), Times.Never);
      taskCompletionSource.SetResult();
      await task;

      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RollbackAsync_DisposedTransaction ()
    {
      var token = CancellationToken.None;
      var sequence = new VerifiableSequence();
      var assumedResult = Task.CompletedTask;

      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.RollbackAsync(token)).Returns(assumedResult).Verifiable();

      _transaction.Dispose();
      var result = _transaction.RollbackAsync(token);
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public async Task RollbackAsyncOverload ()
    {
      var token = CancellationToken.None;
      var sequence = new VerifiableSequence();
      var taskCompletionSource = new TaskCompletionSource();
      var assumedResult = taskCompletionSource.Task;

      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.RollbackAsync("someSavePoint", token)).Returns(assumedResult).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionRolledBack(_connectionID)).Verifiable();

      var task = _transaction.RollbackAsync("someSavePoint", token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.TransactionRolledBack(_connectionID), Times.Never);
      taskCompletionSource.SetResult();
      await task;

      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void RollbackAsyncOverload_DisposedTransaction ()
    {
      var token = CancellationToken.None;
      var sequence = new VerifiableSequence();
      var assumedResult = Task.CompletedTask;

      _innerTransactionMock.InVerifiableSequence(sequence).Protected().Setup("Dispose", [true]).Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.TransactionDisposed(_connectionID)).Verifiable();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.RollbackAsync("someSavepoint", token)).Returns(assumedResult).Verifiable();

      _transaction.Dispose();
      var result = _transaction.RollbackAsync("someSavepoint", token);
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void Release ()
    {
      var param = "someSavepoint";
      var sequence = new VerifiableSequence();
      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release(param)).Verifiable();

      _transaction.Release(param);
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ReleaseAsync ()
    {
      var param = "someSavepoint";
      var sequence = new VerifiableSequence();
      var token = CancellationToken.None;
      var assumedResult = Task.FromException(new InvalidOperationException("Should not get called."));

      _innerTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.ReleaseAsync(param, token)).Returns(assumedResult).Verifiable();

      var result = _transaction.ReleaseAsync(param, token);
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void GetConnection ()
    {
      var dbConnection = new Mock<DbConnection>(MockBehavior.Strict);
      dbConnection.Protected().Setup("Dispose", [false]); // for Finalizer
      _innerTransactionMock.Protected().Setup<DbConnection>("DbConnection").Returns(dbConnection.Object).Verifiable();

      var result = _transaction.Connection;

      Assert.That(result, Is.EqualTo(dbConnection.Object));
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
      dbConnection.Verify();
    }

    [Test]
    public void GetIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      _innerTransactionMock.Setup(mock => mock.IsolationLevel).Returns(isolationLevel).Verifiable();

      var result = _transaction.IsolationLevel;

      Assert.That(result, Is.EqualTo(isolationLevel));
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetSupportsSavepoints ()
    {
      var assumedResult = true;
      _innerTransactionMock.Setup(mock => mock.SupportsSavepoints).Returns(assumedResult).Verifiable();

      var result = _transaction.SupportsSavepoints;

      Assert.That(result, Is.EqualTo(assumedResult));
      _innerTransactionMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void TestNoFinalizerImplemented ()
    {
      TracingTestHelper.AssertNoFinalizerImplemented(typeof(TracingDbTransaction));
    }

    [Test]
    public void TestAllVirtualMethodsOverridden ()
    {
      TracingTestHelper.AssertAllVirtualMethodsOverridden(typeof(TracingDbTransaction));
    }
  }
}
