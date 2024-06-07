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
using Remotion.Data.DomainObjects.Queries;
using Remotion.SecurityManager.Domain.OrganizationalStructure;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderUserFinder : IAclExpanderUserFinder
  {
    private readonly string? _firstName;
    private readonly string? _lastName;
    private readonly string? _userName;

    public AclExpanderUserFinder () : this(null,null,null) {}

    public AclExpanderUserFinder (string? firstName, string? lastName, string? userName)
    {
      _firstName = firstName;
      _lastName = lastName;
      _userName = userName;
    }

    public List<User> FindUsers ()
    {
      var findAllUsersQuery = from u in QueryFactory.CreateLinqQuery<User>()
                              where
                                (_lastName == null || u.LastName == _lastName) &&
                                (_firstName == null || u.FirstName == _firstName) &&
                                (_userName == null || u.UserName == _userName)
                              orderby u.LastName , u.FirstName
                              select u;
      return findAllUsersQuery.ToList();
    }
  }
}
