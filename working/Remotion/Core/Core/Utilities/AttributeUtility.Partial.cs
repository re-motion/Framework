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
using System.Reflection;
using Remotion.Collections;
using Remotion.Utilities.AttributeRetrieval;

namespace Remotion.Utilities
{
  /// <summary>
  /// Utility class for finding custom attributes via their type or an interface implemented by the type.
  /// </summary>
  public static partial class AttributeUtility
  {
    // Caching SuppressAttributes is safe because they will never mutate through usage.
    // (As shown by their implementation, and all used members are non-virtual.)

    private static readonly ICache<Tuple<Type, bool>, AttributeWithMetadata[]> s_suppressAttributesCache =
        CacheFactory.CreateWithLocking<Tuple<Type, bool>, AttributeWithMetadata[]>();

    private static readonly PropertyCustomAttributeRetriever s_propertyCustomAttributeRetriever = new PropertyCustomAttributeRetriever ();
    private static readonly EventCustomAttributeRetriever s_eventCustomAttributeRetriever = new EventCustomAttributeRetriever ();

   public static bool IsDefined<T> (MemberInfo element, bool inherit)
       where T : class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");

      return IsDefined (element, typeof (T), inherit);
    }

    public static bool IsDefined (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      return GetCustomAttributes (element, attributeType, inherit).Length > 0;
    }

    public static T GetCustomAttribute<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      return (T) (object) GetCustomAttribute (element, typeof (T), inherit);
    }

    public static Attribute GetCustomAttribute (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      object[] attributeArray = GetCustomAttributes (element, attributeType, inherit);
      if ((attributeArray == null) || (attributeArray.Length == 0))
        return null;
      if (attributeArray.Length != 1)
        throw new AmbiguousMatchException ("Multiple custom attributes of the same type found.");
      return (Attribute)attributeArray[0];
    }

    public static T[] GetCustomAttributes<T> (MemberInfo element, bool inherit)
        where T: class
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (typeof (T), "T");
      
      return (T[])GetCustomAttributes (element, typeof (T), inherit);
    }

    public static object[] GetCustomAttributes (MemberInfo element, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      CheckAttributeType (attributeType, "attributeType");

      var elementAsType = element as Type;
      if (elementAsType != null)
        return GetCustomAttributes (elementAsType, attributeType, inherit);

      var elementAsProperty = element as PropertyInfo;
      if (elementAsProperty != null)
        return s_propertyCustomAttributeRetriever.GetCustomAttributes (elementAsProperty, attributeType, inherit);

      var elementAsEvent = element as EventInfo;
      if (elementAsEvent != null)
        return s_eventCustomAttributeRetriever.GetCustomAttributes (elementAsEvent, attributeType, inherit);

      return element.GetCustomAttributes (attributeType, inherit);
    }

    public static object[] GetCustomAttributes (Type type, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      CheckAttributeType (attributeType, "attributeType");

      var attributesWithRightType = GetCustomAttributesWithMetadata (type, attributeType, inherit);
      var filteredAttributes = AttributeWithMetadata.ExcludeAll (attributesWithRightType, typeof (SuppressAttributesAttribute));

      var suppressAttributes = GetCachedSuppressAttributes (type, inherit);
      var nonSuppressedAttributes = AttributeWithMetadata.Suppress (filteredAttributes, suppressAttributes);
      var attributeInstances = AttributeWithMetadata.ExtractInstances (nonSuppressedAttributes);

      return CreateTypedArray (attributeInstances, attributeType);
    }

    public static IEnumerable<AttributeWithMetadata> GetCustomAttributesWithMetadata (Type type, Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var attributeUsageAttributes = new Dictionary<Type, AttributeUsageAttribute>();

      Type currentType = type;
      do
      {
        var currentAttributes = currentType.GetCustomAttributes (attributeType, false); // get attributes exactly for current type
        foreach (Attribute currentAttribute in currentAttributes)
        {
          var currentAttributeType = currentAttribute.GetType();
          AttributeUsageAttribute currentAttributeUsage;

          if (!attributeUsageAttributes.TryGetValue (currentAttributeType, out currentAttributeUsage))
          {
            currentAttributeUsage = GetAttributeUsage (currentAttributeType);
            attributeUsageAttributes.Add (currentAttributeType, currentAttributeUsage);
            if (currentType == type || currentAttributeUsage.Inherited)
              yield return new AttributeWithMetadata (currentType, currentAttribute);
          }
          else if ((currentType == type || currentAttributeUsage.Inherited) && currentAttributeUsage.AllowMultiple)
          {
            yield return new AttributeWithMetadata (currentType, currentAttribute);
          }
        }
        currentType = currentType.BaseType;
      } while (inherit && currentType != null && currentType != typeof (object)); // iterate unless inherit == false, stop when typeof (object) is reached
    }

    private static object[] CreateTypedArray (IEnumerable attributeInstances, Type elementType)
    {
      var arrayList = new ArrayList ();
      foreach (var attributeInstance in attributeInstances)
      {
        arrayList.Add (attributeInstance);
      }
      return (object[]) arrayList.ToArray (elementType);
    }

    private static AttributeWithMetadata[] GetCachedSuppressAttributes (Type type, bool inherit)
    {
      AttributeWithMetadata[] result;
      var key = Tuple.Create (type, inherit);
      if (!s_suppressAttributesCache.TryGetValue (key, out result))
      {
        result = s_suppressAttributesCache.GetOrCreateValue (
            key, k => GetCustomAttributesWithMetadata (k.Item1, typeof (SuppressAttributesAttribute), k.Item2).ToArray());
      }
      return result;
    }

    private static void CheckAttributeType (Type attributeType, string parameterName)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      if (!typeof (Attribute).IsAssignableFrom (attributeType) && !attributeType.IsInterface)
      {
        string message = "The attribute type must be assignable to System.Attribute or an interface.";
        throw new ArgumentException (message, parameterName);
      }
    }
  }
}
