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
using System.Collections;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  /// <summary>
  /// Supplies enumeration over all <see cref="Role"/>|s of <see cref="User"/>|s (taken from <see cref="IAclExpanderUserFinder"/>) and 
  /// <see cref="AccessControlEntry"/>|s of <see cref="AccessControlList"/>|s (taken from <see cref="IAclExpanderAclFinder"/>)
  /// combinations. 
  /// </summary>
  public class UserRoleAclAceCombinationFinder : IUserRoleAclAceCombinationFinder
  {
    private readonly IAclExpanderUserFinder _userFinder;
    private readonly IAclExpanderAclFinder _accessControlListFinder;

    public UserRoleAclAceCombinationFinder (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
    {
      _userFinder = userFinder;
      _accessControlListFinder = accessControlListFinder;
    }

    public IAclExpanderUserFinder UserFinder
    {
      get { return _userFinder; }
    }

    public IAclExpanderAclFinder AccessControlListFinder
    {
      get { return _accessControlListFinder; }
    }

    public IEnumerator<UserRoleAclAceCombination> GetEnumerator ()
    {
      var users = UserFinder.FindUsers ();
      var acls = AccessControlListFinder.FindAccessControlLists ();

      foreach (var user in users)
      {
        //To.ConsoleLine.e (() => user);
        foreach (var role in user.Roles)
        {
          //To.ConsoleLine.s ("\t").e (() => role);
          foreach (var acl in acls)
          {
            //To.ConsoleLine.s ("\t\t").e (() => acl);
            foreach (var ace in acl.AccessControlEntries)
            {
              //To.ConsoleLine.s ("\t\t\t").e (() => ace);
              yield return new UserRoleAclAceCombination (role, ace);
            }
          }
        }
      }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
