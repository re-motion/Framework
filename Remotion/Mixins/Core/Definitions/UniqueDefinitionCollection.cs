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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay("Count = {Count}")]
  public class UniqueDefinitionCollection<TKey, TValue> : DefinitionCollectionBase<TKey, TValue>, IDefinitionCollection<TKey, TValue>
      where TKey : notnull
      where TValue : IVisitableDefinition
  {
    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();

    public UniqueDefinitionCollection (KeyMaker keyMaker, Predicate<TValue> guardian) : base(keyMaker, guardian)
    {
    }

    public UniqueDefinitionCollection (KeyMaker keyMaker) : base(keyMaker, null)
    {
    }

    public override bool ContainsKey (TKey key)
    {
      return _items.ContainsKey(key);
    }

    protected override void CustomizedAdd (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull("key", key);
      ArgumentUtility.CheckNotNull("value", value);

      if (ContainsKey(key))
      {
        string message = string.Format("Duplicate key {0} for item {1}.", key, value);
        throw new InvalidOperationException(message);
      }
      _items.Add(key, value);
    }

    protected override void CustomizedClear ()
    {
      _items.Clear();
    }

    public TValue this[TKey key]
    {
      get { return ContainsKey(ArgumentUtility.CheckNotNull("key", key)) ? _items[key] : default(TValue)!; }
    }
  }
}
