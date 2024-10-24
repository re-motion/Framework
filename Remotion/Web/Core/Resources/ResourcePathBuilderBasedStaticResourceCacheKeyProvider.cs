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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.Resources
{
  /// <summary>
  /// Defines an API for retrieving the cache key for static resources URLs generated by <see cref="ResourcePathBuilder"/>.
  /// Changes to the folder are watched and will cause the generation of a new cache key.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class ResourcePathBuilderBasedStaticResourceCacheKeyProvider : IStaticResourceCacheKeyProvider, IDisposable
  {
    public interface IMappedResourcesPathLocator
    {
      string GetMappedResourcesPath ();
    }

    public class MappedResourcesPathLocator : IMappedResourcesPathLocator
    {
      private readonly IHttpContextProvider _httpContextProvider;
      private readonly ResourceRoot _resourceRoot;

      public MappedResourcesPathLocator (IHttpContextProvider httpContextProvider, ResourceRoot resourceRoot)
      {
        ArgumentUtility.CheckNotNull(nameof(httpContextProvider), httpContextProvider);
        ArgumentUtility.CheckNotNull(nameof(resourceRoot), resourceRoot);

        _httpContextProvider = httpContextProvider;
        _resourceRoot = resourceRoot;
      }

      /// <inheritdoc />
      public string GetMappedResourcesPath ()
      {
        var context = _httpContextProvider.GetCurrentHttpContext();
        return context.Server.MapPath("~/" + _resourceRoot.Value);
      }
    }

    public interface IResourceFileDetailsAppender
    {
      void AppendResourceFolderDetails (List<(string Name, long Length, long LastWriteTime)> folderDetails, string resourceFolder, IReadOnlyCollection<ResourceType> resourceTypes);
    }

    public class ResourceFileDetailsAppender : IResourceFileDetailsAppender
    {
      /// <inheritdoc />
      public void AppendResourceFolderDetails (List<(string Name, long Length, long LastWriteTime)> folderDetails, string resourceFolder, IReadOnlyCollection<ResourceType> resourceTypes)
      {
        // Format of a resource folder is: '~/<assembly name>/<resource type>/...
        foreach (var assemblyFolder in Directory.EnumerateDirectories(resourceFolder))
        {
          foreach (var resourceType in resourceTypes)
          {
            var resourceTypeDirectory = new DirectoryInfo(Path.Combine(assemblyFolder, resourceType.Name));
            if (!resourceTypeDirectory.Exists)
              continue;

            foreach (var resourceFile in resourceTypeDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
              folderDetails.Add((resourceFile.FullName, resourceFile.Length, resourceFile.LastWriteTimeUtc.ToFileTimeUtc()));
            }
          }
        }
      }
    }

    private enum FileChangeType
    {
      Changed,
      Created,
      Deleted,
      Renamed
    }

    private readonly IMappedResourcesPathLocator _mappedResourcesPathLocator;
    private readonly IResourceFileDetailsAppender _resourceFileDetailsAppender;
    private readonly IReadOnlyList<ResourceType> _resourceTypes;

    private readonly DoubleCheckedLockingContainer<string?> _cacheKey;
    private readonly IReadOnlySet<string> _resourceTypeLookup;

    private string? _physicalPath;
    private FileSystemWatcher? _fileSystemWatcher;

    public ResourcePathBuilderBasedStaticResourceCacheKeyProvider (IHttpContextProvider httpContextProvider, ResourceRoot resourceRoot)
        : this(
            new MappedResourcesPathLocator(
                ArgumentUtility.CheckNotNull("httpContextProvider", httpContextProvider),
                ArgumentUtility.CheckNotNull("resourceRoot", resourceRoot)),
            new ResourceFileDetailsAppender(),
            ResourceType.CacheableResourceTypes)
    {
    }

    protected ResourcePathBuilderBasedStaticResourceCacheKeyProvider (
        IMappedResourcesPathLocator mappedResourcesPathLocator,
        IResourceFileDetailsAppender resourceFileDetailsAppender,
        IReadOnlyList<ResourceType> resourceTypes)
    {
      ArgumentUtility.CheckNotNull(nameof(mappedResourcesPathLocator), mappedResourcesPathLocator);
      ArgumentUtility.CheckNotNull(nameof(resourceFileDetailsAppender), resourceFileDetailsAppender);
      ArgumentUtility.CheckNotNull(nameof(resourceTypes), resourceTypes);

      _mappedResourcesPathLocator = mappedResourcesPathLocator;
      _resourceFileDetailsAppender = resourceFileDetailsAppender;
      _resourceTypes = resourceTypes;

      _cacheKey = new DoubleCheckedLockingContainer<string?>(CreateCacheKeyAndEnsureFileSystemWatcherStarted);
      _resourceTypeLookup = resourceTypes.Select(e => e.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public bool HasCacheKey => _cacheKey.HasValue;

    /// <inheritdoc />
    public string GetStaticResourceCacheKey () => _cacheKey.Value!;

    /// <inheritdoc />
    public void Dispose ()
    {
      _fileSystemWatcher?.Dispose();
      _fileSystemWatcher = null;
    }

    private string CreateCacheKeyAndEnsureFileSystemWatcherStarted ()
    {
      if (_physicalPath == null)
      {
        var physicalPath = _mappedResourcesPathLocator.GetMappedResourcesPath();
        physicalPath = Path.GetFullPath(physicalPath);
        if (!physicalPath.EndsWith("" + Path.DirectorySeparatorChar))
          physicalPath += Path.DirectorySeparatorChar;

        _fileSystemWatcher?.Dispose();
        _fileSystemWatcher = new FileSystemWatcher();
        _fileSystemWatcher.Path = physicalPath;
        _fileSystemWatcher.IncludeSubdirectories = true;
        _fileSystemWatcher.Changed += (sender, args) => CheckForPathUpdate(args.FullPath, FileChangeType.Changed);
        _fileSystemWatcher.Created += (sender, args) => CheckForPathUpdate(args.FullPath, FileChangeType.Created);
        _fileSystemWatcher.Deleted += (sender, args) => CheckForPathUpdate(args.FullPath, FileChangeType.Deleted);
        _fileSystemWatcher.Renamed += (sender, args) => CheckForPathUpdate(args.OldFullPath, FileChangeType.Renamed);

        _physicalPath = physicalPath;
        _fileSystemWatcher.EnableRaisingEvents = true;
      }

      var folderDetails = new List<(string Name, long Length, long LastWriteTime)>();
      _resourceFileDetailsAppender.AppendResourceFolderDetails(folderDetails, _physicalPath, _resourceTypes);

      var memoryStream = new MemoryStream();
      using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
      {
        foreach (var (name, length, lastWriteTime) in folderDetails)
        {
          // Use only the relative path from the root to calculate the cache key.
          // This removes a lot of stuff to cache while also allowing multiple servers
          // to generate the same cache key even if they are store on different paths.
          binaryWriter.Write(name.AsSpan()[_physicalPath.Length..]);
          binaryWriter.Write(length);
          binaryWriter.Write(lastWriteTime);
        }
      }

      memoryStream.Position = 0;

      using var sha256 = SHA256.Create();
      var hashBytes = sha256.ComputeHash(memoryStream);
      var hash = FormatAsHex(hashBytes);

      return hash;
    }

    private void CheckForPathUpdate (string? path, FileChangeType changeType)
    {
      if (path == null)
        return;

      // Perf: ignore updates when we aren't up-to-date anyway
      if (_cacheKey == null)
        return;

      var physicalPath = _physicalPath!;
      if (!path.StartsWith(physicalPath))
        return;

      // Format of a resource folder is: '~/<assembly name>/<resource type>/...
      // We need to check if the resource type is relevant to determine if the notification is relevant
      var endOfAssemblyName = path.IndexOf(Path.DirectorySeparatorChar, physicalPath.Length);
      if (endOfAssemblyName == -1)
      {
        // Updates in the root folder would require complicated checks to ensure that the
        // change requires a cache key changed. Instead, we assume any deletion or rename
        // is relevant and require a recalculation of the cache key.
        // Changed & Created are irrelevant since there would need to be additional
        // notifications for nested files/folder.
        if (changeType is FileChangeType.Deleted or FileChangeType.Renamed)
          _cacheKey.Value = null;

        return;
      }

      var endOfResourceType = path.IndexOf(Path.DirectorySeparatorChar, endOfAssemblyName + 1);
      if (endOfResourceType == -1)
      {
        // Same reasoning as above. We still only care about folders so we don't need to
        // track Changed/Created for this folder level.
        if (changeType is FileChangeType.Deleted or FileChangeType.Renamed)
          _cacheKey.Value = null;

        return;
      }

      // Check if the resource type folder is relevant
      var resourceName = path.Substring(endOfAssemblyName + 1, endOfResourceType - (endOfAssemblyName + 1));
      if (!_resourceTypeLookup.Contains(resourceName))
        return;

      // For deletions we don't know what was deleted so we need to assume it was relevant.
      // For renames it is too complex to check if it was relevant so we also assume it was.
      // If its a Changed or Created we only care about file not directory changes.
      if (changeType is FileChangeType.Deleted or FileChangeType.Renamed || File.Exists(path))
      {
        _cacheKey.Value = null;
      }
    }

    private static string FormatAsHex (byte[] data)
    {
      return Convert.ToHexString(data);
    }

    /// <inheritdoc />
    public bool IsNull => false;
  }
}
