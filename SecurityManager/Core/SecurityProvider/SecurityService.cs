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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager
{
  [ImplementationFor(typeof(ISecurityProvider), Lifetime = LifetimeKind.Singleton, Position = Position, RegistrationType = RegistrationType.Single)]
  public class SecurityService : ISecurityProvider
  {
    public const int Position = NullSecurityProvider.Position - 1;

    private static readonly ILog s_log = LogManager.GetLogger(typeof(SecurityService));

    private readonly IAccessControlListFinder _accessControlListFinder;
    private readonly ISecurityTokenBuilder _securityTokenBuilder;
    private readonly IAccessResolver _accessResolver;

    public SecurityService (
        IAccessControlListFinder accessControlListFinder,
        ISecurityTokenBuilder securityTokenBuilder,
        IAccessResolver accessResolver)
    {
      ArgumentUtility.CheckNotNull("accessControlListFinder", accessControlListFinder);
      ArgumentUtility.CheckNotNull("securityTokenBuilder", securityTokenBuilder);
      ArgumentUtility.CheckNotNull("accessResolver", accessResolver);

      _accessControlListFinder = accessControlListFinder;
      _securityTokenBuilder = securityTokenBuilder;
      _accessResolver = accessResolver;
    }

    public AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("principal", principal);

      using (SecurityFreeSection.Activate())
      {
        IDomainObjectHandle<AccessControlList>? acl;
        SecurityToken token;
        try
        {
          acl = _accessControlListFinder.Find(context);
          token = _securityTokenBuilder.CreateToken(principal, context);
        }
        catch (AccessControlException e)
        {
          s_log.Error("Error during evaluation of security query.", e);
          return new AccessType[0];
        }

        if (acl == null)
          return new AccessType[0];

        try
        {
          return _accessResolver.GetAccessTypes(acl, token);
        }
        catch (ObjectsNotFoundException e)
        {
          s_log.Error("Error during evaluation of security query.", e);
          return new AccessType[0];
        }
      }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
