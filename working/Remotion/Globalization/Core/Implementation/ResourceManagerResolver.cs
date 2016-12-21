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
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Default implementation of the <see cref="IResourceManagerResolver"/>. 
  /// Resource managers are resolved based on the <see cref="IResourcesAttribute"/> applied to the type.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IResourceManagerResolver), Lifetime = LifetimeKind.Singleton)]
  public sealed class ResourceManagerResolver : IResourceManagerResolver
  {
    private readonly LockingCacheDecorator<Type, ResolvedResourceManagerResult> _resourceManagerWrappersCache =
        CacheFactory.CreateWithLocking<Type, ResolvedResourceManagerResult>();

    private readonly IResourceManagerFactory _resourceManagerFactory;

    public ResourceManagerResolver (IResourceManagerFactory resourceManagerFactory)
    {
      ArgumentUtility.CheckNotNull ("resourceManagerFactory", resourceManagerFactory);

      _resourceManagerFactory = resourceManagerFactory;
    }

    public ResolvedResourceManagerResult Resolve (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return GetResolvedResourceManagerFromCache (type);
    }

    private ResolvedResourceManagerResult GetResolvedResourceManagerFromCache (Type type)
    {
      if (type == null)
        return ResolvedResourceManagerResult.Null;

      return _resourceManagerWrappersCache.GetOrCreateValue (type, CreateResolvedResourceManagerResult);
    }

    private ResolvedResourceManagerResult CreateResolvedResourceManagerResult (Type type)
    {
      var definedResourceManager = _resourceManagerFactory.CreateResourceManager (type);
      var inheritedResourceManager = GetResolvedResourceManagerFromCache (type.BaseType).ResourceManager;
      return ResolvedResourceManagerResult.Create (definedResourceManager, inheritedResourceManager);
    }
  }
}