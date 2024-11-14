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
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation
{
  [TestFixture]
  public class PrincipalTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();

      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      _testHelper = testHelper;
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Initialize_WithRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant("tenant");
      User user = _testHelper.CreateUser("userName", null, "lastName", null, null, null);
      Role[] roles = new[] { CreateRole(tenant), CreateRole(tenant) };
      Principal principal = PrincipalTestHelper.Create(tenant, user, roles);

      Assert.That(principal.Tenant, Is.EqualTo(tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(principal.User, Is.EqualTo(user).Using(DomainObjectHandleComparer.Instance));
      Assert.That(principal.Roles, Is.EquivalentTo(roles).Using(PrincipalRoleComparer.Instance));
      Assert.That(principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_WithoutRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant("tenant");
      User user = _testHelper.CreateUser("userName", null, "lastName", null, null, null);
      Principal principal = PrincipalTestHelper.Create(tenant, user, new Role[0]);

      Assert.That(principal.Tenant, Is.EqualTo(tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(principal.User, Is.EqualTo(user).Using(DomainObjectHandleComparer.Instance));
      Assert.That(principal.Roles, Is.Empty);
      Assert.That(principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_CopiesRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant("tenant");
      var roles = new[]
                  {
                      new PrincipalRole(
                          new DomainObjectHandle<Position>(new ObjectID(typeof(Position), Guid.NewGuid())),
                          new DomainObjectHandle<Group>(new ObjectID(typeof(Group), Guid.NewGuid())))
                  };
      Principal principal = new Principal(tenant.GetHandle(), null, roles);

      Assert.That(principal.Roles, Is.Not.SameAs(roles));
      Assert.That(principal.Roles, Is.EquivalentTo(roles));
      Assert.That(principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_WithTenantAndWithoutUserAndWithRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant("tenant");
      Role[] roles = new[] { CreateRole(tenant), CreateRole(tenant) };
      Principal principal = PrincipalTestHelper.Create(tenant, null, roles);

      Assert.That(principal.Tenant, Is.EqualTo(tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(principal.User, Is.Null);
      Assert.That(principal.Roles, Is.EquivalentTo(roles).Using(PrincipalRoleComparer.Instance));
      Assert.That(principal.IsNull, Is.False);
    }

    [Test]
    public void Initialize_WithTenantAndWithoutUserAndWithoutRoles ()
    {
      Tenant tenant = _testHelper.CreateTenant("tenant");
      Principal principal = PrincipalTestHelper.Create(tenant, null, new Role[0]);

      Assert.That(principal.Tenant, Is.EqualTo(tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(principal.User, Is.Null);
      Assert.That(principal.Roles, Is.Empty);
      Assert.That(principal.IsNull, Is.False);
    }

    [Test]
    public void GetNullPrincipal ()
    {
      Principal principal = Principal.Null;

      Assert.That(principal.Tenant, Is.Null);
      Assert.That(principal.User, Is.Null);
      Assert.That(principal.Roles, Is.Empty);
      Assert.That(principal.IsNull, Is.True);
    }

    private Role CreateRole (Tenant tenant)
    {
      return _testHelper.CreateRole(
          null,
          _testHelper.CreateGroup(Guid.NewGuid().ToString(), null, tenant),
          _testHelper.CreatePosition(Guid.NewGuid().ToString()));
    }
  }
}
