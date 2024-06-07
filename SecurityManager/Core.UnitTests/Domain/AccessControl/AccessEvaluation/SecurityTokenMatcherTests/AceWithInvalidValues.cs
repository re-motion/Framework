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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceWithInvalidValues : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper(TestHelper.Transaction);
    }

    [Test]
    public void GroupHierarchyCondition_UndefinedValue ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Undefined;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(ace);
      Assert.That(
          () => matcher.MatchesToken(token),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value 'Undefined' is not a valid value for matching the 'GroupHierarchyCondition'."));
    }

    [Test]
    public void GroupHierarchyCondition_Parent ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Parent;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(ace);
      Assert.That(
          () => matcher.MatchesToken(token),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value 'Parent' is not a valid value for matching the 'GroupHierarchyCondition'."));
    }

    [Test]
    public void GroupHierarchyCondition_Children ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningGroup();
      ace.GroupHierarchyCondition = GroupHierarchyCondition.Children;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(ace);
      Assert.That(
          () => matcher.MatchesToken(token),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value 'Children' is not a valid value for matching the 'GroupHierarchyCondition'."));
    }

    [Test]
    public void TenantHierarchyCondition_UndefinedValue ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant(user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Undefined;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(ace);
      Assert.That(
          () => matcher.MatchesToken(token),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value 'Undefined' is not a valid value for matching the 'TenantHierarchyCondition'."));
    }

    [Test]
    public void TenantHierarchyCondition_Parent ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant(user, owningTenant);

      AccessControlEntry ace = TestHelper.CreateAceWithOwningTenant();
      ace.TenantHierarchyCondition = TenantHierarchyCondition.Parent;

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(ace);
      Assert.That(
          () => matcher.MatchesToken(token),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The value 'Parent' is not a valid value for matching the 'TenantHierarchyCondition'."));
    }
  }
}
