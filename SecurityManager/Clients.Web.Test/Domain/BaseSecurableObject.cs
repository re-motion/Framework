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
using Remotion.Collections.Caching;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid("C9FC9EC0-9F41-4636-9A4C-4927A9B47E85")]
  public abstract class BaseSecurableObject : BaseObject, ISecurableObject, ISecurityContextFactory
  {
    private IObjectSecurityStrategy _objectSecurityStrategy;

    protected BaseSecurableObject ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      if (_objectSecurityStrategy == null)
        _objectSecurityStrategy = ObjectSecurityStrategy.Create(this, InvalidationToken.Create());

      return _objectSecurityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    public ISecurityContext CreateSecurityContext ()
    {
      return SecurityContext.Create(GetPublicDomainObjectType(), GetOwnerName(), GetOwnerGroupName(), GetOwnerTenantName(), GetStates(), GetAbstractRoles());
    }

    private string GetOwnerTenantName ()
    {
      Tenant tenant = GetOwnerTenant();
      if (tenant == null)
        return null;
      return tenant.Name;
    }

    private string GetOwnerGroupName ()
    {
      Group group = GetOwnerGroup();
      if (group == null)
        return null;
      return group.Name;
    }

    private string GetOwnerName ()
    {
      User user = GetOwner();
      if (user == null)
        return null;
      return user.UserName;
    }

    public abstract User GetOwner ();

    public abstract Group GetOwnerGroup ();

    public abstract Tenant GetOwnerTenant ();

    public virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum>();
    }

    public virtual ICollection<Enum> GetAbstractRoles ()
    {
      return new Enum[0];
    }
  }
}
