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
  /// Represents an <see cref="ICollectionEndPoint{TData}"/> holding a <see cref="DomainObjectCollection"/>.
  /// </summary>
  public interface IDomainObjectCollectionEndPoint : ICollectionEndPoint<ReadOnlyDomainObjectCollectionDataDecorator>
  {
    DomainObjectCollection Collection { get; }

    DomainObjectCollection OriginalCollection { get; }

    DomainObjectCollection GetCollectionWithOriginalData ();

    IDataManagementCommand CreateSetCollectionCommand (DomainObjectCollection newCollection);
    IDataManagementCommand CreateInsertCommand (IDomainObject insertedRelatedObject, int index);
    IDataManagementCommand CreateReplaceCommand (int index, IDomainObject replacementObject);

    void SortCurrentData (Comparison<IDomainObject> comparison);

    IDomainObjectCollectionEventRaiser GetCollectionEventRaiser ();

    bool? HasChangedFast { get; }
  }
}
