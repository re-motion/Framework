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
  public class AceForOwningGroupAndParents : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper(TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningGroup();
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;

      Assert.That(_ace.TenantCondition, Is.EqualTo(TenantCondition.None));
      Assert.That(_ace.GroupCondition, Is.EqualTo(GroupCondition.OwningGroup));
      Assert.That(_ace.GroupHierarchyCondition, Is.EqualTo(GroupHierarchyCondition.ThisAndParent));
      Assert.That(_ace.UserCondition, Is.EqualTo(UserCondition.None));
      Assert.That(_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipal_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole(user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInParent_Matches ()
    {
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
    public void TokenWithPrincipalInGrandParent_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianCarTeam;
      Group grandParentGroup = owningGroup.Parent.Parent;
      Assert.That(grandParentGroup, Is.Not.Null);
      TestHelper.CreateRole(user, grandParentGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That(owningGroup, Is.Not.Null);
      TestHelper.CreateRole(user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithPrincipalInSibling_DoesNotMatch ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group branchRoot = _companyHelper.AustrianDivsion;
      Group userGroup = branchRoot.Children[0];
      Group owningGroup = branchRoot.Children[1];
      TestHelper.CreateRole(user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }
  }
}
