﻿// This file is part of re-strict (www.re-motion.org)
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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// Base-Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="BaseSecurityManagerObject"/> type when implementing a 
  /// search service for a specific property.
  /// </summary>
  /// <typeparam name="TReferencingObject">The <see cref="Type"/> of the object that exposes the property.</typeparam>
  /// <remarks>
  /// Inherit from this type and add search delegates for the properties the specified <typeparamref name="TReferencingObject"/> using the <see cref="RegisterQueryFactory"/> 
  /// method from the constructor.
  /// </remarks>
  public abstract class SecurityManagerPropertyBasedSearchServiceBase<TReferencingObject> : SecurityManagerSearchServiceBase<TReferencingObject>
      where TReferencingObject: BaseSecurityManagerObject
  {
    private readonly Dictionary<string, QueryFactory> _queryFactories = new Dictionary<string, QueryFactory>();

    protected void RegisterQueryFactory (string propertyName, QueryFactory queryFactory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("queryFactory", queryFactory);

      _queryFactories.Add (propertyName, queryFactory);
    }

    public override sealed bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      return _queryFactories.ContainsKey (property.Identifier);
    }

    protected override sealed QueryFactory GetQueryFactory (IBusinessObjectReferenceProperty property)
    {
      QueryFactory queryFactory;
      if (!_queryFactories.TryGetValue (property.Identifier, out queryFactory))
      {
        throw new ArgumentException (
            string.Format ("The property '{0}' is not supported by the '{1}' type.", property.Identifier, GetType().FullName));
      }
      return queryFactory;
    }
  }
}