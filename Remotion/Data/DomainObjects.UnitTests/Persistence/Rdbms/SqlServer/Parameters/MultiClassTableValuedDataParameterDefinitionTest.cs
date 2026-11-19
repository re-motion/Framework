// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters
{
  [TestFixture]
  public class MultiClassTableValuedDataParameterDefinitionTest
  {
    private class TableManipulationDataContainerAccessor : ITableManipulationDataContainerAccessor
    {
      private readonly DataContainer _dataContainer;

      public TableManipulationDataContainerAccessor (DataContainer dataContainer)
      {
        ArgumentNullException.ThrowIfNull(dataContainer);

        _dataContainer = dataContainer;
      }

      public ObjectID GetID () => _dataContainer.ID;

      public object GetTimestamp () => _dataContainer.Timestamp!;

      public object GetValue (PropertyDefinition propertyDefinition)
      {
        return _dataContainer.GetValueWithoutEvents(propertyDefinition);
      }

      public object GetOptionalValue (PropertyDefinition propertyDefinition, object defaultValue)
      {
        if (IsOptionalValueSet(propertyDefinition))
          return GetValue(propertyDefinition);

        return null;
      }

      public bool IsOptionalValueSet (PropertyDefinition propertyDefinition)
      {
        return _dataContainer.HasValueChanged(propertyDefinition);
      }
    }

    private TableManipulationRecordDefinitionProvider _tableManipulationRecordDefinitionProvider;

    [SetUp]
    public void Setup ()
    {
      MappingConfiguration.SetCurrent(StandardConfiguration.Instance.GetMappingConfiguration());

      var sqlStorageTypeInformationProvider = new SqlStorageTypeInformationProvider(new DateTimeDefaultStorageTypeProvider());
      var infrastructureStoragePropertyDefinitionProvider =
          new InfrastructureStoragePropertyDefinitionProvider(sqlStorageTypeInformationProvider, new ReflectionBasedStorageNameProvider());
      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();

      _tableManipulationRecordDefinitionProvider = new TableManipulationRecordDefinitionProvider(
          sqlStorageTypeInformationProvider,
          infrastructureStoragePropertyDefinitionProvider,
          rdbmsPersistenceModelProvider);
    }

    [TearDown]
    public void TearDown ()
    {
      MappingConfiguration.SetCurrent(null!);
    }

    [Test]
    public void GetParameterValue_GetsRecordDefinitionFromFunc ()
    {
      var dataContainer = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);

      var getRecordDefinitionFuncHasBeenCalled = false;
      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (provider, definition) =>
          {
            getRecordDefinitionFuncHasBeenCalled = true;
            return provider.GetLockRecordDefinition(definition);
          },
          d => new TableManipulationDataContainerAccessor(d));

      parameterDefinition.GetParameterValue(new List<DataContainer>() { dataContainer });
      Assert.That(getRecordDefinitionFuncHasBeenCalled, Is.True);
    }

    [Test]
    public void GetParameterValue_WithSingleObject_CreatesValidSqlTableValuedParameterValue ()
    {
      var dataContainer1 = DataContainer.CreateForExisting(
          new ObjectID("Company", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")),
          new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
          pd => pd.DefaultValue);

      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (provider, definition) => provider.GetLockRecordDefinition(definition),
          d => new TableManipulationDataContainerAccessor(d));

      var result = parameterDefinition.GetParameterValue(new List<DataContainer>() { dataContainer1 });
      Assert.That(result, Is.TypeOf<SqlTableValuedParameterValue>());


      var expectedTvpValue = new SqlTableValuedParameterValue(
          "TVP_AllTables_Lock",
          new[] { new SqlMetaData("ID", SqlDbType.UniqueIdentifier), new SqlMetaData("Timestamp", SqlDbType.VarBinary, 8) });
      expectedTvpValue.AddRecord([dataContainer1.ID.Value, dataContainer1.Timestamp]);

      var tvpValue = (SqlTableValuedParameterValue)result;
      SqlTableValuedParameterValueChecker.CheckEquals(tvpValue, expectedTvpValue);
    }

    [Test]
    public void GetParameterValue_WithMultipleObjectOfSameClass_CreatesValidSqlTableValuedParameterValue ()
    {
      var dataContainer1 = DataContainer.CreateForExisting(
          new ObjectID("Company", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")),
          new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
          pd => pd.DefaultValue);

      var dataContainer2 = DataContainer.CreateForExisting(
          new ObjectID("Company", new Guid("398FEF1A-3955-46CF-9BBC-ABA07D6A83F1")),
          new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 },
          pd => pd.DefaultValue);

      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (provider, definition) => provider.GetLockRecordDefinition(definition),
          d => new TableManipulationDataContainerAccessor(d));

      var result = parameterDefinition.GetParameterValue(new List<DataContainer>() { dataContainer1, dataContainer2 });
      Assert.That(result, Is.TypeOf<SqlTableValuedParameterValue>());


      var expectedTvpValue = new SqlTableValuedParameterValue(
          "TVP_AllTables_Lock",
          new[] { new SqlMetaData("ID", SqlDbType.UniqueIdentifier), new SqlMetaData("Timestamp", SqlDbType.VarBinary, 8) });
      expectedTvpValue.AddRecord([dataContainer1.ID.Value, dataContainer1.Timestamp]);
      expectedTvpValue.AddRecord([dataContainer2.ID.Value, dataContainer2.Timestamp]);

      var tvpValue = (SqlTableValuedParameterValue)result;
      SqlTableValuedParameterValueChecker.CheckEquals(tvpValue, expectedTvpValue);
    }

    [Test]
    public void GetParameterValue_WithMultipleObjectOfDifferentClass_CreatesValidSqlTableValuedParameterValue ()
    {
      var dataContainer1 = DataContainer.CreateForExisting(
          new ObjectID("Company", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")),
          new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
          pd => pd.DefaultValue);

      var dataContainer2 = DataContainer.CreateForExisting(
          new ObjectID("Customer", new Guid("BB983F05-4600-4E22-8F12-3914CB325760")),
          new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 },
          pd => pd.DefaultValue);

      Assert.That(dataContainer2.ClassDefinition.StorageEntityDefinition, Is.TypeOf<FilterViewDefinition>());
      var dataContainer2TableDefinition = ((FilterViewDefinition)dataContainer2.ClassDefinition.StorageEntityDefinition).BaseEntity;
      Assert.That(dataContainer1.ClassDefinition.StorageEntityDefinition, Is.EqualTo(dataContainer2TableDefinition));

      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (provider, definition) => provider.GetLockRecordDefinition(definition),
          d => new TableManipulationDataContainerAccessor(d));

      var result = parameterDefinition.GetParameterValue(new List<DataContainer>() { dataContainer1, dataContainer2 });
      Assert.That(result, Is.TypeOf<SqlTableValuedParameterValue>());

      var expectedTvpValue = new SqlTableValuedParameterValue(
          "TVP_AllTables_Lock",
          new[] { new SqlMetaData("ID", SqlDbType.UniqueIdentifier), new SqlMetaData("Timestamp", SqlDbType.VarBinary, 8) });
      expectedTvpValue.AddRecord([dataContainer1.ID.Value, dataContainer1.Timestamp]);
      expectedTvpValue.AddRecord([dataContainer2.ID.Value, dataContainer2.Timestamp]);

      var tvpValue = (SqlTableValuedParameterValue)result;
      SqlTableValuedParameterValueChecker.CheckEquals(tvpValue, expectedTvpValue);
    }

    [Test]
    public void GetParameterValue_ThrowsException_WhenMultipleTablesFound ()
    {
      var dataContainerA = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
      var dataContainerB = DataContainer.CreateForExisting(new ObjectID("Order", new Guid("5D35FFBC-3EBE-4DF1-9863-6163EE6F2486")), new byte[8], pd => pd.DefaultValue);

      Assert.That(dataContainerA.ClassDefinition.StorageEntityDefinition, Is.Not.EqualTo(dataContainerB.ClassDefinition.StorageEntityDefinition));
      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (provider, definition) => provider.GetInsertRecordDefinition(definition),
          d => new TableManipulationDataContainerAccessor(d));

      Assert.That(
          () => parameterDefinition.GetParameterValue(new List<DataContainer>() { dataContainerA, dataContainerB }),
          Throws.InvalidOperationException.With.Message.EqualTo("Found different TableTypeDefinitions for one MultiClassTableValuedDataParameterDefinition."));
    }

    [Test]
    public void GetParameterValue_WithEmptyDataContainerList_ThrowsArgumentException ()
    {
      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (_, _) => throw new UnreachableException("This should not be reachable because an other exception should be thrown."),
          d => new TableManipulationDataContainerAccessor(d));

      Assert.That(
          () => parameterDefinition.GetParameterValue(new List<DataContainer>()),
          Throws.ArgumentException.With.Message.EqualTo("The value cannot be an empty collection. (Parameter 'value')"));
    }

    [Test]
    public void CreateDataParameter_CreatesValidSqlParameter ()
    {
      var dataContainerA = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("3F647D79-0CAF-4a53-BAA7-A56831F8CE2D")), new byte[8], pd => pd.DefaultValue);
      var dataContainerB = DataContainer.CreateForExisting(new ObjectID("Computer", new Guid("5D35FFBC-3EBE-4DF1-9863-6163EE6F2486")), new byte[8], pd => pd.DefaultValue);

      var parameterDefinition = new MultiClassTableValuedDataParameterDefinition(
          _tableManipulationRecordDefinitionProvider,
          (provider, definition) => provider.GetInsertRecordDefinition(definition),
          d => new TableManipulationDataContainerAccessor(d));

      var tvpValue = (SqlTableValuedParameterValue)parameterDefinition.GetParameterValue(new List<DataContainer>() { dataContainerA, dataContainerB });

      var command = new SqlCommand();
      var dataParameter = parameterDefinition.CreateDataParameter(command, "@dummy", tvpValue);
      Assert.That(dataParameter, Is.InstanceOf<SqlParameter>());

      var sqlParameter = (SqlParameter)dataParameter;
      Assert.That(sqlParameter.DbType, Is.EqualTo(DbType.Object));
      Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
      Assert.That(sqlParameter.TypeName, Is.EqualTo(tvpValue.TableTypeName));
      Assert.That(sqlParameter.Value, Is.EqualTo(tvpValue));
    }
  }
}
