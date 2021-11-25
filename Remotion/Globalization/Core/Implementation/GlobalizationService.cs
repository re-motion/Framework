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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves and caches <see cref="IResourceManager"/>s for types.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof(IGlobalizationService), Position = 0, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public sealed class GlobalizationService : IGlobalizationService
  {
    private readonly IResourceManagerResolver _resourceManagerResolver;

    private readonly ConcurrentDictionary<ITypeInformation, IResourceManager> _resourceManagerCache = 
        new ConcurrentDictionary<ITypeInformation, IResourceManager>();

    private readonly Func<ITypeInformation, IResourceManager> _getResourceManagerImplementationFunc;

    public GlobalizationService (IResourceManagerResolver resourceManagerResolver)
    {
      ArgumentUtility.CheckNotNull("resourceManagerResolver", resourceManagerResolver);

      _resourceManagerResolver = resourceManagerResolver;

      // Optimized for memory allocations
      _getResourceManagerImplementationFunc = GetResourceManagerImplementation;
    }

    public IResourceManager GetResourceManager (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull("typeInformation", typeInformation);

      return _resourceManagerCache.GetOrAdd(typeInformation, _getResourceManagerImplementationFunc);
    }

    [NotNull]
    private IResourceManager GetResourceManagerImplementation (ITypeInformation typeInformation)
    {
      var runtimeType = typeInformation.AsRuntimeType();
      if (runtimeType == null)
        return NullResourceManager.Instance;

      var result = _resourceManagerResolver.Resolve(runtimeType);
      return result.ResourceManager;
    }
  }
}