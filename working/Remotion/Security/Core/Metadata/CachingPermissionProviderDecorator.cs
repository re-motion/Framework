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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  /// <summary>
  /// Implements a cache for <see cref="IPermissionProvider"/> implementations.
  /// </summary>
  [ImplementationFor (typeof (IPermissionProvider), RegistrationType = RegistrationType.Decorator)]
  public class CachingPermissionProviderDecorator : IPermissionProvider
  {
    private struct CacheKey : IEquatable<CacheKey>
    {
      public readonly Type Type;
      public readonly IMethodInformation MethodInformation;

      public CacheKey (Type type, IMethodInformation methodInformation)
      {
        ArgumentUtility.DebugCheckNotNull ("type", type);
        ArgumentUtility.DebugCheckNotNull ("methodInformation", methodInformation);

        Type = type;
        MethodInformation = methodInformation;
      }

      public override int GetHashCode ()
      {
        return MethodInformation.GetHashCode();
      }

      public bool Equals (CacheKey other)
      {
        return Type == other.Type
               && MethodInformation.Equals (other.MethodInformation);
      }
    }

    private static readonly Enum[] s_emptyPermissions = new Enum[0];
    private readonly IPermissionProvider _innerPermissionProvider;
    private readonly ICache<CacheKey, IReadOnlyList<Enum>> _cache = CacheFactory.CreateWithLocking<CacheKey, IReadOnlyList<Enum>>();
    private readonly Func<CacheKey, IReadOnlyList<Enum>> _cacheValueFactory;

    public CachingPermissionProviderDecorator (IPermissionProvider innerPermissionProvider)
    {
      ArgumentUtility.CheckNotNull ("innerPermissionProvider", innerPermissionProvider);

      _innerPermissionProvider = innerPermissionProvider;
      _cacheValueFactory = GetRequiredMethodPermissions;
    }

    public IPermissionProvider InnerPermissionProvider
    {
      get { return _innerPermissionProvider; }
    }

    public IReadOnlyList<Enum> GetRequiredMethodPermissions (Type type, IMethodInformation methodInformation)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("methodInformation", methodInformation);

      // Optimization to prevent cache polution
      if (methodInformation.IsNull)
        return s_emptyPermissions;

      var cacheKey = new CacheKey (type, methodInformation);
      return _cache.GetOrCreateValue (cacheKey, _cacheValueFactory);
    }

    private IReadOnlyList<Enum> GetRequiredMethodPermissions (CacheKey key)
    {
      var accessTypes = _innerPermissionProvider.GetRequiredMethodPermissions (key.Type, key.MethodInformation);
      if (accessTypes.Count == 0)
        return s_emptyPermissions;

      // Make a read-only copy to keep in the cache.
      return new ReadOnlyCollection<Enum> (accessTypes.ToArray());
    }
  }
}