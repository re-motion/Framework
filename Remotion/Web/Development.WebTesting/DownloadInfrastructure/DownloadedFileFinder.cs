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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{
  /// <summary>
  /// Provides a method to wait for a download to finish. 
  /// Monitors the download directory and uses a heuristic to find out when a download is finished.
  /// </summary>
  public class DownloadedFileFinder
  {
    private class FileInformationTuple
    {
      private readonly DateTime _lastWriteTimeUtc;
      private readonly long _length;

      public FileInformationTuple (DateTime lastWriteTimeUtc, long length)
      {
        _lastWriteTimeUtc = lastWriteTimeUtc;
        _length = length;
      }

      public DateTime LastWriteTimeUtc { get { return _lastWriteTimeUtc; } }
      public long Length { get { return _length; } }
    }


    private static readonly TimeSpan s_minimalDownloadTimeout = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan s_retryInterval = TimeSpan.FromMilliseconds(250);

    private readonly string _downloadDirectory;
    private readonly string _partialFileExtension;
    private readonly IDownloadFileFinderStrategy _downloadFileFinderStrategy;
    private readonly TimeSpan _downloadStartedGracePeriod;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates a new <see cref="DownloadedFileFinder"/>.
    /// </summary>
    /// <param name="downloadDirectory">
    /// Directory where the browser saves the downloaded files.
    /// </param>
    /// <param name="partialFileExtension">
    /// File ending of the partial file created by the browser.
    /// </param>
    /// <param name="downloadStartedGracePeriod">
    /// Specifies the max time between triggering the download and calling <see cref="WaitForDownloadCompleted"/>. 
    /// All files created before this timespan will be ignored.
    /// </param>
    /// <param name="downloadFileFinderStrategy">
    /// Specifies the <see cref="IDownloadFileFinderStrategy"/>.
    /// </param>
    /// <param name="logger">The <see cref="ILogger"/> used for diagnostic output when performing a file download</param>
    public DownloadedFileFinder (
      [JetBrains.Annotations.NotNull] string downloadDirectory,
      [JetBrains.Annotations.NotNull] string partialFileExtension,
      TimeSpan downloadStartedGracePeriod,
      [JetBrains.Annotations.NotNull] IDownloadFileFinderStrategy downloadFileFinderStrategy,
      [JetBrains.Annotations.NotNull] ILogger logger)
    {
      ArgumentUtility.CheckNotNullOrEmpty("downloadDirectory", downloadDirectory);
      ArgumentUtility.CheckNotNullOrEmpty("partialFileExtension", partialFileExtension);
      ArgumentUtility.CheckNotNull("downloadFileFinderStrategy", downloadFileFinderStrategy);
      ArgumentUtility.CheckNotNull("logger", logger);

      _downloadDirectory = downloadDirectory;
      _partialFileExtension = partialFileExtension;
      _downloadStartedGracePeriod = downloadStartedGracePeriod;
      _downloadFileFinderStrategy = downloadFileFinderStrategy;
      _logger = logger;
    }

    public DownloadedFile WaitForDownloadCompleted (
        TimeSpan downloadStartedTimeout,
        TimeSpan downloadUpdatedTimeout,
        [JetBrains.Annotations.NotNull] IReadOnlyCollection<string> filesInDownloadDirectoryBeforeDownload)
    {
      ArgumentUtility.CheckNotNull("filesInDownloadDirectoryBeforeDownload", filesInDownloadDirectoryBeforeDownload);

      if (downloadUpdatedTimeout < s_minimalDownloadTimeout)
        throw new ArgumentException(string.Format("DownloadTimeout must not be less than '{0}'.", s_minimalDownloadTimeout));

      var downloadTimeWithoutUpdate = Stopwatch.StartNew();
      var startedDownloadHandlingUtc = DateTime.UtcNow;
      var currentStateTimeout = downloadStartedTimeout;
      PartialFileState? partialFileStateOfCurrentDownload = null;
      var zeroLengthFileWasFoundInPreviousIteration = false;
      while (downloadTimeWithoutUpdate.ElapsedMilliseconds < currentStateTimeout.TotalMilliseconds)
      {
        var newFiles = GetNewFiles(filesInDownloadDirectoryBeforeDownload, startedDownloadHandlingUtc);

        if (!PartialFileWasFoundInPreviousIteration(partialFileStateOfCurrentDownload)
            && TryGetPartialFile(newFiles, out var partialFile)
            && TryGetFileInformation(partialFile, out var fileInfoOfFoundPartialFile))
        {
          partialFileStateOfCurrentDownload = new PartialFileState(
              partialFile,
              fileInfoOfFoundPartialFile.LastWriteTimeUtc,
              fileInfoOfFoundPartialFile.Length);
          currentStateTimeout = downloadUpdatedTimeout;
        }
        else if (PartialFileWasFoundInPreviousIteration(partialFileStateOfCurrentDownload)
                 && PartialFileWasUpdated(partialFileStateOfCurrentDownload)
                 && TryGetFileInformation(partialFileStateOfCurrentDownload.GetPartialFile(), out var fileInfoOfPartialFile))
        {
          partialFileStateOfCurrentDownload.UpdatePartialFileLastWriteAccessUtc(fileInfoOfPartialFile.LastWriteTimeUtc);
          partialFileStateOfCurrentDownload.UpdatePartialFileLength(fileInfoOfPartialFile.Length);

          downloadTimeWithoutUpdate.Restart();
        }
        else if (!HasPartialFile(newFiles) && !TemporaryFilesExist(newFiles) && !zeroLengthFileWasFoundInPreviousIteration && HasZeroLengthFile(newFiles))
        {
          zeroLengthFileWasFoundInPreviousIteration = true;
          currentStateTimeout = downloadUpdatedTimeout;
        }
        else if (!HasPartialFile(newFiles) && !TemporaryFilesExist(newFiles) && !HasZeroLengthFile(newFiles) && newFiles.Count > 0)
        {
          var fileName = _downloadFileFinderStrategy.FindDownloadedFile(newFiles);

          return new DownloadedFile(Path.Combine(_downloadDirectory, fileName), fileName, _logger);
        }
        else
        {
          Thread.Sleep(s_retryInterval);
        }
      }

      if (!PartialFileWasFoundInPreviousIteration(partialFileStateOfCurrentDownload) && !zeroLengthFileWasFoundInPreviousIteration)
        throw new DownloadResultNotFoundException("Did not find any new files in the download directory.", new List<string>());

      var unmatchedFiles = GetNewFiles(filesInDownloadDirectoryBeforeDownload, startedDownloadHandlingUtc);

      throw new DownloadResultNotFoundException(
          string.Format(
              "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
              downloadUpdatedTimeout),
          unmatchedFiles);

      static bool PartialFileWasFoundInPreviousIteration ([NotNullWhen(true)] PartialFileState? partialFileState) => partialFileState != null;
    }

    private bool TemporaryFilesExist (List<string> newFiles)
    {
      return _downloadFileFinderStrategy.ContainsPreDownloadFiles(newFiles) && newFiles.Any();
    }

    private List<string> GetNewFiles (IEnumerable<string> filesInDownloadDirectoryBeforeDownload, DateTime startedDownloadHandling)
    {
      return Directory.GetFiles(_downloadDirectory)
          .Where(file => FileGotCreatedAfterDownloadHandlingStarted(file, startedDownloadHandling))
          .Select(s => Path.GetFileName(s))
          .Except(filesInDownloadDirectoryBeforeDownload)
          .ToList();
    }

    private bool TryGetFileInformation (string fileName, [MaybeNullWhen(false)] out FileInformationTuple fileInformation)
    {
      var fileInfo = new FileInfo(Path.Combine(_downloadDirectory, fileName));

      try
      {
        var lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
        var length = fileInfo.Length;

        fileInformation = new FileInformationTuple(lastWriteTimeUtc, length);
        return true;
      }
      catch (IOException)
      {
        //File got removed while checking the file information
        fileInformation = null;
        return false;
      }
    }

    private bool FileGotCreatedAfterDownloadHandlingStarted (string file, DateTime startedDownloadHandling)
    {
      var fileInfo = new FileInfo(file);

      try
      {
        //Use fileInfo.LastWriteTimeUtc instead of fileInfo.CreationTimeUtc to check when the file got created.
        //This is due Windows file tunneling, which caches the creation time of files with the same name. 
        //For more information see https://support.microsoft.com/en-us/help/172190/windows-nt-contains-file-system-tunneling-capabilities.
        return startedDownloadHandling < fileInfo.LastWriteTimeUtc + _downloadStartedGracePeriod;
      }
      catch (IOException)
      {
        //The file does not exist anymore --> Not relevant for our query
        return false;
      }
    }

    private bool PartialFileWasUpdated (PartialFileState partialFileState)
    {
      var fileInfo = new FileInfo(Path.Combine(_downloadDirectory, partialFileState.GetPartialFile()));
      try
      {
        return fileInfo.LastWriteTimeUtc != partialFileState.GetPartialFileLastWriteTimeUtc() || fileInfo.Length != partialFileState.GetPartialFileLength();
      }
      catch (IOException)
      {
        //Technically, the file got updated by disappearing. The calling function should be able to handle that the file is not there anymore
        return true;
      }
    }

    private bool TryGetPartialFile (IEnumerable<string> newFiles, [MaybeNullWhen(false)] out string partialFile)
    {
      partialFile = newFiles.SingleOrDefault(file => file.EndsWith(_partialFileExtension));

      return partialFile != null;
    }

    private bool HasPartialFile (IEnumerable<string> newFiles)
    {
      return TryGetPartialFile(newFiles, out _);
    }

    private bool HasZeroLengthFile (IEnumerable<string> newFiles)
    {
      var fileInfos = newFiles.Select(file => new FileInfo(Path.Combine(_downloadDirectory, file)));

      return fileInfos.Any(info => info.Length == 0);
    }
  }
}
