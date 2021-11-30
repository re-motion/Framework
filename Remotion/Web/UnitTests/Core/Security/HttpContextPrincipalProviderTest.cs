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
using System.Web;
using Moq;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.Infrastructure;
using Remotion.Web.Security;

namespace Remotion.Web.UnitTests.Core.Security
{
  [TestFixture]
  public class HttpContextPrincipalProviderTest
  {
    [Test]
    public void GetPrincipal_HttpContextUserIsNotAuthenticated_ReturnsNullSecurityPrincipal ()
    {
      var identityStub = new Mock<IIdentity>();
      identityStub.Setup(_ => _.IsAuthenticated).Returns(false);

      var principalStub = new Mock<IPrincipal>();
      principalStub.Setup(_ => _.Identity).Returns(identityStub.Object);

      var httpContextStub = new Mock<HttpContextBase>();
      httpContextStub.SetupProperty(_ => _.User);
      httpContextStub.Object.User = principalStub.Object;

      var httpContextProviderStub = new Mock<IHttpContextProvider>();
      httpContextProviderStub.Setup(_ => _.GetCurrentHttpContext()).Returns(httpContextStub.Object);

      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider(httpContextProviderStub.Object);

      Assert.That(principalProvider.GetPrincipal(), Is.TypeOf<NullSecurityPrincipal>());
    }

    [Test]
    public void GetPrincipal_HttpContextUserIsAuthenticated_ReturnsSecurityPrincipalWithNameFromUser ()
    {
      var identityStub = new Mock<IIdentity>();
      identityStub.Setup(_ => _.IsAuthenticated).Returns(true);
      identityStub.Setup(_ => _.Name).Returns("The User");

      var principalStub = new Mock<IPrincipal>();
      principalStub.Setup(_ => _.Identity).Returns(identityStub.Object);

      var httpContextStub = new Mock<HttpContextBase>();
      httpContextStub.SetupProperty(_ => _.User);
      httpContextStub.Object.User = principalStub.Object;

      var httpContextProviderStub = new Mock<IHttpContextProvider>();
      httpContextProviderStub.Setup(_ => _.GetCurrentHttpContext()).Returns(httpContextStub.Object);

      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider(httpContextProviderStub.Object);

      var securityPrincipal = principalProvider.GetPrincipal();
      Assert.That(securityPrincipal, Is.TypeOf<SecurityPrincipal>());
      Assert.That(securityPrincipal.User, Is.EqualTo("The User"));
      Assert.That(securityPrincipal.Roles, Is.Null);
      Assert.That(securityPrincipal.SubstitutedUser, Is.Null);
      Assert.That(securityPrincipal.SubstitutedRoles, Is.Null);
    }

    [Test]
    public void GetPrincipal_HttpContextProviderReturnsNull_ThrowsInvalidOperationException ()
    {
      var httpContextProviderStub = new Mock<IHttpContextProvider>();
      httpContextProviderStub.Setup(_ => _.GetCurrentHttpContext()).Returns((HttpContextBase)null);

      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider(httpContextProviderStub.Object);

      Assert.That(
          () => principalProvider.GetPrincipal(),
          Throws.InvalidOperationException.With.Message.EqualTo("IHttpContextProvider.GetCurrentHttpContext() evaludated and returned null."));
    }

    [Test]
    public void GetIsNull ()
    {
      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider(new Mock<IHttpContextProvider>().Object);
      Assert.That(principalProvider.IsNull, Is.False);
    }
  }
}
