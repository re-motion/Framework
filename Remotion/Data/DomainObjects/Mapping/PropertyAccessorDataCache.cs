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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Holds all <see cref="PropertyAccessorData"/> object for a <see cref="TypeDefinition"/>, providing fast access via full property name or
  /// declaring type and short (.NET) property name.
  /// </summary>
  public class PropertyAccessorDataCache
  {
    private readonly TypeDefinition _typeDefinition;
    private readonly Lazy<IReadOnlyDictionary<string, PropertyAccessorData>> _cachedAccessorData;
    private readonly ConcurrentDictionary<IPropertyInformation, PropertyAccessorData?> _cachedAccessorDataByMember;
    private readonly Func<IPropertyInformation, PropertyAccessorData?> _resolvePropertyAccessorDataWithoutCacheFunc;

    public PropertyAccessorDataCache (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      _typeDefinition = typeDefinition;
      _cachedAccessorData = new Lazy<IReadOnlyDictionary<string, PropertyAccessorData>>(
          BuildAccessorDataDictionary,
          LazyThreadSafetyMode.ExecutionAndPublication);
      _cachedAccessorDataByMember = new ConcurrentDictionary<IPropertyInformation, PropertyAccessorData?>();

      // Optimized for memory allocations
      _resolvePropertyAccessorDataWithoutCacheFunc = ResolvePropertyAccessorDataWithoutCache;
    }

    [CanBeNull]
    public PropertyAccessorData? GetPropertyAccessorData ([NotNull] string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyIdentifier", propertyIdentifier);

      _cachedAccessorData.Value.TryGetValue(propertyIdentifier, out var result);
      return result;
    }

    [CanBeNull]
    public PropertyAccessorData? GetPropertyAccessorData ([NotNull] Type domainObjectType, [NotNull] string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      string propertyIdentifier = GetIdentifierFromTypeAndShortName(domainObjectType, shortPropertyName);
      return GetPropertyAccessorData(propertyIdentifier);
    }

    [CanBeNull]
    public PropertyAccessorData? ResolvePropertyAccessorData<TDomainObject, TResult> (
        [NotNull] Expression<Func<TDomainObject, TResult>> propertyAccessExpression)
        where TDomainObject : IDomainObject
    {
      ArgumentUtility.CheckNotNull("propertyAccessExpression", propertyAccessExpression);

      PropertyInfo propertyInfo;
      try
      {
        propertyInfo = MemberInfoFromExpressionUtility.GetProperty(propertyAccessExpression);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException("The expression must identify a property.", "propertyAccessExpression", ex);
      }

      return ResolvePropertyAccessorData(PropertyInfoAdapter.Create(propertyInfo));
    }

    [CanBeNull]
    public PropertyAccessorData? ResolvePropertyAccessorData ([NotNull] IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull("propertyInformation", propertyInformation);

      return _cachedAccessorDataByMember.GetOrAdd(propertyInformation, _resolvePropertyAccessorDataWithoutCacheFunc);
    }

    [CanBeNull]
    private PropertyAccessorData? ResolvePropertyAccessorDataWithoutCache ([NotNull] IPropertyInformation propertyInformation)
    {
      return ReflectionBasedPropertyResolver.ResolveDefinition(propertyInformation, _typeDefinition, GetPropertyAccessorData);
    }

    [NotNull]
    public PropertyAccessorData GetMandatoryPropertyAccessorData ([NotNull] string propertyName)
    {
      // GetPropertyAccessorData already performs the null check
      ArgumentUtility.DebugCheckNotNullOrEmpty("propertyName", propertyName);

      var data = GetPropertyAccessorData(propertyName);
      if (data == null)
      {
        var message = string.Format(
            "The domain object type '{0}' does not have a mapping property named '{1}'.",
            _typeDefinition.Type,
            propertyName);
        throw new MappingException(message);
      }
      return data;
    }

    [NotNull]
    public PropertyAccessorData GetMandatoryPropertyAccessorData ([NotNull] Type domainObjectType, [NotNull] string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      var propertyName = GetIdentifierFromTypeAndShortName(domainObjectType, shortPropertyName);
      return GetMandatoryPropertyAccessorData(propertyName);
    }

    [NotNull]
    public PropertyAccessorData ResolveMandatoryPropertyAccessorData<TDomainObject, TResult> (
        [NotNull] Expression<Func<TDomainObject, TResult>> propertyAccessExpression)
        where TDomainObject : IDomainObject
    {
      //ResolvePropertyAccessorData already performs the null check
      ArgumentUtility.DebugCheckNotNull("propertyAccessExpression", propertyAccessExpression);

      var data = ResolvePropertyAccessorData(propertyAccessExpression);
      if (data == null)
      {
        var message = string.Format(
            "The domain object type '{0}' does not have a mapping property identified by expression '{1}'.",
            _typeDefinition.Type,
            propertyAccessExpression);
        throw new MappingException(message);
      }

      return data;
    }

    [CanBeNull]
    public PropertyAccessorData? FindPropertyAccessorData ([NotNull] Type typeToStartSearch, [NotNull] string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull("typeToStartSearch", typeToStartSearch);
      // The first loop already checks the shortPropertyName argument.
      ArgumentUtility.DebugCheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      Type? currentType = typeToStartSearch;
      PropertyAccessorData? propertyAccessorData = null;
      while (currentType != null && (propertyAccessorData = GetPropertyAccessorData(currentType, shortPropertyName)) == null)
      {
        if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
          currentType = currentType.GetGenericTypeDefinition();
        else
          currentType = currentType.BaseType;
      }
      return propertyAccessorData;
    }

    [NotNull]
    private string GetIdentifierFromTypeAndShortName ([NotNull] Type domainObjectType, [NotNull] string shortPropertyName)
    {
      return domainObjectType.GetFullNameChecked() + "." + shortPropertyName;
    }

    private Dictionary<string, PropertyAccessorData> BuildAccessorDataDictionary ()
    {
      var propertyDefinitions = _typeDefinition.GetPropertyDefinitions();
      var relationEndPointDefinitions = _typeDefinition.GetRelationEndPointDefinitions();

      var propertyDefinitionNames = from PropertyDefinition pd in propertyDefinitions
                                    select pd.PropertyName;
      var virtualRelationEndPointNames =
          from IRelationEndPointDefinition repd in relationEndPointDefinitions
          where repd.IsVirtual
          select (string)repd.PropertyName!;

      var allPropertyNames = propertyDefinitionNames.Concat(virtualRelationEndPointNames);
      var allPropertyAccessorData = allPropertyNames.Select(name => new PropertyAccessorData(_typeDefinition, name));
      return allPropertyAccessorData.ToDictionary(data => data.PropertyIdentifier);
    }
  }
}
