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
using System.Diagnostics.CodeAnalysis;
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions
  {
    private static readonly CompoundValueEqualityComparer<AclExpansionAccessConditions> _equalityComparer =
      new CompoundValueEqualityComparer<AclExpansionAccessConditions>(a => new object?[] {
          a.AbstractRole, a.OwningGroup, a.OwningTenant, a.GroupHierarchyCondition, a.TenantHierarchyCondition, a.IsOwningUserRequired
      }
    );

    public static CompoundValueEqualityComparer<AclExpansionAccessConditions> EqualityComparer
    {
      get { return _equalityComparer; }
    }


    public bool IsOwningUserRequired { get; set; }

    [MemberNotNullWhen(true, nameof(AbstractRole))]
    public bool IsAbstractRoleRequired
    {
      get { return AbstractRole != null; }
    }

    public AbstractRoleDefinition? AbstractRole { get; set; }


    // Owning Group
    public Group? OwningGroup { get; set; }
    public GroupHierarchyCondition GroupHierarchyCondition { get; set; }

    [MemberNotNullWhen(true, nameof(OwningGroup))]
    public bool HasOwningGroupCondition
    {
      get { return OwningGroup != null; }
    }


    // Owning Tenant
    public Tenant? OwningTenant { get; set; }
    public TenantHierarchyCondition TenantHierarchyCondition { get; set; }

    [MemberNotNullWhen(true, nameof(OwningTenant))]
    public bool HasOwningTenantCondition
    {
      get { return OwningTenant != null; }
    }


    public override bool Equals (object? obj)
    {
      return EqualityComparer.Equals(this, obj);
    }

    public override int GetHashCode ()
    {
      return EqualityComparer.GetHashCode(this);
    }
  }
}
