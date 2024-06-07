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
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class ValidationTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Validate_IsValid ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificTenant_IsValid ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificTenant_IsNotValidWithNull ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      ace.SpecificTenant = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsSpecificTenantMissing }));
    }

    [Test]
    public void ValidateTenantHierarchyCondition_IsValid_IfSpecificTenant ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTenantHierarchyCondition_IsNotValidWithUndefined_IfSpecificTenant ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Undefined;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsTenantHierarchyConditionMissing }));
    }

    [Test]
    public void ValidateTenantHierarchyCondition_IsNotValidWithParent_IfSpecificTenant ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Parent;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsTenantHierarchyConditionOnlyParent }));
    }

    [Test]
    public void ValidateTenantHierarchyCondition_IsValid_IfOwningTenant ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningTenant();

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateTenantHierarchyCondition_IsNotValidWithUndefined_IfOwningTenant ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningTenant();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Undefined;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsTenantHierarchyConditionMissing }));
    }

    [Test]
    public void ValidateTenantHierarchyCondition_IsNotValidWithParent_IfOwningTenant ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningTenant();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Parent;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsTenantHierarchyConditionOnlyParent }));
    }

    [Test]
    public void ValidateSpecificGroup_IsValid ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificGroup_IsNotValidWithNull ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      ace.SpecificGroup = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[]{AccessControlEntryValidationError.IsSpecificGroupMissing}));
    }

    [Test]
    public void ValidateSpecificGroupType_IsValid_IfAnyGroupWithSpecificGroupType ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithSpecificGroupType(groupType);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificGroupType_IsNotValidWithNull_IfAnyGroupWithSpecificGroupType ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithSpecificGroupType(groupType);
      ace.SpecificGroupType = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsSpecificGroupTypeMissing }));
    }

    [Test]
    public void ValidateSpecificGroupType_IsValid_IfBranchOfOwningGroup ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithBranchOfOwningGroup(groupType);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificGroupType_IsNotValidWithNull_IfBranchOfOwningGroup ()
    {
      var groupType = GroupType.NewObject();
      var ace = _testHelper.CreateAceWithBranchOfOwningGroup(groupType);
      ace.SpecificGroupType = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsSpecificGroupTypeMissing }));
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsValid_IfSpecificGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsNotValidWithUndefined_IfSpecificGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Undefined;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsGroupHierarchyConditionMissing }));
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsNotValidWithParent_IfSpecificGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Parent;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyParent }));
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsNotValidWithChildren_IfSpecificGroup ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Children;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyChildren }));
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsValid_IfOwningGroup ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningGroup();

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsNotValidWithUndefined_IfOwningGroup ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Undefined;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsGroupHierarchyConditionMissing }));
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsNotValidWithParent_IfOwningGroup ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Parent;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyParent }));
    }

    [Test]
    public void ValidateGroupHierarchyCondition_IsNotValidWithChildren_IfOwningGroup ()
    {
      AccessControlEntry ace = _testHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Children;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsGroupHierarchyConditionOnlyChildren }));
    }

    [Test]
    public void ValidateSpecificUser_IsValid ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var user = _testHelper.CreateUser("TestUser", "user", "user", null, _testHelper.CreateGroup("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser(user);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificUser_IsNotValidWithNull ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var user = _testHelper.CreateUser("TestUser", "user", "user", null, _testHelper.CreateGroup("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser(user);
      ace.SpecificUser = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsSpecificUserMissing }));
    }

    [Test]
    public void ValidateSpecificPosition_IsValid ()
    {
      var ace = _testHelper.CreateAceWithPositionAndGroupCondition(_testHelper.CreatePosition("Position"), GroupCondition.None);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ValidateSpecificPosition_IsNotValidWithNull ()
    {
      var ace = _testHelper.CreateAceWithPositionAndGroupCondition(_testHelper.CreatePosition("Position"), GroupCondition.None);
      ace.SpecificPosition = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.That(result.GetErrors(), Is.EqualTo(new object[] { AccessControlEntryValidationError.IsSpecificPositionMissing }));
    }

    [Test]
    public void ValidateTenantConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.SpecificTenant = null;
        ace.Delete();

        AccessControlEntryValidationResult result = ace.Validate();

        Assert.That(result.IsValid, Is.True);
        Assert.That(ace.State.IsDeleted, Is.True);
      }
    }

    [Test]
    public void ValidateGroupConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      var group = _testHelper.CreateGroup("TestGroup", null, _testHelper.CreateTenant("TestTenant"));
      var ace = _testHelper.CreateAceWithSpecificGroup(group);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.SpecificGroup = null;
        ace.Delete();

        AccessControlEntryValidationResult result = ace.Validate();

        Assert.That(result.IsValid, Is.True);
        Assert.That(ace.State.IsDeleted, Is.True);
      }
    }

    [Test]
    public void ValidateUserConditionWhenObjectIsDeleted_DoesNotThrow ()
    {
      var tenant = _testHelper.CreateTenant("TestTenant");
      var user = _testHelper.CreateUser("TestUser", "user", "user", null, _testHelper.CreateGroup("TestGroup", null, tenant), tenant);
      var ace = _testHelper.CreateAceWithSpecificUser(user);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.SpecificUser = null;
        ace.Delete();

        AccessControlEntryValidationResult result = ace.Validate();

        Assert.That(result.IsValid, Is.True);
        Assert.That(ace.State.IsDeleted, Is.True);
      }
    }

    [Test]
    public void Commit_OneError ()
    {
      Tenant tenant = _testHelper.CreateTenant("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant(tenant);
      ace.SpecificTenant = null;
      Assert.That(
          () => ClientTransactionScope.CurrentTransaction.Commit(),
          Throws.InstanceOf<ConstraintViolationException>()
              .With.Message.EqualTo(
                  "The access control entry is in an invalid state:\r\n"
                  + "  The TenantCondition property is set to SpecificTenant, but no SpecificTenant is assigned."));
    }
  }
}
