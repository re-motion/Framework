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
using System.Collections.ObjectModel;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Reflection;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides a non-generic interface for the <see cref="ExtensibleEnumDefinition{T}"/> class.
  /// </summary>
  /// <remarks>
  /// Do not implement this interface yourself, use it only when working with extensible enums in a reflective context, e.g. via the 
  /// <see cref="ExtensibleEnumDefinitionCache"/> class.
  /// </remarks>
  public interface IExtensibleEnumDefinition
  {
    /// <summary>
    /// Gets the extensible enum type described by this <see cref="IExtensibleEnumDefinition"/>.
    /// </summary>
    Type GetEnumType ();

    /// <summary>
    /// Determines whether the extensible enum type defines a value with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id to be retrieved.</param>
    /// <returns>
    /// 	<see langword="true" /> if a value with the specified ID is defined; otherwise, <see langword="false" />.
    /// </returns>
    bool IsDefined (string id);

    /// <summary>
    /// Determines whether the given value is a valid instance of the extensible enum type described by this <see cref="IExtensibleEnumDefinition"/>.
    /// This includes checking whether the value's <see cref="IExtensibleEnum.ID"/> is defined.
    /// </summary>
    /// <param name="value">The value to be checked.</param>
    /// <returns>
    /// 	<see langword="true"/> if the value is defined; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsDefined (IExtensibleEnum value);

    /// <summary>
    /// Gets <see cref="IExtensibleEnumInfo"/> objects describing the values defined by the extensible enum type.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> holding the <see cref="IExtensibleEnumInfo"/> objects describing the values for the 
    /// extensible enum type.</returns>
    /// <remarks>
    /// By default, the values are retrieved by scanning all types found by <see cref="ContextAwareTypeUtility.GetTypeDiscoveryService"/>
    /// and discovering the extension methods defining values via <see cref="ExtensibleEnumValueDiscoveryService"/>.
    /// </remarks>
    ReadOnlyCollection<IExtensibleEnumInfo> GetValueInfos ();

    /// <summary>
    /// Gets an <see cref="IExtensibleEnumInfo"/> object describing the enum value identified by <paramref name="id"/>, throwing an exception if the 
    /// value cannot be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <returns>An <see cref="IExtensibleEnumInfo"/> describing the enum value identified by <paramref name="id"/>.</returns>
    /// <exception cref="KeyNotFoundException">No enum value with the given <paramref name="id"/> exists.</exception>
    IExtensibleEnumInfo GetValueInfoByID (string id);

    /// <summary>
    /// Gets an <see cref="IExtensibleEnumInfo"/> object describing the enum value identified by <paramref name="id"/>, returning a boolean value 
    /// indicating whether such a value could be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <param name="value">The <see cref="IExtensibleEnumInfo"/> describing the enum value identified by <paramref name="id"/>, or 
    /// <see langword="null" /> if no such value exists.</param>
    /// <returns>
    /// <see langword="true" /> if a value with the given <paramref name="id"/> could be found; <see langword="false" /> otherwise.
    /// </returns>
    bool TryGetValueInfoByID (string id, out IExtensibleEnumInfo value);

    /// <summary>
    /// Gets the custom attributes defined by the types declaring the extension methods defining the enum values.
    /// </summary>
    /// <param name="attributeType">The attribute type to look for. Attributes inheriting from this type are also returned.</param>
    /// <returns>The custom attributes defined by the types declaring the extension methods defining the enum values.</returns>
    object[] GetCustomAttributes (Type attributeType);

    /// <summary>
    /// Gets the custom attributes defined by the types declaring the extension methods defining the enum values.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type to look for. Attributes inheriting from this type are also returned.</typeparam>
    /// <returns>The custom attributes defined by the types declaring the extension methods defining the enum values.</returns>
    TAttribute[] GetCustomAttributes<TAttribute> () where TAttribute : class;
  }
}
