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
using JetBrains.Annotations;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;
using Remotion.Reflection.CodeGeneration;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(IConcreteTypeMetadataImporter), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class AttributeBasedMetadataImporter : IConcreteTypeMetadataImporter
  {
    public AttributeBasedMetadataImporter ()
    {
    }

    [CanBeNull]
    public virtual ClassContext? GetMetadataForMixedType (Type concreteMixedType)
    {
      ArgumentUtility.CheckNotNull("concreteMixedType", concreteMixedType);

      var attribute =
          (ConcreteMixedTypeAttribute?)concreteMixedType.GetCustomAttributes(typeof(ConcreteMixedTypeAttribute), false).SingleOrDefault();
      if (attribute != null)
        return attribute.GetClassContext();
      else
        return null;
    }

    [CanBeNull]
    public virtual ConcreteMixinTypeIdentifier? GetIdentifierForMixinType (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull("concreteMixinType", concreteMixinType);

      var attribute =
          (ConcreteMixinTypeAttribute?)concreteMixinType.GetCustomAttributes(typeof(ConcreteMixinTypeAttribute), false).SingleOrDefault();
      if (attribute != null)
        return attribute.GetIdentifier();
      else
        return null;
    }

    public virtual Dictionary<MethodInfo, MethodInfo> GetMethodWrappersForMixinType (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull("concreteMixinType", concreteMixinType);
      var wrappers = from potentialWrapper in concreteMixinType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                     let wrappedMethod = GetWrappedMethod(potentialWrapper)
                     where wrappedMethod != null
                     select new { Method = (MethodInfo)wrappedMethod, Wrapper = potentialWrapper };
      return wrappers.ToDictionary(pair => pair.Method, pair => pair.Wrapper);
    }

    // Searches the mixin methods corresponding to the methods of the override interface based on a signature comparison and returns a mapping from
    // mixin method to interface method.
    public virtual Dictionary<MethodInfo, MethodInfo> GetOverrideInterfaceMethodsByMixinMethod (
        Type interfaceType,
        ConcreteMixinTypeIdentifier identifier)
    {
      var mixinMethodsWithInterfaceMethods =
          from interfaceMethod in interfaceType.GetMethods()
          let attribute = (OverrideInterfaceMappingAttribute)interfaceMethod.GetCustomAttributes(typeof(OverrideInterfaceMappingAttribute), false)
              .Single()
          let resolvedMethod = attribute.ResolveReferencedMethod()
          select new { resolvedMethod, interfaceMethod };

      return mixinMethodsWithInterfaceMethods.ToDictionary(pair => pair.resolvedMethod, pair => pair.interfaceMethod);
    }

    private MethodInfo? GetWrappedMethod (MethodInfo potentialWrapper)
    {
      var attribute = GetWrapperAttribute(potentialWrapper);
      if (attribute != null)
        return attribute.ResolveReferencedMethod();
      else
        return null;
    }

    // This is a separate method in order to be able to test it with Rhino.Mocks.
    protected virtual GeneratedMethodWrapperAttribute? GetWrapperAttribute (MethodInfo potentialWrapper)
    {
      return AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute>(potentialWrapper, false);
    }

    public ConcreteMixinType? GetMetadataForMixinType (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull("concreteMixinType", concreteMixinType);

      var identifier = GetIdentifierForMixinType(concreteMixinType);
      if (identifier == null)
        return null;

      var generatedOverrideInterface = concreteMixinType.GetNestedType("IOverriddenMethods");
      if (generatedOverrideInterface == null)
      {
        var message = string.Format(
            "The given type '{0}' has a concrete mixin type identifier, but no IOverriddenMethods interface.",
            concreteMixinType);
        throw new TypeImportException(message);
      }

      var overrideInterfaceMethods = GetOverrideInterfaceMethodsByMixinMethod(generatedOverrideInterface, identifier);
      var methodWrappers = GetMethodWrappersForMixinType(concreteMixinType);

      return new ConcreteMixinType(identifier, concreteMixinType, generatedOverrideInterface, overrideInterfaceMethods, methodWrappers);
    }
  }
}
