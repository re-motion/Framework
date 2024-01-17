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
using System.Collections.Concurrent;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Resources
{
  /// <summary>
  /// Base class for implementation of the <see cref="IResourcePathBuilder"/> interface.
  /// </summary>
  public abstract class ResourcePathBuilderBase : IResourcePathBuilder
  {
    private static readonly ConcurrentDictionary<Assembly, AssemblyName> s_assemblyNameCache = new ConcurrentDictionary<Assembly, AssemblyName>();

    private readonly IStaticResourceCacheKeyProvider _staticResourceCacheKeyProvider;

    protected ResourcePathBuilderBase (IStaticResourceCacheKeyProvider staticResourceCacheKeyProvider)
    {
      ArgumentUtility.CheckNotNull("staticResourceCacheKeyProvider", staticResourceCacheKeyProvider);

      _staticResourceCacheKeyProvider = staticResourceCacheKeyProvider;
    }

    protected abstract string GetResourceRoot ();

    protected abstract string BuildPath (string[] completePath);

    public string BuildAbsolutePath (Assembly assembly, params string[] assemblyRelativePathParts)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);
      ArgumentUtility.CheckNotNull("assemblyRelativePathParts", assemblyRelativePathParts);

      string root = GetResourceRoot();
      // C# compiler 7.2 already provides caching for anonymous method.
      string assemblyName = s_assemblyNameCache.GetOrAdd(assembly, key => key.GetName()).GetNameChecked();

      var cacheKey = _staticResourceCacheKeyProvider.GetStaticResourceCacheKey();
      string[] completePath = cacheKey != null
          ? [root, $"cache_{cacheKey}", assemblyName, ..assemblyRelativePathParts]
          : [root, assemblyName, ..assemblyRelativePathParts];

      return BuildPath(completePath);
    }
  }
}
