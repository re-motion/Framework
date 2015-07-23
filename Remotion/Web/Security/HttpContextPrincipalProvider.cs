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
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.Security
{
  [ImplementationFor (typeof (IPrincipalProvider),
      Lifetime = LifetimeKind.Singleton, Position = Position, RegistrationType = RegistrationType.Single)]
  public sealed class HttpContextPrincipalProvider : IPrincipalProvider
  {
    public const int Position = ThreadPrincipalProvider.Position - 1;

    private static readonly NullSecurityPrincipal s_nullSecurityPrincipal = new NullSecurityPrincipal();

    private readonly IHttpContextProvider _httpContextProvider;

    public HttpContextPrincipalProvider (IHttpContextProvider httpContextProvider)
    {
      ArgumentUtility.CheckNotNull ("httpContextProvider", httpContextProvider);

      _httpContextProvider = httpContextProvider;
    }

    public ISecurityPrincipal GetPrincipal ()
    {
      var httpContext = _httpContextProvider.GetCurrentHttpContext();
      Assertion.IsNotNull (httpContext, "IHttpContextProvider.GetCurrentHttpContext() evaludated and returned null.");

      var identity = httpContext.User.Identity;
      if (!identity.IsAuthenticated)
        return s_nullSecurityPrincipal;

      return new SecurityPrincipal (identity.Name, null, null, null);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}