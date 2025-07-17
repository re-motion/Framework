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
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbCommandTest
  {
    private DbCommand _command;
    private Mock<DbCommand> _innerCommandMock;
    private Mock<IPersistenceExtension> _extensionMock;
    private Guid _connectionID;

    [SetUp]
    public void SetUp ()
    {
      _innerCommandMock = new Mock<DbCommand>(MockBehavior.Strict);
      _innerCommandMock.Protected().Setup("Dispose", [false]); // for Finalizer
      _extensionMock = new Mock<IPersistenceExtension>(MockBehavior.Strict);
      _connectionID = Guid.NewGuid();

      _command = new TracingDbCommand(_innerCommandMock.Object, _extensionMock.Object, _connectionID);
    }

    [Test]
    public void Dispose ()
    {
      _innerCommandMock.Protected().Setup("Dispose", [true]).Verifiable();

      _command.Dispose();

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Dispose_NotDisposing ()
    {
      PrivateInvoke.InvokeNonPublicMethod(_command, "Dispose", false);

      _innerCommandMock.Protected().Verify("Dispose", Times.Never(), [It.IsAny<bool>()]);
      _extensionMock.Verify();
    }

    [Test]
    public void DisposeAsync ()
    {
      var assumedResult = ValueTask.FromException(new InvalidOperationException("Should not get called."));
      _innerCommandMock.Setup(mock => mock.DisposeAsync()).Returns(assumedResult).Verifiable();

      var result = _command.DisposeAsync();

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      Assert.That(result, Is.EqualTo(assumedResult));
    }

    [Test]
    public void Prepare ()
    {
      _innerCommandMock.Setup(mock => mock.Prepare()).Verifiable();

      _command.Prepare();

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void Cancel ()
    {
      _innerCommandMock.Setup(mock => mock.Cancel()).Verifiable();

      _command.Cancel();

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void CreateParameter ()
    {
      var parameterStub = new Mock<DbParameter>();
      _innerCommandMock.Protected().Setup<DbParameter>("CreateDbParameter").Returns(parameterStub.Object);

      Assert.That(_command.CreateParameter(), Is.SameAs(parameterStub.Object));
    }

    [Test]
    public void GetConnectionFromInterface ()
    {
      var connectionStub = new Mock<DbConnection>();
      _innerCommandMock.Protected().Setup<DbConnection>("DbConnection").Returns(connectionStub.Object);

      Assert.That(((IDbCommand)_command).Connection, Is.SameAs(connectionStub.Object));
    }

    [Test]
    public void SetConnectionFromInterface ()
    {
      var connectionStub = new Mock<DbConnection>();
      _innerCommandMock.SetupSet(mock => mock.Connection = connectionStub.Object).Verifiable();

      ((IDbCommand)_command).Connection = connectionStub.Object;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerConnection_WithInstance ()
    {
      var connectionStub = new Mock<DbConnection>();
      _innerCommandMock.SetupSet(mock => mock.Connection = connectionStub.Object).Verifiable();

      ((TracingDbCommand)_command).SetInnerConnection(new TracingDbConnection(connectionStub.Object, new Mock<IPersistenceExtension>().Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerConnection_WithNull ()
    {
      _innerCommandMock.SetupSet(mock => mock.Connection = null).Verifiable();

      ((TracingDbCommand)_command).SetInnerConnection(null);

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetTransactionFromInterface ()
    {
      var transactionStub = new Mock<DbTransaction>();
      _innerCommandMock.Protected().Setup<DbTransaction>("DbTransaction").Returns(transactionStub.Object);

      Assert.That(((IDbCommand)_command).Transaction, Is.SameAs(transactionStub.Object));
    }

    [Test]
    public void SetTransactionFromInterface ()
    {
      var transactionStub = new Mock<DbTransaction>();
      _innerCommandMock.SetupSet(mock => mock.Transaction = transactionStub.Object).Verifiable();

      ((IDbCommand)_command).Transaction = transactionStub.Object;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerTransaction_WithInstance ()
    {
      var transactionStub = new Mock<DbTransaction>();
      _innerCommandMock.SetupSet(mock => mock.Transaction = transactionStub.Object).Verifiable();

      ((TracingDbCommand)_command).SetInnerTransaction(new TracingDbTransaction(transactionStub.Object, new Mock<IPersistenceExtension>().Object, Guid.NewGuid()));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerTransaction_WithNull ()
    {
      _innerCommandMock.SetupSet(mock => mock.Connection = null).Verifiable();

      ((TracingDbCommand)_command).SetInnerConnection(null);

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetCommandText ()
    {
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      Assert.That(_command.CommandText, Is.EqualTo("commandText"));
    }

    [Test]
    public void SetCommandText ()
    {
      _innerCommandMock.SetupSet(mock => mock.CommandText = "commandText").Verifiable();

      _command.CommandText = "commandText";

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetCommandTimeout ()
    {
      _innerCommandMock.Setup(mock => mock.CommandTimeout).Returns(100);

      Assert.That(_command.CommandTimeout, Is.EqualTo(100));
    }

    [Test]
    public void SetCommandTimeout ()
    {
      _innerCommandMock.SetupSet(mock => mock.CommandTimeout = 100).Verifiable();

      _command.CommandTimeout = 100;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetCommandType ()
    {
      _innerCommandMock.Setup(mock => mock.CommandType).Returns(CommandType.TableDirect);
      Assert.That(_command.CommandType, Is.EqualTo(CommandType.TableDirect));
    }

    [Test]
    public void SetCommandType ()
    {
      _innerCommandMock.SetupSet(mock => mock.CommandType = CommandType.TableDirect).Verifiable();

      _command.CommandType = CommandType.TableDirect;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetParameters ()
    {
      var collectionStub = new Mock<DbParameterCollection>();
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(collectionStub.Object);
      Assert.That(_command.Parameters, Is.SameAs(collectionStub.Object));
    }

    [Test]
    public void GetUpdatedRowSource ()
    {
      _innerCommandMock.Setup(mock => mock.UpdatedRowSource).Returns(UpdateRowSource.FirstReturnedRecord);
      Assert.That(_command.UpdatedRowSource, Is.EqualTo(UpdateRowSource.FirstReturnedRecord));
    }

    [Test]
    public void SetUpdatedRowSource ()
    {
      _innerCommandMock.SetupSet(mock => mock.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord).Verifiable();

      _command.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void ExecuteNonQuery ()
    {
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());
      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteNonQuery()).Returns(100).Verifiable();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryCompleted(_connectionID, ((TracingDbCommand)_command).QueryID, TimeSpan.Zero, 100)).Verifiable();

      Assert.That(_command.ExecuteNonQuery(), Is.EqualTo(100));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteNonQuery_WithError ()
    {
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteNonQuery()).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteNonQuery(), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task ExecuteNonQueryAsync ()
    {
      var token = CancellationToken.None;
      var taskCompletionSource = new TaskCompletionSource<int>();
      var assumedResult = taskCompletionSource.Task;
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());
      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteNonQueryAsync(token)).Returns(assumedResult).Verifiable();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryCompleted(_connectionID, ((TracingDbCommand)_command).QueryID, TimeSpan.Zero, 100)).Verifiable();

      var task = _command.ExecuteNonQueryAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.QueryCompleted(_connectionID, ((TracingDbCommand)_command).QueryID, TimeSpan.Zero, 100), Times.Never);

      taskCompletionSource.SetResult(100);
      await task;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task ExecuteNonQueryAsync_WithError ()
    {
      var token = CancellationToken.None;
      var exception = new Exception("TestException");
      var taskCompletionSource = new TaskCompletionSource<int>();
      var assumedResult = taskCompletionSource.Task;
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteNonQueryAsync(token)).Returns(assumedResult).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      var task = _command.ExecuteNonQueryAsync(token);

      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception), Times.Never);
      taskCompletionSource.SetException(exception);
      await Assert.ThatAsync(() => task, Throws.Exception.SameAs(exception));
      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderWithNewImplementation ()
    {
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Protected().Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.Default).Returns(readerStub.Object).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      var actualReader = ((TracingDbCommand)_command).ExecuteReader();

      Assert.That(actualReader.WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(actualReader.ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(actualReader.QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(actualReader.PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderWithNewImplementationAndCommandBehavior ()
    {
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock
          .InVerifiableSequence(sequence)
          .Protected()
          .Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.SchemaOnly)
          .Returns(readerStub.Object)
          .Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      var actualReader = ((TracingDbCommand)_command).ExecuteReader(CommandBehavior.SchemaOnly);

      Assert.That(actualReader.WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(actualReader.ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(actualReader.QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(actualReader.PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReader_WithError ()
    {
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Protected().Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.Default).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteReader(), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderWithOverload_WithError ()
    {
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock
          .InVerifiableSequence(sequence)
          .Protected()
          .Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.SchemaOnly)
          .Throws(exception)
          .Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteReader(CommandBehavior.SchemaOnly), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReader ()
    {
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Protected().Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.Default).Returns(readerStub.Object).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      DbDataReader actualReader = _command.ExecuteReader();

      Assert.That(actualReader, Is.InstanceOf(typeof(TracingDataReader)));
      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderWithCommandBehavior ()
    {
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock
          .InVerifiableSequence(sequence)
          .Protected()
          .Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.SchemaOnly)
          .Returns(readerStub.Object)
          .Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      DbDataReader actualReader = _command.ExecuteReader(CommandBehavior.SchemaOnly);

      Assert.That(actualReader, Is.InstanceOf(typeof(TracingDataReader)));
      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderExplicitInterfaceImplementation ()
    {
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Protected().Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.Default).Returns(readerStub.Object).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      var actualReader = ((IDbCommand)_command).ExecuteReader();
      Assert.That(actualReader, Is.InstanceOf<DbDataReader>());
      Assert.That(actualReader, Is.InstanceOf<TracingDataReader>());

      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderExplicitInterfaceImplementationWithCommandBehavior ()
    {
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock
          .InVerifiableSequence(sequence)
          .Protected()
          .Setup<DbDataReader>("ExecuteDbDataReader", CommandBehavior.SchemaOnly)
          .Returns(readerStub.Object)
          .Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      var actualReader = ((IDbCommand)_command).ExecuteReader(CommandBehavior.SchemaOnly);
      Assert.That(actualReader, Is.InstanceOf<DbDataReader>());
      Assert.That(actualReader, Is.InstanceOf<TracingDataReader>());

      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task ExecuteReaderAsync ()
    {
      var token = CancellationToken.None;
      var readerStub = new Mock<DbDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock
          .InVerifiableSequence(sequence)
          .Protected()
          .Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.Default, token)
          .Returns(Task.FromResult(readerStub.Object)).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      DbDataReader actualReader = await _command.ExecuteReaderAsync(token);

      Assert.That(actualReader, Is.InstanceOf(typeof(TracingDataReader)));
      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(((TracingDbCommand)_command).QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderAsync_WithError ()
    {
      var token = CancellationToken.None;
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Protected().Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.Default, token).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteReaderAsync(token), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteScalar ()
    {
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteScalar()).Returns(30).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryCompleted(_connectionID, ((TracingDbCommand)_command).QueryID, TimeSpan.Zero, 1)).Verifiable();

      Assert.That(_command.ExecuteScalar(), Is.EqualTo(30));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteScalar_WithError ()
    {
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteScalar()).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteScalar(), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task ExecuteScalarAsync ()
    {
      var taskCompletionSource = new TaskCompletionSource<object>();
      var assumedResult = taskCompletionSource.Task;
      var token = CancellationToken.None;

      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteScalarAsync(token)).Returns(assumedResult).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, ((TracingDbCommand)_command).QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryCompleted(_connectionID, ((TracingDbCommand)_command).QueryID, TimeSpan.Zero, 1)).Verifiable();

      var task = _command.ExecuteScalarAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.QueryCompleted(_connectionID, ((TracingDbCommand)_command).QueryID, TimeSpan.Zero, 1), Times.Never);
      taskCompletionSource.SetResult(33);
      await task;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public async Task ExecuteScalarAsync_WithError ()
    {
      var token = CancellationToken.None;
      var taskCompletionSource = new TaskCompletionSource<object>();
      var assumedResult = taskCompletionSource.Task;
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, ((TracingDbCommand)_command).QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteScalarAsync(token)).Returns(assumedResult).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception)).Verifiable();

      var task = _command.ExecuteScalarAsync(token);
      Assert.That(task, Is.Not.EqualTo(assumedResult));
      _extensionMock.Verify(mock => mock.QueryError(_connectionID, ((TracingDbCommand)_command).QueryID, exception), Times.Never);
      taskCompletionSource.SetException(exception);

      await Assert.ThatAsync(()=> task, Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetDesignTimeVisible ()
    {
      _innerCommandMock.Setup(mock => mock.DesignTimeVisible).Returns(true);
      Assert.That(_command.DesignTimeVisible, Is.EqualTo(true));
    }

    [Test]
    public void SetDesignTimeVisible ()
    {
      _innerCommandMock.SetupSet(mock => mock.DesignTimeVisible = true).Verifiable();

      _command.DesignTimeVisible = true;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetConnection ()
    {
      var connection = new SqlConnection();
      _innerCommandMock.Protected().Setup<DbConnection>("DbConnection").Returns(connection);
      Assert.That(_command.Connection, Is.EqualTo(connection));
    }

    [Test]
    public void SetConnection ()
    {
      var connection = new SqlConnection();

      _innerCommandMock.SetupSet(mock => mock.Connection = connection).Verifiable();

      _command.Connection = connection;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetTransaction ()
    {
      var transaction = new Mock<DbTransaction>().Object; // simulate DbTransaction
      _innerCommandMock.Protected().Setup<DbTransaction>("DbTransaction").Returns(transaction);
      Assert.That(_command.Transaction, Is.EqualTo(transaction));
    }

    [Test]
    public void SetTransaction ()
    {
      var transaction = new Mock<DbTransaction>().Object; // simulate DbTransaction

      _innerCommandMock.SetupSet(mock => mock.Transaction = transaction).Verifiable();

      _command.Transaction = transaction;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }


    [Test]
    public void TestFinalizerImplemented ()
    {
      TracingTestHelper.AssertFinalizerImplemented(typeof(TracingDbCommand));
    }

    [Test]
    public void TestAllVirtualMethodsOverridden ()
    {
      TracingTestHelper.AssertAllVirtualMethodsOverridden(typeof(TracingDbCommand));
    }

    private DbParameterCollection CreateParameterCollection ()
    {
      var command = new SqlCommand();
      return command.Parameters;
    }
  }
}
