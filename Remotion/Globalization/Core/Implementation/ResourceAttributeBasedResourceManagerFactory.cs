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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Implements the <see cref="IResourceManagerFactory"/> interface for attributes that implement the <see cref="IResourcesAttribute"/>.
  /// </summary>
  /// <seealso cref="MultiLingualResourcesAttribute"/>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof(IResourceManagerFactory), Position = Position, RegistrationType = RegistrationType.Multiple)]
  public sealed class ResourceAttributeBasedResourceManagerFactory : IResourceManagerFactory
  {
    public const int Position = 19;

    private readonly ConcurrentDictionary<Tuple<Assembly, string>, ResourceManagerWrapper> _resourceManagersCache =
        new ConcurrentDictionary<Tuple<Assembly, string>, ResourceManagerWrapper>();

    public ResourceAttributeBasedResourceManagerFactory ()
    {
    }

    public IResourceManager CreateResourceManager (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var resourceAttributes = AttributeUtility.GetCustomAttributes<IResourcesAttribute>(type, false);
      var assembly = type.Assembly;
      var resourceManagers = resourceAttributes.Select(resourcesAttribute => GetResourceManagerFromCache(assembly, resourcesAttribute));
      return new ResourceManagerSet(resourceManagers);
    }

    private ResourceManagerWrapper GetResourceManagerFromCache (Assembly assembly, IResourcesAttribute resourcesAttribute)
    {
      // C# compiler 7.2 does not provide caching for delegate but when creating the ResourceManager there is already a significant amount of GC pressure so the delegate creation does not matter
      return _resourceManagersCache.GetOrAdd(
          Tuple.Create(resourcesAttribute.ResourceAssembly ?? assembly, resourcesAttribute.BaseName),
          GetResourceManager);
    }

    private ResourceManagerWrapper GetResourceManager (Tuple<Assembly, string> key)
    {
      var resourceManager = new ResourceManager(key.Item2, key.Item1);
      var neutralSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
      if (neutralSet == null)
      {
        throw new MissingManifestResourceException(
            string.Format(
                "Could not find any resources appropriate for the neutral culture. Make sure '{1}.resources' was correctly embedded into assembly '{0}' at compile time.",
                key.Item1.GetName().GetNameSafe(),
                key.Item2));
      }

      var availableCultures = GetAvailableCultures(key.Item1);

      return new ResourceManagerWrapper(resourceManager, availableCultures);
    }

    private IReadOnlyList<CultureInfo> GetAvailableCultures (Assembly assembly)
    {
      var availableResourceLanguagesAttribute = assembly.GetCustomAttribute<AvailableResourcesLanguagesAttribute>();
      if (availableResourceLanguagesAttribute == null)
        return CultureInfo.GetCultures(CultureTypes.AllCultures);

      var result = availableResourceLanguagesAttribute.CultureNames.Select(CultureInfo.GetCultureInfo);

      var neutralResourcesLanguageAttribute = assembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>();
      if (neutralResourcesLanguageAttribute != null)
        result = result.Concat(CultureInfo.GetCultureInfo(neutralResourcesLanguageAttribute.CultureName));

      return result.ToArray();
    }
  }
}
