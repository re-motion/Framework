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
  public class AceForSpecificTenant : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp();

      _companyHelper = new CompanyStructureHelper(TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithSpecificTenant(_companyHelper.CompanyTenant);

      Assert.That(_ace.TenantCondition, Is.EqualTo(TenantCondition.SpecificTenant));
      Assert.That(_ace.SpecificTenant, Is.SameAs(_companyHelper.CompanyTenant));
      Assert.That(_ace.TenantHierarchyCondition, Is.EqualTo(TenantHierarchyCondition.This));
      Assert.That(_ace.GroupCondition, Is.EqualTo(GroupCondition.None));
      Assert.That(_ace.UserCondition, Is.EqualTo(UserCondition.None));
      Assert.That(_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipal_Matches ()
    {
      User principal = CreateUser(_companyHelper.CompanyTenant, null);
      SecurityToken token = TestHelper.CreateTokenWithOwningUser(principal, null);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalAndDifferentSpecificTenant_DoesNotMatch ()
    {
      User principal = CreateUser(TestHelper.CreateTenant("Tenant"), null);
      SecurityToken token = TestHelper.CreateTokenWithOwningUser(principal, null);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithPrincipalInParent_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant("Tenant");
      owningTenant.Parent = principalTenant;
      User principal = CreateUser(principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser(principal, null);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher(_ace);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant("Tenant");
      principalTenant.Parent = owningTenant;
      User principal = CreateUser(principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningUser(principal, null);

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
