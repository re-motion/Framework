﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web.Hosting;
using Remotion.Utilities;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// Represents a physical resource file as a virtual file.
  /// </summary>
  public class ResourceVirtualFile : VirtualFile
  {
    private readonly FileInfo? _physicalFile;

    public ResourceVirtualFile (string virtualPath, FileInfo? physicalFile)
        : base(virtualPath)
    {
      // TODO RM-8063: The constructor of ResourceVirtualFile should not accept null FileInfo arguments
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      _physicalFile = physicalFile;
    }

    public string? PhysicalPath
    {
      get
      {
        if (!Exists)
          return null;
        return _physicalFile.FullName;
      }
    }

    [MemberNotNullWhen(true, nameof(_physicalFile))]
    [MemberNotNullWhen(true, nameof(PhysicalPath))]
    public bool Exists
    {
      get { return _physicalFile != null && _physicalFile.Exists; }
    }

    public override string Name
    {
      get { return base.Name!; }
    }

    public override Stream Open ()
    {
      return Assertion.IsNotNull(_physicalFile, "'_physicalFile' must not be null to be able to open a Stream.").OpenRead();
    }
  }
}
