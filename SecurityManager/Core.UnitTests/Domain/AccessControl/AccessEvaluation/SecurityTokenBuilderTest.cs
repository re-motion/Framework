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
using System.Collections.Generic;
using System.Linq;
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
using Rhino.Mocks;

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
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      var userRevisionProvider = new UserRevisionProvider();
      _securityPrincipalRepository = new SecurityPrincipalRepository (userRevisionProvider);

      var userNamesRevisionProvider = new UserNamesRevisionProvider();
      _securityTokenBuilder = new SecurityTokenBuilder (
          _securityPrincipalRepository,
          new SecurityContextRepository (new RevisionProvider(), userNamesRevisionProvider));

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
      ISecurityPrincipal principal = CreateTestPrincipal ();
      Assert.That (principal.Roles, Is.Null);

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant, Is.EqualTo (user.Tenant).Using (DomainObjectHandleComparer.Instance));
      Assert.That (token.Principal.Roles, Is.Not.Empty);
      Assert.That (token.Principal.Roles, Is.EquivalentTo (user.Roles).Using (PrincipalRoleComparer.Instance));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithRolesEmpty ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return ("test.user");
      principalStub.Stub (stub => stub.Roles).Return (new ISecurityPrincipalRole[0]);

      SecurityContext context = CreateContext();

      SecurityToken token = _securityTokenBuilder.CreateToken (principalStub, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant, Is.EqualTo (user.Tenant).Using (DomainObjectHandleComparer.Instance));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithRoles ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return ("test.user");
      var princialRole1Stub = MockRepository.GenerateStub<ISecurityPrincipalRole>();
      princialRole1Stub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialRole1Stub.Stub (stub => stub.Position).Return ("UID: Official");
      var princialRole2Stub = MockRepository.GenerateStub<ISecurityPrincipalRole>();
      princialRole2Stub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialRole2Stub.Stub (stub => stub.Position).Return ("UID: Manager");
      principalStub.Stub (stub => stub.Roles).Return (new[] { princialRole1Stub, princialRole2Stub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var principalUser = token.Principal.User.GetObject();
      Assert.That (principalUser.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant, Is.EqualTo (principalUser.Tenant).Using (DomainObjectHandleComparer.Instance));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (2));
      Assert.That (
          token.Principal.Roles.Select (r => Tuple.Create (r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo (new[] { Tuple.Create ("UID: testGroup", "UID: Manager"), Tuple.Create ("UID: testGroup", "UID: Official") }));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithOneSubstitutedRole_MatchingSubstitionWithRole_UsesSubstitutedRole ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return (null);
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (1));
      Assert.That (
          token.Principal.Roles.Select (r => Tuple.Create (r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo (new[] { Tuple.Create ("UID: testGroup", "UID: Official") }));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithSubstitutedRolesEmpty_HasNoRoles ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return (null);
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new ISecurityPrincipalRole[0]);

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithMultipleSubstitutedRoles_DoesNotMatch ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return (null);
      var princialSubstitutedRoleStub1 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub1.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub1.Stub (stub => stub.Position).Return ("UID: Official");
      var princialSubstitutedRoleStub2 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub2.Stub (stub => stub.Group).Return ("UID: testOwningGroup");
      princialSubstitutedRoleStub2.Stub (stub => stub.Position).Return ("UID: Manager");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub1, princialSubstitutedRoleStub2 });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithoutSubstitutedUser_WithTenantMismatch_HasNoRoles ()
    {
      var principalUser = _securityPrincipalRepository.GetUser ("User.Tenant2");
      var substitution = principalUser.GetActiveSubstitutions().Single();
      substitution.SubstitutedRole = substitution.SubstitutedUser.Roles.First (r => r.User.Tenant != principalUser.Tenant);
      principalUser.RootTransaction.Commit();
      Assert.That (substitution.IsActive, Is.True, "Substitution must be unchanged.");

      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return (principalUser.UserName);
      principalStub.Stub (stub => stub.SubstitutedUser).Return (null);
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return (substitution.SubstitutedRole.Group.UniqueIdentifier);
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return (substitution.SubstitutedRole.Position.UniqueIdentifier);
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.Not.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithSubstitutedRolesNull_UsesSubstitutedUserAndAllRolesOfSubstitutedUser ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (null);

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Not.Empty);
      Assert.That (token.Principal.Roles, Is.EquivalentTo (user.Roles).Using (PrincipalRoleComparer.Instance));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithSubstitutedRolesEmpty_UsesSubstitutedUserButHasNoRoles ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new ISecurityPrincipalRole[0]);

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithOneSubstitutedRole_MatchingSubstitionWithRole_UsesSubstitutedUserAndRole ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (1));
      Assert.That (
          token.Principal.Roles.Select (r => Tuple.Create (r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo (new[] { Tuple.Create ("UID: testGroup", "UID: Official") }));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithOneSubstitutedRole_WouldMatchRoleOnlyInOtherSubstitution_UsesSubstitutedUserButUserHasNoRoles ()
    {
      var principalUser = _securityPrincipalRepository.GetUser ("substituting.user");
      using (principalUser.RootTransaction.EnterNonDiscardingScope())
      {
        var testHelper = new OrganizationalStructureTestHelper (principalUser.RootTransaction);
        var otherUser = testHelper.CreateUser ("otherUser", "First", "Last", null, principalUser.OwningGroup, principalUser.Tenant);
        var testRootGroup = Group.FindByUnqiueIdentifier ("UID: rootGroup");
        var managerPosition = Position.FindAll().Single (p => p.UniqueIdentifier == "UID: Manager");
        var role2 = testHelper.CreateRole (otherUser, testRootGroup, managerPosition);
        var substitution2 = Substitution.NewObject();
        substitution2.SubstitutedRole = role2;
        substitution2.SubstitutedUser = otherUser;
        substitution2.SubstitutingUser = principalUser;
        principalUser.RootTransaction.Commit();
        Assert.That (substitution2.IsActive, Is.True, "Substitution must be unchanged.");
      }

      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: rootGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Manager");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithMultipleSubstitutedRoles_MatchesUserSubstition_UsesSubstitutedUserAndReducesSetOfRolesToContainOnlySubstitutedRoles ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");

      var princialSubstitutedRoleStub1 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub1.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub1.Stub (stub => stub.Position).Return ("UID: Official");

      var princialSubstitutedRoleStub2 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub2.Stub (stub => stub.Group).Return ("UID: testOwningGroup");
      princialSubstitutedRoleStub2.Stub (stub => stub.Position).Return ("UID: Manager");

      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub1, princialSubstitutedRoleStub2 });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (2));
      Assert.That (
          token.Principal.Roles.Select (r => Tuple.Create (r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo (new[] { Tuple.Create ("UID: testGroup", "UID: Official"), Tuple.Create ("UID: testOwningGroup", "UID: Manager") }));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithMultipleSubstitutedRoles_MatchesUserSubstition_UsesSubstitutedUserAndReducesSetOfRolesToContainOnlyRolesOfSubstitutedUser ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");

      var princialSubstitutedRoleStub1 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub1.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub1.Stub (stub => stub.Position).Return ("UID: Official");

      var princialSubstitutedRoleStub2 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub2.Stub (stub => stub.Group).Return ("UID: testOwningGroup");
      princialSubstitutedRoleStub2.Stub (stub => stub.Position).Return ("UID: Manager");

      var princialSubstitutedRoleStub3 = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub3.Stub (stub => stub.Group).Return ("UID: rootGroup");
      princialSubstitutedRoleStub3.Stub (stub => stub.Position).Return ("UID: Manager");

      principalStub
          .Stub (stub => stub.SubstitutedRoles)
          .Return (new[] { princialSubstitutedRoleStub1, princialSubstitutedRoleStub2, princialSubstitutedRoleStub3 });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      var user = token.Principal.User.GetObject();
      Assert.That (user.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (2));
      Assert.That (
          token.Principal.Roles.Select (r => Tuple.Create (r.Group.GetObject().UniqueIdentifier, r.Position.GetObject().UniqueIdentifier)),
          Is.EquivalentTo (new[] { Tuple.Create ("UID: testGroup", "UID: Official"), Tuple.Create ("UID: testOwningGroup", "UID: Manager") }));
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInactiveSubstitution ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return (null);
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Manager");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedUser ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("notexisting.user");

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedRoleFromGroup_ThrowsAccessControlException ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: notexisting.group");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      Assert.That (
          () => _securityTokenBuilder.CreateToken (principal, context),
          Throws.TypeOf<AccessControlException>().With.Message.EqualTo ("The group 'UID: notexisting.group' could not be found."));
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedRoleFromPosition_ThrowsAccessControlException ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole>();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: notexisting.position");
      principalStub.Stub (stub => stub.SubstitutedRoles).Return (new[] { princialSubstitutedRoleStub });

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      Assert.That (
          () => _securityTokenBuilder.CreateToken (principal, context),
          Throws.TypeOf<AccessControlException>().With.Message.EqualTo ("The position 'UID: notexisting.position' could not be found."));
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser_WithTenantMismatch_HasNoUserAndNoRoles ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("User.Tenant2");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");

      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = principalStub;

      SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant, Is.Not.Null);
      Assert.That (token.Principal.Tenant.GetObject().UniqueIdentifier, Is.Not.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.False);
    }

    [Test]
    public void Create_WithNullPrincipal ()
    {
      SecurityContext context = CreateContext();
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.IsNull).Return (true);

      SecurityToken token = _securityTokenBuilder.CreateToken (principalStub, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant, Is.Null);
      Assert.That (token.Principal.Roles, Is.Empty);
      Assert.That (token.Principal.IsNull, Is.True);
    }

    [Test]
    public void Create_WithInvalidPrincipal_InvalidUserName ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreatePrincipal ("notexisting.user");
      Assert.That (
          () => _securityTokenBuilder.CreateToken (principal, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo (
                  "The user 'notexisting.user' could not be found."));
    }

    [Test]
    public void Create_WithInvalidPrincipal_EmptyUserName ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreatePrincipal ("");
      Assert.That (
          () => _securityTokenBuilder.CreateToken (principal, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo ("No principal was provided."));
    }

    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      SecurityContext context = CreateContext();

      SecurityToken token = _securityTokenBuilder.CreateToken (CreateTestPrincipal(), context);

      Assert.That (token.AbstractRoles, Is.Empty);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.QualityManager);

      SecurityToken token = _securityTokenBuilder.CreateToken (CreateTestPrincipal(), context);

      Assert.That (token.AbstractRoles.Count, Is.EqualTo (1));
      Assert.That (
          token.AbstractRoles[0].GetObject().Name,
          Is.EqualTo ("QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests"));
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      SecurityContext context = CreateContext (ProjectRoles.QualityManager, ProjectRoles.Developer);


      SecurityToken token = _securityTokenBuilder.CreateToken (CreateTestPrincipal(), context);

      Assert.That (token.AbstractRoles.Count, Is.EqualTo (2));
    }

    [Test]
    public void Create_WithNotExistingAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);
      Assert.That (
          () => _securityTokenBuilder.CreateToken (CreateTestPrincipal(), context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo (
                  "The abstract role 'Undefined|Remotion.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Remotion.SecurityManager.UnitTests' could not be found."
));
    }

    [Test]
    public void Create_WithValidOwningTenant ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken (user, context);

      var tenant = token.OwningTenant;
      Assert.That (tenant, Is.Not.Null);
      Assert.That (tenant.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
    }

    [Test]
    public void Create_WithoutOwningTenant ()
    {
      SecurityContext context = CreateContextWithoutOwningTenant();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken (user, context);

      Assert.That (token.OwningTenant, Is.Null);
    }

    [Test]
    public void Create_WithNotExistingOwningTenant ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningTenant();
      ISecurityPrincipal user = CreateTestPrincipal();
      Assert.That (
          () => _securityTokenBuilder.CreateToken (user, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo (
                  "The tenant 'UID: NotExistingTenant' could not be found."));
    }

    [Test]
    public void Create_WithValidOwningGroup ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken (user, context);

      var group = token.OwningGroup;
      Assert.That (group, Is.Not.Null);
      Assert.That (group.GetObject().UniqueIdentifier, Is.EqualTo ("UID: testOwningGroup"));
    }

    [Test]
    public void Create_WithoutOwningGroup ()
    {
      SecurityContext context = CreateContextWithoutOwningGroup();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken (user, context);

      Assert.That (token.OwningGroup, Is.Null);
    }

    [Test]
    public void Create_WithNotExistingOwningGroup ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningGroup();
      ISecurityPrincipal user = CreateTestPrincipal();
      Assert.That (
          () => _securityTokenBuilder.CreateToken (user, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo (
                  "The group 'UID: NotExistingGroup' could not be found."));
    }

    [Test]
    public void Create_WithValidOwningUser ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();


      SecurityToken token = _securityTokenBuilder.CreateToken (user, context);

      var owningUser = token.OwningUser;
      Assert.That (owningUser, Is.Not.Null);
      Assert.That (owningUser.GetObject().UserName, Is.EqualTo ("group0/user1"));
    }

    [Test]
    public void Create_WithoutOwningUser ()
    {
      SecurityContext context = CreateContextWithoutOwningUser();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityToken token = _securityTokenBuilder.CreateToken (user, context);

      Assert.That (token.OwningUser, Is.Null);
    }

    [Test]
    public void Create_WithNotExistingOwningUser ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningUser();
      ISecurityPrincipal user = CreateTestPrincipal();
      Assert.That (
          () => _securityTokenBuilder.CreateToken (user, context),
          Throws.InstanceOf<AccessControlException>()
              .With.Message.EqualTo (
                  "The user 'notExistingUser' could not be found."));
    }

    [Test]
    public void Create_WithInactiveTransaction ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal principal = CreateTestPrincipal ();

      using (ClientTransactionTestHelper.MakeInactive (ClientTransactionScope.CurrentTransaction))
      {  
        SecurityToken token = _securityTokenBuilder.CreateToken (principal, context);

        Assert.That (token.Principal.IsNull, Is.False);
      }
    }

    private ISecurityPrincipal CreateTestPrincipal ()
    {
      return CreatePrincipal ("test.user");
    }

    private ISecurityPrincipal CreatePrincipal (string userName)
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return (userName);
      return principalStub;
    }

    private SecurityContext CreateContext (params Enum[] abstractRoles)
    {
      return SecurityContext.Create (
          typeof (Order), "group0/user1", "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), abstractRoles);
    }

    private SecurityContext CreateContextWithoutOwningTenant ()
    {
      return SecurityContext.Create (typeof (Order), "group0/user1", "UID: testOwningGroup", null, new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningTenant ()
    {
      return SecurityContext.Create (
          typeof (Order), "group0/user1", "UID: testOwningGroup", "UID: NotExistingTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningGroup ()
    {
      return SecurityContext.Create (typeof (Order), "group0/user1", null, "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningGroup ()
    {
      return SecurityContext.Create (
          typeof (Order), "group0/user1", "UID: NotExistingGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningUser ()
    {
      return SecurityContext.Create (typeof (Order), null, "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningUser ()
    {
      return SecurityContext.Create (
          typeof (Order), "notExistingUser", "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }
  }
}
