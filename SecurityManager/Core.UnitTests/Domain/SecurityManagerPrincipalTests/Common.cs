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
using Moq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class Common : SecurityManagerPrincipalTestBase
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
    public void Get_Current_NotInitialized ()
    {
      Assert.That(SecurityManagerPrincipal.Current.IsNull, Is.True);
    }

    [Test]
    public void SetAndGet_Current ()
    {
      User user = User.FindByUserName("substituting.user");

      var principal = CreateSecurityManagerPrincipal(user.Tenant, user, null, null);
      SecurityManagerPrincipal.Current = principal;
      Assert.That(SecurityManagerPrincipal.Current, Is.SameAs(principal));
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      User user = User.FindByUserName("substituting.user");

      var principal = CreateSecurityManagerPrincipal(user.Tenant, user, null, null);
      SecurityManagerPrincipal.Current = principal;
      Assert.That(SecurityManagerPrincipal.Current, Is.SameAs(principal));

      ThreadRunner.Run(
          delegate
          {
            using (SafeContext.Instance.OpenSafeContextBoundary())
            {
              using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
              {
                User otherUser = User.FindByUserName("group1/user1");
                var otherPrincipal = CreateSecurityManagerPrincipal(otherUser.Tenant, otherUser, null, null);

                Assert.That(SecurityManagerPrincipal.Current.IsNull, Is.True);
                SecurityManagerPrincipal.Current = otherPrincipal;
                Assert.That(SecurityManagerPrincipal.Current, Is.SameAs(otherPrincipal));
              }
            }
          });

      Assert.That(SecurityManagerPrincipal.Current, Is.SameAs(principal));
    }

    [Test]
    public void GetValuesInNewTransaction ()
    {
      var user = User.FindByUserName("substituting.user");
      var tenant = user.Tenant;
      var roles = user.Roles.Take(2).ToArray();
      var substitution = user.GetActiveSubstitutions().First();

      var principal = CreateSecurityManagerPrincipal(tenant, user, roles, substitution);

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That(principal.Tenant.ID, Is.EqualTo(tenant.ID));
        Assert.That(principal.Tenant, Is.Not.SameAs(tenant));

        Assert.That(principal.User.ID, Is.EqualTo(user.ID));
        Assert.That(principal.User, Is.Not.SameAs(user));

        Assert.That(principal.Roles.Select(r => r.ID), Is.EqualTo(roles.Select(r => r.ID)));
        Assert.That(principal.Roles, Is.Not.EquivalentTo(roles));

        Assert.That(principal.Substitution.ID, Is.EqualTo(substitution.ID));
        Assert.That(principal.Substitution, Is.Not.SameAs(substitution));
      }
    }

    [Test]
    public void Test_IsNull ()
    {
      User user = User.FindByUserName("substituting.user");
      Tenant tenant = user.Tenant;

      ISecurityManagerPrincipal principal = CreateSecurityManagerPrincipal(tenant, user, null, null);

      Assert.That(principal.IsNull, Is.False);
    }

    [Test]
    public void ActiveSecurityProviderAddsSecurityClientTransactionExtension ()
    {
      User user = User.FindByUserName("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(stub => stub.IsNull).Returns(false);
      securityProviderStub
          .Setup(_ => _.GetAccess(It.IsAny<ISecurityContext>(), It.IsAny<ISecurityPrincipal>()))
          .Returns(new AccessType[0]);

      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => securityProviderStub.Object);
      serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
      serviceLocator.RegisterSingle(() => storageSettings);
      using (new ServiceLocatorScope(serviceLocator))
      {
        var principal = CreateSecurityManagerPrincipal(tenant, user, null, substitution);

        //Must test for observable effect
        Assert.That(principal.GetActiveSubstitutions(), Is.Empty);
      }
    }

    [Test]
    public void NullSecurityProviderDoesNotAddSecurityClientTransactionExtension ()
    {
      Assert.That(SafeServiceLocator.Current.GetInstance<ISecurityProvider>().IsNull, Is.True);

      User user = User.FindByUserName("substituting.user");
      Tenant tenant = user.Tenant;
      Substitution substitution = user.GetActiveSubstitutions().First();

      var principal = CreateSecurityManagerPrincipal(tenant, user, null, substitution);

      //Must test for observable effect
      Assert.That(principal.GetActiveSubstitutions(), Is.Not.Empty);
    }
  }
}
