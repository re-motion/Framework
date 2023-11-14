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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Default implementation of the <see cref="IMemberInformationNameResolver"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(IMemberInformationNameResolver), Lifetime = LifetimeKind.Singleton)]
  public class ReflectionBasedMemberInformationNameResolver : IMemberInformationNameResolver
  {
    private readonly ConcurrentDictionary<IPropertyInformation, string> s_propertyNameCache = new ConcurrentDictionary<IPropertyInformation, string>();

    private readonly ConcurrentDictionary<ITypeInformation, string> s_typeNameCache = new ConcurrentDictionary<ITypeInformation, string>();

    private readonly ConcurrentDictionary<Enum, string> s_enumCache = new ConcurrentDictionary<Enum, string>();

    private readonly Func<ITypeInformation, string> _getTypeNameInternalFunc;
    private readonly Func<Enum, string> _getEnumNameInternalFunc;
    private readonly Func<IPropertyInformation, string> _getPropertyNameInternalFunc;

    public ReflectionBasedMemberInformationNameResolver ()
    {
      // Optimized for memory allocations
      _getTypeNameInternalFunc = GetTypeNameInternal;
      _getEnumNameInternalFunc = GetEnumNameInternal;
      _getPropertyNameInternalFunc = GetPropertyNameInternal;
    }

    /// <summary>
    /// Returns the mapping name for the given <paramref name="propertyInformation"/>.
    /// </summary>
    /// <param name="propertyInformation">The property whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="propertyInformation"/> as used internally by the mapping.</returns>
    public string GetPropertyName (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull("propertyInformation", propertyInformation);

      return s_propertyNameCache.GetOrAdd(propertyInformation, _getPropertyNameInternalFunc);
    }

    /// <summary>
    /// Returns the mapping name for the given <paramref name="typeInformation"/>.
    /// </summary>
    /// <param name="typeInformation">The type whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="typeInformation"/> as used internally by the mapping.</returns>
    public string GetTypeName (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull("typeInformation", typeInformation);

      return s_typeNameCache.GetOrAdd(typeInformation, _getTypeNameInternalFunc);
    }

    public string GetEnumName (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull("enumValue", enumValue);

      return s_enumCache.GetOrAdd(enumValue, _getEnumNameInternalFunc);
    }

    private string GetPropertyNameInternal (IPropertyInformation propertyInformation)
    {
      //TODO RM-7432: GetOriginalDeclaringType may return null
      return GetTypeName(propertyInformation.GetOriginalDeclaringType()!) + "." + propertyInformation.Name;
    }

    private string GetTypeNameInternal (ITypeInformation typeInformation)
    {
      //TODO RM-7432: FullName may return null
      if (typeInformation.IsGenericType && !typeInformation.IsGenericTypeDefinition)
        typeInformation = typeInformation.GetGenericTypeDefinition();

      return typeInformation.FullName!;
    }

    private string GetEnumNameInternal (Enum enumValue)
    {
      return enumValue.GetType().GetFullNameChecked() + "." + enumValue;
    }
  }
}
