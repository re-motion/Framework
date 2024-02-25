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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Enables configuration of the set of <see cref="StorageProviderDefinition"/> instances available in the application.
  /// </summary>
  /// <seealso cref="DeferredStorageSettings"/>
  /// <threadsafety static="true" instance="true"/>
  public sealed class StorageSettings : IStorageSettings
  {
    private readonly StorageProviderDefinition? _defaultStorageProviderDefinition;
    private readonly IReadOnlyCollection<StorageProviderDefinition> _storageProviderDefinitions;
    private readonly IReadOnlyDictionary<Type, StorageProviderDefinition> _storageProviderByStorageGroupType;

    public StorageSettings (
        StorageProviderDefinition? defaultStorageProviderDefinition,
        IReadOnlyCollection<StorageProviderDefinition> storageProviderDefinitions)
    {
      ArgumentUtility.CheckNotNullOrItemsNull(nameof(storageProviderDefinitions), storageProviderDefinitions);

      var duplicates = storageProviderDefinitions.GroupBy(sp => sp.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
      if (duplicates.Count > 0)
      {
        throw new ArgumentException(
            $"Storage providers must have distinct names. The following duplicate names where found: '{string.Join("', '", duplicates)}'",
            nameof(storageProviderDefinitions));
      }

      _defaultStorageProviderDefinition = defaultStorageProviderDefinition;
      _storageProviderDefinitions = storageProviderDefinitions;
      _storageProviderByStorageGroupType = CreateStorageGroups(storageProviderDefinitions);
    }

    /// <inheritdoc />
    public StorageProviderDefinition? GetDefaultStorageProviderDefinition () => _defaultStorageProviderDefinition;

    /// <inheritdoc />
    public IReadOnlyCollection<StorageProviderDefinition> GetStorageProviderDefinitions () => _storageProviderDefinitions;

    /// <inheritdoc />
    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var storageGroupTypeOrNull = classDefinition.StorageGroupType;

      return GetStorageProviderDefinition(storageGroupTypeOrNull);
    }

    /// <inheritdoc />
    public StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull)
    {
      if (storageGroupTypeOrNull == null)
        return GetDefaultStorageProviderDefinition() ?? throw CreateMissingDefaultProviderException();

      if (!_storageProviderByStorageGroupType.TryGetValue(storageGroupTypeOrNull, out var storageProvider))
        return GetDefaultStorageProviderDefinition() ?? throw CreateMissingDefaultProviderException();

      return storageProvider;
    }

    /// <inheritdoc />
    public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("storageProviderName", storageProviderName);

      return _storageProviderDefinitions.FirstOrDefault(p => p.Name == storageProviderName)
             ?? throw new ConfigurationException($"The requested storage provider '{storageProviderName}' could not be found.");
    }

    private IReadOnlyDictionary<Type, StorageProviderDefinition> CreateStorageGroups (IReadOnlyCollection<StorageProviderDefinition> storageProviderDefinitions)
    {
      var storageGroupToProvider = new Dictionary<Type, StorageProviderDefinition>();

      foreach (var storageProvider in storageProviderDefinitions)
      {
        foreach (var type in storageProvider.AssignedStorageGroups)
        {
          if (storageGroupToProvider.TryGetValue(type, out var otherStorageProvider))
          {
            throw new ArgumentException(
                $"Storage group '{type}' is assigned to multiple storage providers: '{storageProvider.Name}' and '{otherStorageProvider.Name}'",
                nameof(storageProviderDefinitions));
          }
          storageGroupToProvider.Add(type, storageProvider);
        }
      }

      return storageGroupToProvider;
    }

    private ConfigurationException CreateMissingDefaultProviderException ()
    {
      return new ConfigurationException(
          """
          Missing default storage provider.

          To properly set up the StorageSettings, add an implementation of IStorageSettingsFactory to the IoC container.
          An example of an implementation for IStorageSettingsFactory can be found as Remotion.Data.DomainObjects.Persistence.Configuration.RdbmsStorageSettingsFactory.

          Example that inserts a default instance of the RdbmsStorageSettingsFactory:
          var serviceLocator = DefaultServiceLocator.Create();
          serviceLocator.RegisterSingle(() => StorageSettingsFactory.CreateForSqlServer("Data Source=DemoServer;Initial Catalog=AdventureWorks;Integrated Security=true;"));
          var serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
          """);
    } }
}
