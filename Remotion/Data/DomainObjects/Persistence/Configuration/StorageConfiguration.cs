// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class StorageConfiguration: ExtendedConfigurationSection
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly StorageProviderDefinitionHelper _defaultStorageProviderDefinitionHelper;
    private readonly List<ProviderHelperBase> _providerHelpers = new List<ProviderHelperBase>();
    private readonly ConfigurationProperty _storageProviderGroupsProperty;

    public StorageConfiguration ()
    {
      _storageProviderGroupsProperty = new ConfigurationProperty(
          "groups",
          typeof(ConfigurationElementCollection<StorageGroupElement>),
          null,
          ConfigurationPropertyOptions.None);

      _defaultStorageProviderDefinitionHelper = new StorageProviderDefinitionHelper(this);
      _providerHelpers.Add(_defaultStorageProviderDefinitionHelper);

      _properties.Add(_storageProviderGroupsProperty);
      _providerHelpers.ForEach(current => current.InitializeProperties(_properties));
    }

    public StorageConfiguration (ProviderCollection<StorageProviderDefinition> providers, StorageProviderDefinition defaultProvider)
        : this()
    {
      ArgumentUtility.CheckNotNull("providers", providers);

      _defaultStorageProviderDefinitionHelper.Provider = defaultProvider;

      ProviderCollection<StorageProviderDefinition> providersCopy = CopyProvidersAsReadOnly(providers);
      _defaultStorageProviderDefinitionHelper.Providers = providersCopy;
    }

    public ConfigurationElementCollection<StorageGroupElement> StorageGroups
    {
      get { return (ConfigurationElementCollection<StorageGroupElement>)this[_storageProviderGroupsProperty]; }
    }

    public StorageProviderDefinition? DefaultStorageProviderDefinition
    {
      get { return _defaultStorageProviderDefinitionHelper.Provider; }
    }

    public ProviderCollection<StorageProviderDefinition> StorageProviderDefinitions
    {
      get { return _defaultStorageProviderDefinitionHelper.Providers; }
    }

    protected override void PostDeserialize ()
    {
      base.PostDeserialize();

      _providerHelpers.ForEach(current => current.PostDeserialze());
    }

    public ConfigurationException CreateMissingDefaultProviderException (string? context)
    {
      return new ConfigurationException(
          "Missing default storage provider. "
          + context
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

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    private ProviderCollection<StorageProviderDefinition> CopyProvidersAsReadOnly (ProviderCollection<StorageProviderDefinition> providers)
    {
      ProviderCollection<StorageProviderDefinition> providersCopy = new ProviderCollection<StorageProviderDefinition>();
      foreach (StorageProviderDefinition provider in providers)
        providersCopy.Add(provider);

      providersCopy.SetReadOnly();
      return providersCopy;
    }
  }
}
