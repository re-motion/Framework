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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Keeps track of <see cref="DomainObjectCollection"/> references to be used by <see cref="CollectionEndPoint"/> instances.
  /// </summary>
  [Serializable]
  public class CollectionEndPointCollectionProvider : ICollectionEndPointCollectionProvider
  {
    private readonly IAssociatedCollectionDataStrategyFactory _dataStrategyFactory;
    
    private readonly IDataStore<RelationEndPointID, DomainObjectCollection> _collections = new SimpleDataStore<RelationEndPointID, DomainObjectCollection>();

    public CollectionEndPointCollectionProvider (IAssociatedCollectionDataStrategyFactory dataStrategyFactory)
    {
      ArgumentUtility.CheckNotNull ("dataStrategyFactory", dataStrategyFactory);
      _dataStrategyFactory = dataStrategyFactory;
    }

    public IAssociatedCollectionDataStrategyFactory DataStrategyFactory
    {
      get { return _dataStrategyFactory; }
    }

    public DomainObjectCollection GetCollection (RelationEndPointID endPointID)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      var collection = _collections.GetOrCreateValue (endPointID, id =>
      {
        var dataStrategy = _dataStrategyFactory.CreateDataStrategyForEndPoint (id);
        return DomainObjectCollectionFactory.Instance.CreateCollection (id.Definition.PropertyInfo.PropertyType, dataStrategy);
      });
      Assertion.IsTrue (collection.AssociatedEndPointID == endPointID);
      return collection;
    }

    public void RegisterCollection (RelationEndPointID endPointID, DomainObjectCollection collection)
    {
      ArgumentUtility.CheckNotNull ("endPointID", endPointID);
      ArgumentUtility.CheckNotNull ("collection", collection);

      if (collection.AssociatedEndPointID != endPointID)
        throw new ArgumentException ("The collection must be associated with the given endPointID.", "collection");

      _collections[endPointID] = collection;
    }
  }
}