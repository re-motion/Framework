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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Provides a method to resolve the hostname of a <see cref="Uri"/>.
  /// </summary>
  public static class HostnameResolveHelper
  {
    /// <summary>
    /// Resolves the hostname of the given <paramref name="uri"/>. If it contains an IP address, the original URI is returned.
    /// </summary>
    public static Uri ResolveHostname (Uri uri)
    {
      if (uri.HostNameType != UriHostNameType.Dns)
        return uri;

      var host = new RetryUntilTimeout<IPHostEntry> (() => Dns.GetHostEntry (uri.Host), TimeSpan.FromSeconds (30), TimeSpan.FromSeconds (1)).Run();
      var address = host.AddressList.FirstOrDefault (a => a.AddressFamily == AddressFamily.InterNetwork);

      if (address == null)
        throw new InvalidOperationException ($"Could not resolve hostname '{uri}' to a matching inter-network address.");

      var iPv4 = address.MapToIPv4();
      var uriBuilder = new UriBuilder (uri) { Host = iPv4.ToString() };

      var resolvedUri = uriBuilder.Uri;

      if (resolvedUri.IsLoopback)
        return uri;

      return resolvedUri;
    }
  }
}