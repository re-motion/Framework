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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class DeleteGroup : GroupTestBase
  {
    [Test]
    public void DeleteGroup_WithAccessControlEntry ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        dbFixtures.CreateEmptyDomain();
        var group = testHelper.CreateGroup("group", null, testHelper.CreateTenant("tenant"));
        var ace = testHelper.CreateAceWithSpecificGroup(group);
        ClientTransaction.Current.Commit();

        group.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(ace.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void DeleteGroup_WithRole ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        Tenant tenant = testHelper.CreateTenant("TestTenant", "UID: testTenant");
        Group userGroup = testHelper.CreateGroup("UserGroup", Guid.NewGuid().ToString(), null, tenant);
        Group roleGroup = testHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, tenant);
        User user = testHelper.CreateUser("user", "Firstname", "Lastname", "Title", userGroup, tenant);
        Position position = testHelper.CreatePosition("Position");
        Role role = testHelper.CreateRole(user, roleGroup, position);

        roleGroup.Delete();

        Assert.That(role.State.IsInvalid, Is.True);
      }
    }
  }
}
