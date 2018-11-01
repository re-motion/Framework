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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// Base-Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="BaseSecurityManagerObject"/> type when implementing a 
  /// search service for a specific property type.
  /// </summary>
  /// <typeparam name="TReferencedObject">The <see cref="Type"/> of the object referenced by the property.</typeparam>
  /// <remarks>
  /// Inherit from this type and implement the <see cref="CreateQuery"/> template method to return a new query for the 
  /// <typeparamref name="TReferencedObject"/>.
  /// </remarks>
  public abstract class SecurityManagerPropertyTypeBasedSearchServiceBase<TReferencedObject>
      : SecurityManagerSearchServiceBase<BaseSecurityManagerObject>
      where TReferencedObject: BaseSecurityManagerObject
  {
    protected abstract IQueryable<IBusinessObject> CreateQuery (
        BaseSecurityManagerObject referencingObject,
        IBusinessObjectReferenceProperty property,
        TenantConstraint tenantConstraint,
        DisplayNameConstraint displayNameConstraint);

    public override sealed bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return typeof (TReferencedObject).IsAssignableFrom (property.PropertyType);
    }

    protected override sealed QueryFactory GetQueryFactory (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (!SupportsProperty (property))
      {
        throw new ArgumentException (
            string.Format (
                "The type of the property '{0}', declared on '{1}', is not supported by the '{2}' type.",
                property.Identifier,
                property.ReflectedClass.Identifier,
                GetType().FullName));
      }

      return CreateQuery;
    }
  }
}