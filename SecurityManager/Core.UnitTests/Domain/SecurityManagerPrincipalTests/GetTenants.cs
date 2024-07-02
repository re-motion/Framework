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
  public class GetTenants : SecurityManagerPrincipalTestBase
  {
    private IDomainObjectHandle<Tenant> _rootTenantHandle;
    private IDomainObjectHandle<Tenant> _childTenantHandle;
    private IDomainObjectHandle<Tenant> _grandChildTenantHandle;
    private IDomainObjectHandle<User> _userHandle;

    public override void SetUp ()
    {
      base.SetUp();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();

      User user = User.FindByUserName("substituting.user");
      _userHandle = user.GetHandle();
      _rootTenantHandle = user.Tenant.GetHandle();
      _childTenantHandle = user.Tenant.Children.Single().GetHandle();
      _grandChildTenantHandle = user.Tenant.Children.Single().Children.Single().GetHandle();
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void GetTenantHierarchyFromUser ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal(_childTenantHandle, _userHandle, null, null, null, null);

      Assert.That(
          principal.GetTenants(true).Select(t => t.ID),
          Is.EqualTo(new[] { _rootTenantHandle.ObjectID, _childTenantHandle.ObjectID, _grandChildTenantHandle.ObjectID }));
    }

    [Test]
    public void IncludeAbstractTenants ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal(_rootTenantHandle, _userHandle, null, null, null, null);

      Assert.That(
          principal.GetTenants(true).Select(t => t.ID),
          Is.EqualTo(new[] { _rootTenantHandle.ObjectID, _childTenantHandle.ObjectID, _grandChildTenantHandle.ObjectID }));
    }

    [Test]
    public void ExcludeAbstractTenants ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal(_rootTenantHandle, _userHandle, null, null, null, null);

      Assert.That(
          principal.GetTenants(false).Select(t => t.ID), Is.EqualTo(new[] { _rootTenantHandle.ObjectID, _grandChildTenantHandle.ObjectID }));
    }

    [Test]
    public void UsesSecurityFreeSectionToAccessTenantOfUser ()
    {
      ISecurityContext userSecurityContext;
      ISecurityContext tenantSecurityContext;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var user = _userHandle.GetObject();
        userSecurityContext = ((ISecurityContextFactory)user).CreateSecurityContext();
        tenantSecurityContext = ((ISecurityContextFactory)user.Tenant).CreateSecurityContext();
      }

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(stub => stub.IsNull).Returns(false);
      securityProviderStub
          .Setup(_ => _.GetAccess(userSecurityContext, It.IsAny<ISecurityPrincipal>()))
          .Throws(new AssertionException("GetAccess should not have been called."));
      securityProviderStub
          .Setup(_ => _.GetAccess(tenantSecurityContext, It.IsAny<ISecurityPrincipal>()))
          .Returns(new AccessType[0]);

      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle(() => securityProviderStub.Object);
      serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
      serviceLocator.RegisterSingle(() => storageSettings);
      using (new ServiceLocatorScope(serviceLocator))
      {
        SecurityManagerPrincipal principal = new SecurityManagerPrincipal(_rootTenantHandle, _userHandle, null, null, null, null);

        Assert.That(principal.GetTenants(true), Is.Empty);
      }
    }
  }
}
