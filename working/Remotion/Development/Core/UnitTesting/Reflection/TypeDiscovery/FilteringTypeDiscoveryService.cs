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
using System.Collections;
using System.ComponentModel.Design;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Reflection.TypeDiscovery
{
  public class FilteringTypeDiscoveryService : ITypeDiscoveryService
  {
    private readonly ITypeDiscoveryService _decoratedTypeDiscoveryService;
    private readonly Func<Type, bool> _filter;

    public static FilteringTypeDiscoveryService CreateFromNamespaceWhitelist (ITypeDiscoveryService decoratedTypeDiscoveryService, params string[] whitelistedNamespaces)
    {
      ArgumentUtility.CheckNotNull ("decoratedTypeDiscoveryService", decoratedTypeDiscoveryService);
      ArgumentUtility.CheckNotNullOrEmpty ("whitelistedNamespaces", whitelistedNamespaces);

      return new FilteringTypeDiscoveryService (
          decoratedTypeDiscoveryService,
          type => whitelistedNamespaces.Any (whitelistedNamespace => GetNamespaceSafe (type).StartsWith (whitelistedNamespace)));
    }

    public static FilteringTypeDiscoveryService CreateFromNamespaceBlacklist (ITypeDiscoveryService decoratedTypeDiscoveryService, params string[] blacklistedNamespaces)
    {
      ArgumentUtility.CheckNotNull ("decoratedTypeDiscoveryService", decoratedTypeDiscoveryService);
      ArgumentUtility.CheckNotNullOrEmpty ("blacklistedNamespaces", blacklistedNamespaces);

      return new FilteringTypeDiscoveryService (
          decoratedTypeDiscoveryService,
          type => !blacklistedNamespaces.Any (blacklistedNamespace => GetNamespaceSafe (type).StartsWith (blacklistedNamespace)));
    }

    public FilteringTypeDiscoveryService (ITypeDiscoveryService decoratedTypeDiscoveryService, Func<Type, bool> filter)
    {
      ArgumentUtility.CheckNotNull ("decoratedTypeDiscoveryService", decoratedTypeDiscoveryService);
      ArgumentUtility.CheckNotNull ("filter", filter);

      _decoratedTypeDiscoveryService = decoratedTypeDiscoveryService;
      _filter = filter;
    }

    public ICollection GetTypes (Type baseType, bool excludeGlobalTypes)
    {
      var collection = _decoratedTypeDiscoveryService.GetTypes (baseType, excludeGlobalTypes);
      return collection.Cast<Type>().Where (_filter).ToList();
    }

    private static string GetNamespaceSafe (Type type)
    {
      return type.Namespace ?? "";
    }
  }
}