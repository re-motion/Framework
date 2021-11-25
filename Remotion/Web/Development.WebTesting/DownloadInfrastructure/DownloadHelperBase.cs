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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{
  /// <summary>
  /// Base class for implementations of <see cref="IDownloadHelper"/> to be used when using browser-specific download helpers.
  /// </summary>
  public abstract class DownloadHelperBase : IDownloadHelper
  {
    private readonly TimeSpan _downloadStartedTimeout;
    private readonly TimeSpan _downloadUpdatedTimeout;
    private readonly List<string> _tempDirectories = new List<string>();

    protected DownloadHelperBase (TimeSpan downloadStartedTimeout, TimeSpan downloadUpdatedTimeout)
    {
      _downloadStartedTimeout = downloadStartedTimeout;
      _downloadUpdatedTimeout = downloadUpdatedTimeout;
    }
    
    protected abstract DownloadedFileFinder CreateDownloadedFileFinderForExpectedFileName ([NotNull] string fileName);
    protected abstract DownloadedFileFinder CreateDownloadedFileFinderForUnknownFileName ();
    protected abstract IDownloadedFile HandleDownload ([NotNull] DownloadedFileFinder downloadedFileFinder, TimeSpan downloadStartedTimeout, TimeSpan downloadUpdatedTimeout);

    protected abstract void AdditionalCleanup ();

    public IDownloadedFile HandleDownloadWithExpectedFileName (string fileName, TimeSpan? downloadStartedTimeout = null, TimeSpan? downloadUpdatedTimeout = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty("fileName", fileName);

      var localDownloadStartedTimeout = downloadStartedTimeout ?? _downloadStartedTimeout;
      var localDownloadUpdatedTimeout = downloadUpdatedTimeout ?? _downloadUpdatedTimeout;

      return HandleDownload(CreateDownloadedFileFinderForExpectedFileName(fileName), localDownloadStartedTimeout, localDownloadUpdatedTimeout);
    }

    public IDownloadedFile HandleDownloadWithDetectedFileName (TimeSpan? downloadStartedTimeout = null, TimeSpan? downloadUpdatedTimeout = null)
    {
      var localDownloadStartedTimeout = downloadStartedTimeout ?? _downloadStartedTimeout;
      var localDownloadUpdatedTimeout = downloadUpdatedTimeout ?? _downloadUpdatedTimeout;
      
      return HandleDownload(CreateDownloadedFileFinderForUnknownFileName(), localDownloadStartedTimeout, localDownloadUpdatedTimeout);
    }
    
    public void DeleteFiles ()
    {
      foreach (var tempDirectory in _tempDirectories)
      {
        if (Directory.Exists(tempDirectory))
        {
          try
          {
            Directory.Delete(tempDirectory, true);
          }
          catch (Exception)
          {
            //We really do not care if this temp directory could not be deleted.
          }
        }
      }
      AdditionalCleanup();
    }

    /// <summary>
    /// Moves the <paramref name="downloadedFile"/> to the temp directory maintained by the <see cref="DownloadHelperBase"/>. 
    /// This will prevent the file from interfering with future downloads. 
    /// </summary>
    [NotNull] 
    protected IDownloadedFile MoveDownloadedFile ([NotNull] DownloadedFile downloadedFile)
    {
      ArgumentUtility.CheckNotNull("downloadedFile", downloadedFile);

      var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory(tempDirectory);
      _tempDirectories.Add(tempDirectory);

      var newFilePath = Path.Combine(tempDirectory, Path.GetFileName(downloadedFile.FullFilePath));

      return downloadedFile.Move(newFilePath);
    }
  }
}