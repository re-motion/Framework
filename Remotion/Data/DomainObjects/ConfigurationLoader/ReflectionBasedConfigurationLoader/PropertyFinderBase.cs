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
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>The <see cref="PropertyFinderBase"/> is used to find all <see cref="PropertyInfo"/> objects relevant for the mapping.</summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="PropertyFinderBase"/>'s constructor signature. </remarks>
  public abstract class PropertyFinderBase
  {
    public const BindingFlags PropertyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    /// <remarks>
    /// Note that <see cref="HashSet{T}"/> is not documented as threadsafe for read-only access in MSDN (as of May 2018) but we only require 
    /// the use of <see cref="HashSet{T}.Contains"/> on a non-modified <see cref="HashSet{T}"/>. By checking the source code (.NET Framework 4.7.2),
    /// it was verified that this operation is not modifying the <see cref="HashSet{T}"/>. This is deemed sufficient in for the relevant circumstances.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, HashSet<IMethodInformation>> s_explicitInterfaceImplementations =
        new ConcurrentDictionary<Type, HashSet<IMethodInformation>>();

    private readonly Type _type;
    private readonly bool _includeBaseProperties;
    private readonly bool _includeMixinProperties;
    private readonly IMemberInformationNameResolver _nameResolver;
    private readonly IPersistentMixinFinder _persistentMixinFinder;
    private readonly IPropertyMetadataProvider _propertyMetadataProvider;
    private readonly Lazy<HashSet<IMethodInformation>> _explicitInterfaceImplementations;

    protected PropertyFinderBase (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull("persistentMixinFinder", persistentMixinFinder);
      ArgumentUtility.CheckNotNull("propertyMetadataProvider", propertyMetadataProvider);

      _type = type;
      _includeBaseProperties = includeBaseProperties;
      _includeMixinProperties = includeMixinProperties;
      _nameResolver = nameResolver;
      _persistentMixinFinder = persistentMixinFinder;
      _propertyMetadataProvider = propertyMetadataProvider;
      // C# compiler 7.2 does not provide caching for delegate but calls are only during application start so no caching is needed.
      _explicitInterfaceImplementations = new Lazy<HashSet<IMethodInformation>>(
          () => s_explicitInterfaceImplementations.GetOrAdd(_type, GetExplicitInterfaceImplementations));
    }

    public Type Type
    {
      get { return _type; }
    }

    public bool IncludeBaseProperties
    {
      get { return _includeBaseProperties; }
    }

    public bool IncludeMixinProperties
    {
      get { return _includeMixinProperties; }
    }

    public IMemberInformationNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    protected IPropertyMetadataProvider PropertyMetadataProvider
    {
      get { return _propertyMetadataProvider; }
    }

    protected abstract PropertyFinderBase CreateNewFinder (
        Type type,
        bool includeBaseProperties,
        bool includeMixinProperties,
        IMemberInformationNameResolver nameResolver,
        IPersistentMixinFinder persistentMixinFinder,
        IPropertyMetadataProvider propertyMetadataProvider);

    public IPropertyInformation[] FindPropertyInfos ()
    {
      var propertyInfos = new List<IPropertyInformation>();

      if (_includeBaseProperties && _type.BaseType != typeof(DomainObject) && _type.BaseType != null)
      {
        // Use a new PropertyFinder of the same type as this one to get all properties from above this class. Mixins are not included for the base
        // classes; the mixin finder below will include the mixins for those classes anyway.
        var propertyFinder = CreateNewFinder(_type.BaseType, true, false, _nameResolver, _persistentMixinFinder, _propertyMetadataProvider);
        propertyInfos.AddRange(propertyFinder.FindPropertyInfos());
      }

      propertyInfos.AddRange(FindPropertyInfosDeclaredOnThisType());

      if (_includeMixinProperties)
      {
        Func<Type, bool, bool, PropertyFinderBase> propertyFinderFactory =
            (type, includeBaseProperties, includeMixinProperties) =>
                CreateNewFinder(
                    type,
                    includeBaseProperties,
                    includeMixinProperties,
                    _nameResolver,
                    _persistentMixinFinder,
                    _propertyMetadataProvider);
        // Base mixins are included only when base properties are included.
        var includeBaseMixins = _includeBaseProperties;
        var mixinPropertyFinder = new MixinPropertyFinder(propertyFinderFactory, _persistentMixinFinder, includeBaseMixins);
        propertyInfos.AddRange(mixinPropertyFinder.FindPropertyInfosOnMixins());
      }

      return propertyInfos.ToArray();
    }

    protected virtual bool FindPropertiesFilter (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      if (!propertyInfo.IsOriginalDeclaration())
        return false;

      if (IsUnmanagedProperty(propertyInfo))
        return false;

      if (IsUnmanagedExplictInterfaceImplementation(propertyInfo))
        return false;

      return true;
    }

    protected bool IsUnmanagedProperty (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      var storageClass = _propertyMetadataProvider.GetStorageClass(propertyInfo);
      if (storageClass == null)
        return false;

      return storageClass == StorageClass.None;
    }

    protected bool IsUnmanagedExplictInterfaceImplementation (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      bool isExplicitInterfaceImplementation = Array.Exists(
          propertyInfo.GetAccessors(true),
          accessor => _explicitInterfaceImplementations.Value.Contains(accessor));
      if (!isExplicitInterfaceImplementation)
        return false;

      var storageClass = _propertyMetadataProvider.GetStorageClass(propertyInfo);
      if (storageClass == null)
        return true;

      return storageClass == StorageClass.None;
    }

    public IEnumerable<IPropertyInformation> FindPropertyInfosDeclaredOnThisType ()
    {
      MemberInfo[] memberInfos = _type.FindMembers(
          MemberTypes.Property,
          PropertyBindingFlags | BindingFlags.DeclaredOnly,
          FindPropertiesFilter,
          null);

      var propertyInfos = Array.ConvertAll(memberInfos, input => PropertyInfoAdapter.Create((PropertyInfo)input));

      return propertyInfos;
    }

    private bool FindPropertiesFilter (MemberInfo member, object? filterCriteria)
    {
      return FindPropertiesFilter(PropertyInfoAdapter.Create((PropertyInfo)member));
    }

    private static HashSet<IMethodInformation> GetExplicitInterfaceImplementations (Type type)
    {
      var explicitInterfaceImplementationSet = new HashSet<MethodInfo>();

      foreach (Type interfaceType in type.GetInterfaces())
      {
        InterfaceMapping interfaceMapping = type.GetInterfaceMap(interfaceType);
        MethodInfo[] explicitInterfaceImplementations = Array.FindAll(
            interfaceMapping.TargetMethods,
            targetMethod => targetMethod.IsSpecialName && !targetMethod.IsPublic);
        explicitInterfaceImplementationSet.UnionWith(explicitInterfaceImplementations);
      }

      return new HashSet<IMethodInformation>(explicitInterfaceImplementationSet.Select(MethodInfoAdapter.Create));
    }
  }
}
