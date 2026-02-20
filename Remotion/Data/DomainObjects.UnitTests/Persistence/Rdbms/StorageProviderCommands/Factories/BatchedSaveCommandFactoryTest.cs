// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories;

public class BatchedSaveCommandFactoryTest : StandardMappingTest
{
  private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStrictMock;
  private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
  private Mock<ITableDefinitionFinder> _tableDefinitionFinderStrictMock;
  private BatchedSaveCommandFactory _factory;
  private TableDefinition _tableDefinition1;

  public override void SetUp ()
  {
    base.SetUp();

    _dbCommandBuilderFactoryStrictMock = new Mock<IDbCommandBuilderFactory>(MockBehavior.Strict);
    _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
    _tableDefinitionFinderStrictMock = new Mock<ITableDefinitionFinder>(MockBehavior.Strict);

    var sqlStorageTypeInformationProvider = new SqlStorageTypeInformationProvider(new DateTimeDefaultStorageTypeProvider());
    var infrastructureStoragePropertyDefinitionProvider =
        new InfrastructureStoragePropertyDefinitionProvider(sqlStorageTypeInformationProvider, new ReflectionBasedStorageNameProvider());
    var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();

    var tableManipulationRecordDefinitionProvider = new TableManipulationRecordDefinitionProvider(
        sqlStorageTypeInformationProvider,
        infrastructureStoragePropertyDefinitionProvider,
        rdbmsPersistenceModelProvider);

    _factory = new BatchedSaveCommandFactory(
        _dbCommandBuilderFactoryStrictMock.Object,
        _rdbmsPersistenceModelProvider,
        _tableDefinitionFinderStrictMock.Object,
        tableManipulationRecordDefinitionProvider);

    _tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table1"));
  }

