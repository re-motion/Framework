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
    public static readonly IPersistenceExtension Instance = new NullPersistenceExtension ();

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