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
  public class AceForBranchOfOwningGroup : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper(TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithBranchOfOwningGroup(_companyHelper.DivisionGroupType);

      Assert.That(_ace.TenantCondition, Is.EqualTo(TenantCondition.None));
      Assert.That(_ace.GroupCondition, Is.EqualTo(GroupCondition.BranchOfOwningGroup));
      Assert.That(_ace.SpecificGroupType, Is.SameAs(_companyHelper.DivisionGroupType));
      Assert.That(_ace.GroupHierarchyCondition, Is.EqualTo(GroupHierarchyCondition.Undefined));
      Assert.That(_ace.UserCondition, Is.EqualTo(UserCondition.None));
      Assert.That(_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipalInOwningGroup_OwningGroupIsBelowBranchRoot_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInOwningGroup_OwningGroupIsBranchRoot_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianDivsion;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInParentGroupWithGroupType_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group userGroup = _companyHelper.AustrianDivsion;
      Assert.That(owningGroup.Parent, Is.SameAs(userGroup));
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInSiblingGroup_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group userGroup = _companyHelper.AustrianFinanceDepartment;
      Assert.That(owningGroup.Parent, Is.SameAs(userGroup.Parent));
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInChildOfSiblingGroup_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group userGroup = _companyHelper.AustrianAccountingTeam;
      Assert.That(owningGroup.Parent, Is.SameAs(userGroup.Parent.Parent));
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalAboveBranch_DoesNotMatch ()
    {
      _ace.SpecificGroupType = _companyHelper.DepartmentGroupType;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianHumanResourcesDepartment;
      Group userGroup = _companyHelper.AustrianFinanceDepartment;
      Assert.That(owningGroup.GroupType, Is.SameAs(_ace.SpecificGroupType));
      Assert.That(userGroup.GroupType, Is.SameAs(_ace.SpecificGroupType));
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithOwningGroupWithoutGroupType_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianHumanResourcesDepartment;
      owningGroup.GroupType = null;
      Group userGroup = _companyHelper.AustrianFinanceDepartment;
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithOwningGroupInBranchWithoutGroupType_DoesNotMatch ()
    {
      _companyHelper.AustrianDivsion.GroupType = null;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianHumanResourcesDepartment;
      Group userGroup = owningGroup;
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithoutOwningGroup_DoesNotMatch ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole(user, _companyHelper.AustrianProjectsDepartment, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }
  }
}
