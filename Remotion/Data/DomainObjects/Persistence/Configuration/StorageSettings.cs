using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Enables configuration of the storage definition providers.
  /// </summary>
  /// <seealso cref="DeferredStorageSettings"/>
  public class StorageSettings : IStorageSettings
  {
    /// <summary>
    /// Gets the default storage provider definition.
    /// </summary>
    /// <value>The default storage provider definition or <see langword="null"/>.</value>
    public StorageProviderDefinition? DefaultStorageProviderDefinition => _defaultStorageProviderDefinition;

    /// <summary>
    /// Gets all storage provider definitions.
    /// </summary>
    public IReadOnlyCollection<StorageProviderDefinition> StorageProviderDefinitions => _storageProviderCollection;

    private readonly StorageProviderDefinition? _defaultStorageProviderDefinition;

    private readonly IReadOnlyCollection<StorageProviderDefinition> _storageProviderCollection;

    private readonly Dictionary<Type, StorageProviderDefinition> _storageGroupToProvider;

    public StorageSettings (
        StorageProviderDefinition? defaultStorageProviderDefinition,
        IReadOnlyCollection<StorageProviderDefinition> storageProviderCollection)
    {
      ArgumentUtility.CheckNotNull("storageProviderCollection", storageProviderCollection);

      _defaultStorageProviderDefinition = defaultStorageProviderDefinition;
      _storageProviderCollection = storageProviderCollection;

      CheckStorageProviderDefinitionsAreUnique();

      _storageGroupToProvider = new Dictionary<Type, StorageProviderDefinition>();
      InitializeStorageGroups();
    }

    /// <summary>
    /// Gets a storage provider definition based on its class definition.
    /// Returns the default storage provider if the <see cref="ClassDefinition.StorageGroupType"/> is null or no storage
    /// group with the given <see cref="ClassDefinition"/>.<see cref="ClassDefinition.StorageGroupType"/> could be found.
    /// An exception is thrown if no default storage provider is registered either.
    /// </summary>
    /// <returns>The provider with the <see cref="ClassDefinition"/>.<see cref="ClassDefinition.StorageGroupType"/>.</returns>
    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var storageGroupTypeOrNull = classDefinition.StorageGroupType;

      return GetStorageProviderDefinition(storageGroupTypeOrNull);
    }

    /// <summary>
    /// Gets a storage provider definition based on its storage group type.
    /// Returns the default storage provider if the <see cref="Type"/> is null or no storage
    /// group with the given <see cref="Type"/> could be found.
    /// An exception is thrown if no default storage provider is registered either.
    /// </summary>
    /// <returns>The provider with the <see cref="Type"/> supplied as a parameter. If no <see cref="Type"/> was supplied or no
    /// storage group pertaining to the supplied type was found, the default storage provider is returned instead.
    /// In this case, if the default storage provider is <see langword="null"/> as well, an exception is thrown.</returns>
    public StorageProviderDefinition GetStorageProviderDefinition (Type? storageGroupTypeOrNull)
    {
      if (storageGroupTypeOrNull == null)
        return DefaultStorageProviderDefinition ?? throw CreateMissingDefaultProviderException();

      if (!_storageGroupToProvider.TryGetValue(storageGroupTypeOrNull, out var storageProvider))
        return DefaultStorageProviderDefinition ?? throw CreateMissingDefaultProviderException();

      return storageProvider;
    }


    /// <summary>
    /// Gets a storage provider definition based on its name.
    /// </summary>
    public StorageProviderDefinition GetStorageProviderDefinition (string storageProviderName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("storageProviderName", storageProviderName);

      return _storageProviderCollection.FirstOrDefault(p => p.Name == storageProviderName)
             ?? throw new ConfigurationException($"The requested storage provider '{storageProviderName}' could not be found.");
    }

    private void CheckStorageProviderDefinitionsAreUnique ()
    {
      //we could also do this by putting all elements in a hashset and checking that way. Not sure which is better, but with the hashset we could name the non unique elements
      //and it should be faster as well. Thoughts?
      var count = _storageProviderCollection.Select(p => p.Name).Distinct().Count();

      if (count != _storageProviderCollection.Count)
      {
        throw new ConfigurationException("There were storage providers with non unique ids defined.");
      }
    }

    private void InitializeStorageGroups ()
    {
      foreach (var storageProvider in _storageProviderCollection)
      {
        foreach (var type in storageProvider.AssignedStorageGroups)
        {
          if (_storageGroupToProvider.ContainsKey(type) && _storageGroupToProvider[type] != storageProvider)
          {
            throw new ConfigurationException($"The storage group with type '{type}' is contained in more than one storage provider.");
          }
          _storageGroupToProvider[type] = storageProvider;
        }
      }
    }

    private ConfigurationException CreateMissingDefaultProviderException ()
    {
      return new ConfigurationException(
          "Missing default storage provider.\n"
          + "To properly set up the StorageSettings, add an implementation of IStorageSettingsFactory to the IoC container.\n"
          + "An example of an implementation for IStorageSettingsFactory can be found as Remotion.Data.DomainObjects.Persistence.Configuration.RdbmsStorageSettingsFactory.\n\n"

          + "Example that inserts a default instance of the RdbmsStorageSettingsFactory:" + @"
var serviceLocator = DefaultServiceLocator.Create();
serviceLocator.RegisterSingle(() => StorageSettingsFactory.CreateForSqlServer(""connectionString""));

var serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
");
    } }
}
