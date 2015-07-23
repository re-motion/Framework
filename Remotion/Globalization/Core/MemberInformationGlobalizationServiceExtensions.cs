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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Provides extension methods for retrieving for retrieving the human-readable localized representation of an <see cref="ITypeInformation"/>
  /// or an <see cref="IPropertyInformation"/> using the <see cref="IMemberInformationGlobalizationService"/>.
  /// </summary>
  public static class MemberInformationGlobalizationServiceExtensions
  {
    /// <summary>
    ///   Gets the human-readable type name of the specified reflection object, 
    ///   using the <paramref name="typeInformation"/>'s <see cref="IMemberInformation.Name"/> as fallback.
    /// </summary>
    /// <param name="memberInformationGlobalizationService">
    ///   The <see cref="IMemberInformationGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformation">
    ///   The <see cref="ITypeInformation"/> that defines the type name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    ///   The human-readable localized representation of the <paramref name="typeInformation"/> 
    ///   or the <paramref name="typeInformation"/>'s <see cref="IMemberInformation.Name"/> if no resource could be found.
    /// </returns>
    [NotNull]
    public static string GetTypeDisplayName (
        [NotNull] this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        [NotNull] ITypeInformation typeInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      if (memberInformationGlobalizationService.TryGetTypeDisplayName (typeInformation, typeInformationForResourceResolution, out resourceValue))
        return resourceValue;

      return typeInformation.Name;
    }

    /// <summary>
    ///   Gets the human-readable type name of the specified reflection object, using <see langword="null" /> as fallback.
    /// </summary>
    /// <param name="memberInformationGlobalizationService">
    ///   The <see cref="IMemberInformationGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformation">
    ///   The <see cref="ITypeInformation"/> that defines the type name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    ///   The human-readable localized representation of the <paramref name="typeInformation"/>
    ///   or <see langword="null" /> if no resource could be found.
    /// </returns>
    [CanBeNull]
    public static string GetTypeDisplayNameOrDefault (
        [NotNull] this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        [NotNull] ITypeInformation typeInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      if (memberInformationGlobalizationService.TryGetTypeDisplayName (typeInformation, typeInformationForResourceResolution, out resourceValue))
        return resourceValue;

      return null;
    }

    /// <summary>
    ///   Checks whether a human-readable type name of the spefified reflection object exists.
    /// </summary>
    /// <param name="memberInformationGlobalizationService">
    ///   The <see cref="IMemberInformationGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformation">
    ///   The <see cref="ITypeInformation"/> that defines the type name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a resource could be found.</returns>
    public static bool ContainsTypeDisplayName (
        [NotNull] this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        [NotNull] ITypeInformation typeInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      return memberInformationGlobalizationService.TryGetTypeDisplayName (typeInformation, typeInformationForResourceResolution, out resourceValue);
    }

    /// <summary>
    ///   Tries to get the human-readable property name of the spefified reflection object,
    ///   using the <paramref name="propertyInformation"/>'s <see cref="IMemberInformation.Name"/> as fallback.
    /// </summary>
    /// <param name="memberInformationGlobalizationService">
    ///   The <see cref="IMemberInformationGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="propertyInformation">
    ///   The <see cref="IPropertyInformation"/> that defines the property name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    ///   The human-readable localized representation of the <paramref name="propertyInformation"/> 
    ///   or the <paramref name="propertyInformation"/>'s <see cref="IMemberInformation.Name"/> if no resource could be found.
    /// </returns>
    [NotNull]
    public static string GetPropertyDisplayName (
        [NotNull] this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        [NotNull] IPropertyInformation propertyInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      if (memberInformationGlobalizationService.TryGetPropertyDisplayName (
          propertyInformation,
          typeInformationForResourceResolution,
          out resourceValue))
        return resourceValue;

      return propertyInformation.Name;
    }

    /// <summary>
    ///   Tries to get the human-readable property name of the spefified reflection object, using <see langword="null" /> as fallback.
    /// </summary>
    /// <param name="memberInformationGlobalizationService">
    ///   The <see cref="IMemberInformationGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="propertyInformation">
    ///   The <see cref="IPropertyInformation"/> that defines the property name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    ///   The human-readable localized representation of the <paramref name="propertyInformation"/> 
    ///   or <see langword="null" /> if no resource could be found.
    /// </returns>
    [CanBeNull]
    public static string GetPropertyDisplayNameOrDefault (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      if (memberInformationGlobalizationService.TryGetPropertyDisplayName (
          propertyInformation,
          typeInformationForResourceResolution,
          out resourceValue))
        return resourceValue;

      return null;
    }

    /// <summary>
    ///   Checks whether a human-readable property name of the spefified reflection object exists.
    /// </summary>
    /// <param name="memberInformationGlobalizationService">
    ///   The <see cref="IMemberInformationGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="propertyInformation">
    ///   The <see cref="IPropertyInformation"/> that defines the property name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="typeInformationForResourceResolution">
    ///   The <see cref="ITypeInformation"/> that should be used for the resource resolution. Must not be <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a resource could be found.</returns>
    public static bool ContainsPropertyDisplayName (
        [NotNull] this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        [NotNull] IPropertyInformation propertyInformation,
        [NotNull] ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      return memberInformationGlobalizationService.TryGetPropertyDisplayName (
          propertyInformation,
          typeInformationForResourceResolution,
          out resourceValue);
    }
  }
}