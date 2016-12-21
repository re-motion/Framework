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
using Remotion.FunctionalProgramming;
using Remotion.Globalization.Implementation;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Utilities.ReSharperAnnotations;

namespace Remotion.Globalization.Mixins.Obsolete
{
  /// <summary>
  /// Implementation for the <see cref="MixedMultiLingualResources"/> class.
  /// </summary>
  [Obsolete]
  [ReflectionAPI]
  [ImplementationFor (typeof (MixedMultiLingualResources.IImplementation), Position = 1, Lifetime = LifetimeKind.Singleton)]
  internal class MixedMultiLingualResourcesImplementation : MixedMultiLingualResources.IImplementation
  {
    private static readonly DoubleCheckedLockingContainer<IResourceManagerResolver> s_resolver =
        new DoubleCheckedLockingContainer<IResourceManagerResolver> (() => SafeServiceLocator.Current.GetInstance<IResourceManagerResolver>());

    private static readonly DoubleCheckedLockingContainer<MixinGlobalizationService> s_mixinGlobalizationService =
        new DoubleCheckedLockingContainer<MixinGlobalizationService> (() => new MixinGlobalizationService (s_resolver.Value));

    public IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      var targetType = MixinTypeUtility.GetUnderlyingTargetType (objectType);
      var result = s_resolver.Value.Resolve (targetType);
      var mixinResourceManager = s_mixinGlobalizationService.Value.GetResourceManager (TypeAdapter.Create (targetType));

      if (includeHierarchy)
      {
        if (result.IsNull && mixinResourceManager.IsNull)
          throw new ResourceException (string.Format ("Type {0} and its base classes do not define a resource attribute.", objectType.FullName));
        return ResourceManagerSet.Create (result.DefinedResourceManager, mixinResourceManager, result.InheritedResourceManager);
      }

      if (result.IsNull && mixinResourceManager.IsNull)
        throw new ResourceException (string.Format ("Type {0} and its base classes do not define a resource attribute.", objectType.FullName));

      if (result.DefinedResourceManager.IsNull && !mixinResourceManager.IsNull)
        return mixinResourceManager;

      if (result.DefinedResourceManager.IsNull) // we already know there is a resource defined on a base-type so no additional checks are required.
        return MultiLingualResources.GetResourceManager (targetType.BaseType, false);

      return ResourceManagerSet.Create (result.DefinedResourceManager, mixinResourceManager);
    }

    public IResourceManager GetResourceManager (Type objectType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);

      return GetResourceManager (objectType, false);
    }

    public string GetResourceText (Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);

      var resourceManager = GetResourceManager (objectTypeToGetResourceFor, false);
      var text = resourceManager.GetString (name);
      if (text == name)
        return String.Empty;
      return text;
    }

    public bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
    {
      try
      {
        var resourceManager = GetResourceManager (objectTypeToGetResourceFor, false);
        string text = resourceManager.GetString (name);
        return (text != name);
      }
      catch
      {
        return false;
      }
    }

    public bool ExistsResource (Type objectTypeToGetResourceFor)
    {
      try
      {
        return !GetResourceManager (objectTypeToGetResourceFor, false).IsNull;
      }
      catch (ResourceException)
      {
        return false;
      }
    }
  }
}