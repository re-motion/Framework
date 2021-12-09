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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Represents the <seealso cref="IVirtualCollectionData"/> in situations when the collection has not been loaded.
  /// This type does not implement any kind of data management for the collection. It is simply intended as a null-object in certain infrastructure use cases.
  /// </summary>
  public class IncompleteEndPointModificationVirtualCollectionData : IVirtualCollectionData
  {
    private readonly RelationEndPointID _associatedEndPointID;

    public IncompleteEndPointModificationVirtualCollectionData (RelationEndPointID associatedEndPointID)
    {
      ArgumentUtility.CheckNotNull("associatedEndPointID", associatedEndPointID);

      _associatedEndPointID = associatedEndPointID;
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return Enumerable.Empty<DomainObject>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count => 0;

    public Type RequiredItemType => _associatedEndPointID.Definition.GetOppositeEndPointDefinition().TypeDefinition.Type;

    public bool IsReadOnly => false;

    public RelationEndPointID AssociatedEndPointID => _associatedEndPointID;

    public bool IsDataComplete => false;

    public void EnsureDataComplete ()
    {
    }

    public bool ContainsObjectID (ObjectID objectID) => false;

    public DomainObject GetObject (int index) => throw new ArgumentOutOfRangeException("index");

    public DomainObject? GetObject (ObjectID objectID) => null;

    public int IndexOf (ObjectID objectID) => -1;

    public void Clear ()
    {
    }

    public void Add (DomainObject domainObject)
    {
    }

    public bool Remove (DomainObject domainObject)
    {
      return true;
    }
  }
}
