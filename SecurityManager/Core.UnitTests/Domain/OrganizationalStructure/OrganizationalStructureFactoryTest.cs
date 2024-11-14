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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class OrganizationalStructureFactoryTest : DomainTest
  {
    private IOrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new OrganizationalStructureFactory();
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    [Test]
    public void CreateTenant ()
    {
      Assert.That(_factory.CreateTenant(), Is.InstanceOf(typeof(Tenant)));
    }

    [Test]
    public void CreateGroup ()
    {
      Assert.That(_factory.CreateGroup(), Is.InstanceOf(typeof(Group)));
    }

    [Test]
    public void CreateUser ()
    {
      Assert.That(_factory.CreateUser(), Is.InstanceOf(typeof(User)));
    }

    [Test]
    public void CreatePosition ()
    {
      Assert.That(_factory.CreatePosition(), Is.InstanceOf(typeof(Position)));
    }

    [Test]
    public void GetTenantType ()
    {
      Assert.That(_factory.GetTenantType(), Is.SameAs(typeof(Tenant)));
    }

    [Test]
    public void GetGroupType ()
    {
      Assert.That(_factory.GetGroupType(), Is.SameAs(typeof(Group)));
    }

    [Test]
    public void GetUserType ()
    {
      Assert.That(_factory.GetUserType(), Is.SameAs(typeof(User)));
    }

    [Test]
    public void GetPositionType ()
    {
      Assert.That(_factory.GetPositionType(), Is.SameAs(typeof(Position)));
    }
  }
}
