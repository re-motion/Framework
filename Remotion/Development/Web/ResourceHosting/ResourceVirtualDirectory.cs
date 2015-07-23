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
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Remotion.Utilities;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// Represents a physical resource directory as a virtual directory.
  /// </summary>
  public class ResourceVirtualDirectory : VirtualDirectory
  {
    private readonly string _virtualPath;
    private readonly DirectoryInfo _physicalDirectory;
    private readonly string _displayName;

    public ResourceVirtualDirectory (string virtualPath, DirectoryInfo physicalDirectory, string displayName = null)
        : base (virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("virtualPath", virtualPath);
      ArgumentUtility.CheckNotNull ("physicalDirectory", physicalDirectory);

      _virtualPath = virtualPath;
      _physicalDirectory = physicalDirectory;
      _displayName = displayName;
    }

    public virtual string AppRelativeVirtualPath
    {
      get { return _virtualPath;  }
    }
    
    public virtual string PhysicalPath
    {
      get
      {
        if (!Exists)
          return null;
        return _physicalDirectory.FullName;
      }
    }

    public virtual bool Exists
    {
      get { return _physicalDirectory != null && _physicalDirectory.Exists; }
    }

    public override string Name
    {
      get { return _displayName ?? base.Name; }
    }

    public override IEnumerable Directories
    {
      get
      {
        if (Exists)
        {
          foreach (var directory in _physicalDirectory.GetDirectories())
          {
            var path = VirtualPathUtility.AppendTrailingSlash (VirtualPathUtility.Combine (AppRelativeVirtualPath, directory.Name));
            yield return new ResourceVirtualDirectory (path, directory);
          }
        }
      }
    }

    public override IEnumerable Files
    {
      get
      {
        if (Exists)
        {
          foreach (var file in _physicalDirectory.GetFiles())
          {
            var path = VirtualPathUtility.Combine (AppRelativeVirtualPath, file.Name);
            yield return new ResourceVirtualFile (path, file);
          }
        }
      }
    }

    public override IEnumerable Children
    {
      get
      {
        foreach (var directory in Directories)
          yield return directory;

        foreach (var file in Files)
          yield return file;
      }
    }
  }
}