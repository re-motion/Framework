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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Caches the types returned by <see cref="Assembly.GetTypes"/>.
  /// </summary>
  public static class AssemblyTypeCache
  {
    private static readonly ConcurrentDictionary<Assembly, (IReadOnlyCollection<Type> Types, bool IsGacAssembly)> s_typeCache = new();

    public static bool IsGacAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return s_typeCache.GetOrAdd (assembly, GetTypeCacheValue).IsGacAssembly;
    }

    public static IReadOnlyCollection<Type> GetTypes (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return s_typeCache.GetOrAdd (assembly, GetTypeCacheValue).Types;
    }

    private static (IReadOnlyCollection<Type> Types, bool IsGacAssembly) GetTypeCacheValue (Assembly assembly)
    {
#if FEATURE_GAC
      var isGacAssembly = assembly.GlobalAssemblyCache;
#else
      var isGacAssembly = false;
#endif

      var types = Array.AsReadOnly (assembly.GetTypes());

      return (types, isGacAssembly);
    }
  }
}