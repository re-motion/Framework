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
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Holds the results of mixin code generation when a concrete mixin type was generated.
  /// </summary>
  /// <remarks>
  /// A concrete mixin type is a type derived from a mixin type that implements <see cref="OverrideMixinAttribute">mixin overrides</see> and holds
  /// public wrappers for protected methods needed to be accessed from the outside.
  /// </remarks>
  public class ConcreteMixinType : IMixinInfo
  {
    private readonly ConcreteMixinTypeIdentifier _identifier;
    private readonly Type _generatedType;
    private readonly Type _generatedOverrideInterface;
    private readonly Dictionary<MethodInfo, MethodInfo> _methodWrappers;
    private readonly Dictionary<MethodInfo, MethodInfo> _overrideInterfaceMethodsByMixinMethod;

    public ConcreteMixinType (
        ConcreteMixinTypeIdentifier identifier, 
        Type generatedType, 
        Type generatedOverrideInterface,
        Dictionary<MethodInfo, MethodInfo> overrideInterfaceMethodsByMixinMethod,
        Dictionary<MethodInfo, MethodInfo> methodWrappers)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("generatedType", generatedType);
      ArgumentUtility.CheckNotNull ("generatedOverrideInterface", generatedOverrideInterface);
      ArgumentUtility.CheckNotNull ("overrideInterfaceMethodsByMixinMethod", overrideInterfaceMethodsByMixinMethod);
      ArgumentUtility.CheckNotNull ("methodWrappers", methodWrappers);

      _identifier = identifier;
      _generatedType = generatedType;
      _generatedOverrideInterface = generatedOverrideInterface;
      _methodWrappers = methodWrappers;
      _overrideInterfaceMethodsByMixinMethod = overrideInterfaceMethodsByMixinMethod;
    }

    public ConcreteMixinTypeIdentifier Identifier
    {
      get { return _identifier; }
    }

    public Type GeneratedType
    {
      get { return _generatedType; }
    }

    public Type GeneratedOverrideInterface
    {
      get { return _generatedOverrideInterface; }
    }

    public Type MixinType
    {
      get { return GeneratedType; }
    }

    public IEnumerable<Type> GetInterfacesToImplement ()
    {
      yield return GeneratedOverrideInterface;
    }

    public MethodInfo GetPubliclyCallableMixinMethod (MethodInfo methodToBeCalled)
    {
      ArgumentUtility.CheckNotNull ("methodToBeCalled", methodToBeCalled);

      if (methodToBeCalled.IsPublic)
        return methodToBeCalled;

      MethodInfo wrapper;
      if (!_methodWrappers.TryGetValue (methodToBeCalled, out wrapper))
      {
        string message =
            string.Format ("No public wrapper was generated for method '{0}.{1}'.", methodToBeCalled.DeclaringType.FullName, methodToBeCalled.Name);
        throw new KeyNotFoundException (message);
      }
      else
      {
        return wrapper;
      }
    }

    public MethodInfo GetOverrideInterfaceMethod (MethodInfo mixinMethod)
    {
      ArgumentUtility.CheckNotNull ("mixinMethod", mixinMethod);

      MethodInfo interfaceMethod;
      if (!_overrideInterfaceMethodsByMixinMethod.TryGetValue (mixinMethod, out interfaceMethod))
      {
        string message =
            string.Format ("No override interface method was generated for method '{0}.{1}'.", mixinMethod.DeclaringType.FullName, mixinMethod.Name);
        throw new KeyNotFoundException (message);
      }
      else
      {
        return interfaceMethod;
      }
    }

    public ConcreteMixinType SubstituteMutableReflectionObjects (GeneratedTypesContext context)
    {
      var identifier = SubstituteConcreteMixinIdentifier (context, _identifier);
      var generatedType = Substitute (context, _generatedType);
      var generatedOverrideInterface = Substitute (context, _generatedOverrideInterface);
      var overrideInterfaceMethodsByMixinMethod = Substitute (context, _overrideInterfaceMethodsByMixinMethod);
      var methodWrappers = Substitute (context, _methodWrappers);

      return new ConcreteMixinType (identifier, generatedType, generatedOverrideInterface, overrideInterfaceMethodsByMixinMethod, methodWrappers);
    }

    private static ConcreteMixinTypeIdentifier SubstituteConcreteMixinIdentifier (GeneratedTypesContext context, ConcreteMixinTypeIdentifier identifier)
    {
      var mixinType = Substitute (context, identifier.MixinType);
      var overriders = identifier.Overriders.Select (m => Substitute (context, m));
      var overridden = identifier.Overridden.Select (m => Substitute (context, m));

      return new ConcreteMixinTypeIdentifier (mixinType, new HashSet<MethodInfo> (overriders), new HashSet<MethodInfo> (overridden));
    }

    private static Dictionary<MethodInfo, MethodInfo> Substitute (GeneratedTypesContext context, Dictionary<MethodInfo, MethodInfo> dictionary)
    {
      return dictionary.ToDictionary (p => Substitute (context, p.Key), p => Substitute (context, p.Value));
    }

    private static T Substitute<T> (GeneratedTypesContext context, T member)
        where T : MemberInfo
    {
      var mutableMember = member as IMutableMember;
      if (mutableMember != null)
        return (T) context.GetGeneratedMember (mutableMember);
      else
        return member;
    }
  }
}
