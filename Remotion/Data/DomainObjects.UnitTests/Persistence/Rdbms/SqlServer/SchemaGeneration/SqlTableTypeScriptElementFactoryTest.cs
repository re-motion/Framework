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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlTableTypeScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlTableTypeScriptElementFactory _factory;
    private SqlTableTypeDefinition _tableTypeDefinitionPlain;
    private SqlTableTypeDefinition _tableTypeDefinitionWithClusteredPrimaryKeyConstraint;
    private SqlTableTypeDefinition _tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint;
    private SqlTableTypeDefinition _tableTypeDefinitionWithClusteredUniqueConstraint;
    private SqlTableTypeDefinition _tableTypeDefinitionWithNonClusteredUniqueConstraint;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlTableTypeScriptElementFactory();

      var column1 = new ColumnDefinition("Column1", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(false), false);
      var column2 = new ColumnDefinition("Column2", StorageTypeInformationObjectMother.CreateBitStorageTypeInformation(true), false);
      var property1 = new SimpleStoragePropertyDefinition(typeof(string), column1);
      var property2 = new SimpleStoragePropertyDefinition(typeof(bool), column2);

      _tableTypeDefinitionPlain = TableTypeDefinitionObjectMother.Create(
          typeName: "TypeName",
          schemaName: "SchemaName",
          propertyDefinitions: new[] { property1, property2 }
      );

      _tableTypeDefinitionWithClusteredPrimaryKeyConstraint = TableTypeDefinitionObjectMother.Create(
          typeName: "TypeName",
          propertyDefinitions: new[] { property1, property2 },

          constraints: new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition("PKName", true, new[] { column1 }) });

      _tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint = TableTypeDefinitionObjectMother.Create(
          typeName: "TypeName",
          propertyDefinitions: new[] { property1, property2 },

          constraints:  new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition("PKName", false, new[] { column1, column2 }) });

      _tableTypeDefinitionWithClusteredUniqueConstraint = TableTypeDefinitionObjectMother.Create(
          typeName: "TypeName",
          propertyDefinitions: new[] { property1, property2 },

          constraints: new ITableConstraintDefinition[] { new UniqueConstraintDefinition("UQName", true, new[] { column1 }) });

      _tableTypeDefinitionWithNonClusteredUniqueConstraint = TableTypeDefinitionObjectMother.Create(
          typeName: "TypeName",
          propertyDefinitions: new[] { property1, property2 },

          constraints: new ITableConstraintDefinition[] { new UniqueConstraintDefinition("UQName", false, new[] { column1, column2 }) });
    }

    [Test]
    public void GetCreateElement_TableTypeDefinition_Plain ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionPlain);

      var expectedResult =
          "IF TYPE_ID('[SchemaName].[TypeName]') IS NULL CREATE TYPE [SchemaName].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableTypeDefinition_WithClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF TYPE_ID('[dbo].[TypeName]') IS NULL CREATE TYPE [dbo].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL\r\n"
          + "  PRIMARY KEY CLUSTERED ([Column1])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableTypeDefinition_WithNonClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF TYPE_ID('[dbo].[TypeName]') IS NULL CREATE TYPE [dbo].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL\r\n"
          + "  PRIMARY KEY NONCLUSTERED ([Column1], [Column2])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableTypeDefinition_WithClusteredUniqueIndex ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithClusteredUniqueConstraint);

      var expectedResult =
          "IF TYPE_ID('[dbo].[TypeName]') IS NULL CREATE TYPE [dbo].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL\r\n"
          + "  UNIQUE CLUSTERED ([Column1])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableTypeDefinition_WithNonClusteredUniqueIndex ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithNonClusteredUniqueConstraint);

      var expectedResult =
          "IF TYPE_ID('[dbo].[TypeName]') IS NULL CREATE TYPE [dbo].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL\r\n"
          + "  UNIQUE NONCLUSTERED ([Column1], [Column2])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement ()
    {
      var result = _factory.GetDropElement(_tableTypeDefinitionPlain);

      var expectedResult =
          "DROP TYPE IF EXISTS [SchemaName].[TypeName]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement(_tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "DROP TYPE IF EXISTS [dbo].[TypeName]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }
  }
}
