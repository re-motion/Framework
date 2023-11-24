using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Enables configuration of the storage definition providers through IoC in a lazy manner.
  /// Creates an instance of <see cref="Configuration.StorageSettings"/> at the first access to one of its methods
  /// based on the supplied <see cref="IStorageSettingsFactory"/> and <see cref="IStorageObjectFactoryFactory"/>.
  /// This <see cref="Configuration.StorageSettings"/> object is then used in every method call afterwards as well,
  /// supplying the actual functionality.
  /// </summary>
  [UsedImplicitly]
  [ImplementationFor(typeof(IStorageSettings), Lifetime = LifetimeKind.Singleton)]
  public class DeferredStorageSettings : IStorageSettings
  {
    private readonly IStorageSettingsFactory _storageSettingsFactory;
    private readonly IStorageObjectFactoryFactory _storageObjectFactoryFactory;

    private readonly Lazy<IStorageSettings> _lazyStorageSettings;
    //TODO use lazy<t>

    /// <summary>
    /// Creates an uninitialized instance of this class. When one of the methods on this object is called, it
    /// becomes initialized by creating an internal <see cref="IStorageSettings"/> object.
    /// </summary>
    /// <param name="storageSettingsFactory">The settings factory which creates the internal <see cref="IStorageSettings"/>.</param>
    /// <param name="storageObjectFactoryFactory">
    /// The object factory used by the <paramref name="storageSettingsFactory"/> to create the internal <see cref="IStorageSettings"/>.
    /// </param>
    public DeferredStorageSettings (IStorageSettingsFactory storageSettingsFactory, IStorageObjectFactoryFactory storageObjectFactoryFactory)
    {
      ArgumentUtility.CheckNotNull("storageSettingsFactory", storageSettingsFactory);
      ArgumentUtility.CheckNotNull("storageObjectFactoryFactory", storageObjectFactoryFactory);

      _storageSettingsFactory = storageSettingsFactory;
      _storageObjectFactoryFactory = storageObjectFactoryFactory;

      _lazyStorageSettings = new Lazy<IStorageSettings>(Initialize);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      return _lazyStorageSettings.Value.GetStorageProviderDefinition(classDefinition);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull)
    {
      return _lazyStorageSettings.Value.GetStorageProviderDefinition(storageGroupTypeOrNull);
    }


    public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
    {
      return _lazyStorageSettings.Value.GetStorageProviderDefinition(storageProviderName);
    }

    public StorageProviderDefinition? DefaultStorageProviderDefinition => _lazyStorageSettings.Value.DefaultStorageProviderDefinition;

    public IReadOnlyCollection<StorageProviderDefinition> StorageProviderDefinitions => _lazyStorageSettings.Value.StorageProviderDefinitions;

    private IStorageSettings Initialize ()
    {
      return _storageSettingsFactory.Create(_storageObjectFactoryFactory);
    }
  }
}
