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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  /// <summary>
  /// Holds the <see cref="ClassContext"/> instances for a <see cref="MixinConfiguration"/>, providing easy means to search exactly for a given type
  /// (<see cref="GetExact"/>) and with inheritance rules (<see cref="GetWithInheritance"/>).
  /// This class is immutable, i.e., it is initialized on construction and cannot be changed later on.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class ClassContextCollection : ICollection, ICollection<ClassContext>
  {
    private readonly Dictionary<Type, ClassContext> _values = new Dictionary<Type, ClassContext> ();
    private readonly IMixinInheritancePolicy _inheritancePolicy = DefaultMixinInheritancePolicy.Instance;

    private readonly LockingCacheDecorator<Type, ClassContext> _inheritedContextCache = CacheFactory.CreateWithLocking<Type, ClassContext>();

    public ClassContextCollection (IEnumerable<ClassContext> classContexts)
    {
      _values = classContexts.ToDictionary (cc => cc.Type);
    }

    public ClassContextCollection (params ClassContext[] classContexts)
        : this ((IEnumerable<ClassContext>) classContexts)
    {
    }

    public int Count
    {
      get { return _values.Count; }
    }

    public IEnumerator<ClassContext> GetEnumerator ()
    {
      return _values.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public void CopyTo (ClassContext[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) this).CopyTo (array, arrayIndex);
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) _values.Values).CopyTo (array, index);
    }

    public ClassContext GetExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      ClassContext result;
      _values.TryGetValue (type, out result);
      Assertion.IsTrue (result == null || result.Type == type);
      return result;
    }

    public ClassContext GetWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var exactMatch = GetExact (type);
      if (exactMatch != null)
        return exactMatch;
      else
        return _inheritedContextCache.GetOrCreateValue (type, DeriveInheritedContext);
    }

    private ClassContext DeriveInheritedContext (Type type)
    {
      var contextsToInheritFrom = _inheritancePolicy.GetClassContextsToInheritFrom (type, GetWithInheritance); // Recursion!

      var inheritedContextCombiner = new ClassContextCombiner ();
      inheritedContextCombiner.AddRangeAllowingNulls (contextsToInheritFrom);

      var result = inheritedContextCombiner.GetCombinedContexts (type);
      Assertion.IsTrue (result == null || result.Type == type);
      return result;
    }

    public bool ContainsExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return GetExact (type) != null;
    }

    public bool ContainsWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return GetWithInheritance (type) != null;
    }

    public bool Contains (ClassContext item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return item.Equals (GetExact (item.Type));
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_values).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    bool ICollection<ClassContext>.IsReadOnly
    {
      get { return true; }
    }

    void ICollection<ClassContext>.Clear ()
    {
      throw new NotSupportedException ("This collection is read-only.");
    }

    void ICollection<ClassContext>.Add (ClassContext value)
    {
      throw new NotSupportedException ("This collection is read-only.");
    }

    bool ICollection<ClassContext>.Remove (ClassContext item)
    {
      throw new NotSupportedException ("This collection is read-only.");
    }
  }
}
