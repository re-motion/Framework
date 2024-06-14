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
    [LinqPropertyRedirection(typeof(StatefulAccessControlList), "MyClass")]
    public static SecurableClassDefinition GetClassForQuery (this StatefulAccessControlList acl)
    {
      ArgumentUtility.CheckNotNull("acl", acl);

      Assertion.IsNotNull(acl.Class, "AccessControlList{{{0}}}.Class must not be null when used in a query.", acl.ID);
      return acl.Class;
    }

    [LinqPropertyRedirection(typeof(StatelessAccessControlList), "MyClass")]
    public static SecurableClassDefinition GetClassForQuery (this StatelessAccessControlList acl)
    {
      ArgumentUtility.CheckNotNull("acl", acl);

      Assertion.IsNotNull(acl.Class, "AccessControlList{{{0}}}.Class must not be null when used in a query.", acl.ID);
      return acl.Class;
    }

    [LinqPropertyRedirection(typeof(StatefulAccessControlList), "StateCombinationsInternal")]
    public static ObjectList<StateCombination> GetStateCombinationsForQuery (this StatefulAccessControlList acl)
    {
      ArgumentUtility.CheckNotNull("acl", acl);

      return new ObjectList<StateCombination>(acl.StateCombinations);
    }

    [LinqPropertyRedirection(typeof(StateCombination), "StateUsages")]
    public static ObjectList<StateUsage> GetStateUsagesForQuery (this StateCombination stateCombination)
    {
      throw new NotSupportedException("GetStateUsages() is only supported for building LiNQ query expressions.");
    }

    [LinqPropertyRedirection(typeof(AccessControlEntry), "PermissionsInternal")]
    public static ObjectList<Permission> GetPermissionsForQuery (this AccessControlEntry ace)
    {
      ArgumentUtility.CheckNotNull("ace", ace);

      return new ObjectList<Permission>(ace.GetPermissions());
    }
  }
}
