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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.DomainObjects.UnitTests.Database
{
  [TestFixture]
  public class TestDomainDBScriptGenerationTest : StandardMappingTest
  {
    private const string c_scriptPrefix = @"--
-- This file is auto-generated. Do not edit manually.
-- See " + nameof(TestDomainDBScriptGenerationTest) + @" for the relevant tests.
--

";

    /// <summary>
    /// Compares the SQL DDL generated from the discovered type definitions to the SQL scripts stored in the Database folder.
    /// Use the explicit test below to override the stored scripts with the generated ones.
    /// </summary>
    [Test]
    public void GeneratedScriptsMatchStoredScripts ()
    {
      foreach (var script in GenerateScripts())
      {
        var storageProviderName = script.StorageProviderDefinition.Name;
        Assert.That(script.SetUpScript, Is.EqualTo(GetStoredScript($"DataDomainObjects_{storageProviderName}_SetupDB.sql")));
        Assert.That(script.TearDownScript, Is.EqualTo(GetStoredScript($"DataDomainObjects_{storageProviderName}_TearDownDB.sql")));
      }
    }

    [Explicit("Run this test to replace the stored scripts with the generated scripts (making the test above green)")]
    [Test]
    public void SaveGeneratedScriptsAsNewStoredScripts ()
    {
      foreach (var script in GenerateScripts())
      {
        var storageProviderName = script.StorageProviderDefinition.Name;
        SaveStoredScript($"DataDomainObjects_{storageProviderName}_SetupDB.sql", script.SetUpScript);
        SaveStoredScript($"DataDomainObjects_{storageProviderName}_TearDownDB.sql", script.TearDownScript);
      }
    }

    private IEnumerable<Script> GenerateScripts ()
    {
      var scriptGenerator = new ScriptGenerator(
          pd => pd.Factory.CreateSchemaScriptBuilder(pd),
          new RdbmsStorageEntityDefinitionProvider(),
          new ScriptToStringConverter());

      var typeDefinitions = MappingConfiguration.Current.GetTypeDefinitions()
          .Where(e => e.StorageEntityDefinition.StorageProviderID is c_testDomainProviderID or TableInheritanceMappingTest.TableInheritanceTestDomainProviderID)
          .Where(e => !Attribute.IsDefined(e.Type, typeof(ExcludeFromTestDomainDBAttribute)));

      return scriptGenerator.GetScripts(typeDefinitions)
          .Select(e => new Script(e.StorageProviderDefinition, PatchGeneratedScript(e.SetUpScript), PatchGeneratedScript(e.TearDownScript)));
    }

    private void SaveStoredScript (string name, string newContent)
    {
      var filePath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../Database", name));
      Assert.That(filePath, Does.Exist, "Could not find stored SQL script.");

      File.WriteAllText(filePath, newContent);
    }

    private string PatchGeneratedScript (string script)
    {
      return c_scriptPrefix + script.Replace($"USE {DatabaseConfiguration.DatabaseNamePrefix}TestDomain", "USE DBPrefix_TestDomain");
    }

    private string GetStoredScript (string name)
    {
      var filePath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $"Database/{name}"));
      Assert.That(filePath, Does.Exist, "Could not find stored SQL script '{0}'.");

      return File.ReadAllText(filePath);
    }
  }
}
