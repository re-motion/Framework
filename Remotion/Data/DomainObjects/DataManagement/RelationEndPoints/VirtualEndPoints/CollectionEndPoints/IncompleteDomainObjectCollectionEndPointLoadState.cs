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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the state of a <see cref="DomainObjectCollectionEndPoint"/> where not all of its data is available (ie., the end-point has not been (lazily) 
  /// loaded, or it has been unloaded).
  /// </summary>
  public class IncompleteDomainObjectCollectionEndPointLoadState
      : IncompleteVirtualEndPointLoadStateBase<IDomainObjectCollectionEndPoint, ReadOnlyDomainObjectCollectionDataDecorator, IDomainObjectCollectionEndPointDataManager,
          IDomainObjectCollectionEndPointLoadState>,
          IDomainObjectCollectionEndPointLoadState
  {
    private readonly IDomainObjectCollectionEndPointDataManagerFactory _dataManagerFactory;

    public IncompleteDomainObjectCollectionEndPointLoadState (
        IEndPointLoader endPointLoader,
        IDomainObjectCollectionEndPointDataManagerFactory dataManagerFactory)
        : base(endPointLoader)
    {
      ArgumentNullException.ThrowIfNull(dataManagerFactory);
      _dataManagerFactory = dataManagerFactory;
    }

    public IDomainObjectCollectionEndPointDataManagerFactory DataManagerFactory
    {
      get { return _dataManagerFactory; }
    }

    public bool? HasChangedFast ()
    {
      return false;
    }

    public void EnsureDataComplete (IDomainObjectCollectionEndPoint endPoint)
    {
      ArgumentNullException.ThrowIfNull(endPoint);

      EndPointLoader.LoadEndPointAndGetNewState(endPoint);
    }

    public new void MarkDataComplete (
        IDomainObjectCollectionEndPoint collectionEndPoint,
        IEnumerable<DomainObject> items,
        Action<IDomainObjectCollectionEndPointDataManager> stateSetter)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(items);
      ArgumentNullException.ThrowIfNull(stateSetter);

      base.MarkDataComplete(collectionEndPoint, items, stateSetter);

      var eventRaiser = collectionEndPoint.GetCollectionEventRaiser();
      eventRaiser.WithinReplaceData();
    }

    public void SortCurrentData (IDomainObjectCollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(comparison);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      completeState.SortCurrentData(collectionEndPoint, comparison);
    }

    public IDataManagementCommand CreateSetCollectionCommand (
        IDomainObjectCollectionEndPoint collectionEndPoint,
        DomainObjectCollection newCollection,
        IDomainObjectCollectionEndPointCollectionManager collectionEndPointCollectionManager)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(newCollection);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      return completeState.CreateSetCollectionCommand(collectionEndPoint, newCollection, collectionEndPointCollectionManager);
    }

    public IDataManagementCommand CreateRemoveCommand (IDomainObjectCollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(removedRelatedObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      return completeState.CreateRemoveCommand(collectionEndPoint, removedRelatedObject);
    }

    public IDataManagementCommand CreateDeleteCommand (IDomainObjectCollectionEndPoint collectionEndPoint)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      return completeState.CreateDeleteCommand(collectionEndPoint);
    }

    public IDataManagementCommand CreateInsertCommand (IDomainObjectCollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(insertedRelatedObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      return completeState.CreateInsertCommand(collectionEndPoint, insertedRelatedObject, index);
    }

    public IDataManagementCommand CreateAddCommand (IDomainObjectCollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(addedRelatedObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      return completeState.CreateAddCommand(collectionEndPoint, addedRelatedObject);
    }

    public IDataManagementCommand CreateReplaceCommand (IDomainObjectCollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject)
    {
      ArgumentNullException.ThrowIfNull(collectionEndPoint);
      ArgumentNullException.ThrowIfNull(replacementObject);

      var completeState = EndPointLoader.LoadEndPointAndGetNewState(collectionEndPoint);
      return completeState.CreateReplaceCommand(collectionEndPoint, index, replacementObject);
    }

    protected override IDomainObjectCollectionEndPointDataManager CreateEndPointDataManager (IDomainObjectCollectionEndPoint endPoint)
    {
      ArgumentNullException.ThrowIfNull(endPoint);
      return _dataManagerFactory.CreateEndPointDataManager(endPoint.ID);
    }
  }
}
