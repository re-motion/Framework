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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.Data.UnitTesting.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class FindGroup : GroupTestBase
  {
    private readonly QueryableComparer _queryableComparer 
        = new QueryableComparer ((actual, exptected) => Assert.That (actual, Is.EqualTo (exptected)));

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
    public void FindByUnqiueIdentifier_ValidGroup ()
    {
      Group foundGroup = Group.FindByUnqiueIdentifier ("UID: testGroup");

      Assert.That (foundGroup.UniqueIdentifier, Is.EqualTo ("UID: testGroup"));
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingGroup ()
    {
      Group foundGroup = Group.FindByUnqiueIdentifier ("UID: NotExistingGroup");

      Assert.That (foundGroup, Is.Null);
    }

    [Test]
    public void Find_GroupsByTenantID ()
    {
      var expected = from g in QueryFactory.CreateLinqQuery<Group>()
                     where g.Tenant.ID == _expectedTenantHandle.ObjectID
                     orderby g.Name, g.ShortName
                     select g;

      var actual = Group.FindByTenant (_expectedTenantHandle);

      _queryableComparer.Compare (expected, actual);

      Assert.That (actual.Count(), Is.EqualTo (9));
    }
  }
}