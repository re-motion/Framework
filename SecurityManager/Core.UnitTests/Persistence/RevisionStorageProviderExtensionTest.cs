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
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain;

namespace Remotion.SecurityManager.UnitTests.Persistence
{
  [TestFixture]
  public class RevisionStorageProviderExtensionTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      _factory = new OrganizationalStructureFactory();
    }

    [Test]
    public void Saving_OneSecurityManagerDomainObject ()
    {
      var tenant = _factory.CreateTenant();
      tenant.Name = "MyTenant";

      ClientTransactionScope.CurrentTransaction.Commit();

      var value = ClientTransaction.CreateRootTransaction().QueryManager.GetScalar(Revision.GetGetRevisionQuery(new RevisionKey()));
      Assert.That(value, Is.InstanceOf<Guid>());
      Assert.That(value, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Saving_DisacardedDomainObject ()
    {
      Tenant tenant = _factory.CreateTenant();
      tenant.Delete();

      ClientTransactionScope.CurrentTransaction.Commit();

      Assert.That(ClientTransaction.CreateRootTransaction().QueryManager.GetScalar(Revision.GetGetRevisionQuery(new RevisionKey())), Is.Null);
    }
  }
}
