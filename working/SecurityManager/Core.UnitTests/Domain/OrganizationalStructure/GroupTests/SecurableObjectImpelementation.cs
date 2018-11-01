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
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class SecurableObjectImpelementation : GroupTestBase
  {
    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject group = CreateGroup();

      IObjectSecurityStrategy objectSecurityStrategy = group.GetSecurityStrategy();
      Assert.That (objectSecurityStrategy, Is.Not.Null);
      Assert.IsInstanceOf (typeof (DomainObjectSecurityStrategyDecorator), objectSecurityStrategy);
      DomainObjectSecurityStrategyDecorator domainObjectSecurityStrategyDecorator = (DomainObjectSecurityStrategyDecorator) objectSecurityStrategy;
      Assert.That (domainObjectSecurityStrategyDecorator.RequiredSecurityForStates, Is.EqualTo (RequiredSecurityForStates.None));
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject group = CreateGroup();

      Assert.That (@group.GetSecurityStrategy(), Is.SameAs (@group.GetSecurityStrategy()));
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject group = CreateGroup();

      Assert.That (@group.GetSecurableType(), Is.SameAs (typeof (Group)));
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Group group = CreateGroup();
      IDomainObjectSecurityContextFactory factory = group;

      Assert.That (factory.IsInvalid, Is.False);
      Assert.That (factory.IsNew, Is.True);
      Assert.That (factory.IsDeleted, Is.False);

      group.Delete();

      Assert.That (factory.IsInvalid, Is.True);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Group group = CreateGroup();
      group.Parent = TestHelper.CreateGroup ("ParentGroup", "UID: ParentGroup", null, group.Tenant);

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext();
      Assert.That (Type.GetType (securityContext.Class), Is.EqualTo (@group.GetPublicDomainObjectType()));
      Assert.That (securityContext.Owner, Is.Null);
      Assert.That (securityContext.OwnerGroup, Is.EqualTo (@group.UniqueIdentifier));
      Assert.That (securityContext.OwnerTenant, Is.EqualTo (@group.Tenant.UniqueIdentifier));
      Assert.That (securityContext.AbstractRoles, Is.Empty);
      Assert.That (securityContext.IsStateless, Is.False);
    }

    [Test]
    public void CreateSecurityContext_WithoutTenant ()
    {
      Group group = CreateGroup();
      group.Tenant = null;

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext();
      Assert.That (Type.GetType (securityContext.Class), Is.EqualTo (@group.GetPublicDomainObjectType()));
      Assert.That (securityContext.Owner, Is.Null);
      Assert.That (securityContext.OwnerGroup, Is.EqualTo (@group.UniqueIdentifier));
      Assert.That (securityContext.OwnerTenant, Is.Null);
      Assert.That (securityContext.AbstractRoles, Is.Empty);
      Assert.That (securityContext.IsStateless, Is.False);
    }

    [Test]
    public void CreateSecurityContext_WithoutParent ()
    {
      Group group = CreateGroup();
      group.Parent = null;

      ISecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext();
      Assert.That (Type.GetType (securityContext.Class), Is.EqualTo (@group.GetPublicDomainObjectType()));
      Assert.That (securityContext.Owner, Is.Null);
      Assert.That (securityContext.OwnerGroup, Is.EqualTo (@group.UniqueIdentifier));
      Assert.That (securityContext.OwnerTenant, Is.EqualTo (@group.Tenant.UniqueIdentifier));
      Assert.That (securityContext.AbstractRoles, Is.Empty);
      Assert.That (securityContext.IsStateless, Is.False);
    }

  }
}