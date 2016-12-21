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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Provides an abstract base implementation for classes encapsulating persistence-related functionality. Subclasses of <see cref="StorageProvider"/> 
  /// are used by <see cref="RootPersistenceStrategy"/> to load and store <see cref="DataContainer"/> 
  /// instances and execute queries.
  /// </summary>
  /// <remarks>
  /// Implementers must ensure that calls to the storage provider do not modify the internal state of the calling 
  /// <see cref="RootPersistenceStrategy"/>. They cannot use <see cref="ClientTransaction.Current"/> to 
  /// determine the calling <see cref="RootPersistenceStrategy"/> as that property is not guaranteed to be 
  /// set by the caller.
  /// </remarks>
  public abstract class StorageProvider : IDisposable
  {
    private StorageProviderDefinition _storageProviderDefinition;
    private bool _disposed;
    private readonly IPersistenceExtension _persistenceExtension;

    protected StorageProvider (
        StorageProviderDefinition storageProviderDefinition,
        IPersistenceExtension persistenceExtension)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("persistenceExtension", persistenceExtension);

      _storageProviderDefinition = storageProviderDefinition;
      _persistenceExtension = persistenceExtension;
    }

    ~StorageProvider ()
    {
      Dispose (false);
    }

    public void Dispose ()
    {
      Dispose (true);
      GC.SuppressFinalize (this);
    }

    protected virtual void Dispose (bool disposing)
    {
      if (disposing)
        _storageProviderDefinition = null;

      _disposed = true;
    }

    public abstract ObjectLookupResult<DataContainer> LoadDataContainer (ObjectID id);

    public abstract IEnumerable<ObjectLookupResult<DataContainer>> LoadDataContainers (IEnumerable<ObjectID> ids);

    public abstract IEnumerable<DataContainer> LoadDataContainersByRelatedID (
        RelationEndPointDefinition relationEndPointDefinition, SortExpressionDefinition sortExpressionDefinition, ObjectID relatedID);

    public abstract void Save (IEnumerable<DataContainer> dataContainers);
    public abstract void UpdateTimestamps (IEnumerable<DataContainer> dataContainers);
    public abstract void BeginTransaction ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract ObjectID CreateNewObjectID (ClassDefinition classDefinition);
    public abstract IEnumerable<DataContainer> ExecuteCollectionQuery (IQuery query);
    public abstract IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);
    public abstract object ExecuteScalarQuery (IQuery query);

    public StorageProviderDefinition StorageProviderDefinition
    {
      get
      {
        CheckDisposed();
        return _storageProviderDefinition;
      }
    }

    protected virtual void CheckQuery (IQuery query, QueryType expectedQueryType, string argumentName)
    {
      CheckDisposed();
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.StorageProviderDefinition != StorageProviderDefinition)
      {
        throw CreateArgumentException (
            "query",
            "The StorageProviderID '{0}' of the provided query '{1}' does not match with this StorageProvider's ID '{2}'.",
            query.StorageProviderDefinition.Name,
            query.ID,
            StorageProviderDefinition.Name);
      }

      if (query.QueryType != expectedQueryType)
        throw CreateArgumentException (argumentName, "Expected query type is '{0}', but was '{1}'.", expectedQueryType, query.QueryType);
    }

    protected bool IsDisposed
    {
      get { return _disposed; }
    }

    public IPersistenceExtension PersistenceExtension
    {
      get { return _persistenceExtension; }
    }

    protected void CheckDisposed ()
    {
      if (_disposed)
        throw new ObjectDisposedException ("StorageProvider", "A disposed StorageProvider cannot be accessed.");
    }

    protected ArgumentException CreateArgumentException (string argumentName, string formatString, params object[] args)
    {
      return new ArgumentException (string.Format (formatString, args), argumentName);
    }
  }
}