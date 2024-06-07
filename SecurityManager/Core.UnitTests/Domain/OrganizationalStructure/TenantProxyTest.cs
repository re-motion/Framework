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
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class TenantProxyTest : DomainTest
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
      var tenant = CreateTenant();
      var proxy = TenantProxy.Create(tenant);

      Assert.That(proxy.ID, Is.EqualTo(tenant.ID));
      Assert.That(proxy.UniqueIdentifier, Is.EqualTo(((IBusinessObjectWithIdentity)tenant).UniqueIdentifier));
      Assert.That(proxy.DisplayName, Is.EqualTo(((IBusinessObjectWithIdentity)tenant).DisplayName));
    }

    [Test]
    public void Serialization ()
    {
      var proxy = TenantProxy.Create(CreateTenant());

      var deserialized = Serializer.SerializeAndDeserialize(proxy);

      Assert.That(deserialized.ID, Is.EqualTo(proxy.ID));
      Assert.That(deserialized.UniqueIdentifier, Is.EqualTo(proxy.UniqueIdentifier));
      Assert.That(deserialized.DisplayName, Is.EqualTo(proxy.DisplayName));
    }

    [Test]
    public void Equals_EqualObject_True ()
    {
      var tenant = CreateTenant();
      var proxy1 = TenantProxy.Create(tenant);
      var proxy2 = TenantProxy.Create(tenant);

      Assert.That(proxy1.Equals(proxy2), Is.True);
      Assert.That(proxy2.Equals(proxy1), Is.True);
    }

    [Test]
    public void Equals_SameObject_True ()
    {
      var tenant = CreateTenant();
      var proxy1 = TenantProxy.Create(tenant);

      // ReSharper disable EqualExpressionComparison
      Assert.That(proxy1.Equals(proxy1), Is.True);
      // ReSharper restore EqualExpressionComparison
    }

    [Test]
    public void Equals_Null_False ()
    {
      var user = CreateTenant();
      var proxy1 = TenantProxy.Create(user);

      Assert.That(proxy1.Equals(null), Is.False);
    }

    [Test]
    public void Equals_OtherObject_False ()
    {
      var proxy1 = TenantProxy.Create(CreateTenant());
      var proxy2 = TenantProxy.Create(CreateTenant());

      Assert.That(proxy1.Equals(proxy2), Is.False);
    }

    [Test]
    public void Equals_OtherType_False ()
    {
      var proxy1 = TenantProxy.Create(CreateTenant());

      Assert.That(proxy1.Equals("other"), Is.False);
    }

    [Test]
    public void GetHashcode_EqualObject_SameHashcode ()
    {
      var tenant = CreateTenant();
      var proxy1 = TenantProxy.Create(tenant);
      var proxy2 = TenantProxy.Create(tenant);

      Assert.That(proxy2.GetHashCode(), Is.EqualTo(proxy1.GetHashCode()));
    }

    private Tenant CreateTenant ()
    {
      return _testHelper.CreateTenant("TheTenant", "UID");
    }
  }
}
