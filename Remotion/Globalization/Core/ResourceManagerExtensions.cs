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
using System.Collections.Specialized;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Defines extension methods for the <see cref="IResourceManager"/> interface.
  /// </summary>
  public static class ResourceManagerExtensions
  {
    /// <summary>
    ///   Returns all string resources inside the resource manager.
    /// </summary>
    /// <returns>
    ///   A collection of string pairs, the key being the resource's ID, the value being the string.
    /// </returns>
    [NotNull]
    public static NameValueCollection GetAllStrings ([NotNull] this IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      return resourceManager.GetAllStrings (null);
    }


    /// <summary>
    ///   Gets the value of the specified String resource.
    /// </summary>
    /// <param name="resourceManager">The <see cref="IResourceManager"/> that is used for the resource lookup.</param>
    /// <param name="id">The ID of the resource to get. </param>
    /// <returns>
    ///   The value of the resource. If no match is possible, the identifier is returned.
    /// </returns>
    [NotNull]
    public static string GetString ([NotNull] this IResourceManager resourceManager, [NotNull] string id)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("id", id);

      string value;
      if (resourceManager.TryGetString (id, out value))
        return value;

      return id;
    }

    /// <summary>
    ///   Gets the value of the specified String resource.
    /// </summary>
    /// <param name="resourceManager">The <see cref="IResourceManager"/> that is used for the resource lookup.</param>
    /// <param name="id">The ID of the resource to get. </param>
    /// <returns>
    ///   The value of the resource. If no match is possible, <see langword="null"/> is returned.
    /// </returns>
    [CanBeNull]
    public static string GetStringOrDefault ([NotNull] this IResourceManager resourceManager, [NotNull] string id)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("id", id);

      string value;
      if (resourceManager.TryGetString (id, out value))
        return value;

      return default(string);
    }

    /// <summary>
    ///   Gets the value of the specified string resource. The resource is identified by
    ///   concatenating type and value name.
    /// </summary>
    /// <remarks> See <see cref="ResourceIdentifiersAttribute.GetResourceIdentifier"/> for resource identifier syntax. </remarks>
    /// <returns>
    ///   The value of the resource. If no match is possible, the identifier is returned.
    /// </returns>
    [NotNull]
    public static string GetString ([NotNull] this IResourceManager resourceManager, [NotNull] Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      return resourceManager.GetString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>
    ///   Gets the value of the specified string resource. The resource is identified by
    ///   concatenating type and value name.
    /// </summary>
    /// <remarks> See <see cref="ResourceIdentifiersAttribute.GetResourceIdentifier"/> for resource identifier syntax. </remarks>
    /// <returns>
    ///   The value of the resource. If no match is possible, null is returned.
    /// </returns>
    [CanBeNull]
    public static string GetStringOrDefault ([NotNull] this IResourceManager resourceManager, [NotNull] Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      return resourceManager.GetStringOrDefault (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="resourceManager">The <see cref="IResourceManager"/> that is used for the resource lookup.</param>
    /// <param name="enumValue">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    public static bool ContainsString ([NotNull] this IResourceManager resourceManager, [NotNull] Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      return resourceManager.ContainsString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="resourceManager">The <see cref="IResourceManager"/> that is used for the resource lookup.</param>
    /// <param name="id">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    public static bool ContainsString ([NotNull] this IResourceManager resourceManager, [NotNull] string id)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("id", id);

      string value;
      return resourceManager.TryGetString (id, out value);
    }

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="resourceManager">The <see cref="IResourceManager"/> that is used for the resource lookup.</param>
    /// <param name="enumValue">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    [Obsolete ("Use ContainsString(...) instead. (Version 1.13.223.0)")]
    public static bool ContainsResource ([NotNull] this IResourceManager resourceManager, [NotNull] Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      return resourceManager.ContainsString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>Tests whether the <see cref="IResourceManager"/> contains the specified resource.</summary>
    /// <param name="resourceManager">The <see cref="IResourceManager"/> that is used for the resource lookup.</param>
    /// <param name="id">The ID of the resource to get. </param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    [Obsolete ("Use ContainsString(...) instead. (Version 1.13.223.0)")]
    public static bool ContainsResource ([NotNull] this IResourceManager resourceManager, [NotNull] string id)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("id", id);

      return resourceManager.ContainsString (id);
    }
  }
}