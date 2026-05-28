// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Moq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Persistence.SortingOptimization;
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
  [UnsafeAccessor(UnsafeAccessorKind.Method)]
  private static extern IRdbmsProviderCommand CreateForSave (
      BatchedSaveCommandFactory saveCommandFactory,
      IEnumerable<DataContainer> dataContainers,
      IPersistenceModelSortingProvider persistenceModelSortingProvider);

  private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStrictMock;
  private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
  private Mock<ITableDefinitionFinder> _tableDefinitionFinderStrictMock;
  private Mock<IPersistenceModelSortingProvider> _sortOrderProviderStrictMock;
  private TestableBatchedSaveCommandFactory _factory;
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

    _sortOrderProviderStrictMock = new Mock<IPersistenceModelSortingProvider>(MockBehavior.Strict);

    _factory = new TestableBatchedSaveCommandFactory(
        _dbCommandBuilderFactoryStrictMock.Object,
        _rdbmsPersistenceModelProvider,
        _tableDefinitionFinderStrictMock.Object,
        tableManipulationRecordDefinitionProvider);

    _tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table1"));
  }

  [Test]
  public void CreateForSave_New_WithIncorrectOrder_ForeignKeyRelevantProperties_WithNewRelatedObject_CreatesUpdate ()
  {
    var computerDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var employeeDataContainer = DataContainer.CreateNew(DomainObjectIDs.Employee2);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    StubTableDefinitionFinder(computerDataContainer);
    StubTableDefinitionFinder(employeeDataContainer);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var computerSpec = specifications[0];
          var actualComputerParameter = computerSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedComputerTvpValue = GetTvpComputerInsertParameterValue();
          AddRecordToComputerInsertTvp(expectedComputerTvpValue, computerDataContainer, "123456", null);
          SqlTableValuedParameterValueChecker.CheckEquals(actualComputerParameter.Value, expectedComputerTvpValue);

          var employeeSpec = specifications[1];
          var actualEmployeeParameter = employeeSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedEmployeeTvpValue = GetTvpEmployeeInsertParameterValue();
          AddRecordToEmployeeInsertTvp(expectedEmployeeTvpValue, employeeDataContainer, null, null);
          SqlTableValuedParameterValueChecker.CheckEquals(actualEmployeeParameter.Value, expectedEmployeeTvpValue);
        })
        .Returns(insertDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var spec = specifications[0];
          var actualParameter = spec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedUpdateTvpValue = GetTvpComputerUpdateParameterValue();
          AddRecordToComputerUpdateTvp(expectedUpdateTvpValue, computerDataContainer, "", false, employeeDataContainer.ID);

          SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

        })
        .Returns(updateDbCommandBuilder.Object);

    var computerEmployeePropertyDefinition = GetPropertySpecification(typeof(Computer), nameof(Computer.Employee));
    SetupSortOrderProviderSortPositionForClassAndTable(computerDataContainer.ClassDefinition, employeeDataContainer.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(computerDataContainer.ClassDefinition)).Returns([computerEmployeePropertyDefinition]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(employeeDataContainer.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [computerDataContainer, employeeDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.Cast<BatchedObjectsRdbmsProviderCommand>().ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));
    Assert.That(contexts[0].AffectedDataContainers, Is.EqualTo([computerDataContainer, employeeDataContainer]));
    Assert.That(contexts[0].CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));

    Assert.That(contexts[1].AffectedDataContainers, Is.EqualTo([computerDataContainer]));
    Assert.That(contexts[1].CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled([computerDataContainer, employeeDataContainer], [computerDataContainer]);
  }

  [Test]
  public void CreateForSave_New_WithIncorrectOrder_ForeignKeyRelevantProperties_WithExistingRelatedObject_DoesNotCreatesUpdate ()
  {
    var computerDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    StubTableDefinitionFinder(computerDataContainer);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var computerSpec = specifications[0];
          var actualComputerParameter = computerSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedComputerTvpValue = GetTvpComputerInsertParameterValue();
          AddRecordToComputerInsertTvp(expectedComputerTvpValue, computerDataContainer, "123456", DomainObjectIDs.Employee2);
          SqlTableValuedParameterValueChecker.CheckEquals(actualComputerParameter.Value, expectedComputerTvpValue);
        })
        .Returns(insertDbCommandBuilder.Object);

    var computerEmployeePropertyDefinition = GetPropertySpecification(typeof(Computer), nameof(Computer.Employee));
    SetupSortOrderProviderSortPosition(computerDataContainer.ClassDefinition, 0, true, false);
    SetupSortOrderProviderSortPosition(DomainObjectIDs.Employee2.ClassDefinition, 1, true, false);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(computerDataContainer.ClassDefinition)).Returns([computerEmployeePropertyDefinition]).Verifiable();

    var result = CreateForSave(_factory, [computerDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.Cast<BatchedObjectsRdbmsProviderCommand>().ToList();

    Assert.That(contexts.Count, Is.EqualTo(1));
    Assert.That(contexts[0].AffectedDataContainers, Is.EqualTo([computerDataContainer]));
    Assert.That(contexts[0].CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled([computerDataContainer]);
  }

  [Test]
  public void CreateForSave_New_WithIncorrectOrder_ForeignKeyRelevantProperties_WithExistingRelatedObject_WithAlwaysBreakHint_CreatesUpdate ()
  {
    var computerDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    StubTableDefinitionFinder(computerDataContainer);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var computerSpec = specifications[0];
          var actualComputerParameter = computerSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedComputerTvpValue = GetTvpComputerInsertParameterValue();
          AddRecordToComputerInsertTvp(expectedComputerTvpValue, computerDataContainer, "123456", null);
          SqlTableValuedParameterValueChecker.CheckEquals(actualComputerParameter.Value, expectedComputerTvpValue);
        })
        .Returns(insertDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var spec = specifications[0];
          var actualParameter = spec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedUpdateTvpValue = GetTvpComputerUpdateParameterValue();
          AddRecordToComputerUpdateTvp(expectedUpdateTvpValue, computerDataContainer, "", false, DomainObjectIDs.Employee2);

          SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

        })
        .Returns(updateDbCommandBuilder.Object);
    var computerEmployeePropertyDefinition = GetPropertySpecification(typeof(Computer), nameof(Computer.Employee), true, ForeignKeyCycleBreakHint.AlwaysBreak);

    SetupSortOrderProviderSortPosition(computerDataContainer.ClassDefinition, 0, true, false);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(computerDataContainer.ClassDefinition)).Returns([computerEmployeePropertyDefinition]).Verifiable();

    var result = CreateForSave(_factory, [computerDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.Cast<BatchedObjectsRdbmsProviderCommand>().ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));
    Assert.That(contexts[0].AffectedDataContainers, Is.EqualTo([computerDataContainer]));
    Assert.That(contexts[0].CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));

    Assert.That(contexts[1].AffectedDataContainers, Is.EqualTo([computerDataContainer]));
    Assert.That(contexts[1].CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled([computerDataContainer], [computerDataContainer]);
  }

  [Test]
  public void CreateForSave_New_WithIncorrectOrder_ForeignKeyRelevantProperties_WithNewRelatedObject_WithNeverBreakHint_DoesNotCreatesUpdate ()
  {
    var computerDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var employeeDataContainer = DataContainer.CreateNew(DomainObjectIDs.Employee2);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    StubTableDefinitionFinder(computerDataContainer);
    StubTableDefinitionFinder(employeeDataContainer);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var computerSpec = specifications[0];
          var actualComputerParameter = computerSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedComputerTvpValue = GetTvpComputerInsertParameterValue();
          AddRecordToComputerInsertTvp(expectedComputerTvpValue, computerDataContainer, "123456", employeeDataContainer.ID);
          SqlTableValuedParameterValueChecker.CheckEquals(actualComputerParameter.Value, expectedComputerTvpValue);

          var employeeSpec = specifications[1];
          var actualEmployeeParameter = employeeSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedEmployeeTvpValue = GetTvpEmployeeInsertParameterValue();
          AddRecordToEmployeeInsertTvp(expectedEmployeeTvpValue, employeeDataContainer, null, null);
          SqlTableValuedParameterValueChecker.CheckEquals(actualEmployeeParameter.Value, expectedEmployeeTvpValue);
        })
        .Returns(insertDbCommandBuilder.Object);


    var computerEmployeePropertyDefinition = GetPropertySpecification(typeof(Computer), nameof(Computer.Employee), true, ForeignKeyCycleBreakHint.NeverBreak);
    SetupSortOrderProviderSortPositionForClassAndTable(computerDataContainer.ClassDefinition, employeeDataContainer.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(computerDataContainer.ClassDefinition)).Returns([computerEmployeePropertyDefinition]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(employeeDataContainer.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [computerDataContainer, employeeDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.Cast<BatchedObjectsRdbmsProviderCommand>().ToList();

    Assert.That(contexts.Count, Is.EqualTo(1));
    Assert.That(contexts[0].AffectedDataContainers, Is.EqualTo([computerDataContainer, employeeDataContainer]));
    Assert.That(contexts[0].CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled([computerDataContainer, employeeDataContainer]);
  }

  [Test]
  public void CreateForSave_New_WithIncorrectOrder_NoForeignKeyRelevantProperties_WithNewRelatedObject_DoesNotCreatesUpdate ()
  {
    var computerDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var employeeDataContainer = DataContainer.CreateNew(DomainObjectIDs.Employee2);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    StubTableDefinitionFinder(computerDataContainer);
    StubTableDefinitionFinder(employeeDataContainer);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var computerSpec = specifications[0];
          var actualComputerParameter = computerSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedComputerTvpValue = GetTvpComputerInsertParameterValue();
          AddRecordToComputerInsertTvp(expectedComputerTvpValue, computerDataContainer, "123456", DomainObjectIDs.Employee2);
          SqlTableValuedParameterValueChecker.CheckEquals(actualComputerParameter.Value, expectedComputerTvpValue);

          var employeeSpec = specifications[1];
          var actualEmployeeParameter = employeeSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedEmployeeTvpValue = GetTvpEmployeeInsertParameterValue();
          AddRecordToEmployeeInsertTvp(expectedEmployeeTvpValue, employeeDataContainer, null, null);
          SqlTableValuedParameterValueChecker.CheckEquals(actualEmployeeParameter.Value, expectedEmployeeTvpValue);
        })
        .Returns(insertDbCommandBuilder.Object);

    SetupSortOrderProviderSortPositionForClassAndTable(computerDataContainer.ClassDefinition, employeeDataContainer.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(computerDataContainer.ClassDefinition)).Returns([]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(employeeDataContainer.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [computerDataContainer, employeeDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.Cast<BatchedObjectsRdbmsProviderCommand>().ToList();

    Assert.That(contexts.Count, Is.EqualTo(1));
    Assert.That(contexts[0].AffectedDataContainers, Is.EqualTo([computerDataContainer, employeeDataContainer]));
    Assert.That(contexts[0].CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled([computerDataContainer, employeeDataContainer]);
  }

  [Test]
  public void CreateForSave_New_WithCorrectOrder_WithNewRelatedObject_DoesNotCreatesUpdate ()
  {
    var computerDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var employeeDataContainer = DataContainer.CreateNew(DomainObjectIDs.Employee2);
    SetPropertyValue(computerDataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    StubTableDefinitionFinder(computerDataContainer);
    StubTableDefinitionFinder(employeeDataContainer);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedInsert(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var employeeSpec = specifications[0];
          var actualEmployeeParameter = employeeSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedEmployeeTvpValue = GetTvpEmployeeInsertParameterValue();
          AddRecordToEmployeeInsertTvp(expectedEmployeeTvpValue, employeeDataContainer, null, null);
          SqlTableValuedParameterValueChecker.CheckEquals(actualEmployeeParameter.Value, expectedEmployeeTvpValue);

          var computerSpec = specifications[1];
          var actualComputerParameter = computerSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedComputerTvpValue = GetTvpComputerInsertParameterValue();
          AddRecordToComputerInsertTvp(expectedComputerTvpValue, computerDataContainer, "123456", DomainObjectIDs.Employee2);
          SqlTableValuedParameterValueChecker.CheckEquals(actualComputerParameter.Value, expectedComputerTvpValue);
        })
        .Returns(insertDbCommandBuilder.Object);

    SetupSortOrderProviderSortPositionForClassAndTable(employeeDataContainer.ClassDefinition, computerDataContainer.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(It.IsAny<ClassDefinition>())).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [computerDataContainer, employeeDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.Cast<BatchedObjectsRdbmsProviderCommand>().ToList();

    Assert.That(contexts.Count, Is.EqualTo(1));
    Assert.That(contexts[0].AffectedDataContainers, Is.EqualTo([employeeDataContainer, computerDataContainer]));
    Assert.That(contexts[0].CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled([computerDataContainer, employeeDataContainer]);
  }

  [Test]
  public void CreateForSave_Changed ()
  {
    var dataContainerChangedSerialNumber = DataContainer.CreateForExisting(DomainObjectIDs.Computer1, new byte[8], pd => pd.DefaultValue);
    SetPropertyValue(dataContainerChangedSerialNumber, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var dataContainerChangedEmployee = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, new byte[8], pd => pd.DefaultValue);
    SetPropertyValue(dataContainerChangedEmployee, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);
    var dataContainerChangedMarkedAsChanged = DataContainer.CreateForExisting(DomainObjectIDs.Computer3, new byte[8], pd => pd.DefaultValue);
    dataContainerChangedMarkedAsChanged.MarkAsChanged();

    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinition = (TableDefinition)dataContainerChangedSerialNumber.ClassDefinition.StorageEntityDefinition;
    Assertion.DebugAssert((TableDefinition)dataContainerChangedEmployee.ClassDefinition.StorageEntityDefinition == tableDefinition);
    Assertion.DebugAssert((TableDefinition)dataContainerChangedMarkedAsChanged.ClassDefinition.StorageEntityDefinition == tableDefinition);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var spec = specifications[0];
          var actualParameter = spec.CreateDbParameter(new SqlCommand(), "DUMMY");

          var expectedUpdateTvpValue = GetTvpComputerUpdateParameterValue();
          AddRecordToComputerUpdateTvp(expectedUpdateTvpValue, dataContainerChangedSerialNumber, "123456", true, null);
          AddRecordToComputerUpdateTvp(expectedUpdateTvpValue, dataContainerChangedEmployee, "", false, DomainObjectIDs.Employee2);
          AddRecordToComputerUpdateTvp(expectedUpdateTvpValue, dataContainerChangedMarkedAsChanged, "", false, null);

          SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

        })
        .Returns(updateDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerChangedSerialNumber.ID, tableDefinition);
    StubTableDefinitionFinder(dataContainerChangedEmployee.ID, tableDefinition);
    StubTableDefinitionFinder(dataContainerChangedMarkedAsChanged.ID, tableDefinition);

    var result =
        CreateForSave(_factory, [dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged], _sortOrderProviderStrictMock.Object);

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    _tableDefinitionFinderStrictMock.Verify();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged]));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        [dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged, dataContainerChangedSerialNumber],
        null,
        [dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged, dataContainerChangedSerialNumber]);
  }

  [Test]
  public void CreateForSave_Deleted_WithIncorrectOrder_ForeignKeyRelevantProperties_WithDeletedRelatedObject_CreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Product));
    var propertyDefinitionProductReviewReviewer = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Reviewer));
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct.PropertyDefinition)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer.PropertyDefinition)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProduct.Delete();
    dataContainerProductReview.Delete();

    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
       .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
       .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
       {
         Assert.That(specifications.Count, Is.EqualTo(1));
         var spec = specifications[0];
         var actualParameter = spec.CreateDbParameter(new SqlCommand(), "DUMMY");

         var expectedUpdateTvpValue = GetTvpProductReviewUpdateParameterValue();
         AddRecordToProductReviewUpdateTvp(expectedUpdateTvpValue, dataContainerProductReview, null, null, DateTime.MinValue, "", false);

         SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

       })
       .Returns(updateDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var productReviewSpec = specifications[0];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);

          var productSpec = specifications[1];
          var actualProductParameter = productSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductTvpValue, dataContainerProduct);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductParameter.Value, expectedProductTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);

    SetupSortOrderProviderSortPositionForClassAndTable(dataContainerProduct.ClassDefinition, dataContainerProductReview.ClassDefinition);

    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProductReview.ClassDefinition)).Returns([propertyDefinitionProductReviewProduct]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProduct.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(3));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProduct, dataContainerProductReview]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).AffectedDataContainers, Is.EqualTo([dataContainerProduct, dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        [dataContainerProductReview],
        [dataContainerProduct, dataContainerProductReview],
        [dataContainerProduct, dataContainerProductReview]);
  }

  [Test]
  public void CreateForSave_Deleted_WithIncorrectOrder_ForeignKeyRelevantProperties_WithDeletedRelatedObject_WithNeverBreakHint_DoesNotCreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Product), true, ForeignKeyCycleBreakHint.NeverBreak);
    var propertyDefinitionProductReviewReviewer = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Reviewer), true, ForeignKeyCycleBreakHint.NeverBreak);
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct.PropertyDefinition)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer.PropertyDefinition)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProduct.Delete();
    dataContainerProductReview.Delete();

    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var productReviewSpec = specifications[0];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);

          var productSpec = specifications[1];
          var actualProductParameter = productSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductTvpValue, dataContainerProduct);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductParameter.Value, expectedProductTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);

    SetupSortOrderProviderSortPositionForClassAndTable(dataContainerProduct.ClassDefinition, dataContainerProductReview.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProductReview.ClassDefinition)).Returns([propertyDefinitionProductReviewProduct]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProduct.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProduct, dataContainerProductReview]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProduct, dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        null,
        [dataContainerProduct, dataContainerProductReview],
        [dataContainerProduct, dataContainerProductReview]);
  }

  [Test]
  public void CreateForSave_Deleted_WithIncorrectOrder_NoForeignKeyRelevantProperties_WithDeletedRelatedObject_DoesNotCreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertyDefinition(typeof(ProductReview), nameof(ProductReview.Product));
    var propertyDefinitionProductReviewReviewer = GetPropertyDefinition(typeof(ProductReview), nameof(ProductReview.Reviewer));
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProduct.Delete();
    dataContainerProductReview.Delete();

    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var productReviewSpec = specifications[0];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);

          var productSpec = specifications[1];
          var actualProductParameter = productSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductTvpValue, dataContainerProduct);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductParameter.Value, expectedProductTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);

    SetupSortOrderProviderSortPositionForClassAndTable(dataContainerProduct.ClassDefinition, dataContainerProductReview.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProductReview.ClassDefinition)).Returns([]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProduct.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProduct, dataContainerProductReview]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProduct, dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        null,
        [dataContainerProduct, dataContainerProductReview],
        [dataContainerProduct, dataContainerProductReview]);
  }

  [Test]
  public void CreateForSave_Deleted_WithCorrectOrder_WithDeletedRelatedObject_DoesNotCreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertyDefinition(typeof(ProductReview), nameof(ProductReview.Product));
    var propertyDefinitionProductReviewReviewer = GetPropertyDefinition(typeof(ProductReview), nameof(ProductReview.Reviewer));
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProduct.Delete();
    dataContainerProductReview.Delete();

    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(2));
          var productSpec = specifications[0];
          var actualProductParameter = productSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductTvpValue, dataContainerProduct);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductParameter.Value, expectedProductTvpValue);

          var productReviewSpec = specifications[1];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);

    SetupSortOrderProviderSortPositionForClassAndTable(dataContainerProductReview.ClassDefinition, dataContainerProduct.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(It.IsAny<ClassDefinition>())).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview, dataContainerProduct]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview, dataContainerProduct]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        null,
        [dataContainerProduct, dataContainerProductReview],
        [dataContainerProduct, dataContainerProductReview]);
  }

  [Test]
  public void CreateForSave_Deleted_WithCorrectOrder_WithExistingRelatedObject_DoesNotCreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Product));
    var propertyDefinitionProductReviewReviewer = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Reviewer));
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct.PropertyDefinition)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer.PropertyDefinition)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProductReview.Delete();

    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var productReviewSpec = specifications[0];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);
    SetupSortOrderProviderSortPosition(dataContainerProductReview.ClassDefinition, 0, true, true);
    SetupSortOrderProviderSortPosition(dataContainerProduct.ClassDefinition, 1, true, true);
    SetupSortOrderProviderSortPosition(DomainObjectIDs.Person1.ClassDefinition, 2, true, false);

    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProductReview.ClassDefinition)).Returns([propertyDefinitionProductReviewProduct, propertyDefinitionProductReviewReviewer]).Verifiable("AB");

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        null,
        [dataContainerProductReview],
        [dataContainerProductReview]);
  }

  [Test]
  public void CreateForSave_Deleted_WithCorrectOrder_WithExistingRelatedObject_WithAlwaysBreakHint_CreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Product), true, ForeignKeyCycleBreakHint.AlwaysBreak);
    var propertyDefinitionProductReviewReviewer = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Reviewer), true, ForeignKeyCycleBreakHint.AlwaysBreak);
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct.PropertyDefinition)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer.PropertyDefinition)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProductReview.Delete();

    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedUpdate(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var spec = specifications[0];
          var actualParameter = spec.CreateDbParameter(new SqlCommand(), "DUMMY");

          var expectedUpdateTvpValue = GetTvpProductReviewUpdateParameterValue();
          AddRecordToProductReviewUpdateTvp(expectedUpdateTvpValue, dataContainerProductReview, null, null, DateTime.MinValue, "", false);

          SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

        })
        .Returns(updateDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var productReviewSpec = specifications[0];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);
    SetupSortOrderProviderSortPosition(dataContainerProductReview.ClassDefinition, 0, true, true);
    SetupSortOrderProviderSortPosition(dataContainerProduct.ClassDefinition, 1, false, true);

    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProductReview.ClassDefinition)).Returns([propertyDefinitionProductReviewProduct, propertyDefinitionProductReviewReviewer]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(3));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        [dataContainerProductReview],
        [dataContainerProductReview],
        [dataContainerProductReview]);
  }

  [Test]
  public void CreateForSave_Deleted_WithIncorrectOrder_ForeignKeyRelevantProperties_WithExistingRelatedObject_DoesNotCreatesUpdate ()
  {
    var propertyDefinitionProductReviewProduct = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Product));
    var propertyDefinitionProductReviewReviewer = GetPropertySpecification(typeof(ProductReview), nameof(ProductReview.Reviewer));
    var dataContainerProduct = DataContainer.CreateForExisting(DomainObjectIDs.Product1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd => pd.DefaultValue);
    var dataContainerProductReview = DataContainer.CreateForExisting(DomainObjectIDs.ProductReview1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, pd =>
    {
      if (pd == propertyDefinitionProductReviewProduct.PropertyDefinition)
        return dataContainerProduct.ID;

      if (pd == propertyDefinitionProductReviewReviewer.PropertyDefinition)
        return DomainObjectIDs.Person1;
      return pd.DefaultValue;
    });

    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Product), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Reviewer), null);
    SetPropertyValue(dataContainerProductReview, typeof(ProductReview), nameof(ProductReview.Comment), "Comment");

    dataContainerProductReview.Delete();

    var deleteDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var lockDbCommandBuilder = new Mock<IDbCommandBuilder>();

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedDelete(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Callback((IReadOnlyList<IBatchedCommandSpecification> specifications) =>
        {
          Assert.That(specifications.Count, Is.EqualTo(1));
          var productReviewSpec = specifications[0];
          var actualProductReviewParameter = productReviewSpec.CreateDbParameter(new SqlCommand(), "DUMMY");
          var expectedProductReviewTvpValue = GetTvpAllTablesDeleteParameterValue();
          AddRecordToAllTablesDeleteTvp(expectedProductReviewTvpValue, dataContainerProductReview);
          SqlTableValuedParameterValueChecker.CheckEquals(actualProductReviewParameter.Value, expectedProductReviewTvpValue);
        })
        .Returns(deleteDbCommandBuilder.Object);

    _dbCommandBuilderFactoryStrictMock
        .Setup(stub => stub.CreateForBatchedLock(It.IsAny<IReadOnlyList<IBatchedCommandSpecification>>()))
        .Returns(lockDbCommandBuilder.Object);

    StubTableDefinitionFinder(dataContainerProductReview);
    StubTableDefinitionFinder(dataContainerProduct);

    SetupSortOrderProviderSortPositionForClassAndTable(dataContainerProduct.ClassDefinition, dataContainerProductReview.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerProductReview.ClassDefinition)).Returns([propertyDefinitionProductReviewProduct]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerProductReview, dataContainerProduct], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerProductReview]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        null,
        [dataContainerProductReview],
        [dataContainerProductReview]);
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

    var tableDefinitionA = GetTableDefinition(dataContainerDeletedWithoutRelations.ClassDefinition);
    var tableDefinitionB = GetTableDefinition(dataContainerDeletedWithRelations1.ClassDefinition);

    Assertion.DebugAssert(GetTableDefinition(dataContainerDeletedWithRelations2.ClassDefinition) == tableDefinitionB);

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

    var propertyDefinitionMock = GetPropertySpecification(typeof(Computer), "Employee");
    SetupSortOrderProviderSortPositionForClassAndTable(dataContainerDeletedWithoutRelations.ClassDefinition, dataContainerDeletedWithRelations1.ClassDefinition);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerDeletedWithRelations1.ClassDefinition)).Returns([propertyDefinitionMock]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainerDeletedWithoutRelations.ClassDefinition)).Returns([]).Verifiable();

    var result = CreateForSave(_factory, [dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(2));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EqualTo([dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));

    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));

    AssertTableManipulationAccessorExtensionPointsCalled(
        null,
        null,
        [dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2],
        [dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2]);
  }

  private void SetupSortOrderProviderSortPosition (ClassDefinition classDefinition, int position, bool forClassDefinition, bool forTableDefintion)
  {
    if (forClassDefinition)
      _sortOrderProviderStrictMock.Setup(stub => stub.GetSortPosition(classDefinition)).Returns(position).Verifiable(
          $"{nameof(IPersistenceModelSortingProvider.GetSortPosition)} with class definition '{classDefinition.ID}' has not been called.");

    if (forTableDefintion)
    {
      var tableDefinition = GetTableDefinition(classDefinition);
      _sortOrderProviderStrictMock.Setup(stub => stub.GetSortPosition(tableDefinition)).Returns(position).Verifiable(
          $"{nameof(IPersistenceModelSortingProvider.GetSortPosition)} with table definition '{tableDefinition.TableName.EntityName}' has not been called.");
    }
  }

  private void SetupSortOrderProviderSortPositionForClassAndTable (params ClassDefinition[] classDefinitions)
  {
    for (int i = 0; i < classDefinitions.Length; i++)
    {
      var classDefinition = classDefinitions[i];
      SetupSortOrderProviderSortPosition(classDefinition, i, true, true);
    }
  }

  [Test]
  public void CreateForSave_LockInsertUpdateDelete_CommandsAreInCorrectOrder ()
  {
    var deletedDataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Computer3, new byte[8], pd => pd.DefaultValue);
    deletedDataContainer.Delete();
    var changedDataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, new byte[8], pd => pd.DefaultValue);
    SetPropertyValue(changedDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var newDataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(newDataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");

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

    var propertyDefinitionMock = GetPropertySpecification(typeof(Computer), "Employee");
    SetupSortOrderProviderSortPosition(DomainObjectIDs.Computer1.ClassDefinition, 0, true, false);
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(It.IsAny<ClassDefinition>())).Returns([propertyDefinitionMock]).Verifiable();

    var result = CreateForSave(_factory, [deletedDataContainer, changedDataContainer, newDataContainer], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var contexts = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(contexts.Count, Is.EqualTo(4));

    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).AffectedDataContainers, Is.EquivalentTo([changedDataContainer, deletedDataContainer]));
    Assert.That(((BatchedLockRdbmsProviderCommand)contexts[0]).CommandBuilder, Is.SameAs(lockDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).AffectedDataContainers, Is.EqualTo([newDataContainer]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[1]).CommandBuilder, Is.SameAs(insertDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).AffectedDataContainers, Is.EqualTo([changedDataContainer]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[2]).CommandBuilder, Is.SameAs(updateDbCommandBuilder.Object));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[3]).AffectedDataContainers, Is.EqualTo([deletedDataContainer]));
    Assert.That(((BatchedObjectsRdbmsProviderCommand)contexts[3]).CommandBuilder, Is.SameAs(deleteDbCommandBuilder.Object));
  }

  [Test]
  public void CreateForSave_Unchanged ()
  {
    var dataContainerUnchanged = DataContainer.CreateForExisting(DomainObjectIDs.Order4, null, pd => pd.DefaultValue);

    StubTableDefinitionFinder(DomainObjectIDs.Order4, _tableDefinition1);

    var result = CreateForSave(_factory, [dataContainerUnchanged], _sortOrderProviderStrictMock.Object);

    _tableDefinitionFinderStrictMock.Verify();

    Assert.That(result, Is.TypeOf(typeof(CompoundRdbmsProviderCommand)));
    var tuples = ((CompoundRdbmsProviderCommand)result).InnerCommands.ToList();

    Assert.That(tuples.Count, Is.EqualTo(0));

    AssertTableManipulationAccessorExtensionPointsCalled();
  }

  [Test]
  public void CreateForSave_DoesNotAddOptionalValuesInUpdateForNewObject ()
  {
    var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Computer1);
    SetPropertyValue(dataContainer, typeof(Computer), nameof(Computer.SerialNumber), "123456");
    var employeeDataContainer = DataContainer.CreateNew(DomainObjectIDs.Employee2, pd => pd.DefaultValue);
    SetPropertyValue(dataContainer, typeof(Computer), nameof(Computer.Employee), DomainObjectIDs.Employee2);

    var insertDbCommandBuilder = new Mock<IDbCommandBuilder>();
    var updateDbCommandBuilder = new Mock<IDbCommandBuilder>();

    var tableDefinition = (TableDefinition)dataContainer.ClassDefinition.StorageEntityDefinition;
    var tableDefinitionB = (TableDefinition)employeeDataContainer.ClassDefinition.StorageEntityDefinition;
    StubTableDefinitionFinder(dataContainer.ID, tableDefinition);
    StubTableDefinitionFinder(employeeDataContainer.ID, tableDefinitionB);

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

          var expectedUpdateTvpValue = GetTvpComputerUpdateParameterValue();
          AddRecordToComputerUpdateTvp(expectedUpdateTvpValue, dataContainer, "", false, employeeDataContainer.ID);

          SqlTableValuedParameterValueChecker.CheckEquals(actualParameter.Value, expectedUpdateTvpValue);

        })
        .Returns(updateDbCommandBuilder.Object)
        .Verifiable($"{nameof(IDbCommandBuilderFactory.CreateForBatchedUpdate)} should have been called.");

    var propertyDefinitionMock = GetPropertySpecification(typeof(Computer), "Employee");
    var propertyDefinitionMockA = GetPropertySpecification(typeof(Employee), nameof(Employee.Supervisor));

    SetupSortOrderProviderSortPositionForClassAndTable(dataContainer.ClassDefinition, employeeDataContainer.ClassDefinition);

    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(dataContainer.ClassDefinition)).Returns([propertyDefinitionMock]).Verifiable();
    _sortOrderProviderStrictMock.Setup(stub => stub.GetPropertySpecificationsForForeignKeyProperties(employeeDataContainer.ClassDefinition)).Returns([propertyDefinitionMockA]).Verifiable();

    CreateForSave(_factory, [dataContainer, employeeDataContainer], _sortOrderProviderStrictMock.Object);
    _dbCommandBuilderFactoryStrictMock.Verify();
    _sortOrderProviderStrictMock.Verify();
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

  private void AssertTableManipulationAccessorExtensionPointsCalled (
      IReadOnlyCollection<DataContainer> expectedInsert = null,
      IReadOnlyCollection<DataContainer> expectedUpdate = null,
      IReadOnlyCollection<DataContainer> expectedDeletes = null,
      IReadOnlyCollection<DataContainer> expectedLocks = null)
  {

    Assert.That(_factory.CreateInsertDataContainerAccessorCalledForDataContainers, Is.EquivalentTo(expectedInsert ?? Array.Empty<DataContainer>()));
    Assert.That(_factory.CreateUpdateDataContainerAccessorCalledForDataContainers, Is.EquivalentTo(expectedUpdate ?? Array.Empty<DataContainer>()));
    Assert.That(_factory.CreateDeleteDataContainerAccessorForDataContainers, Is.EquivalentTo(expectedDeletes ?? Array.Empty<DataContainer>()));
    Assert.That(_factory.CreateLockDataContainerAccessorAccessorCalledForDataContainers, Is.EquivalentTo(expectedLocks ?? Array.Empty<DataContainer>()));
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

  private SqlTableValuedParameterValue GetTvpComputerInsertParameterValue ()
  {
    return new SqlTableValuedParameterValue(
        "TVP_Computer_Insert",
        [
            new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
            new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
            new SqlMetaData("SerialNumber", SqlDbType.NVarChar, 20),
            new SqlMetaData("EmployeeID", SqlDbType.UniqueIdentifier)
        ]);
  }

  private SqlTableValuedParameterValue GetTvpEmployeeInsertParameterValue ()
  {
    return new SqlTableValuedParameterValue(
        "TVP_Employee_Insert",
        [
            new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
            new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
            new SqlMetaData("Name", SqlDbType.NVarChar, 100),
            new SqlMetaData("SupervisorID", SqlDbType.UniqueIdentifier)
        ]);
  }

  private SqlTableValuedParameterValue GetTvpComputerUpdateParameterValue ()
  {
    return new SqlTableValuedParameterValue(
        "TVP_Computer_Update",
        [
            new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
            new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
            new SqlMetaData("SerialNumber", SqlDbType.NVarChar, 20),
            new SqlMetaData("SerialNumber__IsSet", SqlDbType.Bit),
            new SqlMetaData("EmployeeID", SqlDbType.UniqueIdentifier)
        ]);
  }

  private SqlTableValuedParameterValue GetTvpProductReviewUpdateParameterValue ()
  {
    return new SqlTableValuedParameterValue(
        "TVP_ProductReview_Update",
        [
            new SqlMetaData("ID", SqlDbType.UniqueIdentifier),
            new SqlMetaData("ClassID", SqlDbType.VarChar, 100),
            new SqlMetaData("ProductID", SqlDbType.UniqueIdentifier),
            new SqlMetaData("ReviewerID", SqlDbType.UniqueIdentifier),
            new SqlMetaData("CreatedAt", SqlDbType.DateTime2),
            new SqlMetaData("Comment", SqlDbType.NVarChar, 1000),
            new SqlMetaData("Comment__IsSet", SqlDbType.Bit)
        ]);
  }

  private SqlTableValuedParameterValue GetTvpAllTablesDeleteParameterValue ()
  {
    return new SqlTableValuedParameterValue(
        "TVP_AllTables_Delete",
        [
            new SqlMetaData("ID", SqlDbType.UniqueIdentifier)
        ]);
  }

  private void AddRecordToComputerInsertTvp (SqlTableValuedParameterValue parameterValue, DataContainer dataContainer, string serialNumber, [CanBeNull] ObjectID emplObjectID)
  {
    parameterValue.AddRecord([dataContainer.ID.Value, dataContainer.ID.ClassID, serialNumber, emplObjectID?.Value]);
  }

  private void AddRecordToEmployeeInsertTvp (SqlTableValuedParameterValue parameterValue, DataContainer dataContainer, string name, [CanBeNull] ObjectID supervisorID)
  {
    parameterValue.AddRecord([dataContainer.ID.Value, dataContainer.ID.ClassID, name, supervisorID?.Value]);
  }

  private void AddRecordToComputerUpdateTvp (SqlTableValuedParameterValue parameterValue, DataContainer dataContainer, string serialNumber, bool serialNumberIsSet, [CanBeNull] ObjectID emplObjectID)
  {
    parameterValue.AddRecord([dataContainer.ID.Value, dataContainer.ID.ClassID, serialNumber, serialNumberIsSet, emplObjectID?.Value]);
  }

  private void AddRecordToProductReviewUpdateTvp (SqlTableValuedParameterValue parameterValue, DataContainer dataContainer, Guid? productID, Guid? reviewerID, DateTime createdAt, string comment, bool commentIsSet)
  {
    parameterValue.AddRecord([dataContainer.ID.Value, dataContainer.ID.ClassID, productID, reviewerID, createdAt, comment, commentIsSet]);
  }

  private void AddRecordToAllTablesDeleteTvp (SqlTableValuedParameterValue parameterValue, DataContainer dataContainer)
  {
    parameterValue.AddRecord([dataContainer.ID.Value]);
  }

  protected SortingOptimizationObjectIDPropertySpecification GetPropertySpecification (Type declaringType, string shortPropertyName, bool hasForeignKeyConstraint = true, ForeignKeyCycleBreakHint cycleBreakHint = ForeignKeyCycleBreakHint.Automatic)
  {
    var propertyDefinition = GetPropertyDefinition(declaringType, shortPropertyName);
    return new SortingOptimizationObjectIDPropertySpecification(propertyDefinition, hasForeignKeyConstraint, cycleBreakHint);
  }

  private void StubTableDefinitionFinder (ObjectID objectID, TableDefinition tableDefinition)
  {
    _tableDefinitionFinderStrictMock.Setup(mock => mock.GetTableDefinition(objectID)).Returns(tableDefinition).Verifiable();
  }

  private TableDefinition StubTableDefinitionFinder (DataContainer dataContainer)
  {
    var tableDefinition = GetTableDefinition(dataContainer.ClassDefinition);
    StubTableDefinitionFinder(dataContainer.ID, tableDefinition);
    return tableDefinition;
  }

  private TableDefinition GetTableDefinition (ClassDefinition classDefinition)
  {
    var entityDefinition = classDefinition.StorageEntityDefinition;
    while (true)
    {
      if (entityDefinition is TableDefinition tableDefinition)
      {
        return tableDefinition;
      }

      if (entityDefinition is not FilterViewDefinition filterViewDefinition)
      {
        Assert.Fail($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}");
        throw new UnreachableException();
      }

      entityDefinition = filterViewDefinition.BaseEntity;
    }
  }
}
