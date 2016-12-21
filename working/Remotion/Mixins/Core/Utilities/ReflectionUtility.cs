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
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  public static class ReflectionUtility
  {
    private static readonly LockingCacheDecorator<Assembly, bool> s_isAssemblySignedCache = CacheFactory.CreateWithLocking<Assembly, bool>();

    public static bool IsMixinType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      
      return Reflection.TypeExtensions.CanAscribeTo (type, typeof (Mixin<>));
    }

    public static bool IsEqualOrInstantiationOf (Type typeToCheck, Type expectedType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("expectedType", expectedType);

      return typeToCheck.Equals (expectedType) || (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition().Equals (expectedType));
    }

    public static bool IsPublicOrProtected (MethodBase methodToCheck)
    {
      ArgumentUtility.CheckNotNull ("methodToCheck", methodToCheck);
      return methodToCheck.IsPublic || methodToCheck.IsFamily || methodToCheck.IsFamilyOrAssembly;
    }

    public static bool IsPublicOrProtectedOrExplicit (MethodBase methodToCheck)
    {
      ArgumentUtility.CheckNotNull ("methodToCheck", methodToCheck);
      return IsPublicOrProtected (methodToCheck) || (methodToCheck.IsPrivate && methodToCheck.IsVirtual);
    }

    public static bool IsNewSlotMember (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      return CheckMethodAttributeOnMember (member, MethodAttributes.NewSlot);
    }

    public static bool IsVirtualMember (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      return CheckMethodAttributeOnMember (member, MethodAttributes.Virtual);
    }

    private static bool CheckMethodAttributeOnMember (MemberInfo member, MethodAttributes attribute)
    {
      var method = member as MethodInfo;
      if (method != null)
        return (method.Attributes & attribute) == attribute;

      var property = member as PropertyInfo;
      if (property != null)
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);
        return (getMethod != null && CheckMethodAttributeOnMember (getMethod, attribute))
            || (setMethod != null && CheckMethodAttributeOnMember (setMethod, attribute));
      }

      var eventInfo = member as EventInfo;
      if (eventInfo != null)
        return CheckMethodAttributeOnMember(eventInfo.GetAddMethod (), attribute)
            || CheckMethodAttributeOnMember(eventInfo.GetRemoveMethod (), attribute);

      string message = String.Format (
          "The given member {0}.{1} is neither property, method, nor event.",
          member.DeclaringType.FullName,
          member.Name);
      throw new ArgumentException (message, "member");
    }

    public static IEnumerable<MethodInfo> RecursiveGetAllMethods (Type type, BindingFlags bindingFlags)
    {
      foreach (MethodInfo method in type.GetMethods(bindingFlags | BindingFlags.DeclaredOnly))
        yield return method;

      if (type.BaseType != null)
      {
        foreach (MethodInfo method in RecursiveGetAllMethods (type.BaseType, bindingFlags))
          yield return method;
      }
    }

    public static IEnumerable<PropertyInfo> RecursiveGetAllProperties (Type type, BindingFlags bindingFlags)
    {
      foreach (PropertyInfo property in type.GetProperties (bindingFlags | BindingFlags.DeclaredOnly))
        yield return property;

      if (type.BaseType != null)
      {
        foreach (PropertyInfo property in RecursiveGetAllProperties (type.BaseType, bindingFlags))
          yield return property;
      }
    }

    public static IEnumerable<EventInfo> RecursiveGetAllEvents (Type type, BindingFlags bindingFlags)
    {
      foreach (EventInfo eventInfo in type.GetEvents (bindingFlags | BindingFlags.DeclaredOnly))
        yield return eventInfo;

      if (type.BaseType != null)
      {
        foreach (EventInfo eventInfo in RecursiveGetAllEvents (type.BaseType, bindingFlags))
          yield return eventInfo;
      }
    }

    public static bool IsAssemblySigned (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      return s_isAssemblySignedCache.GetOrCreateValue (assembly, asm => IsAssemblySigned (asm.GetName ()));
    }

    public static bool IsAssemblySigned (AssemblyName assemblyName)
    {
      ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);
      byte[] publicKeyOrToken = assemblyName.GetPublicKey () ?? assemblyName.GetPublicKeyToken ();
      return publicKeyOrToken != null && publicKeyOrToken.Length > 0;
    }

    public static bool IsReachableFromSignedAssembly (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (!IsAssemblySigned (type.Assembly))
        return false;

      if (type.IsGenericType)
        return type.GetGenericArguments ().All (IsReachableFromSignedAssembly);
      else
        return true;
    }

    public static bool IsRangeReachableFromSignedAssembly (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      return types.All (IsReachableFromSignedAssembly);
    }

    public static MethodInfo[] GetAssociatedMethods (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
          return new[] { (MethodInfo) memberInfo };

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo) memberInfo;
          return propertyInfo.GetAccessors (true);

        case MemberTypes.Event:
          var eventInfo = (EventInfo) memberInfo;
          return new[] { eventInfo.GetAddMethod (true), eventInfo.GetRemoveMethod (true) };

        default:
          throw new InvalidOperationException ("Associated methods can only be retrieved for methods, properties, and events.");
      }
    }
  }
}
