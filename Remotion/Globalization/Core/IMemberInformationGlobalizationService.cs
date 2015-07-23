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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines an interface for retrieving the human-readable localized representation of the reflection object.
  /// </summary>
  /// <seealso cref="ResourceManagerBasedMemberInformationGlobalizationService"/>
  /// <threadsafety static="true" instance="true" />
  public interface IMemberInformationGlobalizationService
  {
    /// <summary>
    ///   Tries to get the human-readable type name of the specified reflection object.
    /// </summary>
    /// <param name="typeInformation">
    ///   The <see cref="ITypeInformation"/> that defines the type name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <param name="result">
    ///   The human-readable localized representation of the <paramref name="typeInformation"/> 
    ///   or <see langword="null" /> if no resource could be found.
    /// </param>
    /// <returns><see langword="true" /> if a resource could be found.</returns>
    bool TryGetTypeDisplayName (
        [NotNull] ITypeInformation typeInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution,
        out string result);

    /// <summary>
    ///   Tries to get the human-readable property name of the spefified reflection object.
    /// </summary>
    /// <param name="propertyInformation">
    ///   The <see cref="IPropertyInformation"/> that defines the property name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <param name="result">
    ///   The human-readable localized representation of the <paramref name="propertyInformation"/> 
    ///   or <see langword="null" /> if no resource could be found.
    /// </param>
    /// <returns><see langword="true" /> if a resource could be found.</returns>
    bool TryGetPropertyDisplayName (
        [NotNull] IPropertyInformation propertyInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution,
        out string result);
  }
}