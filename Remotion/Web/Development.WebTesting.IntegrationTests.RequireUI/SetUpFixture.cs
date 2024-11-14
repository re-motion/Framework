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
using System.IO;
using log4net.Config;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.Logging.Log4Net;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private WebTestSetUpFixtureHelper _setUpFixtureHelper;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      var loggerFactory = new LoggerFactory(new[] { new Log4NetLoggerProvider() });
      WebTestSettings.SetCurrent(WebTestSettings.CreateAppConfigBasedWebTestSettings(loggerFactory));

      _setUpFixtureHelper = WebTestSetUpFixtureHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      AppContext.SetSwitch("Switch.System.Net.DontEnableSchUseStrongCrypto", false);

      try
      {
        var screenshotDirectory = _setUpFixtureHelper.ScreenshotDirectory;

        if (Directory.Exists(screenshotDirectory))
          Directory.Delete(screenshotDirectory, true);

        XmlConfigurator.Configure();
        _setUpFixtureHelper.OnSetUp();
      }
      catch (Exception e)
      {
        Console.WriteLine("SetUpFixture failed: " + e);
        Console.WriteLine();
        throw;
      }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown ()
    {
      try
      {
        _setUpFixtureHelper.OnTearDown();
      }
      catch (Exception e)
      {
        Console.WriteLine("SetUpFixture failed: " + e);
        Console.WriteLine();
        throw;
      }
    }
  }
}
