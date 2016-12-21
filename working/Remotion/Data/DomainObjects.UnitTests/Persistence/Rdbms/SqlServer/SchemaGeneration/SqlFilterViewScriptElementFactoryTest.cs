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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlFilterViewScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlFilterViewScriptElementFactory _factory;
    private FilterViewDefinition _filterViewDefinitionWithCustomSchema;
    private FilterViewDefinition _filterViewDefinitionWithDefaultSchema;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new SqlFilterViewScriptElementFactory();

      var tableDefinitionWithCustomSchema = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "TableName1"));
      var tableDefinitionWithDefaultSchema = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "TableName2"));

      var property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");

      _filterViewDefinitionWithCustomSchema = FilterViewDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "FilterView1"),
          tableDefinitionWithCustomSchema,
          new[] { "ClassID1" },
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new[] { property1 });
      _filterViewDefinitionWithDefaultSchema = FilterViewDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "FilterView2"),
          tableDefinitionWithDefaultSchema,
          new[] { "ClassID1", "ClassID2" },
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new[] { property1, property2 });
    }

    [Test]
    public void GetCreateElement_CustomSchema_WithSchemaBinding ()
    {
      var result = _factory.GetCreateElement (_filterViewDefinitionWithCustomSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo(3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof(ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [SchemaName].[FilterView1] ([ID], [ClassID], [Timestamp], [Column1])\r\n"
         +"  WITH SCHEMABINDING AS\r\n"
         + "  SELECT [ID], [ClassID], [Timestamp], [Column1]\r\n"
         +"    FROM [SchemaName].[TableName1]\r\n"
         +"    WHERE [ClassID] IN ('ClassID1')\r\n"
         +"  WITH CHECK OPTION";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_DefaultSchema_WithoutSchemaBinding ()
    {
      var factory = new ExtendedSqlFilterViewScriptElementFactory();

      var result = factory.GetCreateElement (_filterViewDefinitionWithDefaultSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [dbo].[FilterView2] ([ID], [ClassID], [Timestamp], [Column1], [Column2])\r\n"
          +"  AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Column1], [Column2]\r\n"
          +"    FROM [dbo].[TableName2]\r\n"
          +"    WHERE [ClassID] IN ('ClassID1', 'ClassID2')\r\n"
          +"  WITH CHECK OPTION";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement (_filterViewDefinitionWithCustomSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FilterView1' AND TABLE_SCHEMA = 'SchemaName')\r\n"
          + "  DROP VIEW [SchemaName].[FilterView1]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_filterViewDefinitionWithDefaultSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'FilterView2' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[FilterView2]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}