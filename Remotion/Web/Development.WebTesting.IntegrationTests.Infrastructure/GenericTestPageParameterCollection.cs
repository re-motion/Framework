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
using JetBrains.Annotations;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  public class GenericTestPageParameterCollection : ICollection<GenericTestPageParameter>
  {
    /// <summary>
    /// Merges the two specified collections <paramref name="a"/> and <paramref name="b"/> while prioritizing items in collection <paramref name="b"/>.
    /// </summary>
    public static GenericTestPageParameterCollection Merge (
        [NotNull] GenericTestPageParameterCollection a,
        [NotNull] GenericTestPageParameterCollection b)
    {
      ArgumentUtility.CheckNotNull ("a", a);
      ArgumentUtility.CheckNotNull ("b", b);

      var collection = new GenericTestPageParameterCollection();
      foreach (var entry in b)
        collection.Add (entry);

      foreach (var entry in a)
      {
        if (!collection.Contains (entry))
        {
          collection.Add (entry);
        }
      }

      return collection;
    }

    private readonly HashSet<string> _parameterNames = new HashSet<string>();
    private readonly List<GenericTestPageParameter> _parameters = new List<GenericTestPageParameter>();

    public GenericTestPageParameterCollection ()
    {
    }

    private GenericTestPageParameterCollection (GenericTestPageParameter[] data)
    {
      _parameters.AddRange (data);
      foreach (var entry in data)
        _parameterNames.Add (entry.Name);
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _parameters.Count; }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <inheritdoc />
    public IEnumerator<GenericTestPageParameter> GetEnumerator ()
    {
      return _parameters.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add ([NotNull] GenericTestPageParameter item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      var name = item.Name;
      if (_parameterNames.Contains (name))
        throw new InvalidOperationException ("Parameter with that name is already registered.");

      _parameters.Add (item);
      _parameterNames.Add (name);
    }

    public void Add ([NotNull] string name, [NotNull] params string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNullOrItemsNull ("arguments", arguments);

      Add (new GenericTestPageParameter (name, arguments));
    }

    /// <inheritdoc />
    public void Clear ()
    {
      _parameters.Clear();
      _parameterNames.Clear();
    }

    /// <inheritdoc />
    public GenericTestPageParameterCollection Clone ()
    {
      return new GenericTestPageParameterCollection (_parameters.ToArray());
    }

    /// <inheritdoc />
    public bool Contains ([NotNull] GenericTestPageParameter item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return _parameterNames.Contains (item.Name);
    }

    /// <inheritdoc />
    public void CopyTo (GenericTestPageParameter[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("array", array);

      _parameters.CopyTo (array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove ([NotNull] GenericTestPageParameter item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      var success = _parameters.Remove (item);
      if (success)
        _parameterNames.Remove (item.Name);
      return success;
    }
  }
}