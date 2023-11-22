using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Defines an API for configuring the storage settings.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  /// <seealso cref="StorageSettings"/>
  /// <seealso cref="DeferredStorageSettings"/>
  public interface IStorageSettings
  {
    /// <summary>
    /// Gets a storage provider definition based on the given class definition.
    /// </summary>
    StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition);

    /// <summary>
    /// Gets a storage provider definition based on the given storage group type. If null is supplied, the default storage
    /// provider definition is returned instead.
    /// </summary>
    StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull);

    /// <summary>
    /// Gets a storage provider definition based on the given storage provider name.
    /// </summary>
    StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName);

    /// <summary>
    /// Gets the default storage provider definition or null if it doesn't exist.
    /// </summary>
    StorageProviderDefinition? GetDefaultStorageProviderDefinition ();

    /// <summary>
    /// Gets all storage provider definitions
    /// </summary>
    ProviderCollection<StorageProviderDefinition> GetStorageProviderDefinitions ();
  }
}
