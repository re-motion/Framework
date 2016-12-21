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
using System.Reflection;
using System.Web.Hosting;
using Remotion.Development.Web.ResourceHosting;

namespace Remotion.Development.UnitTests.Web.ResourceHosting
{
  public class TestableResourceVirtualPathProvider : ResourceVirtualPathProvider
  {
    private readonly string _projectRoot;
    private Func<string, string, string> _overrideCombineVirtualPathFunc;
    private Func<string, string, string> _overrideMakeRelativeVirtualPathFunc;
    private Func<string, string> _overrideMapPathFunc;

    public TestableResourceVirtualPathProvider (ResourcePathMapping[] mappings, string projectRoot)
        : base (mappings)
    {
      _projectRoot = projectRoot;
    }

    public void SetPrevious (VirtualPathProvider provider)
    {
      typeof (VirtualPathProvider).GetField ("_previous", BindingFlags.Instance | BindingFlags.NonPublic).SetValue (this, provider);
    }

    protected override string GetProjectRoot ()
    {
      return _projectRoot;
    }

    public void SetCombineVirtualPathOverride (Func<string, string, string> overrideFunc)
    {
      _overrideCombineVirtualPathFunc = overrideFunc;
    }

    protected override string CombineVirtualPath (string basePath, string relativePath)
    {
      if (_overrideCombineVirtualPathFunc != null)
        return _overrideCombineVirtualPathFunc (basePath, relativePath);
      return base.CombineVirtualPath (basePath, relativePath);
    }

    protected override string ToAppRelativeVirtualPath (string virtualPath)
    {
      return virtualPath;
    }

    public void SetMakeRelativeVirtualPathOverride (Func<string, string, string> overrideFunc)
    {
      _overrideMakeRelativeVirtualPathFunc = overrideFunc;
    }

    protected override string MakeRelativeVirtualPath (string fromPath, string toPath)
    {
      if (_overrideMakeRelativeVirtualPathFunc != null)
        return _overrideMakeRelativeVirtualPathFunc (fromPath, toPath);
      return base.MakeRelativeVirtualPath (fromPath, toPath);
    }

    public void SetMapPathOverride (Func<string, string> overrideFunc)
    {
      _overrideMapPathFunc = overrideFunc;
    }

    protected override string MapPath (string virtualPath)
    {
      if (_overrideMapPathFunc != null)
        return _overrideMapPathFunc (virtualPath);
      return base.MapPath (virtualPath);
    }
  }
}