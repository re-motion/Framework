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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class OnCommitTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void ClearSpecificTenant ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        Assert.That(ace.SpecificTenant, Is.Not.Null);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificTenant, Is.Null);
      }
    }

    [Test]
    public void ClearTenantHierarchyCondition ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.TenantCondition = TenantCondition.None;

        Assert.That(ace.TenantHierarchyCondition, Is.Not.EqualTo(TenantHierarchyCondition.Undefined));
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.TenantHierarchyCondition, Is.EqualTo(TenantHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearTenantHierarchyCondition_IfOwningTenant ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.TenantHierarchyCondition, Is.Not.EqualTo(TenantHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearTenantHierarchyCondition_IfSpecificTenant ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.TenantCondition = TenantCondition.SpecificTenant;

        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.TenantHierarchyCondition, Is.Not.EqualTo(TenantHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void ClearSpecificGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        Assert.That(ace.SpecificGroup, Is.Not.Null);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificGroup, Is.Null);
      }
    }

    [Test]
    public void ClearGroupHierarchyCondition ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.None;

        Assert.That(ace.GroupHierarchyCondition, Is.Not.EqualTo(GroupHierarchyCondition.Undefined));
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.GroupHierarchyCondition, Is.EqualTo(GroupHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearGroupHierarchyCondition_IfOwningGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.GroupHierarchyCondition, Is.Not.EqualTo(GroupHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void DoNotClearGroupHierarchyCondition_IfSpecificGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.SpecificGroup;

        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.GroupHierarchyCondition, Is.Not.EqualTo(GroupHierarchyCondition.Undefined));
      }
    }

    [Test]
    public void ClearSpecificGroupType ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithSpecificGroupType(groupType);
      ace.GroupCondition = GroupCondition.BranchOfOwningGroup;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.None;

        Assert.That(ace.SpecificGroupType, Is.Not.Null);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificGroupType, Is.Null);
      }
    }

    [Test]
    public void DoNotClearSpecificGroupType_IfAnyGroupWithSpecificGroupType ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithSpecificGroupType(groupType);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.AnyGroupWithSpecificGroupType;

        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificGroupType, Is.Not.Null);
      }
    }

    [Test]
    public void DoNotClearSpecificGroupType_IfBranchOfOwningGroup ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithSpecificGroupType(groupType);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.BranchOfOwningGroup;

        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificGroupType, Is.Not.Null);
      }
    }

    [Test]
    public void ClearSpecificUser ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var user = _testHelper.CreateUser("TestUser", "user", "user", null, _testHelper.CreateGroup("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser(user);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.That(ace.SpecificUser, Is.Not.Null);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificUser, Is.Null);
      }
    }

    [Test]
    public void ClearSpecificPosition ()
    {
      var ace = _testHelper.CreateAceWithPositionAndGroupCondition(_testHelper.CreatePosition("Position"), GroupCondition.None);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.That(ace.SpecificPosition, Is.Not.Null);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That(ace.SpecificPosition, Is.Null);
      }
    }

    [Test]
    public void DoNotAccessTenantConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.TenantCondition = TenantCondition.OwningTenant;

        Assert.That(ace.SpecificTenant, Is.Not.Null);
        ace.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    [Test]
    public void DoNotAccessGroupConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.GroupCondition = GroupCondition.OwningGroup;

        Assert.That(ace.SpecificGroup, Is.Not.Null);
        ace.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    [Test]
    public void DoNotAccessUserConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var user = _testHelper.CreateUser("TestUser", "user", "user", null, _testHelper.CreateGroup("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser(user);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.UserCondition = UserCondition.Owner;

        Assert.That(ace.SpecificUser, Is.Not.Null);
        ace.Delete();
        ClientTransactionScope.CurrentTransaction.Commit();
      }
    }

    [Test]
    public void AceHasChanged_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      var acl = _testHelper.CreateStatelessAcl(classDefinition);
      var ace = _testHelper.CreateAceWithOwningUser();
      acl.AccessControlEntries.Add(ace);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        ace.RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void AceIsDeleted_RegistersClassForCommit ()
    {
      var classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      var acl = _testHelper.CreateStatelessAcl(classDefinition);
      var ace = _testHelper.CreateAceWithOwningUser();
      acl.AccessControlEntries.Add(ace);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        ace.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }
  }
}
