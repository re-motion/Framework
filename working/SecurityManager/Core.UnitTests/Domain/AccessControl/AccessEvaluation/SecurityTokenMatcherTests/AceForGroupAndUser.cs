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
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForGroupAndUser : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningGroup();
      _ace.UserCondition = UserCondition.Owner;

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.OwningGroup));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.Owner));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalFromOwningUser_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User owningUser = _companyHelper.CarTeamMember;
      Group owningGroup = _companyHelper.AustrianCarTeam;
      User user = owningUser;

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void AceWithExactGroup_TokenWithPrincipalAndOtherOwningUser_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;
      User owningUser = _companyHelper.CarTeamMember;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group owningGroup = _companyHelper.AustrianCarTeam;

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithPrincipalInGroup_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      User owningUser = user;
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void AceWithGroupAndParentGroup_TokenWithPrincipalInParent_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndParent;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      User owningUser = user;
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      Group parentGroup = owningGroup.Parent;
      Assert.That (parentGroup, Is.Not.Null);
      TestHelper.CreateRole (user, parentGroup, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithPrincipalInGroup_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      User owningUser = user;
      Group owningGroup = _companyHelper.AustrianCarTeam;
      TestHelper.CreateRole (user, owningGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void AceWithGroupAndChildGroup_TokenWithPrincipalInChild_Matches ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.ThisAndChildren;
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      User owningUser = user;
      Group childGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = childGroup.Parent;
      Assert.That (owningGroup, Is.Not.Null);
      TestHelper.CreateRole (user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateToken (user, null, owningGroup, owningUser, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithoutUser_DoesNotMatch ()
    {
      _ace.GroupHierarchyCondition = GroupHierarchyCondition.This;

      SecurityToken token = TestHelper.CreateTokenWithoutUser ();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }
  }
}
