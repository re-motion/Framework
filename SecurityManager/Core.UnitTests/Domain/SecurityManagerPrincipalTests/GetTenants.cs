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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Rhino.Mocks;

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

      User user = User.FindByUserName ("substituting.user");
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
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_childTenantHandle, _userHandle, null);

      Assert.That (
          principal.GetTenants (true).Select (t => t.ID),
          Is.EqualTo (new[] { _rootTenantHandle.ObjectID, _childTenantHandle.ObjectID, _grandChildTenantHandle.ObjectID }));
    }

    [Test]
    public void IncludeAbstractTenants ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_rootTenantHandle, _userHandle, null);

      Assert.That (
          principal.GetTenants (true).Select (t => t.ID),
          Is.EqualTo (new[] { _rootTenantHandle.ObjectID, _childTenantHandle.ObjectID, _grandChildTenantHandle.ObjectID }));
    }

    [Test]
    public void ExcludeAbstractTenants ()
    {
      SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_rootTenantHandle, _userHandle, null);

      Assert.That (
          principal.GetTenants (false).Select (t => t.ID), Is.EqualTo (new[] { _rootTenantHandle.ObjectID, _grandChildTenantHandle.ObjectID }));
    }

    [Test]
    public void UsesSecurityFreeSectionToAccessTenantOfUser ()
    {
      ISecurityContext userSecurityContext;
      ISecurityContext tenantSecurityContext;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var user = _userHandle.GetObject();
        userSecurityContext = ((ISecurityContextFactory) user).CreateSecurityContext();
        tenantSecurityContext = ((ISecurityContextFactory) user.Tenant).CreateSecurityContext();
      }

      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);
      securityProviderStub
          .Stub (_ => _.GetAccess (Arg.Is (userSecurityContext), Arg<ISecurityPrincipal>.Is.Anything))
          .Throw (new AssertionException ("GetAccess should not have been called."));
      securityProviderStub
          .Stub (_ => _.GetAccess (Arg.Is (tenantSecurityContext), Arg<ISecurityPrincipal>.Is.Anything))
          .Return (new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => securityProviderStub);
      serviceLocator.RegisterSingle<IPrincipalProvider> (() => new NullPrincipalProvider());
      using (new ServiceLocatorScope (serviceLocator))
      {
        SecurityManagerPrincipal principal = new SecurityManagerPrincipal (_rootTenantHandle, _userHandle, null);

        Assert.That (principal.GetTenants (true), Is.Empty);
      }
    }
  }
}