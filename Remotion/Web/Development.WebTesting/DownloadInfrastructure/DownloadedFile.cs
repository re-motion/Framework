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
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{
  /// <summary>
  /// Implements the <see cref="IDownloadedFile"/> interface and provides methods for the webtesting infrastructure for manipulation of the downloaded file or the object.
  /// </summary>
  public class DownloadedFile : IDownloadedFile
  {
    private readonly string _fullFilePath;
    private readonly string _fileName;

    public DownloadedFile ([NotNull] string fullFilePath, [NotNull] string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("fullFilePath", fullFilePath);
      ArgumentUtility.CheckNotNullOrEmpty("fileName", fileName);

      _fullFilePath = fullFilePath;
      _fileName = fileName;
    }

    /// <inheritdoc/>
    public string FileName
    {
      get { return _fileName; }
    }

    /// <inheritdoc/>
    public string FullFilePath
    {
      get { return _fullFilePath; }
    }

    /// <inheritdoc/>
    public long FileLength
    {
      get { return new FileInfo(_fullFilePath).Length; }
    }

    /// <inheritdoc/>
    public FileStream GetStream ()
    {
      return new FileStream(_fullFilePath, FileMode.Open);
    }

    /// <inheritdoc/>
    public void Delete ()
    {
      File.Delete(_fullFilePath);
    }

    /// <summary>
    /// Provides a way for the <see cref="Remotion.Web.Development.WebTesting.DownloadInfrastructure"/> to move the downloaded file while retaining the original <see cref="FileName"/>.
    /// </summary>
    public IDownloadedFile Move ([NotNull] string newFilePath)
    {
      ArgumentUtility.CheckNotNull("newFilePath", newFilePath);

      // In racy cases the file we are trying to move is still used, resulting in an IOException.
      // So we retry the move a couple of times, allowing the browser to finish the download.
      return RetryUntilTimeout.Run(
          () =>
          {
            File.Move(_fullFilePath, newFilePath);

            return new DownloadedFile(newFilePath, _fileName);
          },
          TimeSpan.FromMilliseconds(500),
          TimeSpan.FromMilliseconds(50));
    }

    /// <summary>
    /// Provides a way for the <see cref="Remotion.Web.Development.WebTesting.DownloadInfrastructure"/> to rename the <see cref="FileName"/> property while retaining the original path.
    /// </summary>
    public DownloadedFile Rename ([NotNull] string newName)
    {
      ArgumentUtility.CheckNotNull("newName", newName);

      return new DownloadedFile(_fullFilePath, newName);
    }
  }
}
