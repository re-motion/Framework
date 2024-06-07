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
using System.Linq;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/>for properties of the <see cref="Substitution"/> type.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The service is applied to the <see cref="Substitution.SubstitutedRole"/> property via the
  /// <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </para>
  /// <para>
  /// The service expected search arguments of type <see cref="SecurityManagerSearchArguments"/>,
  /// <see cref="DefaultSearchArguments"/>, or <see langword="null" />.
  /// </para>
  /// </remarks>
  public sealed class SubstitutionPropertiesSearchService : SecurityManagerPropertyBasedSearchServiceBase<Substitution>
  {
    public SubstitutionPropertiesSearchService ()
    {
      RegisterQueryFactory("SubstitutedRole", FindPossibleSubstitutedRoles);
    }

    private IQueryable<IBusinessObject> FindPossibleSubstitutedRoles (
        Substitution substitution,
        IBusinessObjectReferenceProperty property,
        TenantConstraint? tenantConstraint,
        DisplayNameConstraint? displayNameConstraint)
    {
      ArgumentUtility.CheckNotNull("substitution", substitution);

      if (substitution.SubstitutedUser == null)
        return Enumerable.Empty<IBusinessObject>().AsQueryable();
      return substitution.SubstitutedUser.Roles.Cast<IBusinessObject>().AsQueryable();
    }
  }
}
