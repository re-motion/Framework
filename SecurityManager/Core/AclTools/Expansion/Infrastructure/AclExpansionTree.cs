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
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionTree
  {
    private readonly Func<AclExpansionEntry, string> _orderbyForSecurableClass;

    // IEqualityComparer which ignores differences in states (AclExpansionEntry.StateCombinations) to
    // group AclExpansionEntry|s together which only differ in state.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryIgnoreStateEqualityComparer =
        new CompoundValueEqualityComparer<AclExpansionEntry>(a => new object?[] {
            a.Class, a.Role, a.User,
            a.AccessConditions.AbstractRole,
            a.AccessConditions.GroupHierarchyCondition,
            a.AccessConditions.IsOwningUserRequired,
            a.AccessConditions.OwningGroup,
            a.AccessConditions.OwningTenant,
            a.AccessConditions.TenantHierarchyCondition,
            ComponentwiseEqualsAndHashcodeWrapper.New(a.AllowedAccessTypes),
            ComponentwiseEqualsAndHashcodeWrapper.New(a.DeniedAccessTypes)
          }
        );

    private readonly List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition,
      AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>> _aclExpansionTree;

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion)
        : this(aclExpansion, (classEntry  => (classEntry.AccessControlList is StatelessAccessControlList) ? "" : classEntry.Class.DisplayName))
    {
    }

    public AclExpansionTree (List<AclExpansionEntry> aclExpansion, Func<AclExpansionEntry, string> orderbyForSecurableClass)
    {
      _orderbyForSecurableClass = orderbyForSecurableClass;
      _aclExpansionTree = CreateAclExpansionTree(aclExpansion);
    }

    public List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition,
      AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>> Tree
    {
      get { return _aclExpansionTree; }
    }


    public static CompoundValueEqualityComparer<AclExpansionEntry> AclExpansionEntryIgnoreStateEqualityComparer
    {
      get { return _aclExpansionEntryIgnoreStateEqualityComparer; }
    }

    private List<AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition,
      AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>>>
      CreateAclExpansionTree (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull("aclExpansion", aclExpansion);

      var aclExpansionTree = (aclExpansion.OrderBy(entry => entry.User.DisplayName).GroupBy(entry => entry.User).Select(
          grouping => AclExpansionTreeNode.New(grouping.Key, CountRowsBelow(grouping), RoleGrouping(grouping).ToList()))).ToList();

      return aclExpansionTree;
    }

    private IEnumerable<AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>> RoleGrouping (
        IGrouping<User, AclExpansionEntry> grouping)
    {
      return grouping
          .OrderBy(roleEntry => roleEntry.Role.Group?.DisplayName)
          .ThenBy(roleEntry => roleEntry.Role.Position?.DisplayName)
          .GroupBy(roleEntry => roleEntry.Role)
          .Select(roleGrouping => AclExpansionTreeNode.New(roleGrouping.Key, CountRowsBelow(roleGrouping), ClassGrouping(roleGrouping).ToList()));
    }

    private IEnumerable<AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>> ClassGrouping (
        IGrouping<Role, AclExpansionEntry> roleGrouping)
    {
      return (roleGrouping.OrderBy(classEntry => _orderbyForSecurableClass(classEntry)).GroupBy(
          classEntry => classEntry.Class).Select(
          classGrouping => AclExpansionTreeNode.New(classGrouping.Key, CountRowsBelow(classGrouping), StateGrouping(classGrouping).ToList())));
    }

    private List<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> StateGrouping (IGrouping<SecurableClassDefinition, AclExpansionEntry> classGrouping)
    {
      return classGrouping.GroupBy(
          aee => aee, aee => aee, AclExpansionEntryIgnoreStateEqualityComparer).
          Select(x => AclExpansionTreeNode.New(x.Key, x.Count(), x.ToList())).ToList();
    }



    private int CountRowsBelow<T> (IGrouping<T, AclExpansionEntry> grouping)
    {
      return grouping.Distinct(AclExpansionEntryIgnoreStateEqualityComparer).Count();
    }
  }
}
