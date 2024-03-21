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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Implements creation, retrieval, and deletion of <see cref="DomainObject"/> references.
  /// </summary>
  [Serializable]
  public class ObjectLifetimeAgent : IObjectLifetimeAgent
  {
    private readonly ClientTransaction _clientTransaction;
    private readonly IClientTransactionEventSink _eventSink;
    private readonly IInvalidDomainObjectManager _invalidDomainObjectManager;
    private readonly IDataManager _dataManager;
    private readonly IEnlistedDomainObjectManager _enlistedDomainObjectManager;
    private readonly IPersistenceStrategy _persistenceStrategy;

    public ObjectLifetimeAgent (
        ClientTransaction clientTransaction,
        IClientTransactionEventSink eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager,
        IEnlistedDomainObjectManager enlistedDomainObjectManager,
        IPersistenceStrategy persistenceStrategy)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("eventSink", eventSink);
      ArgumentUtility.CheckNotNull("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("persistenceStrategy", persistenceStrategy);

      _clientTransaction = clientTransaction;
      _eventSink = eventSink;
      _invalidDomainObjectManager = invalidDomainObjectManager;
      _dataManager = dataManager;
      _enlistedDomainObjectManager = enlistedDomainObjectManager;
      _persistenceStrategy = persistenceStrategy;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IClientTransactionEventSink EventSink
    {
      get { return _eventSink; }
    }

    public IInvalidDomainObjectManager InvalidDomainObjectManager
    {
      get { return _invalidDomainObjectManager; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public IEnlistedDomainObjectManager EnlistedDomainObjectManager
    {
      get { return _enlistedDomainObjectManager; }
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public DomainObject NewObject (ClassDefinition classDefinition, ParamList constructorParameters)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("constructorParameters", constructorParameters);

      if (classDefinition.IsAbstract)
        throw new InvalidOperationException(
            string.Format(
                "Cannot instantiate type '{0}' because it is abstract. For classes with automatic properties, InstantiableAttribute must be used.",
                classDefinition.Type));

      _eventSink.RaiseNewObjectCreatingEvent(classDefinition.Type);

      var objectID = _persistenceStrategy.CreateNewObjectID(classDefinition);
      var initializationContext = new NewObjectInitializationContext(
          objectID, _clientTransaction.RootTransaction, _enlistedDomainObjectManager, _dataManager);

      try
      {
        return classDefinition.InstanceCreator.CreateNewObject(initializationContext, constructorParameters, _clientTransaction);
      }
      catch (Exception ex)
      {
        if (initializationContext.RegisteredObject != null)
          CleanupCreatedObject(objectID, initializationContext.RegisteredObject, ex);
        throw;
      }
    }

    public DomainObject GetObjectReference (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (_invalidDomainObjectManager.IsInvalid(objectID))
        return _invalidDomainObjectManager.GetInvalidObjectReference(objectID);

      var enlistedObject = _enlistedDomainObjectManager.GetEnlistedDomainObject(objectID);
      if (enlistedObject != null)
        return enlistedObject;

      var creator = objectID.ClassDefinition.InstanceCreator;
      var initializationContext = new ObjectReferenceInitializationContext(
          objectID, _clientTransaction.RootTransaction, _enlistedDomainObjectManager);
      return creator.CreateObjectReference(initializationContext, _clientTransaction);
    }

    public DomainObject GetObject (ObjectID objectID, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      // GetDataContainerWithLazyLoad throws on invalid objectID
      var dataContainer = _dataManager.GetDataContainerWithLazyLoad(objectID, throwOnNotFound: true)!;

      if (dataContainer.State.IsDeleted && !includeDeleted)
        throw new ObjectDeletedException(objectID);

      return dataContainer.DomainObject;
    }

    public DomainObject? TryGetObject (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);

      if (_invalidDomainObjectManager.IsInvalid(objectID))
        return _invalidDomainObjectManager.GetInvalidObjectReference(objectID);

      var dataContainer = _dataManager.GetDataContainerWithLazyLoad(objectID, throwOnNotFound: false);
      if (dataContainer == null)
        return null;

      return dataContainer.DomainObject;
    }

    public T[] GetObjects<T> (IEnumerable<ObjectID> objectIDs)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      // GetDataContainersWithLazyLoad throws on invalid objectID
      return _dataManager.GetDataContainersWithLazyLoad(objectIDs, throwOnNotFound: true).Select(dc => dc!)
          .Select(dc => dc.DomainObject)
          .Cast<T>()
          .ToArray();
    }

    public T?[] TryGetObjects<T> (IEnumerable<ObjectID> objectIDs)
        where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

      var objectIDsAsCollection = objectIDs.ConvertToCollection();

      var validObjectIDs = objectIDsAsCollection.Where(id => !_invalidDomainObjectManager.IsInvalid(id)).ConvertToCollection();

      // this performs a bulk load operation
      var dataContainersByID = validObjectIDs
          .Zip(_dataManager.GetDataContainersWithLazyLoad(validObjectIDs, false))
          .ToDictionary(t => t.Item1, t => t.Item2);

      var result = objectIDsAsCollection.Select(
          id =>
          {
            if (dataContainersByID.TryGetValue(id, out var loadResult))
              return loadResult == null ? null : (T?)loadResult.DomainObject;
            else
            {
              Assertion.IsTrue(
                  _invalidDomainObjectManager.IsInvalid(id),
                  "All valid IDs have been passed to GetDataContainersWithLazyLoad, so if its not in the loadResult, it must be invalid.");
              return (T)_invalidDomainObjectManager.GetInvalidObjectReference(id);
            }
          });
      return result.ToArray();

    }

    public void Delete (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      // DataManager checks that object is enlisted and not invalid
      var command = _dataManager.CreateDeleteCommand(domainObject);
      var fullCommand = command.ExpandToAllRelatedObjects();
      fullCommand.NotifyAndPerform();
    }

    private void CleanupCreatedObject (ObjectID objectID, DomainObject domainObject, Exception creationException)
    {
      try
      {
        Delete(domainObject);
      }
      catch (Exception deleteException)
      {
        var message = string.Format(
            "While cleaning up an object of type '{0}' that threw an exception of type "
            + "'{1}' from its constructor, another exception of type '{2}' was encountered. "
            + "Cleanup was therefore aborted, and a partially constructed object with ID '{3}' remains within the ClientTransaction '{4}'."
            + " Rollback the transaction to get rid of the partially constructed instance." + Environment.NewLine
            + "Message of original exception: {5}" + Environment.NewLine
            + "Message of exception occurring during cleanup: {6}",
            domainObject.GetPublicDomainObjectType(),
            creationException.GetType(),
            deleteException.GetType(),
            objectID,
            _clientTransaction,
            creationException.Message,
            deleteException.Message);
        throw new ObjectCleanupException(message, objectID, creationException, deleteException);
      }

      _enlistedDomainObjectManager.DisenlistDomainObject(domainObject);
    }
  }
}
