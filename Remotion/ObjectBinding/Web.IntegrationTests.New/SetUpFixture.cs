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
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.HostingStrategies;
using Remotion.Web.Development.WebTesting.HostingStrategies.Configuration;

namespace Remotion.ObjectBinding.Web.IntegrationTests.New
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private IHostingStrategy _hostingStrategy;

    public SetUpFixture ()
    {
    }

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      var webTestSettings = WebTestSettings.CreateAppConfigBasedWebTestSettings(new NullLoggerFactory());
      WebTestSettings.SetCurrent(webTestSettings);

      var hostingConfiguration = new HostingConfiguration(webTestSettings, new TestSiteLayoutConfiguration(webTestSettings));
      _hostingStrategy = hostingConfiguration.GetHostingStrategy();
      _hostingStrategy.DeployAndStartWebApplication();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown ()
    {
      _hostingStrategy?.StopAndUndeployWebApplication();
    }
  }
}
