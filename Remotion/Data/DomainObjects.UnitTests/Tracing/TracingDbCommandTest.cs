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
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbCommandTest
  {
    private TracingDbCommand _command;
    private Mock<IDbCommand> _innerCommandMock;
    private Mock<IPersistenceExtension> _extensionMock;
    private Guid _connectionID;

    [SetUp]
    public void SetUp ()
    {
      _innerCommandMock = new Mock<IDbCommand>(MockBehavior.Strict);
      _extensionMock = new Mock<IPersistenceExtension>(MockBehavior.Strict);
      _connectionID = Guid.NewGuid();

      _command = new TracingDbCommand(_innerCommandMock.Object, _extensionMock.Object, _connectionID);
    }

    [Test]
    public void Dispose ()
    {
      _innerCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      _command.Dispose();

      _innerCommandMock.Verify();
      _extensionMock.Verify();
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
      var parameterStub = new Mock<IDbDataParameter>();
      _innerCommandMock.Setup(mock => mock.CreateParameter()).Returns(parameterStub.Object);

      Assert.That(_command.CreateParameter(), Is.SameAs(parameterStub.Object));
    }

    [Test]
    public void GetConnectionFromInterface ()
    {
      var connectionStub = new Mock<IDbConnection>();
      _innerCommandMock.Setup(mock => mock.Connection).Returns(connectionStub.Object);

      Assert.That(((IDbCommand)_command).Connection, Is.SameAs(connectionStub.Object));
    }

    [Test]
    public void SetConnectionFromInterface ()
    {
      var connectionStub = new Mock<IDbConnection>();
      _innerCommandMock.SetupSet(mock => mock.Connection = connectionStub.Object).Verifiable();

      ((IDbCommand)_command).Connection = connectionStub.Object;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerConnection_WithInstance ()
    {
      var connectionStub = new Mock<IDbConnection>();
      _innerCommandMock.SetupSet(mock => mock.Connection = connectionStub.Object).Verifiable();

      _command.SetInnerConnection(new TracingDbConnection(connectionStub.Object, new Mock<IPersistenceExtension>().Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerConnection_WithNull ()
    {
      _innerCommandMock.SetupSet(mock => mock.Connection = null).Verifiable();

      _command.SetInnerConnection(null);

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void GetTransactionFromInterface ()
    {
      var transactionStub = new Mock<IDbTransaction>();
      _innerCommandMock.Setup(mock => mock.Transaction).Returns(transactionStub.Object);

      Assert.That(((IDbCommand)_command).Transaction, Is.SameAs(transactionStub.Object));
    }

    [Test]
    public void SetTransactionFromInterface ()
    {
      var transactionStub = new Mock<IDbTransaction>();
      _innerCommandMock.SetupSet(mock => mock.Transaction = transactionStub.Object).Verifiable();

      ((IDbCommand)_command).Transaction = transactionStub.Object;

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerTransaction_WithInstance ()
    {
      var transactionStub = new Mock<IDbTransaction>();
      _innerCommandMock.SetupSet(mock => mock.Transaction = transactionStub.Object).Verifiable();

      _command.SetInnerTransaction(new TracingDbTransaction(transactionStub.Object, new Mock<IPersistenceExtension>().Object, Guid.NewGuid()));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
    }

    [Test]
    public void SetInnerTransaction_WithNull ()
    {
      _innerCommandMock.SetupSet(mock => mock.Connection = null).Verifiable();

      _command.SetInnerConnection(null);

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
      var collectionStub = new Mock<IDataParameterCollection>();
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(collectionStub.Object);
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
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());
      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteNonQuery()).Returns(100).Verifiable();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, _command.QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();
      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryCompleted(_connectionID, _command.QueryID, TimeSpan.Zero, 100)).Verifiable();

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
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteNonQuery()).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, _command.QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteNonQuery(), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReader ()
    {
      var readerStub = new Mock<IDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteReader()).Returns(readerStub.Object).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, _command.QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      IDataReader actualReader = _command.ExecuteReader();

      Assert.That(actualReader, Is.InstanceOf(typeof(TracingDataReader)));
      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(_command.QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReader_WithError ()
    {
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteReader()).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, _command.QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteReader(), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderWithOverload ()
    {
      var readerStub = new Mock<IDataReader>();
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecuteReader(CommandBehavior.SchemaOnly)).Returns(readerStub.Object)
          .Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, _command.QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      IDataReader actualReader = _command.ExecuteReader(CommandBehavior.SchemaOnly);

      Assert.That(actualReader, Is.InstanceOf(typeof(TracingDataReader)));
      Assert.That(((TracingDataReader)actualReader).WrappedInstance, Is.SameAs(readerStub.Object));
      Assert.That(((TracingDataReader)actualReader).ConnectionID, Is.EqualTo(_connectionID));
      Assert.That(((TracingDataReader)actualReader).QueryID, Is.EqualTo(_command.QueryID));
      Assert.That(((TracingDataReader)actualReader).PersistenceExtension, Is.SameAs(_extensionMock.Object));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReaderWithOverload_WithError ()
    {
      Exception exception = new Exception("TestException");
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteReader(CommandBehavior.SchemaOnly)).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, _command.QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteReader(CommandBehavior.SchemaOnly), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteScalar ()
    {
      _innerCommandMock.Setup(mock => mock.CommandText).Returns("commandText");
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteScalar()).Returns(30).Verifiable();

      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuted(_connectionID, _command.QueryID, It.Is<TimeSpan>(_ => _ > TimeSpan.Zero)))
          .Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryCompleted(_connectionID, _command.QueryID, TimeSpan.Zero, 1)).Verifiable();

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
      _innerCommandMock.Setup(mock => mock.Parameters).Returns(CreateParameterCollection());

      var sequence = new VerifiableSequence();
      _extensionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.QueryExecuting(_connectionID, _command.QueryID, "commandText", It.IsNotNull<IDictionary<string, object>>()))
          .Verifiable();
      _innerCommandMock.InVerifiableSequence(sequence).Setup(mock => mock.ExecuteScalar()).Throws(exception).Verifiable();

      _extensionMock.InVerifiableSequence(sequence).Setup(mock => mock.QueryError(_connectionID, _command.QueryID, exception)).Verifiable();

      Assert.That(() => _command.ExecuteScalar(), Throws.Exception.SameAs(exception));

      _innerCommandMock.Verify();
      _extensionMock.Verify();
      sequence.Verify();
    }

    private IDataParameterCollection CreateParameterCollection ()
    {
      var command = new SqlCommand();
      return command.Parameters;
    }
  }
}
