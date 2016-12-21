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
using System.Reflection;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  [ImplementationFor (typeof (IMemberResolver), Lifetime = LifetimeKind.Singleton)]
  public class ReflectionBasedMemberResolver : IMemberResolver
  {
    private class CacheKey : IEquatable<CacheKey>
    {
      private readonly Type _type;
      private readonly string _methodName;
      private readonly BindingFlags _bindingFlags;

      public CacheKey (Type type, string methodName, BindingFlags bindingFlags)
      {
        ArgumentUtility.DebugCheckNotNull ("type", type);
        ArgumentUtility.DebugCheckNotNullOrEmpty ("methodName", methodName);

        _type = type;
        _methodName = methodName;
        _bindingFlags = bindingFlags;
      }

      public Type Type
      {
        get { return _type; }
      }

      public string MethodName
      {
        get { return _methodName; }
      }

      public BindingFlags BindingFlags
      {
        get { return _bindingFlags; }
      }

      public override int GetHashCode ()
      {
        return _type.GetHashCode () ^ _methodName[0];
      }

      public bool Equals (CacheKey other)
      {
        return EqualityUtility.NotNullAndSameType (this, other)
               && _type.Equals (other._type)
               && string.Equals (_methodName, other._methodName)
               && _bindingFlags == other._bindingFlags;
      }
    }

    private static readonly ICache<CacheKey, IMethodInformation> s_cache = CacheFactory.CreateWithLocking<CacheKey, IMethodInformation>();

    public ReflectionBasedMemberResolver ()
    {
    }

    public IMethodInformation GetMethodInformation (Type type, string methodName, MemberAffiliation memberAffiliation)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      
      switch (memberAffiliation)
      {
        case MemberAffiliation.Instance:
          return GetMethodFromCache (type, methodName, BindingFlags.Public | BindingFlags.Instance);
        case MemberAffiliation.Static:
          return GetMethodFromCache (type, methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        default:
          throw new ArgumentException ("Wrong parameter 'memberAffiliation' passed.");
      }
    }

    public IMethodInformation GetMethodInformation (Type type, MethodInfo methodInfo, MemberAffiliation memberAffiliation)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      
      return GetMethodInformation (type, methodInfo.Name, memberAffiliation);
    }

    private IMethodInformation GetMethodFromCache (Type type, string memberName, BindingFlags bindingFlags)
    {
      var cacheKey = new CacheKey (type, memberName, bindingFlags);
      return s_cache.GetOrCreateValue (cacheKey, key => GetMethod (key.Type, key.MethodName, key.BindingFlags));
    }

    private IMethodInformation GetMethod (Type type, string methodName, BindingFlags bindingFlags)
    {
      if (!TypeHasMember (type, methodName, bindingFlags))
        throw new ArgumentException (string.Format ("The method '{0}' could not be found.", methodName), "methodName");

      var foundMembers = new List<MemberInfo> ();
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
        foundMembers.AddRange (currentType.FindMembers (MemberTypes.Method, bindingFlags | BindingFlags.DeclaredOnly, IsSecuredMethod, methodName));

      if (foundMembers.Count == 0)
        return new NullMethodInformation();

      var foundMethodInfo = (MethodInfo) foundMembers[0];
      if (type.BaseType != null && foundMethodInfo.DeclaringType == type && TypeHasMember (type.BaseType, methodName, bindingFlags))
      {
        throw new ArgumentException (
            string.Format (
                "The DemandPermissionAttribute must not be defined on methods overriden or redefined in derived classes. "
                + "A method '{0}' exists in class '{1}' and its base class.",
                methodName,
                type.FullName),
            "methodName");
      }

      return MethodInfoAdapter.Create(foundMethodInfo);
    }

    private bool TypeHasMember (Type type, string methodName, BindingFlags bindingFlags)
    {
      MemberInfo[] existingMembers = type.GetMember (methodName, MemberTypes.Method, bindingFlags);
      return existingMembers.Length > 0;
    }

    private bool IsSecuredMethod (MemberInfo memberInfo, object filterCriteria)
    {
      string memberName = (string) filterCriteria;
      return memberInfo.Name == memberName && memberInfo.IsDefined (typeof (DemandPermissionAttribute), false);
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}