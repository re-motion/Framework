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
  public class SqlTableScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlTableScriptElementFactory _factory;
    private TableDefinition _tableDefinitionWithoutPrimaryKeyConstraint;
    private TableDefinition _tableDefinitionWithClusteredPrimaryKeyConstraint;
    private TableDefinition _tableDefinitionWithNonClusteredPrimaryKeyConstraint;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlTableScriptElementFactory();

      var column1 = new ColumnDefinition("Column1", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(false), false);
      var column2 = new ColumnDefinition("Column2", StorageTypeInformationObjectMother.CreateBitStorageTypeInformation(true), false);
      var property1 = new SimpleStoragePropertyDefinition(typeof(string), column1);
      var property2 = new SimpleStoragePropertyDefinition(typeof(bool), column2);

      var idColumn = new ColumnDefinition("ID", StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation(false), true);
      var classIDColumn = new ColumnDefinition("ClassID", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(true), false);
      var objectIDProperty = new ObjectIDStoragePropertyDefinition(
          new SimpleStoragePropertyDefinition(typeof(object), idColumn),
          new SimpleStoragePropertyDefinition(typeof(string), classIDColumn));
      var timestampColumn = new ColumnDefinition("Timestamp", StorageTypeInformationObjectMother.CreateDateTimeStorageTypeInformation(true), false);
      var timestampProperty = new SimpleStoragePropertyDefinition(typeof(object), timestampColumn);

      _tableDefinitionWithoutPrimaryKeyConstraint = TableDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition("SchemaName", "EntityName"),
          null,
          objectIDProperty,
          timestampProperty,
          property1);

      _tableDefinitionWithClusteredPrimaryKeyConstraint = TableDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition("SchemaName", "EntityName"),
          null,
          objectIDProperty,
          timestampProperty,
          new[] { property1, property2 },
          new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition("PKName", true, new[] { column1 }) });

      _tableDefinitionWithNonClusteredPrimaryKeyConstraint = TableDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition(null, "EntityName"),
          null,
          objectIDProperty,
          timestampProperty,
          new[] { property1, property2 },
          new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition("PKName", false, new[] { column1, column2 }) });
    }

    [Test]
    public void GetCreateElement_TableDefinitionWithoutPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableDefinitionWithoutPrimaryKeyConstraint);

      var expectedResult =
          "CREATE TABLE [SchemaName].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar(100) NULL,\r\n"
          + "  [Timestamp] datetime2 NULL,\r\n"
          + "  [Column1] varchar(100) NOT NULL\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableDefinitionWithClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableDefinitionWithClusteredPrimaryKeyConstraint);

      var expectedResult =
          "CREATE TABLE [SchemaName].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar(100) NULL,\r\n"
          + "  [Timestamp] datetime2 NULL,\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL,\r\n"
          + "  CONSTRAINT [PKName] PRIMARY KEY CLUSTERED ([Column1])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableDefinitionWithNonClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "CREATE TABLE [dbo].[EntityName]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar(100) NULL,\r\n"
          + "  [Timestamp] datetime2 NULL,\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL,\r\n"
          + "  CONSTRAINT [PKName] PRIMARY KEY NONCLUSTERED ([Column1], [Column2])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement ()
    {
      var result = _factory.GetDropElement(_tableDefinitionWithClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EntityName' AND TABLE_SCHEMA = 'SchemaName')\r\n"
          + "  DROP TABLE [SchemaName].[EntityName]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement(_tableDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = 'EntityName' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP TABLE [dbo].[EntityName]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }
  }
}
