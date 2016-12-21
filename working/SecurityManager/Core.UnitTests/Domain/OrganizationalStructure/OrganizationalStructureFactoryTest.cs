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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class OrganizationalStructureFactoryTest : DomainTest
  {
    private IOrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new OrganizationalStructureFactory();
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void CreateTenant ()
    {
      Assert.That (_factory.CreateTenant (), Is.InstanceOf (typeof (Tenant)));
    }

    [Test]
    public void CreateGroup ()
    {
      Assert.That (_factory.CreateGroup (), Is.InstanceOf (typeof (Group)));
    }

    [Test]
    public void CreateUser ()
    {
      Assert.That (_factory.CreateUser (), Is.InstanceOf (typeof (User)));
    }

    [Test]
    public void CreatePosition ()
    {
      Assert.That (_factory.CreatePosition (), Is.InstanceOf (typeof (Position)));
    }

    [Test]
    public void GetTenantType ()
    {
      Assert.That (_factory.GetTenantType(), Is.SameAs (typeof (Tenant)));
    }

    [Test]
    public void GetGroupType ()
    {
      Assert.That (_factory.GetGroupType (), Is.SameAs (typeof (Group)));
    }

    [Test]
    public void GetUserType ()
    {
      Assert.That (_factory.GetUserType (), Is.SameAs (typeof (User)));
    }

    [Test]
    public void GetPositionType ()
    {
      Assert.That (_factory.GetPositionType (), Is.SameAs (typeof (Position)));
    }
  }
}
