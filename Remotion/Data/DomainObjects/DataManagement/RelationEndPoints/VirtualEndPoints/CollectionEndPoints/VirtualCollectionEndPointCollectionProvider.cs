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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Keeps track of <see cref="IObjectList{IDomainObject}"/> references to be used by <see cref="VirtualCollectionEndPoint"/> instances.
  /// </summary>
  [Serializable]
  public class VirtualCollectionEndPointCollectionProvider : IVirtualCollectionEndPointCollectionProvider
  {
    private readonly Dictionary<RelationEndPointID, IObjectList<IDomainObject>> _collections = new Dictionary<RelationEndPointID, IObjectList<IDomainObject>>();
    private readonly Func<RelationEndPointID, IObjectList<IDomainObject>> _getCollectionWithoutCacheFunc;
    private readonly IVirtualEndPointProvider _virtualEndPointProvider;

    public VirtualCollectionEndPointCollectionProvider (IVirtualEndPointProvider virtualEndPointProvider)
    {
      ArgumentUtility.CheckNotNull("virtualEndPointProvider", virtualEndPointProvider);

      _virtualEndPointProvider = virtualEndPointProvider;

      // Optimized for memory allocations
      _getCollectionWithoutCacheFunc = GetCollectionWithoutCache;
    }

    public IVirtualEndPointProvider VirtualEndPointProvider
    {
      get { return _virtualEndPointProvider; }
    }

    public IObjectList<IDomainObject> GetCollection (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      var collection = _collections.GetOrCreateValue(endPointID, _getCollectionWithoutCacheFunc);
      Assertion.IsTrue(collection.AssociatedEndPointID == endPointID);
      return collection;
    }

    private IObjectList<IDomainObject> GetCollectionWithoutCache (RelationEndPointID endPointID)
    {
      var requiredItemType = endPointID.Definition.GetOppositeEndPointDefinition().TypeDefinition.Type;
      var dataStrategy = new EndPointDelegatingVirtualCollectionData(endPointID, requiredItemType, _virtualEndPointProvider);
      return ObjectListFactory.Create(dataStrategy);
    }
  }
}
