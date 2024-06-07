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
using System.Runtime.Serialization;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="NullSecurityManagerPrincipal"/> type is the <see cref="INullObject"/> implementation 
  /// of the <see cref="ISecurityManagerPrincipal"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class NullSecurityManagerPrincipal
      : ISecurityManagerPrincipal,
#pragma warning disable SYSLIB0050
          IObjectReference
#pragma warning restore SYSLIB0050
  {
    private static readonly TenantProxy[] s_emptyTenantProxies = new TenantProxy[0];
    private static readonly SubstitutionProxy[] s_emptySubstitutionProxies = new SubstitutionProxy[0];
    private static readonly NullSecurityPrincipal s_nullSecurityPrincipal = new NullSecurityPrincipal();

    internal NullSecurityManagerPrincipal ()
    {
    }

    public TenantProxy? Tenant
    {
      get { return null; }
    }

    public UserProxy? User
    {
      get { return null; }
    }

    public IReadOnlyList<RoleProxy>? Roles
    {
      get { return null; }
    }

    public SubstitutionProxy? Substitution
    {
      get { return null; }
    }

    public ISecurityManagerPrincipal GetRefreshedInstance ()
    {
      return this;
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      return s_emptyTenantProxies;
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      return s_emptySubstitutionProxies;
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      return s_nullSecurityPrincipal;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return SecurityManagerPrincipal.Null;
    }
  }
}
