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
using System.Collections.Generic;
using System.Collections.Specialized;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;

namespace Remotion.Web.Development.WebTesting.HostingStrategies
{
  /// <summary>
  /// Hosts the web application using IIS Express.
  /// </summary>
  public class IisExpressHostingStrategy : IHostingStrategy
  {
    private readonly IisExpressProcessWrapper _iisExpressInstance;

    /// <param name="testSiteLayoutConfiguration">The configuration of the layout of the used test site.</param>
    /// <param name="port">Port to be used.</param>
    public IisExpressHostingStrategy ([NotNull] ITestSiteLayoutConfiguration testSiteLayoutConfiguration, int port)
    {
      ArgumentUtility.CheckNotNull("testSiteLayoutConfiguration", testSiteLayoutConfiguration);

      _iisExpressInstance = new IisExpressProcessWrapper(testSiteLayoutConfiguration.RootPath, port);
    }

    /// <summary>
    /// Constructor required for direct usage in <see cref="WebTestConfigurationSection"/>.
    /// </summary>
    /// <param name="testSiteLayoutConfiguration">The configuration of the layout of the used test site.</param>
    /// <param name="properties">The configuration properties.</param>
    [UsedImplicitly]
    public IisExpressHostingStrategy ([NotNull] ITestSiteLayoutConfiguration testSiteLayoutConfiguration, [NotNull] IReadOnlyDictionary<string, string> properties)
        : this(testSiteLayoutConfiguration, int.Parse(ArgumentUtility.CheckNotNull("properties", properties)["port"]!))
    {
      // TODO RM-8113: Guard used properties against null values.
    }

    /// <inheritdoc/>
    public void DeployAndStartWebApplication ()
    {
      _iisExpressInstance.Run();
    }

    /// <inheritdoc/>
    public void StopAndUndeployWebApplication ()
    {
      if (_iisExpressInstance != null)
        _iisExpressInstance.Dispose();
    }
  }
}
