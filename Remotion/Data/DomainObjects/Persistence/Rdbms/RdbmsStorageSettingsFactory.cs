using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  public class RdbmsStorageSettingsFactory : IStorageSettingsFactory
  {
    private readonly string _connectionString;
    private readonly Type _storageSettingsFactoryType;

    public RdbmsStorageSettingsFactory (string connectionString, Type storageSettingsFactoryType)
    {
      ArgumentUtility.CheckNotNullOrEmpty("connectionString", connectionString);
      ArgumentUtility.CheckNotNull("storageSettingsFactoryType", storageSettingsFactoryType);

      _connectionString = connectionString;
      _storageSettingsFactoryType = storageSettingsFactoryType;
    }

    public IStorageSettings Create (IStorageObjectFactoryFactory storageObjectFactoryFactory)
    {
      var providerDefinition = new RdbmsProviderDefinition("RdbmsProvider", storageObjectFactoryFactory.Create(_storageSettingsFactoryType), _connectionString);

      var storageProviderCollection = new ProviderCollection<StorageProviderDefinition>();
      storageProviderCollection.Add(providerDefinition);

      var settings = new StorageSettings(providerDefinition, storageProviderCollection, null);
      return settings;
    }
  }
}
