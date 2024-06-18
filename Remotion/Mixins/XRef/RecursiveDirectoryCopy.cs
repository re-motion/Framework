// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.Mixins.XRef
{
  public static class RecursiveDirectoryCopy
  {
    public static void CopyTo (this DirectoryInfo sourceDirectory, string destinationDirectoryPath)
    {
      ArgumentUtility.CheckNotNull("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNull("destinationDirectoryPath", destinationDirectoryPath);

      if (!sourceDirectory.Exists)
        throw new DirectoryNotFoundException("source directory '" + sourceDirectory.FullName + "' not found ");

      DirectoryInfo destinationDirectory = new DirectoryInfo(destinationDirectoryPath);

      // does nothing if the directory already exists
      destinationDirectory.Create();

      // copy all files
      foreach (FileInfo file in sourceDirectory.GetFiles())
        file.CopyTo(Path.Combine(destinationDirectory.FullName, file.Name), true);

      // recursive call for subdirectories
      foreach (DirectoryInfo subDirectory in sourceDirectory.GetDirectories())
        CopyTo(subDirectory, Path.Combine(destinationDirectory.FullName, subDirectory.Name));
    }
  }
}
