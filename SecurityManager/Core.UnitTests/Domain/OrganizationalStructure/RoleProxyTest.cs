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
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class RoleProxyTest : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Create ()
    {
      var role = CreateRole();
      var proxy = RoleProxy.Create(role);

      Assert.That(proxy.ID, Is.EqualTo(role.ID));
      Assert.That(proxy.UniqueIdentifier, Is.EqualTo(((IBusinessObjectWithIdentity)role).UniqueIdentifier));
      Assert.That(proxy.DisplayName, Is.EqualTo(((IBusinessObjectWithIdentity)role).DisplayName));
    }

    [Test]
    public void Equals_EqualObject_True ()
    {
      var role = CreateRole();
      var proxy1 = RoleProxy.Create(role);
      var proxy2 = RoleProxy.Create(role);

      Assert.That(proxy1.Equals(proxy2), Is.True);
      Assert.That(proxy2.Equals(proxy1), Is.True);
    }

    [Test]
    public void Equals_SameObject_True ()
    {
      var role = CreateRole();
      var proxy1 = RoleProxy.Create(role);

      // ReSharper disable EqualExpressionComparison
      Assert.That(proxy1.Equals(proxy1), Is.True);
      // ReSharper restore EqualExpressionComparison
    }

    [Test]
    public void Equals_Null_False ()
    {
      var role = CreateRole();
      var proxy1 = RoleProxy.Create(role);

      Assert.That(proxy1.Equals(null), Is.False);
    }

    [Test]
    public void Equals_OtherObject_False ()
    {
      var proxy1 = RoleProxy.Create(CreateRole());
      var proxy2 = RoleProxy.Create(CreateRole());

      Assert.That(proxy1.Equals(proxy2), Is.False);
    }

    [Test]
    public void Equals_OtherType_False ()
    {
      var proxy1 = RoleProxy.Create(CreateRole());

      Assert.That(proxy1.Equals("other"), Is.False);
    }

    [Test]
    public void GetHashcode_EqualObject_SameHashcode ()
    {
      var role = CreateRole();
      var proxy1 = RoleProxy.Create(role);
      var proxy2 = RoleProxy.Create(role);

      Assert.That(proxy2.GetHashCode(), Is.EqualTo(proxy1.GetHashCode()));
    }

    private Role CreateRole ()
    {
      var tenant = _testHelper.CreateTenant("TheTenant", "UID");
      var group = _testHelper.CreateGroup("TheGroup", null, tenant);
      var user = _testHelper.CreateUser("UserName", "FN", "LN", null, group, tenant);
      var position = _testHelper.CreatePosition("ThePosition");

      return  _testHelper.CreateRole(user, group, position);
    }
  }
}
