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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// Base-Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="BaseSecurityManagerObject"/> type.
  /// </summary>
  /// <typeparam name="TReferencingObject">The <see cref="Type"/> of the object that exposes the property.</typeparam>
  public abstract class SecurityManagerSearchServiceBase<TReferencingObject> : ISearchAvailableObjectsService
      where TReferencingObject: BaseSecurityManagerObject
  {
    protected delegate IQueryable<IBusinessObject> QueryFactory (
        TReferencingObject referencingObject,
        IBusinessObjectReferenceProperty property,
        TenantConstraint? tenantConstraint,
        DisplayNameConstraint? displayNameConstraint);

    public abstract bool SupportsProperty (IBusinessObjectReferenceProperty property);

    protected abstract QueryFactory GetQueryFactory (IBusinessObjectReferenceProperty property);

    public IBusinessObject[] Search (
        IBusinessObject? referencingObject,
        IBusinessObjectReferenceProperty property,
        ISearchAvailableObjectsArguments? searchArguments)
    {
      var referencingSecurityManagerObject = ArgumentUtility.CheckType<TReferencingObject>("referencingObject", referencingObject);
      ArgumentUtility.CheckNotNull("property", property);

      var queryFactory = GetQueryFactory(property);

      var securityManagerSearchArguments = CreateSearchArguments(searchArguments);
      return CreateQuery(queryFactory, referencingSecurityManagerObject, property, securityManagerSearchArguments).ToArray();
    }

    private IQueryable<IBusinessObject> CreateQuery (
        QueryFactory queryFactory,
        TReferencingObject referencingSecurityManagerObject,
        IBusinessObjectReferenceProperty property,
        SecurityManagerSearchArguments searchArguments)
    {
      var query = queryFactory(referencingSecurityManagerObject, property, searchArguments.TenantConstraint, searchArguments.DisplayNameConstraint);
      return query.Apply(searchArguments.ResultSizeConstraint);
    }

    private SecurityManagerSearchArguments CreateSearchArguments (ISearchAvailableObjectsArguments? searchArguments)
    {
      if (searchArguments == null)
        return new SecurityManagerSearchArguments(null, null, null);

      var defaultSearchArguments = searchArguments as DefaultSearchArguments;
      if (defaultSearchArguments != null)
      {
        if (string.IsNullOrEmpty(defaultSearchArguments.SearchStatement))
          return new SecurityManagerSearchArguments(null, null, null);

        var tenantHandle = ObjectID.Parse(defaultSearchArguments.SearchStatement).GetHandle<Tenant>();
        return new SecurityManagerSearchArguments(new TenantConstraint(tenantHandle), null, null);
      }

      return ArgumentUtility.CheckType<SecurityManagerSearchArguments>("searchArguments", searchArguments);
    }
  }
}
