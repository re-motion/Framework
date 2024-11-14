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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides a common interface for classes that can load <see cref="DataContainer"/> instances from a data source and persist them.
  /// </summary>
  public interface IPersistenceStrategy
  {
    /// <summary>
    /// Creates a new <see cref="ObjectID"/> for the given class definition. The <see cref="ObjectID"/> must be created in a such a way that it can 
    /// later be used to identify objects when persisting or loading <see cref="DataContainer"/> instances.
    /// </summary>
    /// <param name="classDefinition">The class definition to create a new <see cref="ObjectID"/> for.</param>
    /// <returns>A new <see cref="ObjectID"/> for the given class definition.</returns>
    ObjectID CreateNewObjectID (ClassDefinition classDefinition);

    /// <summary>
    /// Loads the data for the given <see cref="ObjectID"/> from the underlying data source.
    /// </summary>
    /// <param name="id">The id of the data to load.</param>
    /// <returns>An <see cref="ILoadedObjectData"/> instance for the given <paramref name="id"/>. Items that
    /// couldn't be found are represented by <see cref="NotFoundLoadedObjectData"/> objects.</returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container. 
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    ILoadedObjectData LoadObjectData (ObjectID id);

    /// <summary>
    /// Loads the data for a number of <see cref="ObjectID"/> values from the underlying data source.
    /// </summary>
    /// <param name="objectIDs">The ids of the data to load.</param>
    /// <returns>A sequence of <see cref="ILoadedObjectData"/> instances in the same order as in <paramref name="objectIDs"/>. Items that
    /// couldn't be found are represented by <see cref="NotFoundLoadedObjectData"/> objects.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    IEnumerable<ILoadedObjectData> LoadObjectData (IEnumerable<ObjectID> objectIDs);

    /// <summary>
    /// Resolves the relation identified by the given <see cref="RelationEndPointID"/>, loading the related object's data unless already available.
    /// </summary>
    /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> of the end point that should be resolved.
    /// <paramref name="relationEndPointID"/> must refer to an <see cref="ObjectEndPoint"/> (i.e., represent a single object, not a collection).
    /// Must not be <see langword="null"/>.</param>
    /// <param name="alreadyLoadedObjectDataProvider">An implementation of <see cref="ILoadedObjectDataProvider"/> that is used to determine
    /// whether the result object is already known by the <see cref="ClientTransaction"/>. If so, the existing object data is returned; otherwise,
    /// the data is loaded and returned.</param>
    /// <returns>
    /// An <see cref="ILoadedObjectData"/> instance representing the related object. If the object already exists, the existing object
    /// is returned. Otherwise, a new one is created. If the related object is <see langword="null"/>, a <see cref="NullLoadedObjectData"/> instance
    /// is returned.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.InvalidCastException"><paramref name="relationEndPointID"/> does not refer to an
    /// <see cref="ObjectEndPoint"/></exception>
    /// <exception cref="Persistence.PersistenceException">
    /// The related object could not be loaded, but is mandatory.<br/> -or- <br/>
    /// The relation refers to non-existing object.<br/> -or- <br/>
    /// 	<paramref name="relationEndPointID"/> does not refer to an <see cref="ObjectEndPoint"/>.
    /// </exception>
    /// <exception cref="Persistence.StorageProviderException">
    /// The Mapping does not contain a class definition for the given <paramref name="relationEndPointID"/>.<br/> -or- <br/>
    /// An error occurred while accessing the data source.
    /// </exception>
    /// <remarks>
    /// 	<para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller.
    /// </para>
    /// 	<para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    ILoadedObjectData ResolveObjectRelationData (
        RelationEndPointID relationEndPointID,
        ILoadedObjectDataProvider alreadyLoadedObjectDataProvider);

    /// <summary>
    /// Resolves the relation identified by the given <see cref="RelationEndPointID"/>, loading all the related objects' data unless already available.
    /// </summary>
    /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> of the end point that should be evaluated.
    ///   <paramref name="relationEndPointID"/> must refer to a <see cref="DomainObjectCollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
    /// <param name="alreadyLoadedObjectDataProvider">An implementation of <see cref="ILoadedObjectDataProvider"/> that is used to determine
    ///   whether the result object is already known by the <see cref="ClientTransaction"/>. If so, the existing object data is returned; otherwise,
    ///   the data is loaded and returned.</param>
    /// <returns>
    /// A sequence of <see cref="ILoadedObjectData"/> instances representing the related objects. If an object already exists, the existing object
    /// is returned. Otherwise, a new one is created.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.PersistenceException">
    /// 	<paramref name="relationEndPointID"/> does not refer to one-to-many relation.<br/> -or- <br/>
    /// The StorageProvider for the related objects could not be initialized.
    /// </exception>
    IEnumerable<ILoadedObjectData> ResolveCollectionRelationData (RelationEndPointID relationEndPointID, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider);

    /// <summary>
    /// Executes the given <see cref="IQuery"/>, loading the result objects' data unless already available.
    /// </summary>
    /// <param name="query">The <see cref="IQuery"/> to be executed.</param>
    /// <param name="alreadyLoadedObjectDataProvider">An implementation of <see cref="ILoadedObjectDataProvider"/> that is used to determine
    ///   whether the result object is already known by the <see cref="ClientTransaction"/>. If so, the existing object data is returned; otherwise,
    ///   the data is loaded and returned.</param>
    /// <returns>
    /// A sequence of <see cref="ILoadedObjectData"/> instances representing the result of the query.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="QueryType"/> of
    /// <see cref="QueryType.CollectionReadOnly"/> or <see cref="QueryType.CollectionReadWrite"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    /// The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    /// The <see cref="Remotion.Data.DomainObjects.Persistence.IReadOnlyStorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    /// An error occurred while executing the query.
    /// </exception>
    IEnumerable<ILoadedObjectData> ExecuteCollectionQuery (IQuery query, ILoadedObjectDataProvider alreadyLoadedObjectDataProvider);

    /// <summary>
    /// Executes the given custom <see cref="IQuery"/>.
    /// </summary>
    /// <param name="query">The <see cref="IQuery"/> to be executed.</param>
    /// <returns>A collection of <see cref="IQueryResultRow"/> instances representing the result of the query.</returns>
    IEnumerable<IQueryResultRow> ExecuteCustomQuery (IQuery query);

    /// <summary>
    /// Executes the given <see cref="IQuery"/> and returns its result as a scalar value.
    /// </summary>
    /// <param name="query">The query to be executed.</param>
    /// <returns>The scalar query result.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="QueryType"/>
    ///   of <see cref="QueryType.ScalarReadOnly"/> or <see cref="QueryType.ScalarReadWrite"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    /// The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    /// The <see cref="Remotion.Data.DomainObjects.Persistence.IStorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    /// An error occurred while executing the query.
    /// </exception>
    object? ExecuteScalarQuery (IQuery query);

    /// <summary>
    /// Persists the given data.
    /// </summary>
    /// <param name="data">The <see cref="PersistableData"/> items describing the data to be persisted.</param>
    /// <remarks>
    /// The caller must ensure that the data represented by <paramref name="data"/> is consistent and complete; otherwise an inconsistent state
    /// might arise in the underlying data store.
    /// </remarks>
    void PersistData (IEnumerable<PersistableData> data);
  }
}
