// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests;
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

    var dataContainerNewWithoutRelations = DataContainer.CreateNew(DomainObjectIDs.Official3);
    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinitionA = (TableDefinition)dataContainerNew1.ClassDefinition.StorageEntityDefinition;
    Assertion.DebugAssert((TableDefinition)dataContainerNew2.ClassDefinition.StorageEntityDefinition == tableDefinitionA);
    var tableDefinitionB = (TableDefinition)dataContainerNewWithoutRelations.ClassDefinition.StorageEntityDefinition;

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(insertDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(updateDbCommandBuilder.Object);

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
    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerNew1, dataContainerNew2, dataContainerNewWithoutRelations]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerNew1,dataContainerNew2]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));
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

    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinition = (TableDefinition)dataContainerChangedSerialNumber.ClassDefinition.StorageEntityDefinition;
    Assertion.DebugAssert((TableDefinition)dataContainerChangedEmployee.ClassDefinition.StorageEntityDefinition == tableDefinition);
    Assertion.DebugAssert((TableDefinition)dataContainerChangedMarkedAsChanged.ClassDefinition.StorageEntityDefinition == tableDefinition);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(updateDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerChangedSerialNumber.ID, tableDefinition);
    StubTableDefinitionFinder(dataContainerChangedEmployee.ID, tableDefinition);
    StubTableDefinitionFinder(dataContainerChangedMarkedAsChanged.ID, tableDefinition);

    var result =
        _factory.CreateForSave(new[] { dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged });

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    _tableDefinitionFinderStrictMock.Verify();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged]));
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

    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
       .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
       .Returns(updateDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

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
    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(3));

    Assert.That(
        ((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers,
        Is.EqualTo([dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).AffectedDataContainers, Is.EqualTo([dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));
  }

  [Test]
  public void CreateForSave_LockInsertUpdateDelete_CommandsAreInCorrectOrder ()
  {
    var deletedDataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Computer3, new byte[8], pd => pd.DefaultValue);
    deletedDataContainer.Delete();
    var changedDataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, new byte[8], pd => pd.DefaultValue);
    SetPropertyValue(changedDataContainer, typeof(Computer), "SerialNumber", "123456");
    var newDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(newDataContainer, typeof(Computer), "SerialNumber", "123456");

    var tableDefinition = (TableDefinition)deletedDataContainer.ClassDefinition.StorageEntityDefinition;

    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(updateDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
       .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
       .Returns(insertDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(deletedDataContainer.ID, tableDefinition);
    StubTableDefinitionFinder(changedDataContainer.ID, tableDefinition);
    StubTableDefinitionFinder(newDataContainer.ID, tableDefinition);

    var result = _factory.CreateForSave([deletedDataContainer, changedDataContainer, newDataContainer]);

    _tableDefinitionFinderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(4));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EquivalentTo([changedDataContainer, deletedDataContainer]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([newDataContainer]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).AffectedDataContainers, Is.EqualTo([deletedDataContainer, changedDataContainer, newDataContainer]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[3]).AffectedDataContainers, Is.EqualTo([deletedDataContainer]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[3]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));
  }

  [Test]
  public void CreateForSave_Unchanged ()
  {
    var dataContainerUnchanged = DataContainer.CreateForExisting(DomainObjectIDs.Order4, null, pd => pd.DefaultValue);

    StubTableDefinitionFinder(DomainObjectIDs.Order4, _tableDefinition1);

    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    var result = _factory.CreateForSave(new[] { dataContainerUnchanged });

    _tableDefinitionFinderStrictMock.Verify();
    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var tuples = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(tuples.Count, Is.EqualTo(0));
  }


  [Test]
  public void CreateForSave_DoesNotAddOptionalValuesInUpdateForNewObject ()
  {
    var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(dataContainer, typeof(Computer), "SerialNumber", "123456");

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinition = (TableDefinition)dataContainer.ClassDefinition.StorageEntityDefinition;
    StubTableDefinitionFinder(dataContainer.ID, tableDefinition);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(insertDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var spec = specifications[0];
          var actualParameter = spec.CreateDbParameter(new SqlCommand(), "DUMMY");

          var expectedUpdateTvpValue = new SqlTableValuedParameterValue("TVP_Computer_Update", new[]
                                                                                               {
                                                                                                   new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
                                                                                                   new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
                                                                                                   new SqlMetaData("SerialNumber", SqlDbType.NVarChar, 20),
                                                                                                   new SqlMetaData("SerialNumber__IsSet", SqlDbType.Bit),
                                                                                                   new SqlMetaData("EmployeeID", SqlDbType.UniqueIdentifier)
                                                                                               });
          expectedUpdateTvpValue.AddRecord([dataContainer.ID.Value, dataContainer.ID.ClassID, "", false, null]);

          SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

        })
        .Returns(updateDbCommandBuilder.Object)
        .Verifiable($"{nameof(IDbCommandBuilderFactory.CreateForBatchedUpdate)} should have been called.");

    _factory.CreateForSave([dataContainer]);
    _dbCommandBuilderFactoryStrictMock.Verify();
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

    Assert.That(comparedColumnValues.Count, Is.EqualTo(1));
    Assert.That(comparedColumnValues[0].Column, Is.SameAs(StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(tableDefinition.ObjectIDProperty)));
    Assert.That(comparedColumnValues[0].Value, Is.SameAs(dataContainer.ID.Value));
  }
}
