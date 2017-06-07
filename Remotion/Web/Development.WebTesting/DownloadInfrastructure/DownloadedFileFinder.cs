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
    private readonly string _partialFileEnding;
    private readonly IDownloadFileFinderStrategy _downloadFileFinderStrategy;
    private readonly TimeSpan _downloadStartedGracePeriod;


    /// <summary>
    /// Creates a new <see cref="DownloadedFileFinder"/>.
    /// </summary>
    /// <param name="downloadDirectory">
    /// Directory where the browser saves the downloaded files.
    /// </param>
    /// <param name="partialFileEnding">
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
      [NotNull] string partialFileEnding,
      TimeSpan downloadStartedGracePeriod,
      [NotNull] IDownloadFileFinderStrategy downloadFileFinderStrategy)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("downloadDirectory", downloadDirectory);
      ArgumentUtility.CheckNotNullOrEmpty ("partialFileEnding", partialFileEnding);
      ArgumentUtility.CheckNotNull ("downloadFileFinderStrategy", downloadFileFinderStrategy);

      _downloadDirectory = downloadDirectory;
      _partialFileEnding = partialFileEnding;
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

      PartialFileState partialFileState = null;
      var downloadTimeWithoutUpdate = Stopwatch.StartNew();
      var startedDownloadHandlingUtc = DateTime.UtcNow;
      var currentStateTimeout = downloadStartedTimeout;

      while (downloadTimeWithoutUpdate.ElapsedMilliseconds < currentStateTimeout.TotalMilliseconds)
      {
        var newFiles = GetNewFiles (filesInDownloadDirectoryBeforeDownload, startedDownloadHandlingUtc);

         //The first step, we haven't found a partial File yet to monitor the download process
        if (partialFileState == null)
        {
          var partialFile = GetPartialFile (newFiles);
          //Browser has started the download, remember the partial File for further monitoring
          if (partialFile != null)
          {
            var fileInfo = GetFileInformation (partialFile);

            if (fileInfo != null)
            {
              partialFileState = new PartialFileState (partialFile, fileInfo.LastWriteTimeUtc, fileInfo.Length);
              currentStateTimeout = downloadUpdatedTimeout;
            }
          }
          //No partial file found (yet) 
          else if (
              //Some browser create a temporary file shortly before creating a partial file, 
              //so if that happens we know that a partial file is going to come
              !_downloadFileFinderStrategy.ContainsPreDownloadFiles (newFiles)
              //Try to find the downloaded file
              && newFiles.Any())
          {
            var fileName = _downloadFileFinderStrategy.FindDownloadedFile (newFiles);

            return new DownloadedFile (Path.Combine (_downloadDirectory, fileName), fileName);
          }
        }
        //We already have found a partial file which we can use to monitor the state of the download
        else
        {
          //If we cant find the partial file anymore, we assume the download is finished
          if (!newFiles.Contains (partialFileState.GetPartialFile()))
          {
            var fileName = _downloadFileFinderStrategy.FindDownloadedFile (newFiles);

            return new DownloadedFile (Path.Combine (_downloadDirectory, fileName), fileName);
          }
          
          if (FileGotUpdated (
              partialFileState.GetPartialFile(),
              partialFileState.GetPartialFileLastWriteTimeUtc(),
              partialFileState.GetPartialFileLength()))
          {
            var fileInfo = GetFileInformation (partialFileState.GetPartialFile());

            //Download In process, update last write time
            if (fileInfo != null)
            {
              partialFileState.UpdatePartialFileLastWriteAccessUtc (fileInfo.LastWriteTimeUtc);
              partialFileState.UpdatePartialFileLength (fileInfo.Length);
            }

            downloadTimeWithoutUpdate.Restart();
          }
        }

        Thread.Sleep (s_retryInterval);
      }

      if (partialFileState == null)
      {
        throw new DownloadResultNotFoundException ("Did not find any new files in the download directory.", new List<string>());
      }
      
      var unmatchedFiles = GetNewFiles (filesInDownloadDirectoryBeforeDownload, startedDownloadHandlingUtc);

      throw new DownloadResultNotFoundException (
          string.Format (
              "The download result file did not get updated for longer than the downloadUpdatedTimeout of '{0}'. The download appears to have failed.",
              downloadUpdatedTimeout),
          unmatchedFiles);
    }

    private List<string> GetNewFiles (IEnumerable<string> filesInDownloadDirectoryBeforeDownload, DateTime startedDownloadHandling)
    {
      return Directory.GetFiles (_downloadDirectory)
          .Where (file => FileGotCreatedAfterDownloadHandlingStarted (file, startedDownloadHandling))
          .Select (Path.GetFileName)
          .Except (filesInDownloadDirectoryBeforeDownload)
          .ToList();
    }

    [CanBeNull]
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
        return fileInfo.CreationTimeUtc - _downloadStartedGracePeriod < startedDownloadHandling;
      }
      catch (IOException)
      {
        //The file does not exist anymore --> Not relevant for our query
        return false;
      }
    }

    private bool FileGotUpdated (string oldPartialFile, DateTime oldPartialFileLastWriteAccessUtc, long length)
    {
      var fileInfo = new FileInfo (Path.Combine (_downloadDirectory, oldPartialFile));
      try
      {
        return fileInfo.LastWriteTimeUtc != oldPartialFileLastWriteAccessUtc || fileInfo.Length != length;
      }
      catch (IOException)
      {
        //Technically, the file got updated by disappearing. The calling function should be able to handle that the file is not there anymore
        return true;
      }
    }

    [CanBeNull]
    private string GetPartialFile (IEnumerable<string> newFiles)
    {
      return newFiles.SingleOrDefault (file => file.EndsWith (_partialFileEnding));
    }
  }
}