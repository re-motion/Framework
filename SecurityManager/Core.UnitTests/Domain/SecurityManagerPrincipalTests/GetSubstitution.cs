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
  public class GetSubstitution : SecurityManagerPrincipalTestBase
  {
    private User _user;
    private Tenant _tenant;
    private Substitution _substitution;
    private SecurityManagerPrincipal _principal;

    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();

      _user = User.FindByUserName("substituting.user");
      _tenant = _user.Tenant;
      _substitution = _user.GetActiveSubstitutions().First();

      _principal = CreateSecurityManagerPrincipal(_tenant, _user, null, _substitution);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Test ()
    {
      var substitutionProxy = _principal.Substitution;

      Assert.That(substitutionProxy.ID, Is.EqualTo(_substitution.ID));
    }

    [Test]
    public void UsesCache ()
    {
      Assert.That(_principal.Substitution, Is.SameAs(_principal.Substitution));
    }

    [Test]
    public void SerializesCache ()
    {
      var deserialized = Serializer.SerializeAndDeserialize(Tuple.Create(_principal, _principal.Substitution));
      SecurityManagerPrincipal deserialziedSecurityManagerPrincipal = deserialized.Item1;
      SubstitutionProxy deserialziedSubstitution = deserialized.Item2;

      Assert.That(deserialziedSecurityManagerPrincipal.Substitution, Is.SameAs(deserialziedSubstitution));
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

        var substitutionProxy = refreshedInstance.Substitution;

        Assert.That(substitutionProxy.ID, Is.EqualTo(_substitution.ID));
      }
      finally
      {
        securityProvider.ResetCustomSecurityProvider();
      }
    }
  }
}
