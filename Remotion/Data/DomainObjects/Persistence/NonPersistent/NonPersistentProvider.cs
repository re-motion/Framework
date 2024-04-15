using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent
{
  public class NonPersistentProvider : IStorageProvider
  {
    private StorageProviderDefinition _storageProviderDefinition;
    private bool _disposed;

    public NonPersistentProvider (StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull("storageProviderDefinition", storageProviderDefinition);

      _storageProviderDefinition = storageProviderDefinition;
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get
      {
        CheckDisposed();
        return _storageProviderDefinition;
      }
    }

    public ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("id", id);
      CheckStorageProvider(id, "id");

      return new ObjectLookupResult<DataContainer>(id, null);
    }

    public IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IReadOnlyCollection<ObjectID> ids)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("ids", ids);

      var checkedIDs = ids.Select(id => CheckStorageProvider(id, "ids"));
      return checkedIDs.Select(id => new ObjectLookupResult<DataContainer>(id, null)).ToArray();
    }

    public IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition,
        SortExpressionDefinition? sortExpressionDefinition,
        ObjectID relatedID)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("relatedID", relatedID);
      CheckClassDefinition(relationEndPointDefinition.ClassDefinition, "classDefinition");

      return new DataContainerCollection();
    }

    public void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      // NOP
    }

    public void UpdateTimestamps (IReadOnlyCollection<DataContainer> dataContainers)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      foreach (var dataContainer in dataContainers)
        dataContainer.SetTimestamp(Guid.NewGuid().ToByteArray());
    }

    public void BeginTransaction ()
    {
      CheckDisposed();
    }

    public void Commit ()
    {
      CheckDisposed();
    }

    public void Rollback ()
    {
      CheckDisposed();
    }

    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      CheckClassDefinition(classDefinition, "classDefinition");

      return new ObjectID(classDefinition, Guid.NewGuid());
    }

    public IEnumerable<DataContainer> ExecuteCollectionQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);

      return Enumerable.Empty<DataContainer>();
    }

    public IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);

      return Enumerable.Empty<IQueryResultRow>();
    }

    public object? ExecuteScalarQuery (IQuery query)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull("query", query);

      return null;
    }

    public void Dispose ()
    {
      _storageProviderDefinition = null!;
      _disposed = true;
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

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageEntityDefinition.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException(
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name,
            StorageProviderDefinition.Name);
      }
    }

    private void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException("NonPersistentProvider", "A disposed NonPersistentProvider cannot be accessed.");
    }

    private ArgumentException CreateArgumentException (string argumentName, string formatString, params object[] args)
    {
      return new ArgumentException(string.Format(formatString, args), argumentName);
    }
  }
}
