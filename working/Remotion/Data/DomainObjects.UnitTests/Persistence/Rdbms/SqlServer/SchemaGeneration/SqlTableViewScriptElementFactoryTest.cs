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
  public class SqlTableViewScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlTableViewScriptElementFactory _factory;
    private TableDefinition _tableDefinitionWithCustomSchema;
    private TableDefinition _tableDefinitionWithDefaultSchema;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new SqlTableViewScriptElementFactory();

      _tableDefinitionWithCustomSchema = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition ("SchemaName", "Table1"),
          new EntityNameDefinition ("SchemaName", "View1"),
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1"));
      _tableDefinitionWithDefaultSchema = TableDefinitionObjectMother.Create (
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition (null, "Table2"),
          new EntityNameDefinition (null, "View2"),
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1"));
    }

    [Test]
    public void GetCreateElement_CustomSchema_WithSchemaBinding ()
    {
      var result = _factory.GetCreateElement (_tableDefinitionWithCustomSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [SchemaName].[View1] ([ID], [ClassID], [Timestamp], [Column1])\r\n"
         + "  WITH SCHEMABINDING AS\r\n"
         + "  SELECT [ID], [ClassID], [Timestamp], [Column1]\r\n"
         + "    FROM [SchemaName].[Table1]\r\n"
         + "  WITH CHECK OPTION";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_DefaultSchema_WithoutSchemaBinding ()
    {
      var factory = new ExtendedSqlTableViewScriptElementFactory();

      var result = factory.GetCreateElement (_tableDefinitionWithDefaultSchema);

      Assert.That (result, Is.TypeOf (typeof (ScriptElementCollection)));
      var elements = ((ScriptElementCollection) result).Elements;
      Assert.That (elements.Count, Is.EqualTo (3));
      Assert.That (elements[0], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[2], Is.TypeOf (typeof (BatchDelimiterStatement)));
      Assert.That (elements[1], Is.TypeOf (typeof (ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [dbo].[View2] ([ID], [ClassID], [Timestamp], [Column1])\r\n"
          + "  AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Column1]\r\n"
          + "    FROM [dbo].[Table2]\r\n"
          + "  WITH CHECK OPTION";
      Assert.That (((ScriptStatement) elements[1]).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement (_tableDefinitionWithCustomSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'View1' AND TABLE_SCHEMA = 'SchemaName')\r\n"
          + "  DROP VIEW [SchemaName].[View1]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_tableDefinitionWithDefaultSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'View2' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[View2]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}