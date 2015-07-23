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
  public class AceForOwningTenantAndParents : SecurityTokenMatcherTestBase
  {
    private CompanyStructureHelper _companyHelper;
    private AccessControlEntry _ace;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _companyHelper = new CompanyStructureHelper (TestHelper.Transaction);

      _ace = TestHelper.CreateAceWithOwningTenant ();
      _ace.TenantHierarchyCondition = TenantHierarchyCondition.ThisAndParent;

      Assert.That (_ace.TenantCondition, Is.EqualTo (TenantCondition.OwningTenant));
      Assert.That (_ace.TenantHierarchyCondition, Is.EqualTo (TenantHierarchyCondition.ThisAndParent));
      Assert.That (_ace.GroupCondition, Is.EqualTo (GroupCondition.None));
      Assert.That (_ace.UserCondition, Is.EqualTo (UserCondition.None));
      Assert.That (_ace.SpecificAbstractRole, Is.Null);
    }

    [Test]
    public void TokenWithPrincipal_Matches ()
    {
      User principal = CreateUser (_companyHelper.CompanyTenant, null);
      Tenant owningTenant = _companyHelper.CompanyTenant;
      
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInParent_Matches ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant ("Tenant");
      owningTenant.Parent = principalTenant;
      User principal = CreateUser (principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInGrandParent_Matches ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant parentTenant = TestHelper.CreateTenant ("Parent");
      owningTenant.Parent = parentTenant;
      Tenant grandParentTenant = TestHelper.CreateTenant ("GrangParent");
      parentTenant.Parent = grandParentTenant;
      User principal = CreateUser (grandParentTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithPrincipalInChild_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant principalTenant = TestHelper.CreateTenant ("Tenant");
      principalTenant.Parent = owningTenant;
      User principal = CreateUser (principalTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }

    [Test]
    public void TokenWithPrincipalInSibling_DoesNotMatch ()
    {
      Tenant owningTenant = _companyHelper.CompanyTenant;
      Tenant parentTenant = TestHelper.CreateTenant ("Parent");
      owningTenant.Parent = parentTenant;
      Tenant siblingTenant = TestHelper.CreateTenant ("Sibling");
      siblingTenant.Parent = parentTenant;
      User principal = CreateUser (siblingTenant, null);

      SecurityToken token = TestHelper.CreateTokenWithOwningTenant (principal, owningTenant);

      SecurityTokenMatcher matcher = new SecurityTokenMatcher (_ace);

      Assert.That (matcher.MatchesToken (token), Is.False);
    }
  }
}
