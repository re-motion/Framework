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
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// <see cref="ISearchAvailableObjectsArguments"/> implementation used for properties using auto-complete-style value selection.
  /// </summary>
  public class SecurityManagerSearchArguments : ISearchAvailableObjectsArguments
  {
    private readonly DisplayNameConstraint _displayNameConstraint;
    private readonly TenantConstraint _tenantConstraint;
    private readonly ResultSizeConstraint _resultSizeConstraint;

    public SecurityManagerSearchArguments (
        TenantConstraint tenantConstraint, ResultSizeConstraint resultSizeConstraint, DisplayNameConstraint displayNameConstraint)
    {
      _tenantConstraint = tenantConstraint;
      _resultSizeConstraint = resultSizeConstraint;
      _displayNameConstraint = displayNameConstraint;
    }

    /// <summary>
    /// When set, gets the constraint used for filtering based on the object's <see cref="Tenant"/>.
    /// </summary>
    public TenantConstraint TenantConstraint
    {
      get { return _tenantConstraint; }
    }

    /// <summary>
    /// When set, gets the constraint used for filtering based on the object's display name.
    /// </summary>
    public DisplayNameConstraint DisplayNameConstraint
    {
      get { return _displayNameConstraint; }
    }

    /// <summary>
    /// When set, gets the constraint used for constraining the number of the returned objects.
    /// </summary>
    public ResultSizeConstraint ResultSizeConstraint
    {
      get { return _resultSizeConstraint; }
    }
  }
}