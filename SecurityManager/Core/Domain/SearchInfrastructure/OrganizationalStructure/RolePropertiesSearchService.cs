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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for properties of the <see cref="Role"/> type.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The service is applied to the <see cref="Role.Position"/> property via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </para>
  /// <para>
  /// The service expected search arguments of type <see cref="RolePropertiesSearchArguments"/> or <see langword="null" />.
  /// </para>
  /// </remarks>
  public class RolePropertiesSearchService : ISearchAvailableObjectsService
  {
    public bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull("property", property);

      return property.Identifier == "Position";
    }

    public IBusinessObject[] Search (
        IBusinessObject? referencingObject,
        IBusinessObjectReferenceProperty property,
        ISearchAvailableObjectsArguments? searchArguments)
    {
      ArgumentUtility.CheckNotNull("property", property);
      var rolePropertiesSearchArguments = ArgumentUtility.CheckType<RolePropertiesSearchArguments>("searchArguments", searchArguments);

      if (!SupportsProperty(property))
      {
        throw new ArgumentException(
            string.Format("The property '{0}' is not supported by the '{1}' type.", property.Identifier, GetType().GetFullNameSafe()));
      }

      var positions = GetPositions(rolePropertiesSearchArguments);
      var filteredPositions = FilterByAccess(positions, SecurityManagerAccessTypes.AssignRole);
      return filteredPositions.ToArray();
    }

    private IQueryable<Position> GetPositions (RolePropertiesSearchArguments defaultSearchArguments)
    {
      var positions = Position.FindAll();

      var groupType = GetGroupType(defaultSearchArguments);
      if (groupType == null)
        return positions;

      return positions.Where(p => p.GroupTypes.Any(gtp => gtp.GroupType == groupType));
    }

    private GroupType? GetGroupType (RolePropertiesSearchArguments? searchArguments)
    {
      if (searchArguments == null)
        return null;

      using (SecurityFreeSection.Activate())
      {
        var group = searchArguments.GroupHandle.GetObject();
        return group.GroupType;
      }
    }

    private IEnumerable<T> FilterByAccess<T> (IEnumerable<T> securableObjects, params Enum[] requiredAccessTypeEnums) where T: ISecurableObject
    {
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      AccessType[] requiredAccessTypes = Array.ConvertAll(requiredAccessTypeEnums, AccessType.Get);

      return securableObjects.Where(o => securityClient.HasAccess(o, requiredAccessTypes));
    }
  }
}
