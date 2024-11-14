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
using Remotion.Collections.Caching;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityPrincipalRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(ISecurityPrincipalRepository), Lifetime = LifetimeKind.Singleton)]
  public sealed class SecurityPrincipalRepository : ISecurityPrincipalRepository
  {
    private readonly IUserRevisionProvider _revisionProvider;

    private readonly ICache<string, CachedUser> _cache = CacheFactory.CreateWithSynchronization<string, CachedUser>();
    private Func<string, CachedUser>? _userCacheValueFactory;

    public SecurityPrincipalRepository (IUserRevisionProvider revisionProvider)
    {
      ArgumentUtility.CheckNotNull("revisionProvider", revisionProvider);

      _revisionProvider = revisionProvider;
    }

    public User GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("userName", userName);

      // Optimized for memory allocations
      if (_userCacheValueFactory == null)
        _userCacheValueFactory = key => new CachedUser(_revisionProvider, key);

      var cacheItem = _cache.GetOrCreateValue(userName, _userCacheValueFactory);
      return cacheItem.GetValue();
    }
  }
}
