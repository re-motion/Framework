// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Remotion.Mixins.XRef.Utility
{
  public class FastMemberInvokerCache
  {
    public class CacheKey
    {
      private readonly Type _declaringType;
      private readonly string _memberName;
      private readonly Type[] _typeParameters;
      private readonly Type[] _argumentTypes;

      public CacheKey (Type declaringType, string memberName, Type[] typeParameters, Type[] argumentTypes)
      {
        Utilities.ArgumentUtility.CheckNotNull("declaringType", declaringType);
        Utilities.ArgumentUtility.CheckNotNull("memberName", memberName);
        Utilities.ArgumentUtility.CheckNotNull("argumentTypes", argumentTypes);

        _declaringType = declaringType;
        _memberName = memberName;
        _typeParameters = typeParameters;
        _argumentTypes = argumentTypes;
      }

      public override int GetHashCode ()
      {
        return _declaringType.GetHashCode()
               ^ _memberName.GetHashCode()
               ^ _typeParameters.Length
               ^ _argumentTypes.Length;
      }

      public override bool Equals (object obj)
      {
        var other = (CacheKey)obj;
        return _declaringType == other._declaringType
               && _memberName == other._memberName
               && _typeParameters.Length == other._typeParameters.Length
               && _typeParameters.SequenceEqual(other._typeParameters)
               && _argumentTypes.Length == other._argumentTypes.Length
               && _argumentTypes.SequenceEqual(other._argumentTypes);
      }
    }

    private readonly FastMemberInvokerGenerator _generator = new FastMemberInvokerGenerator();

    private readonly Dictionary<CacheKey, Func<object, object[], object>> _cache = new Dictionary<CacheKey, Func<object, object[], object>>();

    public Func<object, object[], object> GetOrCreateFastMethodInvoker (
        Type declaringType,
        string methodName,
        Type[] typeParameters,
        Type[] argumentTypes,
        BindingFlags bindingFlags)
    {
      Func<object, object[], object> invoker;

      var key = new CacheKey(declaringType, methodName, typeParameters, argumentTypes);
      if (!_cache.TryGetValue(key, out invoker))
      {
        invoker = _generator.GetFastMethodInvoker(declaringType, methodName, typeParameters, argumentTypes, bindingFlags);
        _cache.Add(key, invoker);
      }

      return invoker;
    }
  }
}
