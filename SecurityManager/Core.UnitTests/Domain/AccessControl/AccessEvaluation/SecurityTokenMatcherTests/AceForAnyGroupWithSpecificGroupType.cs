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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation.SecurityTokenMatcherTests
{
  [TestFixture]
  public class AceForAnyGroupWithSpecificGroupType : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithSpecificGroupType(_companyHelper.DepartmentGroupType);

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.AnyGroupWithSpecificGroupType));
      Assert.That (_ace.SpecificGroupType, Is.SameAs (_companyHelper.DepartmentGroupType));
      Assert.That (_ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipalInGroupMatchingGroupType_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithoutPrincipalUserButWithPrincipalRole_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianProjectsDepartment;
      Role userRole = TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = SecurityToken.Create (
          PrincipalTestHelper.Create (_companyHelper.CompanyTenant, null, new[] { userRole }),
          null,
          userGroup,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInGroupWithOtherGroupType_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      Group userGroup = _companyHelper.AustrianCarTeam;
      Group owningGroup = _companyHelper.AustrianProjectsDepartment;
      TestHelper.CreateRole (user, userGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, owningGroup);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }

    [Test]
    public void TokenWithoutPrincipalRoles_DoesNotMatch ()
    {
      SecurityToken token = SecurityToken.Create (
          PrincipalTestHelper.Create (_companyHelper.CompanyTenant, _companyHelper.CarTeamMember, new Role[0]),
          null,
          _companyHelper.AustrianCarTeam,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }

    [Test]
    public void TokenWithoutPrincipalAndWithoutOwningGroup_DoesNotMatch ()
    {
      SecurityToken token = TestHelper.CreateTokenWithoutUser();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }
  }
}
