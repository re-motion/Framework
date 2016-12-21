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
  public class AceForBranchOfOwningGroupAndPosition : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithPositionAndGroupCondition (_companyHelper.HeadPosition, GroupCondition.BranchOfOwningGroup);
      _ace.SpecificGroupType = _companyHelper.DivisionGroupType;

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.None));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.BranchOfOwningGroup));
      Assert.That (_ace.SpecificGroupType, Is.SameAs (_companyHelper.DivisionGroupType));
      Assert.That (_ace.GroupHierarchyCondition, Is.EqualTo (GroupHierarchyCondition.Undefined));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.SpecificPosition));
      Assert.That (_ace.SpecificPosition, Is.SameAs (_companyHelper.HeadPosition));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPositionInBranchRoot_Matches ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianDivsion, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, _companyHelper.AustrianProjectsDepartment);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithOtherPositionInBranchRoot_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianDivsion, _companyHelper.MemberPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, _companyHelper.AustrianDivsion);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }

    [Test]
    public void TokenWithPositionBelowBranchRoot_DoesNotMatch ()
    {
      User user = CreateUser (_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole (user, _companyHelper.AustrianProjectsDepartment, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningGroup (user, _companyHelper.AustrianProjectsDepartment);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }
  }
}
