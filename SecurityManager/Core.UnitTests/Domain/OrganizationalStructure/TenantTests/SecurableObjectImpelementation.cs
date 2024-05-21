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
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class SecurableObjectImpelementation : TenantTestBase
  {
    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");

      IObjectSecurityStrategy objectSecurityStrategy = tenant.GetSecurityStrategy();
      Assert.That(objectSecurityStrategy, Is.Not.Null);
      Assert.That(objectSecurityStrategy, Is.InstanceOf(typeof(DomainObjectSecurityStrategyDecorator)));
      DomainObjectSecurityStrategyDecorator domainObjectSecurityStrategyDecorator = (DomainObjectSecurityStrategyDecorator)objectSecurityStrategy;
      Assert.That(domainObjectSecurityStrategyDecorator.RequiredSecurityForStates, Is.EqualTo(RequiredSecurityForStates.None));
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");

      Assert.That(tenant.GetSecurityStrategy(), Is.SameAs(tenant.GetSecurityStrategy()));
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");

      Assert.That(tenant.GetSecurableType(), Is.SameAs(typeof(Tenant)));
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      IDomainObjectSecurityContextFactory factory = tenant;

      Assert.That(factory.IsInvalid, Is.False);
      Assert.That(factory.IsNew, Is.True);
      Assert.That(factory.IsDeleted, Is.False);

      tenant.Delete();

      Assert.That(factory.IsInvalid, Is.True);
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation_InSubTransaction ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      IDomainObjectSecurityContextFactory factory = tenant;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(factory.IsInvalid, Is.False);
        Assert.That(factory.IsNew, Is.True);
        Assert.That(factory.IsDeleted, Is.False);

        tenant.Delete();

        Assert.That(factory.IsDeleted, Is.True);
      }
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      tenant.Parent = TestHelper.CreateTenant("ParentTenant", "UID: ParentTenant");

      ISecurityContext securityContext = ((ISecurityContextFactory)tenant).CreateSecurityContext();
      Assert.That(Type.GetType(securityContext.Class), Is.EqualTo(tenant.GetPublicDomainObjectType()));
      Assert.That(securityContext.Owner, Is.Null);
      Assert.That(securityContext.OwnerGroup, Is.Null);
      Assert.That(securityContext.OwnerTenant, Is.EqualTo(tenant.UniqueIdentifier));
      Assert.That(securityContext.AbstractRoles, Is.Empty);
      Assert.That(securityContext.IsStateless, Is.False);
    }

    [Test]
    public void CreateSecurityContext_WithoutParent ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");

      ISecurityContext securityContext = ((ISecurityContextFactory)tenant).CreateSecurityContext();
      Assert.That(Type.GetType(securityContext.Class), Is.EqualTo(tenant.GetPublicDomainObjectType()));
      Assert.That(securityContext.Owner, Is.Null);
      Assert.That(securityContext.OwnerGroup, Is.Null);
      Assert.That(securityContext.OwnerTenant, Is.EqualTo(tenant.UniqueIdentifier));
      Assert.That(securityContext.AbstractRoles, Is.Empty);
      Assert.That(securityContext.IsStateless, Is.False);
    }
  }
}
