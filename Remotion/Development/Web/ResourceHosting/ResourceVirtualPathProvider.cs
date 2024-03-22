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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Resources;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// Allows mapping of the re-motion resource directories as virtual directories.
  /// </summary>
  public class ResourceVirtualPathProvider : VirtualPathProvider
  {
    private static readonly IHttpHandler s_staticFileHandler;


    static ResourceVirtualPathProvider ()
    {
      var staticFileHandlerType = typeof(HttpApplication).Assembly.GetType("System.Web.StaticFileHandler", true)!;
      s_staticFileHandler = (IHttpHandler)Activator.CreateInstance(staticFileHandlerType, true)!;
    }

    public static IHttpHandler StaticFileHandler
    {
      get { return s_staticFileHandler; }
    }

    private readonly string _resourceRoot;
    private readonly Dictionary<string, ResourcePathMapping> _resourcePathMappings;
    private readonly Dictionary<string, FileExtensionHandlerMapping> _fileExtensionHandlerMappings;

    public ResourceVirtualPathProvider (ResourcePathMapping[] mappings, FileExtensionHandlerMapping[]? fileExtensionHandlerMappings = null)
    {
      ArgumentUtility.CheckNotNull("mappings", mappings);

      var resourceRoot = SafeServiceLocator.Current.GetInstance<ResourceRoot>();
      _resourceRoot = VirtualPathUtility.AppendTrailingSlash(CombineVirtualPath("~/", resourceRoot.Value));

      _resourcePathMappings = new Dictionary<string, ResourcePathMapping>(StringComparer.OrdinalIgnoreCase);
      foreach (var mapping in mappings)
        _resourcePathMappings.Add(CombineVirtualPath(_resourceRoot, mapping.VirtualPath), mapping);

      _fileExtensionHandlerMappings = (fileExtensionHandlerMappings ?? new FileExtensionHandlerMapping[0])
          .ToDictionary(f => f.Extension, StringComparer.OrdinalIgnoreCase);
    }

    public void Register ()
    {
      HostingEnvironment.RegisterVirtualPathProvider(this);
    }

    public void HandleBeginRequest ()
    {
      Assertion.IsNotNull(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath);

      if (IsMappedPath(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath))
      {
        if (DirectoryExists(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath))
          HttpContext.Current.RemapHandler(new DirectoryListingHandler());
        else
        {
          var extension = VirtualPathUtility.GetExtension(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath)?.TrimStart('.') ?? string.Empty;
          if (_fileExtensionHandlerMappings.ContainsKey(extension))
            HttpContext.Current.RemapHandler(_fileExtensionHandlerMappings[extension].Handler);
        }
      }
    }

    public bool IsMappedPath (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      var checkPath = ToAppRelativeVirtualPath(virtualPath);

      if (checkPath.Equals(_resourceRoot, StringComparison.OrdinalIgnoreCase))
        return true;

      if (!checkPath.StartsWith(_resourceRoot, StringComparison.OrdinalIgnoreCase))
        return false;

      return GetResourcePathMapping(checkPath) != null;
    }

    public override bool FileExists (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      if (IsMappedPath(virtualPath))
      {
        var file = GetResourceVirtualFile(virtualPath);
        return file.Exists;
      }

      return Previous.FileExists(virtualPath);
    }

    public override VirtualFile GetFile (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      if (IsMappedPath(virtualPath))
        return GetResourceVirtualFile(virtualPath);

      return Previous.GetFile(virtualPath);
    }

    public override bool DirectoryExists (string virtualDir)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualDir", virtualDir);

      if (IsMappedPath(virtualDir))
      {
        var virtualDirectory = GetResourceVirtualDirectory(virtualDir);
        return virtualDirectory.Exists;
      }

      return Previous.DirectoryExists(virtualDir);
    }

    public override VirtualDirectory GetDirectory (string virtualDir)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualDir", virtualDir);

      if (IsMappedPath(virtualDir))
        return GetResourceVirtualDirectory(virtualDir);

      return Previous.GetDirectory(virtualDir);
    }

    public override CacheDependency? GetCacheDependency (string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      if (IsMappedPath(virtualPath))
        return null;

      //based on MapPathBasedVirtualPathProvider:

      if (virtualPathDependencies == null)
        return (CacheDependency?)null;
      StringCollection? stringCollection = (StringCollection?)null;
      foreach (string virtualPath1 in virtualPathDependencies)
      {
        string? str;

        var appRelative = ToAppRelativeVirtualPath(virtualPath1);
        if (IsMappedPath(appRelative))
        {
          if (FileExists(appRelative))
            str = GetResourceVirtualFile(appRelative).PhysicalPath;
          else if (DirectoryExists(appRelative))
            str = GetResourceVirtualDirectory(appRelative).PhysicalPath;
          else
            throw new InvalidOperationException(string.Format("Virtual filesystem object '{0}' does not exist.", appRelative));
        }
        else
          str = HostingEnvironment.MapPath(virtualPath1);

        Assertion.IsNotNull(str, "The physical path of '{0}' must not be null.", appRelative);

        if (stringCollection == null)
          stringCollection = new StringCollection();
        stringCollection.Add(str);
      }
      if (stringCollection == null)
        return (CacheDependency?)null;
      string[] strArray = new string[stringCollection.Count];
      stringCollection.CopyTo(strArray, 0);
      return new CacheDependency(strArray, new string[0], utcStart);
    }

    private ResourceVirtualFile GetResourceVirtualFile (string virtualPath)
    {
      var appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualPath);
      var mapping = GetResourcePathMapping(appRelativeVirtualPath);

      FileInfo? physicalFile = null;
      if (mapping != null)
      {
        var resourceRootPath = CombineVirtualPath(_resourceRoot, mapping.VirtualPath);
        var directoryRelativePath = MakeRelativeVirtualPath(resourceRootPath, appRelativeVirtualPath).Replace('/', '\\');
        var directory = Path.GetFullPath(Path.Combine(GetProjectRoot(), mapping.RelativeFileSystemPath));
        var filePath = Path.Combine(directory, directoryRelativePath);

        physicalFile = new FileInfo(filePath);
      }

      return new ResourceVirtualFile(virtualPath, physicalFile);
    }

    private ResourceVirtualDirectory GetResourceVirtualDirectory (string virtualDir)
    {
      var appRelativeVirtualPath = ToAppRelativeVirtualPath(virtualDir);
      if (appRelativeVirtualPath.Equals(_resourceRoot, StringComparison.OrdinalIgnoreCase))
      {
        var projectRoot = MapPath(_resourceRoot);
        return new RootMappingVirtualDirectory(
            _resourceRoot,
            _resourcePathMappings.Values,
            new DirectoryInfo(projectRoot),
            v => GetResourceVirtualDirectory(v));
      }

      var mapping = GetResourcePathMapping(appRelativeVirtualPath);

      DirectoryInfo? directoryInfo = null;
      string? displayName = null;
      if (mapping != null)
      {
        displayName = mapping.VirtualPath.TrimEnd('/');
        var resourceRootPath = CombineVirtualPath(_resourceRoot, mapping.VirtualPath);
        var directoryRelativePath = MakeRelativeVirtualPath(resourceRootPath, appRelativeVirtualPath).Replace('/', '\\');
        var mappedRootDirectory = Path.GetFullPath(Path.Combine(GetProjectRoot(), mapping.RelativeFileSystemPath));
        var absolutePath = Path.Combine(mappedRootDirectory, directoryRelativePath);

        directoryInfo = new DirectoryInfo(absolutePath);
      }
      else
      {
        throw new InvalidOperationException(string.Format("No resource path mapping for '{0}' exists.", appRelativeVirtualPath));
      }

      return new ResourceVirtualDirectory(virtualDir, directoryInfo, displayName);
    }

    private ResourcePathMapping? GetResourcePathMapping (string appRelativePath)
    {
      var rootPath = VirtualPathUtility.AppendTrailingSlash(appRelativePath);
      if (_resourcePathMappings.ContainsKey(rootPath))
        return _resourcePathMappings[rootPath];

      var subDirectories = appRelativePath.Substring(_resourceRoot.Length).Split('/');
      for (var index = 0; index < subDirectories.Length; index++)
      {
        var partialPath = VirtualPathUtility.AppendTrailingSlash(_resourceRoot + string.Join("/", subDirectories, 0, index + 1));
        if (_resourcePathMappings.ContainsKey(partialPath))
          return _resourcePathMappings[partialPath];
      }

      return null;
    }

    protected virtual string GetProjectRoot ()
    {
      return HttpContext.Current.Server.MapPath("~/");
    }

    protected virtual string CombineVirtualPath (string basePath, string relativePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("basePath", basePath);
      ArgumentUtility.CheckNotNullOrEmpty("relativePath", relativePath);

      return VirtualPathUtility.Combine(basePath, relativePath);
    }

    protected virtual string MakeRelativeVirtualPath (string fromPath, string toPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("fromPath", fromPath);
      ArgumentUtility.CheckNotNullOrEmpty("toPath", toPath);

      return VirtualPathUtility.MakeRelative(fromPath, toPath);
    }

    protected virtual string ToAppRelativeVirtualPath (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      return VirtualPathUtility.ToAppRelative(virtualPath);
    }

    protected virtual string MapPath (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("virtualPath", virtualPath);

      return HttpContext.Current.Request.MapPath(virtualPath);
    }
  }
}