  [Test]
  public void CreateForSave_New ()
  {
    var dataContainerNew1 = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(dataContainerNew1, typeof(Computer), "SerialNumber", "123456");
    var dataContainerNew2 = DataContainer.CreateNew(DomainObjectIDs.Computer2);
    SetPropertyValue(dataContainerNew2, typeof(Computer), "SerialNumber", "654321");

    var dataContainerNewWithoutRelations = DataContainer.CreateNew(DomainObjectIDs.Official1);

    var insertDbCommandBuilderNew1 = new Mock<IDbCommandBuilder>();
    var insertDbCommandBuilderNew2 = new Mock<IDbCommandBuilder>();
    var insertDbCommandBuilderNew3 = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilderNew1 = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilderNew2 = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinitionA = (TableDefinition)dataContainerNew1.ClassDefinition.StorageEntityDefinition;
    Assertion.DebugAssert((TableDefinition)dataContainerNew2.ClassDefinition.StorageEntityDefinition == tableDefinitionA);
    var tableDefinitionB = (TableDefinition)dataContainerNewWithoutRelations.ClassDefinition.StorageEntityDefinition;

    var insertStatementParameters = new Queue<DataContainer>(new[] { dataContainerNew1, dataContainerNew2 });
    var insertStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { insertDbCommandBuilderNew1.Object, insertDbCommandBuilderNew2.Object });
    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForInsert(tableDefinitionA, It.IsNotNull<IEnumerable<ColumnValue>>()))
        .Callback((TableDefinition _, IEnumerable<ColumnValue> insertedColumns) => CheckInsertedComputerColumns(insertedColumns.ToArray(), insertStatementParameters.Dequeue()))
        .Returns(() => insertStatementReturnValues.Dequeue());
    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForInsert(tableDefinitionB, It.IsAny<IEnumerable<ColumnValue>>()))
        .Returns(insertDbCommandBuilderNew3.Object);

    var updateStatementParameters = new Queue<DataContainer>(new[] { dataContainerNew1, dataContainerNew2 });
    var updateStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { updateDbCommandBuilderNew1.Object, updateDbCommandBuilderNew2.Object });
    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForUpdate(tableDefinitionA, It.IsNotNull<IEnumerable<ColumnValue>>(), It.IsNotNull<IEnumerable<ColumnValue>>()))
        .Callback((TableDefinition _, IEnumerable<ColumnValue> updatedColumns, IEnumerable<ColumnValue> comparedColumnValues) =>
        {
          var dataContainer = updateStatementParameters.Dequeue();
          CheckUpdatedComputerColumns(updatedColumns.ToArray(), dataContainer, expectEmployee: true, expectSerialNumber: false, expectClassID: false);
          CheckComparedColumns(comparedColumnValues.ToArray(), dataContainer, tableDefinitionA);
        })
        .Returns(() => updateStatementReturnValues.Dequeue());

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IBatchedLockCommandSpecification[]>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerNew1.ID, tableDefinitionA);
    StubTableDefinitionFinder(dataContainerNew2.ID, tableDefinitionA);
    StubTableDefinitionFinder(dataContainerNewWithoutRelations.ID, tableDefinitionB);

    var result = _factory.CreateForSave(
        new[]
        {
            dataContainerNew1,
            dataContainerNew2,
            dataContainerNewWithoutRelations
        });

    _tableDefinitionFinderStrictMock.Verify();
    Assert.That(insertStatementReturnValues, Is.Empty);
    Assert.That(updateStatementReturnValues, Is.Empty);
    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(5));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[0]).ObjectID, Is.EqualTo(dataContainerNew1.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(insertDbCommandBuilderNew1.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).ObjectID, Is.EqualTo(dataContainerNew2.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(insertDbCommandBuilderNew2.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[2]).ObjectID, Is.EqualTo(dataContainerNewWithoutRelations.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(insertDbCommandBuilderNew3.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[3]).ObjectID, Is.EqualTo(dataContainerNew1.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[3]).CommandBuilder, Is.SameAs(updateDbCommandBuilderNew1.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[4]).ObjectID, Is.EqualTo(dataContainerNew2.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[4]).CommandBuilder, Is.SameAs(updateDbCommandBuilderNew2.Object));
  }

  [Test]
  public void CreateForSave_Changed ()
  {
    var dataContainerChangedSerialNumber = DataContainer.CreateForExisting(DomainObjectIDs.Computer1, new byte[8], pd => pd.DefaultValue);
    SetPropertyValue(dataContainerChangedSerialNumber, typeof(Computer), "SerialNumber", "123456");
    var dataContainerChangedEmployee = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, new byte[8], pd => pd.DefaultValue);
    SetPropertyValue(dataContainerChangedEmployee, typeof(Computer), "Employee", DomainObjectIDs.Employee2);
    var dataContainerChangedMarkedAsChanged = DataContainer.CreateForExisting(DomainObjectIDs.Computer3, new byte[8], pd => pd.DefaultValue);
    dataContainerChangedMarkedAsChanged.MarkAsChanged();

    var updateDbCommandBuilderChangedSerialNumber = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilderChangedEmployee = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilderMarkedAsChanged = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinition = (TableDefinition)dataContainerChangedSerialNumber.ClassDefinition.StorageEntityDefinition;
    Assertion.DebugAssert((TableDefinition)dataContainerChangedEmployee.ClassDefinition.StorageEntityDefinition == tableDefinition);
    Assertion.DebugAssert((TableDefinition)dataContainerChangedMarkedAsChanged.ClassDefinition.StorageEntityDefinition == tableDefinition);

    var updateStatementParameters = new Queue<(DataContainer DataContainer, bool ExpectEmployee, bool ExpectSerialNumber, bool ExpectClassID)>(
        new[]
        {
            (DataContainer: dataContainerChangedSerialNumber, ExpectEmployee: false, ExpectSerialNumber: true, ExpectClassID: false),
            (DataContainer: dataContainerChangedEmployee, ExpectEmployee: true, ExpectSerialNumber: false, ExpectClassID: false),
            (DataContainer: dataContainerChangedMarkedAsChanged, ExpectEmployee: false, ExpectSerialNumber: false, ExpectClassID: true)
        });
    var updateStatementReturnValues = new Queue<IDbCommandBuilder>(
        new[] { updateDbCommandBuilderChangedSerialNumber.Object, updateDbCommandBuilderChangedEmployee.Object, updateDbCommandBuilderMarkedAsChanged.Object });
    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForUpdate(tableDefinition, It.IsNotNull<IEnumerable<ColumnValue>>(), It.IsNotNull<IEnumerable<ColumnValue>>()))
        .Callback((TableDefinition _, IEnumerable<ColumnValue> updatedColumns, IEnumerable<ColumnValue> comparedColumnValues) =>
        {
          var parameters = updateStatementParameters.Dequeue();
          CheckUpdatedComputerColumns(
              updatedColumns.ToArray(),
              parameters.DataContainer,
              expectEmployee: parameters.ExpectEmployee,
              expectSerialNumber: parameters.ExpectSerialNumber,
              expectClassID: parameters.ExpectClassID);
          CheckComparedColumns(
              comparedColumnValues.ToArray(),
              parameters.DataContainer,
              tableDefinition);
        })
        .Returns(() => updateStatementReturnValues.Dequeue());

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IBatchedLockCommandSpecification[]>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerChangedSerialNumber.ID, tableDefinition);
    StubTableDefinitionFinder(dataContainerChangedEmployee.ID, tableDefinition);
    StubTableDefinitionFinder(dataContainerChangedMarkedAsChanged.ID, tableDefinition);

    var result =
        _factory.CreateForSave(new[] { dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged });

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    _tableDefinitionFinderStrictMock.Verify();
    Assert.That(updateStatementReturnValues, Is.Empty);
    Assert.That(contexts.Count, Is.EqualTo(4));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));
    Assert.That(
        ((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers,
        Is.EqualTo([dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged]));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilderChangedSerialNumber.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).ObjectID, Is.EqualTo(dataContainerChangedSerialNumber.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilderChangedSerialNumber.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[2]).ObjectID, Is.EqualTo(dataContainerChangedEmployee.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(updateDbCommandBuilderChangedEmployee.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[3]).ObjectID, Is.EqualTo(dataContainerChangedMarkedAsChanged.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[3]).CommandBuilder, Is.SameAs(updateDbCommandBuilderMarkedAsChanged.Object));
  }

  [Test]
  public void CreateForSave_Deleted ()
  {
    var dataContainerDeletedWithoutRelations = DataContainer.CreateForExisting(DomainObjectIDs.Official3, new byte[8], pd => pd.DefaultValue);
    dataContainerDeletedWithoutRelations.Delete();
    var dataContainerDeletedWithRelations1 = DataContainer.CreateForExisting(DomainObjectIDs.Computer1, new byte[8], pd => pd.DefaultValue);
    dataContainerDeletedWithRelations1.Delete();
    var dataContainerDeletedWithRelations2 = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, new byte[8], pd => pd.DefaultValue);
    dataContainerDeletedWithRelations2.Delete();

    var tableDefinitionA = (TableDefinition)dataContainerDeletedWithoutRelations.ClassDefinition.StorageEntityDefinition;
    var tableDefinitionB = (TableDefinition)dataContainerDeletedWithRelations1.ClassDefinition.StorageEntityDefinition;

    Assertion.DebugAssert((TableDefinition)dataContainerDeletedWithRelations2.ClassDefinition.StorageEntityDefinition == tableDefinitionB);

    var updateDbCommandBuilderDeleted2 = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilderDeleted3 = new Mock<IDbCommandBuilder>();
    var deleteDbCommandBuilderDeleted1 = new Mock<IDbCommandBuilder>();
    var deleteDbCommandBuilderDeleted2 = new Mock<IDbCommandBuilder>();
    var deleteDbCommandBuilderDeleted3 = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder1 = new Mock<IDbCommandBuilder>();

    var updateStatementParameters = new Queue<DataContainer>(new[] { dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2 });
    var updateStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { updateDbCommandBuilderDeleted2.Object, updateDbCommandBuilderDeleted3.Object });
    var lockStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { lockDbCommandBuilder1.Object });
    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForUpdate(tableDefinitionB, It.IsNotNull<IEnumerable<ColumnValue>>(), It.IsNotNull<IEnumerable<ColumnValue>>()))
        .Callback((TableDefinition _, IEnumerable<ColumnValue> updatedColumns, IEnumerable<ColumnValue> comparedColumnValues) =>
        {
          var dataContainer = updateStatementParameters.Dequeue();
          CheckUpdatedComputerColumns(updatedColumns.ToArray(), dataContainer, expectEmployee: true, expectSerialNumber: false, expectClassID: false);
          CheckComparedColumns(comparedColumnValues.ToArray(), dataContainer, tableDefinitionB);
        })
        .Returns(() => updateStatementReturnValues.Dequeue());

    var deleteStatementParameters = new Queue<DataContainer>(
        new[] { dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2 });
    var deleteStatementReturnValues = new Queue<IDbCommandBuilder>(
        new[] { deleteDbCommandBuilderDeleted1.Object, deleteDbCommandBuilderDeleted2.Object, deleteDbCommandBuilderDeleted3.Object });
    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForDelete(
            It.Is<TableDefinition>(p => p == deleteStatementParameters.Peek().ClassDefinition.StorageEntityDefinition),
            It.IsNotNull<IEnumerable<ColumnValue>>()))
        .Callback((TableDefinition _, IEnumerable<ColumnValue> comparedColumnValues) =>
        {
          var dataContainer = deleteStatementParameters.Dequeue();
          CheckComparedColumns(comparedColumnValues.ToArray(), dataContainer, (TableDefinition)dataContainer.ClassDefinition.StorageEntityDefinition);
        })
        .Returns(() => deleteStatementReturnValues.Dequeue());

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IBatchedLockCommandSpecification[]>()))
        .Returns(() => lockStatementReturnValues.Dequeue());

    StubTableDefinitionFinder(dataContainerDeletedWithoutRelations.ID, tableDefinitionA);
    StubTableDefinitionFinder(dataContainerDeletedWithRelations1.ID, tableDefinitionB);
    StubTableDefinitionFinder(dataContainerDeletedWithRelations2.ID, tableDefinitionB);

    var result = _factory.CreateForSave(
        new[]
        {
            dataContainerDeletedWithoutRelations,
            dataContainerDeletedWithRelations1,
            dataContainerDeletedWithRelations2
        });

    _tableDefinitionFinderStrictMock.Verify();
    Assert.That(updateStatementReturnValues, Is.Empty);
    Assert.That(deleteStatementReturnValues, Is.Empty);
    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(6));

    Assert.That(
        ((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers,
        Is.EqualTo([dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder1.Object));

    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).ObjectID, Is.EqualTo(dataContainerDeletedWithRelations1.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilderDeleted2.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[2]).ObjectID, Is.EqualTo(dataContainerDeletedWithRelations2.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(updateDbCommandBuilderDeleted3.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[3]).ObjectID, Is.EqualTo(dataContainerDeletedWithoutRelations.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[3]).CommandBuilder, Is.SameAs(deleteDbCommandBuilderDeleted1.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[4]).ObjectID, Is.EqualTo(dataContainerDeletedWithRelations1.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[4]).CommandBuilder, Is.SameAs(deleteDbCommandBuilderDeleted2.Object));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[5]).ObjectID, Is.EqualTo(dataContainerDeletedWithRelations2.ID));
    Assert.That(((SingleObjectRdbmsProviderCommand)contexts[5]).CommandBuilder, Is.SameAs(deleteDbCommandBuilderDeleted3.Object));
  }

  [Test]
  public void CreateForSave_Unchanged ()
  {
    var dataContainerUnchanged = DataContainer.CreateForExisting(DomainObjectIDs.Order4, null, pd => pd.DefaultValue);

    StubTableDefinitionFinder(DomainObjectIDs.Order4, _tableDefinition1);

    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IBatchedLockCommandSpecification[]>()))
        .Returns(lockDbCommandBuilder.Object);

    var result = _factory.CreateForSave(new[] { dataContainerUnchanged });

    _tableDefinitionFinderStrictMock.Verify();
    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var tuples = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(tuples.Count, Is.EqualTo(0));
  }

  [Test]
  public void Lock_WaitToFail_ForOtherTransaction_WithIsolationLevel_Serializable ()
  {
    DisposeTransactionScope();
    var commandTimeout = 2;
    var waitTimeOut = (commandTimeout * 1000) / 2;

    IDomainObjectHandle<Computer> computerHandle;
    using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
    {
      var computer = Computer.NewObject();
      computer.SerialNumber = "12345";
      computerHandle = computer.GetHandle();
      ClientTransaction.Current!.Commit();
    }

    Action<CompoundRdbmsProviderCommand> commandAssertions = (c) =>
    {
      Assert.That(c.InnerCommands.Count, Is.GreaterThan(0));
      Assert.That(c.InnerCommands[0], Is.TypeOf<BatchedLockRdbmsProviderCommand>());
      var lockCommandContext = (BatchedLockRdbmsProviderCommand)c.InnerCommands[0];
      Assert.That(lockCommandContext.AffectedDataContainers.Count, Is.EqualTo(1));
      Assert.That(lockCommandContext.AffectedDataContainers[0].ID, Is.EqualTo(computerHandle.ObjectID));
    };

    var task1ObjectLoadedResetEvent = new ManualResetEventSlim();
    var task2ObjectLoadedResetEvent = new ManualResetEventSlim();
    var task1SaveResetEvent = new ManualResetEventSlim();
    var task2BeginTransactionResetEvent = new ManualResetEventSlim();

    var task2SaveFinished = false;
    var task1 = SafeContext.Task.Run(() =>
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computer = computerHandle.GetObject();
        computer.SerialNumber = "Task1";
        var dataContainer = computer.InternalDataContainer;

        task1ObjectLoadedResetEvent.Set();
        task2ObjectLoadedResetEvent.Wait();

        using (var provider = CreateTestableRdbmsProvider(IsolationLevel.Serializable, commandAssertions, commandTimeout))
        {
          provider.BeginTransaction();
          provider.Save([dataContainer]);
          task1SaveResetEvent.Set();
          task2BeginTransactionResetEvent.Wait();
          // wait a second to give Task2 the chance to execute the lock command
          Task.Delay(waitTimeOut).Wait();
          Assert.That(task2SaveFinished, Is.False, "Task1 should reach this before Task2 Save finished.");
          provider.Commit();
        }
      }
    });

    var task2 = SafeContext.Task.Run(() =>
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computer = computerHandle.GetObject();
        computer.SerialNumber = "Task2";
        var dataContainer = computer.InternalDataContainer;

        task2ObjectLoadedResetEvent.Set();
        task1ObjectLoadedResetEvent.Wait();

        task1SaveResetEvent.Wait();
        using (var provider = CreateTestableRdbmsProvider(IsolationLevel.Serializable, commandAssertions, commandTimeout))
        {
          provider.BeginTransaction();
          task2BeginTransactionResetEvent.Set();
          try
          {
            provider.Save([dataContainer]);
            Assert.Fail($"Task2 Save should fail with a {nameof(ConcurrencyViolationException)}");
          }
          catch (Exception ex)
          {
            Assert.That(ex, Is.TypeOf<ConcurrencyViolationException>());
            task2SaveFinished = true;
          }
        }
      }
    });

    var waitAllSucceeded = Task.WaitAll([task1, task2], waitTimeOut * 2);
    Assert.That(waitAllSucceeded, Is.True, "WaitAll did not finish within timeout. Therefore something in the Test went wrong.");

    using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
    {
      var computer = computerHandle.GetObject();
      computer.Delete();
      ClientTransaction.Current.Commit();
    }
  }

  [Test]
  public void Lock_DoesNotWaitToFail_ForOtherTransaction_WithIsolationLevel_ReadCommitted ()
  {
    DisposeTransactionScope();
    var commandTimeout = 2;
    var waitTimeOut = (commandTimeout * 1000) / 2;

    IDomainObjectHandle<Computer> computerHandle;
    using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
    {
      var computer = Computer.NewObject();
      computer.SerialNumber = "12345";
      computerHandle = computer.GetHandle();
      ClientTransaction.Current!.Commit();
    }

    Action<CompoundRdbmsProviderCommand> commandAssertions = (c) =>
    {
      Assert.That(c.InnerCommands.Count, Is.GreaterThan(0));
      Assert.That(c.InnerCommands[0], Is.TypeOf<BatchedLockRdbmsProviderCommand>());
      var lockCommandContext = (BatchedLockRdbmsProviderCommand)c.InnerCommands[0];
      Assert.That(lockCommandContext.AffectedDataContainers.Count, Is.EqualTo(1));
      Assert.That(lockCommandContext.AffectedDataContainers[0].ID, Is.EqualTo(computerHandle.ObjectID));
    };

    var task1ObjectLoadedResetEvent = new ManualResetEventSlim();
    var task2ObjectLoadedResetEvent = new ManualResetEventSlim();
    var task1SaveResetEvent = new ManualResetEventSlim();
    var task2SaveFailed = new ManualResetEventSlim();

    var task1 = SafeContext.Task.Run(() =>
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computer = computerHandle.GetObject();
        computer.SerialNumber = "Task1";
        var dataContainer = computer.InternalDataContainer;

        task1ObjectLoadedResetEvent.Set();
        task2ObjectLoadedResetEvent.Wait(waitTimeOut);

        using (var provider = CreateTestableRdbmsProvider(IsolationLevel.ReadCommitted, commandAssertions, commandTimeout))
        {
          provider.BeginTransaction();
          provider.Save([dataContainer]);
          task1SaveResetEvent.Set();

          var task2SaveFailedWasSet = task2SaveFailed.Wait(waitTimeOut);
          Assert.That(task2SaveFailedWasSet, Is.True);
          provider.Commit();
        }
      }
    });

    var task2 = SafeContext.Task.Run(() =>
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computer = computerHandle.GetObject();
        computer.SerialNumber = "Task2";
        var dataContainer = computer.InternalDataContainer;

        task2ObjectLoadedResetEvent.Set();
        task1ObjectLoadedResetEvent.Wait(waitTimeOut);

        task1SaveResetEvent.Wait();
        using (var provider = CreateTestableRdbmsProvider(IsolationLevel.ReadCommitted, commandAssertions, commandTimeout))
        {
          provider.BeginTransaction();
          try
          {
            provider.Save([dataContainer]);
            Assert.Fail($"Task2 Save should fail with a {nameof(ConcurrencyViolationException)}");
          }
          catch (Exception ex)
          {
            Assert.That(ex, Is.TypeOf<ConcurrencyViolationException>());
            task2SaveFailed.Set();
          }
        }
      }
    });

    var waitAllSucceeded = Task.WaitAll([task1, task2], waitTimeOut * 2);
    Assert.That(waitAllSucceeded, Is.True, "WaitAll did not finish within timeout. Therefore something in the Test went wrong.");

    using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
    {
      var computer = computerHandle.GetObject();
      computer.Delete();
      ClientTransaction.Current.Commit();
    }
  }

  private TestableRdbmsProvider CreateTestableRdbmsProvider (IsolationLevel isolationLevel, Action<CompoundRdbmsProviderCommand> commandAssertions, int commandTimeout)
  {
    var typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
    var dataContainerValidator = SafeServiceLocator.Current.GetInstance<IDataContainerValidator>();
    var domainModelConstraintProvider = SafeServiceLocator.Current.GetInstance<IDomainModelConstraintProvider>();
    var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

    var connectionStringBuilder = new SqlConnectionStringBuilder(TestDomainStorageProviderDefinition.ConnectionString);
    connectionStringBuilder.MaxPoolSize = 2;
    connectionStringBuilder.CommandTimeout = commandTimeout;

    var providerDefinition = new RdbmsProviderDefinition(
        TestDomainStorageProviderDefinition.Name,
        TestDomainStorageProviderDefinition.Factory,
        connectionStringBuilder.ConnectionString,
        connectionStringBuilder.ConnectionString,
        TestDomainStorageProviderDefinition.AssignedStorageGroups);

    var storageObjectFactory = new SqlStorageObjectFactory(storageSettings, typeConversionProvider, dataContainerValidator, domainModelConstraintProvider);
    var rdbmsPersistenceModelProvider = storageObjectFactory.CreateRdbmsPersistenceModelProvider(TestDomainStorageProviderDefinition);
    var batchedSaveCommandFactory = new BatchedSaveCommandFactory(
        storageObjectFactory.CreateDbCommandBuilderFactory(TestDomainStorageProviderDefinition),
        rdbmsPersistenceModelProvider,
        new TableDefinitionFinder(rdbmsPersistenceModelProvider),
        storageObjectFactory.CreateTableManipulationRecordDefinitionProvider(TestDomainStorageProviderDefinition));

    var commandFactoryMock = new Mock<IRdbmsProviderCommandFactory>();
    commandFactoryMock
        .Setup(stub => stub.CreateForSave(It.IsAny<IEnumerable<DataContainer>>()))
        .Returns<IEnumerable<DataContainer>>((d) =>
        {
          var command = batchedSaveCommandFactory.CreateForSave(d);
          Assert.That(command, Is.TypeOf<CompoundRdbmsProviderCommand>());
          commandAssertions((CompoundRdbmsProviderCommand)command);
          return command;
        });

    var provider = new TestableRdbmsProvider(
        providerDefinition,
        providerDefinition.ConnectionString,
        NullPersistenceExtension.Instance,
        commandFactoryMock.Object,
        () => new SqlConnection());

    provider.SetIsolationLevel(isolationLevel);
    return provider;
  }

  private void StubTableDefinitionFinder (ObjectID objectID, TableDefinition tableDefinition)
  {
    _tableDefinitionFinderStrictMock.Setup(mock => mock.GetTableDefinition(objectID)).Returns(tableDefinition).Verifiable();
  }

  private void CheckComparedColumns (IReadOnlyList<ColumnValue> columnValues, DataContainer dataContainer, TableDefinition tableDefinition)
  {
    var comparedColumnValues = columnValues;

    Assert.That(comparedColumnValues[0].Column, Is.SameAs(StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(tableDefinition.ObjectIDProperty)));
    Assert.That(comparedColumnValues[0].Value, Is.SameAs(dataContainer.ID.Value));
    if (dataContainer.ClassDefinition.GetPropertyDefinitions().All(propertyDefinition => !propertyDefinition.IsObjectID))
    {
      Assert.That(comparedColumnValues[1].Column, Is.SameAs(StoragePropertyDefinitionTestHelper.GetSingleColumn(tableDefinition.TimestampProperty)));
      Assert.That(comparedColumnValues[1].Value, Is.SameAs(dataContainer.Timestamp));
    }
  }

  private void CheckUpdatedComputerColumns (
      IReadOnlyList<ColumnValue> columnValues,
      DataContainer dataContainer,
      bool expectEmployee,
      bool expectSerialNumber,
      bool expectClassID)
  {
    CheckColumnValue("SerialNumber", expectSerialNumber, columnValues, GetPropertyValue(dataContainer, typeof(Computer), "SerialNumber"));
    CheckColumnValue("EmployeeID", expectEmployee, columnValues, GetObjectIDValue(dataContainer, typeof(Computer), "Employee"));
    CheckColumnValue("ClassID", expectClassID, columnValues, dataContainer.ID.ClassID);

    var expectedColumnCount =
        (expectEmployee ? 1 : 0)
        + (expectSerialNumber ? 1 : 0)
        + (expectClassID ? 1 : 0);
    Assert.That(columnValues.Count(), Is.EqualTo(expectedColumnCount));
  }

  private void CheckInsertedComputerColumns (IReadOnlyList<ColumnValue> columnValues, DataContainer dataContainer)
  {
    CheckColumnValue("ID", true, columnValues, dataContainer.ID.Value);
    CheckColumnValue("ClassID", true, columnValues, dataContainer.ID.ClassID);
    CheckColumnValue("SerialNumber", true, columnValues, GetPropertyValue(dataContainer, typeof(Computer), "SerialNumber"));
    CheckColumnValue("EmployeeID", false, columnValues, GetObjectIDValue(dataContainer, typeof(Computer), "Employee"));

    Assert.That(columnValues.Count(), Is.EqualTo(3));
  }

  private object GetObjectIDValue (DataContainer dataContainer, Type declaringType, string shortPropertyName)
  {
    var objectID = (ObjectID)GetPropertyValue(dataContainer, declaringType, shortPropertyName);
    return objectID != null ? objectID.Value : null;
  }

  private void CheckColumnValue (
      string columnName,
      bool shouldBeIncluded,
      IReadOnlyList<ColumnValue> columnValues,
      object expectedValue)
  {
    var column = columnValues.FirstOrDefault(cv => cv.Column.Name == columnName);
    if (column.Column == null)
    {
      Assert.That(shouldBeIncluded, Is.False, $"Column '{columnName}' was expected, but not found.");
    }
    else
    {
      Assert.That(shouldBeIncluded, Is.True, $"Column '{columnName}' was not expected, but found.");
      Assert.That(column.Value, Is.EqualTo(expectedValue));
    }
  }
}
