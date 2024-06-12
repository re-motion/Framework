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

namespace Remotion.Collections
{
  [Obsolete("Dummy declaration for DependDB. Moved to Remotion.Extensions.dll", true)]
  public class ReadOnlyCollectionDecorator<T> : ICollection<T>, IReadOnlyCollection<T>
  {
    public ReadOnlyCollectionDecorator (ICollection<T> collection)
    {
      throw new NotImplementedException();
    }

    public bool Contains (T item)
    {
      throw new NotImplementedException();
    }

    public void CopyTo (T[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public int Count
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    void ICollection<T>.Add (T item)
    {
      throw new NotImplementedException();
    }

    bool ICollection<T>.Remove (T item)
    {
      throw new NotImplementedException();
    }

    void ICollection<T>.Clear ()
    {
      throw new NotImplementedException();
    }
  }
}
