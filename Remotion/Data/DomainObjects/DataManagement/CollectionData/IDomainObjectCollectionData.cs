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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Provides an interface for an encapsulation of the data stored inside a <see cref="DomainObjectCollection"/>. A number of decorators
  /// implements this interface in order to wrap the data store with additional functionality.
  /// </summary>
  public interface IDomainObjectCollectionData : IReadOnlyCollection<IDomainObject>
  {
    Type? RequiredItemType { get; }
    bool IsReadOnly { get; }

    RelationEndPointID? AssociatedEndPointID { get; }
    bool IsDataComplete { get; }

    void EnsureDataComplete ();

    bool ContainsObjectID (ObjectID objectID);

    IDomainObject GetObject (int index);
    IDomainObject? GetObject (ObjectID objectID);

    int IndexOf (ObjectID objectID);

    void Clear ();
    void Insert (int index, IDomainObject domainObject);
    bool Remove (IDomainObject domainObject); // this overload should be called from DomainObjectCollection.Remove (DomainObject)
    bool Remove (ObjectID objectID); // this overload should be called from DomainObjectCollection.Remove (ObjectID)
    void Replace (int index, IDomainObject value);

    void Sort (Comparison<IDomainObject> comparison);
  }
}
