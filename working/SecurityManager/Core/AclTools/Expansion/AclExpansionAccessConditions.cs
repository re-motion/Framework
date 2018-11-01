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
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpansionAccessConditions  
  {
    private static readonly CompoundValueEqualityComparer<AclExpansionAccessConditions> _equalityComparer =
      new CompoundValueEqualityComparer<AclExpansionAccessConditions> (a => new object[] {
          a.AbstractRole, a.OwningGroup, a.OwningTenant, a.GroupHierarchyCondition, a.TenantHierarchyCondition, a.IsOwningUserRequired
      }
    );

    public static CompoundValueEqualityComparer<AclExpansionAccessConditions> EqualityComparer
    {
      get { return _equalityComparer; }
    }


    public bool IsOwningUserRequired { get; set; }

    public bool IsAbstractRoleRequired
    {
      get { return AbstractRole != null; }
    }
    
    public AbstractRoleDefinition AbstractRole { get; set; }


    // Owning Group
    public Group OwningGroup { get; set; }
    public GroupHierarchyCondition GroupHierarchyCondition { get; set; }

    public bool HasOwningGroupCondition
    {
      get { return OwningGroup != null; }
    }


    // Owning Tenant
    public Tenant OwningTenant { get; set; }
    public TenantHierarchyCondition TenantHierarchyCondition { get; set; }
    
    public bool HasOwningTenantCondition 
    {
      get { return OwningTenant != null; }
    }


    public override bool Equals (object obj)
    {
      return EqualityComparer.Equals (this, obj);
    }

    public override int GetHashCode ()
    {
      return EqualityComparer.GetHashCode (this);
    }
  }
}
