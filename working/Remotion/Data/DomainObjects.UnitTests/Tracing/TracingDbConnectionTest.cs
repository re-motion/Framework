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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Tracing;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Tracing
{
  [TestFixture]
  public class TracingDbConnectionTest
  {
    private MockRepository _mockRepository;
    private IDbConnection _connection;
    private IDbConnection _innerConnectionMock;
    private IPersistenceExtension _extensionMock;
    
    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _innerConnectionMock = _mockRepository.StrictMock<IDbConnection>();
      _extensionMock = _mockRepository.StrictMock<IPersistenceExtension> ();
      
      _connection = new TracingDbConnection (_innerConnectionMock, _extensionMock);
    }

    [Test]
    public void ChangeDatabase ()
    {
      string databaseName = "databaseName";
      _innerConnectionMock.Expect (mock => mock.ChangeDatabase (databaseName));
      _mockRepository.ReplayAll();

      _connection.ChangeDatabase (databaseName);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConnectionString ()
    {
      var connectionString = "connectionString";
      _innerConnectionMock.Expect (mock => mock.ConnectionString).Return (connectionString);
      _mockRepository.ReplayAll();

      var result = _connection.ConnectionString;

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo(connectionString));
    }

    [Test]
    public void SetConnectionString ()
    {
      var connectionString = "connectionString";
      _innerConnectionMock.ConnectionString = connectionString;
      _mockRepository.ReplayAll ();

      _connection.ConnectionString = connectionString;

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetConnectionTimeout ()
    {
      var assumedResult = 10;
      _innerConnectionMock.Expect (mock => mock.ConnectionTimeout).Return (assumedResult);
      _mockRepository.ReplayAll();

      var result = _connection.ConnectionTimeout;

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetDatabase ()
    {
      var assumedResult = "database";
      _innerConnectionMock.Expect (mock => mock.Database).Return (assumedResult);
      _mockRepository.ReplayAll ();

      var result = _connection.Database;

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetState ()
    {
      var assumedResult = new ConnectionState();
      _innerConnectionMock.Expect (mock => mock.State).Return (assumedResult);
      _mockRepository.ReplayAll ();

      var result = _connection.State;

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.EqualTo (assumedResult));
    }

    [Test]
    public void GetWrappedInstance ()
    {
      var wrappedInstance = ((TracingDbConnection)_connection).WrappedInstance;
      Assert.That (wrappedInstance, Is.EqualTo (_innerConnectionMock));
    }

    [Test]
    public void Open ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerConnectionMock.Expect (mock => mock.Open());
        _extensionMock.Expect (mock => mock.ConnectionOpened (((TracingDbConnection) _connection).ConnectionID));
      }
      _mockRepository.ReplayAll();

      _connection.Open();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Dispose_ConnectionOpen ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerConnectionMock.Expect (mock => mock.Dispose());
        _extensionMock.Expect (mock => mock.ConnectionClosed (((TracingDbConnection) _connection).ConnectionID));
      }
      _mockRepository.ReplayAll();

      _connection.Dispose();
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Close ()
    {
      using (_mockRepository.Ordered ())
      {
        _innerConnectionMock.Expect (mock => mock.Close ());
        _extensionMock.Expect (mock => mock.ConnectionClosed (((TracingDbConnection) _connection).ConnectionID));
      }
      _mockRepository.ReplayAll ();

      _connection.Close ();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void BeginTransaction ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      var dbTransactionMock = _mockRepository.StrictMock<IDbTransaction> ();
      dbTransactionMock.Expect (mock => mock.IsolationLevel).Return (isolationLevel);

      _innerConnectionMock.Expect (mock => mock.BeginTransaction()).Return(dbTransactionMock);
      _extensionMock.Expect (mock => mock.TransactionBegan (((TracingDbConnection) _connection).ConnectionID, isolationLevel));
      
      _mockRepository.ReplayAll();

      var tracingDbTransaction = ((TracingDbConnection) _connection).BeginTransaction ();

      _mockRepository.VerifyAll();
      Assert.That (tracingDbTransaction.WrappedInstance, Is.EqualTo (dbTransactionMock));
      Assert.That (tracingDbTransaction.PersistenceExtension, Is.EqualTo (_extensionMock));
      Assert.That (tracingDbTransaction.ConnectionID, Is.EqualTo (((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransaction_WithIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;

      var dbTransactionMock = _mockRepository.StrictMock<IDbTransaction> ();
      
      _innerConnectionMock.Expect (mock => mock.BeginTransaction (isolationLevel)).Return (dbTransactionMock);
      _extensionMock.Expect (mock => mock.TransactionBegan (((TracingDbConnection)_connection).ConnectionID, isolationLevel));

      _mockRepository.ReplayAll ();

      var tracingDbTransaction = ((TracingDbConnection)_connection).BeginTransaction (isolationLevel);

      _mockRepository.VerifyAll ();

      Assert.That (tracingDbTransaction.WrappedInstance, Is.EqualTo (dbTransactionMock));
      Assert.That (tracingDbTransaction.PersistenceExtension, Is.EqualTo (_extensionMock));
      Assert.That (tracingDbTransaction.ConnectionID, Is.EqualTo (((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void CreateCommand ()
    {
      var commandMock = _mockRepository.StrictMock<IDbCommand>();
      _innerConnectionMock.Expect (mock => mock.CreateCommand()).Return (commandMock);

      _mockRepository.ReplayAll();

      var tracingDbCommand = ((TracingDbConnection)_connection).CreateCommand();

      _mockRepository.VerifyAll();

      Assert.That (tracingDbCommand.WrappedInstance, Is.EqualTo (commandMock));
      Assert.That (tracingDbCommand.PersistenceExtension, Is.EqualTo (_extensionMock));
      Assert.That (tracingDbCommand.ConnectionID, Is.EqualTo (((TracingDbConnection)_connection).ConnectionID));
    }

    [Test]
    public void BeginTransactionExplicitInterfaceImplementation ()
    {
      var isolationLevel = IsolationLevel.Chaos;
      var dbTransactionMock = _mockRepository.StrictMock<IDbTransaction> ();
      dbTransactionMock.Expect (mock => mock.IsolationLevel).Return (isolationLevel);

      _innerConnectionMock.Expect (mock => mock.BeginTransaction ()).Return (dbTransactionMock);
      _extensionMock.Expect (mock => mock.TransactionBegan (((TracingDbConnection) _connection).ConnectionID, isolationLevel));

      _mockRepository.ReplayAll ();

      _connection.BeginTransaction();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void BeginTransactionExplicitInterfaceImplementationWithIsolationLevel ()
    {
      var isolationLevel = IsolationLevel.Chaos;

      var dbTransactionMock = _mockRepository.StrictMock<IDbTransaction> ();

      _innerConnectionMock.Expect (mock => mock.BeginTransaction (isolationLevel)).Return (dbTransactionMock);
      _extensionMock.Expect (mock => mock.TransactionBegan (((TracingDbConnection) _connection).ConnectionID, isolationLevel));

      _mockRepository.ReplayAll ();

      _connection.BeginTransaction (isolationLevel);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CreateCommandExplicitInterfaceImplementation ()
    {
      var commandMock = _mockRepository.StrictMock<IDbCommand> ();
      _innerConnectionMock.Expect (mock => mock.CreateCommand ()).Return (commandMock);

      _mockRepository.ReplayAll ();

      var result = _connection.CreateCommand();

      _mockRepository.VerifyAll();
    }
  }
}