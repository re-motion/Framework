using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction operations only executable while a read-only transaction is unlocked.
  /// Required by <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public class UnlockedParentTransactionContext : IUnlockedParentTransactionContext
  {
    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;
    private readonly IDisposable _scope;

    private bool _disposed;

    public UnlockedParentTransactionContext (
        ClientTransaction parentTransaction,
        IInvalidDomainObjectManager parentInvalidDomainObjectManager,
        IDisposable scope)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("scope", scope);

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;

      _scope = scope;
    }

    public void Dispose ()
    {
      if (!_disposed)
      {
        _disposed = true;
        _scope.Dispose ();
      }
    }

    public void MarkNotInvalid (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      CheckDisposed ();

      _parentInvalidDomainObjectManager.MarkNotInvalid (objectID);
    }

    public void RegisterDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      CheckDisposed ();

      _parentTransaction.DataManager.RegisterDataContainer (dataContainer);
    }

    public void Discard (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      CheckDisposed();

      _parentTransaction.DataManager.Discard (dataContainer);
    }

    private void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException (GetType ().ToString ());
    }
  }
}