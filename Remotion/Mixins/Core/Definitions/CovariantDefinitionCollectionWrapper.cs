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
using System.Diagnostics;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("Count = {Count}")]
  public class CovariantDefinitionCollectionWrapper<TKey, TValue, TValueBase> : IDefinitionCollection<TKey, TValueBase>
      where TValue : class, TValueBase
      where TValueBase : IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<TKey, TValue> _items;

    public CovariantDefinitionCollectionWrapper(UniqueDefinitionCollection<TKey, TValue> items)
    {
      _items = items;
    }

    public TValueBase[] ToArray()
    {
      return _items.ToArray ();
    }

    public int Count
    {
      get { return _items.Count; }
    }

    public bool ContainsKey (TKey key)
    {
      return _items.ContainsKey (key);
    }

    public TValueBase this [int index]
    {
      get { return _items[index]; }
    }

    public TValueBase this [TKey key]
    {
      get { return _items[key]; }
    }

    public IEnumerator<TValueBase> GetEnumerator()
    {
      foreach (TValue item in _items)
        yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
