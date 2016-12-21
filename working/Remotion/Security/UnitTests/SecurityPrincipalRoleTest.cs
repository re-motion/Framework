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
  public class SecurityPrincipalRoleTest
  {
    [Test]
    public void Initialize_WithGroup ()
    {
      SecurityPrincipalRole role = new SecurityPrincipalRole ("TheGroup", null);

      Assert.That (role.Group, Is.EqualTo ("TheGroup"));
      Assert.That (role.Position, Is.Null);
    }

    [Test]
    public void Initialize_WithGroupAndPosition ()
    {
      SecurityPrincipalRole role = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (role.Group, Is.EqualTo ("TheGroup"));
      Assert.That (role.Position, Is.EqualTo ("ThePosition"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: group")]
    public void Initialize_WithoutGroup ()
    {
      new SecurityPrincipalRole (null, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'group' cannot be empty.\r\nParameter name: group")]
    public void Initialize_WithGroupEmpty ()
    {
      new SecurityPrincipalRole (string.Empty, null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'position' cannot be empty.\r\nParameter name: position")]
    public void Initialize_WithPositionEmpty ()
    {
      new SecurityPrincipalRole ("TheGroup", string.Empty);
    }

    [Test]
    public void Equals_WithEqualGroupAndNoPosition ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", null);
      var right = new SecurityPrincipalRole ("TheGroup", null);

      Assert.That (left.Equals (right), Is.True);
      Assert.That (right.Equals (left), Is.True);
    }

    [Test]
    public void Equals_WithEqualGroupAndEqualPosition ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (left.Equals (right), Is.True);
      Assert.That (right.Equals (left), Is.True);
    }

    [Test]
    public void Equals_WithEqualGroupAndNotEqualPosition ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = new SecurityPrincipalRole ("TheGroup", "OtherPosition");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithEqualGroupAndNotEqualPosition_ThisPositionNull ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", null);
      var right = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithEqualGroupAndNotEqualPosition_OtherPositionNull ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = new SecurityPrincipalRole ("TheGroup", null);

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithNotEqualGroupAndEqualPosition ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = new SecurityPrincipalRole ("OtherGroup", "ThePosition");

      Assert.That (left.Equals (right), Is.False);
      Assert.That (right.Equals (left), Is.False);
    }

    [Test]
    public void Equals_WithNull ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = (SecurityPrincipalRole)null;

      Assert.That (left.Equals (right), Is.False);
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (left.Equals ((object) right), Is.True);
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      var role = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (role.Equals ((object) null), Is.False);
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      var role = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (role.Equals (new object ()), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      var left = new SecurityPrincipalRole ("TheGroup", "ThePosition");
      var right = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      Assert.That (left.GetHashCode(), Is.EqualTo (right.GetHashCode()));
    }

    [Test]
    public void Serialization ()
    {
      var role = new SecurityPrincipalRole ("TheGroup", "ThePosition");

      var deserializedRole = Serializer.SerializeAndDeserialize (role);

      Assert.That (deserializedRole, Is.Not.SameAs (role));
      Assert.That (deserializedRole, Is.EqualTo (role));
    }
  }
}
