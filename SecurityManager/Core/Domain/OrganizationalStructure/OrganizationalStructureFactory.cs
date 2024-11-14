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
