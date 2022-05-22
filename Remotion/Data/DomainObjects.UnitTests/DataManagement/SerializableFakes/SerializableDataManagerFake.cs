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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableDataManagerFake : IDataManager
  {
    public IVirtualEndPoint GetOrCreateVirtualEndPoint (RelationEndPointID endPointID)
    {
      throw new NotImplementedException();
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      throw new NotImplementedException();
    }

    public IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      throw new NotImplementedException();
    }

    public DataContainer GetDataContainerWithoutLoading (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public void RegisterDataContainer (DataContainer dataContainer)
    {
      throw new NotImplementedException();
    }

    public void Discard (DataContainer dataContainer)
    {
      throw new NotImplementedException();
    }

    public void LoadLazyCollectionEndPoint (RelationEndPointID endPointID)
    {
      throw new NotImplementedException();
    }

    public void LoadLazyVirtualObjectEndPoint (RelationEndPointID endPointID)
    {
      throw new NotImplementedException();
    }

    public DataContainer LoadLazyDataContainer (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public IDataContainerMapReadOnlyView DataContainers
    {
      get { throw new NotImplementedException(); }
    }

    public IRelationEndPointMapReadOnlyView RelationEndPoints
    {
      get { throw new NotImplementedException(); }
    }

    public DomainObjectState GetState (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public DataContainer GetDataContainerWithLazyLoad (ObjectID objectID, bool throwOnNotFound)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<DataContainer> GetDataContainersWithLazyLoad (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<PersistableData> GetLoadedDataByObjectState (Predicate<DomainObjectState> predicate)
    {
      throw new NotImplementedException();
    }

    public void MarkInvalid (IDomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public void MarkNotInvalid (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public void Commit ()
    {
      throw new NotImplementedException();
    }

    public void Rollback ()
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateDeleteCommand (IDomainObject deletedObject)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateUnloadCommand (params ObjectID[] objectIDs)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateUnloadVirtualEndPointsCommand (params RelationEndPointID[] endPointIDs)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateUnloadAllCommand ()
    {
      throw new NotImplementedException();
    }
  }
}
