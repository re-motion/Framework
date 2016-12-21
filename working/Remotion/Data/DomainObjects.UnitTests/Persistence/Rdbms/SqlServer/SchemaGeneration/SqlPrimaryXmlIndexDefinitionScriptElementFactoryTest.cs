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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlPrimaryXmlIndexDefinitionScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlPrimaryXmlIndexDefinitionScriptElementFactory _factory;
    private SqlPrimaryXmlIndexDefinition _indexDefinitionWithCustomSchema;
    private SqlPrimaryXmlIndexDefinition _indexDefinitionWithDefaultSchema;
    private ColumnDefinition _xmlColumn;
    private EntityNameDefinition _customSchemaNameDefinition;
    private EntityNameDefinition _defaultSchemaNameDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new SqlPrimaryXmlIndexDefinitionScriptElementFactory ();

      _xmlColumn = ColumnDefinitionObjectMother.CreateColumn ("XmlColumn");

      _customSchemaNameDefinition = new EntityNameDefinition ("SchemaName", "TableName1");
      _indexDefinitionWithCustomSchema = new SqlPrimaryXmlIndexDefinition("Index1", _xmlColumn);
      _defaultSchemaNameDefinition = new EntityNameDefinition (null, "TableName2");
      _indexDefinitionWithDefaultSchema = new SqlPrimaryXmlIndexDefinition ("Index2", _xmlColumn);
    }

    [Test]
    public void GetCreateElement_CustomSchema ()
    {
      var result = _factory.GetCreateElement (_indexDefinitionWithCustomSchema, _customSchemaNameDefinition);

      var expectedResult =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          + "  ON [SchemaName].[TableName1] ([XmlColumn])";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_DefaultSchema ()
    {
      var result = _factory.GetCreateElement (_indexDefinitionWithDefaultSchema, _defaultSchemaNameDefinition);

      var expectedResult =
          "CREATE PRIMARY XML INDEX [Index2]\r\n"
          + "  ON [dbo].[TableName2] ([XmlColumn])";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_WithAllOptionsOn ()
    {
      var entityNameDefinition = new EntityNameDefinition (null, "TableName");
      var indexDefinition = new SqlPrimaryXmlIndexDefinition (
          "Index1", _xmlColumn, true, 12, true, true, true, true, true, 2);

      var result = _factory.GetCreateElement (indexDefinition, entityNameDefinition);

      var expectedResult =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          +"  ON [dbo].[TableName] ([XmlColumn])\r\n"
          +"  WITH (PAD_INDEX = ON, FILLFACTOR = 12, SORT_IN_TEMPDB = ON, STATISTICS_NORECOMPUTE = ON, DROP_EXISTING = ON, ALLOW_ROW_LOCKS = ON, "
          +"ALLOW_PAGE_LOCKS = ON, MAXDOP = 2)";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetCreateElement_WithAllOptionsOff ()
    {
      var entityNameDefinition = new EntityNameDefinition (null, "TableName");
      var indexDefinition = new SqlPrimaryXmlIndexDefinition (
          "Index1", _xmlColumn, false, 0, false, false, false, false, false, 0);

      var result = _factory.GetCreateElement (indexDefinition, entityNameDefinition);

      var expectedResult =
          "CREATE PRIMARY XML INDEX [Index1]\r\n"
          + "  ON [dbo].[TableName] ([XmlColumn])\r\n"
          + "  WITH (PAD_INDEX = OFF, FILLFACTOR = 0, SORT_IN_TEMPDB = OFF, STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS = OFF, "
          + "ALLOW_PAGE_LOCKS = OFF, MAXDOP = 0)";
      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement (_indexDefinitionWithCustomSchema, _customSchemaNameDefinition);

      var expectedResult =
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName1' AND "
          + "schema_name (so.schema_id)='SchemaName' AND si.[name] = 'Index1')\r\n"
          + "  DROP INDEX [Index1] ON [SchemaName].[TableName1]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement (_indexDefinitionWithDefaultSchema, _defaultSchemaNameDefinition);

      var expectedResult =
          "IF EXISTS (SELECT * FROM sys.objects so JOIN sysindexes si ON so.[object_id] = si.[id] WHERE so.[name] = 'TableName2' AND "
          + "schema_name (so.schema_id)='dbo' AND si.[name] = 'Index2')\r\n"
          + "  DROP INDEX [Index2] ON [dbo].[TableName2]";

      Assert.That (result, Is.TypeOf (typeof (ScriptStatement)));
      Assert.That (((ScriptStatement) result).Statement, Is.EqualTo (expectedResult));
    }
  }
}