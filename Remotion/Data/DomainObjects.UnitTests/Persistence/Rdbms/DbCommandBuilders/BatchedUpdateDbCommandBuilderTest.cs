// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data.Common;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders;

[TestFixture]
public class BatchedUpdateDbCommandBuilderTest : StandardMappingTest
{
  private Mock<ISqlDialect> _sqlDialectStub;
  private Mock<DbParameter> _dbDataParameterStub;
  private Mock<DbParameterCollection> _dataParameterCollectionMock;
  private Mock<DbCommand> _dbCommandStub;
  private Mock<IDbCommandFactory> _dbCommandFactoryStub;

  public override void SetUp ()
  {
    base.SetUp();

    _sqlDialectStub = new Mock<ISqlDialect>();
    _sqlDialectStub.Setup(stub => stub.StatementDelimiter).Returns(";");
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _sqlDialectStub.Setup(stub => stub.GetParameterName(It.IsAny<string>())).Returns<string>((x) => $"@{x}");

    _dbDataParameterStub = new Mock<DbParameter>();
    _dataParameterCollectionMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    _dbCommandStub = new Mock<DbCommand>();
    _dbCommandStub.Protected().Setup<DbParameter>("CreateDbParameter").Returns(_dbDataParameterStub.Object);
    _dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(_dataParameterCollectionMock.Object);
    _dbCommandStub.SetupProperty(stub => stub.CommandText);

    _dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    _dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(_dbCommandStub.Object);
  }

  [Test]
  public void Create_WithOneTableAndDefaultSchema ()
  {
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"));

    var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
    commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2", "col2" + TableManipulationRecordDefinitionProvider.IsSetColumnPostFix]);
    commandSpecification.Setup(stub => stub.TableDefinition).Returns(tableDefinition);
    commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), "@TVP_Update_Table")).Returns(_dbDataParameterStub.Object);

    var builder = new BatchedUpdateDbCommandBuilder(_sqlDialectStub.Object, [commandSpecification.Object]);
    var result = builder.Create(_dbCommandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [Table] [T]
            INNER JOIN @TVP_Update_Table [P] ON [P].[ID] = [T].[ID];
            """));
  }

  [Test]
  public void Create_WithOneTableCustomSchema ()
  {
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table"));

    var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
    commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
    commandSpecification.Setup(stub => stub.TableDefinition).Returns(tableDefinition);
    commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), "@TVP_Update_Table")).Returns(_dbDataParameterStub.Object);

    var builder = new BatchedUpdateDbCommandBuilder(_sqlDialectStub.Object, [commandSpecification.Object]);
    var result = builder.Create(_dbCommandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = [P].[col2]
            FROM [customscheme].[Table] [T]
            INNER JOIN @TVP_Update_Table [P] ON [P].[ID] = [T].[ID];
            """));
  }

  [Test]
  public void Create_WithMultipleTablesDefaultSchema ()
  {
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table1"));
    var tableDefinition2 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table2"));
    var tableDefinition3 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table3"));

    var commandSpecifications = new List<IBatchedCommandSpecification>();
    foreach (var td in new List<TableDefinition> { tableDefinition1, tableDefinition2, tableDefinition3 })
    {
      var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
      commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2", "col2" + TableManipulationRecordDefinitionProvider.IsSetColumnPostFix]);
      commandSpecification.Setup(stub => stub.TableDefinition).Returns(td);
      commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), _sqlDialectStub.Object.GetParameterName("TVP_Update_" + td.TableName.EntityName)))
          .Returns(_dbDataParameterStub.Object);
      commandSpecifications.Add(commandSpecification.Object);
    }

    var builder = new BatchedUpdateDbCommandBuilder(_sqlDialectStub.Object, commandSpecifications.ToArray());
    var result = builder.Create(_dbCommandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [Table1] [T]
            INNER JOIN @TVP_Update_Table1 [P] ON [P].[ID] = [T].[ID];
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [Table2] [T]
            INNER JOIN @TVP_Update_Table2 [P] ON [P].[ID] = [T].[ID];
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [Table3] [T]
            INNER JOIN @TVP_Update_Table3 [P] ON [P].[ID] = [T].[ID];
            """));
  }

  [Test]
  public void Create_WithMultipleTablesCustomSchema ()
  {
    _sqlDialectStub.Setup(stub => stub.DelimitIdentifier(It.IsAny<string>())).Returns<string>((x) => $"[{x}]");
    _dataParameterCollectionMock.Setup(stub => stub.Add(_dbDataParameterStub.Object)).Returns(1);

    var tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table1"));
    var tableDefinition2 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table2"));
    var tableDefinition3 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customscheme", "Table3"));

    var commandSpecifications = new List<IBatchedCommandSpecification>();
    foreach (var td in new List<TableDefinition> { tableDefinition1, tableDefinition2, tableDefinition3 })
    {
      var commandSpecification = new Mock<IBatchedCommandSpecification>(MockBehavior.Strict);
      commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2", "col2" + TableManipulationRecordDefinitionProvider.IsSetColumnPostFix]);
      commandSpecification.Setup(stub => stub.TableDefinition).Returns(td);
      commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), _sqlDialectStub.Object.GetParameterName("TVP_Update_" + td.TableName.EntityName)))
          .Returns(_dbDataParameterStub.Object);
      commandSpecifications.Add(commandSpecification.Object);
    }

    var builder = new BatchedUpdateDbCommandBuilder(_sqlDialectStub.Object, commandSpecifications.ToArray());
    var result = builder.Create(_dbCommandFactoryStub.Object);

    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [customscheme].[Table1] [T]
            INNER JOIN @TVP_Update_Table1 [P] ON [P].[ID] = [T].[ID];
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [customscheme].[Table2] [T]
            INNER JOIN @TVP_Update_Table2 [P] ON [P].[ID] = [T].[ID];
            UPDATE [T]
            SET
            [T].[col1] = [P].[col1],
            [T].[col2] = CASE WHEN [P].[col2__IsSet] = 1 THEN [P].[col2] ELSE [T].[col2] END
            FROM [customscheme].[Table3] [T]
            INNER JOIN @TVP_Update_Table3 [P] ON [P].[ID] = [T].[ID];
            """));
  }
}
