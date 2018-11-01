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
using Castle.DynamicProxy;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Scripting.StableBindingImplementation
{
  /// <summary>
  /// Builds a proxy object which exposes only selected methods/properties, as decided by its <see cref="ITypeFilter"/>. 
  /// </summary>
  /// <remarks>
  /// What methods/properties are to be exposed is dependent on whether the method/property comes from a type which is
  /// classified as "valid" by the <see cref="ITypeFilter"/> of the class.
  /// <para/> 
  /// Used by <see cref="StableBindingProxyProvider"/>.
  /// <para/> 
  /// Uses <see cref="ForwardingProxyBuilder"/>.
  /// </remarks>
  public class StableBindingProxyBuilder
  {
    private readonly Type _proxiedType;
    private readonly ITypeFilter _typeFilter;
    private readonly ForwardingProxyBuilder _forwardingProxyBuilder;
    private readonly Dictionary<MethodInfo, HashSet<MethodInfo>> _classMethodToInterfaceMethodsMap = new Dictionary<MethodInfo, HashSet<MethodInfo>> ();
    private readonly ModuleScope _moduleScope;
    private readonly Type[] _knownInterfaces;
    private readonly MethodInfo[] _publicMethodsInFirstKnownBaseType;
    private readonly Type _firstKnownBaseType;
    private readonly StableMetadataTokenToMethodInfoMap _knownBaseTypeStableMetadataTokenToMethodInfoMap;
    private readonly Dictionary<StableMetadataToken, PropertyInfo> _firstKnownBaseTypeSpecialMethodsToPropertyMap;

    public StableBindingProxyBuilder (Type proxiedType, ITypeFilter typeFilter, ModuleScope moduleScope)
    {
      ArgumentUtility.CheckNotNull ("proxiedType", proxiedType);
      ArgumentUtility.CheckNotNull ("typeFilter", typeFilter);
      _typeFilter = typeFilter;
      _moduleScope = moduleScope;
      _proxiedType = proxiedType;
      
      _knownInterfaces = FindKnownInterfaces();
      BuildClassMethodToInterfaceMethodsMap ();

      _forwardingProxyBuilder = new ForwardingProxyBuilder (_proxiedType.Name, _moduleScope, _proxiedType, _knownInterfaces);

      _firstKnownBaseType = GetFirstKnownBaseType();
      if (_firstKnownBaseType != null)
      {
        _publicMethodsInFirstKnownBaseType = _firstKnownBaseType.GetMethods().ToArray();
        _knownBaseTypeStableMetadataTokenToMethodInfoMap = new StableMetadataTokenToMethodInfoMap (_firstKnownBaseType);
        _firstKnownBaseTypeSpecialMethodsToPropertyMap = BuildSpecialMethodsToPropertyMap (_firstKnownBaseType);
      }
    }

 
    public Type ProxiedType
    {
      get { return _proxiedType; }
    }

    /// <summary>
    /// Builds the proxy <see cref="Type"/> which exposes all known methods and properties and forwards calls to the proxied <see cref="Type"/>.
    /// </summary>
    public Type BuildProxyType ()
    {
      ImplementKnownMethods ();
      ImplementKnownProperties ();
      return _forwardingProxyBuilder.BuildProxyType ();
    }

    private void ImplementKnownProperties ()
    {
      ImplementKnownClassProperties();
      ImplementKnownInterfaceProperties();
    }


    private void ImplementKnownClassProperties ()
    {
      var specialMethodsInProxiedType = _proxiedType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (mi => mi.IsSpecialName);

      foreach (var proxiedTypeMethod in specialMethodsInProxiedType)
      {
        PropertyInfo knownBaseTypeProperty = null;
        if (_firstKnownBaseType != null)
        {
          _firstKnownBaseTypeSpecialMethodsToPropertyMap.TryGetValue (new StableMethodMetadataToken (proxiedTypeMethod), out knownBaseTypeProperty);
        }

        if (knownBaseTypeProperty != null)
        {
          _forwardingProxyBuilder.AddForwardingPropertyFromClassOrInterfacePropertyInfoCopy (knownBaseTypeProperty);
        }
      }
    }

    private void ImplementKnownInterfaceProperties ()
    {
      Type type = _proxiedType;
      while (type != null)
      {
        var classMethodToInterfaceMethodsMap = GetClassMethodToInterfaceMethodsMap(type);
        ImplementPublicInterfaceProperties(type, classMethodToInterfaceMethodsMap);
        ImplementNonPublicInterfaceProperties(type, classMethodToInterfaceMethodsMap);
        type = type.BaseType;
      }
    }


    private void ImplementPublicInterfaceProperties (Type type, Dictionary<MethodInfo, HashSet<MethodInfo>> classMethodToInterfaceMethodsMap)
    {
      var typePublicProperties = type.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

      foreach (var publicProperty in typePublicProperties)
      {
        var typeNonPublicPropertyAccessors = publicProperty.GetAccessors (true);
        if (typeNonPublicPropertyAccessors.Any (mi => classMethodToInterfaceMethodsMap.ContainsKey (mi)))
        {
          _forwardingProxyBuilder.AddForwardingPropertyFromClassOrInterfacePropertyInfoCopy (publicProperty);
        }
      }
    }
    
    private void ImplementNonPublicInterfaceProperties (Type type, Dictionary<MethodInfo, HashSet<MethodInfo>> classMethodToInterfaceMethodsMap)
    {
      var typeNonPublicProperties = type.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);

      foreach (var nonPublicProperty in typeNonPublicProperties)
      {
        var typeNonPublicPropertyAccessors = nonPublicProperty.GetAccessors (true);
        if (typeNonPublicPropertyAccessors.Any (mi => classMethodToInterfaceMethodsMap.ContainsKey (mi)))
        {
          var getter = GetInterfaceMethodsToClassMethod (nonPublicProperty.GetGetMethod (true),classMethodToInterfaceMethodsMap).Single (); 
          var setter = GetInterfaceMethodsToClassMethod (nonPublicProperty.GetSetMethod (true),classMethodToInterfaceMethodsMap).Single ();

          _forwardingProxyBuilder.AddForwardingExplicitInterfaceProperty (nonPublicProperty, getter, setter);
        }
      }
    }


    private Dictionary<MethodInfo, HashSet<MethodInfo>> GetClassMethodToInterfaceMethodsMap (Type type)
    {
      var classMethodToInterfaceMethodsMap = new Dictionary<MethodInfo, HashSet<MethodInfo>>();
      var knownInterfacesInType = _knownInterfaces.Intersect (type.GetInterfaces());
      foreach (var knownInterface in knownInterfacesInType)
      {
        var interfaceMapping = type.GetInterfaceMap (knownInterface);
        var classMethods = interfaceMapping.TargetMethods;
        var interfaceMethods = interfaceMapping.InterfaceMethods;

        for (int i = 0; i < classMethods.Length; i++)
        {
          var classMethod = classMethods[i];
          if (classMethod.IsSpecialName)
          {
            if (!classMethodToInterfaceMethodsMap.ContainsKey (classMethod))
            {
              classMethodToInterfaceMethodsMap[classMethod] = new HashSet<MethodInfo>();
            }
            classMethodToInterfaceMethodsMap[classMethod].Add (interfaceMethods[i]);
          }
        }
      }

      return classMethodToInterfaceMethodsMap;
    }


 
    private Dictionary<StableMetadataToken, PropertyInfo> BuildSpecialMethodsToPropertyMap (Type startType)
    {
      const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public; 
      
      var specialMethodsToPropertiesMap = new Dictionary<StableMetadataToken, PropertyInfo> ();

      var declaredOnlyProperties = _firstKnownBaseType.CreateSequence (t => t.BaseType).SelectMany (t => t.GetProperties (bindingFlags));
      foreach (var property in declaredOnlyProperties)
      {
        foreach (var getterSetter in property.GetAccessors (true))
        {
          var stableMethodMetadataToken = new StableMethodMetadataToken (getterSetter);
          // Only store first (= nearest to proxiedType) property.
          if (!specialMethodsToPropertiesMap.ContainsKey (stableMethodMetadataToken))
          {
            specialMethodsToPropertiesMap[stableMethodMetadataToken] = property;
          }
        }
      }

      return specialMethodsToPropertiesMap;
    }

 
    private void ImplementKnownMethods ()
    {
      var regularMethodsInProxiedType = _proxiedType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(mi => !mi.IsSpecialName);
      foreach (var proxiedTypeMethod in regularMethodsInProxiedType)
      {
        MethodInfo proxiedTypeMethodInKnownBaseType = null;
        if (_firstKnownBaseType != null)
        {
          proxiedTypeMethodInKnownBaseType = _knownBaseTypeStableMetadataTokenToMethodInfoMap.GetMethod (proxiedTypeMethod);
        }

        if (proxiedTypeMethodInKnownBaseType != null && // method exists in first known base type
            IsMethodBound (proxiedTypeMethodInKnownBaseType, _publicMethodsInFirstKnownBaseType)) // method is visible in first known base type
        {
          _forwardingProxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (proxiedTypeMethodInKnownBaseType);
        }
        else
        {
          var interfaceMethodsToClassMethod = GetInterfaceMethodsToClassMethod (proxiedTypeMethod);
          foreach (var interfaceMethod in interfaceMethodsToClassMethod)
          {
            // Add forwarding interface implementations for methods whose target method info has not already been implemented.
            _forwardingProxyBuilder.AddForwardingExplicitInterfaceMethod (interfaceMethod);
          }
        }
      }
    }

    /// <summary>
    /// Returns <see langword="true"/> if the passed <paramref name="method"/> would be picked by C#
    /// out of the passed methods when calling a method with the <paramref name="method"/>|s name and parameter <see cref="Type"/>|s.
    /// </summary>
    public static bool IsMethodBound (MethodInfo method, MethodInfo[] candidateMethods)
    {
      var parameterTypes = method.GetParameters ().Select (pi => pi.ParameterType).ToArray ();

      // Note: SelectMethod needs the candidateMethods already to have been filtered by name, otherwise AmbiguousMatchException|s may occur.
      candidateMethods = candidateMethods.Where (mi => (mi.Name == method.Name)).ToArray ();

      // Binder.SelectMethod throws when candidateMethods are empty.
      if (candidateMethods.Length == 0)
      {
        return false;
      }

      var boundMethod = Type.DefaultBinder.SelectMethod (BindingFlags.Instance | BindingFlags.Public, 
        candidateMethods, parameterTypes, null);

      return Object.ReferenceEquals (method, boundMethod);
    }


    // TODO: If IsMethodEqualToBaseTypeMethod can be expressed as a CompoundValueEqualityComparer<MethodInfo>,
    // (MethodInfoFromRelatedTypesEqualityComparer) refactor back to initial implementation using HashSet, 
    // to get rid of quadratic runtime behavior (see "CreateMethodsKnownInBaseTypeSet" in bag.txt).
    public bool IsMethodKnownInBaseType (MethodInfo method)
    {
      foreach (var baseTypeMethod in _publicMethodsInFirstKnownBaseType)
      {
        if (!baseTypeMethod.IsSpecialName)
        {
          if (IsMethodEqualToBaseTypeMethod (method, baseTypeMethod))
          {
            return true;
          }
        }
      }

      return false; 
    }

    public bool IsMethodEqualToBaseTypeMethod (MethodInfo method, MethodInfo baseTypeMethod)
    {
      return method.GetBaseDefinition().MetadataToken == baseTypeMethod.GetBaseDefinition().MetadataToken;
    }

 
    private Type GetFirstKnownBaseType ()
    {
      // Object is always known.
      return ProxiedType.CreateSequence (t => t.BaseType).FirstOrDefault (_typeFilter.IsTypeValid) ?? typeof(object);
    }

    private Type[] FindKnownInterfaces ()
    {
      return ProxiedType.GetInterfaces ().Where (i => _typeFilter.IsTypeValid (i)).ToArray ();
    }

    private void BuildClassMethodToInterfaceMethodsMap ()
    {
      foreach (var knownInterface in _knownInterfaces)
      {
        var interfaceMapping = ProxiedType.GetInterfaceMap (knownInterface);
        var classMethods = interfaceMapping.TargetMethods;
        var interfaceMethods = interfaceMapping.InterfaceMethods;
        for (int i = 0; i < classMethods.Length; i++) 
        {
          AddTo_MethodToInterfaceMethodsMap (classMethods[i], interfaceMethods[i]);
        }
      }
    }

    private void AddTo_MethodToInterfaceMethodsMap (MethodInfo classMethod, MethodInfo interfaceMethod)
    {
      if (!_classMethodToInterfaceMethodsMap.ContainsKey (classMethod))
      {
        _classMethodToInterfaceMethodsMap[classMethod] = new HashSet<MethodInfo> ();
      }
      _classMethodToInterfaceMethodsMap[classMethod].Add (interfaceMethod);
    }

    private IEnumerable<MethodInfo> GetInterfaceMethodsToClassMethod (MethodInfo classMethod)
    {
      return GetInterfaceMethodsToClassMethod (classMethod, _classMethodToInterfaceMethodsMap);
    }

    private IEnumerable<MethodInfo> GetInterfaceMethodsToClassMethod (MethodInfo classMethod, 
      Dictionary<MethodInfo, HashSet<MethodInfo>> classMethodToInterfaceMethodsMap)
    {
      HashSet<MethodInfo> interfaceMethodsToClassMethod;
      classMethodToInterfaceMethodsMap.TryGetValue (classMethod, out interfaceMethodsToClassMethod);
      return (IEnumerable<MethodInfo>) interfaceMethodsToClassMethod ?? new MethodInfo[0];
    }
  }

 
}
