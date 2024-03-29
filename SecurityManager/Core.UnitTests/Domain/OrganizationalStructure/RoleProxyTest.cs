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
using Remotion.Development.UnitTesting;
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
    public void Serialization ()
    {
      var role = CreateRole();
      var proxy = RoleProxy.Create(role);

      var deserialized = Serializer.SerializeAndDeserialize(proxy);

      Assert.That(deserialized.ID, Is.EqualTo(proxy.ID));
      Assert.That(deserialized.UniqueIdentifier, Is.EqualTo(proxy.UniqueIdentifier));
      Assert.That(deserialized.DisplayName, Is.EqualTo(proxy.DisplayName));
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
