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
  public class AceForGroupAndPosition : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper(TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithPositionAndGroupCondition(_companyHelper.HeadPosition, GroupCondition.OwningGroup);

      Assert.That(_ace.TenantCondition, Is.EqualTo(TenantCondition.None));
      Assert.That(_ace.GroupCondition, Is.EqualTo(GroupCondition.OwningGroup));
      Assert.That(_ace.UserCondition, Is.EqualTo(UserCondition.SpecificPosition));
      Assert.That(_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndPosition_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndOtherPosition_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndPositionInParent_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That(parentGroup, Is.Not.Null);
      TestHelper.CreateRole(user, parentGroup, _companyHelper.HeadPosition);
      TestHelper.CreateRole(user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndPositionInChild_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That(owningGroup, Is.Not.Null);
      TestHelper.CreateRole(user, childGroup, _companyHelper.HeadPosition);
      TestHelper.CreateRole(user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithPrincipalAndPositionInParent_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That(parentGroup, Is.Not.Null);
      TestHelper.CreateRole(user, parentGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithPrincipalAndOtherPositionInParent_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That(parentGroup, Is.Not.Null);
      TestHelper.CreateRole(user, parentGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithPrincipalAndPositionInChild_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That(owningGroup, Is.Not.Null);
      TestHelper.CreateRole(user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithPrincipalAndOtherPositionInChild_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That(owningGroup, Is.Not.Null);
      TestHelper.CreateRole(user, childGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithoutUser_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;

      SecurityToken token = TestHelper.CreateTokenWithoutUser();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }
  }
}
