using System;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Resolves or creates an <see cref="IStorageObjectFactory"/> object.
  /// </summary>
  /// <seealso cref="StorageObjectFactoryFactory"/>
  public interface IStorageObjectFactoryFactory
  {
    /// <summary>
    /// Resolves or creates an <see cref="IStorageObjectFactory"/> object based on the supplied <see cref="Type"/>.
    /// </summary>
    IStorageObjectFactory Create (Type storageObjectFactoryType);

    /// <summary>
    /// Resolves or creates an <see cref="IStorageObjectFactory"/> object based on the <see cref="Type"/> of the generic parameter.
    /// </summary>
    T Create<T> ()
        where T : IStorageObjectFactory;
  }
}
