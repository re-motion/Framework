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
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Globalization.ExtensibleEnums
{
  /// <summary>
  /// Provides extension methods for retrieving for retrieving the human-readable localized representation of an <see cref="IExtensibleEnum"/> object
  /// using the <see cref="IExtensibleEnumGlobalizationService"/>.
  /// </summary>
  public static class ExtensibleEnumGlobalizationServiceExtensions
  {
    /// <summary>
    ///   Gets the human-readable extensible-enumeration name of the spefified reflection object,
    ///   using the <paramref name="value"/>'s name as fallback.
    /// </summary>
    /// <param name="extensibleEnumGlobalizationService">
    ///   The <see cref="IExtensibleEnumGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="value">
    ///   The <see cref="IExtensibleEnum"/> that defines the name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    ///   The human-readable localized representation of the <paramref name="value"/> 
    ///   or the <paramref name="value"/>'s <see cref="IExtensibleEnum.ValueName"/> if no resource could be found.
    /// </returns>
    [NotNull]
    public static string GetExtensibleEnumValueDisplayName (
        [NotNull] this IExtensibleEnumGlobalizationService extensibleEnumGlobalizationService,
        [NotNull] IExtensibleEnum value)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumGlobalizationService", extensibleEnumGlobalizationService);
      ArgumentUtility.CheckNotNull ("value", value);

      string result;
      if (extensibleEnumGlobalizationService.TryGetExtensibleEnumValueDisplayName (value, out result))
        return result;

      return value.ValueName;
    }

    /// <summary>
    ///   Gets the human-readable extensible-enumeration name of the spefified reflection object, using <see langword="null" /> as fallback.
    /// </summary>
    /// <param name="extensibleEnumGlobalizationService">
    ///   The <see cref="IExtensibleEnumGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="value">
    ///   The <see cref="IExtensibleEnum"/> that defines the name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    ///   The human-readable localized representation of the <paramref name="value"/> 
    ///   or <see langword="null" /> if no resource could be found.
    /// </returns>
    [CanBeNull]
    public static string GetExtensibleEnumValueDisplayNameOrDefault (
        [NotNull] this IExtensibleEnumGlobalizationService extensibleEnumGlobalizationService,
        [NotNull] IExtensibleEnum value)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumGlobalizationService", extensibleEnumGlobalizationService);
      ArgumentUtility.CheckNotNull ("value", value);

      string result;
      if (extensibleEnumGlobalizationService.TryGetExtensibleEnumValueDisplayName (value, out result))
        return result;

      return null;
    }

    /// <summary>
    ///   Checks whether a human-readable extensible-enumeration name of the spefified reflection object exists.
    /// </summary>
    /// <param name="extensibleEnumGlobalizationService">
    ///   The <see cref="IExtensibleEnumGlobalizationService"/> to use during the lookup. Must not be <see langword="null" />.
    /// </param>
    /// <param name="value">
    ///   The <see cref="IExtensibleEnum"/> that defines the name for the resource lookup. Must not be <see langword="null" />.
    /// </param>
    /// <returns><see langword="true" /> if a resource could be found.</returns>
    public static bool ContainsExtensibleEnumValueDisplayName (
        [NotNull] this IExtensibleEnumGlobalizationService extensibleEnumGlobalizationService,
        [NotNull] IExtensibleEnum value)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumGlobalizationService", extensibleEnumGlobalizationService);
      ArgumentUtility.CheckNotNull ("value", value);

      string result;
      return extensibleEnumGlobalizationService.TryGetExtensibleEnumValueDisplayName (value, out result);
    }
  }
}