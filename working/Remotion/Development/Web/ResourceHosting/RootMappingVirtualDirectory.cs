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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// Represents the virtual directory containing all the mapped virtual directories.
  /// </summary>
  public class RootMappingVirtualDirectory : ResourceVirtualDirectory
  {
    private readonly IEnumerable<ResourcePathMapping> _mappings;
    private readonly Func<string, ResourceVirtualDirectory> _virtualDirectoryFactory;

    public RootMappingVirtualDirectory (
        string virtualPath,
        IEnumerable<ResourcePathMapping> mappings,
        DirectoryInfo directoryInfo,
        Func<string, ResourceVirtualDirectory> virtualDirectoryFactory)
        : base (virtualPath, directoryInfo)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("virtualPath", virtualPath);
      ArgumentUtility.CheckNotNull ("mappings", mappings);
      ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
      ArgumentUtility.CheckNotNull ("virtualDirectoryFactory", virtualDirectoryFactory);

      _mappings = mappings;
      _virtualDirectoryFactory = virtualDirectoryFactory;
    }

    public override bool Exists
    {
      get { return true; }
    }

    public override IEnumerable Directories
    {
      get
      {
        foreach (var resourcePathMapping in _mappings)
        {
          yield return _virtualDirectoryFactory (
              VirtualPathUtility.AppendTrailingSlash (VirtualPathUtility.Combine (AppRelativeVirtualPath, resourcePathMapping.VirtualPath)));
        }
      }
    }

    public override IEnumerable Files
    {
      get { return Enumerable.Empty<ResourceVirtualFile>(); }
    }

    public override IEnumerable Children
    {
      get { return Directories; }
    }
  }
}