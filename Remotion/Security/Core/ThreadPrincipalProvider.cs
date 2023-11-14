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
using System.Threading;
using Remotion.ServiceLocation;

namespace Remotion.Security
{
  [ImplementationFor(typeof(IPrincipalProvider), Lifetime = LifetimeKind.Singleton, Position = Position, RegistrationType = RegistrationType.Single)]
  public sealed class ThreadPrincipalProvider : IPrincipalProvider
  {
    public const int Position = Int32.MaxValue;
    private static readonly NullSecurityPrincipal s_nullSecurityPrincipal = new NullSecurityPrincipal();

    public ThreadPrincipalProvider ()
    {
    }

    public ISecurityPrincipal GetPrincipal ()
    {
      var identity = Thread.CurrentPrincipal?.Identity;

      if (identity is null or { IsAuthenticated: false })
        return s_nullSecurityPrincipal;

      // TODO RM-7873: Add notnull assertion
      return new SecurityPrincipal(identity.Name!, null, null, null);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
