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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Defines query extensions for types declared in the <c>Remotion.SecurityManager.Domain.AccessControl</c> namespace.
  /// </summary>
  public static class AccessControlExtensions
  {
    [LinqPropertyRedirection(typeof (StatefulAccessControlList), "MyClass")]
    public static SecurableClassDefinition GetClassForQuery (this StatefulAccessControlList acl)
    {
      ArgumentUtility.CheckNotNull ("acl", acl);

      return acl.Class;
    }

    [LinqPropertyRedirection(typeof (StatelessAccessControlList), "MyClass")]
    public static SecurableClassDefinition GetClassForQuery (this StatelessAccessControlList acl)
    {
      ArgumentUtility.CheckNotNull ("acl", acl);

      return acl.Class;
    }

    [LinqPropertyRedirection(typeof (StatefulAccessControlList), "StateCombinationsInternal")]
    public static ObjectList<StateCombination> GetStateCombinationsForQuery (this StatefulAccessControlList acl)
    {
      ArgumentUtility.CheckNotNull ("acl", acl);

      return new ObjectList<StateCombination> (acl.StateCombinations);
    }

    [LinqPropertyRedirection(typeof (StateCombination), "StateUsages")]
    public static ObjectList<StateUsage> GetStateUsagesForQuery (this StateCombination stateCombination)
    {
      throw new NotSupportedException ("GetStateUsages() is only supported for building LiNQ query expressions.");
    }

    [LinqPropertyRedirection(typeof (AccessControlEntry), "PermissionsInternal")]
    public static ObjectList<Permission> GetPermissionsForQuery (this AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull ("ace", ace);

      return new ObjectList<Permission> (ace.GetPermissions());
    }
  }
}