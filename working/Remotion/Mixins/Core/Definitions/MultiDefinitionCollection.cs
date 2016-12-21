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
using System.Diagnostics;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("Count = {Count}")]
  public class MultiDefinitionCollection<TKey, TValue> : DefinitionCollectionBase<TKey, TValue>
      where TValue : IVisitableDefinition
  {
    private MultiDictionary<TKey, TValue> _items = new MultiDictionary<TKey, TValue>();

    public MultiDefinitionCollection (KeyMaker keyMaker)
        : base (keyMaker, null)
    {
    }


    public override bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items.ContainsKey (key);
    }

    protected override void CustomizedAdd (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _items.Add (key, value);
    }

    protected override void CustomizedClear ()
    {
      _items.Clear();
    }

    public IEnumerable<TValue> this[TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("key", key);
        return _items[key];
      }
    }

    public int GetItemCount (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items[key].Count;
    }

    public TValue GetFirstItem (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      if (GetItemCount (key) == 0)
        throw new ArgumentException ("There is no item with the given key.", "key");
      else
        return _items[key][0];
    }

    public IEnumerable<TKey> Keys
    {
      get { return _items.Keys; }
    }
  }
}
