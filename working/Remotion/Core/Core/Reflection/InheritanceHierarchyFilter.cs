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
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// The <see cref="InheritanceHierarchyFilter"/> can be used to get all leaf classes within a deifned set of types passed into the 
  /// constructor.
  /// </summary>
  public class InheritanceHierarchyFilter
  {
    private readonly Type[] _types;

    public InheritanceHierarchyFilter (Type[] types)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("types", types);
      _types = types;
    }

    public Type[] GetLeafTypes ()
    {
      HashSet<Type> baseTypes = new HashSet<Type> ();
      foreach (Type type in _types)
      {
        baseTypes.Add (type.BaseType);
        if (type.BaseType.IsGenericType)
          baseTypes.Add (type.BaseType.GetGenericTypeDefinition());
      }

      return Array.FindAll (_types, delegate (Type type) { return !baseTypes.Contains (type); });
    }
  }
}
