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

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// This exception is thrown when the expectation for the downloaded file is not matched. 
  /// </summary>
  public class DownloadResultNotFoundException : Exception
  {
    private readonly IReadOnlyCollection<string> _unmatchedFilesInDownloadDirectory;

    public DownloadResultNotFoundException ([NotNull] string message, [NotNull] IReadOnlyCollection<string> unmatchedFiles)
        : base(FormatMessage(message, unmatchedFiles))
    {
      _unmatchedFilesInDownloadDirectory = unmatchedFiles;
    }

    private static string FormatMessage ([NotNull] string message, [NotNull] IReadOnlyCollection<string> unmatchedFiles)
    {
      ArgumentUtility.CheckNotNull("message", message);
      ArgumentUtility.CheckNotNull("unmatchedFiles", unmatchedFiles);

      if (!unmatchedFiles.Any())
        return message;

      return message + string.Format(
          @"

Unmatched files in the download directory (will be cleaned up by the infrastructure):
 - {0}",
          string.Join("\r\n - ", unmatchedFiles));
    }

    public IReadOnlyCollection<string> GetUnmatchedFilesInDownloadDirectory ()
    {
      return _unmatchedFilesInDownloadDirectory;
    }
  }
}