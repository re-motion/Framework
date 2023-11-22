using System;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Creates an <see cref="IStorageSettings"/> object.
  /// </summary>
  public interface IStorageSettingsFactory
  {
    /// <summary>
    /// Creates an <see cref="IStorageSettings"/> object.
    /// </summary>
    /// <param name="storageObjectFactoryFactory">Used by the factory to create the <see cref="StorageProviderDefinition"/>s to be contained
    /// in the new <see cref="IStorageSettings"/> object.</param>
    IStorageSettings Create (IStorageObjectFactoryFactory storageObjectFactoryFactory);
  }
}
