﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Web.Globalization
{
  public static class ResourceManagerExtensions
  {
    /// <summary>
    ///   Gets the value of the specified String resource.
    /// </summary>
    /// <returns>
    ///   The value of the resource as <see cref="WebString"/>. If no match is possible, the identifier is returned.
    /// </returns>
    public static WebString GetWebString (this IResourceManager resourceManager, string id, WebStringType webStringType)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("id", id);

      var resourceString = resourceManager.GetString (id);

      return CreateWebStringOfType (webStringType, resourceString);
    }

    /// <summary>
    ///   Gets the value of the specified String resource.
    /// </summary>
    /// <returns>
    ///   The value of the resource as <see cref="WebString"/>. If no match is possible, <see langword="null"/> is returned.
    /// </returns>
    public static WebString? GetWebStringOrDefault (this IResourceManager resourceManager, string id, WebStringType webStringType)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("id", id);

      var resourceString = resourceManager.GetStringOrDefault (id);

      if (resourceString == null)
        return null;

      return CreateWebStringOfType (webStringType, resourceString);
    }

    /// <summary>
    ///   Gets the value of the specified string resource. The resource is identified by
    ///   concatenating type and value name.
    /// </summary>
    /// <remarks> See <see cref="ResourceIdentifiersAttribute.GetResourceIdentifier"/> for resource identifier syntax. </remarks>
    /// <returns>
    ///   The value of the resource as <see cref="WebString"/>. If no match is possible, the identifier is returned.
    /// </returns>
    public static WebString GetWebString (this IResourceManager resourceManager, Enum enumValue, WebStringType webStringType)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      var resourceString = resourceManager.GetString (enumValue);

      return CreateWebStringOfType (webStringType, resourceString);
    }

    /// <summary>
    ///   Gets the value of the specified string resource. The resource is identified by
    ///   concatenating type and value name.
    /// </summary>
    /// <remarks> See <see cref="ResourceIdentifiersAttribute.GetResourceIdentifier"/> for resource identifier syntax. </remarks>
    /// <returns>
    ///   The value of the resource as <see cref="WebString"/>. If no match is possible, null is returned.
    /// </returns>
    public static WebString? GetWebStringOrDefault (this IResourceManager resourceManager, Enum enumValue, WebStringType webStringType)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);

      var resourceString = resourceManager.GetStringOrDefault (enumValue);

      if (resourceString == null)
        return null;

      return CreateWebStringOfType (webStringType, resourceString);
    }

    private static WebString CreateWebStringOfType (WebStringType webStringType, string value) =>
        webStringType switch
        {
            WebStringType.PlainText => WebString.CreateFromText (value),
            WebStringType.Encoded => WebString.CreateFromHtml (value),
            _ => throw new ArgumentOutOfRangeException (nameof (webStringType)),
        };
  }
}
