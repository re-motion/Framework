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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents a top-level <see cref="ClientTransaction"/>, which does not have a parent transaction.
  /// </summary>
  [Serializable]
  public class RootPersistenceStrategy : IFetchEnabledPersistenceStrategy
  {
    private readonly Guid _transactionID;
    private readonly IPersistenceService _persistenceService;
    private readonly IStorageAccessResolver _storageAccessResolver;

    public RootPersistenceStrategy (Guid transactionID, IPersistenceService persistenceService, IStorageAccessResolver storageAccessResolver)
    {
      ArgumentUtility.CheckNotNull("persistenceService", persistenceService);
      ArgumentUtility.CheckNotNull("storageAccessResolver", storageAccessResolver);

      _transactionID = transactionID;
      _persistenceService = persistenceService;
      _storageAccessResolver = storageAccessResolver;
    }

    public Guid TransactionID
    {
      get { return _transactionID; }
    }

    public virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      using (var storageProviderManager = CreateStorageProviderManager())
      {
        return _persistenceService.CreateNewObjectID(storageProviderManager, classDefinition);
      }
    }

    public virtual ILoadedObjectData LoadObjectData (ObjectID id)
    {
      ArgumentUtility.CheckNotNull("id", id);

      var storageAccess = _storageAccessResolver.ResolveStorageAccessForLoadingDomainObjectsByObjectID();
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var result = _persistenceService.LoadDataContainer(storageProviderManager, id);
        return GetLoadedObjectDataForObjectLookupResult(result);
      }
    }

    public virtual IEnumerable<ILoadedObjectData> LoadObjectData (IEnumerable<ObjectID> objectIDs)
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      var storageAccess = _storageAccessResolver.ResolveStorageAccessForLoadingDomainObjectsByObjectID();
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var results = _persistenceService.LoadDataContainers(storageProviderManager, objectIDs);
        return results.Select(GetLoadedObjectDataForObjectLookupResult);
      }
    }

    public virtual ILoadedObjectData ResolveObjectRelationData (
        RelationEndPointID relationEndPointID,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      var storageAccess = _storageAccessResolver.ResolveStorageAccessForLoadingDomainObjectRelation();
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var dataContainer = _persistenceService.LoadRelatedDataContainer(storageProviderManager, relationEndPointID);
        return GetLoadedObjectDataForDataContainer(dataContainer, alreadyLoadedObjectDataProvider);
      }
    }

    public virtual IEnumerable<ILoadedObjectData> ResolveCollectionRelationData (
        RelationEndPointID relationEndPointID, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);
      ArgumentUtility.CheckNotNull("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      var storageAccess = _storageAccessResolver.ResolveStorageAccessForLoadingDomainObjectRelation();
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var dataContainers = _persistenceService.LoadRelatedDataContainers(storageProviderManager, relationEndPointID);
        return dataContainers.Select(dc => GetLoadedObjectDataForDataContainer(dc, alreadyLoadedObjectDataProvider));
      }
    }

    public virtual IEnumerable<ILoadedObjectData> ExecuteCollectionQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull("query", query);ArgumentUtility.CheckNotNull("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);


      if (query.QueryType != QueryType.CollectionReadWrite && query.QueryType != QueryType.CollectionReadOnly)
        throw new ArgumentException("Only collection queries can be used to load data containers.", "query");

      var dataContainers = ExecuteDataContainerQuery(query);
      return dataContainers.Select(dc => GetLoadedObjectDataForDataContainer(dc, alreadyLoadedObjectDataProvider));
    }

    public virtual IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      if (query.QueryType != QueryType.CustomReadOnly && query.QueryType != QueryType.CustomReadWrite)
        throw new ArgumentException("Only custom queries can be used to load custom results", "query");

      var storageAccess = ResolveStorageAccessForQuery(query);
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var provider = storageProviderManager.GetMandatory(query.StorageProviderDefinition.Name);
        // This foreach/yield combination is needed to force the using block to stay open until the whole result set has finished enumeration.
        foreach (var queryResultRow in provider.ExecuteCustomQuery(query))
        {
          yield return queryResultRow;
        }
      }
    }

    private IEnumerable<DataContainer?> ExecuteDataContainerQuery (IQuery query)
    {
      IEnumerable<DataContainer?> dataContainers;

      var storageAccess = ResolveStorageAccessForQuery(query);
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var provider = storageProviderManager.GetMandatory(query.StorageProviderDefinition.Name);
        dataContainers = provider.ExecuteCollectionQuery(query);
      }
      return dataContainers;
    }

    public virtual object? ExecuteScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      if (query.QueryType != QueryType.ScalarReadOnly && query.QueryType != QueryType.ScalarReadWrite)
        throw new ArgumentException("Only scalar queries can be used to load scalar results.", "query");

      var storageAccess = ResolveStorageAccessForQuery(query);
      using (var storageProviderManager = CreateStorageProviderManager(storageAccess))
      {
        var provider = storageProviderManager.GetMandatory(query.StorageProviderDefinition.Name);
        return provider.ExecuteScalarQuery(query);
      }
    }

    public virtual void PersistData (IEnumerable<PersistableData> data)
    {
      ArgumentUtility.CheckNotNull("data", data);

      // Filter out those items whose state is only Changed due to relation changes - we don't persist those
      var dataContainers = data.Select(item => item.DataContainer).Where(dc => !dc.State.IsUnchanged);
      var collection = new DataContainerCollection(dataContainers, false);

      if (collection.Count > 0)
      {
        using (var storageProviderManager = CreateStorageProviderManager())
        {
          _persistenceService.Save(storageProviderManager, collection);
        }
      }

      foreach (var dataContainer in collection)
        dataContainer.ClearNewInHierarchy();
    }

    public virtual IEnumerable<LoadedObjectDataWithDataSourceData> ExecuteFetchQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      ArgumentUtility.CheckNotNull("query", query);
      ArgumentUtility.CheckNotNull("alreadyLoadedObjectDataProvider", alreadyLoadedObjectDataProvider);

      if (query.QueryType != QueryType.CollectionReadOnly)
        throw new ArgumentException("Only readonly collection queries can be used for fetching.", "query");

      var dataContainers = ExecuteDataContainerQuery(query);
      return dataContainers.Select(dc => new LoadedObjectDataWithDataSourceData(GetLoadedObjectDataForDataContainer(dc, alreadyLoadedObjectDataProvider), dc));

    }

    private IReadOnlyStorageProviderManager CreateStorageProviderManager (StorageAccessType accessType)
    {
      return accessType switch
      {
          StorageAccessType.ReadOnly => new ReadOnlyStorageProviderManager(CreatePersistenceExtension()),
          StorageAccessType.ReadWrite => new StorageProviderManager(CreatePersistenceExtension()),
          _ => throw new NotSupportedException($"Unhandled storage access type: '{accessType.ToString()}'.")
      };
    }

    private IReadOnlyStorageProviderManager CreateReadOnlyStorageProviderManager ()
    {
      return new ReadOnlyStorageProviderManager(CreatePersistenceExtension());
    }

    private StorageAccessType ResolveStorageAccessForQuery (IQuery query)
    {
      return query.QueryType switch
      {
          QueryType.CollectionReadWrite => StorageAccessType.ReadWrite,
          QueryType.CustomReadWrite => StorageAccessType.ReadWrite,
          QueryType.ScalarReadWrite => StorageAccessType.ReadWrite,
          _ => _storageAccessResolver.ResolveStorageAccessForQuery(query)
      };
    }

    private IStorageProviderManager CreateStorageProviderManager ()
    {
      return new StorageProviderManager(CreatePersistenceExtension());
    }

    protected IPersistenceExtension CreatePersistenceExtension ()
    {
      var listenerFactory = SafeServiceLocator.Current.GetInstance<IPersistenceExtensionFactory>();
      return new CompoundPersistenceExtension(listenerFactory.CreatePersistenceExtensions(_transactionID));
    }

    private ILoadedObjectData GetLoadedObjectDataForDataContainer (DataContainer? dataContainer, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider)
    {
      if (dataContainer == null)
        return new NullLoadedObjectData();

      var knownLoadedObjectData = alreadyLoadedObjectDataProvider.GetLoadedObject(dataContainer.ID);
      return knownLoadedObjectData ?? new FreshlyLoadedObjectData(dataContainer);
    }

    private ILoadedObjectData GetLoadedObjectDataForObjectLookupResult (ObjectLookupResult<DataContainer> result)
    {
      if (result.LocatedObject == null)
        return new NotFoundLoadedObjectData(result.ObjectID);
      else
        return new FreshlyLoadedObjectData(result.LocatedObject);
    }
  }
}
