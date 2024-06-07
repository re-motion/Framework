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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  public abstract class SecurityManagerPrincipalTestBase : DomainTest
  {
    protected SecurityManagerPrincipal CreateSecurityManagerPrincipal (
        Tenant tenant,
        User user,
        Role[] roles,
        Substitution substitution,
        User substitutedUser = null,
        Role[] substitutedRoles = null)
    {
      ArgumentUtility.CheckNotNull("tenant", tenant);
      ArgumentUtility.CheckNotNull("user", user);

      return new SecurityManagerPrincipal(
          tenant.GetHandle(),
          user.GetHandle(),
          roles == null ? null : roles.Select(r => r.GetHandle()).ToArray(),
          substitution.GetSafeHandle(),
          substitutedUser.GetSafeHandle(),
          substitutedRoles == null ? null : substitutedRoles.Select(r => r.GetHandle()).ToArray());
    }

    protected void IncrementDomainRevision ()
    {
      ClientTransaction.Current.QueryManager.GetScalar(Revision.GetIncrementRevisionQuery(new RevisionKey()));
      SafeServiceLocator.Current.GetInstance<IDomainRevisionProvider>().InvalidateRevision(new RevisionKey());
    }

    protected void IncrementUserRevision (string userName)
    {
      ClientTransaction.Current.QueryManager.GetScalar(Revision.GetIncrementRevisionQuery(new UserRevisionKey(userName)));
      SafeServiceLocator.Current.GetInstance<IUserRevisionProvider>().InvalidateRevision(new UserRevisionKey(userName));
    }
  }
}
