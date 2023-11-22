using System;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  class StorageSettings : IStorageSettings
  {
    private readonly StorageProviderDefinition? _defaultStorageProviderDefinition;
    private readonly ProviderCollection<StorageProviderDefinition> _storageProviderCollection;
    private readonly ConfigurationElementCollection<StorageGroupElement>? _storageGroups;

    public StorageSettings (
        StorageProviderDefinition? defaultStorageProviderDefinition,
        ProviderCollection<StorageProviderDefinition> storageProviderCollection,
        ConfigurationElementCollection<StorageGroupElement> storageGroups)
    {
      _defaultStorageProviderDefinition = defaultStorageProviderDefinition;
      _storageProviderCollection = storageProviderCollection;
      _storageGroups = storageGroups;
    }

    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var storageGroupTypeOrNull = classDefinition.StorageGroupType;

      return GetStorageProviderDefinition(storageGroupTypeOrNull);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull)
    {
      if (storageGroupTypeOrNull == null)
        return GetDefaultStorageProviderDefinition() ?? throw CreateMissingDefaultProviderException();

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName(storageGroupTypeOrNull);
      var storageGroup = _storageGroups?[storageGroupName];
      if (storageGroup == null)
        return GetDefaultStorageProviderDefinition() ?? throw CreateMissingDefaultProviderException();

      return _storageProviderCollection.GetMandatory(storageGroup.StorageProviderName);
    }

    public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("storageProviderName", storageProviderName);

      return _storageProviderCollection[storageProviderName];
    }

    public StorageProviderDefinition? GetDefaultStorageProviderDefinition ()
    {
      return _defaultStorageProviderDefinition;
    }

    // This probably can't be an IReadOnlyCollection, as most use cases want the specific ProviderCollection methods.
    public ProviderCollection<StorageProviderDefinition> GetStorageProviderDefinitions ()
    {
      return _storageProviderCollection;
    }


    private ConfigurationException CreateMissingDefaultProviderException ()
    {
      return new ConfigurationException(
          "Missing default storage provider. "
          + Environment.NewLine
          + "Please add an application or Web configuration file to your application, declare the "
          + "remotion.data.domainObjects section group, and add the storage element to configure the default storage provider. Alternatively, "
          + "explicitly assign all storage groups and queries to specific storage providers."
          + Environment.NewLine + Environment.NewLine
          + "Configuration File Example:"
          + Environment.NewLine +
          @"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
  <configSections>
    <sectionGroup name=""remotion.data.domainObjects"" type=""Remotion.Data.DomainObjects.Configuration.DomainObjectsConfiguration, Remotion.Data.DomainObjects"">
      <section name=""storage"" type=""Remotion.Data.DomainObjects.Persistence.Configuration.StorageConfiguration, Remotion.Data.DomainObjects"" />
    </sectionGroup>
  </configSections>

  <remotion.data.domainObjects xmlns=""http://www.re-motion.org/Data/DomainObjects/Configuration/2.1"">
    <storage defaultProviderDefinition=""Default"">
      <providerDefinitions>
        <add type=""Remotion.Data.DomainObjects::Persistence.Rdbms.RdbmsProviderDefinition""
             name=""Default""
             factoryType=""Remotion.Data.DomainObjects::Persistence.Rdbms.SqlServer.Sql2016.SqlStorageObjectFactory""
             connectionString=""DefaultConnection"" />
      </providerDefinitions>
    </storage>
  </remotion.data.domainObjects>

  <connectionStrings>
    <add name=""DefaultConnection"" connectionString=""Integrated Security=SSPI;Initial Catalog=TestDatabase;Data Source=localhost"" />
  </connectionStrings>
</configuration>");
    }

  }
}
