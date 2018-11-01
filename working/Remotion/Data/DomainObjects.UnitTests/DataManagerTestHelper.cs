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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class DataManagerTestHelper
  {
    public static DataContainerMap GetDataContainerMap (IDataManager dataManager)
    {
      return (DataContainerMap) dataManager.DataContainers;
    }

    public static IRelationEndPointManager GetRelationEndPointManager (IDataManager dataManager)
    {
      return (IRelationEndPointManager) PrivateInvoke.GetNonPublicField (dataManager, "_relationEndPointManager");
    }

    public static void RemoveEndPoint (IDataManager dataManager, RelationEndPointID endPointID)
    {
      RelationEndPointManagerTestHelper.RemoveEndPoint ((RelationEndPointManager) GetRelationEndPointManager (dataManager), endPointID);
    }

    public static void AddEndPoint (DataManager dataManager, IRelationEndPoint endPoint)
    {
      RelationEndPointManagerTestHelper.AddEndPoint ((RelationEndPointManager) GetRelationEndPointManager (dataManager), endPoint);
    }

    public static IInvalidDomainObjectManager GetInvalidDomainObjectManager (DataManager dataManager)
    {
      return (IInvalidDomainObjectManager) PrivateInvoke.GetNonPublicField (dataManager, "_invalidDomainObjectManager");
    }

    public static IObjectLoader GetObjectLoader (DataManager dataManager)
    {
      return (IObjectLoader) PrivateInvoke.GetNonPublicField (dataManager, "_objectLoader");
    }

    public static void AddDataContainer (DataManager dataManager, DataContainer dataContainer)
    {
      GetDataContainerMap (dataManager).Register (dataContainer);
    }
  }
}