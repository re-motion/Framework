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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides a default implementation of <see cref="IClientTransactionExtension"/>, acting as a base class for extensions that need only override
  /// a few of the <see cref="IClientTransactionExtension"/> notification methods.
  /// </summary>
  /// <remarks>
  /// This class implements all notification methods of <see cref="IClientTransactionExtension"/> as virtual "no-op" methods. Concrete derivations
  /// can override one or more of these methods in order to react on the notification methods. It is not required to call the base implementation.
  /// </remarks>
  [Serializable]
  public abstract class ClientTransactionExtensionBase : IClientTransactionExtension
  {
    private readonly string _key;

    protected ClientTransactionExtensionBase (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);
      _key = key;
    }

    public string Key
    {
      get { return _key; }
    }

    public virtual void TransactionInitialize (ClientTransaction clientTransaction)
    {
    }

    public virtual void TransactionDiscard (ClientTransaction clientTransaction)
    {
    }

    public virtual void SubTransactionCreating (ClientTransaction parentClientTransaction)
    {
    }

    public virtual void SubTransactionInitialize (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
    }

    public virtual void SubTransactionCreated (ClientTransaction parentClientTransaction, ClientTransaction subTransaction)
    {
    }

    public virtual void NewObjectCreating (ClientTransaction clientTransaction, Type type)
    {
    }

    public virtual void ObjectsLoading (ClientTransaction clientTransaction, IReadOnlyList<ObjectID> objectIDs)
    {
    }

    public virtual void ObjectsLoaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> loadedDomainObjects)
    {
    }

    public virtual void ObjectsUnloading (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectsUnloaded (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> unloadedDomainObjects)
    {
    }

    public virtual void ObjectDeleting (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void ObjectDeleted (ClientTransaction clientTransaction, DomainObject domainObject)
    {
    }

    public virtual void PropertyValueReading (ClientTransaction clientTransaction, DomainObject domainObject, PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? value,
        ValueAccess valueAccess)
    {
    }

    public virtual void PropertyValueChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
    }

    public virtual void PropertyValueChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        PropertyDefinition propertyDefinition,
        object? oldValue,
        object? newValue)
    {
    }

    public virtual void RelationReading (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? relatedObject,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationRead (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        IReadOnlyCollectionData<IDomainObject> relatedObjects,
        ValueAccess valueAccess)
    {
    }

    public virtual void RelationChanging (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
    }

    public virtual void RelationChanged (
        ClientTransaction clientTransaction,
        DomainObject domainObject,
        IRelationEndPointDefinition relationEndPointDefinition,
        DomainObject? oldRelatedObject,
        DomainObject? newRelatedObject)
    {
    }

    public virtual QueryResult<T> FilterQueryResult<T> (ClientTransaction clientTransaction, QueryResult<T> queryResult) where T: DomainObject
    {
      return queryResult;
    }

    public virtual void Committing (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> changedDomainObjects, ICommittingEventRegistrar eventRegistrar)
    {
    }

    public virtual void CommitValidate (ClientTransaction clientTransaction, IReadOnlyList<PersistableData> committedData)
    {
    }

    public virtual void Committed (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> changedDomainObjects)
    {
    }

    public virtual void RollingBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> changedDomainObjects)
    {
    }

    public virtual void RolledBack (ClientTransaction clientTransaction, IReadOnlyList<DomainObject> changedDomainObjects)
    {
    }

    /// <summary>
    /// Installs this extension with the specified <see cref="ClientTransaction"/>, returning a value indicating whether the operation succeeded.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> to register with.</param>
    /// <returns><see langword="true" /> if the extension was added to the <see cref="ClientTransaction"/>; <see langword="false" /> if the extension
    /// couldn't be added because another <see cref="IClientTransactionExtension"/> with the same key already exists in the transaction.</returns>
    /// <remarks>
    /// When a <see cref="IClientTransactionExtension"/> needs to be propagated from parent transactions to their subtransactions, override 
    /// <see cref="SubTransactionCreated"/> and use <see cref="TryInstall"/> to install the extension with the newly created subtransaction.
    /// </remarks>
    protected bool TryInstall (ClientTransaction clientTransaction)
    {
      if (clientTransaction.Extensions[Key] != null)
        return false;

      clientTransaction.Extensions.Add(this);
      return true;
    }
  }
}
