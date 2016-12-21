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
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.GlobalAccessTypeCache.Implementation;
using Remotion.SecurityManager.SecurityProvider.Implementation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.SecurityProvider
{
  /// <summary>
  /// 2nd-level cache
  /// </summary>
  [ImplementationFor (typeof (ISecurityProvider), Position = Position, RegistrationType = RegistrationType.Decorator)]
  public class RevisionBasedCachingSecurityProviderDecorator : ISecurityProvider
  {
    //TODO RM-5521: test

    public const int Position = 0;

    private readonly ISecurityProvider _innerSecurityProvider;
    private readonly IUserRevisionProvider _userRevisionProvider;
    private readonly SecurityContextCache _securityContextCache;

    public RevisionBasedCachingSecurityProviderDecorator (
        ISecurityProvider innerSecurityProvider,
        IDomainRevisionProvider revisionProvider,
        IUserRevisionProvider userRevisionProvider)
    {
      ArgumentUtility.CheckNotNull ("innerSecurityProvider", innerSecurityProvider);
      ArgumentUtility.CheckNotNull ("revisionProvider", revisionProvider);
      ArgumentUtility.CheckNotNull ("userRevisionProvider", userRevisionProvider);

      _innerSecurityProvider = innerSecurityProvider;
      _securityContextCache = new SecurityContextCache (revisionProvider);
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
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("principal", principal);

      var accessTypeCache = _securityContextCache.Items.GetOrCreateValue (principal, key => new AccessTypeCache (_userRevisionProvider, key.User));

      return accessTypeCache.Items.GetOrCreateValue (context, key => _innerSecurityProvider.GetAccess (key, principal));
    }
  }
}