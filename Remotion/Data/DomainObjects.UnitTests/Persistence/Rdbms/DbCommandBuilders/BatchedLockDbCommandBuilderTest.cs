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
public class BatchedLockDbCommandBuilderTest : StandardMappingTest
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
    commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
    commandSpecification.Setup(stub => stub.TableDefinition).Returns(tableDefinition);
    commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), "@TVP_Lock_Table")).Returns(_dbDataParameterStub.Object);

    var builder = new BatchedLockDbCommandBuilder(_sqlDialectStub.Object, [commandSpecification.Object]);
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            DECLARE @TransactionIsolationLevel int;
            DECLARE @IsReadCommittedSnapshotOn bit;
            SET @TransactionIsolationLevel = (SELECT [transaction_isolation_level] FROM [sys].[dm_exec_sessions] WHERE [session_id] = @@SPID);
            SET @IsReadCommittedSnapshotOn = (SELECT [is_read_committed_snapshot_on] FROM [sys].[databases] WHERE [database_id] = DB_ID());
            IF (@TransactionIsolationLevel = 2 AND @IsReadCommittedSnapshotOn = 1)
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [Table] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
            ELSE
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [Table] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
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
    commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), "@TVP_Lock_Table")).Returns(_dbDataParameterStub.Object);

    var builder = new BatchedLockDbCommandBuilder(_sqlDialectStub.Object, [commandSpecification.Object]);
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            DECLARE @TransactionIsolationLevel int;
            DECLARE @IsReadCommittedSnapshotOn bit;
            SET @TransactionIsolationLevel = (SELECT [transaction_isolation_level] FROM [sys].[dm_exec_sessions] WHERE [session_id] = @@SPID);
            SET @IsReadCommittedSnapshotOn = (SELECT [is_read_committed_snapshot_on] FROM [sys].[databases] WHERE [database_id] = DB_ID());
            IF (@TransactionIsolationLevel = 2 AND @IsReadCommittedSnapshotOn = 1)
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
            ELSE
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
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
      commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
      commandSpecification.Setup(stub => stub.TableDefinition).Returns(td);
      commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), _sqlDialectStub.Object.GetParameterName("TVP_Lock_" + td.TableName.EntityName))).Returns(_dbDataParameterStub.Object);
      commandSpecifications.Add(commandSpecification.Object);
    }

    var builder = new BatchedLockDbCommandBuilder(_sqlDialectStub.Object, commandSpecifications.ToArray());
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            DECLARE @TransactionIsolationLevel int;
            DECLARE @IsReadCommittedSnapshotOn bit;
            SET @TransactionIsolationLevel = (SELECT [transaction_isolation_level] FROM [sys].[dm_exec_sessions] WHERE [session_id] = @@SPID);
            SET @IsReadCommittedSnapshotOn = (SELECT [is_read_committed_snapshot_on] FROM [sys].[databases] WHERE [database_id] = DB_ID());
            IF (@TransactionIsolationLevel = 2 AND @IsReadCommittedSnapshotOn = 1)
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [Table1] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table1 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [Table2] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table2 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [Table3] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table3 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
            ELSE
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [Table1] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table1 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [Table2] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table2 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [Table3] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table3 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
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
      commandSpecification.Setup(stub => stub.Columns).Returns(["col1", "col2"]);
      commandSpecification.Setup(stub => stub.TableDefinition).Returns(td);
      commandSpecification.Setup(stub => stub.CreateDbParameter(It.IsAny<DbCommand>(), _sqlDialectStub.Object.GetParameterName("TVP_Lock_" + td.TableName.EntityName))).Returns(_dbDataParameterStub.Object);
      commandSpecifications.Add(commandSpecification.Object);
    }

    var builder = new BatchedLockDbCommandBuilder(_sqlDialectStub.Object, commandSpecifications.ToArray());
    var result = builder.Create(_dbCommandFactoryStub.Object);
    Assert.That(
        result.CommandText,
        Is.EqualTo(
            """
            DECLARE @TransactionIsolationLevel int;
            DECLARE @IsReadCommittedSnapshotOn bit;
            SET @TransactionIsolationLevel = (SELECT [transaction_isolation_level] FROM [sys].[dm_exec_sessions] WHERE [session_id] = @@SPID);
            SET @IsReadCommittedSnapshotOn = (SELECT [is_read_committed_snapshot_on] FROM [sys].[databases] WHERE [database_id] = DB_ID());
            IF (@TransactionIsolationLevel = 2 AND @IsReadCommittedSnapshotOn = 1)
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table1] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table1 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table2] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table2 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table3] [T] WITH(ROWLOCK, XLOCK, READPAST)
            RIGHT JOIN @TVP_Lock_Table3 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
            ELSE
            BEGIN
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table1] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table1 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table2] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table2 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL
            UNION ALL 
            SELECT [P].[col1], [P].[col2] FROM [customscheme].[Table3] [T] WITH(ROWLOCK, XLOCK)
            RIGHT JOIN @TVP_Lock_Table3 [P] ON [P].[col1] = [T].[col1] AND [P].[col2] = [T].[col2]
            WHERE [T].[ID] IS NULL;
            END
            """));
  }
}
