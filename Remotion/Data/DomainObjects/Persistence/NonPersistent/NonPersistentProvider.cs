using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent
{
  public class NonPersistentProvider : StorageProvider
  {
    public NonPersistentProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
        : base(storageProviderDefinition, persistenceExtension)
    {
    }

    public override ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("id", id);
      CheckStorageProvider(id, "id");

      return new ObjectLookupResult<DataContainer>(id, null);
    }

    public override IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("ids", ids);

      var checkedIDs = ids.Select(id => CheckStorageProvider(id, "ids"));
      return checkedIDs.Select(id => new ObjectLookupResult<DataContainer>(id, null)).ToArray();
    }

    public override IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition? sortExpressionDefinition,
        ObjectID relatedID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("relatedID", relatedID);
      CheckTypeDefinition(relationEndPointDefinition.TypeDefinition, "typeDefinition");

      return new DataContainerCollection();
    }

    public override void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      // NOP
    }

    public override void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      foreach (var dataContainer in dataContainers)
        dataContainer.SetTimestamp(Guid.NewGuid().ToByteArray());
    }

    public override void BeginTransaction ()
    {
      CheckDisposed();
    }

    public override void Commit ()
    {
      CheckDisposed();
    }

    public override void Rollback ()
    {
      CheckDisposed();
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      CheckTypeDefinition(classDefinition, "classDefinition");

      return new ObjectID(classDefinition, Guid.NewGuid());
    }

    public override IEnumerable<DataContainer> ExecuteCollectionQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);

      return Enumerable.Empty<DataContainer>();
    }

    public override IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);

      return Enumerable.Empty<IQueryResultRow>();
    }

    public override object? ExecuteScalarQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);

      return null;
    }

    private ObjectID CheckStorageProvider (ObjectID id, string argumentName)
    {
      if (id.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException(
            argumentName,
            "The StorageProviderID '{0}' of the provided ObjectID '{1}' does not match with this StorageProvider's ID '{2}'.",
            id.StorageProviderDefinition.Name,
            id,
            StorageProviderDefinition.Name);
      }
      return id;
    }

    private void CheckTypeDefinition (TypeDefinition typeDefinition, string argumentName)
    {
      if (typeDefinition.StorageEntityDefinition.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException(
            argumentName,
            "The StorageProviderID '{0}' of the provided TypeDefinition does not match with this StorageProvider's ID '{1}'.",
            typeDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
            StorageProviderDefinition.Name);
      }
    }
  }
}
