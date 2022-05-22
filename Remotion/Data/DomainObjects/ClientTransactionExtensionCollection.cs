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
using System.ComponentModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// A collection of <see cref="IClientTransactionExtension"/>s.
  /// </summary>
  [Serializable]
  public class ClientTransactionExtensionCollection : CommonCollection, IClientTransactionExtension
  {
    private readonly string _key;

    public ClientTransactionExtensionCollection (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);

      _key = key;
    }

    /// <summary>
    /// Gets an <see cref="IClientTransactionExtension"/> by the extension name.
    /// </summary>
    /// <param name="key">The <see cref="IClientTransactionExtension.Key"/> of the extension. Must not be <see langword="null"/> or 
    /// <see cref="System.String.Empty"/>.</param>
    /// <returns>The <see cref="IClientTransactionExtension"/> of the given <paramref name="key"/> or <see langword="null"/> if the name was not found.</returns>
    public IClientTransactionExtension? this[string key]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty("key", key);

        return (IClientTransactionExtension?)BaseGetObject(key);
      }
    }

    /// <summary>
    /// Gets the <see cref="IClientTransactionExtension"/> of a given <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The index of the extension to be retrieved.</param>
    /// <returns>The <see cref="IClientTransactionExtension"/> of the given <paramref name="index"/>.</returns>
    public IClientTransactionExtension this[int index]
    {
      get { return (IClientTransactionExtension)BaseGetObject(index); }
    }

    string IClientTransactionExtension.Key
    {
      get { return _key; }
    }

    /// <summary>
    /// Adds an <see cref="IClientTransactionExtension"/> to the collection.
    /// </summary>
    /// <param name="clientTransactionExtension">The extension to add. Must not be <see langword="null"/>.</param>
    /// <exception cref="InvalidOperationException">An extension with the same <see cref="IClientTransactionExtension.Key"/> as the given 
    /// <paramref name="clientTransactionExtension"/> is already part of the collection.</exception>
    /// <remarks>The order of the extensions in the collection is the order in which they are notified.</remarks>
    public void Add (IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNull("clientTransactionExtension", clientTransactionExtension);

      var key = clientTransactionExtension.Key;
      Assertion.IsNotNull(key, "IClientTransactionExtension.Key must not return null");

      if (BaseContainsKey(key))
        throw new InvalidOperationException(string.Format("An extension with key '{0}' is already part of the collection.", key));

      BaseAdd(key, clientTransactionExtension);
    }

    /// <summary>
    /// Removes an <see cref="IClientTransactionExtension"/> from the collection.
    /// </summary>
    /// <param name="key">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    public void Remove (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);

      BaseRemove(key);
    }

    /// <summary>
    /// Gets the index of an <see cref="IClientTransactionExtension"/> with a given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The name of the extension. Must not be <see langword="null"/> or <see cref="System.String.Empty"/>.</param>
    /// <returns>The index of the extension, or -1 if <paramref name="key"/> is not found.</returns>
    public int IndexOf (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);

      return BaseIndexOfKey(key);
    }

    /// <summary>
    /// Inserts an <see cref="IClientTransactionExtension"/> intto the collection at a specified index.
    /// </summary>
    /// <param name="clientTransactionExtension">The extension to insert. Must not be <see langword="null"/>.</param>
    /// <param name="index">The index where the extension should be inserted.</param>
    /// <exception cref="System.ArgumentException">An extension with the same <see cref="IClientTransactionExtension.Key"/> as the given 
    /// <paramref name="clientTransactionExtension"/> is already part of the collection.</exception>
    /// <remarks>The order of the extensions in the collection is the order in which they are notified.</remarks>
    public void Insert (int index, IClientTransactionExtension clientTransactionExtension)
    {
      ArgumentUtility.CheckNotNull("clientTransactionExtension", clientTransactionExtension);

      var key = clientTransactionExtension.Key;
      Assertion.IsNotNull(key, "IClientTransactionExtension.Key must not return null");

      if (BaseContainsKey(key))
        throw new InvalidOperationException(string.Format("An extension with key '{0}' is already part of the collection.", key));

      BaseInsert(index, key, clientTransactionExtension);
    }

    #region Notification methods

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void TransactionInitialize (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("clientTransaction", clientTransaction);

      for (int i = 0; i < Count; i++)
        this[i].TransactionInitialize(clientTransaction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void TransactionDiscard (ClientTransaction clientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("clientTransaction", clientTransaction);

      for (int i = 0; i < Count; i++)
        this[i].TransactionDiscard(clientTransaction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("parentClientTransaction", parentClientTransaction);

      for (int i = 0; i < Count; i++)
        this[i].SubTransactionCreating(parentClientTransaction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SubTransactionInitialize (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("parentClientTransaction", parentClientTransaction);
      ArgumentUtility.DebugCheckNotNull("subTransaction", subTransaction);

      for (int i = 0; i < Count; i++)
        this[i].SubTransactionInitialize(parentClientTransaction, subTransaction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
      ArgumentUtility.DebugCheckNotNull("parentClientTransaction", parentClientTransaction);
      ArgumentUtility.DebugCheckNotNull("subTransaction", subTransaction);

      for (int i = 0; i < Count; i++)
        this[i].SubTransactionCreated(parentClientTransaction, subTransaction);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
      ArgumentUtility.DebugCheckNotNull("type", type);

      for (int i = 0; i < Count; i++)
        this[i].NewObjectCreating(clientTransaction, type);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
      ArgumentUtility.DebugCheckNotNull("objectIDs", objectIDs);

      for (int i = 0; i < Count; i++)
        this[i].ObjectsLoading(clientTransaction, objectIDs);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> loadedDomainObjects)
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty("loadedDomainObjects", loadedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].ObjectsLoaded(clientTransaction, loadedDomainObjects);
    }

    public void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.DebugCheckNotNull("unloadedDomainObjects", unloadedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].ObjectsUnloading(clientTransaction, unloadedDomainObjects);
    }

    public void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> unloadedDomainObjects)
    {
      ArgumentUtility.DebugCheckNotNull("unloadedDomainObjects", unloadedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].ObjectsUnloaded(clientTransaction, unloadedDomainObjects);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ObjectDeleting (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      for (int i = 0; i < Count; i++)
        this[i].ObjectDeleting(clientTransaction, domainObject);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ObjectDeleted (ClientTransaction clientTransaction, IDomainObject domainObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);

      for (int i = 0; i < Count; i++)
        this[i].ObjectDeleted(clientTransaction, domainObject);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void PropertyValueReading (ClientTransaction clientTransaction, IDomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("propertyDefinition", propertyDefinition);

      for (int i = 0; i < Count; i++)
        this[i].PropertyValueReading(clientTransaction, domainObject, propertyDefinition, valueAccess);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void PropertyValueRead (ClientTransaction clientTransaction, IDomainObject domainObject, PropertyDefinition propertyDefinition, object? value, ValueAccess valueAccess)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("propertyDefinition", propertyDefinition);

      for (int i = 0; i < Count; i++)
        this[i].PropertyValueRead(clientTransaction, domainObject, propertyDefinition, value, valueAccess);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void PropertyValueChanging (ClientTransaction clientTransaction, IDomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("propertyDefinition", propertyDefinition);

      for (int i = 0; i < Count; i++)
        this[i].PropertyValueChanging(clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void PropertyValueChanged (ClientTransaction clientTransaction, IDomainObject domainObject, PropertyDefinition propertyDefinition, object? oldValue, object? newValue)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("propertyDefinition", propertyDefinition);

      for (int i = 0; i < Count; i++)
        this[i].PropertyValueChanged(clientTransaction, domainObject, propertyDefinition, oldValue, newValue);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RelationReading (ClientTransaction clientTransaction, IDomainObject domainObject, IRelationEndPointDefinition relationEndPointDefinition, ValueAccess valueAccess)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      for (int i = 0; i < Count; i++)
        this[i].RelationReading(clientTransaction, domainObject, relationEndPointDefinition, valueAccess);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RelationRead (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? relatedObject,
        ValueAccess valueAccess)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      for (int i = 0; i < Count; i++)
        this[i].RelationRead(clientTransaction, domainObject, relationEndPointDefinition, relatedObject, valueAccess);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RelationRead (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.DebugCheckNotNull("relatedObjects", relatedObjects);

      for (int i = 0; i < Count; i++)
        this[i].RelationRead(clientTransaction, domainObject, relationEndPointDefinition, relatedObjects, valueAccess);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RelationChanging (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      for (int i = 0; i < Count; i++)
        this[i].RelationChanging(clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RelationChanged (
        ClientTransaction clientTransaction,
        IDomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IDomainObject? oldRelatedObject,
        IDomainObject? newRelatedObject)
    {
      ArgumentUtility.DebugCheckNotNull("domainObject", domainObject);
      ArgumentUtility.DebugCheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      for (int i = 0; i < Count; i++)
        this[i].RelationChanged(clientTransaction, domainObject, relationEndPointDefinition, oldRelatedObject, newRelatedObject);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("queryResult", queryResult);

      return this
          .Cast<IClientTransactionExtension>()
          .Aggregate(queryResult, (current, extension) => extension.FilterQueryResult(clientTransaction, current));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Committing (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> changedDomainObjects, ICommittingEventRegistrar eventRegistrar)
    {
      ArgumentUtility.DebugCheckNotNull("changedDomainObjects", changedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].Committing(clientTransaction, changedDomainObjects, eventRegistrar);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void CommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
      ArgumentUtility.DebugCheckNotNull("committedData", committedData);

      for (int i = 0; i < Count; i++)
        this[i].CommitValidate(clientTransaction, committedData);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Committed (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> changedDomainObjects)
    {
      ArgumentUtility.DebugCheckNotNull("changedDomainObjects", changedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].Committed(clientTransaction, changedDomainObjects);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RollingBack (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> changedDomainObjects)
    {
      ArgumentUtility.DebugCheckNotNull("changedDomainObjects", changedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].RollingBack(clientTransaction, changedDomainObjects);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void RolledBack (ClientTransaction clientTransaction, IReadOnlyList<IDomainObject> changedDomainObjects)
    {
      ArgumentUtility.DebugCheckNotNull("changedDomainObjects", changedDomainObjects);

      for (int i = 0; i < Count; i++)
        this[i].RolledBack(clientTransaction, changedDomainObjects);
    }

    #endregion
  }
}
