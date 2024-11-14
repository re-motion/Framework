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
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class SecurityPrincipalTest
  {
    [Test]
    public void Initialize_WithUser ()
    {
      var principal = new SecurityPrincipal("TheUser", null, null, null);

      Assert.That(principal.User, Is.EqualTo("TheUser"));
      Assert.That(principal.Roles, Is.Null);
      Assert.That(principal.SubstitutedUser, Is.Null);
      Assert.That(principal.SubstitutedRoles, Is.Null);
    }

    [Test]
    public void Initialize_WithUserAndEmptyRoles ()
    {
      var principal = new SecurityPrincipal("TheUser", new SecurityPrincipalRole[0], null, null);

      Assert.That(principal.User, Is.EqualTo("TheUser"));
      Assert.That(principal.Roles, Is.Empty);
      Assert.That(principal.SubstitutedUser, Is.Null);
      Assert.That(principal.SubstitutedRoles, Is.Null);
    }

    [Test]
    public void Initialize_WithUserAndRolesAndSubstitutedUserAndSubstitutedRoles ()
    {
      var role1 = new SecurityPrincipalRole("TheGroup1", null);
      var role2 = new SecurityPrincipalRole("TheGroup2", null);
      var substitutedRole1 = new SecurityPrincipalRole("SomeGroup1", null);
      var substitutedRole2 = new SecurityPrincipalRole("SomeGroup2", null);
      var principal = new SecurityPrincipal("TheUser", new[] { role1, role2 }, "SomeUser", new[] { substitutedRole1, substitutedRole2 });

      Assert.That(principal.User, Is.EqualTo("TheUser"));
      Assert.That(principal.Roles, Is.EqualTo(new[] { role1, role2 }));
      Assert.That(principal.SubstitutedUser, Is.EqualTo("SomeUser"));
      Assert.That(principal.SubstitutedRoles, Is.EqualTo(new[] { substitutedRole1, substitutedRole2 }));
    }

    [Test]
    public void Initialize_WithUserAndSubstitutedUserAndEmptySubstitutedRoles ()
    {
      var role1 = new SecurityPrincipalRole("TheGroup1", null);
      var role2 = new SecurityPrincipalRole("TheGroup2", null);
      var principal = new SecurityPrincipal("TheUser", new[] { role1, role2 }, "SomeUser", new ISecurityPrincipalRole[0]);

      Assert.That(principal.User, Is.EqualTo("TheUser"));
      Assert.That(principal.Roles, Is.EqualTo(new[] { role1, role2 }));
      Assert.That(principal.SubstitutedUser, Is.EqualTo("SomeUser"));
      Assert.That(principal.SubstitutedRoles, Is.Empty);
    }

    [Test]
    public void Initialize_WithoutGroup ()
    {
      Assert.That(
          () => new SecurityPrincipal(null, null, null, null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "user"));
    }

    [Test]
    public void Initialize_WithUserEmpty ()
    {
      Assert.That(
          () => new SecurityPrincipal(string.Empty, null, null, null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'user' cannot be empty.", "user"));
    }

    [Test]
    public void Initialize_WithSubstitutedUserEmpty ()
    {
      Assert.That(
          () => new SecurityPrincipal("TheUser", null, string.Empty, null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'substitutedUser' cannot be empty.", "substitutedUser"));
    }

    [Test]
    public void Initialize_WithSubstitutedUserMissingAndSubstitutedRoles ()
    {
      var substitutedRole = new SecurityPrincipalRole("group", "position");
      var principal = new SecurityPrincipal("TheUser", null, null, new[] { substitutedRole });
      Assert.That(principal.User, Is.EqualTo("TheUser"));
      Assert.That(principal.Roles, Is.Null);
      Assert.That(principal.SubstitutedUser, Is.Null);
      Assert.That(principal.SubstitutedRoles, Is.EqualTo(new[] { substitutedRole }));
    }

    [Test]
    public void Initialize_WithSubstitutedUserMissingAndEmptySubstitutedRoles ()
    {
      var principal = new SecurityPrincipal("TheUser", null, null, new ISecurityPrincipalRole[0]);
      Assert.That(principal.User, Is.EqualTo("TheUser"));
      Assert.That(principal.Roles, Is.Null);
      Assert.That(principal.SubstitutedUser, Is.Null);
      Assert.That(principal.SubstitutedRoles, Is.Empty);
    }

    [Test]
    public void Equals_WithEqualUser ()
    {
      var left = CreatePrincipal("TheUser", null, null, null);
      var right = CreatePrincipal("TheUser", null, null, null);

      Assert.That(left.Equals(right), Is.True);
      Assert.That(right.Equals(left), Is.True);
    }

    [Test]
    public void Equals_WithAllEqual ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup1", "TheGroup2" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new[] { "TheGroup2", "TheGroup1" }, "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals(right), Is.True);
      Assert.That(right.Equals(left), Is.True);
    }

    [Test]
    public void Equals_WithUserNotEqual ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("OtherUser", new[] { "TheGroup" }, "SomeUser", new[] {"SomeGroup" });

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithRoleNotEqual ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup", "SomeGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new[] { "TheGroup", "TheOtherGroup" }, "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithRoleNotEqual_RoleIsNull ()
    {
      var left = CreatePrincipal("TheUser", new [] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", null, "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithRoleNotEqual_RoleIsEmpty ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new string[0], "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithSubstitutedUserNotEqual ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new[] { "TheGroup" }, "OtherUser", new[] { "SomeGroup" });

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithSubstitutedRoleNotEqual ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "OtherGroup" });

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithSubstitutedRoleNotEqual_RoleIsNull ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", null);

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithNull ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = (SecurityPrincipal)null;

      Assert.That(left.Equals(right), Is.False);
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals((object)right), Is.True);
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals((object)null), Is.False);
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });

      Assert.That(left.Equals(new object()), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      var left = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });
      var right = CreatePrincipal("TheUser", null, "SomeUser", null);

      Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
    }

    [Test]
    public void IsNull ()
    {
      var principal = CreatePrincipal("TheUser", new[] { "TheGroup" }, "SomeUser", new[] { "SomeGroup" });

      Assert.That(principal.IsNull, Is.False);
    }

    private SecurityPrincipal CreatePrincipal (string user, string[] roleGroups, string substitutedUser, string[] substitutedRoleGroup)
    {
      var roles = roleGroups == null ? null : roleGroups.Select(g => new SecurityPrincipalRole(g, null)).ToArray();
      var substitutedRole = substitutedRoleGroup == null ? null : substitutedRoleGroup.Select(g=> new SecurityPrincipalRole(g, null)).ToArray();
      return new SecurityPrincipal(user, roles, substitutedUser, substitutedRole);
    }
  }
}
