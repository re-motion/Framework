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
using Remotion.Development.UnitTesting;
using Remotion.Scripting.StableBindingImplementation;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  public static class StableBindingProxyProviderTestHelper
  {
    public static Type BuildProxyType (this StableBindingProxyProvider proxyProvider, Type proxiedType)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (proxyProvider, "BuildProxyType", proxiedType);
    }

    public static Type GetProxyType (this StableBindingProxyProvider proxyProvider, Type proxiedType)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (proxyProvider, "GetProxyType", proxiedType);
    }

    public static object BuildProxy (this StableBindingProxyProvider proxyProvider, object proxied)
    {
      return PrivateInvoke.InvokeNonPublicMethod (proxyProvider, "BuildProxy", proxied);
    }

    public static object GetProxy (this StableBindingProxyProvider proxyProvider, object proxied)
    {
      return PrivateInvoke.InvokeNonPublicMethod (proxyProvider, "GetProxy", proxied);
    }

  }
}
