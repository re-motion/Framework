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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration.Loader;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using File = System.IO.File;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration.Loader
{
  [TestFixture]
  public class QueryDefinitionFileLoaderTest : StandardMappingTest
  {
    private IStorageSettings _storageSettings;

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


      _storageSettings = StandardConfiguration.Instance.GetStorageSettings();
    }

    public override void TestFixtureTearDown ()
    {
      File.Delete(_tempQueriesFilePath);

      base.TestFixtureTearDown();
    }

    [Test]
    public void LoadQueryDefinitions ()
    {
      var loader = new QueryDefinitionFileLoader(_storageSettings);
      var actualQueries = loader.LoadQueryDefinitions(GetFullScriptPath("QueriesForLoaderTest.xml"));
      var expectedQueries = new[]
                            {
                                TestQueryFactory.CreateOrderQueryWithCustomCollectionType(),
                                TestQueryFactory.CreateOrderQueryDefinitionWithObjectListOfOrder(),
                                TestQueryFactory.CreateCustomerTypeQueryDefinition(),
                                TestQueryFactory.CreateOrderSumQueryDefinition()
                            };

      QueryDefinitionChecker checker = new QueryDefinitionChecker();
      checker.Check(expectedQueries, actualQueries);
    }

    [Test]
    public void LoadQueryDefinitions_ScalarQueryWithCollectionType_ThrowsQueryConfigurationException ()
    {
      var loader = new QueryDefinitionFileLoader(_storageSettings);
      Assert.That(
          () => loader.LoadQueryDefinitions(GetFullScriptPath("ScalarQueryWithCollectionType.xml")),
          Throws.InstanceOf<QueryConfigurationException>()
              .With.Message.EqualTo("A scalar query 'OrderSumQuery' must not specify a collectionType."));
    }

    [Test]
    public void LoadQueryDefinitions_QueryConfigurationWithInvalidNamespace_ThrowsQueryConfigurationException ()
    {
      string configurationFile = GetFullScriptPath("QueriesWithInvalidNamespace.xml");
      try
      {
        var loader = new QueryDefinitionFileLoader(_storageSettings);
        loader.LoadQueryDefinitions(configurationFile);

        Assert.Fail("QueryConfigurationException was expected");
      }
      catch (QueryConfigurationException ex)
      {
        string expectedMessage = string.Format(
            "Error while reading query configuration: The namespace 'http://www.re-motion.org/Data/DomainObjects/InvalidNamespace' of"
            + " the root element is invalid. Expected namespace: 'http://www.re-motion.org/Data/DomainObjects/Queries/1.0'. File: '{0}'.",
            Path.GetFullPath(configurationFile));

        Assert.That(ex.Message, Is.EqualTo(expectedMessage));
      }
    }

    [Test]
    public void LoadQueryDefinitions_ProviderFromDefaultStorageProvider ()
    {
      var loader = new QueryDefinitionFileLoader(_storageSettings);
      var queries = loader.LoadQueryDefinitions(GetFullScriptPath("QueriesForStorageGroupTest.xml"));

      Assert.That(
          queries.Single(e => e.ID == "QueryFromDefaultStorageProvider").StorageProviderDefinition,
          Is.SameAs(_storageSettings.DefaultStorageProviderDefinition));
    }

    [Test]
    public void LoadQueryDefinitions_ProviderFromCustomStorageGroup ()
    {
      var loader = new QueryDefinitionFileLoader(_storageSettings);
      var queries = loader.LoadQueryDefinitions(GetFullScriptPath("QueriesForStorageGroupTest.xml"));

      Assert.That(
          queries.Single(e => e.ID == "QueryFromCustomStorageGroup").StorageProviderDefinition,
          Is.SameAs(_storageSettings.GetStorageProviderDefinition("TestDomain")));
      Assert.That(
         queries.Single(e => e.ID == "QueryFromCustomStorageGroup").StorageProviderDefinition,
         Is.Not.SameAs(_storageSettings.DefaultStorageProviderDefinition));
    }

    [Test]
    public void LoadQueryDefinitions_ProviderFromUndefinedStorageGroup ()
    {
      var loader = new QueryDefinitionFileLoader(_storageSettings);
      var queries = loader.LoadQueryDefinitions(GetFullScriptPath("QueriesForStorageGroupTest.xml"));

      Assert.That(
          queries.Single(e => e.ID == "QueryFromUndefinedStorageGroup").StorageProviderDefinition,
          Is.SameAs(_storageSettings.DefaultStorageProviderDefinition));
    }

    private string GetFullScriptPath (string script)
    {
      return Path.Combine(TestContext.CurrentContext.TestDirectory, script);
    }
  }
}
