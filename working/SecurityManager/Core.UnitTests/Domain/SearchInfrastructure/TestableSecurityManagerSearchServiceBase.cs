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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure
{
  public class TestableSecurityManagerSearchServiceBase : SecurityManagerSearchServiceBase<User>
  {
    private readonly IQueryable<User> _queryable;

    public TestableSecurityManagerSearchServiceBase (IQueryable<User> queryable)
    {
      ArgumentUtility.CheckNotNull ("queryable", queryable);
      _queryable = queryable;
    }

    public override bool SupportsProperty (IBusinessObjectReferenceProperty property)
    {
      return true;
    }

    protected override QueryFactory GetQueryFactory (IBusinessObjectReferenceProperty property)
    {
      return delegate { return _queryable.Cast<IBusinessObject>(); };
    }
  }
}