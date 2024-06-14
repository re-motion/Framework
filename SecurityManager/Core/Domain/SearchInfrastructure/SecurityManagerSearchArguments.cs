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
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// <see cref="ISearchAvailableObjectsArguments"/> implementation used for properties using auto-complete-style value selection.
  /// </summary>
  public class SecurityManagerSearchArguments : ISearchAvailableObjectsArguments
  {
    private readonly DisplayNameConstraint? _displayNameConstraint;
    private readonly TenantConstraint? _tenantConstraint;
    private readonly ResultSizeConstraint? _resultSizeConstraint;

    public SecurityManagerSearchArguments (
        TenantConstraint? tenantConstraint, ResultSizeConstraint? resultSizeConstraint, DisplayNameConstraint? displayNameConstraint)
    {
      _tenantConstraint = tenantConstraint;
      _resultSizeConstraint = resultSizeConstraint;
      _displayNameConstraint = displayNameConstraint;
    }

    /// <summary>
    /// When set, gets the constraint used for filtering based on the object's <see cref="Tenant"/>.
    /// </summary>
    public TenantConstraint? TenantConstraint
    {
      get { return _tenantConstraint; }
    }

    /// <summary>
    /// When set, gets the constraint used for filtering based on the object's display name.
    /// </summary>
    public DisplayNameConstraint? DisplayNameConstraint
    {
      get { return _displayNameConstraint; }
    }

    /// <summary>
    /// When set, gets the constraint used for constraining the number of the returned objects.
    /// </summary>
    public ResultSizeConstraint? ResultSizeConstraint
    {
      get { return _resultSizeConstraint; }
    }
  }
}
