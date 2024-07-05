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
  public class SqlEmptyViewScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlEmptyViewScriptElementFactory _factory;
    private EmptyViewDefinition _emptyViewDefinitionWithCustomSchema;
    private EmptyViewDefinition _emptyViewDefinitionWithDefaultSchema;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlEmptyViewScriptElementFactory();

      var property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Column1", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Column2", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());

      _emptyViewDefinitionWithCustomSchema = EmptyViewDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition("SchemaName", "EmptyView1"),
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new[] { property1 });
      _emptyViewDefinitionWithDefaultSchema = EmptyViewDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition(null, "EmptyView2"),
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new[] { property1, property2 });
    }

    [Test]
    public void GetCreateElement_CustomSchema ()
    {
      var result = _factory.GetCreateElement(_emptyViewDefinitionWithCustomSchema);

      Assert.That(result, Is.TypeOf(typeof(ScriptElementCollection)));
      var elements = ((ScriptElementCollection)result).Elements;
      Assert.That(elements.Count, Is.EqualTo(3));
      Assert.That(elements[0], Is.TypeOf(typeof(BatchDelimiterStatement)));
      Assert.That(elements[2], Is.TypeOf(typeof(BatchDelimiterStatement)));
      Assert.That(elements[1], Is.TypeOf(typeof(ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [SchemaName].[EmptyView1] ([ID], [ClassID], [Timestamp], [Column1])\r\n"
          + "  AS\r\n"
          + "  SELECT CONVERT(uniqueidentifier,NULL) AS [ID], CONVERT(varchar(100),NULL) AS [ClassID], CONVERT(datetime2,NULL) AS [Timestamp], CONVERT(varchar(100),NULL) AS [Column1]\r\n"
          + "    WHERE 1 = 0";
      Assert.That(((ScriptStatement)elements[1]).Statement, Is.EqualTo(expectedResult));
    }

     [Test]
    public void GetCreateElement_DefaultSchema ()
    {
      var result = _factory.GetCreateElement(_emptyViewDefinitionWithDefaultSchema);

      Assert.That(result, Is.TypeOf(typeof(ScriptElementCollection)));
      var elements = ((ScriptElementCollection)result).Elements;
      Assert.That(elements.Count, Is.EqualTo(3));
      Assert.That(elements[0], Is.TypeOf(typeof(BatchDelimiterStatement)));
      Assert.That(elements[2], Is.TypeOf(typeof(BatchDelimiterStatement)));
      Assert.That(elements[1], Is.TypeOf(typeof(ScriptStatement)));
      var expectedResult =
          "CREATE VIEW [dbo].[EmptyView2] ([ID], [ClassID], [Timestamp], [Column1], [Column2])\r\n"
          + "  AS\r\n"
          + "  SELECT CONVERT(uniqueidentifier,NULL) AS [ID], CONVERT(varchar(100),NULL) AS [ClassID], CONVERT(datetime2,NULL) AS [Timestamp], CONVERT(varchar(100),NULL) AS [Column1], CONVERT(varchar(100),NULL) AS [Column2]\r\n"
          + "    WHERE 1 = 0";
      Assert.That(((ScriptStatement)elements[1]).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement_CustomSchema ()
    {
      var result = _factory.GetDropElement(_emptyViewDefinitionWithCustomSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmptyView1' AND TABLE_SCHEMA = 'SchemaName')\r\n"
          + "  DROP VIEW [SchemaName].[EmptyView1]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement(_emptyViewDefinitionWithDefaultSchema);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'EmptyView2' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[EmptyView2]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }
  }
}
