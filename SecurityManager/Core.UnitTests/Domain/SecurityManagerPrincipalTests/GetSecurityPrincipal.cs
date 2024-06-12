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
using Remotion.Security;
using Remotion.Security.Development;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.SecurityManagerPrincipalTests
{
  [TestFixture]
  public class GetSecurityPrincipal : SecurityManagerPrincipalTestBase
  {
    private User _user;
    private Tenant _tenant;
    private Role[] _roles;
    private Substitution _substitution;

    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();

      _user = User.FindByUserName("substituting.user");
      _tenant = _user.Tenant;
      _roles = _user.Roles.Skip(1).Take(2).ToArray();
      Assert.That(_roles.Length, Is.EqualTo(2));
      _substitution = _user.GetActiveSubstitutions().First(s => s.SubstitutedRole != null);
      Assert.That(_substitution, Is.Not.Null);

    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Test ()
    {
      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, null);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();
      Assert.That(securityPrincipal.IsNull, Is.False);
      Assert.That(securityPrincipal.User, Is.EqualTo(_user.UserName));

      Assert.That(securityPrincipal.Roles, Is.Not.Null);
      Assert.That(securityPrincipal.Roles.Count, Is.EqualTo(2));
      Assert.That(securityPrincipal.Roles[0].Group, Is.EqualTo(_roles[0].Group.UniqueIdentifier));
      Assert.That(securityPrincipal.Roles[0].Position, Is.EqualTo(_roles[0].Position.UniqueIdentifier));
      Assert.That(securityPrincipal.Roles[1].Group, Is.EqualTo(_roles[1].Group.UniqueIdentifier));
      Assert.That(securityPrincipal.Roles[1].Position, Is.EqualTo(_roles[1].Position.UniqueIdentifier));
    }

    [Test]
    public void Test_WithSubstitution_AndWithImplicitSubstutedUserAndRoles ()
    {
      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();

      Assert.That(securityPrincipal.SubstitutedUser, Is.EqualTo(_substitution.SubstitutedUser.UserName));
      Assert.That(securityPrincipal.SubstitutedRoles.Count, Is.EqualTo(1));
      Assert.That(securityPrincipal.SubstitutedRoles[0].Group, Is.EqualTo(_substitution.SubstitutedRole.Group.UniqueIdentifier));
      Assert.That(securityPrincipal.SubstitutedRoles[0].Position, Is.EqualTo(_substitution.SubstitutedRole.Position.UniqueIdentifier));
    }

    [Test]
    public void Test_WithSubstitutionAndExplicitSubstitutedUserAndRoles_UsesExplicitData ()
    {
      var substitutedUser = User.FindByUserName("group1/user1");
      var substitutedRoles = substitutedUser.Roles.Take(1).ToArray();
      Assert.That(substitutedRoles.Length, Is.EqualTo(1));

      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution, substitutedUser, substitutedRoles);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();

      Assert.That(securityPrincipal.SubstitutedUser, Is.EqualTo(substitutedUser.UserName));
      Assert.That(securityPrincipal.SubstitutedRoles.Count, Is.EqualTo(1));
      Assert.That(securityPrincipal.SubstitutedRoles[0].Group, Is.EqualTo(substitutedRoles[0].Group.UniqueIdentifier));
      Assert.That(securityPrincipal.SubstitutedRoles[0].Position, Is.EqualTo(substitutedRoles[0].Position.UniqueIdentifier));
    }

    [Test]
    public void Test_WithSubstitutionAndExplicitSubstitutedUser_UsesExplicitData ()
    {
      var substitutedUser = User.FindByUserName("group1/user1");

      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution, substitutedUser, null);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();

      Assert.That(securityPrincipal.SubstitutedUser, Is.EqualTo(substitutedUser.UserName));
      Assert.That(securityPrincipal.SubstitutedRoles, Is.Null);
    }

    [Test]
    public void Test_WithSubstitutionAndExplicitSubstitutedRoles_UsesExplicitData ()
    {
      var substitutedUser = User.FindByUserName("group1/user1");
      var substitutedRoles = substitutedUser.Roles.Take(1).ToArray();
      Assert.That(substitutedRoles.Length, Is.EqualTo(1));

      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution, null, substitutedRoles);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();

      Assert.That(securityPrincipal.SubstitutedUser, Is.Null);
      Assert.That(securityPrincipal.SubstitutedRoles.Count, Is.EqualTo(1));
      Assert.That(securityPrincipal.SubstitutedRoles[0].Group, Is.EqualTo(substitutedRoles[0].Group.UniqueIdentifier));
      Assert.That(securityPrincipal.SubstitutedRoles[0].Position, Is.EqualTo(substitutedRoles[0].Position.UniqueIdentifier));
    }

    [Test]
    public void Test_WithSubstitutionAndExplicitSubstitutedRolesEmpty_UsesExplicitData ()
    {
      var substitutedUser = User.FindByUserName("group1/user1");
      var substitutedRoles = substitutedUser.Roles.Take(1).ToArray();
      Assert.That(substitutedRoles.Length, Is.EqualTo(1));

      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution, null, new Role[0]);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();

      Assert.That(securityPrincipal.SubstitutedUser, Is.Null);
      Assert.That(securityPrincipal.SubstitutedRoles, Is.Empty);
    }

    [Test]
    public void Test_WithMinimumData ()
    {
      var principal = CreateSecurityManagerPrincipal(_tenant, _user, null, null);
      ISecurityPrincipal securityPrincipal = principal.GetSecurityPrincipal();
      Assert.That(securityPrincipal.IsNull, Is.False);
      Assert.That(securityPrincipal.User, Is.EqualTo(_user.UserName));
      Assert.That(securityPrincipal.Roles, Is.Null);
      Assert.That(securityPrincipal.SubstitutedUser, Is.Null);
    }

    [Test]
    public void UsesCache ()
    {
      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution);
      Assert.That(principal.GetSecurityPrincipal(), Is.SameAs(principal.GetSecurityPrincipal()));
    }

    [Test]
    public void UsesSecurityFreeSection ()
    {
      var principal = CreateSecurityManagerPrincipal(_tenant, _user, _roles, _substitution);
      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub.Setup(stub => stub.IsNull).Returns(false);

      IncrementDomainRevision();

      var securityProvider = (FakeSecurityProvider)SafeServiceLocator.Current.GetInstance<ISecurityProvider>();
      securityProvider.SetCustomSecurityProvider(securityProviderStub.Object);
      try
      {
        var refreshedInstance = principal.GetRefreshedInstance();
        Assert.That(refreshedInstance, Is.Not.SameAs(principal));

        ISecurityPrincipal securityPrincipal = refreshedInstance.GetSecurityPrincipal();
        Assert.That(securityPrincipal.IsNull, Is.False);
        Assert.That(securityPrincipal.User, Is.EqualTo("substituting.user"));
        Assert.That(securityPrincipal.Roles, Is.Not.Empty);
        Assert.That(securityPrincipal.SubstitutedUser, Is.Not.Null);
      }
      finally
      {
        securityProvider.ResetCustomSecurityProvider();
      }
    }
  }
}
