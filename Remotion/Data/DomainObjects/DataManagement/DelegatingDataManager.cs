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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Implements <see cref="IDataManager"/> by passing on all calls to an <see cref="InnerDataManager"/>. This class is used to resolve a
  /// dependency cycle between <see cref="DataManager"/> and <see cref="ObjectLoader"/>.
  /// </summary>
  [Serializable]
  public class DelegatingDataManager : IDataManager
  {
    private IDataManager? _innerDataManager;

    public IDataManager? InnerDataManager
    {
      get { return _innerDataManager; }
      set { _innerDataManager = value; }
    }

    public IVirtualEndPoint GetOrCreateVirtualEndPoint (RelationEndPointID endPointID)
    {
      return SafeInnerDataManager.GetOrCreateVirtualEndPoint(endPointID);
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      return SafeInnerDataManager.GetRelationEndPointWithLazyLoad(endPointID);
    }

    public IRelationEndPoint? GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      return SafeInnerDataManager.GetRelationEndPointWithoutLoading(endPointID);
    }

    public DataContainer? GetDataContainerWithoutLoading (ObjectID objectID)
    {
      return SafeInnerDataManager.GetDataContainerWithoutLoading(objectID);
    }

    public void RegisterDataContainer (DataContainer dataContainer)
    {
      SafeInnerDataManager.RegisterDataContainer(dataContainer);
    }

    public void Discard (DataContainer dataContainer)
    {
      SafeInnerDataManager.Discard(dataContainer);
    }

    public IDataContainerMapReadOnlyView DataContainers
    {
      get { return SafeInnerDataManager.DataContainers; }
    }

    public IRelationEndPointMapReadOnlyView RelationEndPoints
    {
      get { return SafeInnerDataManager.RelationEndPoints; }
    }

    public DomainObjectState GetState (ObjectID objectID)
    {
      return SafeInnerDataManager.GetState(objectID);
    }

    public DataContainer? GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound)
    {
      return SafeInnerDataManager.GetDataContainerWithLazyLoad(objectID, throwOnNotFound);
    }

    public IEnumerable<DataContainer?> GetDataContainersWithLazyLoad (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
    {
      return SafeInnerDataManager.GetDataContainersWithLazyLoad(objectIDs, throwOnNotFound);
    }

    public IEnumerable<PersistableData> GetLoadedDataByObjectState (Predicate<DomainObjectState> predicate)
    {
      return SafeInnerDataManager.GetLoadedDataByObjectState(predicate);
    }

    public void MarkInvalid (IDomainObject domainObject)
    {
      SafeInnerDataManager.MarkInvalid(domainObject);
    }

    public void MarkNotInvalid (ObjectID objectID)
    {
      SafeInnerDataManager.MarkNotInvalid(objectID);
    }

    public void Commit ()
    {
      SafeInnerDataManager.Commit();
    }

    public void Rollback ()
    {
      SafeInnerDataManager.Rollback();
    }

    public IDataManagementCommand CreateDeleteCommand (IDomainObject deletedObject)
    {
      return SafeInnerDataManager.CreateDeleteCommand(deletedObject);
    }

    public IDataManagementCommand CreateUnloadCommand (params ObjectID[] objectIDs)
    {
      return SafeInnerDataManager.CreateUnloadCommand(objectIDs);
    }

    public IDataManagementCommand CreateUnloadVirtualEndPointsCommand (params RelationEndPointID[] endPointIDs)
    {
      return SafeInnerDataManager.CreateUnloadVirtualEndPointsCommand(endPointIDs);
    }

    public IDataManagementCommand CreateUnloadAllCommand ()
    {
      return SafeInnerDataManager.CreateUnloadAllCommand();
    }

    public void LoadLazyCollectionEndPoint (RelationEndPointID endPointID)
    {
      SafeInnerDataManager.LoadLazyCollectionEndPoint(endPointID);
    }

    public void LoadLazyVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      SafeInnerDataManager.LoadLazyVirtualObjectEndPoint(endPointID);
    }

    public DataContainer LoadLazyDataContainer (ObjectID objectID)
    {
      return SafeInnerDataManager.LoadLazyDataContainer(objectID);
    }

    private IDataManager SafeInnerDataManager
    {
      get
      {
        if (_innerDataManager == null)
          throw new InvalidOperationException("InnerDataManager property must be set before it can be used.");
        return _innerDataManager;
      }
    }
  }
}
