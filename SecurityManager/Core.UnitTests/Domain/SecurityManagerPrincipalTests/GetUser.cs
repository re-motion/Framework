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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Development;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetUser : SecurityManagerPrincipalTestBase
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
      var userProxy = _principal.User;

      Assert.That(userProxy.ID, Is.EqualTo(_user.ID));
    }

    [Test]
    public void UsesCache ()
    {
      Assert.That(_principal.User, Is.SameAs(_principal.User));
    }

    [Test]
    public void SerializesCache ()
    {
      var deserialized = Serializer.SerializeAndDeserialize(Tuple.Create(_principal, _principal.User));
      SecurityManagerPrincipal deserialziedSecurityManagerPrincipal = deserialized.Item1;
      UserProxy deserialziedUser = deserialized.Item2;

      Assert.That(deserialziedSecurityManagerPrincipal.User, Is.SameAs(deserialziedUser));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(stub => stub.IsNull).Throws(new AssertionException("Should not be called."));

      IncrementDomainRevision();

      var securityProvider = (FakeSecurityProvider)SafeServiceLocator.Current.GetInstance<ISecurityProvider>();
      securityProvider.SetCustomSecurityProvider(securityProviderStub.Object);
      try
      {
        var refreshedInstance = _principal.GetRefreshedInstance();
        Assert.That(refreshedInstance, Is.Not.SameAs(_principal));

        var userProxy = refreshedInstance.User;

        Assert.That(userProxy.ID, Is.EqualTo(_user.ID));
      }
      finally
      {
        securityProvider.ResetCustomSecurityProvider();
      }
    }
  }
}
