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
  public class MixedAces : SecurityTokenMatcherTestBase
  {
    [Test]
    public void AceForOwningTenantAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant("Testtenant");
      Group group = TestHelper.CreateGroup("Testgroup", null, tenant);
      User user = CreateUser(tenant, group);
      AccessControlEntry entry = TestHelper.CreateAceWithOwningTenant();
      entry.SpecificAbstractRole = TestHelper.CreateTestAbstractRole();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher(entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningTenant(user, tenant);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }

    [Test]
    public void AceForPositionFromOwningGroupAndAbstractRole_DoesNotMatch ()
    {
      Tenant tenant = TestHelper.CreateTenant("Testtenant");
      Position managerPosition = TestHelper.CreatePosition("Manager");
      Group group = TestHelper.CreateGroup("Testgroup", null, tenant);
      User user = CreateUser(tenant, group);
      Role role = TestHelper.CreateRole(user, group, managerPosition);
      AccessControlEntry entry = TestHelper.CreateAceWithPositionAndGroupCondition(managerPosition, GroupCondition.OwningGroup);
      entry.SpecificAbstractRole = TestHelper.CreateTestAbstractRole();
      SecurityTokenMatcher matcher = new SecurityTokenMatcher(entry);
      SecurityToken token = TestHelper.CreateTokenWithOwningGroup(user, group);

      Assert.That(matcher.MatchesToken(token), Is.False);
    }
  }
}
