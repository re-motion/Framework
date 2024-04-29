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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery
{
  /// <summary>
  /// Holds a cache of type hierarchies.
  /// </summary>
  public sealed class BaseTypeCache
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<BaseTypeCache>();

    public static BaseTypeCache Create (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull("types", types);

      s_logger.LogDebug("Beginning to build BaseTypeCache...");
      using (StopwatchScope.CreateScope(s_logger, LogLevel.Debug, string.Format("Built BaseTypeCache. Time taken: {{elapsed}}")))
      {
        // Note: there is no measurable impact when switching this code to parallel execution.
        var classes = new List<KeyValuePair<Type, Type>>();
        var interfaces = new List<KeyValuePair<Type, Type>>();

        var typesToProcess = new Queue<Type>();
        var enqueuedTypes = new HashSet<Type>();
        foreach (var type in types)
        {
          CheckTypeIsNormalized(type);
          if (enqueuedTypes.Add(type))
            typesToProcess.Enqueue(type);
        }

        var processedInterfaces = new HashSet<Type>();
        while (typesToProcess.Count > 0)
        {
          var type = typesToProcess.Dequeue();

          if (type.BaseType != null)
          {
            var normalizedBaseType = GetNormalizedType(type.BaseType);
            if (enqueuedTypes.Add(normalizedBaseType))
              typesToProcess.Enqueue(normalizedBaseType);

            var nextBaseType = normalizedBaseType;
            do
            {
              classes.Add(new KeyValuePair<Type, Type>(nextBaseType, type));
              nextBaseType = GetNormalizedType(nextBaseType.BaseType);
            } while (nextBaseType != null);
          }

          processedInterfaces.Clear();
          foreach (var interfaceType in type.GetInterfaces())
          {
            var normalizedInterfaceType = GetNormalizedType(interfaceType);
            if (enqueuedTypes.Add(normalizedInterfaceType))
              typesToProcess.Enqueue(normalizedInterfaceType);

            if (processedInterfaces.Add(normalizedInterfaceType))
              interfaces.Add(new KeyValuePair<Type, Type>(normalizedInterfaceType, type));
          }

          if (type.IsInterface)
            interfaces.Add(new KeyValuePair<Type, Type>(type, type));
          else
            classes.Add(new KeyValuePair<Type, Type>(type, type));
        }

        var classCache = classes.ToLookup(kvp => kvp.Key, kvp => kvp.Value, MemberInfoEqualityComparer<Type>.Instance);
        var interfaceCache = interfaces.ToLookup(kvp => kvp.Key, kvp => kvp.Value, MemberInfoEqualityComparer<Type>.Instance);

        return new BaseTypeCache(classCache, interfaceCache);
      }

      static void CheckTypeIsNormalized (Type type)
      {
        if (type.IsConstructedGenericType)
        {
          throw new ArgumentException(
              string.Format(
                  "Only non-generic types and open generic types may be used when creating a BaseTypeCache. Type '{0}' is a closed generic type.",
                  type.GetFullNameSafe()),
              "types");
        }
      }

      [return: NotNullIfNotNull("type")]
      static Type? GetNormalizedType (Type? type)
      {
        if (type == null)
          return null;
        if (type.IsConstructedGenericType)
          return type.GetGenericTypeDefinition();
        return type;
      }
    }

    private readonly ILookup<Type, Type> _classCache;
    private readonly ILookup<Type, Type> _interfaceCache;

    private BaseTypeCache (ILookup<Type, Type> classCache, ILookup<Type, Type> interfaceCache)
    {
      _classCache = classCache;
      _interfaceCache = interfaceCache;
    }

    public ICollection GetTypes (Type baseType)
    {
      ArgumentUtility.CheckNotNull("baseType", baseType);

      if (baseType == typeof(object))
        return _classCache.Concat(_interfaceCache).Select(g => g.Key).ToArray();

      var cache = baseType.IsInterface ? _interfaceCache : _classCache;

      return cache[baseType].ToList();
    }
  }
}
