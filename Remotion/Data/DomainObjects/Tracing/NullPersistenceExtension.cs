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

namespace Remotion.Data.DomainObjects.Tracing
{
  /// <summary>
  /// <see cref="INullObject"/> implementation of <see cref="IPersistenceExtension"/>.
  /// </summary>
  public sealed class NullPersistenceExtension : IPersistenceExtension
  {
    public static readonly IPersistenceExtension Instance = new NullPersistenceExtension();

    private NullPersistenceExtension ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void ConnectionOpened (Guid connectionID)
    {
    }

    public void ConnectionClosed (Guid connectionID)
    {
    }

    public void TransactionBegan (Guid connectionID, IsolationLevel isolationLevel)
    {
    }

    public void TransactionCommitted (Guid connectionID)
    {
    }

    public void TransactionRolledBack (Guid connectionID)
    {
    }

    public void TransactionDisposed (Guid connectionID)
    {
    }

    public void QueryExecuting (Guid connectionID, Guid queryID, string commandText, IDictionary<string, object> parameters)
    {
    }

    public void QueryExecuted (Guid connectionID, Guid queryID, TimeSpan durationOfQueryExecution)
    {
    }

    public void QueryCompleted (Guid connectionID, Guid queryID, TimeSpan durationOfDataRead, int rowCount)
    {
    }

    public void QueryError (Guid connectionID, Guid queryID, Exception e)
    {
    }
  }
}