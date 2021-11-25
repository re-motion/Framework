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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class FileSystemReflectionBusinessObjectStorageProvider : IReflectionBusinessObjectStorageProvider
  {
    private readonly string _rootPath;

    public FileSystemReflectionBusinessObjectStorageProvider (string rootPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("rootPath", rootPath);

      _rootPath = rootPath;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> GetObjectIDsForType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var typeDirectory = GetDirectoryForType(type);

      try
      {
        var objectIDs = new List<Guid>();

        var directoryInfo = new DirectoryInfo(typeDirectory);
        foreach (var filePath in directoryInfo.EnumerateFiles())
        {
          if (Guid.TryParse(filePath.Name, out var objectID))
            objectIDs.Add(objectID);
        }

        return objectIDs;
      }
      catch (DirectoryNotFoundException)
      {
        return Array.Empty<Guid>();
      }
    }

    /// <inheritdoc />
    public Stream GetReadObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      try
      {
        var fileName = GetFileNameForTypeAndID(type, id);
        return new FileStream(fileName, FileMode.Open, FileAccess.Read);
      }
      catch (FileNotFoundException)
      {
        return null;
      }
    }

    /// <inheritdoc />
    public Stream GetWriteObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      Directory.CreateDirectory(GetDirectoryForType(type));

      var fileName = GetFileNameForTypeAndID(type, id);
      return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
    }

    private string GetFileNameForTypeAndID (Type type, Guid id)
    {
      return Path.Combine(GetDirectoryForType(type), id.ToString());
    }

    private string GetDirectoryForType (Type type)
    {
      return Path.Combine(_rootPath, type.GetFullNameChecked());
    }
  }
}
