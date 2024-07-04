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
#if !NETFRAMEWORK
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Remotion.Utilities;

namespace Remotion.Web.Resources;

/// <summary>
/// Extension methods for <see cref="RemotionStaticFilesCacheKeyRemovalRewriteRule"/>.
/// </summary>
public static class CachingApplicationBuilderExtensions
{
  /// <summary>
  /// Enables static file serving with caching support for the current request path.
  /// Replaces the call to <see cref="StaticFileExtensions.UseStaticFiles(Microsoft.AspNetCore.Builder.IApplicationBuilder)"/>.
  /// </summary>
  /// <remarks>
  /// Requires <see cref="ResourcePathBuilderBasedStaticResourceCacheKeyProvider"/> to be useful.
  /// </remarks>
  public static IApplicationBuilder UseRemotionStaticFiles (this IApplicationBuilder builder, StaticFileOptions options, StaticFileCachingOptions cachingOptions)
  {
    ArgumentUtility.CheckNotNull(nameof(builder), builder);
    ArgumentUtility.CheckNotNull(nameof(options), options);
    ArgumentUtility.CheckNotNull(nameof(cachingOptions), cachingOptions);

    var rewriteOptions = new RewriteOptions()
        .Add(new RemotionStaticFilesCacheKeyRemovalRewriteRule(options.RequestPath, stopProcessing: true));

    // Manually register the RewriteMiddleware to prevent auto-route discovery shenanigans
    // Otherwise, the static file middleware might be skipped because an endpoint is already assigned
    builder.UseMiddleware<RewriteMiddleware>(Options.Create(rewriteOptions));

    var originalOnPrepareResponse = options.OnPrepareResponse;
    options.OnPrepareResponse = context =>
    {
      if (RemotionStaticFilesCacheKeyRemovalRewriteRule.TryGetRemovedCacheKey(context.Context, out _))
        context.Context.Response.Headers.CacheControl = cachingOptions.GetCacheControlHeader();

      originalOnPrepareResponse(context);
    };

    builder.UseStaticFiles(options);

    return builder;
  }
}
#endif
