using System;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
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
  }
}
