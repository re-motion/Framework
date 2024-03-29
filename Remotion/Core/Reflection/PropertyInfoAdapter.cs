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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IPropertyInformation"/> interface to wrap a <see cref="PropertyInfo"/> instance.
  /// </summary>
  [TypeConverter(typeof(PropertyInfoAdapterConverter))]
  public sealed class PropertyInfoAdapter : IPropertyInformation
  {
    private static readonly ConcurrentDictionary<PropertyInfo, PropertyInfoAdapter> s_dataStore =
        new ConcurrentDictionary<PropertyInfo, PropertyInfoAdapter>(MemberInfoEqualityComparer<PropertyInfo>.Instance);

    ///<remarks>Optimized for memory allocations</remarks>
    private static readonly Func<PropertyInfo, PropertyInfoAdapter> s_ctorFunc = pi => new PropertyInfoAdapter(pi);

    public static PropertyInfoAdapter Create (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      return s_dataStore.GetOrAdd(propertyInfo, s_ctorFunc);
    }

    [ContractAnnotation("null => null; notnull => notnull")]
    [return: NotNullIfNotNull("propertyInfo")]
    public static PropertyInfoAdapter? CreateOrNull (PropertyInfo? propertyInfo)
    {
      if (propertyInfo == null)
        return null;

      return s_dataStore.GetOrAdd(propertyInfo, s_ctorFunc);
    }

    private readonly PropertyInfo _propertyInfo;
    private readonly Lazy<IMethodInformation?> _publicGetMethod;
    private readonly Lazy<IMethodInformation?> _publicOrNonPublicGetMethod;
    private readonly Lazy<IMethodInformation?> _publicSetMethod;
    private readonly Lazy<IMethodInformation?> _publicOrNonPublicSetMethod;
    private readonly Lazy<IReadOnlyCollection<IMethodInformation>> _publicAccessors;
    private readonly Lazy<IReadOnlyCollection<IMethodInformation>> _publicOrNonPublicAccessors;

    private readonly Lazy<ITypeInformation?> _cachedDeclaringType;
    private readonly Lazy<ITypeInformation> _cachedOriginalDeclaringType;
    private readonly Lazy<IPropertyInformation> _cachedOriginalDeclaration;

    private readonly Lazy<IReadOnlyCollection<IPropertyInformation>> _interfaceDeclarations;

    private PropertyInfoAdapter (PropertyInfo propertyInfo)
    {
      _propertyInfo = propertyInfo;

      // TODO RM-7777: Check if Create or CreateOrNull should be used in the following instantiations.
      _publicGetMethod = new Lazy<IMethodInformation?>(
          () => MethodInfoAdapter.CreateOrNull(_propertyInfo.GetGetMethod(false)),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _publicOrNonPublicGetMethod = new Lazy<IMethodInformation?>(
          () => MethodInfoAdapter.CreateOrNull(_propertyInfo.GetGetMethod(true)),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _publicSetMethod = new Lazy<IMethodInformation?>(
          () => MethodInfoAdapter.CreateOrNull(_propertyInfo.GetSetMethod(false)),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _publicOrNonPublicSetMethod = new Lazy<IMethodInformation?>(
          () => MethodInfoAdapter.CreateOrNull(_propertyInfo.GetSetMethod(true)),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _publicAccessors = new Lazy<IReadOnlyCollection<IMethodInformation>>(
          () => _propertyInfo.GetAccessors(false).Select(MethodInfoAdapter.Create).ToArray(),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _publicOrNonPublicAccessors = new Lazy<IReadOnlyCollection<IMethodInformation>>(
          () => _propertyInfo.GetAccessors(true).Select(MethodInfoAdapter.Create).ToArray(),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _cachedDeclaringType = new Lazy<ITypeInformation?>(
          () => TypeAdapter.CreateOrNull(_propertyInfo.DeclaringType),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _cachedOriginalDeclaringType = new Lazy<ITypeInformation>(
          () => TypeAdapter.Create(_propertyInfo.GetOriginalDeclaringType()),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _cachedOriginalDeclaration = new Lazy<IPropertyInformation>(
          () => PropertyInfoAdapter.Create(_propertyInfo.GetBaseDefinition()),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _interfaceDeclarations = new Lazy<IReadOnlyCollection<IPropertyInformation>>(
          FindInterfaceDeclarationsImplementation,
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    public string Name
    {
      get { return _propertyInfo.Name; }
    }

    public ITypeInformation? DeclaringType
    {
      get { return _cachedDeclaringType.Value; }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      return _cachedOriginalDeclaringType.Value;
    }

    public IPropertyInformation GetOriginalDeclaration ()
    {
      return _cachedOriginalDeclaration.Value;
    }

    public bool CanBeSetFromOutside
    {
      get { return _publicSetMethod.Value != null; }
    }

    public T? GetCustomAttribute<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttribute<T>(_propertyInfo, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttributes<T>(_propertyInfo, inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return AttributeUtility.IsDefined<T>(_propertyInfo, inherited);
    }

    public object? GetValue (object? instance, object[]? indexParameters)
    {
      //TODO RM-7432: Remove null check, parameter should be nullable
      ArgumentUtility.CheckNotNull("instance", instance!);

      return _propertyInfo.GetValue(instance, indexParameters);
    }

    public void SetValue (object? instance, object? value, object[]? indexParameters)
    {
      //TODO RM-7432: Remove null check, parameter should be nullable
      ArgumentUtility.CheckNotNull("instance", instance!);

      _propertyInfo.SetValue(instance, value, indexParameters);
    }

    public IMethodInformation? GetGetMethod (bool nonPublic)
    {
      if (nonPublic)
        return _publicOrNonPublicGetMethod.Value;
      else
        return _publicGetMethod.Value;
    }

    public IMethodInformation? GetSetMethod (bool nonPublic)
    {
      if (nonPublic)
        return _publicOrNonPublicSetMethod.Value;
      else
        return _publicSetMethod.Value;
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return _propertyInfo.GetIndexParameters();
    }

    public IMethodInformation[] GetAccessors (bool nonPublic)
    {
      if (nonPublic)
        return _publicOrNonPublicAccessors.Value.ToArray();
      else
        return _publicAccessors.Value.ToArray();
    }

    public IPropertyInformation? FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull("implementationType", implementationType);

      // TODO RM-7802: DeclaringType being null should be handled.
      if (!DeclaringType!.IsInterface)
        throw new InvalidOperationException("This property is not an interface property.");

      var interfaceAccessorMethod = _publicAccessors.Value.First();
      var implementationMethod = interfaceAccessorMethod.FindInterfaceImplementation(implementationType);
      if (implementationMethod == null)
        return null;

      var implementationProperty = implementationMethod.FindDeclaringProperty();

      Assertion.IsNotNull(
          implementationProperty,
          "We assume that property acessor '" + implementationMethod + "' must be found on '" + implementationType + "'.");

      return implementationProperty;
    }

    public IEnumerable<IPropertyInformation> FindInterfaceDeclarations ()
    {
      return _interfaceDeclarations.Value.ToArray();
    }

    private IReadOnlyCollection<IPropertyInformation> FindInterfaceDeclarationsImplementation ()
    {
      // TODO RM-7802: DeclaringType being null should be handled.
      if (DeclaringType!.IsInterface)
        throw new InvalidOperationException("This property is itself an interface member, so it cannot have an interface declaration.");

      var accessorMethod = _publicOrNonPublicAccessors.Value.First();
      var interfaceAccessorMethods = accessorMethod.FindInterfaceDeclarations();
      //TODO RM-7432: Implementation does not match nullability of interface
      return interfaceAccessorMethods.Select(m => m.FindDeclaringProperty()!).ToArray();
    }

    public override bool Equals (object? obj)
    {
      return ReferenceEquals(this, obj);
    }

    public override int GetHashCode ()
    {
      return RuntimeHelpers.GetHashCode(this);
    }

    public override string? ToString ()
    {
      return _propertyInfo.ToString();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
