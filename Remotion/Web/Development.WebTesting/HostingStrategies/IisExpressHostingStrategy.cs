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
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Hosts the web application using IIS Express.
  /// </summary>
  public class IisExpressHostingStrategy : IHostingStrategy
  {
    private readonly IisExpressProcessWrapper _iisExpressInstance;

    /// <param name="webApplicationPath">Absolute or relative path to the web application source.</param>
    /// <param name="port">Port to be used.</param>
    public IisExpressHostingStrategy ([NotNull] string webApplicationPath, int port)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("webApplicationPath", webApplicationPath);

      var absoluteWebApplicationPath = Path.GetFullPath (webApplicationPath);
      _iisExpressInstance = new IisExpressProcessWrapper (absoluteWebApplicationPath, port);
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestingConfiguration"/>.
    /// </summary>
    /// <param name="properties">The configuration properties.</param>
    [UsedImplicitly]
    public IisExpressHostingStrategy ([NotNull] NameValueCollection properties)
        : this (
            ArgumentUtility.CheckNotNull ("properties", properties)["path"],
            int.Parse (ArgumentUtility.CheckNotNull ("properties", properties)["port"]))
    {
    }

    /// <inheritdoc/>
    public void DeployAndStartWebApplication ()
    {
      var iisExpressThread = new Thread (() => _iisExpressInstance.Run()) { IsBackground = true };
      iisExpressThread.Start();
    }

    /// <inheritdoc/>
    public void StopAndUndeployWebApplication ()
    {
      if (_iisExpressInstance != null)
        _iisExpressInstance.Dispose();
    }
  }
}