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
using System.Security.Principal;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class ThreadPrincipalProviderTest
  {
    private IPrincipalProvider _principalProvider;

    [SetUp]
    public void SetUp ()
    {
      _principalProvider = new ThreadPrincipalProvider();
    }

    [Test]
    public void GetUser ()
    {
      Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("user"), new string[0]);
      var securityPrincipal = _principalProvider.GetPrincipal();
      Assert.That(securityPrincipal.User, Is.EqualTo("user"));
      Assert.That(securityPrincipal.Roles, Is.Null);
      Assert.That(securityPrincipal.SubstitutedUser, Is.Null);
      Assert.That(securityPrincipal.SubstitutedRoles, Is.Null);
    }

    [Test]
    public void GetUser_NotAuthenticated ()
    {
      Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
      Assert.That(Thread.CurrentPrincipal.Identity.IsAuthenticated, Is.False);
      Assert.That(_principalProvider.GetPrincipal().IsNull, Is.True);
    }

    [Test]
    public void GetPrincipal_WithCurrentPrincipalNull_ReturnsNullObject ()
    {
      Thread.CurrentPrincipal = null;
      Assert.That(Thread.CurrentPrincipal, Is.Null);
      Assert.That(_principalProvider.GetPrincipal().IsNull, Is.True);
    }

    [Test]
    public void GetPrincipal_WithCurrentPrincipalIdentityNull_ReturnsNullObject ()
    {
      var principalStub = new Mock<IPrincipal>();
      Thread.CurrentPrincipal = principalStub.Object;

      Assert.That(Thread.CurrentPrincipal, Is.Not.Null);
      Assert.That(Thread.CurrentPrincipal.Identity, Is.Null);
      Assert.That(_principalProvider.GetPrincipal().IsNull, Is.True);
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That(_principalProvider.IsNull, Is.False);
    }
  }
}
