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
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [ImplementationFor(typeof(IOrganizationalStructureFactory), Lifetime = LifetimeKind.Singleton)]
  public class OrganizationalStructureFactory : IOrganizationalStructureFactory
  {
    public virtual Tenant CreateTenant ()
    {
      return Tenant.NewObject();
    }

    public virtual Group CreateGroup ()
    {
      return Group.NewObject();
    }

    public virtual User CreateUser ()
    {
      return User.NewObject();
    }

    public virtual Position CreatePosition ()
    {
      return Position.NewObject();
    }

    public virtual Type GetTenantType ()
    {
      return typeof(Tenant);
    }

    public virtual Type GetGroupType ()
    {
      return typeof(Group);
    }

    public virtual Type GetUserType ()
    {
      return typeof(User);
    }

    public virtual Type GetPositionType ()
    {
      return typeof(Position);
    }
  }
}
