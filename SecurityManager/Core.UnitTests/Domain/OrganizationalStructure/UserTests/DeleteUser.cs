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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class DeleteUser : UserTestBase
  {
    [Test]
    public void CascadeToAccessControlEntry ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        dbFixtures.CreateEmptyDomain();
        var tenant = testHelper.CreateTenant("TestTenant");
        var owningGroup = testHelper.CreateGroup("group", null, tenant);
        var user = testHelper.CreateUser("user", null, "user", null, owningGroup, tenant);
        var ace = testHelper.CreateAceWithSpecificUser(user);
        ClientTransaction.Current.Commit();

        user.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(ace.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void CascadeToRole ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", "UID: testTenant");
      Group userGroup = TestHelper.CreateGroup("UserGroup", Guid.NewGuid().ToString(), null, tenant);
      Group roleGroup = TestHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", userGroup, tenant);
      Position position = TestHelper.CreatePosition("Position");
      Role role = TestHelper.CreateRole(user, roleGroup, position);

      user.Delete();

      Assert.That(role.State.IsInvalid, Is.True);
    }

    [Test]
    public void CascadeToSubstitution_FromSubstitutingUser ()
    {
      User substitutingUser = TestHelper.CreateUser("user", null, "Lastname", null, null, null);
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutingUser = substitutingUser;

      substitutingUser.Delete();

      Assert.That(substitution.State.IsInvalid, Is.True);
    }

    [Test]
    public void CascadeToSubstitution_FromSubstitutedUser ()
    {
      User substitutedUser = TestHelper.CreateUser("user", null, "Lastname", null, null, null);
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedUser = substitutedUser;

      substitutedUser.Delete();

      Assert.That(substitution.State.IsInvalid, Is.True);
    }
  }
}
