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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="VirtualCollectionEndPoint"/>.
  /// </summary>
  public interface IVirtualCollectionEndPointDataManager : IFlattenedSerializable
  {
    //bool? HasDataChangedFast ();

    IVirtualCollectionData CollectionData { get; }
    ReadOnlyVirtualCollectionDataDecorator GetOriginalCollectionData ();

    IRealObjectEndPoint[] OriginalOppositeEndPoints { get; }
    DomainObject[] OriginalItemsWithoutEndPoints { get; }
    IRealObjectEndPoint[] CurrentOppositeEndPoints { get; }

    //bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject);

    //void SortCurrentData (Comparison<DomainObject> comparison);
    //void SortCurrentAndOriginalData (Comparison<DomainObject> comparison);
    void SetDataFromSubTransaction (IVirtualCollectionEndPointDataManager sourceDataManager, IRelationEndPointProvider endPointProvider);

    RelationEndPointID EndPointID { get; }

    bool ContainsOriginalObjectID (ObjectID objectID);

    void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject);
    void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject);

    void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    //bool HasDataChanged();
    void Commit();
    void Rollback();
  }
}