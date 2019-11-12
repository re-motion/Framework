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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.Default
{
  /// <summary>
  /// Default implementation of the <see cref="IDownloadFileFinderStrategy"/> interface for performing a download with an unknown file.
  /// </summary>
  public class DefaultUnknownFileNameFinderStrategy : IDownloadFileFinderStrategy
  {
    private readonly string _partialFileEnding;

    public DefaultUnknownFileNameFinderStrategy ([NotNull] string partialFileEnding)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("partialFileEnding", partialFileEnding);

      _partialFileEnding = partialFileEnding;
    }

    public string FindDownloadedFile (IReadOnlyCollection<string> newFiles)
    {
      ArgumentUtility.CheckNotNull ("newFiles", newFiles);
      
      if (newFiles.Count == 0)
        throw new DownloadResultNotFoundException ("Did not find any new files in the download directory.", newFiles);
      
      if (newFiles.Count > 1)
      {
        throw new DownloadResultNotFoundException ("Multiple files found in the download directory but only expected one file.", newFiles);
      }

      var downloadedFoundFile = newFiles.Single();

      if (downloadedFoundFile.EndsWith (_partialFileEnding))
      {
        throw new DownloadResultNotFoundException (
          "Only a single file was found in the download directory. The file name indicates that the file belongs to an incomplete download.",
          newFiles);
      }
      
      return downloadedFoundFile;
    }

    public bool ContainsPreDownloadFiles (IReadOnlyCollection<string> newFiles)
    {
      ArgumentUtility.CheckNotNull ("newFiles", newFiles);
      
      return newFiles.Any(x => x.EndsWith (".tmp"));
    }
  }
}