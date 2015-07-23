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
using Coypu.Drivers;
using Coypu.Drivers.Selenium;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Custom <see cref="SeleniumWebDriver"/> implementation for <see cref="Browser.InternetExplorer"/>. The default implementation of Coypu does not
  /// set all <see cref="InternetExplorerOptions"/> as required and does not enable driver-internal logging.
  /// </summary>
  public class CustomInternetExplorerDriver : SeleniumWebDriver
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (CustomInternetExplorerDriver));

    public CustomInternetExplorerDriver ()
        : base (CreateInternetExplorerDriver(), Browser.InternetExplorer)
    {
    }

    private static IWebDriver CreateInternetExplorerDriver ()
    {
      var driverService = InternetExplorerDriverService.CreateDefaultService();
      driverService.LogFile = GetLogFile();
      driverService.LoggingLevel = InternetExplorerDriverLogLevel.Info;

      return
          new InternetExplorerDriver (
              driverService,
              new InternetExplorerOptions
              {
                  EnableNativeEvents = true,
                  RequireWindowFocus = true,
                  EnablePersistentHover = false
              });
    }

    private static string GetLogFile ()
    {
      EnsureLogsDirectoryExists();

      // Note: unfortunately there is no append-mode for this log and we do not have enough context information to create a nice name.
      for (var i = 0;; ++i)
      {
        var fileName = string.Format ("InternetExplorerDriver{0}.log", i);
        var logFile = Path.Combine (WebTestingConfiguration.Current.LogsDirectory, fileName);

        if (File.Exists (logFile))
          continue;

        // Log file name in order to give the user the chance to correlate the log file to test executions.
        s_log.InfoFormat ("Internet explorer driver logs to: '{0}'.", fileName);
        return logFile;
      }
    }

    private static void EnsureLogsDirectoryExists ()
    {
      Directory.CreateDirectory (WebTestingConfiguration.Current.LogsDirectory);
    }
  }
}