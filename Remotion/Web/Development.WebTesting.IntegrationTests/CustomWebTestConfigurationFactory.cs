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
using System.Configuration;
using System.IO;
using System.IO.Compression;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  public class CustomWebTestConfigurationFactory : WebTestConfigurationFactory
  {
    protected override IChromeConfiguration CreateChromeConfiguration (WebTestConfigurationSection configSettings)
    {
      var chromeVersionArchivePath = ConfigurationManager.AppSettings["ChromeVersionArchive"];

      if (string.IsNullOrEmpty (chromeVersionArchivePath))
        return new ChromeConfiguration (configSettings);

      var customChromeBinary = PrepareCustomChromeBinary (chromeVersionArchivePath);

      var customUserDirectoryPath = CustomUserDirectory.GetCustomUserDirectory();

      var chromeExecutable = ChromeExecutable.CreateForCustomInstance (customChromeBinary, customUserDirectoryPath);

      return new ChromeConfiguration (configSettings, chromeExecutable, true);
    }

    private string PrepareCustomChromeBinary (string chromeVersionArchivePath)
    {
      var versionedChromeFolder = string.Format ("Chrome_v{0}", LatestTestedChromeVersion);

      var localChromeBinaryPath = Path.Combine (Path.GetTempPath(), versionedChromeFolder);

      if (!Directory.Exists (localChromeBinaryPath))
      {
        var versionedChromeZipFile = string.Format ("Chrome_v{0}.zip", LatestTestedChromeVersion);

        ZipFile.ExtractToDirectory (
            Path.Combine (chromeVersionArchivePath, versionedChromeZipFile),
            localChromeBinaryPath);
      }

      return Path.Combine (localChromeBinaryPath, "chrome.exe");
    }
  }
}