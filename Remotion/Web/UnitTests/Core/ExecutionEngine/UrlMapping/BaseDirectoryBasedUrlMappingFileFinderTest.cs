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
using System.IO;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;
using Remotion.Utilities;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.UrlMapping
{
  [TestFixture]
  public class BaseDirectoryBasedUrlMappingFileFinderTest
  {
    public class TestableBaseDirectoryBasedUrlMappingFileFinder : Web.ExecutionEngine.UrlMapping.BaseDirectoryBasedUrlMappingFileFinder
    {
      public TestableBaseDirectoryBasedUrlMappingFileFinder ([NotNull] IAppContextProvider appContextProvider, [NotNull] string urlMappingFile)
          : base(appContextProvider, urlMappingFile)
      {
      }
    }

    [Test]
    public void Initialize_DoesNotResolveThePath ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>(MockBehavior.Strict);

      Assert.That(() => new TestableBaseDirectoryBasedUrlMappingFileFinder(appContextProviderMock.Object, "invalid"), Throws.Nothing);
    }

    [Test]
    public void GetUrlMappingFilePath_WithRelativeFilePathAndFileExists_ReturnsAbsolutePath ()
    {
      using var temporaryTestFile = new TempFile();
      Assertion.IsTrue(Path.IsPathRooted(temporaryTestFile.FileName));

      var appContextProviderMock = new Mock<IAppContextProvider>();
      var urlMappingFileFinder = new TestableBaseDirectoryBasedUrlMappingFileFinder(appContextProviderMock.Object, Path.GetFileName(temporaryTestFile.FileName));

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(Path.GetDirectoryName(temporaryTestFile.FileName));

      Assert.That(urlMappingFileFinder.GetUrlMappingFilePaths(), Is.EqualTo(new[] { temporaryTestFile.FileName }));
    }

    [Test]
    public void GetUrlMappingFilePath_WithRelativeFilePathAndFileDoesNotExist_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      var urlMappingFileFinder = new TestableBaseDirectoryBasedUrlMappingFileFinder(appContextProviderMock.Object, "urlMapping.xml");

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(@"C:\nonexistent");

      Assert.That(
          () => urlMappingFileFinder.GetUrlMappingFilePaths(),
          Throws.TypeOf<FileNotFoundException>()
              .With.Message.EqualTo(@"The URL mapping file 'C:\nonexistent\urlMapping.xml' does not exist."));
    }

    [Test]
    public void GetUrlMappingFilePath_WithAbsoluteFilePathAndFileExists_ReturnsAbsolutePath ()
    {
      using var temporaryTestFile = new TempFile();
      Assertion.IsTrue(Path.IsPathRooted(temporaryTestFile.FileName));

      var appContextProviderMock = new Mock<IAppContextProvider>();
      var urlMappingFileFinder = new TestableBaseDirectoryBasedUrlMappingFileFinder(appContextProviderMock.Object, temporaryTestFile.FileName);

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns((@"C:\nonexistent"));

      Assert.That(urlMappingFileFinder.GetUrlMappingFilePaths(), Is.EqualTo(new[] { temporaryTestFile.FileName }));
    }

    [Test]
    public void GetUrlMappingFilePath_WithAbsoluteFilePathAndFileDoesNotExists_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      var urlMappingFileFinder = new TestableBaseDirectoryBasedUrlMappingFileFinder(appContextProviderMock.Object, @"C:\mappings\urlMapping.xml");

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(@"C:\nonexistent");

      Assert.That(
          () => urlMappingFileFinder.GetUrlMappingFilePaths(),
          Throws.TypeOf<FileNotFoundException>()
              .With.Message.EqualTo(@"The URL mapping file 'C:\mappings\urlMapping.xml' does not exist."));
    }
  }
}
