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
using System.Diagnostics;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Provides a wrapper for implementations of <see cref="IDbCommand"/>. Execution of the query is traced using the 
  /// <see cref="IPersistenceExtension"/> passed during the instantiation.
  /// </summary>
  public class TracingDbCommand : IDbCommand
  {
    #region IDbCommand implementation

    public void Dispose ()
    {
      _command.Dispose();
    }

    public void Prepare ()
    {
      _command.Prepare();
    }

    public void Cancel ()
    {
      _command.Cancel();
    }

    public IDbDataParameter CreateParameter ()
    {
      return _command.CreateParameter();
    }

    IDbConnection IDbCommand.Connection
    {
      get { return _command.Connection; }
      set { _command.Connection = value; }
    }

    IDbTransaction IDbCommand.Transaction
    {
      get { return _command.Transaction; }
      set { _command.Transaction = value; }
    }

    public string CommandText
    {
      get { return _command.CommandText; }
      set { _command.CommandText = value; }
    }

    public int CommandTimeout
    {
      get { return _command.CommandTimeout; }
      set { _command.CommandTimeout = value; }
    }

    public CommandType CommandType
    {
      get { return _command.CommandType; }
      set { _command.CommandType = value; }
    }

    public IDataParameterCollection Parameters
    {
      get { return _command.Parameters; }
    }

    public UpdateRowSource UpdatedRowSource
    {
      get { return _command.UpdatedRowSource; }
      set { _command.UpdatedRowSource = value; }
    }

    #endregion

    private readonly IDbCommand _command;
    private readonly IPersistenceExtension _persistenceExtension;
    private readonly Guid _connectionID;
    private readonly Guid _queryID;

    public TracingDbCommand (IDbCommand command, IPersistenceExtension persistenceExtension, Guid connectionID)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);

      _command = command;
      _persistenceExtension = persistenceExtension;
      _connectionID = connectionID;
      _queryID = Guid.NewGuid();
    }

    public IDbCommand WrappedInstance
    {
      get { return _command; }
    }

    public Guid ConnectionID
    {
      get { return _connectionID; }
    }

    public Guid QueryID
    {
      get { return _queryID; }
    }

    public IPersistenceExtension PersistenceExtension
    {
      get { return _persistenceExtension; }
    }

    public int ExecuteNonQuery ()
    {
      int numberOfRowsAffected = ExecuteWithProfiler (() => _command.ExecuteNonQuery());
      _persistenceExtension.QueryCompleted (_connectionID, _queryID, TimeSpan.Zero, numberOfRowsAffected);

      return numberOfRowsAffected;
    }

    public IDataReader ExecuteReader ()
    {
      IDataReader dataReader = ExecuteWithProfiler (() => _command.ExecuteReader());

      return new TracingDataReader (dataReader, _persistenceExtension, _connectionID, _queryID);
    }

    public IDataReader ExecuteReader (CommandBehavior behavior)
    {
      IDataReader dataReader = ExecuteWithProfiler (() => _command.ExecuteReader (behavior));

      return new TracingDataReader (dataReader, _persistenceExtension, _connectionID, _queryID);
    }

    public object ExecuteScalar ()
    {
      object result = ExecuteWithProfiler (() => _command.ExecuteScalar());
      _persistenceExtension.QueryCompleted (_connectionID, _queryID, TimeSpan.Zero, 1);
      return result;
    }

    public void SetInnerConnection (TracingDbConnection connection)
    {
      _command.Connection = connection == null ? null : connection.WrappedInstance;
    }

    public void SetInnerTransaction (TracingDbTransaction transaction)
    {
      _command.Transaction = transaction == null ? null : transaction.WrappedInstance;
    }

    private T ExecuteWithProfiler<T> (Func<T> operation)
    {
      try
      {
        _persistenceExtension.QueryExecuting (_connectionID, _queryID, _command.CommandText, ConvertToDictionary (_command.Parameters));
        var stopWatch = Stopwatch.StartNew();
        T result = operation();
        _persistenceExtension.QueryExecuted (_connectionID, _queryID, stopWatch.Elapsed);
        return result;
      }
      catch (Exception ex)
      {
        _persistenceExtension.QueryError (_connectionID, _queryID, ex);
        throw;
      }
    }

    private IDictionary<string, object> ConvertToDictionary (IDataParameterCollection parameters)
    {
      return parameters.Cast<IDbDataParameter>().ToDictionary (parameter => parameter.ParameterName, parameter => parameter.Value);
    }
  }
} 