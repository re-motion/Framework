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
using System.Web;
using Remotion.Context;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.Context
{
#if NETFRAMEWORK
  [ImplementationFor(typeof(ISafeContextStorageProvider), Lifetime = LifetimeKind.Singleton, Position = 0)]
  public class HttpContextStorageProviderWithCallContextBasedFallback : ISafeContextStorageProvider
  {
    private readonly CallContextStorageProvider _fallbackProvider = new();

    public SafeContextBoundary OpenSafeContextBoundary ()
    {
      return default;
    }

    public object? GetData (string key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var httpContext = HttpContext.Current;
      if (httpContext != null)
        return httpContext.Items[key];
      else
        return _fallbackProvider.GetData(key);
    }

    public void SetData (string key, object? value)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var httpContext = HttpContext.Current;
      if (httpContext != null)
        httpContext.Items[key] = value;
      else
        _fallbackProvider.SetData(key, value);
    }

    public void FreeData (string key)
    {
      ArgumentUtility.CheckNotNull("key", key);

      var httpContext = HttpContext.Current;
      if (httpContext != null)
        httpContext.Items.Remove(key);
      else
        _fallbackProvider.FreeData(key);
    }
  }
#endif
}
