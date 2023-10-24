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
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.Configuration.Loader;
using Remotion.Development.UnitTesting.IO;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration.Loader
{
  [TestFixture]
  public class BaseDirectoryBasedQueryFileFinderTest
  {
    public class TestableBaseDirectoryBasedQueryFileFinder : BaseDirectoryBasedQueryFileFinder
    {
      public TestableBaseDirectoryBasedQueryFileFinder ([NotNull] IAppContextProvider appContextProvider, [NotNull] string queryFile)
          : base(appContextProvider, queryFile)
      {
      }
    }

    [Test]
    public void Initialize_DoesNotResolveThePath ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>(MockBehavior.Strict);

      Assert.That(() => new TestableBaseDirectoryBasedQueryFileFinder(appContextProviderMock.Object, "invalid"), Throws.Nothing);
    }

    [Test]
    public void GetQueryFilePath_WithRelativeFilePathAndFileExists_ReturnsAbsolutePath ()
    {
      using var temporaryTestFile = new TempFile();
      Assertion.IsTrue(Path.IsPathRooted(temporaryTestFile.FileName));

      var appContextProviderMock = new Mock<IAppContextProvider>();
      var queryFileFinder = new TestableBaseDirectoryBasedQueryFileFinder(appContextProviderMock.Object, Path.GetFileName(temporaryTestFile.FileName));

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(Path.GetDirectoryName(temporaryTestFile.FileName));

      Assert.That(queryFileFinder.GetQueryFilePaths(), Is.EqualTo(new[] { temporaryTestFile.FileName }));
    }

    [Test]
    public void GetQueryFilePath_WithRelativeFilePathAndFileDoesNotExist_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      var queryFileFinder = new TestableBaseDirectoryBasedQueryFileFinder(appContextProviderMock.Object, "query.xml");

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(@"C:\nonexistent");

      Assert.That(
          () => queryFileFinder.GetQueryFilePaths(),
          Throws.TypeOf<ConfigurationException>()
              .With.Message.EqualTo(@"The query file 'C:\nonexistent\query.xml' does not exist."));
    }

    [Test]
    public void GetQueryFilePath_WithAbsoluteFilePathAndFileExists_ReturnsAbsolutePath ()
    {
      using var temporaryTestFile =new TempFile();
      Assertion.IsTrue(Path.IsPathRooted(temporaryTestFile.FileName));

      var appContextProviderMock = new Mock<IAppContextProvider>();
      var queryFileFinder = new TestableBaseDirectoryBasedQueryFileFinder(appContextProviderMock.Object, temporaryTestFile.FileName);

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns((@"C:\nonexistent"));

      Assert.That(queryFileFinder.GetQueryFilePaths(), Is.EqualTo(new[] { temporaryTestFile.FileName }));
    }

    [Test]
    public void GetQueryFilePath_WithAbsoluteFilePathAndFileDoesNotExists_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      var queryFileFinder = new TestableBaseDirectoryBasedQueryFileFinder(appContextProviderMock.Object, @"C:\queries\query.xml");

      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(@"C:\nonexistent");

      Assert.That(
          () => queryFileFinder.GetQueryFilePaths(),
          Throws.TypeOf<ConfigurationException>()
              .With.Message.EqualTo(@"The query file 'C:\queries\query.xml' does not exist."));
    }
  }
}
