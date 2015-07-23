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
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// The <see cref="Principal"/> type represents a <see cref="Tenant"/> object, a <see cref="User"/> object, 
  /// and one or more <see cref="Role"/> objects. Together, they specify the principal for which the permissions are evaluated.
  /// </summary>
  public class Principal : INullObject
  {
    public static readonly Principal Null = new Principal();

    private readonly bool _isNull;
    private readonly IDomainObjectHandle<Tenant> _tenant;
    private readonly IDomainObjectHandle<User> _user;
    private readonly ReadOnlyCollection<PrincipalRole> _roles;

    private Principal ()
    {
      _isNull = true;
      _roles = new ReadOnlyCollection<PrincipalRole> (new PrincipalRole[0]);
    }

    public Principal (
        [NotNull] IDomainObjectHandle<Tenant> tenant,
        [CanBeNull] IDomainObjectHandle<User> user,
        [NotNull] IEnumerable<PrincipalRole> roles)
    {
      ArgumentUtility.CheckNotNull ("tenant", tenant);
      ArgumentUtility.CheckNotNull ("roles", roles);

      _isNull = false;
      _tenant = tenant;
      _user = user;
      _roles = roles.ToList().AsReadOnly();
    }

    [CanBeNull]
    public IDomainObjectHandle<Tenant> Tenant
    {
      get { return _tenant; }
    }

    [CanBeNull]
    public IDomainObjectHandle<User> User
    {
      get { return _user; }
    }

    [NotNull]
    public ReadOnlyCollection<PrincipalRole> Roles
    {
      get { return _roles; }
    }

    public bool IsNull
    {
      get { return _isNull; }
    }
  }
}