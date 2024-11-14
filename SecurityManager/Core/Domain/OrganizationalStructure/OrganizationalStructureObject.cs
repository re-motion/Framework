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
using Remotion.Collections.Caching;
using Remotion.Data.DomainObjects.Security;
using Remotion.Globalization;
using Remotion.Security;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [PermanentGuid("8DBA42FE-ECD9-4b10-8F79-48E7A1119414")]
  [MultiLingualResources("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.OrganizationalStructureObject")]
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
