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
using Remotion.SecurityManager.Domain;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class NullSecurityManagerPrincipalTest
  {
    [Test]
    public void Get_Members ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;

      Assert.That(principal.Tenant, Is.Null);
      Assert.That(principal.User, Is.Null);
      Assert.That(principal.Roles, Is.Null);
      Assert.That(principal.Substitution, Is.Null);
    }

    [Test]
    public void GetRefreshedInstance ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;
      Assert.That(principal.GetRefreshedInstance(), Is.SameAs(principal));
    }

    [Test]
    public void GetTenants ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;

      Assert.That(principal.GetTenants(true), Is.Empty);
    }

    [Test]
    public void GetActiveSubstitutions ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;

      Assert.That(principal.GetActiveSubstitutions(), Is.Empty);
    }

    [Test]
    public void GetSecurityPrincipal ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;

      Assert.That(principal.GetSecurityPrincipal().IsNull, Is.True);
    }

    [Test]
    public void Serialization ()
    {
      var principal = SecurityManagerPrincipal.Null;

      var deserializedPrincipal = Serializer.SerializeAndDeserialize(principal);

      Assert.That(principal, Is.SameAs(deserializedPrincipal));
    }

    [Test]
    public void Get_IsNull ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;
      Assert.That(principal.IsNull, Is.True);
    }
  }
}
