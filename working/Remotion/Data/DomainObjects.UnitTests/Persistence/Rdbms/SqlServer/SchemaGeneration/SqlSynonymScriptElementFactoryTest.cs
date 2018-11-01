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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlSynonymScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlSynonymScriptElementFactory _factory;

    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition1;
    private UnionViewDefinition _unionViewDefinition2;
    private FilterViewDefinition _filterViewDefinition1;
    private FilterViewDefinition _filterViewDefinition2;
    private EmptyViewDefinition _emptyViewDefinition1;
    private EmptyViewDefinition _emptyViewDefinition2;

    private EntityNameDefinition _synonymWithCustomSchema;
    private EntityNameDefinition _synonymWithDefaultSchema;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new SqlSynonymScriptElementFactory();

      _tableDefinition1 = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "TableName1"));
      _tableDefinition2 = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "TableName2"));

      _unionViewDefinition1 = UnionViewDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "UnionView1"));
      _unionViewDefinition2 = UnionViewDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "UnionView2"));

      _filterViewDefinition1 = FilterViewDefinitionObjectMother.Create (
         SchemaGenerationFirstStorageProviderDefinition,
         new EntityNameDefinition ("SchemaName", "FilterView1"));
      _filterViewDefinition2 = FilterViewDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView2"));

      _emptyViewDefinition1 = EmptyViewDefinitionObjectMother.Create (
         SchemaGenerationFirstStorageProviderDefinition,
         new EntityNameDefinition ("SchemaName", "EmptyView1"));
      _emptyViewDefinition2 = EmptyViewDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "EmptyView2"));

      _synonymWithCustomSchema = new EntityNameDefinition ("SynonymSchemaName", "Synonym1");
      _synonymWithDefaultSchema = new EntityNameDefinition (null, "Synonym2");
    }

    [Test]
    public void GetCreateElement_TableDefinition_And_CustomSchema ()
    {
      var result = _factory.GetCreateElement (_tableDefinition1, _synonymWithCustomSchema);

      var expectedResult = "CREATE SYNONYM [SynonymSchemaName].[Synonym1] FOR [SchemaName].[TableName1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_TableDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (_tableDefinition2, _synonymWithDefaultSchema);

      var expectedResult = "CREATE SYNONYM [dbo].[Synonym2] FOR [dbo].[TableName2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_TableDefinition_And_CustomSchema ()
    {
      var result = _factory.GetDropElement (_tableDefinition1, _synonymWithCustomSchema);

      var expectedResult = 
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'SynonymSchemaName' AND SCHEMA_NAME(schema_id) = 'Synonym1')\r\n"
       +"  DROP SYNONYM [SynonymSchemaName].[Synonym1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_TableDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_tableDefinition2, _synonymWithDefaultSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'dbo' AND SCHEMA_NAME(schema_id) = 'Synonym2')\r\n"
       + "  DROP SYNONYM [dbo].[Synonym2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_UnionViewDefinition_And_CustomSchema ()
    {
      var result = _factory.GetCreateElement (_unionViewDefinition1, _synonymWithCustomSchema);

      var expectedResult = "CREATE SYNONYM [SynonymSchemaName].[Synonym1] FOR [SchemaName].[UnionView1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
    
    [Test]
    public void GetCreateElement_UnionViewDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (_unionViewDefinition2, _synonymWithDefaultSchema);

      var expectedResult = "CREATE SYNONYM [dbo].[Synonym2] FOR [dbo].[UnionView2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_UnionViewDefinition_And_CustomSchema ()
    {
      var result = _factory.GetDropElement (_unionViewDefinition1, _synonymWithCustomSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'SynonymSchemaName' AND SCHEMA_NAME(schema_id) = 'Synonym1')\r\n"
       + "  DROP SYNONYM [SynonymSchemaName].[Synonym1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_UnionViewDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_unionViewDefinition2, _synonymWithDefaultSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'dbo' AND SCHEMA_NAME(schema_id) = 'Synonym2')\r\n"
       + "  DROP SYNONYM [dbo].[Synonym2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_FilterViewDefinition_And_CustomSchema ()
    {
      var result = _factory.GetCreateElement (_filterViewDefinition1, _synonymWithCustomSchema);

      var expectedResult = "CREATE SYNONYM [SynonymSchemaName].[Synonym1] FOR [SchemaName].[FilterView1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_FilterViewDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (_filterViewDefinition2, _synonymWithDefaultSchema);

      var expectedResult = "CREATE SYNONYM [dbo].[Synonym2] FOR [dbo].[FilterView2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_FilterViewDefinition_And_CustomSchema ()
    {
      var result = _factory.GetDropElement (_filterViewDefinition1, _synonymWithCustomSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'SynonymSchemaName' AND SCHEMA_NAME(schema_id) = 'Synonym1')\r\n"
       + "  DROP SYNONYM [SynonymSchemaName].[Synonym1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_FilterViewDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_filterViewDefinition2, _synonymWithDefaultSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'dbo' AND SCHEMA_NAME(schema_id) = 'Synonym2')\r\n"
       + "  DROP SYNONYM [dbo].[Synonym2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_EmptyViewDefinition_And_CustomSchema ()
    {
      var result = _factory.GetCreateElement (_emptyViewDefinition1, _synonymWithCustomSchema);

      var expectedResult = "CREATE SYNONYM [SynonymSchemaName].[Synonym1] FOR [SchemaName].[EmptyView1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_EmptyViewDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (_emptyViewDefinition2, _synonymWithDefaultSchema);

      var expectedResult = "CREATE SYNONYM [dbo].[Synonym2] FOR [dbo].[EmptyView2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_EmptyViewDefinition_And_CustomSchema ()
    {
      var result = _factory.GetDropElement (_emptyViewDefinition1, _synonymWithCustomSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'SynonymSchemaName' AND SCHEMA_NAME(schema_id) = 'Synonym1')\r\n"
       + "  DROP SYNONYM [SynonymSchemaName].[Synonym1]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_EmptyViewDefinition_And_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_emptyViewDefinition2, _synonymWithDefaultSchema);

      var expectedResult =
        "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = 'dbo' AND SCHEMA_NAME(schema_id) = 'Synonym2')\r\n"
       + "  DROP SYNONYM [dbo].[Synonym2]";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

  }
}