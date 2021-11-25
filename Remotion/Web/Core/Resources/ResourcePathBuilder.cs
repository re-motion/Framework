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
using Remotion.Web.Utilities;
using VirtualPathUtility = System.Web.VirtualPathUtility;

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
      ArgumentUtility.CheckNotNull("httpContextProvider", httpContextProvider);
      ArgumentUtility.CheckNotNull("configuredResourceRoot", configuredResourceRoot);

      _httpContextProvider = httpContextProvider;
      _configuredResourceRoot = configuredResourceRoot;
    }

    protected override string BuildPath (string[] completePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("completePath", completePath);

      return completePath.Aggregate(CombineVirtualPaths);
    }

    protected override string GetResourceRoot ()
    {
      var applicationPath = GetApplicationPath();
      Assertion.IsTrue(VirtualPathUtility.IsAbsolute(applicationPath));

      return CombineVirtualPaths(applicationPath, _configuredResourceRoot);
    }

    private string CombineVirtualPaths (string left, string right)
    {
      return VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(left), right);
    }

    private string GetApplicationPath ()
    {
      var context = _httpContextProvider.GetCurrentHttpContext();
      return UrlUtility.ResolveUrlCaseSensitive(context, "~/");
    }
  }
}