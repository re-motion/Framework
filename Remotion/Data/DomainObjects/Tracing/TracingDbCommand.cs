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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="DbCommand"/>. Execution of the query is traced using the 
  /// <see cref="IPersistenceExtension"/> passed during the instantiation.
  /// </summary>
  public sealed class TracingDbCommand : DbCommand
  {
    private readonly DbCommand _command;
    private readonly IPersistenceExtension _persistenceExtension;
    private readonly Guid _connectionID;
    private readonly Guid _queryID;

    public TracingDbCommand (DbCommand command, IPersistenceExtension persistenceExtension, Guid connectionID)
    {
      ArgumentNullException.ThrowIfNull(command);
      ArgumentNullException.ThrowIfNull(persistenceExtension);

      _command = command;
      _persistenceExtension = persistenceExtension;
      _connectionID = connectionID;
      _queryID = Guid.NewGuid();
    }

    [AllowNull]
    public override string CommandText
    {
      get => _command.CommandText;
      set => _command.CommandText = value;
    }

    public override int CommandTimeout
    {
      get => _command.CommandTimeout;
      set => _command.CommandTimeout = value;
    }

    public override CommandType CommandType
    {
      get => _command.CommandType;
      set => _command.CommandType = value;
    }

    public override bool DesignTimeVisible
    {
      get => _command.DesignTimeVisible;
      set => _command.DesignTimeVisible = value;
    }

    public override UpdateRowSource UpdatedRowSource
    {
      get => _command.UpdatedRowSource;
      set => _command.UpdatedRowSource = value;
    }

    protected override DbConnection? DbConnection
    {
      get => _command.Connection;
      set => _command.Connection = value;
    }
    protected override DbTransaction? DbTransaction
    {
      get => _command.Transaction;
      set => _command.Transaction = value;
    }

    public DbCommand WrappedInstance => _command;
    public Guid ConnectionID => _connectionID;
    public Guid QueryID => _queryID;
    public IPersistenceExtension PersistenceExtension => _persistenceExtension;
    protected override DbParameterCollection DbParameterCollection => _command.Parameters;


    public override int ExecuteNonQuery ()
    {
      int numberOfRowsAffected = ExecuteWithProfiler(() => _command.ExecuteNonQuery());
      _persistenceExtension.QueryCompleted(_connectionID, _queryID, TimeSpan.Zero, numberOfRowsAffected);

      return numberOfRowsAffected;
    }

    public new TracingDataReader ExecuteReader ()
    {
      return (TracingDataReader)base.ExecuteReader();
    }

    public new TracingDataReader ExecuteReader (CommandBehavior behavior)
    {
      return (TracingDataReader)base.ExecuteReader(behavior);
    }

    public override object? ExecuteScalar ()
    {
      object? result = ExecuteWithProfiler(() => _command.ExecuteScalar());
      _persistenceExtension.QueryCompleted(_connectionID, _queryID, TimeSpan.Zero, 1);
      return result;
    }

    public void SetInnerConnection (TracingDbConnection connection)
    {
      _command.Connection = connection == null ? null : connection.WrappedInstance;
    }

    public void SetInnerTransaction (TracingDbTransaction? transaction)
    {
      _command.Transaction = transaction == null ? null : transaction.WrappedInstance;
    }

    public override void Prepare ()
    {
      _command.Prepare();
    }

    public override void Cancel ()
    {
      _command.Cancel();
    }

    protected override DbParameter CreateDbParameter ()
    {
      return _command.CreateParameter();
    }

    protected override TracingDataReader ExecuteDbDataReader (CommandBehavior behavior)
    {
      DbDataReader dataReader = ExecuteWithProfiler(() => _command.ExecuteReader(behavior));

      return new TracingDataReader(dataReader, _persistenceExtension, _connectionID, _queryID);
    }

    protected override void Dispose (bool disposing)
    {
      if (disposing)
      {
        _command.Dispose();
      }
    }

    private T ExecuteWithProfiler<T> (Func<T> operation)
    {
      try
      {
        _persistenceExtension.QueryExecuting(_connectionID, _queryID, _command.CommandText, ConvertToDictionary(_command.Parameters));
        var stopWatch = Stopwatch.StartNew();
        T result = operation();
        _persistenceExtension.QueryExecuted(_connectionID, _queryID, stopWatch.Elapsed);
        return result;
      }
      catch (Exception ex)
      {
        _persistenceExtension.QueryError(_connectionID, _queryID, ex);
        throw;
      }
    }

    private IDictionary<string, object?> ConvertToDictionary (DbParameterCollection parameters)
    {
      return parameters.Cast<DbParameter>().ToDictionary(parameter => parameter.ParameterName, parameter => parameter.Value);
    }
  }
}
