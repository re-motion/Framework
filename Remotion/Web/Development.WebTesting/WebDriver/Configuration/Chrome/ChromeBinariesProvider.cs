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
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome.Dto;

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

    private const string c_chromeJsonApiUrl = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";

    private const string c_driverExecutableName = "chromedriver.exe";
    private const string c_chromeExecutableName = "chrome.exe";
    private const string c_zipFileName = "chromedriver.zip";

    /// <summary>
    /// First Chrome version supported by <see cref="ChromeBinariesProvider"/>.
    /// </summary>
    private static readonly Version s_minimumSupportedChromeVersion = new Version(102, 0);

    /// <summary>
    /// Returns the <see cref="ChromeExecutable"/> that contains the installed Chrome browser location, the corresponding ChromeDriver location,
    /// and a temporary user directory. If no ChromeDriver exists, it is downloaded.
    /// </summary>
    public ChromeExecutable GetInstalledExecutable ()
    {
      var browserPath = GetInstalledChromePath();
      var driverPath = GetDriverPathAndDownloadIfMissing(browserPath);
      var userDirectory = GetUserDirectoryTempPath();

      return new ChromeExecutable(browserPath, driverPath, userDirectory);
    }

    /// <summary>
    /// Retrieves the path of the installed Chrome version from the default installation location.
    /// </summary>
    private string GetInstalledChromePath ()
    {
      //Even 64 bit Chrome is installed in the 32bit location
      var x86StableChromePath = Path.Combine(Get32BitProgramFilesPath(), "Google", "Chrome", "Application", c_chromeExecutableName);

      if (File.Exists(x86StableChromePath))
        return x86StableChromePath;

      if (!Environment.Is64BitOperatingSystem)
        throw new InvalidOperationException($"No stable Chrome version could be found at '{x86StableChromePath}'.");

      var x64StableChromePath = Path.Combine(Get64BitProgramFilesPath(), "Google", "Chrome", "Application", c_chromeExecutableName);

      if (File.Exists(x64StableChromePath))
        return x64StableChromePath;

      throw new InvalidOperationException($"No stable Chrome version could be found at '{x64StableChromePath}' or at '{x86StableChromePath}'.");
    }

    private string Get32BitProgramFilesPath ()
    {
      var programFiles32BitFolder = Environment.Is64BitOperatingSystem
          ? Environment.SpecialFolder.ProgramFilesX86
          : Environment.SpecialFolder.ProgramFiles;

      return Environment.GetFolderPath(programFiles32BitFolder);
    }

    private string Get64BitProgramFilesPath ()
    {
      return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
    }

    /// <summary>
    /// Gets path of the chrome driver needed for the chrome executable version. If there is no driver, it is downloaded.
    /// </summary>
    private string GetDriverPathAndDownloadIfMissing ([NotNull] string chromePath)
    {
      var chromeVersion = GetFileVersion(chromePath);

      if (chromeVersion < s_minimumSupportedChromeVersion)
      {
        throw new NotSupportedException(
            string.Format(
                "The installed Chrome version ({0}) is lower than the minimum required version of {1}.",
                chromeVersion,
                s_minimumSupportedChromeVersion));
      }

      var strippedChromeDriverVersion = StripRevisionFromVersion(chromeVersion);

      // Legacy download behavior for Chrome versions before 115. Later versions use a JSON endpoint instead
      string chromeDriverVersion, driverDownloadUrl;
      string? zipArchiveFolder;
      if (chromeVersion.Major < 115)
      {
        chromeDriverVersion = GetChromeDriverVersion(strippedChromeDriverVersion);
        driverDownloadUrl = GetDriverDownloadUrl(chromeDriverVersion);
        zipArchiveFolder = null;
      }
      else
      {
        // We don't get the chrome driver version from the API so we use the Chrome version for the storage instead
        // This is not a problem because should be a 1 to 1 mapping between Chrome version and Chrome driver version
        // In the worst case we would download the same driver twice for differing chrome versions
        chromeDriverVersion = chromeVersion.ToString(3);
        driverDownloadUrl = GetDownloadUrlForChrome115AndLater(chromeVersion);
        zipArchiveFolder = "chromedriver-win32";
      }

      var driverPath = GetChromeDriverTempPath(chromeDriverVersion, zipArchiveFolder);
      if (!File.Exists(driverPath))
        DownloadChromeDriver(chromeDriverVersion, driverDownloadUrl);

      return driverPath;
    }

    private Version GetFileVersion (string filePath)
    {
      var fileVersion = Assertion.IsNotNull(FileVersionInfo.GetVersionInfo(filePath).FileVersion, "File version could not be read from '{0}'.", filePath);
      return Version.Parse(fileVersion);
    }

    private string StripRevisionFromVersion (Version chromeVersion)
    {
      return chromeVersion.ToString(3);
    }

    private string GetChromeDriverVersion (string chromeVersion)
    {
      var chromeDriverVersionUrl = new Uri(string.Format(c_fetchChromeDriverVersionUrlFormat, chromeVersion));

      try
      {
#pragma warning disable SYSLIB0014
        using (var webClient = new WebClient()) // TODO RM-8492: Replace with HttpClient
#pragma warning restore SYSLIB0014
        {
          return webClient.DownloadString(chromeDriverVersionUrl);
        }
      }
      catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
      {
        throw new InvalidOperationException(
            $"No matching ChromeDriver could be found for the installed Chrome version {chromeVersion}."
            + "This could mean that no corresponding ChromeDriver has been released for the version of Chrome you are using.",
            ex);
      }
      catch (WebException ex)
      {
        throw new WebException($"Could not fetch the latest ChromeDriver version from '{chromeDriverVersionUrl}': {ex.Message}", ex.Status);
      }
    }

    private string GetChromeDriverTempPath (string chromeDriverVersion, string? zipArchiveFolder)
    {
      return zipArchiveFolder != null
          ? Path.Combine(GetChromeDriverTempDirectory(chromeDriverVersion), zipArchiveFolder, c_driverExecutableName)
          : Path.Combine(GetChromeDriverTempDirectory(chromeDriverVersion), c_driverExecutableName);
    }

    private string GetUserDirectoryTempPath ()
    {
      return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    private string GetChromeDriverRootDirectory ()
    {
      return Path.Combine(Path.GetTempPath(), c_webDriverFolderName);
    }

    private string GetChromeDriverTempDirectory (string chromeDriverVersion)
    {
      return Path.Combine(GetChromeDriverRootDirectory(), string.Format("chromedriver_v{0}", chromeDriverVersion));
    }

    private void RemoveChromeDriverRootDirectoryIfExists ()
    {
      var chromeDriverRootDirectory = GetChromeDriverRootDirectory();
      if (Directory.Exists(chromeDriverRootDirectory))
        Directory.Delete(chromeDriverRootDirectory, true);
    }

    private void DownloadChromeDriver (string chromeDriverVersion, string url)
    {
      var tempPath = GetChromeDriverTempDirectory(chromeDriverVersion);
      var fullZipPath = Path.Combine(Path.GetTempPath(), c_zipFileName);

      RemoveChromeDriverRootDirectoryIfExists();
      Directory.CreateDirectory(tempPath);

      try
      {
#pragma warning disable SYSLIB0014
        using (var webClient = new WebClient()) // TODO RM-8492: Replace with HttpClient
#pragma warning restore SYSLIB0014
        {
          webClient.DownloadFile(url, fullZipPath);
        }
      }
      catch (WebException ex)
      {
        throw new WebException($"Could not download the ChromeDriver for Chrome v{chromeDriverVersion} from '{url}': {ex.Message}", ex.Status);
      }

      ZipFile.ExtractToDirectory(fullZipPath, tempPath);

      File.Delete(fullZipPath);
    }

    private string GetDriverDownloadUrl (string chromeDriverVersion)
    {
      return string.Format(c_driverDownloadUrlFormat, chromeDriverVersion);
    }

    private string GetDownloadUrlForChrome115AndLater (Version chromeVersion)
    {
      string jsonString;
      try
      {
#pragma warning disable SYSLIB0014
        using (var webClient = new WebClient()) // TODO RM-8492: Replace with HttpClient
#pragma warning restore SYSLIB0014
        {
          jsonString = webClient.DownloadString(c_chromeJsonApiUrl);
        }
      }
      catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
      {
        throw new InvalidOperationException(
            $"No matching ChromeDriver could be found for the installed Chrome version {chromeVersion}."
            + "This could mean that no corresponding ChromeDriver has been released for the version of Chrome you are using.",
            ex);
      }
      catch (WebException ex)
      {
        throw new WebException($"Could not fetch the latest ChromeDriver version from '{c_chromeJsonApiUrl}': {ex.Message}", ex.Status);
      }

      try
      {
        var chromeVersionStringWithoutRevision = chromeVersion.ToString(3);
        var knownGoodVersionsWithDownloads = DataContractJsonSerializationUtility.Deserialize<ChromeKnownGoodVersionsWithDownloads>(jsonString)!;

        // We might not get an exact match of our Chrome version so we only test for major.minor.build and use the latest version
        var knownGoodVersionWithDownloads = knownGoodVersionsWithDownloads.Versions.LastOrDefault(e => e.Version.StartsWith(chromeVersionStringWithoutRevision));
        if (knownGoodVersionWithDownloads == null)
          throw new InvalidOperationException($"Could not determine the chromedriver download location because Chrome version {chromeVersion} was not found in the JSON API response.");

        var artifactDownload = knownGoodVersionWithDownloads.Downloads.ChromeDriverArtifacts?.FirstOrDefault(e => e.Platform == "win32");
        if (artifactDownload == null)
          throw new InvalidOperationException($"Could not determine the chromedriver download location because the download link could not be found for Chrome version {chromeVersion}.");

        return artifactDownload.Url;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Could not determine the chromedriver download location from the JSON API response.", ex);
      }
    }
  }
}
