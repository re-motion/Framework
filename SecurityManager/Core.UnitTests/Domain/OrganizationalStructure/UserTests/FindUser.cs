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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class FindUser : UserTestBase
  {
    private DatabaseFixtures _dbFixtures;
    private IDomainObjectHandle<Tenant> _expectedTenantHandle;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      _dbFixtures = new DatabaseFixtures();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.CreateRootTransaction());
      _expectedTenantHandle = tenant.GetHandle();
    }

    [Test]
    public void FindByUserName_ValidUser ()
    {
      User foundUser = User.FindByUserName("test.user");

      Assert.That(foundUser.UserName, Is.EqualTo("test.user"));
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      User foundUser = User.FindByUserName("not.existing");

      Assert.That(foundUser, Is.Null);
    }

    [Test]
    public void Find_UsersByTenantID ()
    {
      var users = User.FindByTenant(_expectedTenantHandle);

      Assert.That(users.Count(), Is.EqualTo(6));
    }
  }
}
