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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for properties referencing the <see cref="AbstractRoleDefinition"/> type.
  /// </summary>
  /// <remarks>
  /// The service can be applied to any <see cref="AbstractRoleDefinition"/>-typed property of a <see cref="BaseSecurityManagerObject"/> 
  /// via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class AbstractRoleDefinitionPropertyTypeSearchService : SecurityManagerPropertyTypeBasedSearchServiceBase<AbstractRoleDefinition>
  {
    protected override IQueryable<IBusinessObject> CreateQuery (
        BaseSecurityManagerObject referencingObject,
        IBusinessObjectReferenceProperty property,
        TenantConstraint tenantConstraint,
        DisplayNameConstraint displayNameConstraint)
    {
      return AbstractRoleDefinition.FindAll().AsEnumerable().Apply (displayNameConstraint).Cast<IBusinessObject>().AsQueryable();
    }
  }
}