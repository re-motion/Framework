// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.DomainObjects.PerformanceTests;

[TestFixture]
[Explicit]
public class CreateTableValuedTypesTests
{
  /// <summary>
  /// Creates the scripts for creating nad dropping TVP parameters and writes them to the console.
  /// If tests fail because TVPs could not be found execute this test and
  /// copy the console output into the DomainObjects.PerformanceTests/Database/SetupDB.sql
  /// </summary>
  [Test]
  public void CreateTableValuedTypeScript ()
  {
    var classDefinitionsByStorageProvider =
        from cd in MappingConfiguration.Current.GetTypeDefinitions()
        where cd.StorageEntityDefinition.StorageProviderDefinition is RdbmsProviderDefinition
        group cd by cd.StorageEntityDefinition.StorageProviderDefinition
        into g
        select new { StorageProviderDefinition = (RdbmsProviderDefinition)g.Key, ClassDefinitions = g.ToArray() };

    var structuredTypeDefinitionProvider = new RdbmsStructuredTypeDefinitionProvider();
    foreach (var group in classDefinitionsByStorageProvider)
    {
      var providerDefinition = group.StorageProviderDefinition;
      var scriptBuilderFactory = providerDefinition.Factory.CreateSchemaScriptBuilder(providerDefinition);
      var typeDefinitions = structuredTypeDefinitionProvider.GetTypeDefinitions(group.StorageProviderDefinition, group.ClassDefinitions);
      foreach (var typeDefinition in typeDefinitions)
      {
        scriptBuilderFactory.AddStructuredTypeDefinition(typeDefinition);
      }

      var scriptToStringConverter = new ScriptToStringConverter();

      var scriptPair = scriptToStringConverter.Convert(scriptBuilderFactory);
      var createTvpStart = scriptPair.SetUpScript.IndexOf("-- Create all structured types", StringComparison.OrdinalIgnoreCase);
      var dropTvpStart = scriptPair.TearDownScript.IndexOf("-- Drop all structured types", StringComparison.OrdinalIgnoreCase);
      var dropTvpEnd = scriptPair.TearDownScript.IndexOf("-- Drop all synonyms", StringComparison.OrdinalIgnoreCase);

      var createScript = scriptPair.SetUpScript.Substring(createTvpStart);
      var dropScript = scriptPair.TearDownScript.Substring(dropTvpStart, dropTvpEnd - dropTvpStart);

      Console.WriteLine(createScript);
      Console.WriteLine(dropScript);
    }
  }
}
