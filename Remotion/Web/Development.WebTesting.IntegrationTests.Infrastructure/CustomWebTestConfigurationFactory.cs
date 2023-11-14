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
using System.Linq;
using System.Text.RegularExpressions;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  public class CustomWebTestConfigurationFactory : WebTestConfigurationFactory
  {
    protected override IChromeConfiguration CreateChromeConfiguration (WebTestConfigurationSection configSettings)
    {
      var chromeVersionArchivePath = ConfigurationManager.AppSettings["ChromeVersionArchive"];

      if (string.IsNullOrEmpty(chromeVersionArchivePath))
        return new ChromeConfiguration(configSettings);

      var versionedChromeFolder = GetVersionedBrowserFolderName("Chrome", chromeVersionArchivePath, LatestTestedChromeVersion);
      var customChromeDirectory = PrepareCustomBrowserDirectory(chromeVersionArchivePath, versionedChromeFolder);

      var customBrowserBinary = GetBinaryPath(customChromeDirectory, "chrome");
      var customDriverBinary = GetBinaryPath(customChromeDirectory, "chromedriver");
      var customUserDirectoryPath = CustomUserDirectory.GetCustomUserDirectory();

      var chromeExecutable = new ChromeExecutable(customBrowserBinary, customDriverBinary, customUserDirectoryPath);

      return new ChromeConfiguration(configSettings, chromeExecutable);
    }

    protected override IEdgeConfiguration CreateEdgeConfiguration (WebTestConfigurationSection configSettings)
    {
      var edgeVersionArchivePath = ConfigurationManager.AppSettings["EdgeVersionArchive"];

      if (string.IsNullOrEmpty(edgeVersionArchivePath))
        return new EdgeConfiguration(configSettings);

      var versionedEdgeFolder = GetVersionedBrowserFolderName("Edge", edgeVersionArchivePath, LatestTestedEdgeVersion);
      var customEdgeDirectory = PrepareCustomBrowserDirectory(edgeVersionArchivePath, versionedEdgeFolder);

      var customBrowserBinary = GetBinaryPath(customEdgeDirectory, "msedge");
      var customDriverBinary = GetBinaryPath(customEdgeDirectory, "msedgedriver");
      var customUserDirectoryPath = CustomUserDirectory.GetCustomUserDirectory();

      var edgeExecutable = new EdgeExecutable(customBrowserBinary, customDriverBinary, customUserDirectoryPath);

      return new EdgeConfiguration(configSettings, edgeExecutable);
    }

    protected override IFirefoxConfiguration CreateFirefoxConfiguration (WebTestConfigurationSection configSettings)
    {
      var firefoxVersionArchivePath = ConfigurationManager.AppSettings["FirefoxVersionArchive"];

      if (string.IsNullOrEmpty(firefoxVersionArchivePath))
        return new FirefoxConfiguration(configSettings);

      var versionedFirefoxFolder = GetVersionedBrowserFolderName("Firefox", firefoxVersionArchivePath, LatestTestedFirefoxVersion);
      var customFirefoxDirectory = PrepareCustomBrowserDirectory(firefoxVersionArchivePath, versionedFirefoxFolder);

      var customBrowserBinary = GetBinaryPath(customFirefoxDirectory, "firefox");
      var customDriverBinary = GetBinaryPath(customFirefoxDirectory, "geckodriver");

      var firefoxExecutable = new FirefoxExecutable(customBrowserBinary, customDriverBinary);

      return new FirefoxConfiguration(configSettings, firefoxExecutable);
    }

    private string GetVersionedBrowserFolderName (string browserName, string versionArchivePath, string latestTestedMajorBrowserVersion)
    {
      return Directory.GetFiles(versionArchivePath)
          .Select(Path.GetFileNameWithoutExtension)
          .Where(x => x.StartsWith(browserName))
          .ToDictionary(fileName => fileName, fileName => CreateVersionFromBrowserZipName(fileName, browserName))
          .OrderByDescending(kvp => kvp.Value)
          .First(kvp => kvp.Value.Major == int.Parse(latestTestedMajorBrowserVersion))
          .Key;
    }

    private Version CreateVersionFromBrowserZipName (string fileName, string browserName)
    {
      var regex = new Regex(@$"^{browserName}_v(?<major>\d+)(?<additionalVersionInfo>(\.\d+)*)");
      var match = regex.Match(fileName);
      var additionalVersionInfoValue = StringUtility.EmptyToNull(match.Groups["additionalVersionInfo"].Value) ?? ".0";

      return new Version($"{match.Groups["major"].Value}{additionalVersionInfoValue}");
    }

    private string PrepareCustomBrowserDirectory (string browserVersionArchivePath, string versionedBrowserFolderName)
    {
      var localBrowserBinaryFolderPath = Path.Combine(Path.GetTempPath(), versionedBrowserFolderName);

      if (!Directory.Exists(localBrowserBinaryFolderPath))
      {
        var versionedChromeZipFile = $"{versionedBrowserFolderName}.zip";

        ZipFile.ExtractToDirectory(
            Path.Combine(browserVersionArchivePath, versionedChromeZipFile),
            localBrowserBinaryFolderPath);
      }

      return localBrowserBinaryFolderPath;
    }

    private string GetBinaryPath (string localBrowserDirectory, string binaryName)
    {
      return Path.Combine(localBrowserDirectory, binaryName + ".exe");
    }
  }
}
