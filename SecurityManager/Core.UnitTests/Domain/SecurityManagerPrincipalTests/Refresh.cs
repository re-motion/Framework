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
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Refresh : SecurityManagerPrincipalTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void SameDomainRevisionDoesNotChangeTransaction ()
    {
      var user = User.FindByUserName("substituting.user");
      var tenant = user.Tenant;

      var principal = CreateSecurityManagerPrincipal(tenant, user, null, null);

      var refreshedInstance = principal.GetRefreshedInstance();

      Assert.That(refreshedInstance, Is.SameAs(principal));
    }

    [Test]
    public void NewDomainRevisionResetsData ()
    {
      var user = User.FindByUserName("substituting.user");
      var helper = new OrganizationalStructure.OrganizationalStructureTestHelper(ClientTransaction.Current);
      var user2 = helper.CreateUser("DomainRevisionTestUser", "FN", "LN", null, user.OwningGroup, user.Tenant);
      ClientTransaction.Current.Commit();
      var tenant = user2.Tenant;

      var principal = CreateSecurityManagerPrincipal(tenant, user2, null, null);

      var oldUser = principal.User;
      var oldTenant = principal.Tenant;

      ClientTransaction.Current.Commit();
      IncrementDomainRevision();

      var refreshedInstance = principal.GetRefreshedInstance();

      Assert.That(refreshedInstance, Is.Not.SameAs(principal));
      Assert.That(refreshedInstance.User.ID, Is.EqualTo(oldUser.ID));
      Assert.That(refreshedInstance.Tenant.ID, Is.EqualTo(oldTenant.ID));
    }

    [Test]
    public void SameUserRevisionDoesNotChangeTransaction ()
    {
      var user = User.FindByUserName("substituting.user");
      var tenant = user.Tenant;

      var principal = CreateSecurityManagerPrincipal(tenant, user, null, null);

      var refreshedInstance = principal.GetRefreshedInstance();

      Assert.That(refreshedInstance, Is.SameAs(principal));
    }

    [Test]
    public void NewUserRevisionResetsData ()
    {
      var user = User.FindByUserName("substituting.user");
      var helper = new OrganizationalStructure.OrganizationalStructureTestHelper(ClientTransaction.Current);
      var user2 = helper.CreateUser("UserRevisionTestUser", "FN", "LN", null, user.OwningGroup, user.Tenant);
      ClientTransaction.Current.Commit();
      var tenant = user2.Tenant;

      var principal = CreateSecurityManagerPrincipal(tenant, user2, null, null);

      var oldUser = principal.User;
      var oldTenant = principal.Tenant;

      ClientTransaction.Current.Commit();
      IncrementUserRevision(user2.UserName);

      var refreshedInstance = principal.GetRefreshedInstance();

      Assert.That(refreshedInstance, Is.Not.SameAs(principal));
      Assert.That(refreshedInstance.User.ID, Is.EqualTo(oldUser.ID));
      Assert.That(refreshedInstance.Tenant.ID, Is.EqualTo(oldTenant.ID));
    }
  }
}