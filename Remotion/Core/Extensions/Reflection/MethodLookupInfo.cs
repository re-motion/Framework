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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public class MethodLookupInfo : MemberLookupInfo
  {
    private static readonly ConcurrentDictionary<Tuple<Type, string>, Delegate> s_instanceMethodCache = new ConcurrentDictionary<Tuple<Type, string>, Delegate>();

    public MethodLookupInfo (string name, BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
        : base(name, bindingFlags, binder, callingConvention, parameterModifiers)
    {
    }

    public MethodLookupInfo (string name, BindingFlags bindingFlags)
        : base(name, bindingFlags)
    {
    }

    public MethodLookupInfo (string name)
        : base(name)
    {
    }

    public Delegate GetInstanceMethodDelegate (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("delegateType", delegateType, typeof(Delegate));

      return s_instanceMethodCache.GetOrAdd(
            new Tuple<Type, string>(delegateType, MemberName),
            key =>
            {
              var delegateTypeFromKey = key.Item1;
              Type[] parameterTypes = GetSignature(delegateTypeFromKey).Item1;
              if (parameterTypes.Length == 0)
                throw new InvalidOperationException("Method call delegate must have at least one argument for the current instance ('this' in C# or 'Me' in Visual Basic).");
              Type definingType = parameterTypes[0];
              parameterTypes = ArrayUtility.Skip(parameterTypes, 1);
              MethodInfo? method = definingType.GetMethod(MemberName, BindingFlags, Binder, CallingConvention, parameterTypes, ParameterModifiers);
              if (method == null)
                throw new InvalidOperationException($"No method named '{MemberName}' could be found on '{definingType.GetFullNameSafe()}'.");

              // TODO: verify return type
              return Delegate.CreateDelegate(delegateTypeFromKey, method);
            });
    }
  }
}
