﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Threading;
using JetBrains.Annotations;
using log4net;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome
{
  /// <summary>
  /// Responsible for removing any files or directories added to the user directory during a test run.
  /// </summary>
  public class ChromeUserDirectoryCleanUpStrategy : IBrowserSessionCleanUpStrategy
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ChromeUserDirectoryCleanUpStrategy));

    private readonly IChromeConfiguration _chromeConfiguration;
    private readonly string _userDirectory;

    public ChromeUserDirectoryCleanUpStrategy ([NotNull] IChromeConfiguration chromeConfiguration, [NotNull] string userDirectory)
    {
      ArgumentUtility.CheckNotNull ("chromeConfiguration", chromeConfiguration);
      ArgumentUtility.CheckNotNullOrEmpty ("userDirectory", userDirectory);

      _chromeConfiguration = chromeConfiguration;
      _userDirectory = userDirectory;
    }

    public void CleanUp ()
    {
      DeleteUserDirectory();
      DeleteUserDirectoryRootOnDemand();
    }

    /// <summary>
    /// Deletes the specific user directory assigned to the current browser session.
    /// </summary>
    private void DeleteUserDirectory ()
    {
      if (string.IsNullOrEmpty (_userDirectory))
        return;

      // The amount of times we try to delete the user data folder before giving up
      const int maxTries = 5;

      // Try to delete the user data folder.
      // One of the files in the folder is still used by a process even if the driver and browser are shutdown.
      // Therefore we retry maxTries times and increase the amount of time we wait between each tries.

      var sleep = 50f;
      var tries = 0;
      do
      {
        if (!Directory.Exists (_userDirectory))
          return;

        try
        {
          Directory.Delete (_userDirectory, true);
        }
        catch (Exception ex)
        {
          // We only handle these exceptions, as they get thrown when trying to delete the directory 
          // and some Chrome process is still accessing a file inside the directory.
          if (!(ex is IOException) && !(ex is UnauthorizedAccessException))
            throw;

          if (tries == maxTries - 1)
          {
            s_log.InfoFormat (
                @"Could not delete the chrome user data folder '{0}' because of an '{1}':
{2}",
                _userDirectory,
                ex.GetType().Name,
                ex.Message);
          }

          Thread.Sleep ((int) sleep);
          sleep *= 1.25f;

          tries++;
        }
      } while (tries < maxTries);
    }

    /// <summary>
    /// Removes the user directory root if we are the last session to use it. 
    /// </summary>
    private void DeleteUserDirectoryRootOnDemand ()
    {
      var userDirectoryRoot = _chromeConfiguration.UserDirectoryRoot;

      if (!_chromeConfiguration.EnableUserDirectoryRootCleanup)
        return;

      if (string.IsNullOrEmpty (userDirectoryRoot))
        return;

      if (!Directory.Exists (userDirectoryRoot))
        return;

      if (Directory.GetDirectories (userDirectoryRoot).Length > 0)
        return;

      Directory.Delete (userDirectoryRoot);
    }
  }
}