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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  public partial class DomainObjectCollection
  {
    bool IList.Contains (object value)
    {
      if (value is DomainObject)
        return ContainsObject ((DomainObject) value);

      if (value is ObjectID)
        return Contains ((ObjectID) value);

      return false;
    }

    int IList.IndexOf (object value)
    {
      if (value is DomainObject)
        return IndexOf ((DomainObject) value);

      if (value is ObjectID)
        return IndexOf ((ObjectID) value);

      return -1;
    }

    object IList.this[int index]
    {
      get { return this[index]; }
      set { this[index] = ArgumentUtility.CheckType<DomainObject> ("value", value); }
    }

    int IList.Add (object value)
    {
      return Add (ArgumentUtility.CheckNotNullAndType<DomainObject> ("value", value));
    }

    void IList.Remove (object value)
    {
      if (value is DomainObject)
        Remove ((DomainObject) value);

      if (value is ObjectID)
        Remove ((ObjectID) value);
    }

    void IList.Insert (int index, object value)
    {
      Insert (index, ArgumentUtility.CheckNotNullAndType<DomainObject> ("value", value));
    }

    object ICollection.SyncRoot
    {
      get { return this; }
    }

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe). Always
    /// returns <see langword="false" />.
    /// </summary>
    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size. Always returns <see langword="false" />.
    /// </summary>
    bool IList.IsFixedSize
    {
      get { return false; }
    }


  }
}