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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="DbTransaction"/>. The lifetime of the transaction is traced using the
  /// <see cref="IPersistenceExtension"/> passed during the instantiation.
  /// </summary>
  public sealed class TracingDbTransaction : DbTransaction
  {
    private readonly DbTransaction _transaction;
    private readonly IPersistenceExtension _persistenceExtension;
    private readonly Guid _connectionID;
    private readonly Guid _transactionID;
    private bool _isTransactionDisposed;

    public TracingDbTransaction (DbTransaction transaction, IPersistenceExtension persistenceExtension, Guid connectionID)
    {
      ArgumentNullException.ThrowIfNull(transaction);
      ArgumentNullException.ThrowIfNull(persistenceExtension);
      _transaction = transaction;
      _persistenceExtension = persistenceExtension;
      _connectionID = connectionID;
      _transactionID = Guid.NewGuid();
    }

    public DbTransaction WrappedInstance => _transaction;
    public Guid ConnectionID => _connectionID;
    public Guid TransactionID => _transactionID;
    public IPersistenceExtension PersistenceExtension => _persistenceExtension;
    protected override DbConnection? DbConnection => _transaction.Connection;
    public override IsolationLevel IsolationLevel => _transaction.IsolationLevel;
    public override bool SupportsSavepoints => _transaction.SupportsSavepoints;

    public override void Commit ()
    {
      _transaction.Commit();
      if (!_isTransactionDisposed)
        PersistenceExtension.TransactionCommitted(_connectionID);
    }

    public override async Task CommitAsync (CancellationToken cancellationToken = default)
    {
      await _transaction.CommitAsync(cancellationToken);
      if (!_isTransactionDisposed)
        PersistenceExtension.TransactionCommitted(_connectionID);
    }

    public override void Save (string savepointName)
    {
      _transaction.Save(savepointName);
    }

    public override Task SaveAsync (string savepointName, CancellationToken cancellationToken = default)
    {
      return _transaction.SaveAsync(savepointName, cancellationToken);
    }

    public override void Rollback ()
    {
      _transaction.Rollback();
      if (!_isTransactionDisposed)
        PersistenceExtension.TransactionRolledBack(_connectionID);
    }

    public override void Rollback (string savepointName)
    {
      _transaction.Rollback(savepointName);
      if (!_isTransactionDisposed)
        PersistenceExtension.TransactionRolledBack(_connectionID);
    }

    public override async Task RollbackAsync (CancellationToken cancellationToken = default)
    {
      await _transaction.RollbackAsync(cancellationToken);
      if (!_isTransactionDisposed)
        PersistenceExtension.TransactionRolledBack(_connectionID);
    }

    public override async Task RollbackAsync (string savepointName, CancellationToken cancellationToken = default)
    {
      await _transaction.RollbackAsync(savepointName, cancellationToken);
      if (!_isTransactionDisposed)
        PersistenceExtension.TransactionRolledBack(_connectionID);
    }

    public override void Release (string savepointName)
    {
      _transaction.Release(savepointName);
    }

    public override Task ReleaseAsync (string savepointName, CancellationToken cancellationToken = default)
    {
      return _transaction.ReleaseAsync(savepointName, cancellationToken);
    }

    protected override void Dispose (bool disposing)
    {
      Assertion.DebugAssert(disposing, "Type is sealed without an implemented Finalizer. 'disposing' flag is always true");

      _transaction.Dispose();

      if (!_isTransactionDisposed)
      {
        PersistenceExtension.TransactionDisposed(_connectionID);
        _isTransactionDisposed = true;
      }
    }

    public override ValueTask DisposeAsync ()
    {
      var result = _transaction.DisposeAsync();

      if (!_isTransactionDisposed)
      {
        PersistenceExtension.TransactionDisposed(_connectionID);
        _isTransactionDisposed = true;
      }

      return result;
    }
  }
}
