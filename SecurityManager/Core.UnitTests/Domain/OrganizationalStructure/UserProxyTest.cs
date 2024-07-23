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
  public class UserProxyTest : DomainTest
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
      var user = CreateUser();
      var proxy = UserProxy.Create(user);

      Assert.That(proxy.ID, Is.EqualTo(user.ID));
      Assert.That(proxy.UniqueIdentifier, Is.EqualTo(((IBusinessObjectWithIdentity)user).UniqueIdentifier));
      Assert.That(proxy.DisplayName, Is.EqualTo(((IBusinessObjectWithIdentity)user).DisplayName));
    }

    [Test]
    public void Equals_EqualObject_True ()
    {
      var user = CreateUser();
      var proxy1 = UserProxy.Create(user);
      var proxy2 = UserProxy.Create(user);

      Assert.That(proxy1.Equals(proxy2), Is.True);
      Assert.That(proxy2.Equals(proxy1), Is.True);
    }

    [Test]
    public void Equals_SameObject_True ()
    {
      var user = CreateUser();
      var proxy1 = UserProxy.Create(user);

// ReSharper disable EqualExpressionComparison
      Assert.That(proxy1.Equals(proxy1), Is.True);
// ReSharper restore EqualExpressionComparison
    }

    [Test]
    public void Equals_Null_False ()
    {
      var user = CreateUser();
      var proxy1 = UserProxy.Create(user);

      Assert.That(proxy1.Equals(null), Is.False);
    }

    [Test]
    public void Equals_OtherObject_False ()
    {
      var proxy1 = UserProxy.Create(CreateUser());
      var proxy2 = UserProxy.Create(CreateUser());

      Assert.That(proxy1.Equals(proxy2), Is.False);
    }

    [Test]
    public void Equals_OtherType_False ()
    {
      var proxy1 = UserProxy.Create(CreateUser());

      Assert.That(proxy1.Equals("other"), Is.False);
    }

    [Test]
    public void GetHashcode_EqualObject_SameHashcode ()
    {
      var user = CreateUser();
      var proxy1 = UserProxy.Create(user);
      var proxy2 = UserProxy.Create(user);

      Assert.That(proxy2.GetHashCode(), Is.EqualTo(proxy1.GetHashCode()));
    }

    private User CreateUser ()
    {
      var tenant = _testHelper.CreateTenant("TheTenant", "UID");
      var group = _testHelper.CreateGroup("TheGroup", null, tenant);
      return _testHelper.CreateUser("UserName", "FN", "LN", null, group, tenant);
    }
  }
}
