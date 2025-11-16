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
using System.Threading;
using System.Threading.Tasks;

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

    public DbConnection WrappedInstance => _connection;
    public Guid ConnectionID => _connectionID;
    public IPersistenceExtension PersistenceExtension => _persistenceExtension;
    public override int ConnectionTimeout => _connection.ConnectionTimeout;
    public override string Database => _connection.Database;
    public override string DataSource => _connection.DataSource;
    protected override DbProviderFactory? DbProviderFactory => DbProviderFactories.GetFactory(_connection);
    public override string ServerVersion => _connection.ServerVersion;
    public override ConnectionState State => _connection.State;
    public override bool CanCreateBatch => _connection.CanCreateBatch;

    [AllowNull]
    public override string ConnectionString
    {
      get => _connection.ConnectionString;
      set => _connection.ConnectionString = value;
    }

    public override event StateChangeEventHandler? StateChange
    {
      add => _connection.StateChange += value;
      remove => _connection.StateChange -= value;
    }

    protected override DbBatch CreateDbBatch ()
    {
      return _connection.CreateBatch();
    }

    public override void ChangeDatabase (string databaseName)
    {
      _connection.ChangeDatabase(databaseName);
    }

    public override Task ChangeDatabaseAsync (string databaseName, CancellationToken cancellationToken = default)
    {
      return _connection.ChangeDatabaseAsync(databaseName, cancellationToken);
    }

    public override void EnlistTransaction (System.Transactions.Transaction? transaction)
    {
      _connection.EnlistTransaction(transaction);
    }

    public override DataTable GetSchema ()
    {
      return _connection.GetSchema();
    }

    public override Task<DataTable> GetSchemaAsync (CancellationToken cancellationToken = default)
    {
      return _connection.GetSchemaAsync(cancellationToken);
    }

    public override DataTable GetSchema (string collectionName)
    {
      return _connection.GetSchema(collectionName);
    }

    public override Task<DataTable> GetSchemaAsync (string collectionName, CancellationToken cancellationToken = default)
    {
      return _connection.GetSchemaAsync(collectionName, cancellationToken);
    }

    public override DataTable GetSchema (string collectionName, string?[] restrictionValues)
    {
      return _connection.GetSchema(collectionName, restrictionValues);
    }

    public override Task<DataTable> GetSchemaAsync (string collectionName, string?[] restrictionValues, CancellationToken cancellationToken = default)
    {
      return _connection.GetSchemaAsync(collectionName, restrictionValues, cancellationToken);
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

    protected override async ValueTask<DbTransaction> BeginDbTransactionAsync (IsolationLevel isolationLevel, CancellationToken cancellationToken)
    {
      var transaction = await _connection.BeginTransactionAsync(isolationLevel, cancellationToken);
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

    public override async Task OpenAsync (CancellationToken cancellationToken)
    {
      await _connection.OpenAsync(cancellationToken);
      PersistenceExtension.ConnectionOpened(_connectionID);
    }

    public override void Close ()
    {
      _connection.Close();

      TraceConnectionClosed();
    }

    public override async Task CloseAsync ()
    {
      await _connection.CloseAsync();
      TraceConnectionClosed();
    }

    protected override void OnStateChange (StateChangeEventArgs stateChange)
    {
      // Method is only implemented to satisfy rule that all virtual members should be re-implemented in the decorator.
      throw new InvalidOperationException("State Change notifications are always triggered by the wrapped connection instance.");
    }

    protected override void Dispose (bool disposing)
    {
      if (disposing)
      {
        _connection.Dispose();

        TraceConnectionClosed();
      }
    }

    public override async ValueTask DisposeAsync ()
    {
      await _connection.DisposeAsync();
      TraceConnectionClosed();
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
