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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Represents the lazy-loading state of a <see cref="CollectionEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  public interface ICollectionEndPointLoadState : 
      IVirtualEndPointLoadState<ICollectionEndPoint, ReadOnlyCollectionDataDecorator, ICollectionEndPointDataManager>
  {
    bool? HasChangedFast ();

    void EnsureDataComplete (ICollectionEndPoint endPoint);
    void MarkDataComplete (ICollectionEndPoint collectionEndPoint, IEnumerable<DomainObject> items, Action<ICollectionEndPointDataManager> stateSetter);
    void SortCurrentData (ICollectionEndPoint collectionEndPoint, Comparison<DomainObject> comparison);

    IDataManagementCommand CreateSetCollectionCommand (
        ICollectionEndPoint collectionEndPoint,
        DomainObjectCollection newCollection,
        ICollectionEndPointCollectionManager collectionEndPointCollectionManager);
    IDataManagementCommand CreateRemoveCommand (ICollectionEndPoint collectionEndPoint, DomainObject removedRelatedObject);
    IDataManagementCommand CreateDeleteCommand (ICollectionEndPoint collectionEndPoint);
    IDataManagementCommand CreateInsertCommand (ICollectionEndPoint collectionEndPoint, DomainObject insertedRelatedObject, int index);
    IDataManagementCommand CreateAddCommand (ICollectionEndPoint collectionEndPoint, DomainObject addedRelatedObject);
    IDataManagementCommand CreateReplaceCommand (ICollectionEndPoint collectionEndPoint, int index, DomainObject replacementObject);
  }
}