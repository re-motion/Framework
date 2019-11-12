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
using System.Configuration;
using System.IO;
using System.IO.Compression;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  public class CustomWebTestConfigurationFactory : WebTestConfigurationFactory
  {
    protected override IChromeConfiguration CreateChromeConfiguration (WebTestConfigurationSection configSettings)
    {
      var chromeVersionArchivePath = ConfigurationManager.AppSettings["ChromeVersionArchive"];
      var advancedChromeOptions = new AdvancedChromeOptions();

      if (string.IsNullOrEmpty (chromeVersionArchivePath))
        return new ChromeConfiguration (configSettings, advancedChromeOptions);

      var versionedChromeFolder = $"Chrome_v{LatestTestedChromeVersion}";
      var customChromeDirectory = PrepareCustomBrowserDirectory (chromeVersionArchivePath, versionedChromeFolder);

      var customBrowserBinary = GetBinaryPath (customChromeDirectory, "chrome");
      var customUserDirectoryPath = CustomUserDirectory.GetCustomUserDirectory();

      var chromeExecutable = ChromeExecutable.CreateForCustomInstance (customBrowserBinary, customUserDirectoryPath);

      return new ChromeConfiguration (configSettings, chromeExecutable, advancedChromeOptions);
    }

    protected override IFirefoxConfiguration CreateFirefoxConfiguration (WebTestConfigurationSection configSettings)
    {
      var firefoxVersionArchivePath = ConfigurationManager.AppSettings["FirefoxVersionArchive"];

      if (string.IsNullOrEmpty (firefoxVersionArchivePath))
        return new FirefoxConfiguration (configSettings);

      var versionedFirefoxFolder = $"Firefox_v{LatestTestedFirefoxVersion}";
      var customFirefoxDirectory = PrepareCustomBrowserDirectory (firefoxVersionArchivePath, versionedFirefoxFolder);

      var customBrowserBinary = GetBinaryPath (customFirefoxDirectory, "firefox");
      var customDriverBinary = GetBinaryPath (customFirefoxDirectory, "geckodriver");

      var firefoxExecutable = new FirefoxExecutable (customBrowserBinary, customDriverBinary);

      return new FirefoxConfiguration (configSettings, firefoxExecutable);
    }

    private string PrepareCustomBrowserDirectory (string browserVersionArchivePath, string versionedBrowserFolderName)
    {
      var localBrowserBinaryFolderPath = Path.Combine (Path.GetTempPath(), versionedBrowserFolderName);

      if (!Directory.Exists (localBrowserBinaryFolderPath))
      {
        var versionedChromeZipFile = $"{versionedBrowserFolderName}.zip";

        ZipFile.ExtractToDirectory (
            Path.Combine (browserVersionArchivePath, versionedChromeZipFile),
            localBrowserBinaryFolderPath);
      }

      return localBrowserBinaryFolderPath;
    }

    private string GetBinaryPath (string localBrowserDirectory, string binaryName)
    {
      return Path.Combine (localBrowserDirectory, binaryName + ".exe");
    }
  }
}