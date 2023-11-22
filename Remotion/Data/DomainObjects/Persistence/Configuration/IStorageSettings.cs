using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public interface IStorageSettings
  {
    StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition);

    StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull);


    StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName);

    StorageProviderDefinition? GetDefaultStorageProviderDefinition ();

    ProviderCollection<StorageProviderDefinition> GetStorageProviderDefinitions ();
  }

}
