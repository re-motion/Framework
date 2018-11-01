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

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{  
  /// <summary>
  /// Represents a file downloaded by the <see cref="Remotion.Web.Development.WebTesting.DownloadInfrastructure"/>.
  /// </summary>
  public interface IDownloadedFile  
  {
    /// <summary>
    /// Returns the original file name of the downloaded file.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// Returns the location of the downloaded file.
    /// Note that the file name may differ from <see cref="FileName"/> due to the way the download is processed by the server.
    /// </summary>
    string FullFilePath { get; }

      /// <summary>
    /// Returns the size of the file
    /// </summary>
    long FileLength { get; }

    /// <summary>
    /// Returns FileStream with FileMode.Open to the downloaded file.
    /// </summary>
    FileStream GetStream ();
    
    /// <summary>
    /// Deletes the downloaded file. Normally used by the <see cref="WebTestHelper"/> to delete downloaded files on test TearDown.
    /// </summary>
    void Delete ();
  }
}