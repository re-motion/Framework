using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  public static class PrincipalTestHelper
  {
    public static Principal Create (Tenant tenant, User user, IEnumerable<Role> roles)
    {
      ArgumentUtility.CheckNotNull ("tenant", tenant);
      ArgumentUtility.CheckNotNull ("roles", roles);

      return new Principal (
          tenant.GetHandle(),
          user.GetSafeHandle(),
          roles.Select (r => new PrincipalRole (r.Position.GetHandle(), r.Group.GetHandle())));
    }
  }
}