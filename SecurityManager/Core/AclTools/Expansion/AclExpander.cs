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
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Core class of the AclTools.Expansion library:
  /// Creates an <see cref="AclExpansionEntry"/> enumeration through  <see cref="GetAclExpansionEntries"/>
  /// for either all <see cref="User"/>|s and all <see cref="AccessControlList"/>|s
  /// or for the ones returned by a <see cref="IUserRoleAclAceCombinationFinder"/> passed to its ctor.
  /// </summary>
  public class AclExpander
  {
    private readonly IUserRoleAclAceCombinationFinder _userRoleAclAceCombinationFinder;
    private readonly AclExpansionEntryCreator _aclExpansionEntryCreator = new AclExpansionEntryCreator();

    // IEqualityComparer for value based comparison of AclExpansionEntry|s.
    private static readonly CompoundValueEqualityComparer<AclExpansionEntry> _aclExpansionEntryEqualityComparer =
      new CompoundValueEqualityComparer<AclExpansionEntry>(a => new object?[] {
          a.User,
          a.Role,
          a.Class,
          a.AccessControlList is StatefulAccessControlList ? ComponentwiseEqualsAndHashcodeWrapper.New(a.GetStateCombinations()) : null,
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

    /// <summary>
    /// <see cref="AclExpander"/> base ctor: Initializes the class instance with an <see cref="IUserRoleAclAceCombinationFinder"/>
    /// which supplies the <see cref="UserRoleAclAceCombination"/>|s the <see cref="AclExpander"/> works on.
    /// </summary>
    /// <param name="userRoleAclAceCombinationFinder"></param>
    public AclExpander (IUserRoleAclAceCombinationFinder userRoleAclAceCombinationFinder)
    {
      ArgumentUtility.CheckNotNull("userRoleAclAceCombinationFinder", userRoleAclAceCombinationFinder);
      _userRoleAclAceCombinationFinder = userRoleAclAceCombinationFinder;
    }

    /// <summary>
    /// Convenience ctor which takes <see cref="IAclExpanderUserFinder"/> and <see cref="IAclExpanderAclFinder"/> separately and combines them into
    /// an <see cref="IUserRoleAclAceCombinationFinder"/> to initialize the class instance with.
    /// </summary>
    /// <param name="userFinder"></param>
    /// <param name="accessControlListFinder"></param>
    public AclExpander (IAclExpanderUserFinder userFinder, IAclExpanderAclFinder accessControlListFinder)
      : this(new UserRoleAclAceCombinationFinder(userFinder, accessControlListFinder))
    {}

    /// <summary>
    /// Default behavior is to use all <see cref="User"/>|s and all <see cref="AccessControlList"/>|s.
    /// </summary>
    public AclExpander () : this(new AclExpanderUserFinder(), new AclExpanderAclFinder()) {}

    public AclExpansionEntryCreator AclExpansionEntryCreator
    {
      get { return _aclExpansionEntryCreator; }
    }

    /// <summary>
    /// Returns the distinct result set of calling <see cref="GetAclExpansionEntryList"/> sorted after the <see cref="User"/>|s last and first name.
    /// </summary>
    virtual public List<AclExpansionEntry> GetAclExpansionEntryListSortedAndDistinct ()
    {
      return (from AclExpansionEntry aclExpansionEntry in GetAclExpansionEntryList()
              orderby aclExpansionEntry.User.LastName, aclExpansionEntry.User.FirstName
              select aclExpansionEntry).Distinct(_aclExpansionEntryEqualityComparer).ToList();
    }


    /// <summary>
    /// Returns the expansion of <see cref="AccessTypeDefinition"/>|s for 
    /// all the <see cref="User"/>|s and all <see cref="AccessControlList"/>|s  
    /// supplied in the ctor as a <see cref="IEnumerable{T}"/> of <see cref="AclExpansionEntry"/>. 
    /// </summary>
    virtual public IEnumerable<AclExpansionEntry> GetAclExpansionEntries ()
    {
      foreach (UserRoleAclAceCombination userRoleAclAce in _userRoleAclAceCombinationFinder)
      {
        AclExpansionEntry? aclExpansionEntry = AclExpansionEntryCreator.CreateAclExpansionEntry(userRoleAclAce);
        if (aclExpansionEntry != null)
        {
          yield return aclExpansionEntry;
        }
      }
    }


    /// <summary>
    /// Returns the expansion of <see cref="AccessTypeDefinition"/>|s for 
    /// all the <see cref="User"/>|s and all <see cref="AccessControlList"/>|s  
    /// supplied in the ctor as a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>. 
    /// </summary>
    virtual public List<AclExpansionEntry> GetAclExpansionEntryList ()
    {
      return GetAclExpansionEntries().ToList();
    }
  }
}
