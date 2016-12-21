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
      RegisterQueryFactory ("SubstitutedRole", FindPossibleSubstitutedRoles);
    }

    private IQueryable<IBusinessObject> FindPossibleSubstitutedRoles (
        Substitution substitution,
        IBusinessObjectReferenceProperty property,
        TenantConstraint tenantConstraint,
        DisplayNameConstraint displayNameConstraint)
    {
      ArgumentUtility.CheckNotNull ("substitution", substitution);

      if (substitution.SubstitutedUser == null)
        return Enumerable.Empty<IBusinessObject>().AsQueryable();
      return substitution.SubstitutedUser.Roles.Cast<IBusinessObject>().AsQueryable();
    }
  }
}