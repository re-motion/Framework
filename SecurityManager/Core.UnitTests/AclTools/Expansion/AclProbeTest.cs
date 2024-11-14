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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using PrincipalTestHelper = Remotion.SecurityManager.UnitTests.Domain.AccessControl.PrincipalTestHelper;


namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclProbeTest : AclToolsTestBase
  {
    [Test]
    public void CreateAclProbe_User_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.Principal.User.ObjectID, Is.EqualTo(User.ID));
    }

    [Test]
    public void CreateAclProbe_Tenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.Principal.Tenant.ObjectID, Is.EqualTo(User.Tenant.ID));
    }

    [Test]
    public void CreateAclProbe_Role_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.Principal.Roles, Is.EquivalentTo(new[] { Role }).Using(PrincipalRoleComparer.Instance));
    }


    [Test]
    public void CreateAclProbe_OwningGroup_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.OwningGroup, Is.EqualTo(Role.Group).Using(DomainObjectHandleComparer.Instance));

      var accessConditionsExpected = new AclExpansionAccessConditions
                                     {
                                         OwningGroup = Group,
                                         GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren
                                     };
      Assert.That(aclProbe.AccessConditions, Is.EqualTo(accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_GroupSelectionAll_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithoutGroupCondition();
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.OwningGroup, Is.Null);

      var accessConditionsExpected = new AclExpansionAccessConditions();
      Assert.That(aclProbe.AccessConditions, Is.EqualTo(accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_SpecificTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithSpecificTenant(Tenant);
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.OwningTenant, Is.Null);

      var accessConditionsExpected = new AclExpansionAccessConditions();
      Assert.That(aclProbe.AccessConditions, Is.EqualTo(accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_OwningTenant_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      FleshOutAccessControlEntryForTest(ace);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.OwningTenant, Is.EqualTo(User.Tenant).Using(DomainObjectHandleComparer.Instance));

      var accessConditionsExpected = new AclExpansionAccessConditions
      {
        OwningTenant = Tenant,
        TenantHierarchyCondition = TenantHierarchyCondition.This
      };
      Assert.That(aclProbe.AccessConditions, Is.EqualTo(accessConditionsExpected));
    }

    [Test]
    public void CreateAclProbe_SpecificAbstractRole_Test ()
    {
      AccessControlEntry ace = TestHelper.CreateAceWithAbstractRole();
      FleshOutAccessControlEntryForTest(ace);
      Assert.That(ace.SpecificAbstractRole, Is.Not.Null);
      AclProbe aclProbe = AclProbe.CreateAclProbe(User, Role, ace);
      Assert.That(aclProbe.SecurityToken.AbstractRoles, Has.Member(ace.SpecificAbstractRole).Using(DomainObjectHandleComparer.Instance));

      var accessConditionsExpected = new AclExpansionAccessConditions();
      accessConditionsExpected.AbstractRole = ace.SpecificAbstractRole;
      Assert.That(aclProbe.AccessConditions, Is.EqualTo(accessConditionsExpected));
    }




    //--------------------------------------------------------------------------------------------------------------------------------
    // SecurityToken creation expectance tests
    //--------------------------------------------------------------------------------------------------------------------------------


    [Test]
    public void AccessControlList_GetAccessTypes2 ()
    {
      var user = User3;
      var acl = TestHelper.CreateStatefulAcl(Ace3);
      Assert.That(Ace3.Validate().IsValid);
      Principal principal = PrincipalTestHelper.Create(user.Tenant, user, user.Roles);
      SecurityToken securityToken = SecurityToken.Create(
          principal,
          user.Tenant,
          null,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());
      AccessInformation accessInformation = acl.GetAccessTypes(securityToken);
      Assert.That(accessInformation.AllowedAccessTypes, Is.EquivalentTo(new[] { ReadAccessType, WriteAccessType }));
    }

    [Test]
    public void AccessControlList_GetAccessTypes_AceWithPosition_GroupSelectionAll ()
    {
      var ace = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.None);
      AttachAccessTypeReadWriteDelete(ace, true, null, true);
      Assert.That(ace.Validate().IsValid);
      var acl = TestHelper.CreateStatefulAcl(ace);
      Principal principal = PrincipalTestHelper.Create(User.Tenant, User, User.Roles);
      SecurityToken securityToken = SecurityToken.Create(
          principal,
          User.Tenant,
          null,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());
      AccessInformation accessInformation = acl.GetAccessTypes(securityToken);
      Assert.That(accessInformation.AllowedAccessTypes, Is.EquivalentTo(new[] { ReadAccessType, DeleteAccessType }));
    }

    [Test]
    public void AccessControlList_GetAccessTypes_AceWithPosition_GroupSelectionOwningGroup ()
    {
      var ace = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(ace, true, null, true);
      Assert.That(ace.Validate().IsValid);
      var acl = TestHelper.CreateStatefulAcl(ace);
      // We pass the Group used in the ace Position above in the owningGroups-list => ACE will match.
      Principal principal = PrincipalTestHelper.Create(User.Tenant, User, User.Roles);
      SecurityToken securityToken = SecurityToken.Create(
          principal,
          User.Tenant,
          Group,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());
      AccessInformation accessInformation = acl.GetAccessTypes(securityToken);
      Assert.That(accessInformation.AllowedAccessTypes, Is.EquivalentTo(new[] { ReadAccessType, DeleteAccessType }));
    }

    private void FleshOutAccessControlEntryForTest (AccessControlEntry ace)
    {
      ace.SpecificGroup = TestHelper.CreateGroup("Specific Group for an ACE", null, Tenant);
      ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
    }
  }
}
