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
using System.Collections.Generic;
using System.Linq;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  /// <summary>
  /// Constraints sequences of <see cref="BaseSecurityManagerObject"/> instances based on whether the <b>DisplayName</b> contains the <see cref="Value"/>.
  /// </summary>
  public class DisplayNameConstraint
  {
    private readonly string _value;

    public DisplayNameConstraint (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      _value = value;
    }

    public string Value
    {
      get { return _value; }
    }

    public IQueryable<Tenant> ApplyTo (IQueryable<Tenant> tenants)
    {
      ArgumentUtility.CheckNotNull ("tenants", tenants);

      if (HasConstraint())
        return tenants.Where (t => t.Name.Contains (Value));

      return tenants;
    }

    public IQueryable<Group> ApplyTo (IQueryable<Group> groups)
    {
      ArgumentUtility.CheckNotNull ("groups", groups);

      if (HasConstraint())
        return groups.Where (g => g.Name.Contains (Value) || g.ShortName.Contains (Value));

      return groups;
    }

    public IQueryable<User> ApplyTo (IQueryable<User> users)
    {
      ArgumentUtility.CheckNotNull ("users", users);

      if (HasConstraint())
        return users.Where (u => u.LastName.Contains (Value) || u.FirstName.Contains (Value));

      return users;
    }

    public IQueryable<Position> ApplyTo (IQueryable<Position> positions)
    {
      ArgumentUtility.CheckNotNull ("positions", positions);

      if (HasConstraint())
        return positions.Where (t => t.Name.Contains (Value));

      return positions;
    }

    public IQueryable<GroupType> ApplyTo (IQueryable<GroupType> groupTypes)
    {
      ArgumentUtility.CheckNotNull ("groupTypes", groupTypes);

      if (HasConstraint())
        return groupTypes.Where (t => t.Name.Contains (Value));

      return groupTypes;
    }

    public IEnumerable<T> ApplyTo<T> (IEnumerable<T> metadataObject)
        where T: MetadataObject
    {
      ArgumentUtility.CheckNotNull ("metadataObject", metadataObject);

      if (HasConstraint())
        return metadataObject.Where (t => t.DisplayName.IndexOf (Value, StringComparison.CurrentCultureIgnoreCase) != -1);

      return metadataObject;
    }

    private bool HasConstraint ()
    {
      return !String.IsNullOrEmpty (Value);
    }
  }
}