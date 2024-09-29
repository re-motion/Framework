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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.Default
{
  /// <summary>
  /// Responsible for default download handling.
  /// </summary>
  public class DefaultDownloadHelper : DownloadHelperBase
  {
    private readonly ILogger _logger;
    private string DownloadDirectory { get; }
    private string PartialFileExtension { get; }
    public TimeSpan DownloadStartedGracePeriod { get; }
    public bool CleanUpDownloadFolderOnError { get; }

    /// <summary>
    /// Creates a new <see cref="DefaultDownloadHelper"/>.
    /// </summary>
    /// <param name="downloadDirectory">
    ///   Directory where the browser saves the downloaded files. Must not be <see langword="null" /> or empty.
    /// </param>
    /// <param name="partialFileExtension">
    ///   File extension for partially downloaded files.
    /// </param>
    /// <param name="downloadStartedTimeout">
    ///   Specifies how long the <see cref="DownloadHelperBase"/> should wait before looking for the downloaded file.
    /// </param>
    /// <param name="downloadUpdatedTimeout">
    ///   Specifies how long the <see cref="DownloadHelperBase"/> should wait for a downloaded partial file to update.
    /// </param>
    /// <param name="downloadStartedGracePeriod">
    ///   Specifies the max time between triggering the download and calling <see cref="DownloadedFileFinder"/>.<see cref="DownloadedFileFinder.WaitForDownloadCompleted"/>. 
    ///   All files created before this time span will be ignored.
    /// </param>
    /// <param name="cleanUpDownloadFolderOnError">
    ///   Clean up the download folder on error.
    /// </param>
    /// <param name="loggerFactory"></param>
    public DefaultDownloadHelper (
        [NotNull] string downloadDirectory,
        [NotNull] string partialFileExtension,
        TimeSpan downloadStartedTimeout,
        TimeSpan downloadUpdatedTimeout,
        TimeSpan downloadStartedGracePeriod,
        bool cleanUpDownloadFolderOnError,
        ILoggerFactory loggerFactory)
        : base(downloadStartedTimeout, downloadUpdatedTimeout)
    {
      ArgumentUtility.CheckNotNullOrEmpty("downloadDirectory", downloadDirectory);
      ArgumentUtility.CheckNotNullOrEmpty("partialFileExtension", partialFileExtension);

      _logger = loggerFactory.CreateLogger<DefaultDownloadHelper>();
      DownloadDirectory = downloadDirectory;
      PartialFileExtension = partialFileExtension;
      DownloadStartedGracePeriod = downloadStartedGracePeriod;
      CleanUpDownloadFolderOnError = cleanUpDownloadFolderOnError;
    }

    protected override IDownloadedFile HandleDownload (
        DownloadedFileFinder downloadedFileFinder,
        TimeSpan downloadStartedTimeout,
        TimeSpan downloadUpdatedTimeout)
    {
      ArgumentUtility.CheckNotNull("downloadedFileFinder", downloadedFileFinder);

      EnsureDownloadDirectoryExists(DownloadDirectory);

      //Empty list, as our infrastructure should keep the download directory clean by moving downloaded files away, so we can assume the download directory is empty.
      //We need this assumption since files are downloaded without a prompt, making it impossible to get the directory state before the download starts.
      var filesInDownloadDirectoryBeforeDownload = new List<string>().AsReadOnly();

      DownloadedFile downloadedFile;

      try
      {
        downloadedFile = downloadedFileFinder.WaitForDownloadCompleted(
            downloadStartedTimeout,
            downloadUpdatedTimeout,
            filesInDownloadDirectoryBeforeDownload);
      }
      catch (DownloadResultNotFoundException ex)
      {
        if (CleanUpDownloadFolderOnError)
          CleanUpUnmatchedDownloadedFiles(ex.GetUnmatchedFilesInDownloadDirectory().ToList());

        throw;
      }

      // The downloaded file detection might race the browser finishing up the downloaded file, which might cause leftover files
      // To prevent/minimize this problem we wait a bit of time before proceeding to move the downloaded file
      Thread.Sleep(TimeSpan.FromMilliseconds(200));

      return MoveDownloadedFile(downloadedFile);
    }

    protected override DownloadedFileFinder CreateDownloadedFileFinderForExpectedFileName (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("fileName", fileName);

      return new DownloadedFileFinder(
          DownloadDirectory,
          PartialFileExtension,
          DownloadStartedGracePeriod,
          new DefaultNamedExpectedFileNameFinderStrategy(fileName),
          _logger);
    }

    protected override DownloadedFileFinder CreateDownloadedFileFinderForUnknownFileName ()
    {
      return new DownloadedFileFinder(
          DownloadDirectory,
          PartialFileExtension,
          DownloadStartedGracePeriod,
          new DefaultUnknownFileNameFinderStrategy(PartialFileExtension),
          _logger);
    }

    protected override void AdditionalCleanup ()
    {
      if (Directory.Exists(DownloadDirectory))
      {
        try
        {
          Directory.Delete(DownloadDirectory, true);
        }
        catch (IOException ex)
        {
          _logger.LogWarning(
              @"Could not delete '{0}'.
{1}",
              DownloadDirectory,
              ex);
        }
      }
    }

    private void EnsureDownloadDirectoryExists (string downloadDirectory)
    {
      if (!Directory.Exists(downloadDirectory))
        Directory.CreateDirectory(downloadDirectory);
    }

    private void CleanUpUnmatchedDownloadedFiles ([NotNull] IEnumerable<string> unmatchedFiles)
    {
      ArgumentUtility.CheckNotNull("unmatchedFiles", unmatchedFiles);

      foreach (var file in unmatchedFiles)
      {
        var fullFilePath = Path.Combine(DownloadDirectory, file);

        try
        {
          WaitUntilTrueOrTimeout(() => !IsFileLocked(fullFilePath), TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(1));

          File.Delete(fullFilePath);
        }
        catch (IOException ex)
        {
          _logger.LogWarning(
              @"Could not delete '{0}'.
{1}",
              fullFilePath,
              ex);
        }
      }
    }

    private void WaitUntilTrueOrTimeout (Func<bool> func, TimeSpan timeout, TimeSpan interval)
    {
      for (var elapsed = 0d; elapsed < timeout.TotalMilliseconds; elapsed += interval.TotalMilliseconds)
      {
        if (func.Invoke())
          return;

        Thread.Sleep(interval);
      }
    }

    private bool IsFileLocked (string filePath)
    {
      try
      {
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
        {
          stream.Close();
        }
      }
      catch (FileNotFoundException)
      {
        return false;
      }
      catch (IOException)
      {
        return true;
      }

      return false;
    }
  }
}
