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
using Remotion.Data.DomainObjects.Queries.ConfigurationLoader;
using Remotion.Data.DomainObjects.UnitTests.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.ConfigurationLoader
{
  [TestFixture]
  public class BaseDirectoryBasedRequiredQueryFileFinderTest
  {
    public class QueryFileFinderImplementation : BaseDirectoryBasedRequiredQueryFileFinder
    {
      public QueryFileFinderImplementation ([NotNull] IAppContextProvider appContextProvider, [NotNull] string queryFile)
          : base(appContextProvider, queryFile)
      {
      }
    }

    [Test]
    public void GetQueryFilePath_WithRelativeFilePathAndFileExists_ReturnsAbsolutePath ()
    {
      using var temporaryTestFile = TemporaryTestFile.Create();

      var appContextProviderMock = new Mock<IAppContextProvider>();
      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(temporaryTestFile.DirectoryName);

      var queryFileFinder = new QueryFileFinderImplementation(appContextProviderMock.Object, temporaryTestFile.FileName);
      Assert.That(queryFileFinder.GetQueryFilePaths(), Is.EqualTo(new[] { temporaryTestFile.FullPath }));
    }

    [Test]
    public void GetQueryFilePath_WithRelativeFilePathAndFileDoesNotExist_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(@"C:\nonexistent");

      var queryFileFinder = new QueryFileFinderImplementation(appContextProviderMock.Object, "query.xml");
      Assert.That(
          () => queryFileFinder.GetQueryFilePaths(),
          Throws.TypeOf<ConfigurationException>()
              .With.Message.EqualTo(@"The query file 'C:\nonexistent\query.xml' does not exist."));
    }

    [Test]
    public void GetQueryFilePath_WithAbsoluteFilePathAndFileExists_ReturnsAbsolutePath ()
    {
      using var temporaryTestFile = TemporaryTestFile.Create();

      var appContextProviderMock = new Mock<IAppContextProvider>();
      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns((@"C:\nonexistent"));

      var queryFileFinder = new QueryFileFinderImplementation(appContextProviderMock.Object, temporaryTestFile.FullPath);
      Assert.That(queryFileFinder.GetQueryFilePaths(), Is.EqualTo(new[] { temporaryTestFile.FullPath }));
    }

    [Test]
    public void GetQueryFilePath_WithAbsoluteFilePathAndFileDoesNotExists_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(@"C:\nonexistent");

      var queryFileFinder = new QueryFileFinderImplementation(appContextProviderMock.Object, @"C:\queries\query.xml");
      Assert.That(
          () => queryFileFinder.GetQueryFilePaths(),
          Throws.TypeOf<ConfigurationException>()
              .With.Message.EqualTo(@"The query file 'C:\queries\query.xml' does not exist."));
    }
  }
}
