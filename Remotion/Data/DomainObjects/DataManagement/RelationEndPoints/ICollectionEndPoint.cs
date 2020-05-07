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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Represents an <see cref="IRelationEndPoint"/> holding a collection of <see cref="DomainObject"/> instances, i.e. the "many" side of a relation.
  /// </summary>
  public interface ICollectionEndPoint : IVirtualEndPoint<ReadOnlyCollectionDataDecorator>
  {
    bool? HasChangedFast { get; } //Keep for new query collection endpoint

    DomainObjectCollection Collection { get; } // move to DomainObjectCollection-specific

    [Obsolete ("Use GetCollectionWithOriginalData() instead.", true)]
    DomainObjectCollection OriginalCollection { get; }

    IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ();

    DomainObjectCollection GetCollectionWithOriginalData (); // move to DomainObjectCollection-specific

    void MarkDataComplete (DomainObject[] items); //Keep for new query collection endpoint

    IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection); // move to DomainObjectCollection-specifc
    IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index);// move to DomainObjectCollection-specifc
    IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject); //Keep for new query collection endpoint
    IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject);// move to DomainObjectCollection-specifc

    void SortCurrentData (Comparison<DomainObject> comparison);//Keep for new query collection endpoint
  }
}