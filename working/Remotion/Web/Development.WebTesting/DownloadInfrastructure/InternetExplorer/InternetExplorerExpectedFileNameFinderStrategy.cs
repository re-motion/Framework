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
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer
{
  /// <summary>
  /// Implements the <see cref="IDownloadFileFinderStrategy"/> interface for performing a download based on an expected file name with Internet Explorer.
  /// </summary>
  public class InternetExplorerExpectedFileNameFinderStrategy : IDownloadFileFinderStrategy
  {
    private readonly string _fileName;

    public InternetExplorerExpectedFileNameFinderStrategy ([NotNull] string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);

      _fileName = fileName;
    }

    public string FindDownloadedFile (IReadOnlyCollection<string> newFiles)
    {
      ArgumentUtility.CheckNotNull ("newFiles", newFiles);

      if (newFiles.Count == 0)
        throw new DownloadResultNotFoundException ("Did not find any new files in the download directory.", newFiles);
      
      var fileNameWithoutExtension = Path.GetFileNameWithoutExtension (_fileName);
      var fileNameExtension = Path.GetExtension (_fileName);
      Func<string, bool> findFileFunction;

      if (fileNameExtension == null || fileNameWithoutExtension == null)
      {
        findFileFunction = file => file == _fileName;
      }
      else
      {
        findFileFunction =
            file =>
                file == _fileName || 
                Regex.IsMatch (
                    file,
                    string.Format ("{0}([0-9A-Z]){{8}}{1}", Regex.Escape (fileNameWithoutExtension), Regex.Escape (fileNameExtension)));
      }

      var foundFiles = newFiles.Where (findFileFunction).ToArray();
      
      if (foundFiles.Length != 1)
      {
        throw new DownloadResultNotFoundException (string.Format (@"Did not find file with the name '{0}' in the download directory.", _fileName), newFiles);
      }

      return foundFiles.First();
    }

    public bool ContainsPreDownloadFiles (IReadOnlyCollection<string> newFiles)
    {
      ArgumentUtility.CheckNotNull ("newFiles", newFiles);
      
      //IE does not create pre download files, so we just return false
      return false;
    }
  }
}