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
