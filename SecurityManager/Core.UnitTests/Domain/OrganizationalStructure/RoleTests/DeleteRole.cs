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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RoleTests
{
  [TestFixture]
  public class DeleteRole : RoleTestBase
  {
    [Test]
    public void CascadeToSubstitution ()
    {
      Tenant tenant = TestHelper.CreateTenant("TestTenant", "UID: testTenant");
      Group userGroup = TestHelper.CreateGroup("UserGroup", Guid.NewGuid().ToString(), null, tenant);
      Group roleGroup = TestHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", userGroup, tenant);
      Position position = TestHelper.CreatePosition("Position");
      Role role = TestHelper.CreateRole(user, roleGroup, position);

      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedRole = role;

      role.Delete();

      Assert.That(substitution.State.IsInvalid, Is.True);
    }
  }
}
