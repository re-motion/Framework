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
using Remotion.Data.DomainObjects.Security;
using Remotion.Globalization;
using Remotion.Security;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [PermanentGuid("8DBA42FE-ECD9-4b10-8F79-48E7A1119414")]
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.OrganizationalStructureObject")]
  [Serializable]
  public abstract class OrganizationalStructureObject : BaseSecurityManagerObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    private IObjectSecurityStrategy? _securityStrategy;

    protected OrganizationalStructureObject ()
    {
    }

    protected virtual string? GetOwningTenant ()
    {
      return null;
    }

    protected virtual string? GetOwner ()
    {
      return null;
    }

    protected virtual string? GetOwningGroup ()
    {
      return null;
    }

    protected virtual IDictionary<string, Enum> GetStates ()
    {
      return new Dictionary<string, Enum>();
    }

    protected virtual IList<Enum> GetAbstractRoles ()
    {
      return new List<Enum>();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      using (SecurityFreeSection.Activate())
      {
        return SecurityContext.Create(GetPublicDomainObjectType(), GetOwner(), GetOwningGroup(), GetOwningTenant(), GetStates(), GetAbstractRoles());
      }
    }

    bool IDomainObjectSecurityContextFactory.IsInvalid
    {
      get { return State.IsInvalid; }
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State.IsNewInHierarchy; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State.IsDeleted; }
    }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      if (_securityStrategy == null)
        _securityStrategy = CreateSecurityStrategy();

      return _securityStrategy;
    }

    protected virtual IObjectSecurityStrategy CreateSecurityStrategy ()
    {
      return new DomainObjectSecurityStrategyDecorator(
          ObjectSecurityStrategy.Create(this, InvalidationToken.Create()),
          this,
          RequiredSecurityForStates.None);
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }
  }
}
