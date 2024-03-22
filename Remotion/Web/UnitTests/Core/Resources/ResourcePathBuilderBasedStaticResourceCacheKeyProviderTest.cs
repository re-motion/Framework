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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Remotion.Web.Resources;

namespace Remotion.Web.UnitTests.Core.Resources
{
  [TestFixture]
  public class ResourcePathBuilderBasedStaticResourceCacheKeyProviderTest
  {
    private class TestStaticResourceCacheKeyProvider : ResourcePathBuilderBasedStaticResourceCacheKeyProvider
    {
      private class TestMappedResourcesPathLocator : IMappedResourcesPathLocator
      {
        private readonly string _path;

        public TestMappedResourcesPathLocator (string path)
        {
          _path = path;
        }

        public string GetMappedResourcesPath () => _path;
      }

      public TestStaticResourceCacheKeyProvider (string rootPath, IReadOnlyList<ResourceType> resourceTypes)
          : base(new TestMappedResourcesPathLocator(rootPath), new ResourceFileDetailsAppender(), resourceTypes)
      {
      }

      public TestStaticResourceCacheKeyProvider (
          IMappedResourcesPathLocator mappedResourcesPathLocator,
          IResourceFileDetailsAppender fileDetailsAppender,
          IReadOnlyList<ResourceType> resourceTypes)
          : base(mappedResourcesPathLocator, fileDetailsAppender, resourceTypes)
      {
      }
    }

    private class TempFolder : IDisposable
    {
      public static TempFolder Create ()
      {
        var folderName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(folderName);

        return new TempFolder(folderName);
      }

      private readonly string _path;

      private TempFolder (string path)
      {
        var fullPath = Path.GetFullPath(path);
        if (!fullPath.EndsWith("" + Path.DirectorySeparatorChar))
          fullPath += Path.DirectorySeparatorChar;
        _path = fullPath;
      }

      public string FullName => _path;

      public void CreateFile (string file, string content = null, DateTime? created = null, DateTime? lastModified = null)
      {
        var path = Path.Combine(_path, file);
        var directory = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(directory))
          Directory.CreateDirectory(directory);

        File.WriteAllText(path, content ?? "");
        if (created.HasValue)
          File.SetCreationTimeUtc(path, created.Value);
        if (lastModified.HasValue)
          File.SetLastWriteTimeUtc(path, lastModified.Value);
      }

      public void CreateDirectory (string directory, DateTime? created = null, DateTime? lastModified = null)
      {
        var path = Path.Combine(_path, directory);
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);

        if (created.HasValue)
          Directory.SetCreationTime(path, created.Value);
        if (lastModified.HasValue)
          Directory.SetLastWriteTime(path, lastModified.Value);
      }

