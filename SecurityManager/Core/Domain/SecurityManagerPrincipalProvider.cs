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
using Remotion.Security;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.Domain
{
  [ImplementationFor (typeof (IPrincipalProvider),
      Lifetime = LifetimeKind.Singleton, Position = Position, RegistrationType = RegistrationType.Single)]
  public sealed class SecurityManagerPrincipalProvider : IPrincipalProvider
  {
    public const int Position = ThreadPrincipalProvider.Position - 113;

    public SecurityManagerPrincipalProvider ()
    {
    }

    public ISecurityPrincipal GetPrincipal ()
    {
      return SecurityManagerPrincipal.Current.GetSecurityPrincipal();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}