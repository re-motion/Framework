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
using JetBrains.Annotations;
using Remotion.Collections.Caching;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Default implementation of the <see cref="IObjectSecurityStrategy"/> interface. A new instance of the <see cref="ObjectSecurityStrategy"/> type
  /// is typically created and held for each <see cref="ISecurableObject"/> implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="ObjectSecurityStrategy"/> supports the use of an <see cref="ISecurityContextFactory"/> 
  /// for creating the relevant <see cref="ISecurityContext"/> and caches the result returned by the <see cref="ISecurityProvider"/>.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [Serializable]
  public sealed class ObjectSecurityStrategy : IObjectSecurityStrategy
  {
    /// <summary>
    /// Instantiates <see cref="ObjectSecurityStrategy"/> with a cache based on the <paramref name="invalidationToken"/>.
    /// </summary>
    public static IObjectSecurityStrategy Create (
        [NotNull] ISecurityContextFactory securityContextFactory,
        [NotNull] InvalidationToken invalidationToken)
    {
      ArgumentUtility.CheckNotNull("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull("invalidationToken", invalidationToken);

      return new ObjectSecurityStrategy(securityContextFactory, CacheFactory.Create<ISecurityPrincipal, AccessType[]>(invalidationToken));
    }

    /// <summary>
    /// Instantiates <see cref="ObjectSecurityStrategy"/> with a custom <paramref name="cache"/> implementation.
    /// </summary>
    public static IObjectSecurityStrategy CreateWithCustomCache (
        [NotNull] ISecurityContextFactory securityContextFactory,
        [NotNull] ICache<ISecurityPrincipal, AccessType[]> cache)
    {
      ArgumentUtility.CheckNotNull("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull("cache", cache);

      return new ObjectSecurityStrategy(securityContextFactory, cache);
    }

    private readonly ISecurityContextFactory _securityContextFactory;
    private readonly ICache<ISecurityPrincipal, AccessType[]> _cache;

    private ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory, ICache<ISecurityPrincipal, AccessType[]> cache)
    {
      ArgumentUtility.DebugCheckNotNull("securityContextFactory", securityContextFactory);
      ArgumentUtility.DebugCheckNotNull("cache", cache);

      _securityContextFactory = securityContextFactory;
      _cache = cache;
    }

    public bool HasAccess (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull("securityProvider", securityProvider);
      ArgumentUtility.DebugCheckNotNull("principal", principal);
      ArgumentUtility.CheckNotNull("requiredAccessTypes", requiredAccessTypes);
      // Performance critical argument check. Can be refactored to ArgumentUtility.CheckNotNullOrEmpty once typed collection checks are supported.
      if (requiredAccessTypes.Count == 0)
        throw ArgumentUtility.CreateArgumentEmptyException("requiredAccessTypes");

      var actualAccessTypes = GetAccessTypesFromCache(securityProvider, principal);
      return requiredAccessTypes.IsSubsetOf(actualAccessTypes);
    }

    private AccessType[] GetAccessTypesFromCache (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      if (_cache.TryGetValue(principal, out var value))
        return value;

      // Split to prevent closure being created during the TryGetValue-operation
      return GetOrCreateAccessTypesFromCache(securityProvider, principal);
    }

    private AccessType[] GetOrCreateAccessTypesFromCache (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      return _cache.GetOrCreateValue(principal, key => GetAccessTypes(securityProvider, key));
    }

    private AccessType[] GetAccessTypes (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      // Explicit null-check since the public method does not perform this check in release-code
      ArgumentUtility.CheckNotNull("securityProvider", securityProvider);

      var context = CreateSecurityContext();

      var accessTypes = securityProvider.GetAccess(context, principal);
      Assertion.IsNotNull(accessTypes, "GetAccess evaluated and returned null.");

      return accessTypes;
    }

    private ISecurityContext CreateSecurityContext ()
    {
      using (SecurityFreeSection.Activate())
      {
        var context = _securityContextFactory.CreateSecurityContext();
        Assertion.IsNotNull(context, "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

        return context;
      }
    }
  }
}
