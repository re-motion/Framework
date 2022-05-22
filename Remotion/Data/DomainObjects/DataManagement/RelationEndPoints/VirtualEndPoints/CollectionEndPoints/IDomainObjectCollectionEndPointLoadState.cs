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
  /// Represents the lazy-loading state of a <see cref="DomainObjectCollectionEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  public interface IDomainObjectCollectionEndPointLoadState :
      IVirtualEndPointLoadState<IDomainObjectCollectionEndPoint, ReadOnlyDomainObjectCollectionDataDecorator, IDomainObjectCollectionEndPointDataManager>
  {
    bool? HasChangedFast ();

    void EnsureDataComplete (IDomainObjectCollectionEndPoint endPoint);
    void MarkDataComplete (IDomainObjectCollectionEndPoint collectionEndPoint, IEnumerable<IDomainObject> items, Action<IDomainObjectCollectionEndPointDataManager> stateSetter);
    void SortCurrentData (IDomainObjectCollectionEndPoint collectionEndPoint, Comparison<IDomainObject> comparison);

    IDataManagementCommand CreateSetCollectionCommand (
        IDomainObjectCollectionEndPoint collectionEndPoint,
        DomainObjectCollection newCollection,
        IDomainObjectCollectionEndPointCollectionManager collectionEndPointCollectionManager);
    IDataManagementCommand CreateRemoveCommand (IDomainObjectCollectionEndPoint collectionEndPoint, IDomainObject removedRelatedObject);
    IDataManagementCommand CreateDeleteCommand (IDomainObjectCollectionEndPoint collectionEndPoint);
    IDataManagementCommand CreateInsertCommand (IDomainObjectCollectionEndPoint collectionEndPoint, IDomainObject insertedRelatedObject, int index);
    IDataManagementCommand CreateAddCommand (IDomainObjectCollectionEndPoint collectionEndPoint, IDomainObject addedRelatedObject);
    IDataManagementCommand CreateReplaceCommand (IDomainObjectCollectionEndPoint collectionEndPoint, int index, IDomainObject replacementObject);
  }
}
