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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// Implements a collection of <see cref="IPersistenceExtension"/> objects.
  /// </summary>
  public class CompoundPersistenceExtension : IPersistenceExtension
  {
    private readonly List<IPersistenceExtension> _listeners = new List<IPersistenceExtension>();

    public CompoundPersistenceExtension (IEnumerable<IPersistenceExtension> listeners)
    {
      ArgumentUtility.CheckNotNull ("listeners", listeners);

      _listeners.AddRange (listeners);
    }

    public void ConnectionOpened (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.ConnectionOpened (connectionID);
    }

    public void ConnectionClosed (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.ConnectionClosed (connectionID);
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
      foreach (var listener in _listeners)
        listener.TransactionBegan (connectionID, isolationLevel);
    }

    public void TransactionCommitted (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TransactionCommitted (connectionID);
    }

    public void TransactionRolledBack (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TransactionRolledBack (connectionID);
    }

    public void TransactionDisposed (Guid connectionID)
    {
      foreach (var listener in _listeners)
        listener.TransactionDisposed (connectionID);
    }

    public void QueryExecuting (Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
      ArgumentUtility.CheckNotNull ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      foreach (var listener in _listeners)
        listener.QueryExecuting (connectionID, queryID, commandText, parameters);
    }

    public void QueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
      foreach (var listener in _listeners)
        listener.QueryExecuted (connectionID, queryID, durationOfQueryExecution);
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
      foreach (var listener in _listeners)
        listener.QueryCompleted (connectionID, queryID, durationOfDataRead, rowCount);
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
      foreach (var listener in _listeners)
        listener.QueryError (connectionID, queryID, e);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}