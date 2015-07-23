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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// Represents the translation of a physical path within the solution to a virtual path within the website.
  /// </summary>
  public class ResourcePathMapping
  {
    private readonly string _virtualPath;
    private readonly string _relativeFileSystemPath;

    public ResourcePathMapping (string virtualPath, string relativeFileSystemPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("virtualPath", virtualPath);
      ArgumentUtility.CheckNotNullOrEmpty ("relativeFileSystemPath", relativeFileSystemPath);

      _virtualPath = VirtualPathUtility.AppendTrailingSlash (virtualPath);
      _relativeFileSystemPath = relativeFileSystemPath;
    }

    public string RelativeFileSystemPath
    {
      get { return _relativeFileSystemPath; }
    }

    public string VirtualPath
    {
      get { return _virtualPath; }
    }
  }
}