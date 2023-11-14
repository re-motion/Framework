// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
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
