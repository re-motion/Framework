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
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides utility functionality for extensible enums.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public static class ExtensibleEnumUtility
  {
    private static readonly TypeAdapter s_extensibleEnumInterfaceType = TypeAdapter.Create (typeof (IExtensibleEnum));
    private static readonly TypeAdapter s_extensibleEnumGenericBaseType = TypeAdapter.Create (typeof (ExtensibleEnum<>));

    /// <summary>
    /// Determines whether the specified type is an <see cref="ExtensibleEnum{T}"/> type.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified type is an extensible enum type; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null" />.</exception>
    /// <remarks>
    /// For performance reasons, this method only checks if the <paramref name="type"/> implements <see cref="IExtensibleEnum"/>, it does not
    /// check whether the type is derived from <see cref="ExtensibleEnum{T}"/>. The <see cref="GetDefinition"/> method, however, will throw
    /// an exception when used with a type not derived from <see cref="ExtensibleEnum{T}"/>.
    /// </remarks>
    public static bool IsExtensibleEnumType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return IsExtensibleEnumType (TypeAdapter.Create (type));
    }

    /// <summary>
    /// Determines whether the specified type is an <see cref="ExtensibleEnum{T}"/> type.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    ///   <see langword="true"/> if the specified type is an extensible enum type; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is <see langword="null" />.</exception>
    /// <remarks>
    /// For performance reasons, this method only checks if the <paramref name="type"/> implements <see cref="IExtensibleEnum"/>, it does not
    /// check whether the type is derived from <see cref="ExtensibleEnum{T}"/>. The <see cref="GetDefinition"/> method, however, will throw
    /// an exception when used with a type not derived from <see cref="ExtensibleEnum{T}"/>.
    /// </remarks>
    public static bool IsExtensibleEnumType (ITypeInformation type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return s_extensibleEnumInterfaceType.IsAssignableFrom (type)
             && !type.Equals (s_extensibleEnumGenericBaseType)
             && !(type.IsGenericType && type.GetGenericTypeDefinition().Equals (s_extensibleEnumGenericBaseType))
             && !type.Equals (s_extensibleEnumInterfaceType);
    }

    /// <summary>
    /// Gets the <see cref="IExtensibleEnumDefinition"/> for the given <paramref name="extensibleEnumType"/>.
    /// </summary>
    /// <param name="extensibleEnumType">The extensible enum type to get a <see cref="IExtensibleEnumDefinition"/> for.</param>
    /// <returns>An instance of <see cref="IExtensibleEnumDefinition"/> describing the enum type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="extensibleEnumType"/> parameter is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">The <paramref name="extensibleEnumType"/> is not derived from <see cref="ExtensibleEnum{T}"/>.</exception>
    public static IExtensibleEnumDefinition GetDefinition (Type extensibleEnumType)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);
      return SafeServiceLocator.Current.GetInstance<ExtensibleEnumDefinitionCache>().GetDefinition (extensibleEnumType);
    }
  }
}