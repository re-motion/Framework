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
using JetBrains.Annotations;

namespace Remotion.Reflection
{
  /// <summary>
  /// Defines an API for retrieving the a fully-qualified name for reflection objects.
  /// </summary>
  /// <seealso cref="ReflectionBasedMemberInformationNameResolver"/>
  /// <threadsafety static="true" instance="true"/>
  public interface IMemberInformationNameResolver
  {
    /// <summary>
    /// Returns the mapping name for the given <paramref name="propertyInformation"/>.
    /// </summary>
    /// <param name="propertyInformation">The property whose name should be retrieved. Must not be <see langword="null" />.</param>
    /// <returns>The name of the given <paramref name="propertyInformation"/> as used internally by type-member lookups.</returns>
    [NotNull]
    string GetPropertyName ([NotNull]IPropertyInformation propertyInformation);

    /// <summary>
    /// Returns the mapping name for the given <paramref name="typeInformation"/>.
    /// </summary>
    /// <param name="typeInformation">The type whose name should be retrieved. Must not be <see langword="null" />.</param>
    /// <returns>The name of the given <paramref name="typeInformation"/> as used internally  by type-member lookups.</returns>
    [NotNull]
    string GetTypeName ([NotNull]ITypeInformation typeInformation);

    /// <summary>
    /// Returns the mapping name for the given <paramref name="enumValue"/>.
    /// </summary>
    /// <param name="enumValue">The enum value whose name should be retrieved. Must not be <see langword="null" />.</param>
    /// <returns>The name of the given <paramref name="enumValue"/> as used internally  by type-member lookups.</returns>
    [NotNull]
    string GetEnumName ([NotNull]Enum enumValue);
  }
}
