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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  public class VirtualCollectionData : IVirtualCollectionData
  {
    public VirtualCollectionData (IList<DomainObject> domainObjects)
    {
    }

    public IEnumerator<DomainObject> GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public int Count => throw new NotImplementedException();

    public Type RequiredItemType => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public RelationEndPointID AssociatedEndPointID => throw new NotImplementedException();

    public bool IsDataComplete => throw new NotImplementedException();

    public void EnsureDataComplete ()
    {
      throw new NotImplementedException();
    }

    public bool ContainsObjectID (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetObject (int index)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetObject (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public int IndexOf (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public void Clear ()
    {
      throw new NotImplementedException();
    }

    public void Add (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public bool Remove (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public bool Remove (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public void Sort (Comparison<DomainObject> comparison)
    {
      throw new NotImplementedException();
    }
  }
}