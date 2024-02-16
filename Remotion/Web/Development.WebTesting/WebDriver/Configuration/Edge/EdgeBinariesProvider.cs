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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge
{
  /// <summary>
  /// Provides Edge and msedgedriver binaries and a custom user directory.
  /// </summary>
  public class EdgeBinariesProvider
  {
    private const string c_webDriverFolderName = @"Remotion.Web.Development.WebTesting.WebDriver\msedgedriver";

    private const string c_driverDownloadUrlFormat = "https://msedgedriver.azureedge.net/{0}/edgedriver_win32.zip";

    private const string c_driverExecutableName = "msedgedriver.exe";
    private const string c_edgeExecutableName = "msedge.exe";
    private const string c_zipFileName = "msedgedriver.zip";

    private static readonly Version s_minimumSupportedEdgeVersion = new Version(119, 0);

    /// <summary>
    /// Returns the <see cref="EdgeExecutable"/> that contains the installed Edge browser location, the corresponding msedgedriver location,
    /// and a temporary user directory. If no msegedriver exists, it is downloaded.
    /// </summary>
    public EdgeExecutable GetInstalledExecutable ()
    {
      var browserPath = GetInstalledEdgePath();
      var driverPath = GetDriverPathAndDownloadIfMissing(browserPath);
      var userDirectory = GetUserDirectoryTempPath();

      return new EdgeExecutable(browserPath, driverPath, userDirectory);
    }

    private string GetInstalledEdgePath ()
    {
      var x86EdgePath = Path.Combine(Get32BitProgramFilesPath(), "Microsoft", "Edge", "Application", c_edgeExecutableName);

      if (File.Exists(x86EdgePath))
        return x86EdgePath;

      if (!Environment.Is64BitOperatingSystem)
        throw new InvalidOperationException($"No stable Edge version could be found at '{x86EdgePath}'.");

      var x64EdgePath = Path.Combine(Get64BitProgramFilesPath(), "Microsoft", "Edge", "Application", c_edgeExecutableName);

      if (File.Exists(x64EdgePath))
        return x64EdgePath;

      throw new InvalidOperationException($"No stable Edge version could be found at '{x64EdgePath}' or at '{x86EdgePath}'.");
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
    /// Gets path of the msedgedriver needed for the Edge executable version. If there is no driver, it is downloaded.
    /// </summary>
    private string GetDriverPathAndDownloadIfMissing ([NotNull] string edgePath)
    {
      var edgeFileVersion = GetFileVersion(edgePath);

      if (edgeFileVersion < s_minimumSupportedEdgeVersion)
      {
        throw new NotSupportedException(
            string.Format(
                "The installed Edge version ({0}) is lower than the minimum required version of {1}.",
                edgeFileVersion,
                s_minimumSupportedEdgeVersion));
      }

      var edgeVersion = edgeFileVersion.ToString();
      var driverPath = GetEdgeDriverTempPath(edgeVersion);

      if (!EdgeDriverExists(edgeVersion))
        DownloadEdgeDriver(edgeVersion);

      return driverPath;
    }

    private Version GetFileVersion (string filePath)
    {
      var fileVersion = Assertion.IsNotNull(FileVersionInfo.GetVersionInfo(filePath).FileVersion, "File version could not be read from '{0}'.", filePath);
      return Version.Parse(fileVersion);
    }

    private bool EdgeDriverExists (string driverVersion)
    {
      return File.Exists(GetEdgeDriverTempPath(driverVersion));
    }

    private string GetEdgeDriverTempPath (string driverVersion)
    {
      return Path.Combine(GetEdgeDriverTempDirectory(driverVersion), c_driverExecutableName);
    }

    private string GetUserDirectoryTempPath ()
    {
      return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    private string GetEdgeDriverRootDirectory ()
    {
      return Path.Combine(Path.GetTempPath(), c_webDriverFolderName);
    }

    private string GetEdgeDriverTempDirectory (string driverVersion)
    {
      return Path.Combine(GetEdgeDriverRootDirectory(), $"msedgedriver_v{driverVersion}");
    }

    private void RemoveEdgeDriverRootDirectoryIfExists ()
    {
      var edgeDriverRootDirectory = GetEdgeDriverRootDirectory();
      if (Directory.Exists(edgeDriverRootDirectory))
        Directory.Delete(edgeDriverRootDirectory, true);
    }

    private void DownloadEdgeDriver (string edgeDriverVersion)
    {
      var tempPath = GetEdgeDriverTempDirectory(edgeDriverVersion);
      var fullZipPath = Path.Combine(Path.GetTempPath(), c_zipFileName);

      var url = GetDriverDownloadUrl(edgeDriverVersion);

      RemoveEdgeDriverRootDirectoryIfExists();
      Directory.CreateDirectory(tempPath);

      try
      {
        FileDownloadUtility.DownloadFileWithRetry(url, fullZipPath);
      }
      catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
      {
        throw new InvalidOperationException(
            $"No matching EdgeDriver could be found for the installed Edge version {edgeDriverVersion}. "
            + "This could mean that no corresponding EdgeDriver has been released for the version of Edge you are using.",
            ex);
      }
      catch (WebException ex)
      {
        throw new WebException($"Could not download the EdgeDriver for Edge v{edgeDriverVersion} from '{url}': {ex.Message}", ex.Status);
      }

      ZipFile.ExtractToDirectory(fullZipPath, tempPath);

      File.Delete(fullZipPath);
    }

    private string GetDriverDownloadUrl (string edgeDriverVersion)
    {
      return string.Format(c_driverDownloadUrlFormat, edgeDriverVersion);
    }
  }
}
