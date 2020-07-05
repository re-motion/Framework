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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.DomainObjects
{
  public interface
      IObjectList<out TDomainObject>
      : IReadOnlyList<TDomainObject>, IList // TODO: RM-7294 add support for IReadOnlyList<T> to BocList. Fallback: implement IList {IsReadOnly=true}
      where TDomainObject : IDomainObject
  {
    new int Count { get; } // TODO: RM07294: Tie-breaker for IReadOnlyList<T> and IList

    new TDomainObject this [int index] { get; } // TODO: RM07294: Tie-breaker for IReadOnlyList<T> and IList

    /// <summary>
    /// Gets the <see cref="IVirtualCollectionEndPoint"/> for the <see cref="IObjectList{TDomainObject}"/>.
    /// </summary>
    /// <value>The associated end point.</value>
    RelationEndPointID AssociatedEndPointID { get; }

    bool IsDataComplete { get; }

    /// <summary>
    /// Ensures that the end point's data has been loaded, loading the data if necessary.
    /// </summary>
    void EnsureDataComplete ();

    bool Contains (ObjectID objectID);

    TDomainObject GetObject (ObjectID objectID);

    [Obsolete ("IObjectList is readonly.", true)]
    new int Add (object value);

    [Obsolete ("IObjectList is readonly.", true)]
    new void Clear ();

    [Obsolete ("IObjectList is readonly.", true)]
    new void Insert (int index, object value);

    [Obsolete ("IObjectList is readonly.", true)]
    new void Remove (object value);

    [Obsolete ("IObjectList is readonly.", true)]
    new void RemoveAt (int index);
  }
}