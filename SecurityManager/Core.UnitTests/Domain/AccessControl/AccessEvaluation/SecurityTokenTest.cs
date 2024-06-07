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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation
{
  [TestFixture]
  public class SecurityTokenTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();
      _factory = new OrganizationalStructureFactory();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    [Test]
    public void Initialize_Values ()
    {
      Tenant principalTenant = CreateTenant("principalTenant");
      Principal principal = PrincipalTestHelper.Create(principalTenant, null, new Role[0]);
      Tenant owningTenant = CreateTenant("owningTenant");
      Group owningGroup = CreateGroup("owningGroup", null, owningTenant);
      User owningUser = CreateUser("owningUser", CreateGroup("owningUserGroup", null, owningTenant), owningTenant);
      AbstractRoleDefinition abstractRole1 = AbstractRoleDefinition.NewObject(Guid.NewGuid(), "role1", 0);
      AbstractRoleDefinition abstractRole2 = AbstractRoleDefinition.NewObject(Guid.NewGuid(), "role2", 1);
      SecurityToken token = SecurityToken.Create(
          principal,
          owningTenant,
          owningGroup,
          owningUser,
          new[] { abstractRole1.GetHandle(), abstractRole2.GetHandle() });

      Assert.That(token.Principal, Is.SameAs(principal));
      Assert.That(token.OwningTenant, Is.EqualTo(owningTenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(token.OwningGroup, Is.EqualTo(owningGroup).Using(DomainObjectHandleComparer.Instance));
      Assert.That(token.OwningUser, Is.EqualTo(owningUser).Using(DomainObjectHandleComparer.Instance));
      Assert.That(token.AbstractRoles, Is.EquivalentTo(new[] { abstractRole1, abstractRole2 }).Using(DomainObjectHandleComparer.Instance));
    }

    [Test]
    public void Initialize_Empty ()
    {
      Tenant principalTenant = CreateTenant("principalTenant");
      Principal principal = PrincipalTestHelper.Create(principalTenant, null, new Role[0]);
      SecurityToken token = SecurityToken.Create(principal, null, null, null, Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

      Assert.That(token.OwningTenant, Is.Null);
      Assert.That(token.OwningGroup, Is.Null);
      Assert.That(token.OwningUser, Is.Null);
      Assert.That(token.AbstractRoles, Is.Empty);
    }

    private Tenant CreateTenant (string name)
    {
      Tenant tenant = _factory.CreateTenant();
      tenant.Name = name;

      return tenant;
    }

    private Group CreateGroup (string name, Group parent, Tenant tenant)
    {
      Group group = _factory.CreateGroup();
      group.Name = name;
      group.Parent = parent;
      group.Tenant = tenant;

      return group;
    }

    private User CreateUser (string userName, Group owningGroup, Tenant tenant)
    {
      User user = _factory.CreateUser();
      user.UserName = userName;
      user.FirstName = "First Name";
      user.LastName = "Last Name";
      user.Title = "Title";
      user.Tenant = tenant;
      user.OwningGroup = owningGroup;

      return user;
    }
  }
}
