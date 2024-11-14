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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Win32;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Firefox
{
  /// <summary>
  /// Provides Firefox and geckodriver binaries.
  /// </summary>
  public class FirefoxBinariesProvider
  {
    private const string c_latestDriverReleaseUrl = "https://api.github.com/repos/mozilla/geckodriver/releases/latest";
    private const string c_firefoxRegistryPath = @"SOFTWARE\Mozilla\Mozilla Firefox";
    private const string c_driverFolderName = @"Remotion.Web.Development.WebTesting.WebDriver\geckodriver";
    private const string c_driverExecutableName = "geckodriver.exe";
    private const string c_driverZipFileName = "geckodriver.zip";
    private const string c_driverBitness = "32";

    /// <summary>
    /// First Firefox version supported by <see cref="FirefoxBinariesProvider"/>.
    /// </summary>
    private static readonly Version s_minimumSupportedFirefoxVersion = new Version(130, 0);

    [NotNull]
    public FirefoxExecutable GetInstalledExecutable ()
    {
      var browserPath = GetInstalledFirefoxPath();
      var driverPath = GetDriverPathAndDownloadIfMissing(browserPath);

      return new FirefoxExecutable(browserPath, driverPath);
    }

    /// <summary>
    /// Retrieves the path of the installed Firefox version from the registry.
    /// </summary>
    private string GetInstalledFirefoxPath ()
    {
      var localMachine64BitViewKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

      // While Chrome overrides its registry entries whenever a different build of Chrome (e.g. Canary, Beta) is installed,
      // Firefox has distinct registry paths for each. In addition, all installed versions are listed with all locations,
      // meaning the newest version must be found and selected.
      var installedFirefoxSubKeys = localMachine64BitViewKey.OpenSubKey(c_firefoxRegistryPath)?.GetSubKeyNames();
      var latestFirefoxSubKeyName = installedFirefoxSubKeys?.OrderByDescending(x => new Version(x.Split(' ')[0])).FirstOrDefault();

      const string errorMessage = "Installed Firefox path could not be read from the registry ({0}).";
      Assertion.IsNotNull(latestFirefoxSubKeyName, errorMessage, c_firefoxRegistryPath);

      var installedFirefoxRegistryKey = localMachine64BitViewKey.OpenSubKey(Path.Combine(c_firefoxRegistryPath, latestFirefoxSubKeyName, "Main"));
      Assertion.IsNotNull(installedFirefoxRegistryKey, errorMessage, c_firefoxRegistryPath);

      return Assertion.IsNotNull(installedFirefoxRegistryKey.GetValue("PathToExe"), errorMessage, c_firefoxRegistryPath).ToString()!;
    }

    private string GetDriverPathAndDownloadIfMissing (string firefoxPath)
    {
      var firefoxVersion = GetFileVersion(firefoxPath);

      if (firefoxVersion < s_minimumSupportedFirefoxVersion)
      {
        throw new NotSupportedException(
            string.Format(
                "The installed Firefox version ({0}) is lower than the minimum required version of {1}.",
                firefoxVersion.ToString(1),
                s_minimumSupportedFirefoxVersion.ToString(1)));
      }

      var driverReleaseInfo = GetLatestGeckoDriverReleaseInfo();

      var driverDownloadUrl = GetBrowserDownloadUrl(driverReleaseInfo);
      var driverVersion = GetDriverVersion(driverReleaseInfo);

      var versionedGeckoDriverDirectory = GetVersionedDriverDirectory(driverVersion);

      if (!DriverExists(driverVersion))
        DownloadDriver(driverDownloadUrl, versionedGeckoDriverDirectory);

      return GetDriverPath(driverVersion);
    }

    private string GetDriverVersion (GithubResponse driverReleaseInfo)
    {
      var driverVersion = driverReleaseInfo.TagName?.TrimStart('v');
      return Assertion.IsNotNull(driverVersion, "Could not fetch the driver's version from GitHub. This could mean GitHub's API has changed.");
    }

    private string GetBrowserDownloadUrl (GithubResponse driverReleaseInfo)
    {
      var driverDownloadUrl = driverReleaseInfo.Assets
          .Select(asset => asset.BrowserDownloadUrl)
          .Where(url => url != null)
          .SingleOrDefault(url => url!.Contains($"win{c_driverBitness}"));

      return Assertion.IsNotNull(
          driverDownloadUrl,
          string.Format(
              "Could not fetch the driver's download URL from GitHub. This could mean GitHub's API has changed, or the release does not contain the win{0} archive.",
              c_driverBitness));
    }

    private GithubResponse GetLatestGeckoDriverReleaseInfo ()
    {
#pragma warning disable SYSLIB0014
      using (var webClient = new WebClient()) // TODO RM-8492: Replace with HttpClient
#pragma warning restore SYSLIB0014
      {
        // The user-agent header is required for github requests.
        webClient.Headers.Add("user-agent", "unknown");

        string githubResponseJson;

        try
        {
          githubResponseJson = webClient.DownloadString(c_latestDriverReleaseUrl);
        }
        catch (WebException ex)
        {
          throw new WebException($"Could not fetch the latest geckodriver download URL from '{c_latestDriverReleaseUrl}': {ex.Message}", ex.Status);
        }

        var serializer = new DataContractJsonSerializer(typeof(GithubResponse));
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(githubResponseJson)))
        {
          var githubResponse = (GithubResponse?)serializer.ReadObject(stream);
          Assertion.IsNotNull(githubResponse, "Could not parse the result of the GitHub API. The API might have changed.");
          return githubResponse;
        }
      }
    }

    private void DownloadDriver (string downloadUrl, string tempPath)
    {
      RemoveDriverRootDirectoryIfExists();
      Directory.CreateDirectory(tempPath);

      var fullZipPath = Path.Combine(Path.GetTempPath(), c_driverZipFileName);
      try
      {
        FileDownloadUtility.DownloadFileWithRetry(downloadUrl, fullZipPath);
      }
      catch (WebException ex)
      {
        throw new WebException($"Could not download the latest geckodriver from '{downloadUrl}': {ex.Message}", ex.Status);
      }

      ZipFile.ExtractToDirectory(fullZipPath, tempPath);
      File.Delete(fullZipPath);
    }

    private Version GetFileVersion (string filePath)
    {
      var fileVersion = Assertion.IsNotNull(FileVersionInfo.GetVersionInfo(filePath).FileVersion, "File version could not be read from '{0}'.", filePath);
      return Version.Parse(fileVersion);
    }

    private bool DriverExists (string driverVersion)
    {
      return File.Exists(GetDriverPath(driverVersion));
    }

    private void RemoveDriverRootDirectoryIfExists ()
    {
      var driverRootDirectory = GetDriverRootDirectory();
      if (Directory.Exists(driverRootDirectory))
        Directory.Delete(driverRootDirectory, true);
    }

    private string GetDriverPath (string driverVersion)
    {
      return Path.Combine(GetVersionedDriverDirectory(driverVersion), c_driverExecutableName);
    }

    private string GetVersionedDriverDirectory (string driverVersion)
    {
      return Path.Combine(GetDriverRootDirectory(), $"geckodriver_v{driverVersion}");
    }

    private string GetDriverRootDirectory ()
    {
      return Path.Combine(Path.GetTempPath(), c_driverFolderName);
    }
#nullable disable
    [DataContract]
    private class GithubResponse
    {
      [DataMember(Name = "tag_name")]
      public string TagName { get; set; }

      [DataMember(Name = "assets")]
      public Asset[] Assets { get; set; }

      [DataContract]
      public class Asset
      {
        [DataMember(Name = "browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
      }
    }
#nullable restore
  }
}
