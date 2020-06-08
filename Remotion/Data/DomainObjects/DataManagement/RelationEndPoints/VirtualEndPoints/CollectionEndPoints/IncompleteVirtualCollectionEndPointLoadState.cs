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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="DomainObjectCollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteVirtualCollectionEndPointLoadState 
      : IncompleteVirtualEndPointLoadStateBase<IVirtualCollectionEndPoint, ReadOnlyVirtualCollectionData, IVirtualCollectionEndPointDataManager, IVirtualCollectionEndPointLoadState>,
        IVirtualCollectionEndPointLoadState
  {
    private readonly IVirtualCollectionEndPointDataManagerFactory _dataManagerFactory;

    public IncompleteVirtualCollectionEndPointLoadState (
        IEndPointLoader endPointLoader, 
        IVirtualCollectionEndPointDataManagerFactory dataManagerFactory)
      : base (endPointLoader)
    {
      ArgumentUtility.CheckNotNull ("dataManagerFactory", dataManagerFactory);
      _dataManagerFactory = dataManagerFactory;
    }

    public IVirtualCollectionEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public bool? HasChangedFast ()
    {
      return false;
    }

    public void EnsureDataComplete (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      EndPointLoader.LoadEndPointAndGetNewState (endPoint);
    }

    public new void MarkDataComplete (IVirtualCollectionEndPoint collectionEndPoint, IEnumerable<DomainObject> items, Action<IVirtualCollectionEndPointDataManager> stateSetter)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("items", items);
      ArgumentUtility.CheckNotNull ("stateSetter", stateSetter);

      base.MarkDataComplete (collectionEndPoint, items, stateSetter);

      var eventRaiser = collectionEndPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }

    public void SortCurrentData (IVirtualCollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      completeState.SortCurrentData (collectionEndPoint, comparison);
    }

    public IDataManagementCommand CreateRemoveCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateRemoveCommand (collectionEndPoint, removedRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (IVirtualCollectionEndPoint collectionEndPoint)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      
      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateDeleteCommand (collectionEndPoint);
    }

    public IDataManagementCommand CreateAddCommand (IVirtualCollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("collectionEndPoint", collectionEndPoint);
      ArgumentUtility.CheckNotNull ("addedRelatedObject", addedRelatedObject);
      
      var completeState = EndPointLoader.LoadEndPointAndGetNewState (collectionEndPoint);
      return completeState.CreateAddCommand (collectionEndPoint, addedRelatedObject);
    }

    protected override IVirtualCollectionEndPointDataManager CreateEndPointDataManager (IVirtualCollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);
      return _dataManagerFactory.CreateEndPointDataManager (endPoint.ID);
    }

    #region Serialization

    public IncompleteVirtualCollectionEndPointLoadState (FlattenedDeserializationInfo info)
        : base (info)
    {
      _dataManagerFactory = info.GetValueForHandle<IVirtualCollectionEndPointDataManagerFactory>();
    }

    protected override void SerializeSubclassData (FlattenedSerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      info.AddHandle (_dataManagerFactory);
    }

    #endregion
  }
}