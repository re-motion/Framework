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
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class SecurityPrincipalTest
  {
    [Test]
    public void Initialize_WithUser ()
    {
      var principal = new SecurityPrincipal ("TheUser", null, null, null);

      Assert.That (principal.User, Is.EqualTo ("TheUser"));
      Assert.That (principal.Role, Is.Null);
      Assert.That (principal.SubstitutedUser, Is.Null);
      Assert.That (principal.SubstitutedRole, Is.Null);
    }

    [Test]
    public void Initialize_WithUserAndRoleAndSubstitutedUserAndSubstitedRole ()
    {
      var role = new SecurityPrincipalRole ("TheGroup", null);
      var substitutedRole = new SecurityPrincipalRole ("SomeGroup", null);
      var principal = new SecurityPrincipal ("TheUser", role, "SomeUser", substitutedRole);

      Assert.That (principal.User, Is.EqualTo ("TheUser"));
      Assert.That (principal.Role, Is.SameAs (role));
      Assert.That (principal.SubstitutedUser, Is.EqualTo ("SomeUser"));
      Assert.That (principal.SubstitutedRole, Is.SameAs (substitutedRole));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: user")]
    public void Initialize_WithoutGroup ()
    {
      new SecurityPrincipal (null, null, null, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'user' cannot be empty.\r\nParameter name: user")]
    public void Initialize_WithUserEmpty ()
    {
      new SecurityPrincipal (string.Empty, null, null, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'substitutedUser' cannot be empty.\r\nParameter name: substitutedUser")]
    public void Initialize_WithSubstitutedUserEmpty ()
    {
      new SecurityPrincipal ("TheUser", null, string.Empty, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The substituted user must be specified if a substituted role is also specified.\r\nParameter name: substitutedUser")]
    public void Initialize_WithSubstitutedUserMissing ()
    {
      new SecurityPrincipal ("TheUser", null, null, new SecurityPrincipalRole("group", "position"));
    }

    [Test]
    public void Equals_WithEqualUser ()
    {
      var left = CreatePrincipal ("TheUser", null, null, null);
      var right = CreatePrincipal ("TheUser", null, null, null);

      Assert.That (left.Equals (right), Is.True);
      Assert.That (right.Equals (left), Is.True);
    }

    [Test]
    public void Equals_WithAllEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.Equals (right), Is.True);
      Assert.That (right.Equals (left), Is.True);
    }

    [Test]
    public void Equals_WithUserNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("OtherUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithRoleNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "OtherGroup", "SomeUser", "SomeGroup");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithRoleNotEqual_RoleIsNull ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", null, "SomeUser", "SomeGroup");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithSubstitutedUserNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "OtherUser", "SomeGroup");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithSubstitutedRoleNotEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "OtherGroup");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithSubstitutedRoleNotEqual_RoleIsNull ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", null);

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithNull ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = (SecurityPrincipal) null;

      Assert.That (left.Equals (right), Is.False);
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.Equals ((object) right), Is.True);
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.Equals ((object) null), Is.False);
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.Equals (new object()), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      var left = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      var right = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      Assert.That (left.GetHashCode(), Is.EqualTo (right.GetHashCode()));
    }

    [Test]
    public void Serialization ()
    {
      var principal = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");

      var deserializedRole = Serializer.SerializeAndDeserialize (principal);

      Assert.That (deserializedRole, Is.Not.SameAs (principal));
      Assert.That (deserializedRole, Is.EqualTo (principal));
    }

    [Test]
    public void IsNull ()
    {
      var principal = CreatePrincipal ("TheUser", "TheGroup", "SomeUser", "SomeGroup");
      
      Assert.That (principal.IsNull, Is.False);
    }

    private SecurityPrincipal CreatePrincipal (string user, string roleGroup, string substitutedUser, string substitutedRoleGroup)
    {
      var role = roleGroup != null ? new SecurityPrincipalRole (roleGroup, null) : null;
      var substitutedRole = substitutedRoleGroup != null ? new SecurityPrincipalRole (substitutedRoleGroup, null) : null;
      return new SecurityPrincipal (user, role, substitutedUser, substitutedRole);
    }
  }
}
