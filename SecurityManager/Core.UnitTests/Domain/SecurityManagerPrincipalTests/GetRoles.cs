﻿// This file is part of re-strict (www.re-motion.org)
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
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetRoles : SecurityManagerPrincipalTestBase
  {
    private User _user;
    private Tenant _tenant;
    private Role[] _roles;
    private SecurityManagerPrincipal _principal;

    public override void SetUp ()
    {
      base.SetUp();

      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();

      _user = User.FindByUserName("test.user");
      _tenant = _user.Tenant;
      _roles = _user.Roles.Skip(1).Take(2).ToArray();
      Assert.That(_roles.Length, Is.EqualTo(2));
      _principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, null);
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
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

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => securityProviderStub);
      ISecurityManagerPrincipal refreshedInstance;
      using (new ServiceLocatorScope(serviceLocator))
      {
        IncrementDomainRevision();
        refreshedInstance = _principal.GetRefreshedInstance();
        Assert.That(refreshedInstance, Is.Not.SameAs(_principal));
      }

      var roleProxies = refreshedInstance.Roles;

      Assert.That(roleProxies.Select(r=>r.ID), Is.EqualTo(_roles.Select(r=>r.ID)));
    }
  }
}