      /// <inheritdoc />
      public void Dispose ()
      {
        Directory.Delete(_path, true);
      }
    }

    [Test]
    public void AppendResourcesFolderDetails ()
    {
      using var folder = TempFolder.Create();
      folder.CreateFile("a.txt");
      folder.CreateFile(@"Remotion.Web\b.txt");
      folder.CreateFile(@"Remotion.Web\Html\file1.txt", "content", DateTime.FromFileTime(130000011111111111), DateTime.FromFileTime(130000022222222222));
      folder.CreateFile(@"Remotion.Web\Html\sub\file2.txt", "abc", DateTime.FromFileTime(130000033333333333), DateTime.FromFileTime(130000044444444444));
      folder.CreateFile(@"Remotion.Web\UI\c.txt");
      folder.CreateFile(@"Remotion.Web\UI\sub\d.txt");
      folder.CreateFile(@"Remotion.ObjectBinding.Web\Html\file3.txt", "defg", DateTime.FromFileTime(130000055555555555), DateTime.FromFileTime(130000066666666666));
      folder.CreateFile(@"Remotion.ObjectBinding.Web\Html\a\b\c\d\file3.txt", "määääääähhh", DateTime.FromFileTime(130000077777777777), DateTime.FromFileTime(130000088888888888));

      using var staticResourceCacheKeyProvider = new TestStaticResourceCacheKeyProvider(folder.FullName, new[] { ResourceType.Html });

      var expectedCacheKey = CreateExpectedCacheKeyForEntries(
          (@"Remotion.ObjectBinding.Web\Html\file3.txt", 4, 130000066666666666),
          (@"Remotion.ObjectBinding.Web\Html\a\b\c\d\file3.txt", 18, 130000088888888888),
          (@"Remotion.Web\Html\file1.txt", 7, 130000022222222222),
          (@"Remotion.Web\Html\sub\file2.txt", 3, 130000044444444444));

      var cacheKey = staticResourceCacheKeyProvider.GetStaticResourceCacheKey();
      Assert.That(cacheKey, Is.EqualTo(expectedCacheKey));
    }

    [Test]
    public void GetStaticResourceCacheKey ()
    {
      using var folder = TempFolder.Create();

      var resourceTypes = new[] { ResourceType.Html };

      var resourcesPhysicalPathLocatorStub = new Mock<ResourcePathBuilderBasedStaticResourceCacheKeyProvider.IMappedResourcesPathLocator>();
      resourcesPhysicalPathLocatorStub.Setup(_ => _.GetMappedResourcesPath()).Returns(folder.FullName);

      var resourcesFileAppenderDetailsMock = new Mock<ResourcePathBuilderBasedStaticResourceCacheKeyProvider.IResourceFileDetailsAppender>(MockBehavior.Strict);
      resourcesFileAppenderDetailsMock
          .Setup(_ => _.AppendResourceFolderDetails(It.IsAny<List<(string, long, long)>>(), folder.FullName, resourceTypes))
          .Callback(
              new Action<List<(string, long, long)>, string, IReadOnlyCollection<ResourceType>>(
                  (entries, _, _) => { entries.Add(($"{folder.FullName}testFile.txt", 1, 3)); }))
          .Verifiable();

      using var testStaticResourceCacheKeyProvider = new TestStaticResourceCacheKeyProvider(
          resourcesPhysicalPathLocatorStub.Object,
          resourcesFileAppenderDetailsMock.Object,
          resourceTypes);

      var cacheKey = testStaticResourceCacheKeyProvider.GetStaticResourceCacheKey();
      var expectedCacheKey = CreateExpectedCacheKeyForEntries(("testFile.txt", 1, 3));
      Assert.That(cacheKey, Is.EqualTo(expectedCacheKey));

      resourcesFileAppenderDetailsMock.Verify();
    }

    [Test]
    public void GetStaticResourceCacheKey_Twice_ReturnsCachedValue ()
    {
      using var folder = TempFolder.Create();

      var resourceTypes = new[] { ResourceType.Html };

      var resourcesPhysicalPathLocatorMock = new Mock<ResourcePathBuilderBasedStaticResourceCacheKeyProvider.IMappedResourcesPathLocator>();
      resourcesPhysicalPathLocatorMock.Setup(_ => _.GetMappedResourcesPath()).Returns(folder.FullName);

      var resourcesFileAppenderDetailsMock = new Mock<ResourcePathBuilderBasedStaticResourceCacheKeyProvider.IResourceFileDetailsAppender>(MockBehavior.Strict);
      resourcesFileAppenderDetailsMock
          .Setup(_ => _.AppendResourceFolderDetails(It.IsAny<List<(string, long, long)>>(), folder.FullName, resourceTypes))
          .Callback(
              new Action<List<(string, long, long)>, string, IReadOnlyCollection<ResourceType>>(
                  (entries, _, _) => { entries.Add(($"{folder.FullName}testFile.txt", 1, 3)); }));

      using var testStaticResourceCacheKeyProvider = new TestStaticResourceCacheKeyProvider(
          resourcesPhysicalPathLocatorMock.Object,
          resourcesFileAppenderDetailsMock.Object,
          resourceTypes);

      var expectedCacheKey = CreateExpectedCacheKeyForEntries(("testFile.txt", 1, 3));
      Assert.That(testStaticResourceCacheKeyProvider.GetStaticResourceCacheKey(), Is.EqualTo(expectedCacheKey));
      Assert.That(testStaticResourceCacheKeyProvider.GetStaticResourceCacheKey(), Is.EqualTo(expectedCacheKey));

      resourcesFileAppenderDetailsMock.Verify(_ => _.AppendResourceFolderDetails(It.IsAny<List<(string, long, long)>>(), folder.FullName, resourceTypes), Times.Once);
    }

    [Test]
    public void FileSystemWatcher_WithRelevantFileChange_UpdatesCacheKey ()
    {
      using var folder = TempFolder.Create();

      var resourceTypes = new[] { ResourceType.Html };

      var resourcesPhysicalPathLocatorMock = new Mock<ResourcePathBuilderBasedStaticResourceCacheKeyProvider.IMappedResourcesPathLocator>();
      resourcesPhysicalPathLocatorMock.Setup(_ => _.GetMappedResourcesPath()).Returns(folder.FullName);

      using var testStaticResourceCacheKeyProvider = new TestStaticResourceCacheKeyProvider(
          resourcesPhysicalPathLocatorMock.Object,
          new ResourcePathBuilderBasedStaticResourceCacheKeyProvider.ResourceFileDetailsAppender(),
          resourceTypes);

      Assert.That(testStaticResourceCacheKeyProvider.GetStaticResourceCacheKey(), Is.EqualTo(CreateExpectedCacheKeyForEntries()));

      folder.CreateFile(@"Remotion.Web\Html\test.txt", "blabla", DateTime.FromFileTime(130000011111111111), DateTime.FromFileTime(130000022222222222));

      WaitForCacheKeyChange(testStaticResourceCacheKeyProvider);

      var expectedCacheKey = CreateExpectedCacheKeyForEntries(
          (@"Remotion.Web\Html\test.txt", 6, 130000022222222222));
      Assert.That(testStaticResourceCacheKeyProvider.GetStaticResourceCacheKey(), Is.EqualTo(expectedCacheKey));
    }

    [Test]
    public void FileSystemWatcher_WithIrrelevantFileChanges_DoesNotUpdateCacheKey ()
    {
      using var folder = TempFolder.Create();

      var resourceTypes = new[] { ResourceType.Html };

      var resourcesPhysicalPathLocatorMock = new Mock<ResourcePathBuilderBasedStaticResourceCacheKeyProvider.IMappedResourcesPathLocator>();
      resourcesPhysicalPathLocatorMock.Setup(_ => _.GetMappedResourcesPath()).Returns(folder.FullName);

      using var testStaticResourceCacheKeyProvider = new TestStaticResourceCacheKeyProvider(
          resourcesPhysicalPathLocatorMock.Object,
          new ResourcePathBuilderBasedStaticResourceCacheKeyProvider.ResourceFileDetailsAppender(),
          resourceTypes);

      Assert.That(testStaticResourceCacheKeyProvider.GetStaticResourceCacheKey(), Is.EqualTo(CreateExpectedCacheKeyForEntries()));

      // Changes in the root folder should not be considered as long as they aren't deletions or renames
      folder.CreateDirectory("Remotion.Web");
      folder.CreateFile("abc.txt", "testContent", DateTime.Today, DateTime.Today);

      // Changes to the assembly folders should not be considered as long as they aren't deletions or renames
      folder.CreateDirectory(@"Remotion.Web\Html");
      folder.CreateDirectory(@"Remotion.Web\UI");
      folder.CreateFile(@"Remotion.Web\abc.txt", "testContent", DateTime.Today, DateTime.Today);

      // Changes to relevant folders only consider files not directories
      folder.CreateDirectory(@"Remotion.Web\Html\subfolder", DateTime.Today, DateTime.Today);

      // Changes to irrelevant folders should be ignored completely
      folder.CreateDirectory(@"Remotion.Web\UI\subfolder");
      folder.CreateFile(@"Remotion.Web\UI\subfolder\abc.txt", "testContent", DateTime.Today, DateTime.Today);

      // No better way than to sleep as we expect no notifications to come in and
      // they can have an arbitrary delay. 100ms should be good enough as tests
      // with 50ms also worked well.
      Thread.Sleep(100);

      Assert.That(testStaticResourceCacheKeyProvider.HasCacheKey, Is.True);
    }

    private void WaitForCacheKeyChange (TestStaticResourceCacheKeyProvider staticResourceCacheKeyProvider)
    {
      for (var i = 0; i < 10; i++)
      {
        if (!staticResourceCacheKeyProvider.HasCacheKey)
          return;

        Thread.Sleep(20);
      }

      throw new InvalidOperationException($"Expected cache key to change but it did not change within the timeout.");
    }

    private static string CreateExpectedCacheKeyForEntries (params (string Name, long Length, long LastWriteTime)[] entries)
    {
      var memoryStream = new MemoryStream();
      using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
      {
        foreach (var (name, length, lastWriteTime) in entries)
        {
#if NETFRAMEWORK
          binaryWriter.Write(name);
#else
          binaryWriter.Write(name.AsSpan());
#endif
          binaryWriter.Write(length);
          binaryWriter.Write(lastWriteTime);
        }
      }

      memoryStream.Position = 0;

      using var sha256 = SHA256.Create();
      var data = sha256.ComputeHash(memoryStream);

#if NETFRAMEWORK
      var hashString = string.Concat(data.Select(e => e.ToString("X2")));
#else
      var hashString = Convert.ToHexString(data);
#endif

      Assert.That(hashString, Does.Match("^[A-F0-9]{64}$"));
      return hashString;
    }
  }
}
