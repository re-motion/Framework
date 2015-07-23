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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Represents a row in an access control list expansion (see <see cref="AclExpander"/>).
  /// </summary>
 public class AclExpansionEntry
  {
    private readonly AccessControlList _accessControlList;

    public AclExpansionEntry (
        User user,
        Role role,
        AccessControlList accessControlList,
        AclExpansionAccessConditions accessConditions,
        AccessTypeDefinition[] allowedAccessTypes,
        AccessTypeDefinition[] deniedAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("accessControlList", accessControlList);
      ArgumentUtility.CheckNotNull ("accessConditions", accessConditions);
      ArgumentUtility.CheckNotNull ("accessTypeDefinitions", allowedAccessTypes);
      User = user;
      Role = role;
      _accessControlList = accessControlList;
      AccessConditions = accessConditions;
      AllowedAccessTypes = allowedAccessTypes;
      DeniedAccessTypes = deniedAccessTypes;
    }

    public User User { get; private set; }
    public Role Role { get; private set; }

    public SecurableClassDefinition Class
    {
      get { return AccessControlList.Class; }
    }


    public IList<StateCombination> GetStateCombinations ()
    {
      if (AccessControlList is StatefulAccessControlList)
        return ((StatefulAccessControlList) AccessControlList).StateCombinations;
      else
      {
        // Throw exception (instead of returning e.g. new StateCombination[0]) in case of StatelessAccessControlList, 
        // to avoid "silent failure" in calling code
        throw new InvalidOperationException (
            @"StateCombinations not defined for StatelessAccessControlList. Test for ""is StatefulAccessControlList"" in calling code.");
      }
    }

    public AclExpansionAccessConditions AccessConditions { get; private set; }
    public AccessTypeDefinition[] AllowedAccessTypes { get; private set; }
    public AccessTypeDefinition[] DeniedAccessTypes { get; private set; }

    public AccessControlList AccessControlList
    {
      get { return _accessControlList; }
    }
  }
}
