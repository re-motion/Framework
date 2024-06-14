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
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.SecurityProvider.Implementation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.SecurityProvider
{
  /// <summary>
  /// 2nd-level cache
  /// </summary>
  [ImplementationFor(typeof(ISecurityProvider), Position = Position, RegistrationType = RegistrationType.Decorator)]
  public class RevisionBasedCachingSecurityProviderDecorator : ISecurityProvider
  {
    //TODO RM-5521: test

    public const int Position = 0;

    private readonly ISecurityProvider _innerSecurityProvider;
    private readonly IUserRevisionProvider _userRevisionProvider;
    private readonly SecurityContextCache _securityContextCache;
    private Func<ISecurityPrincipal, AccessTypeCache>? _securityContextCacheValueFactory;

    public RevisionBasedCachingSecurityProviderDecorator (
        ISecurityProvider innerSecurityProvider,
        IDomainRevisionProvider revisionProvider,
        IUserRevisionProvider userRevisionProvider)
    {
      ArgumentUtility.CheckNotNull("innerSecurityProvider", innerSecurityProvider);
      ArgumentUtility.CheckNotNull("revisionProvider", revisionProvider);
      ArgumentUtility.CheckNotNull("userRevisionProvider", userRevisionProvider);

      _innerSecurityProvider = innerSecurityProvider;
      _securityContextCache = new SecurityContextCache(revisionProvider);
      _userRevisionProvider = userRevisionProvider;
    }

    public ISecurityProvider InnerSecurityProvider
    {
      get { return _innerSecurityProvider; }
    }

    public bool IsNull
    {
      get { return _innerSecurityProvider.IsNull; }
    }

    public AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("principal", principal);

      if (principal.IsNull)
        return _innerSecurityProvider.GetAccess(context, principal);

      Assertion.IsNotNull(principal.User, "SecurityPrincipal.User must not be null unless SecurityPrincipal is defined as a null object.");

      // Optimized for memory allocations
      if (_securityContextCacheValueFactory == null)
      {
        _securityContextCacheValueFactory = key =>
        {
          Assertion.DebugAssert(key.IsNull == false, "key.IsNull == false");
          Assertion.DebugIsNotNull(key.User, "key.User != null when key.IsNull == false");
          return new AccessTypeCache(_userRevisionProvider, key.User);
        };
      }

      var accessTypeCache = _securityContextCache.Items.GetOrCreateValue(principal, _securityContextCacheValueFactory);

      if (accessTypeCache.Items.TryGetValue(context, out var result))
        return result;

      // Split to prevent closure being created during the TryGetValue-operation
      return GetOrCreateAccessTypesFromCache(accessTypeCache.Items, context, principal);
    }

    private AccessType[] GetOrCreateAccessTypesFromCache (
        ICache<ISecurityContext, AccessType[]> accessTypeCache,
        ISecurityContext context,
        ISecurityPrincipal principal)
    {
      return accessTypeCache.GetOrCreateValue(context, key => _innerSecurityProvider.GetAccess(key, principal));
    }
  }
}
