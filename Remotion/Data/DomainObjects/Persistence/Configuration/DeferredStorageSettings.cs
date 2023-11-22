using System;
using JetBrains.Annotations;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  [UsedImplicitly]
  [ImplementationFor(typeof(IStorageSettings), Lifetime = LifetimeKind.Singleton)]
  public class DeferredStorageSettings : IStorageSettings
  {
    private readonly IStorageSettingsFactory _storageSettingsFactory;
    private readonly IStorageObjectFactoryFactory _storageObjectFactoryFactory;
    private IStorageSettings? _storageSettings;

    private IStorageSettings StorageSettings => _storageSettings ??= Initialize();

    public DeferredStorageSettings (IStorageSettingsFactory storageSettingsFactory, IStorageObjectFactoryFactory storageObjectFactoryFactory)
    {
      ArgumentUtility.CheckNotNull("storageSettingsFactory", storageSettingsFactory);
      ArgumentUtility.CheckNotNull("storageObjectFactoryFactory", storageObjectFactoryFactory);

      _storageSettingsFactory = storageSettingsFactory;
      _storageObjectFactoryFactory = storageObjectFactoryFactory;
    }

    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      return StorageSettings.GetStorageProviderDefinition(classDefinition);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull)
    {
      return StorageSettings.GetStorageProviderDefinition(storageGroupTypeOrNull);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
    {
      return StorageSettings.GetStorageProviderDefinition(storageProviderName);
    }

    public StorageProviderDefinition? GetDefaultStorageProviderDefinition ()
    {
      return StorageSettings.GetDefaultStorageProviderDefinition();
    }

    public ProviderCollection<StorageProviderDefinition> GetStorageProviderDefinitions ()
    {
      return StorageSettings.GetStorageProviderDefinitions();
    }

    private IStorageSettings Initialize ()
    {
      return _storageSettingsFactory.Create(_storageObjectFactoryFactory);
    }
  }
}
