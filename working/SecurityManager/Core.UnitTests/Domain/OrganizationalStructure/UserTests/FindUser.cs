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

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      _expectedTenantHandle = tenant.GetHandle();
    }

    [Test]
    public void FindByUserName_ValidUser ()
    {
      User foundUser = User.FindByUserName ("test.user");

      Assert.That (foundUser.UserName, Is.EqualTo ("test.user"));
    }

    [Test]
    public void FindByUserName_NotExistingUser ()
    {
      User foundUser = User.FindByUserName ("not.existing");

      Assert.That (foundUser, Is.Null);
    }

    [Test]
    public void Find_UsersByTenantID ()
    {
      var users = User.FindByTenant (_expectedTenantHandle);

      Assert.That (users.Count(), Is.EqualTo (6));
    }
  }
}
