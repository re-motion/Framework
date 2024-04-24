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
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebDriver.Factories
{
  public static class WebDriverLogUtility
  {
    private static readonly ILogger s_logger = LogManager.GetLogger(typeof(WebDriverLogUtility));
    private const string SubDirectoryName = "BrowserLogs";

    public static string CreateLogFile (string logsDirectory, string browserName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("logsDirectory", logsDirectory);
      ArgumentUtility.CheckNotNullOrEmpty("browserName", browserName);

      var finalLogsDirectory = Path.Combine(logsDirectory, SubDirectoryName);

      EnsureLogsDirectoryExists(finalLogsDirectory);

      //Note: unfortunately there is no append-mode for this log and we do not have enough context information to create a nice name.
      for (var i = 0;; ++i)
      {
        var fileName = string.Format("{0}driver{1}.log", browserName, i);

        var logFile = Path.Combine(finalLogsDirectory, fileName);

        if (File.Exists(logFile))
          continue;

        using (File.Open(logFile, FileMode.CreateNew))
        {
          //NOP
        }

        // Log file name in order to give the user the chance to correlate the log file to test executions.
        s_logger.LogInformation("{0} driver logs to: '{1}'.", browserName, fileName);
        return logFile;
      }
    }

    private static void EnsureLogsDirectoryExists (string logsDirectory)
    {
      Directory.CreateDirectory(logsDirectory);
    }
  }
}
