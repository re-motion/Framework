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
using Remotion.Data.DomainObjects;

namespace Remotion.Development.Data.UnitTesting.DomainObjects
{
  /// <summary>
  /// Implements <see cref="IEqualityComparer"/> so members of type <see cref="IDomainObjectHandle{T}"/> and <see cref="DomainObject"/>
  /// can be compared in unit tests.
  /// </summary>
  public class DomainObjectHandleComparer : IEqualityComparer
  {
    private static readonly DomainObjectHandleComparer s_instance = new DomainObjectHandleComparer();

    public static DomainObjectHandleComparer Instance
    {
      get { return s_instance; }
    }

    private DomainObjectHandleComparer ()
    {
    }

    public new bool Equals (object x, object y)
    {
      if (x == null && y == null)
        return true;

      if (x == null || y == null)
        return false;

      if (x is DomainObject && y is IDomainObjectHandle<DomainObject>)
        return Equals (((DomainObject) x).GetHandle(), (IDomainObjectHandle<DomainObject>) y);

      if (x is IDomainObjectHandle<DomainObject> && y is DomainObject)
        return Equals ((IDomainObjectHandle<DomainObject>) x, ((DomainObject) y).GetHandle());

      if (x is IDomainObjectHandle<DomainObject> && y is IDomainObjectHandle<DomainObject>)
        return Equals ((IDomainObjectHandle<DomainObject>) x, (IDomainObjectHandle<DomainObject>) y);
      
      if (x is DomainObject && y is DomainObject)
        return Equals (((DomainObject) x).GetHandle(), ((DomainObject) y).GetHandle());

      return false;
    }

    private bool Equals (IDomainObjectHandle<DomainObject> x, IDomainObjectHandle<DomainObject> y)
    {
      return x.ObjectID.Equals (y.ObjectID);
    }

    public int GetHashCode (object obj)
    {
      return obj.GetHashCode();
    }
  }
}