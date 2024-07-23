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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Takes <see cref="ILoadedObjectData"/> instances, registers all freshly loaded ones - triggering the necessary load events - and then returns
  /// the corresponding <see cref="DomainObject"/> instances.
  /// </summary>
  public class LoadedObjectDataRegistrationAgent : ILoadedObjectDataRegistrationAgent
  {
    private class RegisteredDataContainerGatheringVisitor : ILoadedObjectVisitor
    {
      private readonly LoadedObjectDataPendingRegistrationCollector _dataPendingRegistrationCollector;
      private readonly ClientTransaction _clientTransaction;
      private readonly List<ObjectID> _notFoundObjectIDs = new List<ObjectID>();

      private readonly List<ILoadedObjectData> _loadedObjectData = new List<ILoadedObjectData>();

      public RegisteredDataContainerGatheringVisitor (
          LoadedObjectDataPendingRegistrationCollector dataPendingRegistrationCollector,
          ClientTransaction clientTransaction)
      {
        ArgumentUtility.CheckNotNull("dataPendingRegistrationCollector", dataPendingRegistrationCollector);
        ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);

        _dataPendingRegistrationCollector = dataPendingRegistrationCollector;
        _clientTransaction = clientTransaction;
      }

      public ReadOnlyCollection<ObjectID> NotFoundObjectIDs
      {
        get { return _notFoundObjectIDs.AsReadOnly(); }
      }

      public ReadOnlyCollection<ILoadedObjectData> LoadedObjectData
      {
        get { return _loadedObjectData.AsReadOnly(); }
      }

      public void VisitFreshlyLoadedObject (FreshlyLoadedObjectData freshlyLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull("freshlyLoadedObjectData", freshlyLoadedObjectData);

        var consolidatedData = _dataPendingRegistrationCollector.Add(freshlyLoadedObjectData);
        _loadedObjectData.Add(consolidatedData);

        if (consolidatedData == freshlyLoadedObjectData)
        {
          var domainObject = _clientTransaction.GetObjectReference(freshlyLoadedObjectData.FreshlyLoadedDataContainer.ID);
          freshlyLoadedObjectData.FreshlyLoadedDataContainer.SetDomainObject(domainObject);
        }
      }

      public void VisitAlreadyExistingLoadedObject (AlreadyExistingLoadedObjectData alreadyExistingLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull("alreadyExistingLoadedObjectData", alreadyExistingLoadedObjectData);

        _loadedObjectData.Add(alreadyExistingLoadedObjectData);
      }

      public void VisitNullLoadedObject (NullLoadedObjectData nullLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull("nullLoadedObjectData", nullLoadedObjectData);

        _loadedObjectData.Add(nullLoadedObjectData);
      }

      public void VisitInvalidLoadedObject (InvalidLoadedObjectData invalidLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull("invalidLoadedObjectData", invalidLoadedObjectData);

        _loadedObjectData.Add(invalidLoadedObjectData);
      }

      public void VisitNotFoundLoadedObject (NotFoundLoadedObjectData notFoundLoadedObjectData)
      {
        ArgumentUtility.CheckNotNull("notFoundLoadedObjectData", notFoundLoadedObjectData);
        _notFoundObjectIDs.Add(notFoundLoadedObjectData.ObjectID);

        _loadedObjectData.Add(notFoundLoadedObjectData);
      }
    }

    private readonly ClientTransaction _clientTransaction;
    private readonly IDataManager _dataManager;
    private readonly ILoadedObjectDataRegistrationListener _registrationListener;

    public LoadedObjectDataRegistrationAgent (
        ClientTransaction clientTransaction,
        IDataManager dataManager,
        ILoadedObjectDataRegistrationListener registrationListener)
    {
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);
      ArgumentUtility.CheckNotNull("registrationListener", registrationListener);

      _dataManager = dataManager;
      _clientTransaction = clientTransaction;
      _registrationListener = registrationListener;
    }

    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    public IDataManager DataManager
    {
      get { return _dataManager; }
    }

    public ILoadedObjectDataRegistrationListener RegistrationListener
    {
      get { return _registrationListener; }
    }

    public IEnumerable<ILoadedObjectData> RegisterIfRequired (IEnumerable<ILoadedObjectData> loadedObjects, bool throwOnNotFound)
    {
      var collector = new LoadedObjectDataPendingRegistrationCollector();
      var result = BeginRegisterIfRequired(loadedObjects, throwOnNotFound, collector);
      EndRegisterIfRequired(collector);
      return result;
    }

    public IEnumerable<ILoadedObjectData> BeginRegisterIfRequired (
        IEnumerable<ILoadedObjectData> loadedObjects, bool throwOnNotFound, LoadedObjectDataPendingRegistrationCollector pendingLoadedObjectDataCollector)
    {
      ArgumentUtility.CheckNotNull("loadedObjects", loadedObjects);

      var visitor = new RegisteredDataContainerGatheringVisitor(pendingLoadedObjectDataCollector, _clientTransaction);
      foreach (var loadedObject in loadedObjects)
        loadedObject.Accept(visitor);

      if (visitor.NotFoundObjectIDs.Any())
      {
        _registrationListener.OnObjectsNotFound(visitor.NotFoundObjectIDs);

        // Note: If this exception is thrown, we have already set the DomainObjects of the freshly loaded DataContainer, and we've also added them to 
        // the collector. This shouldn't make any difference, and it's easier to implement.
        if (throwOnNotFound)
          throw new ObjectsNotFoundException(visitor.NotFoundObjectIDs);
      }

      return visitor.LoadedObjectData;
    }

    public void EndRegisterIfRequired (LoadedObjectDataPendingRegistrationCollector pendingLoadedObjectDataCollector)
    {
      ArgumentUtility.CheckNotNull("pendingLoadedObjectDataCollector", pendingLoadedObjectDataCollector);

      if (pendingLoadedObjectDataCollector.DataPendingRegistration.Count == 0)
        return;

      // Note: After this event, OnAfterObjectRegistration _must_ be raised for the same ObjectIDs! Otherwise, we'll leak "objects currently loading".
      var objectIDs = pendingLoadedObjectDataCollector.DataPendingRegistration.Select(data => data.ObjectID).ToList().AsReadOnly();
      _registrationListener.OnBeforeObjectRegistration(objectIDs);

      var loadedDomainObjects = new List<DomainObject>(pendingLoadedObjectDataCollector.DataPendingRegistration.Count);
      try
      {
        foreach (var data in pendingLoadedObjectDataCollector.DataPendingRegistration)
        {
          var dataContainer = data.FreshlyLoadedDataContainer;
          Assertion.IsTrue(dataContainer.HasDomainObject);

          _dataManager.RegisterDataContainer(dataContainer);
          loadedDomainObjects.Add(dataContainer.DomainObject);
        }
      }
      finally
      {
        _registrationListener.OnAfterObjectRegistration(objectIDs, loadedDomainObjects.AsReadOnly());
      }
    }
  }
}
