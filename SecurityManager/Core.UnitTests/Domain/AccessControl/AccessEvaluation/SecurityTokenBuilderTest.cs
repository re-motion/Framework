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
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation
{
  [TestFixture]
  public class SecurityTokenBuilderTest : DomainTest
  {
    private SecurityTokenBuilder _securityTokenBuilder;
    private SecurityPrincipalRepository _securityPrincipalRepository;
    private ClientTransactionScope _clientTransactionScope;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      var userRevisionProvider = new UserRevisionProvider();
      _securityPrincipalRepository = new SecurityPrincipalRepository(userRevisionProvider);

      var userNamesRevisionProvider = new UserNamesRevisionProvider();
      _securityTokenBuilder = new SecurityTokenBuilder(
          _securityPrincipalRepository,
          new SecurityContextRepository(new RevisionProvider(), userNamesRevisionProvider));

      _clientTransactionScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
    }

    public override void TearDown ()
    {
      _clientTransactionScope.Leave();

      base.TearDown();
    }

    [Test]
    public void Create_WithValidPrincipal_WithRolesNull ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreateTestPrincipal();
      Assert.That(principal.Roles, Is.Null);

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant, Is.EqualTo(user.Tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(token.Principal.Roles, Is.Not.Empty);
      Assert.That(token.Principal.Roles, Is.EquivalentTo(user.Roles).Using(PrincipalRoleComparer.Instance));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithRolesEmpty ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("test.user");
      principalStub.Setup(stub => stub.Roles).Returns(new ISecurityPrincipalRole[0]);

      SecurityContext context = CreateContext();

      SecurityToken token = _securityTokenBuilder.CreateToken(principalStub.Object, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant, Is.EqualTo(user.Tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithRoles ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("test.user");
      var princialRole1Stub = new Mock<ISecurityPrincipalRole>();
      princialRole1Stub.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialRole1Stub.Setup(stub => stub.Position).Returns("UID: Official");
      var princialRole2Stub = new Mock<ISecurityPrincipalRole>();
      princialRole2Stub.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialRole2Stub.Setup(stub => stub.Position).Returns("UID: Manager");
      principalStub.Setup(stub => stub.Roles).Returns(new[] { princialRole1Stub.Object, princialRole2Stub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var principalUser = token.Principal.User.GetObject();
      Assert.That(principalUser.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant, Is.EqualTo(principalUser.Tenant).Using(DomainObjectHandleComparer.Instance));
      Assert.That(token.Principal.Roles.Count, Is.EqualTo(2));
      Assert.That(
          token.Principal.Roles.Select(r => Tuple.Create(r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo(new[] { Tuple.Create("UID: testGroup", "UID: Manager"), Tuple.Create("UID: testGroup", "UID: Official") }));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithOneSubstitutedRole_MatchingSubstitionWithRole_UsesSubstitutedRole ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns((string)null);
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns("UID: Official");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles.Count, Is.EqualTo(1));
      Assert.That(
          token.Principal.Roles.Select(r => Tuple.Create(r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo(new[] { Tuple.Create("UID: testGroup", "UID: Official") }));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithSubstitutedRolesEmpty_HasNoRoles ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns((string)null);
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new ISecurityPrincipalRole[0]);

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithMultipleSubstitutedRoles_DoesNotMatch ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns((string)null);
      var princialSubstitutedRoleStub1 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub1.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub1.Setup(stub => stub.Position).Returns("UID: Official");
      var princialSubstitutedRoleStub2 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub2.Setup(stub => stub.Group).Returns("UID: testOwningGroup");
      princialSubstitutedRoleStub2.Setup(stub => stub.Position).Returns("UID: Manager");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub1.Object, princialSubstitutedRoleStub2.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithTenantMismatch_HasNoRoles ()
    {
      var principalUser = _securityPrincipalRepository.GetUser("User.Tenant2");
      var substitution = principalUser.GetActiveSubstitutions().Single();
      substitution.SubstitutedRole = substitution.SubstitutedUser.Roles.First(r => r.User.Tenant != principalUser.Tenant);
      principalUser.RootTransaction.Commit();
      Assert.That(substitution.IsActive, Is.True, "Substitution must be unchanged.");

      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns(principalUser.UserName);
      principalStub.Setup(stub => stub.SubstitutedUser).Returns((string)null);
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns(substitution.SubstitutedRole.Group.UniqueIdentifier);
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns(substitution.SubstitutedRole.Position.UniqueIdentifier);
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.Not.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithSubstitutedRolesNull_UsesSubstitutedUserAndAllRolesOfSubstitutedUser ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns((IReadOnlyList<ISecurityPrincipalRole>)null);

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Not.Empty);
      Assert.That(token.Principal.Roles, Is.EquivalentTo(user.Roles).Using(PrincipalRoleComparer.Instance));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithSubstitutedRolesEmpty_UsesSubstitutedUserButHasNoRoles ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new ISecurityPrincipalRole[0]);

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithOneSubstitutedRole_MatchingSubstitionWithRole_UsesSubstitutedUserAndRole ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns("UID: Official");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles.Count, Is.EqualTo(1));
      Assert.That(
          token.Principal.Roles.Select(r => Tuple.Create(r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo(new[] { Tuple.Create("UID: testGroup", "UID: Official") }));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithOneSubstitutedRole_WouldMatchRoleOnlyInOtherSubstitution_UsesSubstitutedUserButUserHasNoRoles ()
    {
      var principalUser = _securityPrincipalRepository.GetUser("substituting.user");
      using (principalUser.RootTransaction.EnterNonDiscardingScope())
      {
        var testHelper = new OrganizationalStructureTestHelper(principalUser.RootTransaction);
        var otherUser = testHelper.CreateUser("otherUser", "First", "Last", null, principalUser.OwningGroup, principalUser.Tenant);
        var testRootGroup = Group.FindByUnqiueIdentifier("UID: rootGroup");
        var managerPosition = Position.FindAll().Single(p => p.UniqueIdentifier == "UID: Manager");
        var role2 = testHelper.CreateRole(otherUser, testRootGroup, managerPosition);
        var substitution2 = Substitution.NewObject();
        substitution2.SubstitutedRole = role2;
        substitution2.SubstitutedUser = otherUser;
        substitution2.SubstitutingUser = principalUser;
        principalUser.RootTransaction.Commit();
        Assert.That(substitution2.IsActive, Is.True, "Substitution must be unchanged.");
      }

      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns("UID: rootGroup");
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns("UID: Manager");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void
        Create_WithValidPrincipal_WithSubstitutedUser_WithMultipleSubstitutedRoles_MatchesUserSubstition_UsesSubstitutedUserAndReducesSetOfRolesToContainOnlySubstitutedRoles ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");

      var princialSubstitutedRoleStub1 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub1.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub1.Setup(stub => stub.Position).Returns("UID: Official");

      var princialSubstitutedRoleStub2 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub2.Setup(stub => stub.Group).Returns("UID: testOwningGroup");
      princialSubstitutedRoleStub2.Setup(stub => stub.Position).Returns("UID: Manager");

      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub1.Object, princialSubstitutedRoleStub2.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles.Count, Is.EqualTo(2));
      Assert.That(
          token.Principal.Roles.Select(r => Tuple.Create(r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo(new[] { Tuple.Create("UID: testGroup", "UID: Official"), Tuple.Create("UID: testOwningGroup", "UID: Manager") }));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void
        Create_WithValidPrincipal_WithSubstitutedUser_WithMultipleSubstitutedRoles_MatchesUserSubstition_UsesSubstitutedUserAndReducesSetOfRolesToContainOnlyRolesOfSubstitutedUser ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");

      var princialSubstitutedRoleStub1 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub1.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub1.Setup(stub => stub.Position).Returns("UID: Official");

      var princialSubstitutedRoleStub2 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub2.Setup(stub => stub.Group).Returns("UID: testOwningGroup");
      princialSubstitutedRoleStub2.Setup(stub => stub.Position).Returns("UID: Manager");

      var princialSubstitutedRoleStub3 = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub3.Setup(stub => stub.Group).Returns("UID: rootGroup");
      princialSubstitutedRoleStub3.Setup(stub => stub.Position).Returns("UID: Manager");

      principalStub
          .Setup(stub => stub.SubstitutedRoles)
          .Returns(new[] { princialSubstitutedRoleStub1.Object, princialSubstitutedRoleStub2.Object, princialSubstitutedRoleStub3.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That(user.UserName, Is.EqualTo("test.user"));
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles.Count, Is.EqualTo(2));
      Assert.That(
          token.Principal.Roles.Select(r => Tuple.Create(r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo(new[] { Tuple.Create("UID: testGroup", "UID: Official"), Tuple.Create("UID: testOwningGroup", "UID: Manager") }));
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInactiveSubstitution ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns((string)null);
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns("UID: Manager");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedUser ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("notexisting.user");

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedRoleFromGroup_ThrowsAccessControlException ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns("UID: notexisting.group");
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns("UID: Official");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      Assert.That(
          () => _securityTokenBuilder.CreateToken(principal, context),
          Throws.TypeOf<AccessControlException>().With.Message.EqualTo("The group 'UID: notexisting.group' could not be found."));
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedRoleFromPosition_ThrowsAccessControlException ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("substituting.user");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");
      var princialSubstitutedRoleStub = new Mock<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Setup(stub => stub.Group).Returns("UID: testGroup");
      princialSubstitutedRoleStub.Setup(stub => stub.Position).Returns("UID: notexisting.position");
      principalStub.Setup(stub => stub.SubstitutedRoles).Returns(new[] { princialSubstitutedRoleStub.Object });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      Assert.That(
          () => _securityTokenBuilder.CreateToken(principal, context),
          Throws.TypeOf<AccessControlException>().With.Message.EqualTo("The position 'UID: notexisting.position' could not be found."));
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithTenantMismatch_HasNoUserAndNoRoles ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns("User.Tenant2");
      principalStub.Setup(stub => stub.SubstitutedUser).Returns("test.user");

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub.Object;

      SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant, Is.Not.Null);
      Assert.That(token.Principal.Tenant.GetObject().UniqueIdentifier, Is.Not.EqualTo("UID: testTenant"));
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithNullPrincipal ()
    {
      SecurityContext context = CreateContext();
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.IsNull).Returns(true);

      SecurityToken token = _securityTokenBuilder.CreateToken(principalStub.Object, context);

      Assert.That(token.Principal.User, Is.Null);
      Assert.That(token.Principal.Tenant, Is.Null);
      Assert.That(token.Principal.Roles, Is.Empty);
      Assert.That(token.Principal.IsNull, Is.True);
    }

    [Test]
    public void Create_WithInvalidPrincipal_InvalidUserName ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreatePrincipal("notexisting.user");
      Assert.That(
          () => _securityTokenBuilder.CreateToken(principal, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo(
                  "The user 'notexisting.user' could not be found."));
    }

    [Test]
    public void Create_WithInvalidPrincipal_EmptyUserName ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreatePrincipal("");
      Assert.That(
          () => _securityTokenBuilder.CreateToken(principal, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo("No principal was provided."));
    }

    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      SecurityContext context = CreateContext();

      SecurityToken token = _securityTokenBuilder.CreateToken(CreateTestPrincipal(), context);

      Assert.That(token.AbstractRoles, Is.Empty);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      SecurityContext context = CreateContext(ProjectRoles.QualityManager);

      SecurityToken token = _securityTokenBuilder.CreateToken(CreateTestPrincipal(), context);

      Assert.That(token.AbstractRoles.Count, Is.EqualTo(1));
      Assert.That(
          token.AbstractRoles[0].GetObject().Name,
          Is.EqualTo("QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests"));
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      SecurityContext context = CreateContext(ProjectRoles.QualityManager, ProjectRoles.Developer);


      SecurityToken token = _securityTokenBuilder.CreateToken(CreateTestPrincipal(), context);

      Assert.That(token.AbstractRoles.Count, Is.EqualTo(2));
    }

    [Test]
    public void Create_WithNotExistingAbstractRole ()
    {
      SecurityContext context = CreateContext(ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);
      Assert.That(
          () => _securityTokenBuilder.CreateToken(CreateTestPrincipal(), context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo(
                  "The abstract role 'Undefined|Remotion.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Remotion.SecurityManager.UnitTests' could not be found."
));
    }

    [Test]
    public void Create_WithValidOwningTenant ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken(user, context);

      var tenant = token.OwningTenant;
      Assert.That(tenant, Is.Not.Null);
      Assert.That(tenant.GetObject().UniqueIdentifier, Is.EqualTo("UID: testTenant"));
    }

    [Test]
    public void Create_WithoutOwningTenant ()
    {
      SecurityContext context = CreateContextWithoutOwningTenant();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken(user, context);

      Assert.That(token.OwningTenant, Is.Null);
    }

    [Test]
    public void Create_WithNotExistingOwningTenant ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningTenant();
      ISecurityPrincipal user = CreateTestPrincipal();
      Assert.That(
          () => _securityTokenBuilder.CreateToken(user, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo(
                  "The tenant 'UID: NotExistingTenant' could not be found."));
    }

    [Test]
    public void Create_WithValidOwningGroup ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken(user, context);

      var group = token.OwningGroup;
      Assert.That(group, Is.Not.Null);
      Assert.That(group.GetObject().UniqueIdentifier, Is.EqualTo("UID: testOwningGroup"));
    }

    [Test]
    public void Create_WithoutOwningGroup ()
    {
      SecurityContext context = CreateContextWithoutOwningGroup();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken(user, context);

      Assert.That(token.OwningGroup, Is.Null);
    }

    [Test]
    public void Create_WithNotExistingOwningGroup ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningGroup();
      ISecurityPrincipal user = CreateTestPrincipal();
      Assert.That(
          () => _securityTokenBuilder.CreateToken(user, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo(
                  "The group 'UID: NotExistingGroup' could not be found."));
    }

    [Test]
    public void Create_WithValidOwningUser ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();


      SecurityToken token = _securityTokenBuilder.CreateToken(user, context);

      var owningUser = token.OwningUser;
      Assert.That(owningUser, Is.Not.Null);
      Assert.That(owningUser.GetObject().UserName, Is.EqualTo("group0/user1"));
    }

    [Test]
    public void Create_WithoutOwningUser ()
    {
      SecurityContext context = CreateContextWithoutOwningUser();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken(user, context);

      Assert.That(token.OwningUser, Is.Null);
    }

    [Test]
    public void Create_WithNotExistingOwningUser ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningUser();
      ISecurityPrincipal user = CreateTestPrincipal();
      Assert.That(
          () => _securityTokenBuilder.CreateToken(user, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo(
                  "The user 'notExistingUser' could not be found."));
    }

    [Test]
    public void Create_WithInactiveTransaction ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreateTestPrincipal();

      using (ClientTransactionTestHelper.MakeInactive(ClientTransactionScope.CurrentTransaction))
      {
        SecurityToken token = _securityTokenBuilder.CreateToken(principal, context);

        Assert.That(token.Principal.IsNull, Is.False);
      }
    }

    private ISecurityPrincipal CreateTestPrincipal ()
    {
      return CreatePrincipal("test.user");
    }

    private ISecurityPrincipal CreatePrincipal (string userName)
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(stub => stub.User).Returns(userName);
      return principalStub.Object;
    }

    private SecurityContext CreateContext (params Enum[] abstractRoles)
    {
      return SecurityContext.Create(
          typeof(Order), "group0/user1", "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), abstractRoles);
    }

    private SecurityContext CreateContextWithoutOwningTenant ()
    {
      return SecurityContext.Create(typeof(Order), "group0/user1", "UID: testOwningGroup", null, new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningTenant ()
    {
      return SecurityContext.Create(
          typeof(Order), "group0/user1", "UID: testOwningGroup", "UID: NotExistingTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningGroup ()
    {
      return SecurityContext.Create(typeof(Order), "group0/user1", null, "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningGroup ()
    {
      return SecurityContext.Create(
          typeof(Order), "group0/user1", "UID: NotExistingGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningUser ()
    {
      return SecurityContext.Create(typeof(Order), null, "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningUser ()
    {
      return SecurityContext.Create(
          typeof(Order), "notExistingUser", "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }
  }
}
