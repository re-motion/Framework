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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome
{
  /// <summary>
  /// Provides Chrome and chromedriver binaries and a custom user directory.
  /// </summary>
  public class ChromeBinariesProvider
  {
    private const string c_webDriverFolderName = @"Remotion.Web.Development.WebTesting.WebDriver\chromedriver";

    private const string c_fetchChromeDriverVersionUrlFormat = "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_{0}";
    private const string c_driverDownloadUrlFormat = "https://chromedriver.storage.googleapis.com/{0}/chromedriver_win32.zip";

    private const string c_driverExecutableName = "chromedriver.exe";
    private const string c_chromeExecutableName = "chrome.exe";
    private const string c_zipFileName = "chromedriver.zip";

    /// <summary>
    /// First Chrome version supported by <see cref="ChromeBinariesProvider"/>.
    /// </summary>
    private static readonly Version s_minimumSupportedChromeVersion = new Version (73, 0);

    /// <summary>
    /// Returns the <see cref="ChromeExecutable"/> that contains the installed Chrome browser location, the corresponding ChromeDriver location,
    /// and a temporary user directory. If no ChromeDriver exists, it is downloaded.
    /// </summary>
    public ChromeExecutable GetInstalledExecutable ()
    {
      var browserPath = GetInstalledChromePath();
      var driverPath = GetDriverPathAndDownloadIfMissing (browserPath);
      var userDirectory = GetUserDirectoryTempPath();

      return new ChromeExecutable (browserPath, driverPath, userDirectory);
    }

    /// <summary>
    /// Retrieves the path of the installed Chrome version from the default installation location.
    /// </summary>
    private string GetInstalledChromePath ()
    {
      //Even 64 bit Chrome is installed in the 32bit location
      var defaultStableChromePath = Path.Combine (Get32BitProgramFilesPath(), "Google", "Chrome", "Application", c_chromeExecutableName);

      if (File.Exists (defaultStableChromePath))
        return defaultStableChromePath;

      throw new InvalidOperationException ($"No stable Chrome version could be found at '{defaultStableChromePath}'.");
    }

    private string Get32BitProgramFilesPath ()
    {
      var programFiles32BitFolder = Environment.Is64BitOperatingSystem
          ? Environment.SpecialFolder.ProgramFilesX86
          : Environment.SpecialFolder.ProgramFiles;

      return Environment.GetFolderPath (programFiles32BitFolder);
    }

    /// <summary>
    /// Gets path of the chrome driver needed for the chrome executable version. If there is no driver, it is downloaded.
    /// </summary>
    private string GetDriverPathAndDownloadIfMissing ([NotNull] string chromePath)
    {
      var chromeVersion = GetFileVersion (chromePath);

      if (chromeVersion < s_minimumSupportedChromeVersion)
      {
        throw new NotSupportedException (
            string.Format (
                "The installed Chrome version ({0}) is lower than the minimum required version of {1}.",
                chromeVersion,
                s_minimumSupportedChromeVersion));
      }

      var strippedChromeDriverVersion = StripRevisionFromVersion (chromeVersion);
      var chromeDriverVersion = GetChromeDriverVersion (strippedChromeDriverVersion);
      var driverPath = GetChromeDriverTempPath (chromeDriverVersion);

      if (!ChromeDriverExists (chromeDriverVersion))
        DownloadChromeDriver (chromeDriverVersion);

      return driverPath;
    }

    private Version GetFileVersion (string filePath)
    {
      var fileVersion = FileVersionInfo.GetVersionInfo (filePath).FileVersion;
      return Version.Parse (fileVersion);
    }

    private string StripRevisionFromVersion (Version chromeVersion)
    {
      return chromeVersion.ToString (3);
    }

    private string GetChromeDriverVersion (string chromeVersion)
    {
      var chromeDriverVersionUrl = new Uri (string.Format (c_fetchChromeDriverVersionUrlFormat, chromeVersion));

      try
      {
        using (var webClient = new WebClient())
        {
          return webClient.DownloadString (chromeDriverVersionUrl);
        }
      }
      catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
      {
        throw new InvalidOperationException (
            $"No matching ChromeDriver could be found for the installed Chrome version {chromeVersion}."
            + "This could mean that no corresponding ChromeDriver has been released for the version of Chrome you are using.",
            ex);
      }
    }

    private bool ChromeDriverExists (string chromeDriverVersion)
    {
      return File.Exists (GetChromeDriverTempPath (chromeDriverVersion));
    }

    private string GetChromeDriverTempPath (string chromeDriverVersion)
    {
      return Path.Combine (GetChromeDriverTempDirectory (chromeDriverVersion), c_driverExecutableName);
    }

    private string GetUserDirectoryTempPath ()
    {
      return Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    private string GetChromeDriverRootDirectory ()
    {
      return Path.Combine (Path.GetTempPath(), c_webDriverFolderName);
    }

    private string GetChromeDriverTempDirectory (string chromeDriverVersion)
    {
      return Path.Combine (GetChromeDriverRootDirectory(), string.Format ("chromedriver_v{0}", chromeDriverVersion));
    }

    private void RemoveChromeDriverRootDirectoryIfExists ()
    {
      var chromeDriverRootDirectory = GetChromeDriverRootDirectory();
      if (Directory.Exists (chromeDriverRootDirectory))
        Directory.Delete (chromeDriverRootDirectory, true);
    }

    private void DownloadChromeDriver (string chromeDriverVersion)
    {
      var tempPath = GetChromeDriverTempDirectory (chromeDriverVersion);
      var fullZipPath = Path.Combine (Path.GetTempPath(), c_zipFileName);

      var url = GetDriverDownloadUrl (chromeDriverVersion);

      RemoveChromeDriverRootDirectoryIfExists();
      Directory.CreateDirectory (tempPath);

      using (var webClient = new WebClient())
      {
        webClient.DownloadFile (url, fullZipPath);
      }

      ZipFile.ExtractToDirectory (fullZipPath, tempPath);

      File.Delete (fullZipPath);
    }

    private string GetDriverDownloadUrl (string chromeDriverVersion)
    {
      return string.Format (c_driverDownloadUrlFormat, chromeDriverVersion);
    }
  }
}