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
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.Infrastructure;
using Remotion.Web.Security;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Security
{
  [TestFixture]
  public class HttpContextPrincipalProviderTest
  {
    [Test]
    public void GetPrincipal_HttpContextUserIsNotAuthenticated_ReturnsNullSecurityPrincipal ()
    {
      var identityStub = MockRepository.GenerateStub<IIdentity>();
      identityStub.Stub (_ => _.IsAuthenticated).Return (false);

      var principalStub = MockRepository.GenerateStub<IPrincipal>();
      principalStub.Stub (_ => _.Identity).Return (identityStub);

      var httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      httpContextStub.User = principalStub;

      var httpContextProviderStub = MockRepository.GenerateStub<IHttpContextProvider>();
      httpContextProviderStub.Stub (_ => _.GetCurrentHttpContext()).Return (httpContextStub);

      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider (httpContextProviderStub);

      Assert.That (principalProvider.GetPrincipal(), Is.TypeOf<NullSecurityPrincipal>());
    }

    [Test]
    public void GetPrincipal_HttpContextUserIsAuthenticated_ReturnsSecurityPrincipalWithNameFromUser ()
    {
      var identityStub = MockRepository.GenerateStub<IIdentity>();
      identityStub.Stub (_ => _.IsAuthenticated).Return (true);
      identityStub.Stub (_ => _.Name).Return ("The User");

      var principalStub = MockRepository.GenerateStub<IPrincipal>();
      principalStub.Stub (_ => _.Identity).Return (identityStub);

      var httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      httpContextStub.User = principalStub;

      var httpContextProviderStub = MockRepository.GenerateStub<IHttpContextProvider>();
      httpContextProviderStub.Stub (_ => _.GetCurrentHttpContext()).Return (httpContextStub);

      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider (httpContextProviderStub);

      var securityPrincipal = principalProvider.GetPrincipal();
      Assert.That (securityPrincipal, Is.TypeOf<SecurityPrincipal>());
      Assert.That (securityPrincipal.User, Is.EqualTo ("The User"));
    }

    [Test]
    public void GetPrincipal_HttpContextProviderReturnsNull_ThrowsInvalidOperationException ()
    {
      var httpContextProviderStub = MockRepository.GenerateStub<IHttpContextProvider>();
      httpContextProviderStub.Stub (_ => _.GetCurrentHttpContext()).Return (null);

      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider (httpContextProviderStub);

      Assert.That (
          () => principalProvider.GetPrincipal(),
          Throws.InvalidOperationException.With.Message.EqualTo ("IHttpContextProvider.GetCurrentHttpContext() evaludated and returned null."));
    }

    [Test]
    public void GetIsNull ()
    {
      IPrincipalProvider principalProvider = new HttpContextPrincipalProvider(MockRepository.GenerateStub<IHttpContextProvider>());
      Assert.That (principalProvider.IsNull, Is.False);
    }
  }
}