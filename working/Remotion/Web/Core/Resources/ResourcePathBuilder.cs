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
using System.Linq;
using System.Text;
using System.Web;
using JetBrains.Annotations;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Configuration;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.Resources
{
  /// <summary>
  /// Default implementation of the <see cref="IResourcePathBuilder"/> interface. 
  /// Builds the resource path rooted to the application virtual directory.
  /// </summary>
  [ImplementationFor (typeof (IResourcePathBuilder), Lifetime = LifetimeKind.Singleton)]
  public class ResourcePathBuilder : ResourcePathBuilderBase
  {
    private readonly IHttpContextProvider _httpContextProvider;
    private readonly string _configuredResourceRoot;

    [UsedImplicitly]
    public ResourcePathBuilder (IHttpContextProvider httpContextProvider)
      : this (httpContextProvider, WebConfiguration.Current.Resources.Root)
    {
    }

    protected ResourcePathBuilder (IHttpContextProvider httpContextProvider, string configuredResourceRoot)
    {
      ArgumentUtility.CheckNotNull ("httpContextProvider", httpContextProvider);
      ArgumentUtility.CheckNotNull ("configuredResourceRoot", configuredResourceRoot);

      _httpContextProvider = httpContextProvider;
      _configuredResourceRoot = configuredResourceRoot;
    }

    protected override string BuildPath (string[] completePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("completePath", completePath);

      return completePath.Aggregate (CombineVirtualPaths);
    }

    protected override string GetResourceRoot ()
    {
      var applicationPath = GetApplicationPath();
      Assertion.IsTrue (VirtualPathUtility.IsAbsolute (applicationPath));

      return CombineVirtualPaths (applicationPath, _configuredResourceRoot);
    }

    private string CombineVirtualPaths (string left, string right)
    {
      return VirtualPathUtility.Combine (VirtualPathUtility.AppendTrailingSlash (left), right);
    }

    private string GetApplicationPath ()
    {
      var context = _httpContextProvider.GetCurrentHttpContext();
      return GetApplicationPathFromHttpContext (context);
    }

    private string GetApplicationPathFromHttpContext (HttpContextBase context)
    {
      var applicationPath = VirtualPathUtility.AppendTrailingSlash (context.Request.ApplicationPath) ?? "/";
      if (applicationPath == "/")
        return applicationPath;

      Assertion.IsNotNull (context.Request.Url, "context.Request.Url != null");
      var requestUrlAbsolutePath = VirtualPathUtility.AppendTrailingSlash (context.Request.Url.AbsolutePath);
      if (requestUrlAbsolutePath.StartsWith (applicationPath, StringComparison.OrdinalIgnoreCase))
        return requestUrlAbsolutePath.Remove (applicationPath.Length - 1);

      if (VirtualPathUtility.AppendTrailingSlash (context.Request.Url.LocalPath).StartsWith (applicationPath, StringComparison.OrdinalIgnoreCase))
      {
        var applicationPathPartsCount = applicationPath.Split (new[] { '/' }, StringSplitOptions.None).Length - 1;

        var calculatedApplicationPath = requestUrlAbsolutePath
            .Split (new[] { '/' }, StringSplitOptions.None)
            .Take (applicationPathPartsCount)
            .Aggregate (new StringBuilder (requestUrlAbsolutePath.Length), (sb, part) => sb.Append (part).Append ("/"))
            .ToString();

        Assertion.IsTrue (
            requestUrlAbsolutePath.StartsWith (calculatedApplicationPath),
            "Calculation of application path from request URL failed.\r\n"
            + "  Absolute path from request: {0}\r\n"
            + "  Calculated path: {1}\r\n"
            + "  Application path: {2}\r\n",
            requestUrlAbsolutePath,
            calculatedApplicationPath,
            applicationPath);

        return calculatedApplicationPath;
      }

      throw new InvalidOperationException (
          string.Format (
              "Cannot calculate the application path when the request URL does not start with the application path. "
              + "Possible reasons include the use of escape sequences in the path, e.g. when the application path contains whitespace.\r\n"
              + "  Absolute path from request: {0}\r\n"
              + "  Application path: {1}\r\n",
              requestUrlAbsolutePath,
              applicationPath));
    }
  }
}