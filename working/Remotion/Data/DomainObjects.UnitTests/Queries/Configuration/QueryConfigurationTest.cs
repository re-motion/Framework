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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using File = System.IO.File;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class QueryConfigurationTest : StandardMappingTest
  {
    private StorageGroupBasedStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderDefinitionFinder = new StorageGroupBasedStorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
    }

    [Test]
    public void Loading ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForLoaderTest.xml", _storageProviderDefinitionFinder);
      QueryDefinitionCollection actualQueries = loader.GetQueryDefinitions ();
      QueryDefinitionCollection expectedQueries = CreateExpectedQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, actualQueries);
    }

    [Test]
    [ExpectedException (typeof (QueryConfigurationException),
        ExpectedMessage = "A scalar query 'OrderSumQuery' must not specify a collectionType.")]
    public void ScalarQueryWithCollectionType ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"ScalarQueryWithCollectionType.xml", _storageProviderDefinitionFinder);
      loader.GetQueryDefinitions ();
    }

    [Test]
    public void QueryConfigurationWithInvalidNamespace ()
    {
      string configurationFile = "QueriesWithInvalidNamespace.xml";
      try
      {
        QueryConfigurationLoader loader = new QueryConfigurationLoader (configurationFile, _storageProviderDefinitionFinder);

        Assert.Fail ("QueryConfigurationException was expected");
      }
      catch (QueryConfigurationException ex)
      {
        string expectedMessage = string.Format (
            "Error while reading query configuration: The namespace 'http://www.re-motion.org/Data/DomainObjects/InvalidNamespace' of"
            + " the root element is invalid. Expected namespace: 'http://www.re-motion.org/Data/DomainObjects/Queries/1.0'. File: '{0}'.",
            Path.GetFullPath (configurationFile));

        Assert.That (ex.Message, Is.EqualTo (expectedMessage));
      }
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

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (2));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo (@"..\..\myqueries1.xml"));
      Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries1.xml")));
      Assert.That (configuration.QueryFiles[1].FileName, Is.EqualTo (@"..\..\myqueries2.xml"));
      Assert.That (configuration.QueryFiles[1].RootedFileName, Is.EqualTo (Path.GetFullPath (@"..\..\myqueries2.xml")));
    }

    [Test]
    public void GetDefaultQueryFilePath_BaseDirectory ()
    {
      QueryConfiguration configuration = new QueryConfiguration ();

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (0));
      Assert.That (configuration.QueryDefinitions.Count, Is.GreaterThan (0));

      Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));

      QueryConfigurationLoader loader = new QueryConfigurationLoader (configuration.GetDefaultQueryFilePath (), _storageProviderDefinitionFinder);
      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (loader.GetQueryDefinitions (), configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefaultQueryFilePath_WithRelativeSearchPath ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = Path.GetPathRoot (AppDomain.CurrentDomain.BaseDirectory);
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory).Substring (setup.ApplicationBase.Length); // make a relative path

      new AppDomainRunner (setup, delegate (object[] args)
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        Assert.That (!File.Exists (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));
        Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine ((string) args[0], "queries.xml")));
      }, AppDomain.CurrentDomain.BaseDirectory).Run();
    }

    [Test]
    public void GetDefaultQueryFilePath_WithMultipleRelativeSearchPaths ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = Path.GetPathRoot (AppDomain.CurrentDomain.BaseDirectory);
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = @"A;B;C;Foo;" + Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory).Substring (setup.ApplicationBase.Length);  // make a relative path

      new AppDomainRunner (setup, delegate (object[] args)
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        Assert.That (!File.Exists (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));
        Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine ((string) args[0], "queries.xml")));
      }, AppDomain.CurrentDomain.BaseDirectory).Run ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "No default query file found. Searched for one of the following files:\nC:\\queries.xml")]
    public void GetDefaultQueryFilePath_ThrowsIfNoQueryFileExists ()
    {
      AppDomainRunner.Run (@"C:\", delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        configuration.GetDefaultQueryFilePath ();
      });
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "No default query file found. Searched for one of the following files:\nC:\\queries.xml\nC:\\Bin\\queries.xml\nC:\\Foo\\queries.xml")]
    public void GetDefaultQueryFilePath_ThrowsIfNoQueryFileExists_WithMultipleRelativeSearchPaths ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"C:\";
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = @"Bin;Foo";

      new AppDomainRunner (setup, delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        configuration.GetDefaultQueryFilePath ();
      }).Run ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"Two default query configuration files found", 
        MatchType = MessageMatch.Contains)]
    public void GetDefaultQueryFilePath_ThrowsIfMultipleQueryFilesExist ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = ".";  // simulate multiple files by searching the same directory twice

      new AppDomainRunner (setup, delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        configuration.GetDefaultQueryFilePath ();
      }, AppDomain.CurrentDomain.BaseDirectory).Run ();
    }

    [Test]
    public void GetDefaultQueryFilePath_WithEmptyRelativeSearchPath ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
      setup.DynamicBase = Path.GetTempPath ();
      setup.PrivateBinPath = "";

      new AppDomainRunner (setup, delegate
      {
        QueryConfiguration configuration = new QueryConfiguration ();
        Assert.That (configuration.GetDefaultQueryFilePath (), Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "queries.xml")));
      }, AppDomain.CurrentDomain.BaseDirectory).Run ();
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

      QueryConfiguration configuration = new QueryConfiguration ();

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      // unfortunately, this silently works because identical elements are not considered duplicates
    }

    [Test]
    public void QueryConfiguration_WithFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (1));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo ("QueriesForLoaderTest.xml"));
      Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "QueriesForLoaderTest.xml")));
    }

    [Test]
    public void QueryConfiguration_WithRootedFileName ()
    {
      QueryConfiguration configuration = new QueryConfiguration (@"c:\QueriesForLoaderTest.xml");

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (1));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo (@"c:\QueriesForLoaderTest.xml"));
    }

    [Test]
    public void QueryConfiguration_WithMultipleFileNames ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("Q1.xml", "Q2.xml");

      Assert.That (configuration.QueryFiles.Count, Is.EqualTo (2));
      Assert.That (configuration.QueryFiles[0].FileName, Is.EqualTo ("Q1.xml"));
      Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Q1.xml")));
      Assert.That (configuration.QueryFiles[1].FileName, Is.EqualTo ("Q2.xml"));
      Assert.That (configuration.QueryFiles[1].RootedFileName, Is.EqualTo (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Q2.xml")));
    }

    [Test]
    public void GetDefinitions ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");

      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForLoaderTest.xml", _storageProviderDefinitionFinder);
      QueryDefinitionCollection expectedQueries = loader.GetQueryDefinitions ();

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void GetDefinitions_WithMultipleFiles ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml", "QueriesForLoaderTest2.xml");

      QueryConfigurationLoader loader1 = new QueryConfigurationLoader (@"QueriesForLoaderTest.xml", _storageProviderDefinitionFinder);
      QueryConfigurationLoader loader2 = new QueryConfigurationLoader (@"QueriesForLoaderTest2.xml", _storageProviderDefinitionFinder);
      QueryDefinitionCollection expectedQueries = loader1.GetQueryDefinitions ();
      expectedQueries.Merge (loader2.GetQueryDefinitions());

      Assert.That (expectedQueries.Count > loader1.GetQueryDefinitions ().Count, Is.True);

      QueryDefinitionChecker checker = new QueryDefinitionChecker ();
      checker.Check (expectedQueries, configuration.QueryDefinitions);
    }

    [Test]
    public void RootedPath_UnaffectedByDirectoryChange()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");
      string pathBefore = configuration.QueryFiles[0].RootedFileName;

      string oldDirectory = AppDomain.CurrentDomain.BaseDirectory;
      try
      {
        Environment.CurrentDirectory = @"c:\";
        Assert.That (configuration.QueryFiles[0].RootedFileName, Is.EqualTo (pathBefore));
      }
      finally
      {
        Environment.CurrentDirectory = oldDirectory;
      }
    }

    [Test]
    public void GetDefinitions_UsesRootedPath ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml");
      string oldDirectory = AppDomain.CurrentDomain.BaseDirectory;
      try
      {
        Environment.CurrentDirectory = @"c:\";
        Assert.IsNotEmpty (configuration.QueryDefinitions);
      }
      finally
      {
        Environment.CurrentDirectory = oldDirectory;
      }
    }

    [Test]
    public void CollectionType_SupportsTypeUtilityNotation ()
    {
      QueryDefinitionCollection queries = new QueryConfiguration ("QueriesForStandardMapping.xml").QueryDefinitions;
      Assert.That (queries["QueryWithSpecificCollectionType"].CollectionType, Is.SameAs (typeof (SpecificOrderCollection)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"File '.*QueriesForLoaderTestDuplicate.xml' defines a duplicate "
        + @"for query definition 'OrderQueryWithCustomCollectionType'.", MatchType = MessageMatch.Regex)]
    public void DifferentQueryFiles_SpecifyingDuplicates ()
    {
      QueryConfiguration configuration = new QueryConfiguration ("QueriesForLoaderTest.xml", "QueriesForLoaderTestDuplicate.xml");

      Dev.Null = configuration.QueryDefinitions;
    }

    private QueryDefinitionCollection CreateExpectedQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection ();

      queries.Add (TestQueryFactory.CreateOrderQueryWithCustomCollectionType ());
      queries.Add (TestQueryFactory.CreateOrderQueryDefinitionWithObjectListOfOrder ());
      queries.Add (TestQueryFactory.CreateCustomerTypeQueryDefinition ());
      queries.Add (TestQueryFactory.CreateOrderSumQueryDefinition ());

      return queries;
    }

    [Test]
    public void Load_ProviderFromDefaultStorageProvider ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForStorageGroupTest.xml", _storageProviderDefinitionFinder);
      QueryDefinitionCollection queries = loader.GetQueryDefinitions ();

      Assert.That (
          queries["QueryFromDefaultStorageProvider"].StorageProviderDefinition,
          Is.SameAs(DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition));
    }

    [Test]
    public void Load_ProviderFromCustomStorageGroup ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForStorageGroupTest.xml", _storageProviderDefinitionFinder);
      QueryDefinitionCollection queries = loader.GetQueryDefinitions ();

      Assert.That (
          queries["QueryFromCustomStorageGroup"].StorageProviderDefinition,
          Is.SameAs(DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions["TestDomain"]));
      Assert.That (
         queries["QueryFromCustomStorageGroup"].StorageProviderDefinition,
         Is.Not.SameAs(DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition));
    }

    [Test]
    public void Load_ProviderFromUndefinedStorageGroup ()
    {
      QueryConfigurationLoader loader = new QueryConfigurationLoader (@"QueriesForStorageGroupTest.xml", _storageProviderDefinitionFinder);
      QueryDefinitionCollection queries = loader.GetQueryDefinitions ();

      Assert.That (
          queries["QueryFromUndefinedStorageGroup"].StorageProviderDefinition,
          Is.SameAs(DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition));
    }
  }
}
