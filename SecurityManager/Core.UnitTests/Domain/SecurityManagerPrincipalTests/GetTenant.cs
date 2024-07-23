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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Development;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetTenant : SecurityManagerPrincipalTestBase
  {
    private User _user;
    private Tenant _tenant;
    private SecurityManagerPrincipal _principal;

    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();

      _user = User.FindByUserName("substituting.user");
      _tenant = _user.Tenant;

      _principal = CreateSecurityManagerPrincipal(_tenant, _user, null, null);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Test ()
    {
      var tenantProxy = _principal.Tenant;

      Assert.That(tenantProxy.ID, Is.EqualTo(_tenant.ID));
    }

    [Test]
    public void UsesCache ()
    {
      Assert.That(_principal.Tenant, Is.SameAs(_principal.Tenant));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(stub => stub.IsNull).Returns(false);

      IncrementDomainRevision();

      var securityProvider = (FakeSecurityProvider)SafeServiceLocator.Current.GetInstance<ISecurityProvider>();
      securityProvider.SetCustomSecurityProvider(securityProviderStub.Object);

      try
      {
        var refreshedInstance = _principal.GetRefreshedInstance();
        Assert.That(refreshedInstance, Is.Not.SameAs(_principal));

        var tenantProxy = refreshedInstance.Tenant;

        Assert.That(tenantProxy.ID, Is.EqualTo(_tenant.ID));
      }
      finally
      {
        securityProvider.ResetCustomSecurityProvider();
      }
    }
  }
}
