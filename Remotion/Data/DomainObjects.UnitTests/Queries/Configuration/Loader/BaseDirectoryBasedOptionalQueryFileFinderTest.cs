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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.Configuration.Loader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration.Loader
{
  [TestFixture]
  public class DefaultQueryFileFinderTest
  {
    private string _directory;

    [SetUp]
    public void SetUp ()
    {
      _directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(_directory);
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists(_directory))
        Directory.Delete(_directory, recursive: true);
    }

    [Test]
    public void GetQueryFilePath_WithFileExists_ReturnsAbsolutePath ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(_directory);

      var queryFile = Path.Combine(_directory, "queries.xml");
      File.WriteAllText(queryFile, "test query file");

      var queryFileFinder = new DefaultQueryFileFinder(appContextProviderMock.Object);
      Assert.That(queryFileFinder.GetQueryFilePaths(), Is.EqualTo(new[] { queryFile }));
    }

    [Test]
    public void GetQueryFilePath_WithFileDoesNotExist_ReturnsEmptySequence ()
    {
      var appContextProviderMock = new Mock<IAppContextProvider>();
      appContextProviderMock.SetupGet(e => e.BaseDirectory).Returns(_directory);

      var queryFileFinder = new DefaultQueryFileFinder(appContextProviderMock.Object);
      Assert.That(queryFileFinder.GetQueryFilePaths(), Is.Empty);
    }
  }
}
