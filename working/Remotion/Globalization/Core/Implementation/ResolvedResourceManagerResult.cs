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
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Holds resource manager information cached by <see cref="ResourceManagerResolver"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class ResolvedResourceManagerResult : INullObject
  {
    public static readonly ResolvedResourceManagerResult Null = new ResolvedResourceManagerResult (
        NullResourceManager.Instance,
        NullResourceManager.Instance,
        NullResourceManager.Instance);

    [NotNull]
    public static ResolvedResourceManagerResult Create (
        [NotNull] IResourceManager definedResourceManager,
        [NotNull] IResourceManager inheritedResourceManger)
    {
      ArgumentUtility.CheckNotNull ("definedResourceManager", definedResourceManager);
      ArgumentUtility.CheckNotNull ("inheritedResourceManger", inheritedResourceManger);

      var combinedResourceManager = CombineResourceManagers (definedResourceManager, inheritedResourceManger);
      if (combinedResourceManager.IsNull)
        return Null;
      return new ResolvedResourceManagerResult (combinedResourceManager, definedResourceManager, inheritedResourceManger);
    }

    private readonly IResourceManager _resourceManager;
    private readonly IResourceManager _definedResourceManager;
    private readonly IResourceManager _inheritedResourceManger;

    private ResolvedResourceManagerResult (
        IResourceManager resourceManager,
        IResourceManager definedResourceManager,
        IResourceManager inheritedResourceManger)
    {
      _resourceManager = resourceManager;
      _definedResourceManager = definedResourceManager;
      _inheritedResourceManger = inheritedResourceManger;
    }

    [NotNull]
    public IResourceManager ResourceManager
    {
      get { return _resourceManager; }
    }

    [NotNull]
    public IResourceManager DefinedResourceManager
    {
      get { return _definedResourceManager; }
    }

    [NotNull]
    public IResourceManager InheritedResourceManager
    {
      get { return _inheritedResourceManger; }
    }

    public bool IsNull
    {
      get { return _resourceManager.IsNull; }
    }

    private static IResourceManager CombineResourceManagers (IResourceManager definedResourceManager, IResourceManager inheritedResourceManager)
    {
      if (definedResourceManager.IsNull && inheritedResourceManager.IsNull)
        return NullResourceManager.Instance;
      if (definedResourceManager.IsNull)
        return inheritedResourceManager;
      if (inheritedResourceManager.IsNull)
        return definedResourceManager;
      return ResourceManagerSet.Create (definedResourceManager, inheritedResourceManager);
    }
  }
}