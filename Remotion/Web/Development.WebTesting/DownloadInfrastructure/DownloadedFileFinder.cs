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
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
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
    // TODO RM-7837: This class and related helpers should be reworked and simplified.

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
    

    private static readonly TimeSpan s_minimalDownloadTimeout = TimeSpan.FromSeconds (1);
    private static readonly TimeSpan s_retryInterval = TimeSpan.FromMilliseconds (250);

    private readonly string _downloadDirectory;
    private readonly string _partialFileExtension;
    private readonly IDownloadFileFinderStrategy _downloadFileFinderStrategy;
    private readonly TimeSpan _downloadStartedGracePeriod;

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
    public DownloadedFileFinder (
      [NotNull] string downloadDirectory,
      [NotNull] string partialFileExtension,
      TimeSpan downloadStartedGracePeriod,
      [NotNull] IDownloadFileFinderStrategy downloadFileFinderStrategy)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("downloadDirectory", downloadDirectory);
      ArgumentUtility.CheckNotNullOrEmpty ("partialFileExtension", partialFileExtension);
      ArgumentUtility.CheckNotNull ("downloadFileFinderStrategy", downloadFileFinderStrategy);

      _downloadDirectory = downloadDirectory;
      _partialFileExtension = partialFileExtension;
      _downloadStartedGracePeriod = downloadStartedGracePeriod;
      _downloadFileFinderStrategy = downloadFileFinderStrategy;
    }

    public DownloadedFile WaitForDownloadCompleted (
        TimeSpan downloadStartedTimeout,
        TimeSpan downloadUpdatedTimeout,
        [NotNull] IReadOnlyCollection<string> filesInDownloadDirectoryBeforeDownload)
    {
      ArgumentUtility.CheckNotNull ("filesInDownloadDirectoryBeforeDownload", filesInDownloadDirectoryBeforeDownload);

      if (downloadUpdatedTimeout < s_minimalDownloadTimeout)
        throw new ArgumentException (string.Format ("DownloadTimeout must not be less than '{0}'.", s_minimalDownloadTimeout));

      var downloadTimeWithoutUpdate = Stopwatch.StartNew();
      var startedDownloadHandlingUtc = DateTime.UtcNow;
      var currentStateTimeout = downloadStartedTimeout;
      List<PartialFileState> partialFileStates = new();
      var zeroLengthFileWasFoundInPreviousIteration = false;
      while (downloadTimeWithoutUpdate.ElapsedMilliseconds < currentStateTimeout.TotalMilliseconds)
      {
        var newFiles = GetNewFiles (filesInDownloadDirectoryBeforeDownload, startedDownloadHandlingUtc);

        if (!PartialFileDetected (partialFileStates)
            && TryGetPartialFiles (newFiles, out var foundPartialFiles)
            && TryGetFileInformations (foundPartialFiles, out var foundPartialFilesFileInfos))
        {
          partialFileStates.AddRange (foundPartialFilesFileInfos.Select (i => new PartialFileState (i.Key, i.Value.LastWriteTimeUtc, i.Value.Length)));
          currentStateTimeout = downloadUpdatedTimeout;
        }
        else if (PartialFileDetected (partialFileStates)
                 && PartialFileWasUpdated (partialFileStates)
                 && TryGetFileInformations (partialFileStates.Select (s => s.GetPartialFile()), out var fileInfosOfPartialFiles))
        {
          foreach (var partialFileState in partialFileStates)
          {
            var partialFileName = partialFileState.GetPartialFile();
            if (fileInfosOfPartialFiles.TryGetValue (partialFileName, out var fileInfo)
                && fileInfo != null)
            {
              partialFileState.UpdatePartialFileLastWriteAccessUtc (fileInfo.LastWriteTimeUtc);
              partialFileState.UpdatePartialFileLength (fileInfo.Length);
            }
          }

          downloadTimeWithoutUpdate.Restart();
        }
        else if (!HasPartialFile (newFiles) && !TemporaryFilesExist (newFiles) && !zeroLengthFileWasFoundInPreviousIteration && HasZeroLengthFile (newFiles))
        {
          zeroLengthFileWasFoundInPreviousIteration = true;
          currentStateTimeout = downloadUpdatedTimeout;
        }
        else if (!HasPartialFile (newFiles) && !TemporaryFilesExist (newFiles) && !HasZeroLengthFile (newFiles) && newFiles.Count > 0)
        {
          try
          {
            var fileName = _downloadFileFinderStrategy.FindDownloadedFile (newFiles);
            return new DownloadedFile (Path.Combine (_downloadDirectory, fileName), fileName);
          }
          catch (DownloadResultNotFoundException)
          {
            var elapsedTimeAfterSleep = downloadTimeWithoutUpdate.ElapsedMilliseconds + s_retryInterval.TotalMilliseconds;
            if (elapsedTimeAfterSleep >= currentStateTimeout.TotalMilliseconds)
              throw;

            Thread.Sleep (s_retryInterval);
          }
        }
        else
        {
          Thread.Sleep (s_retryInterval);
        }
      }

      if (!PartialFileDetected (partialFileStates) && !zeroLengthFileWasFoundInPreviousIteration)
        throw new DownloadResultNotFoundException ("Did not find any new files in the download directory.", new List<string>());

      var unmatchedFiles = GetNewFiles (filesInDownloadDirectoryBeforeDownload, startedDownloadHandlingUtc);

      throw new DownloadResultNotFoundException (
          string.Format (
              "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
              downloadUpdatedTimeout),
          unmatchedFiles);

      static bool PartialFileDetected (IReadOnlyCollection<PartialFileState> partialFileStates) => partialFileStates.Any();
    }

    private bool TemporaryFilesExist (List<string> newFiles)
    {
      return _downloadFileFinderStrategy.ContainsPreDownloadFiles (newFiles) && newFiles.Any();
    }

    private List<string> GetNewFiles (IEnumerable<string> filesInDownloadDirectoryBeforeDownload, DateTime startedDownloadHandling)
    {
      return Directory.GetFiles (_downloadDirectory)
          .Where (file => FileGotCreatedAfterDownloadHandlingStarted (file, startedDownloadHandling))
          .Select (Path.GetFileName)
          .Except (filesInDownloadDirectoryBeforeDownload)
          .ToList();
    }

    private bool TryGetFileInformations (IEnumerable<string> fileNames, out IDictionary<string, FileInformationTuple> fileInformations)
    {
      fileInformations = fileNames.Distinct()
          .Select (n => new { FileName = n, FileInformation = GetFileInformation (n) })
          .Where (o => o.FileInformation != null)
          .ToDictionary (o => o.FileName, o => o.FileInformation);

      return fileInformations.Any();
    }

    private FileInformationTuple GetFileInformation (string fileName)
    {
      var fileInfo = new FileInfo (Path.Combine (_downloadDirectory, fileName));

      try
      {
        var lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
        var length = fileInfo.Length;

        return new FileInformationTuple (lastWriteTimeUtc, length);
      }
      catch (IOException)
      {
        //File got removed while checking the file information
        return null;
      }
    }

    private bool FileGotCreatedAfterDownloadHandlingStarted (string file, DateTime startedDownloadHandling)
    {
      var fileInfo = new FileInfo (file);

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

    private bool PartialFileWasUpdated (IReadOnlyCollection<PartialFileState> partialFileStates)
    {
      return partialFileStates.Any (PartialFileWasUpdated);
    }

    private bool PartialFileWasUpdated (PartialFileState partialFileState)
    {
      var fileInfo = new FileInfo (Path.Combine (_downloadDirectory, partialFileState.GetPartialFile()));
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

    private bool TryGetPartialFiles (IEnumerable<string> newFiles, out IReadOnlyCollection<string> partialFiles)
    {
      partialFiles = newFiles.Where (file => file.EndsWith (_partialFileExtension)).ToArray();

      return partialFiles.Any();
    }

    private bool HasPartialFile (IEnumerable<string> newFiles)
    {
      return TryGetPartialFiles (newFiles, out _);
    }

    private bool HasZeroLengthFile (IEnumerable<string> newFiles)
    {
      var fileInfos = newFiles.Select (file => new FileInfo (Path.Combine (_downloadDirectory, file)));

      return fileInfos.Any (info => info.Length == 0);
    }
  }
}