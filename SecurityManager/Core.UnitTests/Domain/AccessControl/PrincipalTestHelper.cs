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