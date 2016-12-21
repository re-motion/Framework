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
  public class MixedTokens : SecurityTokenMatcherTestBase
  {
    [Test]
    public void EmptyAce_Matches ()
    {
      AccessControlEntry entry = AccessControlEntry.NewObject ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateTokenWithoutUser();

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithRoleAndAbstractRole_AceForPositionFromOwningGroup_Matches ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      Role role = TestHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = TestHelper.CreateAceWithPositionAndGroupCondition (managerPosition, GroupCondition.OwningGroup);
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateToken (user, null, group, null, new[] { TestHelper.CreateTestAbstractRole () });

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    [Test]
    public void TokenWithTenantAndAbstractRole_AceForOwningTenant_Matches ()
    {
      Tenant tenant = TestHelper.CreateTenant ("Testtenant");
      Position managerPosition = TestHelper.CreatePosition ("Manager");
      Group group = TestHelper.CreateGroup ("Testgroup", null, tenant);
      User user = CreateUser (tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant ();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher (entry);
      SecurityToken token = TestHelper.CreateToken (user, tenant, null, null, new[] { TestHelper.CreateTestAbstractRole () });

      Assert.That (matcher.MatchesToken (token), Is.True);
    }

    //[Test]
    //public void EmptyToken_AceFilter_Matches ()
    //{
    //  AccessControlEntry entry = AccessControlEntry.NewObject ();
    //  SecurityToken token = TestHelper.CreateEmptyToken ();
    //  token.SpecificAce = entry; 
    //  Assert.IsTrue (entry.MatchesToken (token));
    //}

    //[Test]
    //public void EmptyToken_AceFilter_DoesNotMatch ()
    //{
    //  AccessControlEntry entry = AccessControlEntry.NewObject ();
    //  AccessControlEntry entryOther = AccessControlEntry.NewObject ();
    //  SecurityToken token = TestHelper.CreateEmptyToken ();
    //  token.SpecificAce = entryOther;
    //  Assert.IsFalse (entry.MatchesToken (token));
    //  Assert.IsTrue (entryOther.MatchesToken (token));
    //}
  }
}
