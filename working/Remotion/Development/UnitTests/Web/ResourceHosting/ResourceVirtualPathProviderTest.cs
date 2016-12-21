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
using System.IO;
using System.Web.Hosting;
using NUnit.Framework;
using Remotion.Development.Web.ResourceHosting;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Web.ResourceHosting
{
  [TestFixture]
  public class ResourceVirtualPathProviderTest
  {
    private string _testDirectory;

    [SetUp]
    public void SetUp ()
    {
      _testDirectory = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory (_testDirectory);
    }

    [TearDown]
    public void TearDown ()
    {
      Directory.Delete (_testDirectory, true);
    }

    [Test]
    public void IsMappedPath ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/test"));
    }

    [Test]
    public void IsMappedPath_WithTrailingSlash ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/test/"));
    }

    [Test]
    public void IsMappedPath_IsCaseInsensitive ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/tEsT/"));
    }

    [Test]
    public void IsMappedPath_WithSubPath ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/test/subdirectory"));
    }

    [Test]
    public void IsMappedPath_WithSubDirectoryMapping ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test/subdirectory", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/test/subdirectory/file.txt"));
    }

    [Test]
    public void IsMappedPath_ResourceFolderRoot ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/"));
    }

    [Test]
    public void IsMappedPath_UnknownResourceDirectory ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      Assert.That (provider.IsMappedPath ("~/res/unknown/subdirectory"), Is.False);
    }

    [Test]
    public void FileExists ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testfile.txt");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");

      Directory.CreateDirectory (Path.Combine (_testDirectory, "testResourceFolder"));
      File.WriteAllText (Path.Combine (_testDirectory, "testResourceFolder\\testfile.txt"), "hello");

      Assert.That (provider.FileExists ("~/res/test/testfile.txt"));
    }

    [Test]
    public void FileExists_PhysicalFileDoesNotExist ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testfile.txt");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");

      Directory.CreateDirectory (Path.Combine (_testDirectory, "testResourceFolder"));

      Assert.That (provider.FileExists ("~/res/test/testfile.txt"), Is.False);
    }

    [Test]
    public void FileExists_NotMappedDirectory_FallsBackToPreviousProvider ()
    {
      var previousProviderStub = MockRepository.GenerateStub<VirtualPathProvider>();
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetPrevious (previousProviderStub);

      previousProviderStub.Stub (_ => _.FileExists ("~/res/UnknownDirectory/testfile.txt")).Return (true);

      Assert.That (provider.FileExists ("~/res/UnknownDirectory/testfile.txt"), Is.True);
    }

    [Test]
    public void GetFile ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testfile.txt");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");

      var expectedFilePath = Path.Combine (_testDirectory, "testResourceFolder\\testfile.txt");
      Directory.CreateDirectory (Path.Combine (_testDirectory, "testResourceFolder"));
      File.WriteAllText (expectedFilePath, "hello");

      var actual = (ResourceVirtualFile) provider.GetFile ("~/res/test/testfile.txt");

      Assert.That (actual.PhysicalPath, Is.EqualTo (expectedFilePath));
      Assert.That (actual.Exists);
    }

    [Test]
    public void GetFile_FileInsideMappedSubdirectory ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test/subdirectory", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testfile.txt");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/subdirectory");

      var expectedFilePath = Path.Combine (_testDirectory, "testResourceFolder\\testfile.txt");
      Directory.CreateDirectory (Path.Combine (_testDirectory, "testResourceFolder"));
      File.WriteAllText (expectedFilePath, "hello");

      var actual = (ResourceVirtualFile) provider.GetFile ("~/res/test/subdirectory/testfile.txt");

      Assert.That (actual.PhysicalPath, Is.EqualTo (expectedFilePath));
      Assert.That (actual.Exists);
    }

    [Test]
    public void GetFile_NotMappedPath_FallsBackToPreviousProvider ()
    {
      var previousProviderStub = MockRepository.GenerateStub<VirtualPathProvider>();
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetPrevious (previousProviderStub);

      var expectedFile = new ResourceVirtualFile ("test", new FileInfo (Path.Combine (_testDirectory, "file.txt")));
      previousProviderStub.Stub (_ => _.GetFile ("~/res/UnknownDirectory/testfile.txt")).Return (expectedFile);

      var actual = (ResourceVirtualFile) provider.GetFile ("~/res/UnknownDirectory/testfile.txt");

      Assert.That (actual, Is.SameAs (expectedFile));
    }

    [Test]
    public void GetCacheDependency_ReturnsNullForMappedPaths ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);

      var actual = provider.GetCacheDependency ("~/res/test/testfile.txt", new string[0], DateTime.UtcNow);

      Assert.That (actual, Is.Null);
    }

    [Test]
    public void DirectoryExists ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "subfolder");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");

      Directory.CreateDirectory (Path.Combine (_testDirectory, "testResourceFolder\\subfolder"));

      Assert.That (provider.DirectoryExists ("~/res/test/subfolder"));
    }

    [Test]
    public void DirectoryExists_ResourceRootDirectory ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "subfolder");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");
      provider.SetMapPathOverride ((a) => "c:\\temp");

      Directory.CreateDirectory (Path.Combine (_testDirectory, "testResourceFolder\\subfolder"));

      Assert.That (provider.DirectoryExists ("~/res/"));
    }

    [Test]
    public void DirectoryExists_PhysicalDirectoryDoesNotExist ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testDirectory");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");

      Assert.That (provider.DirectoryExists ("~/res/test/testDirectory"), Is.False);
    }

    [Test]
    public void DirectoryExists_NotMappedDirectory_FallsBackToPreviousProvider ()
    {
      var previousProviderStub = MockRepository.GenerateStub<VirtualPathProvider>();
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetPrevious (previousProviderStub);

      previousProviderStub.Stub (_ => _.DirectoryExists ("~/res/UnknownDirectory/test")).Return (true);

      Assert.That (provider.DirectoryExists ("~/res/UnknownDirectory/test"), Is.True);
    }
    
    [Test]
    public void GetDirectory ()
    {
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testDirectory");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");

      var expectedDirectoryPath = Path.Combine (_testDirectory, "testResourceFolder\\testDirectory");
      Directory.CreateDirectory (expectedDirectoryPath);

      var actual = (ResourceVirtualDirectory) provider.GetDirectory ("~/res/test/testDirectory");

      Assert.That (actual.PhysicalPath, Is.EqualTo (expectedDirectoryPath));
      Assert.That (actual.Exists);
    }

    [Test]
    public void GetDirectory_ResourceRootDirectory ()
    {
      var expectedDirectoryPath = "c:\\temp";
     
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetMakeRelativeVirtualPathOverride ((a, b) => "testDirectory");
      provider.SetCombineVirtualPathOverride ((a, b) => "~/res/test/");
      provider.SetMapPathOverride ((a) => expectedDirectoryPath);

      Directory.CreateDirectory (expectedDirectoryPath);

      var actual = (ResourceVirtualDirectory) provider.GetDirectory ("~/res/");

      Assert.That (actual.PhysicalPath, Is.EqualTo (expectedDirectoryPath));
      Assert.That (actual.Exists);
      Assert.That (actual, Is.InstanceOf<RootMappingVirtualDirectory>());
    }

    [Test]
    public void GetDirectory_NotMappedPath_FallsBackToPreviousProvider ()
    {
      var previousProviderStub = MockRepository.GenerateStub<VirtualPathProvider>();
      var provider = new TestableResourceVirtualPathProvider (new[] { new ResourcePathMapping ("test", "testResourceFolder") }, _testDirectory);
      provider.SetPrevious (previousProviderStub);

      var expectedDirectory = new ResourceVirtualDirectory ("test", new DirectoryInfo (Path.Combine (_testDirectory, "Directory.txt")));
      previousProviderStub.Stub (_ => _.GetDirectory ("~/res/UnknownDirectory/testDirectory")).Return (expectedDirectory);

      var actual = (ResourceVirtualDirectory) provider.GetDirectory ("~/res/UnknownDirectory/testDirectory");

      Assert.That (actual, Is.SameAs (expectedDirectory));
    }
  }
}