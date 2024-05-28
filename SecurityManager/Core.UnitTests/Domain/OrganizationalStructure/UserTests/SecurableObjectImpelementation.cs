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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.UserTests
{
  [TestFixture]
  public class ISecurableObjectImplementation : UserTestBase
  {
    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject user = CreateUser();

      IObjectSecurityStrategy objectSecurityStrategy = user.GetSecurityStrategy();
      Assert.That(objectSecurityStrategy, Is.Not.Null);
      Assert.That(objectSecurityStrategy, Is.InstanceOf(typeof(DomainObjectSecurityStrategyDecorator)));
      DomainObjectSecurityStrategyDecorator domainObjectSecurityStrategyDecorator = (DomainObjectSecurityStrategyDecorator)objectSecurityStrategy;
      Assert.That(domainObjectSecurityStrategyDecorator.RequiredSecurityForStates, Is.EqualTo(RequiredSecurityForStates.None));
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject user = CreateUser();

      Assert.That(user.GetSecurityStrategy(), Is.SameAs(user.GetSecurityStrategy()));
    }

    [Test]
    public void GetSecurableType ()
    {
      ISecurableObject user = CreateUser();

      Assert.That(user.GetSecurableType(), Is.SameAs(typeof(User)));
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      User user = CreateUser();
      IDomainObjectSecurityContextFactory factory = user;

      Assert.That(factory.IsInvalid, Is.False);
      Assert.That(factory.IsNew, Is.True);
      Assert.That(factory.IsDeleted, Is.False);

      user.Delete();

      Assert.That(factory.IsInvalid, Is.True);
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation_InSubTransaction ()
    {
      User user = CreateUser();
      IDomainObjectSecurityContextFactory factory = user;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(factory.IsInvalid, Is.False);
        Assert.That(factory.IsNew, Is.True);
        Assert.That(factory.IsDeleted, Is.False);

        user.Delete();

        Assert.That(factory.IsDeleted, Is.True);
      }
    }

    [Test]
    public void CreateSecurityContext ()
    {
      User user = CreateUser();

      ISecurityContext securityContext = ((ISecurityContextFactory)user).CreateSecurityContext();
      Assert.That(Type.GetType(securityContext.Class), Is.EqualTo(user.GetPublicDomainObjectType()));
      Assert.That(securityContext.Owner, Is.EqualTo(user.UserName));
      Assert.That(securityContext.OwnerGroup, Is.EqualTo(user.OwningGroup.UniqueIdentifier));
      Assert.That(securityContext.OwnerTenant, Is.EqualTo(user.Tenant.UniqueIdentifier));
      Assert.That(securityContext.AbstractRoles, Is.Empty);
      Assert.That(securityContext.IsStateless, Is.False);
    }

    [Test]
    public void CreateSecurityContext_WithoutOwningGroup ()
    {
      User user = CreateUser();
      user.OwningGroup = null;

      ISecurityContext securityContext = ((ISecurityContextFactory)user).CreateSecurityContext();
      Assert.That(Type.GetType(securityContext.Class), Is.EqualTo(user.GetPublicDomainObjectType()));
      Assert.That(securityContext.Owner, Is.EqualTo(user.UserName));
      Assert.That(securityContext.OwnerGroup, Is.Null);
      Assert.That(securityContext.OwnerTenant, Is.EqualTo(user.Tenant.UniqueIdentifier));
      Assert.That(securityContext.AbstractRoles, Is.Empty);
      Assert.That(securityContext.IsStateless, Is.False);
    }

    [Test]
    public void CreateSecurityContext_WithoutTenant ()
    {
      User user = CreateUser();
      user.Tenant = null;

      ISecurityContext securityContext = ((ISecurityContextFactory)user).CreateSecurityContext();
      Assert.That(Type.GetType(securityContext.Class), Is.EqualTo(user.GetPublicDomainObjectType()));
      Assert.That(securityContext.Owner, Is.EqualTo(user.UserName));
      Assert.That(securityContext.OwnerGroup, Is.EqualTo(user.OwningGroup.UniqueIdentifier));
      Assert.That(securityContext.OwnerTenant, Is.Null);
      Assert.That(securityContext.AbstractRoles, Is.Empty);
      Assert.That(securityContext.IsStateless, Is.False);
    }
  }
}
