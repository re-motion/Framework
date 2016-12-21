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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class FindTenant : TenantTestBase
  {
    private DatabaseFixtures _dbFixtures;
    private ObjectID _expectedTenantID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      Tenant tenant = _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      _expectedTenantID = tenant.ID;
    }

    [Test]
    public void FindAll ()
    {
      var tenants = Tenant.FindAll().ToArray();

      Assert.That (tenants.Length, Is.EqualTo (2));
      Assert.That (tenants[1].ID, Is.EqualTo (_expectedTenantID));
    }

    [Test]
    public void FindByUnqiueIdentifier_ValidTenant ()
    {
      Tenant foundTenant = Tenant.FindByUnqiueIdentifier ("UID: testTenant");

      Assert.That (foundTenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingTenant ()
    {
      Tenant foundTenant = Tenant.FindByUnqiueIdentifier ("UID: NotExistingTenant");

      Assert.That (foundTenant, Is.Null);
    }
  }
}