using System;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides access to the parent transaction operations only executable while a read-only transaction is unlocked.
  /// Required by <see cref="SubPersistenceStrategy"/>.
  /// </summary>
  public interface IUnlockedParentTransactionContext : IDisposable
  {
    void MarkNotInvalid (ObjectID objectID);

    void RegisterDataContainer (DataContainer dataContainer);
    void Discard (DataContainer dataContainer);
  }
}