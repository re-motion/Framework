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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbCommandTest
  {
    private MockRepository _mockRepository;
    private TracingDbCommand _command;
    private IDbCommand _innerCommandMock;
    private IPersistenceExtension _extensionMock;
    private Guid _connectionID;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _innerCommandMock = _mockRepository.StrictMock<IDbCommand>();
      _extensionMock = _mockRepository.StrictMock<IPersistenceExtension>();
      _connectionID = Guid.NewGuid();

      _command = new TracingDbCommand (_innerCommandMock, _extensionMock, _connectionID);
    }

    [Test]
    public void Dispose ()
    {
      _innerCommandMock.Expect (mock => mock.Dispose());
      _mockRepository.ReplayAll();

      _command.Dispose();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Prepare ()
    {
      _innerCommandMock.Expect (mock => mock.Prepare());
      _mockRepository.ReplayAll();

      _command.Prepare();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Cancel ()
    {
      _innerCommandMock.Expect (mock => mock.Cancel());
      _mockRepository.ReplayAll();

      _command.Cancel();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CreateParameter ()
    {
      IDbDataParameter parameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _innerCommandMock.Stub (mock => mock.CreateParameter()).Return (parameterStub);
      _mockRepository.ReplayAll();

      Assert.That (_command.CreateParameter(), Is.SameAs (parameterStub));
    }

    [Test]
    public void GetConnectionFromInterface ()
    {
      IDbConnection connectionStub = MockRepository.GenerateStub<IDbConnection>();
      _innerCommandMock.Stub (mock => mock.Connection).Return (connectionStub);
      _mockRepository.ReplayAll();

      Assert.That (((IDbCommand)_command).Connection, Is.SameAs (connectionStub));
    }

    [Test]
    public void SetConnectionFromInterface ()
    {
      IDbConnection connectionStub = MockRepository.GenerateStub<IDbConnection>();
      _innerCommandMock.Expect (mock => mock.Connection = connectionStub);
      _mockRepository.ReplayAll();

      ((IDbCommand) _command).Connection = connectionStub;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SetInnerConnection_WithInstance ()
    {
      IDbConnection connectionStub = MockRepository.GenerateStub<IDbConnection> ();
      _innerCommandMock.Expect (mock => mock.Connection = connectionStub);
      _mockRepository.ReplayAll ();

      _command.SetInnerConnection (new TracingDbConnection (connectionStub, MockRepository.GenerateStub<IPersistenceExtension>()));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SetInnerConnection_WithNull ()
    {
      _innerCommandMock.Expect (mock => mock.Connection = null);
      _mockRepository.ReplayAll ();

      _command.SetInnerConnection (null);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTransactionFromInterface ()
    {
      IDbTransaction transactionStub = MockRepository.GenerateStub<IDbTransaction>();
      _innerCommandMock.Stub (mock => mock.Transaction).Return (transactionStub);
      _mockRepository.ReplayAll();

      Assert.That (((IDbCommand) _command).Transaction, Is.SameAs (transactionStub));
    }

    [Test]
    public void SetTransactionFromInterface ()
    {
      IDbTransaction transactionStub = MockRepository.GenerateStub<IDbTransaction>();
      _innerCommandMock.Expect (mock => mock.Transaction = transactionStub);
      _mockRepository.ReplayAll();

      ((IDbCommand) _command).Transaction = transactionStub;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SetInnerTransaction_WithInstance ()
    {
      IDbTransaction transactionStub = MockRepository.GenerateStub<IDbTransaction>();
      _innerCommandMock.Expect (mock => mock.Transaction = transactionStub);
      _mockRepository.ReplayAll();

      _command.SetInnerTransaction (new TracingDbTransaction (transactionStub, MockRepository.GenerateStub<IPersistenceExtension>(), Guid.NewGuid()));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void SetInnerTransaction_WithNull ()
    {
      _innerCommandMock.Expect (mock => mock.Connection = null);
      _mockRepository.ReplayAll ();

      _command.SetInnerConnection (null);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetCommandText ()
    {
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _mockRepository.ReplayAll();
      Assert.That (_command.CommandText, Is.EqualTo ("commandText"));
    }

    [Test]
    public void SetCommandText ()
    {
      _innerCommandMock.Expect (mock => mock.CommandText = "commandText");
      _mockRepository.ReplayAll();

      _command.CommandText = "commandText";

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetCommandTimeout ()
    {
      _innerCommandMock.Stub (mock => mock.CommandTimeout).Return (100);
      _mockRepository.ReplayAll();

      Assert.That (_command.CommandTimeout, Is.EqualTo (100));
    }

    [Test]
    public void SetCommandTimeout ()
    {
      _innerCommandMock.Expect (mock => mock.CommandTimeout = 100);
      _mockRepository.ReplayAll();

      _command.CommandTimeout = 100;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetCommandType ()
    {
      _innerCommandMock.Stub (mock => mock.CommandType).Return (CommandType.TableDirect);
      _mockRepository.ReplayAll();
      Assert.That (_command.CommandType, Is.EqualTo (CommandType.TableDirect));
    }

    [Test]
    public void SetCommandType ()
    {
      _innerCommandMock.Expect (mock => mock.CommandType = CommandType.TableDirect);
      _mockRepository.ReplayAll();

      _command.CommandType = CommandType.TableDirect;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetParameters ()
    {
      IDataParameterCollection collectionStub = MockRepository.GenerateStub<IDataParameterCollection>();
      _innerCommandMock.Stub (mock => mock.Parameters).Return (collectionStub);
      _mockRepository.ReplayAll();
      Assert.That (_command.Parameters, Is.SameAs (collectionStub));
    }

    [Test]
    public void GetUpdatedRowSource ()
    {
      _innerCommandMock.Stub (mock => mock.UpdatedRowSource).Return (UpdateRowSource.FirstReturnedRecord);
      _mockRepository.ReplayAll();
      Assert.That (_command.UpdatedRowSource, Is.EqualTo (UpdateRowSource.FirstReturnedRecord));
    }

    [Test]
    public void SetUpdatedRowSource ()
    {
      _innerCommandMock.Expect (mock => mock.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord);
      _mockRepository.ReplayAll();

      _command.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteNonQuery ()
    {
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection());
      using (_mockRepository.Ordered())
      {
        _extensionMock.QueryExecuting (Arg.Is(_connectionID),Arg.Is( _command.QueryID), Arg.Is("commandText"), Arg<IDictionary<string,object>>.Is.NotNull);
        _innerCommandMock.Expect (mock => mock.ExecuteNonQuery()).Return (100);
        _extensionMock.QueryExecuted (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg<TimeSpan>.Is.GreaterThan (TimeSpan.Zero));
        _extensionMock.QueryCompleted (_connectionID, _command.QueryID, TimeSpan.Zero, 100);
      }
      _mockRepository.ReplayAll();

      Assert.That (_command.ExecuteNonQuery(), Is.EqualTo (100));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteNonQuery_WithError ()
    {
      Exception exception = new Exception ("TestException");
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());

      using (_mockRepository.Ordered())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteNonQuery ()).Throw (exception);
        _extensionMock.QueryError (_connectionID, _command.QueryID, exception);
      }
      _mockRepository.ReplayAll();

      try
      {
        _command.ExecuteNonQuery();
        Assert.Fail ("No exception");
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteReader ()
    {
      IDataReader readerStub = MockRepository.GenerateStub<IDataReader>();
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());
      
      using (_mockRepository.Ordered ())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteReader ()).Return (readerStub);
        _extensionMock.QueryExecuted (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg<TimeSpan>.Is.GreaterThan (TimeSpan.Zero));
      }
      _mockRepository.ReplayAll();

      IDataReader actualReader = _command.ExecuteReader();
      Assert.That (actualReader, Is.InstanceOf (typeof (TracingDataReader)));
      Assert.That (((TracingDataReader) actualReader).WrappedInstance, Is.SameAs (readerStub));
      Assert.That (((TracingDataReader) actualReader).ConnectionID, Is.EqualTo (_connectionID));
      Assert.That (((TracingDataReader) actualReader).QueryID, Is.EqualTo (_command.QueryID));
      Assert.That (((TracingDataReader) actualReader).PersistenceExtension, Is.SameAs (_extensionMock));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteReader_WithError ()
    {
      Exception exception = new Exception ("TestException");
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());

      using (_mockRepository.Ordered())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteReader ()).Throw (exception);
        _extensionMock.QueryError (_connectionID, _command.QueryID, exception);
      }
      _mockRepository.ReplayAll();

      try
      {
        _command.ExecuteReader();
        Assert.Fail ("No exception");
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteReaderWithOverload ()
    {
      IDataReader readerStub = MockRepository.GenerateStub<IDataReader>();
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());
      
      using (_mockRepository.Ordered ())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteReader (CommandBehavior.SchemaOnly)).Return (readerStub);
        _extensionMock.QueryExecuted (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg<TimeSpan>.Is.GreaterThan (TimeSpan.Zero));
      }
      _mockRepository.ReplayAll();

      IDataReader actualReader = _command.ExecuteReader (CommandBehavior.SchemaOnly);
      Assert.That (actualReader, Is.InstanceOf (typeof (TracingDataReader)));
      Assert.That (((TracingDataReader) actualReader).WrappedInstance, Is.SameAs (readerStub));
      Assert.That (((TracingDataReader) actualReader).ConnectionID, Is.EqualTo (_connectionID));
      Assert.That (((TracingDataReader) actualReader).QueryID, Is.EqualTo (_command.QueryID));
      Assert.That (((TracingDataReader) actualReader).PersistenceExtension, Is.SameAs (_extensionMock));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteReaderWithOverload_WithError ()
    {
      Exception exception = new Exception ("TestException");
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());

      using (_mockRepository.Ordered())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteReader (CommandBehavior.SchemaOnly)).Throw (exception);
        _extensionMock.QueryError (_connectionID, _command.QueryID, exception);
      }
      _mockRepository.ReplayAll();

      try
      {
        _command.ExecuteReader (CommandBehavior.SchemaOnly);
        Assert.Fail ("No exception");
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }
      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteScalar ()
    {
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());
      
      using (_mockRepository.Ordered ())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteScalar ()).Return (30);
        _extensionMock.QueryExecuted (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg<TimeSpan>.Is.GreaterThan (TimeSpan.Zero));
        _extensionMock.QueryCompleted (_connectionID, _command.QueryID, TimeSpan.Zero, 1);
      }
      _mockRepository.ReplayAll();

      Assert.That (_command.ExecuteScalar(), Is.EqualTo (30));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteScalar_WithError ()
    {
      Exception exception = new Exception ("TestException");
      _innerCommandMock.Stub (mock => mock.CommandText).Return ("commandText");
      _innerCommandMock.Stub (mock => mock.Parameters).Return (CreateParameterCollection ());

      using (_mockRepository.Ordered())
      {
        _extensionMock.QueryExecuting (Arg.Is (_connectionID), Arg.Is (_command.QueryID), Arg.Is ("commandText"), CreateParametersArgumentExpectation());
        _innerCommandMock.Expect (mock => mock.ExecuteScalar ()).Throw (exception);
        _extensionMock.QueryError (_connectionID, _command.QueryID, exception);
      }
      _mockRepository.ReplayAll();

      try
      {
        _command.ExecuteScalar();
        Assert.Fail ("No exception");
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.SameAs (exception));
      }
      _mockRepository.VerifyAll();
    }

    private IDataParameterCollection CreateParameterCollection ()
    {
      var command = new SqlCommand ();
      return command.Parameters;
    }

    private IDictionary<string, object> CreateParametersArgumentExpectation ()
    {
      return Arg<IDictionary<string, object>>.Is.NotNull;
    }
  }
}