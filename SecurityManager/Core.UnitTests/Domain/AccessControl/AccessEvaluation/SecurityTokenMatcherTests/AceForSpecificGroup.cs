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
  public class AceForSpecificGroup : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper(TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithSpecificGroup(_companyHelper.AustrianProjectsDepartment);

      Assert.That(_ace.TenantCondition, Is.EqualTo(TenantCondition.None));
      Assert.That(_ace.GroupCondition, Is.EqualTo(GroupCondition.SpecificGroup));
      Assert.That(_ace.SpecificGroup, Is.SameAs(_companyHelper.AustrianProjectsDepartment));
      Assert.That(_ace.GroupHierarchyCondition, Is.EqualTo(GroupHierarchyCondition.This));
      Assert.That(_ace.UserCondition, Is.EqualTo(UserCondition.None));
      Assert.That(_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipal_Matches ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      TestHelper.CreateRole(user, _ace.SpecificGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser(user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInParent_DoesNotMatch ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group parentGroup = _ace.SpecificGroup.Parent;
      Assert.That(parentGroup, Is.Not.Null);
      TestHelper.CreateRole(user, parentGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser(user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      User user = CreateUser(_companyHelper.CompanyTenant, null);
      Group childGroup = _ace.SpecificGroup.Children[0];
      TestHelper.CreateRole(user, childGroup, _companyHelper.HeadPosition);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser(user, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithoutUser_DoesNotMatch ()
    {
      SecurityToken token = TestHelper.CreateTokenWithoutUser();

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }
  }
}
