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

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{
  /// <summary>
  /// Represents the strategy used by the heuristic of <see cref="DownloadedFileFinder"/> to find the correct downloaded file.
  /// </summary>
  public interface IDownloadFileFinderStrategy
  {
    /// <summary>
    /// Returns the name of the downloaded file, if we can uniquely identify the downloaded file from the newFiles collection. 
    /// Throws <see cref="FileNotFoundException"/> if it cant uniquely identify the downloaded file.
    /// </summary>
    string FindDownloadedFile ([NotNull] IReadOnlyCollection<string> newFiles);

    /// <summary>
    /// Checks if the detected newFiles contain a pre download artifact.
    /// </summary>
    bool ContainsPreDownloadFiles ([NotNull] IReadOnlyCollection<string> newFiles);
  }
}
