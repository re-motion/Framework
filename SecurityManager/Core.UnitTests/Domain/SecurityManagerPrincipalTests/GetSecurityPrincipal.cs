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
  public class GetSecurityPrincipal : SecurityManagerPrincipalTestBase
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

      _user = User.FindByUserName ("substituting.user");
      _tenant = _user.Tenant;
      _substitution = _user.GetActiveSubstitutions().First (s => s.SubstitutedRole != null);

      _principal = CreateSecurityManagerPrincipal (_tenant, _user, _substitution);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Test ()
    {
      ISecurityPrincipal securityPrincipal = _principal.GetSecurityPrincipal();
      Assert.That (securityPrincipal.IsNull, Is.False);
      Assert.That (securityPrincipal.User, Is.EqualTo (_user.UserName));
      Assert.That (securityPrincipal.Role, Is.Null);
      Assert.That (securityPrincipal.SubstitutedUser, Is.EqualTo (_substitution.SubstitutedUser.UserName));
      Assert.That (securityPrincipal.SubstitutedRole.Group, Is.EqualTo (_substitution.SubstitutedRole.Group.UniqueIdentifier));
      Assert.That (securityPrincipal.SubstitutedRole.Position, Is.EqualTo (_substitution.SubstitutedRole.Position.UniqueIdentifier));
    }

    [Test]
    public void UsesCache ()
    {
      Assert.That (_principal.GetSecurityPrincipal(), Is.SameAs (_principal.GetSecurityPrincipal()));
    }

    [Test]
    public void SerializesCache ()
    {
      var deserialized = Serializer.SerializeAndDeserialize (Tuple.Create (_principal, _principal.GetSecurityPrincipal()));
      SecurityManagerPrincipal deserialziedSecurityManagerPrincipal = deserialized.Item1;
      ISecurityPrincipal deserialziedSecurityPrincipal = deserialized.Item2;

      Assert.That (deserialziedSecurityManagerPrincipal.GetSecurityPrincipal(), Is.SameAs (deserialziedSecurityPrincipal));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();
      securityProviderStub.Stub (stub => stub.IsNull).Return (false);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => securityProviderStub);
      ISecurityManagerPrincipal refreshedInstance;
      using (new ServiceLocatorScope (serviceLocator))
      {
        IncrementRevision();
        refreshedInstance = _principal.GetRefreshedInstance();
        Assert.That (refreshedInstance, Is.Not.SameAs (_principal));
      }

      ISecurityPrincipal securityPrincipal = refreshedInstance.GetSecurityPrincipal();
      Assert.That (securityPrincipal.IsNull, Is.False);
      Assert.That (securityPrincipal.User, Is.EqualTo ("substituting.user"));
    }
  }
}