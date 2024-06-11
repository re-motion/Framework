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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration.IntegrationTests
{
  [TestFixture]
  public class ScriptGeneratorDatabaseIntegrationTest : SchemaGenerationTestBase
  {
    private ScriptGenerator _standardScriptGenerator;
    private ScriptGenerator _extendedScriptGenerator;

    public override void SetUp ()
    {
      base.SetUp();

      _standardScriptGenerator = new ScriptGenerator(
          pd => pd.Factory.CreateSchemaScriptBuilder(pd),
          new RdbmsStorageEntityDefinitionProvider(),
          new RdbmsStructuredTypeDefinitionProvider(),
          new ScriptToStringConverter());

      _extendedScriptGenerator = new ScriptGenerator(
          pd => new SqlDatabaseSelectionScriptElementBuilder(
              new CompositeScriptBuilder(
                  SchemaGenerationThirdStorageProviderDefinition,
                  new IScriptBuilder[]
                  {
                      CreateTableBuilder(),
                      CreateConstraintBuilder(),
                      CreateExtendedViewBuilder(),
                      CreateIndexBuilder(),
                      CreateSynonymBuilder()
                  }),
              SchemaGenerationThirdStorageProviderDefinition.ConnectionString),
          new RdbmsStorageExtendedEntityDefinitionProvider(),
          new RdbmsStructuredTypeDefinitionProvider(),
          new ScriptToStringConverter());
    }

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      var createDBScript = ResourceUtility.GetResourceString(GetType(), "TestData.SchemaGeneration_CreateDB.sql");

      var masterAgent = new DatabaseAgent(MasterConnectionString);
      masterAgent.ExecuteBatchString(createDBScript, false, DatabaseConfiguration.GetReplacementDictionary());
    }

    [Test]
    public void ExecuteScriptForFirstStorageProvider ()
    {
      DatabaseAgent.SetConnectionString(SchemaGenerationConnectionString1);

      var scripts = _standardScriptGenerator.GetScripts(MappingConfiguration.GetTypeDefinitions())
          .Single(s => s.StorageProviderDefinition == SchemaGenerationFirstStorageProviderDefinition);

      DatabaseAgent.ExecuteBatchString(scripts.TearDownScript + scripts.SetUpScript, false, DatabaseConfiguration.GetReplacementDictionary());
    }

    [Test]
    public void ExecuteScriptForSecondStorageProvider ()
    {
      DatabaseAgent.SetConnectionString(SchemaGenerationConnectionString2);

      var scripts = _standardScriptGenerator.GetScripts(MappingConfiguration.GetTypeDefinitions())
          .Single(s => s.StorageProviderDefinition == SchemaGenerationSecondStorageProviderDefinition);

      DatabaseAgent.ExecuteBatchString(scripts.TearDownScript + scripts.SetUpScript, false, DatabaseConfiguration.GetReplacementDictionary());
    }

    [Test]
    public void ExecuteScriptForThirdStorageProvider ()
    {
      DatabaseAgent.SetConnectionString(SchemaGenerationConnectionString3);

      var scripts = _extendedScriptGenerator.GetScripts(MappingConfiguration.GetTypeDefinitions())
          .Single(s => s.StorageProviderDefinition == SchemaGenerationThirdStorageProviderDefinition);

      DatabaseAgent.ExecuteBatchString(scripts.TearDownScript + scripts.SetUpScript, false, DatabaseConfiguration.GetReplacementDictionary());
    }
  }
}
