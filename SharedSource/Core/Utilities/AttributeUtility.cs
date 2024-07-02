// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Concurrent;
using System.Threading;
#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>
  /// Utility class for finding custom attributes via their type or an interface implemented by the type.
  /// </summary>
  static partial class AttributeUtility
  {
    private static readonly ConcurrentDictionary<Type, Lazy<AttributeUsageAttribute>> s_attributeUsageCache =
        new ConcurrentDictionary<Type, Lazy<AttributeUsageAttribute>>();

    ///<remarks>Optimized for memory allocations</remarks>
    private static readonly Func<Type, Lazy<AttributeUsageAttribute>> s_getLazyAttributeUsageFunc = GetLazyAttributeUsage;

    public static bool IsAttributeInherited (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage(attributeType);
      return usage.Inherited;
    }

    public static bool IsAttributeAllowMultiple (Type attributeType)
    {
      AttributeUsageAttribute usage = GetAttributeUsage(attributeType);
      return usage.AllowMultiple;
    }

    public static AttributeUsageAttribute GetAttributeUsage (Type attributeType)
    {
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");

      var cachedInstance = s_attributeUsageCache.GetOrAdd(attributeType, s_getLazyAttributeUsageFunc).Value;

      var newInstance = new AttributeUsageAttribute(cachedInstance.ValidOn);
      newInstance.AllowMultiple = cachedInstance.AllowMultiple;
      newInstance.Inherited = cachedInstance.Inherited;
      return newInstance;
    }

    private static Lazy<AttributeUsageAttribute> GetLazyAttributeUsage (Type attributeType)
    {
      return new Lazy<AttributeUsageAttribute>(
          () =>
          {
            var usage = (AttributeUsageAttribute[])attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            if (usage.Length == 0)
              return new AttributeUsageAttribute(AttributeTargets.All);

            if (usage.Length > 1)
              throw new InvalidOperationException("AttributeUsageAttribute can only be applied once.");

            return usage[0];
          },
          LazyThreadSafetyMode.ExecutionAndPublication);
    }
  }
}
