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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <threadsafety static="true" instance="false" />
  [Serializable]
  [ImplementationFor(typeof(IPersistenceService), Lifetime = LifetimeKind.Singleton)]
  public class PersistenceService : IPersistenceService,
#pragma warning disable SYSLIB0050
      IObjectReference
#pragma warning restore SYSLIB0050
  {
    private class TransactionContext : IDisposable
    {
      public TransactionContext ()
      {
      }

      public void Dispose ()
      {
      }
    }

    public PersistenceService ()
    {
    }

    public ObjectID CreateNewObjectID (IStorageProviderManager storageProviderManager, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderManager), storageProviderManager);
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      var provider = storageProviderManager.GetMandatory(classDefinition.StorageEntityDefinition.StorageProviderDefinition);
      return provider.CreateNewObjectID(classDefinition);
    }

    public void Save (IStorageProviderManager storageProviderManager, DataContainerCollection dataContainers)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderManager), storageProviderManager);
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      if (dataContainers.Count == 0)
        return;

      var groupedDataContainers = dataContainers
          .ToLookup(dataContainer => dataContainer.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition)
          .Select(group => new { Provider = storageProviderManager.GetMandatory(group.Key), DataContainers = group.ToArray() })
          .ToArray();

      var providers = groupedDataContainers.Select(group => group.Provider).ToArray();
      CheckProvidersCompatibleForSave(providers);

      using (var saveContext = BeginTransaction(providers))
      {
        try
        {
          foreach (var group in groupedDataContainers)
          {
            group.Provider.Save(group.DataContainers);
            group.Provider.UpdateTimestamps(group.DataContainers.Where(dc => !dc.State.IsDeleted).ToArray());
          }

          CommitTransaction(providers, saveContext);
        }
        catch
        {
          try
          {
            RollbackTransaction(providers, saveContext);
          }
          // ReSharper disable EmptyGeneralCatchClause
          catch
              // ReSharper restore EmptyGeneralCatchClause
          {
          }

          throw;
        }
      }
    }

    public ObjectLookupResult<DataContainer> LoadDataContainer (IReadOnlyStorageProviderManager storageProviderManager, ObjectID id)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderManager), storageProviderManager);
      ArgumentUtility.CheckNotNull("id", id);

      var provider = storageProviderManager.GetMandatory(id.StorageProviderDefinition);
      var result = provider.LoadDataContainer(id);

      return result;
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyStorageProviderManager storageProviderManager, IEnumerable<ObjectID> ids)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderManager), storageProviderManager);
      ArgumentUtility.CheckNotNull("ids", ids);

      var idCollection = ids.ConvertToCollection();
      var idsByProvider = GroupIDsByProvider(idCollection);

      var unorderedResultDictionary = idsByProvider
          .SelectMany(idGroup => storageProviderManager.GetMandatory(idGroup.Key).LoadDataContainers(idGroup.Value))
          .ToLookup(dataContainerLookupResult => dataContainerLookupResult.ObjectID);
      return idCollection.Select(id => unorderedResultDictionary[id].First());
    }

    public DataContainerCollection LoadRelatedDataContainers (IReadOnlyStorageProviderManager storageProviderManager, RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderManager), storageProviderManager);
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
      {
        throw CreatePersistenceException(
            "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
            + " relation: '{0}', property: '{1}'. Check your mapping configuration.",
            relationEndPointID.Definition.RelationDefinition.ID,
            relationEndPointID.Definition.PropertyName);
      }

      var relationEndPointDefinition = relationEndPointID.Definition;
      if (relationEndPointDefinition.Cardinality == CardinalityType.One)
      {
        throw CreatePersistenceException(
            "Cannot load multiple related data containers for one-to-one relation '{0}'.",
            relationEndPointDefinition.RelationDefinition.ID);
      }

      var oppositeDataContainers = LoadOppositeDataContainers(storageProviderManager, relationEndPointID);

      if (relationEndPointDefinition.IsMandatory && oppositeDataContainers.Count == 0)
      {
        throw CreatePersistenceException(
            "Collection for mandatory relation property '{0}' on object '{1}' contains no items.",
            relationEndPointDefinition.PropertyName,
            relationEndPointID.ObjectID);
      }

      return oppositeDataContainers;
    }

    public DataContainer? LoadRelatedDataContainer (IReadOnlyStorageProviderManager storageProviderManager, RelationEndPointID relationEndPointID)
    {
      ArgumentUtility.CheckNotNull(nameof(storageProviderManager), storageProviderManager);
      ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

      if (!relationEndPointID.Definition.IsVirtual)
        throw new ArgumentException("LoadRelatedDataContainer can only be used with virtual end points.", "relationEndPointID");

      return GetOppositeDataContainerForVirtualEndPoint(storageProviderManager, relationEndPointID);
    }

    /// <summary>
    /// Extension point for supporting multiple <see cref="IStorageProvider"/> with changed data during a single <see cref="Save"/> operation.
    /// </summary>
    /// <param name="providers">The set of <see cref="IStorageProvider"/> in the current operation.</param>
    /// <remarks>
    /// When extending <see cref="CheckProvidersCompatibleForSave"/> to support multiple <see cref="IStorageProvider"/>, also extend
    /// <see cref="BeginTransaction"/>, <see cref="CommitTransaction"/>, and <see cref="RollbackTransaction"/> with appropriate logic.
    /// </remarks>
    protected virtual void CheckProvidersCompatibleForSave (IEnumerable<IStorageProvider> providers)
    {
      ArgumentUtility.CheckNotNull("providers", providers);

      var persistentStorageProviders = providers.Where(p => !(p is NonPersistentProvider));

      if (persistentStorageProviders.Count() > 1)
        throw CreatePersistenceException("Save does not support multiple storage providers.");
    }

    /// <summary>
    /// Extension point for beginning a transaction.
    /// </summary>
    /// <param name="providers">The set of <see cref="IReadOnlyStorageProvider"/> or <see cref="IStorageProvider"/> in the current operation.</param>
    /// <returns>
    /// A custom context, passed back to <see cref="CommitTransaction"/> and <see cref="RollbackTransaction"/>. The <see cref="IDisposable.Dispose"/>
    /// method is invoked at the call site.
    /// </returns>
    protected virtual IDisposable BeginTransaction (IEnumerable<IReadOnlyStorageProvider> providers)
    {
      ArgumentUtility.CheckNotNull("providers", providers);

      foreach (var provider in providers)
        provider.BeginTransaction();

      return new TransactionContext();
    }

    /// <summary>
    /// Extension point for committing a transaction.
    /// </summary>
    /// <param name="providers">The set of <see cref="IReadOnlyStorageProvider"/> or <see cref="IStorageProvider"/> in the current operation.</param>
    /// <param name="context">
    /// A custom context, created by <see cref="BeginTransaction"/>. The <see cref="IDisposable.Dispose"/> method is invoked at the call site.
    /// </param>
    protected virtual void CommitTransaction (IEnumerable<IReadOnlyStorageProvider> providers, IDisposable context)
    {
      ArgumentUtility.CheckNotNull("providers", providers);

      foreach (var provider in providers)
        provider.Commit();
    }

    /// <summary>
    /// Extension point for rolling a transaction back.
    /// </summary>
    /// <param name="providers">The set of <see cref="IReadOnlyStorageProvider"/> or <see cref="IStorageProvider"/> in the current operation.</param>
    /// <param name="context">
    /// A custom context, created by <see cref="BeginTransaction"/>. The <see cref="IDisposable.Dispose"/> method is invoked at the call site.
    /// </param>
    protected virtual void RollbackTransaction (IEnumerable<IReadOnlyStorageProvider> providers, IDisposable context)
    {
      ArgumentUtility.CheckNotNull("providers", providers);

      foreach (var provider in providers)
      {
        try
        {
          provider.Rollback();
        }
        // ReSharper disable EmptyGeneralCatchClause
        catch
            // ReSharper restore EmptyGeneralCatchClause
        {
        }
      }
    }

    private DataContainer? GetOppositeDataContainerForVirtualEndPoint (IReadOnlyStorageProviderManager storageProviderManager, RelationEndPointID relationEndPointID)
    {
      var relationEndPointDefinition = relationEndPointID.Definition;
      if (relationEndPointDefinition.Cardinality == CardinalityType.Many)
      {
        throw CreatePersistenceException(
            "Cannot load a single related data container for one-to-many relation '{0}'.",
            relationEndPointDefinition.RelationDefinition.ID);
      }

      var oppositeDataContainers = LoadOppositeDataContainers(storageProviderManager, relationEndPointID);
      if (oppositeDataContainers.Count > 1)
      {
        throw CreatePersistenceException(
            "Multiple related DataContainers where found for property '{0}' of DataContainer '{1}'.",
            relationEndPointDefinition.PropertyName,
            relationEndPointID.ObjectID);
      }

      if (oppositeDataContainers.Count == 0)
      {
        if (relationEndPointDefinition.IsMandatory)
        {
          throw CreatePersistenceException(
            "Mandatory relation property '{0}' on object '{1}' contains no item.",
            relationEndPointDefinition.PropertyName,
            relationEndPointID.ObjectID);
        }

        return null;
      }

      return oppositeDataContainers[0];
    }

    private DataContainerCollection LoadOppositeDataContainers (IReadOnlyStorageProviderManager storageProviderManager, RelationEndPointID relationEndPointID)
    {
      var relationEndPointDefinition = relationEndPointID.Definition;
      Assertion.IsTrue(relationEndPointDefinition.IsVirtual, "RelationEndPointDefinition for '{0}' is not virtual.", relationEndPointID);
      Assertion.IsFalse(relationEndPointDefinition.IsAnonymous, "RelationEndPointDefinition for '{0}' is anonymous.", relationEndPointID);
      Assertion.DebugIsNotNull(relationEndPointID.ObjectID, "relationEndPointID.ObjectID != null");

      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition();
      var oppositeProvider =
          storageProviderManager.GetMandatory(oppositeEndPointDefinition.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      SortExpressionDefinition? sortExpression;
      if (relationEndPointDefinition is DomainObjectCollectionRelationEndPointDefinition domainObjectCollectionRelationEndPointDefinition)
        sortExpression = domainObjectCollectionRelationEndPointDefinition.GetSortExpression();
      else
        sortExpression = null;

      var oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID(
          (RelationEndPointDefinition)oppositeEndPointDefinition,
          sortExpression,
          relationEndPointID.ObjectID);

      var oppositeDataContainerCollection = new DataContainerCollection();
      foreach (var oppositeDataContainer in oppositeDataContainers)
      {
        CheckClassIDForVirtualEndPoint(relationEndPointID, oppositeDataContainer);
        oppositeDataContainerCollection.Add(oppositeDataContainer);
      }
      return oppositeDataContainerCollection;
    }

    private void CheckClassIDForVirtualEndPoint (
        RelationEndPointID relationEndPointID,
        DataContainer oppositeDataContainer)
    {
      Assertion.DebugAssert(relationEndPointID.Definition.IsAnonymous == false, "relationEndPointID.Definition.IsAnonymous == false");
      Assertion.DebugIsNotNull(relationEndPointID.ObjectID, "relationEndPointID.ObjectID != null");

      var oppositeEndPointDefinition = (RelationEndPointDefinition)relationEndPointID.Definition.GetOppositeEndPointDefinition();
      var objectID = (ObjectID?)oppositeDataContainer.GetValueWithoutEvents(oppositeEndPointDefinition.PropertyDefinition, ValueAccess.Current);

      Assertion.IsNotNull(
          objectID,
          "The property '{0}' of the loaded DataContainer '{1}' is null.",
          oppositeEndPointDefinition.PropertyName,
          oppositeDataContainer.ID);

      if (relationEndPointID.ObjectID.ClassID != objectID.ClassID)
      {
        throw CreatePersistenceException(
            "The property '{0}' of the loaded DataContainer '{1}'"
            + " refers to ClassID '{2}', but the actual ClassID is '{3}'.",
            oppositeEndPointDefinition.PropertyName,
            oppositeDataContainer.ID,
            objectID.ClassID,
            relationEndPointID.ObjectID.ClassID);
      }
    }

    private PersistenceException CreatePersistenceException (string message, params object?[] args)
    {
      return new PersistenceException(string.Format(message, args));
    }

    private IEnumerable<KeyValuePair<StorageProviderDefinition, List<ObjectID>>> GroupIDsByProvider (IEnumerable<ObjectID> ids)
    {
      var result = new MultiDictionary<StorageProviderDefinition, ObjectID>();
      foreach (var id in ids)
        result[id.StorageProviderDefinition].Add(id);

      return result;
    }

    /// <inheritdoc />
    object IObjectReference.GetRealObject (StreamingContext context) => SafeServiceLocator.Current.GetInstance<IPersistenceService>();
  }
}
