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
using System.IO;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.AclTools.Expansion.TestClasses;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderTest : AclToolsTestBase
  {
    [Test]
    public void Ctor_UserFinderAclFinder_Test ()
    {
      var userFinderStub = new Mock<IAclExpanderUserFinder>();
      var aclFinderStub = new Mock<IAclExpanderAclFinder>();
      var aclExpander = new AclExpander(userFinderStub.Object, aclFinderStub.Object);
      var iFinder = AclExpanderXray.GetUserRoleAclAceCombinationFinder(aclExpander);
      Assert.That(iFinder, Is.TypeOf(typeof(UserRoleAclAceCombinationFinder)));
      var finder = (UserRoleAclAceCombinationFinder)iFinder;
      Assert.That(finder.UserFinder, Is.EqualTo(userFinderStub.Object));
      Assert.That(finder.AccessControlListFinder, Is.EqualTo(aclFinderStub.Object));
    }

    [Test]
    public void Ctor_UserRoleAclAceCombinationFinder_Test ()
    {
      var userRoleAclAceCombinationFinderStub = new Mock<IUserRoleAclAceCombinationFinder>();
      var aclExpander = new AclExpander(userRoleAclAceCombinationFinderStub.Object);
      var finder = AclExpanderXray.GetUserRoleAclAceCombinationFinder(aclExpander);
      Assert.That(finder, Is.EqualTo(userRoleAclAceCombinationFinderStub.Object));
    }


    [Test]
    public void GetAclExpansionEntryListSortedAndDistinctTest ()
    {
      var userRoleAclAceCombinationFinderStub = new Mock<IUserRoleAclAceCombinationFinder>();
      var aclExpanderMock = new Mock<AclExpander>(userRoleAclAceCombinationFinderStub.Object) { CallBase = true };
      var accessConditions = new AclExpansionAccessConditions();
      var accessTypeDefinitions = new AccessTypeDefinition[0];

      var aclExpansionEntry0 = new AclExpansionEntry(User2, Role, Acl, accessConditions, accessTypeDefinitions, accessTypeDefinitions);
      var aclExpansionEntry1 = new AclExpansionEntry(User3, Role, Acl, accessConditions, accessTypeDefinitions, accessTypeDefinitions);
      var aclExpansionEntry2 = new AclExpansionEntry(User, Role, Acl, accessConditions, accessTypeDefinitions, accessTypeDefinitions);

      var aclExpansionEntryList = ListObjectMother.New(aclExpansionEntry0, aclExpansionEntry2, aclExpansionEntry1, aclExpansionEntry1, aclExpansionEntry0);

      aclExpanderMock.Setup(x => x.GetAclExpansionEntryList()).Returns(aclExpansionEntryList).Verifiable();
      var aclExpansionEntryListResult = aclExpanderMock.Object.GetAclExpansionEntryListSortedAndDistinct();
      var aclExpansionEntryListExpected = ListObjectMother.New(aclExpansionEntry1, aclExpansionEntry0, aclExpansionEntry2);

      Assert.That(aclExpansionEntryListResult, Is.EqualTo(aclExpansionEntryListExpected));
      aclExpanderMock.Verify();
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_IUserRoleAclAceCombinations2 ()
    {
      var userRoleAclAceCombinationsMock = new Mock<IUserRoleAclAceCombinationFinder>();
      var myValues = ListObjectMother.New(new UserRoleAclAceCombination(Role, Ace));
      userRoleAclAceCombinationsMock.Setup(mock => mock.GetEnumerator()).Returns(myValues.GetEnumerator()).Verifiable();

      var aclExpander = new AclExpander(userRoleAclAceCombinationsMock.Object);
      aclExpander.GetAclExpansionEntryList();
      userRoleAclAceCombinationsMock.Verify();
    }




    //--------------------------------------------------------------------------------------------------------------------------------
    // AclExpander Integration Tests
    //--------------------------------------------------------------------------------------------------------------------------------


    [Test]
    public void GetAclExpansionEntryList_AceWithPosition_GroupSelectionAll ()
    {
      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserPositionGroupSelection(User,Position,GroupCondition.None, GroupHierarchyCondition.Undefined);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions();
      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));
      Assert.That(aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo(accessTypeDefinitionsExpected));
      Assert.That(aclExpansionEntryList[0].AccessConditions, Is.EqualTo(accessConditions));
    }

    [Test]
    public void GetAclExpansionEntryList_AceWithPosition_GroupSelectionOwningGroup ()
    {
      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserPositionGroupSelection(User, Position, GroupCondition.OwningGroup, GroupHierarchyCondition.This);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions
        {
          OwningGroup = aclExpansionEntryList[0].Role.Group, //  GroupSelection.OwningGroup => group must be owner 
          GroupHierarchyCondition = GroupHierarchyCondition.This
        };
      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));
      Assert.That(aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo(accessTypeDefinitionsExpected));
      Assert.That(aclExpansionEntryList[0].AccessConditions, Is.EqualTo(accessConditions));
    }

    [Test]
    public void GetAclExpansionEntryList_UserList_AceList ()
    {
      var ace = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(ace, true, null, true);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(ace));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };
      var accessConditions = new AclExpansionAccessConditions
        {
          OwningGroup = User.Roles[0].Group, //  GroupSelection.OwningGroup => group must be owner
          GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent
        };
      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));
      Assert.That(aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo(accessTypeDefinitionsExpected));
      Assert.That(aclExpansionEntryList[0].AccessConditions, Is.EqualTo(accessConditions));
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_MultipleAces ()
    {
      var aceGroupOwning = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(aceGroupOwning, true, null, true);

      var aceAbstractRole = TestHelper.CreateAceWithAbstractRole();
      AttachAccessTypeReadWriteDelete(aceAbstractRole, null, false, true);

      var aceGroupAll = TestHelper.CreateAceWithoutGroupCondition();
      AttachAccessTypeReadWriteDelete(aceGroupAll, true, true, null);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceGroupOwning, aceAbstractRole, aceGroupAll));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(3));

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType, DeleteAccessType },
        //new AclExpansionAccessConditions { HasOwningGroupCondition = true });
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[0].Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { ReadAccessType, DeleteAccessType },
       new AclExpansionAccessConditions { AbstractRole = aceAbstractRole.SpecificAbstractRole });

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[2], new[] { ReadAccessType, WriteAccessType },
       new AclExpansionAccessConditions());
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_AnotherTenant ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      TestHelper.CreateRole(otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceGroupSpecificTenant = TestHelper.CreateAceWithSpecificTenant(otherTenant);
      AttachAccessTypeReadWriteDelete(aceGroupSpecificTenant, null, true, null);

      var userList = ListObjectMother.New(otherTenantUser);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceGroupSpecificTenant));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { WriteAccessType },
        new AclExpansionAccessConditions());
    }


    [Test]
    public void SpecificTenantAndOwningTenantTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      TestHelper.CreateRole(otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceGroupSpecificTenant = TestHelper.CreateAceWithSpecificTenant(otherTenant);
      AttachAccessTypeReadWriteDelete(aceGroupSpecificTenant, null, true, null);

      var aceGroupOwningTenant = TestHelper.CreateAceWithOwningTenant();
      AttachAccessTypeReadWriteDelete(aceGroupOwningTenant, null, null, true);

      var userList = ListObjectMother.New(otherTenantUser);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceGroupSpecificTenant, aceGroupOwningTenant));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(2));

      // Test of specific-tenant-ACE: gives write-access
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions());

      // Test of owning-tenant-ACE (specific-tenant-ACE matches also): gives write-access + delete-access with condition: tenant-must-own
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { OwningTenant = otherTenant, TenantHierarchyCondition = TenantHierarchyCondition.This });
    }


    [Test]
    public void TenantHierarchyConditionTest ()
    {
      // Create a new user whose tenant has a parent
      var tenant = TestHelper.CreateTenant("Tenant");
      var parentTenant = TestHelper.CreateTenant("Parent Tenant");
      tenant.Parent = parentTenant;
      var group = TestHelper.CreateGroup("Group", null, tenant);
      var position = TestHelper.CreatePosition("Position");
      var user = TestHelper.CreateUser("User", "U", "Ser", "Dr", group, tenant);
      TestHelper.CreateRole(user, group, position);

      var ace = TestHelper.CreateAceWithOwningTenant();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.ThisAndParent;

      AttachAccessTypeReadWriteDelete(ace, null, true, null);
      Assert.That(ace.Validate().IsValid);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(ace));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList);

      var owningTenant = aclExpansionEntryList[0].User.Tenant;
      var tenantHierarchyCondition = TenantHierarchyCondition.ThisAndParent;

      var accessConditions = new AclExpansionAccessConditions
      {
        OwningTenant = owningTenant,
        TenantHierarchyCondition = tenantHierarchyCondition
      };

      var accessTypeDefinitionsExpected = new[] { WriteAccessType };

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));
      Assert.That(aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo(accessTypeDefinitionsExpected));
      Assert.That(aclExpansionEntryList[0].AccessConditions.OwningTenant, Is.EqualTo(owningTenant));
      Assert.That(aclExpansionEntryList[0].AccessConditions.TenantHierarchyCondition, Is.EqualTo(tenantHierarchyCondition));
      Assert.That(aclExpansionEntryList[0].AccessConditions, Is.EqualTo(accessConditions));
    }



    [Test]
    public void GetAclExpansionEntryList_UserList_AceList_TwoDifferentTenants ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      TestHelper.CreateRole(otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceSpecificTenantWithOtherTenant = TestHelper.CreateAceWithSpecificTenant(otherTenant);
      AttachAccessTypeReadWriteDelete(aceSpecificTenantWithOtherTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(aceGroupOwning, true, null, true);

      var userList = ListObjectMother.New(otherTenantUser, User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceSpecificTenantWithOtherTenant, aceGroupOwning));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(2));

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions());

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { ReadAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[1].Role.Group, GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });
    }


    [Test]
    public void GetAclExpansionEntryList_UserList_MultipleAces ()
    {
      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant();
      AttachAccessTypeReadWriteDelete(aceOwningTenant, true, true, null);

      var acePosition = TestHelper.CreateAceWithPositionAndGroupCondition(Position2, GroupCondition.None);
      AttachAccessTypeReadWriteDelete(acePosition, true, null, true);

      var aceGroupOwning = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(aceGroupOwning, true, false, null);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceOwningTenant, acePosition, aceGroupOwning));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(2));

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions { OwningTenant = User.Tenant, TenantHierarchyCondition = TenantHierarchyCondition.This });

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { ReadAccessType },
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[1].Role.Group,
          GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });
     }


    [Test]
    public void GetAclExpansionEntryList_UserList_SeparateAcls ()
    {
      var aceOwningTenant = TestHelper.CreateAceWithOwningTenant();
      AttachAccessTypeReadWriteDelete(aceOwningTenant, true, true, null);

      var aceSpecificTenant = TestHelper.CreateAceWithSpecificTenant(Tenant);
      AttachAccessTypeReadWriteDelete(aceSpecificTenant, true, true, null);

      var aceGroupOwning = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(aceGroupOwning, true, null, true);

      var userList = ListObjectMother.New(User);

      var aclList = ListObjectMother.New(
          TestHelper.CreateStatefulAcl(aceOwningTenant),
          TestHelper.CreateStatefulAcl(aceSpecificTenant),
          TestHelper.CreateStatefulAcl(aceGroupOwning));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      var aclExpansionEntryListEnumerator = aclExpansionEntryList.GetEnumerator();

      aclExpansionEntryListEnumerator.MoveNext();
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryListEnumerator.Current, new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions { OwningTenant = User.Tenant, TenantHierarchyCondition = TenantHierarchyCondition.This });

      aclExpansionEntryListEnumerator.MoveNext();
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryListEnumerator.Current, new[] { ReadAccessType, WriteAccessType },
        new AclExpansionAccessConditions());

      aclExpansionEntryListEnumerator.MoveNext();
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryListEnumerator.Current, new[] { ReadAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryListEnumerator.Current.Role.Group,
          GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });

      Assert.That(aclExpansionEntryListEnumerator.MoveNext(), Is.EqualTo(false));
    }


    [Test]
    public void NonMatchingAceTest_SpecificTenant ()
    {
      // Together with User or User2 gives non-matching ACEs
      var otherTenant = TestHelper.CreateTenant("OtherTenant");
      var testAce = TestHelper.CreateAceWithSpecificTenant(otherTenant);
      AttachAccessTypeReadWriteDelete(testAce, true, true, true);
      Assert.That(testAce.Validate().IsValid);
      var userList = ListObjectMother.New(User, User2);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(testAce));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      // ACE with specific otherTenant should not match any AclProbe|s
      AssertIsNotInMatchingAces(userList, aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList,aclList, false);

      // If ACE does not macth, the resulting aclExpansionEntryList must be empty.
      Assert.That(aclExpansionEntryList, Is.Empty);
    }

    [Test]
    public void CurrentRoleOnly_SpecificPostitonWithOwningGroupTest ()
    {
      var testAce = TestHelper.CreateAceWithPositionAndGroupCondition(Position, GroupCondition.OwningGroup);
      AttachAccessTypeReadWriteDelete(testAce, true, true, true);
      Assert.That(testAce.Validate().IsValid);

      var userList = ListObjectMother.New(User2);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(testAce));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      // ACE with specific position should not match any AclProbe|s
      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      // Note: Without "current role only"-probing in AclExpander.GetAccessTypes, the role {"Anotha Group","Working Drone"}
      // would also give an aclExpansionEntryList entry.
      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));

      var aclExpansion = aclExpansionEntryList[0];
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansion, new[] { ReadAccessType, WriteAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[0].Role.Group,
          GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent });

      Assert.That(aclExpansion.User, Is.EqualTo(User2));
      // Note: With "current role only"-probing in AclExpander.GetAccessTypes returns only access types for the role
      // {"Anotha Group","Supreme Being"} (and not also {"Anotha Group","Working Drone"}).
      Assert.That(aclExpansion.Role, Is.EqualTo(User2.Roles[2])); //
    }



    [Test]
    public void AbstractRoleAllContributingTest ()
    {
      var ace = TestHelper.CreateAceWithAbstractRole();
      AttachAccessTypeReadWriteDelete(ace, true, true, true);
      Assert.That(ace.Validate().IsValid);
      var userList = ListObjectMother.New(User, User2);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(ace));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);
      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(User.Roles.Count + User2.Roles.Count));
    }


    [Test]
    public void AbstractRoleAllContributingDenyTest ()
    {
      var ace = TestHelper.CreateAceWithAbstractRole();
      AttachAccessTypeReadWriteDelete(ace, true, true, true);
      Assert.That(ace.Validate().IsValid);

      // In addition to AbstractRoleAllContributingTest, deny all rights again => 
      // there should be no resulting AclExpansionEntry|s.
      var aceDeny = TestHelper.CreateAceWithoutGroupCondition();
      AttachAccessTypeReadWriteDelete(aceDeny, false, false, false);
      Assert.That(aceDeny.Validate().IsValid);

      var userList = ListObjectMother.New(User, User2);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(ace, aceDeny));

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(0));
    }


    [Test]
    public void AclExpansionEntryDeniedRightsTest ()
    {
      var ace = TestHelper.CreateAceWithoutGroupCondition();
      AttachAccessTypeReadWriteDelete(ace, true, true, true);
      Assert.That(ace.Validate().IsValid);

      // Deny read and delete rights
      var aceDeny = TestHelper.CreateAceWithoutGroupCondition();
      AttachAccessTypeReadWriteDelete(aceDeny, false, true, false);
      Assert.That(aceDeny.Validate().IsValid);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(ace, aceDeny));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      foreach (AclExpansionEntry aee in aclExpansionEntryList)
      {
        Assert.That(aee.AllowedAccessTypes, Is.EquivalentTo(new[] { WriteAccessType }));
        Assert.That(aee.DeniedAccessTypes, Is.EquivalentTo(new[] { ReadAccessType, DeleteAccessType }));
      }
    }




    //--------------------------------------------------------------------------------------------------------------------------------
    // Group Tests
    //--------------------------------------------------------------------------------------------------------------------------------

    [Test]
    public void SpecificGroupTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant("OtherTenant");
      var otherTenantGroup = TestHelper.CreateGroup("GroupForOtherTenant", null, otherTenant);
      var otherTenantPosition = TestHelper.CreatePosition("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser("UserForOtherTenant", "User", "Other", "Chief", otherTenantGroup, otherTenant);
      TestHelper.CreateRole(otherTenantUser, otherTenantGroup, otherTenantPosition);

      var aceSpecificGroup = TestHelper.CreateAceWithSpecificGroup(otherTenantGroup);
      AttachAccessTypeReadWriteDelete(aceSpecificGroup, null, true, null);
      Assert.That(aceSpecificGroup.Validate().IsValid);

      var aceOwningGroup = TestHelper.CreateAceWithOwningGroup();
      AttachAccessTypeReadWriteDelete(aceOwningGroup, null, null, true);
      Assert.That(aceOwningGroup.Validate().IsValid);

      var userList = ListObjectMother.New(otherTenantUser);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceSpecificGroup, aceOwningGroup));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(2));

      // Test of specific-group-ACE: gives write-access
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions());

      // Test of owning-group-ACE (specific-group-ACE matches also): gives write-access + delete-access with condition: group-must-own
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType },
        new AclExpansionAccessConditions { OwningGroup = aclExpansionEntryList[1].Role.Group,
          GroupHierarchyCondition = GroupHierarchyCondition.This }
      );
    }

    [Test]
    public void SpecificGroupTypeTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant("OtherTenant");

      GroupType groupType1 = TestHelper.CreateGroupType("GroupType1");
      GroupType groupType2 = TestHelper.CreateGroupType("GroupType2");

      var groupWithGroupType1 = TestHelper.CreateGroup("GroupWithGroupType1", null, otherTenant, groupType1);
      var groupWithGroupType2 = TestHelper.CreateGroup("GroupWithGroupType2", null, otherTenant, groupType2);
      var anotherGroupWithGroupType1 = TestHelper.CreateGroup("AnotherGroupWithGroupType1", null, otherTenant, groupType1);

      var otherTenantPosition = TestHelper.CreatePosition("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser("UserForOtherTenant", "User", "Other", "Chief", groupWithGroupType1, otherTenant);
      TestHelper.CreateRole(otherTenantUser, groupWithGroupType1, otherTenantPosition);
      TestHelper.CreateRole(otherTenantUser, groupWithGroupType2, otherTenantPosition);
      TestHelper.CreateRole(otherTenantUser, anotherGroupWithGroupType1, otherTenantPosition);

      var aceSpecificGroupType1 = TestHelper.CreateAceWithSpecificGroupType(groupType1);
      AttachAccessTypeReadWriteDelete(aceSpecificGroupType1, null, true, null);
      Assert.That(aceSpecificGroupType1.Validate().IsValid);

      var aceSpecificGroupType2 = TestHelper.CreateAceWithSpecificGroupType(groupType2);
      AttachAccessTypeReadWriteDelete(aceSpecificGroupType2, null, null, true);
      Assert.That(aceSpecificGroupType2.Validate().IsValid);

      var userList = ListObjectMother.New(otherTenantUser);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceSpecificGroupType1, aceSpecificGroupType2));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(3));

      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions());
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { DeleteAccessType }, new AclExpansionAccessConditions());
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[2], new[] { WriteAccessType }, new AclExpansionAccessConditions());
    }


    [Test]
    public void GroupHierarchyConditionTest ()
    {
      List<AclExpansionEntry> aclExpansionEntryList =
        GetAclExpansionEntryList_UserPositionGroupSelection(User, Position, GroupCondition.OwningGroup, GroupHierarchyCondition.ThisAndParent);

      var accessTypeDefinitionsExpected = new[] { ReadAccessType, DeleteAccessType };

      var owningGroup = aclExpansionEntryList[0].Role.Group;
      var groupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;

      var accessConditions = new AclExpansionAccessConditions
        {
          OwningGroup = owningGroup, //  GroupSelection.OwningGroup => group must be owner 
          GroupHierarchyCondition = groupHierarchyCondition
        };
      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));
      Assert.That(aclExpansionEntryList[0].AllowedAccessTypes, Is.EquivalentTo(accessTypeDefinitionsExpected));
      Assert.That(aclExpansionEntryList[0].AccessConditions.OwningGroup, Is.EqualTo(owningGroup));
      Assert.That(aclExpansionEntryList[0].AccessConditions.GroupHierarchyCondition, Is.EqualTo(groupHierarchyCondition));
      Assert.That(aclExpansionEntryList[0].AccessConditions, Is.EqualTo(accessConditions));
    }


    [Test]
    public void BranchOfOwningGroupTest ()
    {
      // A 2nd tenant + user, etc
      var otherTenant = TestHelper.CreateTenant("OtherTenant");

      GroupType groupType = TestHelper.CreateGroupType("GroupType");
      GroupType groupTypeInAce = TestHelper.CreateGroupType("GroupTypeInAce");

      var groupWithGroupTypeInAce = TestHelper.CreateGroup("GroupWithGroupTypeInAce", null, otherTenant, groupTypeInAce);
      var group = TestHelper.CreateGroup("Group", groupWithGroupTypeInAce, otherTenant, groupType);

      var otherTenantPosition = TestHelper.CreatePosition("Head Honcho");
      var otherTenantUser = TestHelper.CreateUser("UserForOtherTenant", "User", "Other", "Chief", group, otherTenant);
      TestHelper.CreateRole(otherTenantUser, group, otherTenantPosition);

      var aceWithBranchOfOwningGroup = TestHelper.CreateAceWithBranchOfOwningGroup(groupTypeInAce);
      AttachAccessTypeReadWriteDelete(aceWithBranchOfOwningGroup, null, true, null);
      Assert.That(aceWithBranchOfOwningGroup.Validate().IsValid);

      // Negative test: This ACE should not match
      var aceSpecificGroupType2 = TestHelper.CreateAceWithSpecificGroupType(groupTypeInAce);
      AttachAccessTypeReadWriteDelete(aceSpecificGroupType2, null, null, true);
      Assert.That(aceSpecificGroupType2.Validate().IsValid);

      // Negative test: This ACE should not match
      var aceSpecificGroupType3 = TestHelper.CreateAceWithSpecificGroup(groupWithGroupTypeInAce);
      AttachAccessTypeReadWriteDelete(aceSpecificGroupType3, true, null, null);
      Assert.That(aceSpecificGroupType3.Validate().IsValid);

      var userList = ListObjectMother.New(otherTenantUser);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceWithBranchOfOwningGroup, aceSpecificGroupType2, aceSpecificGroupType3));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0],
        new[] { WriteAccessType },
        new AclExpansionAccessConditions  {
          OwningGroup = groupWithGroupTypeInAce,
          GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren
        }
      );
    }


    //--------------------------------------------------------------------------------------------------------------------------------
    // User Tests
    //--------------------------------------------------------------------------------------------------------------------------------

    [Test]
    public void SpecificUserOwningUserTest ()
    {
      var aceSpecificUser = TestHelper.CreateAceWithSpecificUser(User);
      AttachAccessTypeReadWriteDelete(aceSpecificUser, null, true, null);
      Assert.That(aceSpecificUser.Validate().IsValid);

      var aceOwningUser = TestHelper.CreateAceWithOwningUser();
      AttachAccessTypeReadWriteDelete(aceOwningUser, null, null, true);
      Assert.That(aceOwningUser.Validate().IsValid);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceSpecificUser, aceOwningUser));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(2));

      // Test of specific-user-ACE: gives write-access
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions());

      // Test of owning-user-ACE (specific-user-ACE matches also): gives write-access + delete-access with condition: user-must-own
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[1], new[] { WriteAccessType, DeleteAccessType },
        new AclExpansionAccessConditions
        {
          IsOwningUserRequired = true,
        }
      );
    }

    [Test]
    public void SpecificPositionTest ()
    {
      var aceSpecificPosition = TestHelper.CreateAceWithPosition(Role.Position);
      AttachAccessTypeReadWriteDelete(aceSpecificPosition, null, true, null);
      Assert.That(aceSpecificPosition.Validate().IsValid);

      // Role for User2, ACE should not match.
      var aceSpecificPosition2 = TestHelper.CreateAceWithPosition(Role2.Position);
      AttachAccessTypeReadWriteDelete(aceSpecificPosition2, null, true, null);
      Assert.That(aceSpecificPosition2.Validate().IsValid);

      var userList = ListObjectMother.New(User);
      var aclList = ListObjectMother.New(TestHelper.CreateStatefulAcl(aceSpecificPosition, aceSpecificPosition2));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList(userList, aclList, false);

      Assert.That(aclExpansionEntryList.Count, Is.EqualTo(1));

      // Test of specific-user-ACE: gives write-access
      AssertAclExpansionEntryAccessTypesAndConditions(aclExpansionEntryList[0], new[] { WriteAccessType }, new AclExpansionAccessConditions());
    }

    //--------------------------------------------------------------------------------------------------------------------------------
    // Helper Methods
    //--------------------------------------------------------------------------------------------------------------------------------

    private void WriteAclExpansionAsHtmlToStreamWriter (List<AclExpansionEntry> aclExpansion, bool outputRowCount)
    {
      string aclExpansionFileName = Path.Combine(@"c:\temp\AclExpansionTest_", Path.ChangeExtension(StringUtility.GetFileNameTimestampNow(), "html"));

      using (var streamWriter = new StreamWriter(aclExpansionFileName))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(streamWriter, true,
          new AclExpansionHtmlWriterSettings { OutputRowCount = outputRowCount });
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      }
    }


    private void AssertAclExpansionEntryAccessTypesAndConditions (AclExpansionEntry actualAclExpansionEntry, AccessTypeDefinition[] expectedAccessTypeDefinitions,
      AclExpansionAccessConditions expectedAclExpansionAccessConditions)
    {
      Assert.That(actualAclExpansionEntry.AllowedAccessTypes, Is.EquivalentTo(expectedAccessTypeDefinitions));
      Assert.That(actualAclExpansionEntry.AccessConditions, Is.EqualTo(expectedAclExpansionAccessConditions));
    }



    // Returns a list of AclExpansionEntry for the passed users and ACLs
    private List<AclExpansionEntry> GetAclExpansionEntryList (List<User> userList, List<AccessControlList> aclList)
    {
      var userFinderStub = new Mock<IAclExpanderUserFinder>();
      userFinderStub.Setup(mock => mock.FindUsers()).Returns(userList).Verifiable();

      var aclFinderStub = new Mock<IAclExpanderAclFinder>();
      aclFinderStub.Setup(mock => mock.FindAccessControlLists()).Returns(aclList).Verifiable();

      var aclExpander = new AclExpander(userFinderStub.Object, aclFinderStub.Object);

      // Retrieve the resulting list of AclExpansionEntry|s
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList();

      userFinderStub.Verify();
      aclFinderStub.Verify();
      return aclExpansionEntryList;
    }




    // Returns a list of AclExpansionEntry for the passed User, ACE with the passed Positon and passed GroupSelection 
    private List<AclExpansionEntry> GetAclExpansionEntryList_UserPositionGroupSelection (
      User user, Position position, GroupCondition groupCondition, GroupHierarchyCondition groupHierarchyCondition)
    {
      List<User> userList = new List<User>();
      userList.Add(user);

      var userFinderStub = new Mock<IAclExpanderUserFinder>();
      userFinderStub.Setup(mock => mock.FindUsers()).Returns(userList).Verifiable();

      List<AccessControlList> aclList = new List<AccessControlList>();

      var ace = TestHelper.CreateAceWithPositionAndGroupCondition(position, groupCondition);
      ace.GroupHierarchyCondition = groupHierarchyCondition;

      AttachAccessTypeReadWriteDelete(ace, true, null, true);
      Assert.That(ace.Validate().IsValid);
      aclList.Add(TestHelper.CreateStatefulAcl(ace));
      var @class = TestHelper.CreateClassDefinition("FakeClass");
      @class.StatefulAccessControlLists.AddRange(aclList);

      var aclFinderStub = new Mock<IAclExpanderAclFinder>();
      aclFinderStub.Setup(mock => mock.FindAccessControlLists()).Returns(aclList).Verifiable();

      var aclExpander = new AclExpander(userFinderStub.Object, aclFinderStub.Object);

      // Retrieve the resulting list of AclExpansionEntry|s
      var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList();

      return aclExpansionEntryList;
    }


    private void AssertIsNotInMatchingAces (List<User> userList, List<AccessControlList> aclList)
    {
      var userFinderStub = new Mock<IAclExpanderUserFinder>();
      userFinderStub.Setup(mock => mock.FindUsers()).Returns(userList).Verifiable();

      var aclFinderStub = new Mock<IAclExpanderAclFinder>();
      aclFinderStub.Setup(mock => mock.FindAccessControlLists()).Returns(aclList).Verifiable();

      var aclExpander = new AclExpander(userFinderStub.Object, aclFinderStub.Object);
      foreach (User user in userList)
      {
        foreach (Role role in user.Roles)
        {
          foreach (AccessControlList acl in aclList)
          {
            foreach (AccessControlEntry ace in acl.AccessControlEntries)
            {
              var accessTypesResult = aclExpander.AclExpansionEntryCreator.GetAccessTypes(new UserRoleAclAceCombination(role, ace));
              Assert.That(accessTypesResult.AccessTypeStatistics.IsInMatchingAces(ace), Is.False);
            }
          }
        }
      }
    }
  }

}
