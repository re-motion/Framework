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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Utilities;
using File = System.IO.File;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class QueryConfigurationTest : StandardMappingTest
  {
    private StorageGroupBasedStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    private string _tempQueriesFilePath;
    private string _tempQueriesDirectory;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      var queriesFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "queries.xml");
      _tempQueriesDirectory = Path.GetTempPath();
      _tempQueriesFilePath = Path.Combine(_tempQueriesDirectory, "queries.xml");
      File.Copy(queriesFile, _tempQueriesFilePath, true);
    }

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage);
    }

    public override void TestFixtureTearDown ()
    {
      File.Delete(_tempQueriesFilePath);

      base.TestFixtureTearDown();
    }

    [Test]
    public void Deserialize_WithQueryFiles ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add filename=""..\..\myqueries1.xml""/>
                <add filename=""..\..\myqueries2.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration();

      ConfigurationHelper.DeserializeSection(configuration, xmlFragment);

      Assert.That(configuration.QueryFiles.Count, Is.EqualTo(2));
      Assert.That(configuration.QueryFiles[0].FileName, Is.EqualTo(@"..\..\myqueries1.xml"));
      Assert.That(configuration.QueryFiles[0].RootedFileName, Is.SamePath(Path.Combine(AppContext.BaseDirectory, @"..\..\myqueries1.xml")));
      Assert.That(configuration.QueryFiles[1].FileName, Is.EqualTo(@"..\..\myqueries2.xml"));
      Assert.That(configuration.QueryFiles[1].RootedFileName, Is.SamePath(Path.Combine(AppContext.BaseDirectory, @"..\..\myqueries2.xml")));
    }

    [Test]
    public void GetDefaultQueryFilePath_BaseDirectory ()
    {
      QueryConfiguration configuration = new QueryConfiguration();

      Assert.That(configuration.QueryFiles.Count, Is.EqualTo(0));

      Assert.That(configuration.GetDefaultQueryFilePath(), Is.EqualTo(Path.Combine(AppContext.BaseDirectory, "queries.xml")));
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7799: Create out-of-process test infrastructure to replace tests done with app domains")]
#endif
    public void GetDefaultQueryFilePath_WithRelativeSearchPath ()
    {
      var providerStub = new Mock<IAppContextProvider>();
      providerStub.Setup(_ => _.BaseDirectory).Returns(Path.GetPathRoot(_tempQueriesDirectory));
      providerStub
          .Setup(_ => _.RelativeSearchPath)
          .Returns(Path.GetFullPath(_tempQueriesDirectory).Substring(Path.GetPathRoot(_tempQueriesDirectory).Length)); // make a relative path

      QueryConfiguration configuration = new QueryConfiguration(providerStub.Object);
      Assert.That(configuration.GetDefaultQueryFilePath(), Is.EqualTo(Path.Combine(_tempQueriesDirectory, "queries.xml")));
    }

    [Test]
    public void GetDefaultQueryFilePath_WithMultipleRelativeSearchPaths ()
    {
      var providerStub = new Mock<IAppContextProvider>();
      providerStub.Setup(_ => _.BaseDirectory).Returns(Path.GetPathRoot(_tempQueriesDirectory));
      providerStub
          .Setup(_ => _.RelativeSearchPath)
          .Returns(@"A;B;C;Foo;" + Path.GetFullPath(_tempQueriesDirectory).Substring(Path.GetPathRoot(_tempQueriesDirectory).Length)); // make a relative path

      QueryConfiguration configuration = new QueryConfiguration(providerStub.Object);
      Assert.That(configuration.GetDefaultQueryFilePath(), Is.EqualTo(Path.Combine(_tempQueriesDirectory, "queries.xml")));
    }

    [Test]
    public void GetDefaultQueryFilePath_ThrowsIfNoQueryFileExists ()
    {
      var providerStub = new Mock<IAppContextProvider>();
      providerStub.Setup(_ => _.BaseDirectory).Returns(@"C:\");

      QueryConfiguration config = new QueryConfiguration(providerStub.Object);
      Assert.That(
          () => config.GetDefaultQueryFilePath(),
          Throws.InstanceOf<ConfigurationException>().With.Message.EqualTo(
              "No default query file found. Searched for one of the following files:\nC:\\queries.xml"));
    }

    [Test]
    public void GetDefaultQueryFilePath_ThrowsIfNoQueryFileExists_WithMultipleRelativeSearchPaths ()
    {
      var providerStub = new Mock<IAppContextProvider>();
      providerStub.Setup(_ => _.BaseDirectory).Returns(@"C:\");
      providerStub.Setup(_ => _.RelativeSearchPath).Returns(@"Bin;Foo");

      QueryConfiguration configuration = new QueryConfiguration(providerStub.Object);
      Assert.That(
          () => configuration.GetDefaultQueryFilePath(),
          Throws.InstanceOf<ConfigurationException>().With.Message.EqualTo(
              "No default query file found. Searched for one of the following files:\nC:\\queries.xml\nC:\\Bin\\queries.xml\nC:\\Foo\\queries.xml"));
    }

    [Test]
    public void GetDefaultQueryFilePath_ThrowsIfMultipleQueryFilesExist ()
    {
      var providerStub = new Mock<IAppContextProvider>();
      providerStub.Setup(_ => _.BaseDirectory).Returns(_tempQueriesDirectory);
      providerStub.Setup(_ => _.RelativeSearchPath).Returns("."); // simulate multiple files by searching the same directory twice

      QueryConfiguration configuration = new QueryConfiguration(providerStub.Object);
      Assert.That(
          () => configuration.GetDefaultQueryFilePath(),
          Throws.InstanceOf<ConfigurationException>().With.Message.Contains(@"Two default query configuration files found"));
    }

    [Test]
    public void GetDefaultQueryFilePath_WithEmptyRelativeSearchPath ()
    {
      var providerStub = new Mock<IAppContextProvider>();
      providerStub.Setup(_ => _.BaseDirectory).Returns(_tempQueriesDirectory);
      providerStub.Setup(_ => _.RelativeSearchPath).Returns("");

      QueryConfiguration configuration = new QueryConfiguration(providerStub.Object);
      Assert.That(configuration.GetDefaultQueryFilePath(), Is.EqualTo(Path.Combine(_tempQueriesDirectory, "queries.xml")));
    }

    [Test]
    public void Deserialize_WithNonUniqueNames ()
    {
      string xmlFragment =
          @"<query>
              <queryFiles>
                <add filename=""..\..\myqueries1.xml""/>
                <add filename=""..\..\myqueries1.xml""/>
              </queryFiles>
            </query>";

      QueryConfiguration configuration = new QueryConfiguration();

      ConfigurationHelper.DeserializeSection(configuration, xmlFragment);

      // unfortunately, this silently works because identical elements are not considered duplicates
    }

    [Test]
    public void QueryConfiguration_WithFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration("QueriesForLoaderTest.xml");

      Assert.That(configuration.QueryFiles.Count, Is.EqualTo(1));
      Assert.That(configuration.QueryFiles[0].FileName, Is.EqualTo("QueriesForLoaderTest.xml"));
      Assert.That(configuration.QueryFiles[0].RootedFileName, Is.EqualTo(Path.Combine(AppContext.BaseDirectory, "QueriesForLoaderTest.xml")));
    }

    [Test]
    public void QueryConfiguration_WithRootedFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration(@"c:\QueriesForLoaderTest.xml");

      Assert.That(configuration.QueryFiles.Count, Is.EqualTo(1));
      Assert.That(configuration.QueryFiles[0].FileName, Is.EqualTo(@"c:\QueriesForLoaderTest.xml"));
    }

    [Test]
    public void QueryConfiguration_WithMultipleFileNames ()
    {
      QueryConfiguration configuration = new QueryConfiguration("Q1.xml", "Q2.xml");

      Assert.That(configuration.QueryFiles.Count, Is.EqualTo(2));
      Assert.That(configuration.QueryFiles[0].FileName, Is.EqualTo("Q1.xml"));
      Assert.That(configuration.QueryFiles[0].RootedFileName, Is.EqualTo(Path.Combine(AppContext.BaseDirectory, "Q1.xml")));
      Assert.That(configuration.QueryFiles[1].FileName, Is.EqualTo("Q2.xml"));
      Assert.That(configuration.QueryFiles[1].RootedFileName, Is.EqualTo(Path.Combine(AppContext.BaseDirectory, "Q2.xml")));
    }

    [Test]
    public void RootedPath_UnaffectedByDirectoryChange ()
    {
      QueryConfiguration configuration = new QueryConfiguration("QueriesForLoaderTest.xml");
      string pathBefore = configuration.QueryFiles[0].RootedFileName;

      string oldDirectory = AppContext.BaseDirectory;
      try
      {
        Environment.CurrentDirectory = @"c:\";
        Assert.That(configuration.QueryFiles[0].RootedFileName, Is.EqualTo(pathBefore));
      }
      finally
      {
        Environment.CurrentDirectory = oldDirectory;
      }
    }
  }
}
