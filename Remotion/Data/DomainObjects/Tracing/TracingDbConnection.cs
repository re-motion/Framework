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
using System.Diagnostics.CodeAnalysis;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="DbConnection"/>. The lifetime of the connection is traced using the
  /// <see cref="IPersistenceExtension"/> passed during the instantiation.
  /// </summary>
  public sealed class TracingDbConnection : DbConnection
  {
    private readonly DbConnection _connection;
    private readonly IPersistenceExtension _persistenceExtension;
    private readonly Guid _connectionID;
    private bool _isConnectionClosed;

    public TracingDbConnection (DbConnection connection, IPersistenceExtension persistenceExtension)
    {
      ArgumentNullException.ThrowIfNull(connection);
      ArgumentNullException.ThrowIfNull(persistenceExtension);

      _connection = connection;
      _persistenceExtension = persistenceExtension;
      _connectionID = Guid.NewGuid();
    }


    [AllowNull]
    public override string ConnectionString
    {
      get => _connection.ConnectionString;
      set => _connection.ConnectionString = value;
    }
    public DbConnection WrappedInstance => _connection;
    public Guid ConnectionID => _connectionID;
    public IPersistenceExtension PersistenceExtension => _persistenceExtension;
    public override int ConnectionTimeout => _connection.ConnectionTimeout;
    public override string Database => _connection.Database;
    public override string DataSource => _connection.DataSource;
    public override string ServerVersion => _connection.ServerVersion;
    public override ConnectionState State => _connection.State;


    public override void ChangeDatabase (string databaseName)
    {
      _connection.ChangeDatabase(databaseName);
    }

    public new TracingDbTransaction BeginTransaction ()
    {
      return (TracingDbTransaction)base.BeginTransaction();
    }

    public new TracingDbTransaction BeginTransaction (IsolationLevel isolationLevel)
    {
      return (TracingDbTransaction)base.BeginTransaction(isolationLevel);
    }

    protected override TracingDbTransaction BeginDbTransaction (IsolationLevel isolationLevel)
    {
      var transaction = _connection.BeginTransaction(isolationLevel);
      PersistenceExtension.TransactionBegan(_connectionID, transaction.IsolationLevel);
      return CreateTracingTransaction(transaction);
    }

    public new TracingDbCommand CreateCommand ()
    {
      return (TracingDbCommand)base.CreateCommand();
    }

    protected override TracingDbCommand CreateDbCommand ()
    {
      return CreateTracingCommand(_connection.CreateCommand());
    }

    public override void Open ()
    {
      _connection.Open();
      PersistenceExtension.ConnectionOpened(_connectionID);
    }

    public override void Close ()
    {
      _connection.Close();

      TraceConnectionClosed();
    }

    protected override void Dispose (bool disposing)
    {
      if (disposing)
      {
        _connection.Dispose();

        TraceConnectionClosed();
      }
    }


    private TracingDbTransaction CreateTracingTransaction (DbTransaction transaction)
    {
      return new TracingDbTransaction(transaction, _persistenceExtension, _connectionID);
    }

    private TracingDbCommand CreateTracingCommand (DbCommand command)
    {
      return new TracingDbCommand(command, _persistenceExtension, _connectionID);
    }

    private void TraceConnectionClosed ()
    {
      if (!_isConnectionClosed)
      {
        PersistenceExtension.ConnectionClosed(_connectionID);
        _isConnectionClosed = true;
      }
    }
  }
}
