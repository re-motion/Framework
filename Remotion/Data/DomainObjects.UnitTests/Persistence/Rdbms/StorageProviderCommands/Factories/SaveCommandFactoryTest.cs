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
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class SaveCommandFactoryTest : StandardMappingTest
  {
    private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStrictMock;
    private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private Mock<ITableDefinitionFinder> _tableDefinitionFinderStrictMock;
    private SaveCommandFactory _factory;
    private TableDefinition _tableDefinition1;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStrictMock = new Mock<IDbCommandBuilderFactory>(MockBehavior.Strict);
      _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      _tableDefinitionFinderStrictMock = new Mock<ITableDefinitionFinder>(MockBehavior.Strict);

      _factory = new SaveCommandFactory(
          _dbCommandBuilderFactoryStrictMock.Object,
          _rdbmsPersistenceModelProvider,
          _tableDefinitionFinderStrictMock.Object);

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

      var tableDefinitionA = (TableDefinition)dataContainerNew1.ClassDefinition.StorageEntityDefinition;
      Assertion.DebugAssert((TableDefinition)dataContainerNew2.ClassDefinition.StorageEntityDefinition == tableDefinitionA);
      var tableDefinitionB = (TableDefinition)dataContainerNewWithoutRelations.ClassDefinition.StorageEntityDefinition;

      var insertStatementParameters = new Queue<DataContainer>(new[] { dataContainerNew1, dataContainerNew2 });
      var insertStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { insertDbCommandBuilderNew1.Object, insertDbCommandBuilderNew2.Object });
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForInsert(tableDefinitionA, It.IsNotNull<IEnumerable<ColumnValue>>()))
          .Callback(
              (TableDefinition _, IEnumerable<ColumnValue> insertedColumns) => CheckInsertedComputerColumns(insertedColumns.ToArray(), insertStatementParameters.Dequeue()))
          .Returns(() => insertStatementReturnValues.Dequeue());
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForInsert(tableDefinitionB, It.IsAny<IEnumerable<ColumnValue>>()))
          .Returns(insertDbCommandBuilderNew3.Object);

      var updateStatementParameters = new Queue<DataContainer>(new[] { dataContainerNew1, dataContainerNew2 });
      var updateStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { updateDbCommandBuilderNew1.Object, updateDbCommandBuilderNew2.Object });
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForUpdate(tableDefinitionA, It.IsNotNull<IEnumerable<ColumnValue>>(), It.IsNotNull<IEnumerable<ColumnValue>>()))
          .Callback(
              (TableDefinition _, IEnumerable<ColumnValue> updatedColumns, IEnumerable<ColumnValue> comparedColumnValues) =>
              {
                var dataContainer = updateStatementParameters.Dequeue();
                CheckUpdatedComputerColumns(updatedColumns.ToArray(), dataContainer, expectEmployee: true, expectSerialNumber: false, expectClassID: false);
                CheckComparedColumns(comparedColumnValues.ToArray(), dataContainer, tableDefinitionA);
              })
          .Returns(() => updateStatementReturnValues.Dequeue());

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
      Assert.That(result, Is.TypeOf(typeof(MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand)result).Tuples.ToList();

      Assert.That(tuples.Count, Is.EqualTo(5));
      Assert.That(tuples[0].Item1, Is.EqualTo(dataContainerNew1.ID));
      Assert.That(tuples[0].Item2, Is.SameAs(insertDbCommandBuilderNew1.Object));
      Assert.That(tuples[1].Item1, Is.EqualTo(dataContainerNew2.ID));
      Assert.That(tuples[1].Item2, Is.SameAs(insertDbCommandBuilderNew2.Object));
      Assert.That(tuples[2].Item1, Is.EqualTo(dataContainerNewWithoutRelations.ID));
      Assert.That(tuples[2].Item2, Is.SameAs(insertDbCommandBuilderNew3.Object));
      Assert.That(tuples[3].Item1, Is.EqualTo(dataContainerNew1.ID));
      Assert.That(tuples[3].Item2, Is.SameAs(updateDbCommandBuilderNew1.Object));
      Assert.That(tuples[4].Item1, Is.EqualTo(dataContainerNew2.ID));
      Assert.That(tuples[4].Item2, Is.SameAs(updateDbCommandBuilderNew2.Object));
    }

    [Test]
    public void CreateForSave_Changed ()
    {
      var dataContainerChangedSerialNumber = DataContainer.CreateForExisting(DomainObjectIDs.Computer1, null, pd => pd.DefaultValue);
      SetPropertyValue(dataContainerChangedSerialNumber, typeof(Computer), "SerialNumber", "123456");
      var dataContainerChangedEmployee = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, null, pd => pd.DefaultValue);
      SetPropertyValue(dataContainerChangedEmployee, typeof(Computer), "Employee", DomainObjectIDs.Employee2);
      var dataContainerChangedMarkedAsChanged = DataContainer.CreateForExisting(DomainObjectIDs.Computer3, null, pd => pd.DefaultValue);
      dataContainerChangedMarkedAsChanged.MarkAsChanged();

      var updateDbCommandBuilderChangedSerialNumber = new Mock<IDbCommandBuilder>();
      var updateDbCommandBuilderChangedEmployee = new Mock<IDbCommandBuilder>();
      var updateDbCommandBuilderMarkedAsChanged = new Mock<IDbCommandBuilder>();

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
          .Callback(
              (TableDefinition _, IEnumerable<ColumnValue> updatedColumns, IEnumerable<ColumnValue> comparedColumnValues) =>
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

      StubTableDefinitionFinder(dataContainerChangedSerialNumber.ID, tableDefinition);
      StubTableDefinitionFinder(dataContainerChangedEmployee.ID, tableDefinition);
      StubTableDefinitionFinder(dataContainerChangedMarkedAsChanged.ID, tableDefinition);

      var result =
          _factory.CreateForSave(new[] { dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged });

      Assert.That(result, Is.TypeOf(typeof(MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand)result).Tuples.ToList();

      _tableDefinitionFinderStrictMock.Verify();
      Assert.That(updateStatementReturnValues, Is.Empty);
      Assert.That(tuples.Count, Is.EqualTo(3));
      Assert.That(tuples[0].Item1, Is.EqualTo(dataContainerChangedSerialNumber.ID));
      Assert.That(tuples[0].Item2, Is.SameAs(updateDbCommandBuilderChangedSerialNumber.Object));
      Assert.That(tuples[1].Item1, Is.EqualTo(dataContainerChangedEmployee.ID));
      Assert.That(tuples[1].Item2, Is.SameAs(updateDbCommandBuilderChangedEmployee.Object));
      Assert.That(tuples[2].Item1, Is.EqualTo(dataContainerChangedMarkedAsChanged.ID));
      Assert.That(tuples[2].Item2, Is.SameAs(updateDbCommandBuilderMarkedAsChanged.Object));
    }

    [Test]
    public void CreateForSave_Deleted ()
    {
      var dataContainerDeletedWithoutRelations = DataContainer.CreateForExisting(DomainObjectIDs.Official1, null, pd => pd.DefaultValue);
      dataContainerDeletedWithoutRelations.Delete();
      var dataContainerDeletedWithRelations1 = DataContainer.CreateForExisting(DomainObjectIDs.Computer1, null, pd => pd.DefaultValue);
      dataContainerDeletedWithRelations1.Delete();
      var dataContainerDeletedWithRelations2 = DataContainer.CreateForExisting(DomainObjectIDs.Computer2, null, pd => pd.DefaultValue);
      dataContainerDeletedWithRelations2.Delete();

      var tableDefinitionA = (TableDefinition)dataContainerDeletedWithoutRelations.ClassDefinition.StorageEntityDefinition;
      var tableDefinitionB = (TableDefinition)dataContainerDeletedWithRelations1.ClassDefinition.StorageEntityDefinition;

      Assertion.DebugAssert((TableDefinition)dataContainerDeletedWithRelations2.ClassDefinition.StorageEntityDefinition == tableDefinitionB);

      var updateDbCommandBuilderDeleted2 = new Mock<IDbCommandBuilder>();
      var updateDbCommandBuilderDeleted3 = new Mock<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted1 = new Mock<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted2 = new Mock<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted3 = new Mock<IDbCommandBuilder>();

      var updateStatementParameters = new Queue<DataContainer>(new[] { dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2 });
      var updateStatementReturnValues = new Queue<IDbCommandBuilder>(new[] { updateDbCommandBuilderDeleted2.Object, updateDbCommandBuilderDeleted3.Object });
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForUpdate(tableDefinitionB, It.IsNotNull<IEnumerable<ColumnValue>>(), It.IsNotNull<IEnumerable<ColumnValue>>()))
          .Callback(
              (TableDefinition _, IEnumerable<ColumnValue> updatedColumns, IEnumerable<ColumnValue> comparedColumnValues) =>
              {
                var dataContainer = updateStatementParameters.Dequeue();
                CheckUpdatedComputerColumns(updatedColumns.ToArray(), dataContainer, expectEmployee: true, expectSerialNumber: false, expectClassID: false);
                CheckComparedColumns(comparedColumnValues.ToArray(), dataContainer, tableDefinitionB);
              })
          .Returns(()=> updateStatementReturnValues.Dequeue());

      var deleteStatementParameters = new Queue<DataContainer>(
          new[] { dataContainerDeletedWithoutRelations, dataContainerDeletedWithRelations1, dataContainerDeletedWithRelations2 });
      var deleteStatementReturnValues = new Queue<IDbCommandBuilder>(
          new[] { deleteDbCommandBuilderDeleted1.Object, deleteDbCommandBuilderDeleted2.Object, deleteDbCommandBuilderDeleted3.Object });
      _dbCommandBuilderFactoryStrictMock
          .Setup(
              stub => stub.CreateForDelete(
                  It.Is<TableDefinition>(p => p == deleteStatementParameters.Peek().ClassDefinition.StorageEntityDefinition),
                  It.IsNotNull<IEnumerable<ColumnValue>>()))
          .Callback(
              (TableDefinition _, IEnumerable<ColumnValue> comparedColumnValues) =>
              {
                var dataContainer = deleteStatementParameters.Dequeue();
                CheckComparedColumns(comparedColumnValues.ToArray(), dataContainer, (TableDefinition)dataContainer.ClassDefinition.StorageEntityDefinition);
              })
          .Returns(() => deleteStatementReturnValues.Dequeue());

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
      Assert.That(result, Is.TypeOf(typeof(MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand)result).Tuples.ToList();

      Assert.That(tuples.Count, Is.EqualTo(5));
      Assert.That(tuples[0].Item1, Is.EqualTo(dataContainerDeletedWithRelations1.ID));
      Assert.That(tuples[0].Item2, Is.SameAs(updateDbCommandBuilderDeleted2.Object));
      Assert.That(tuples[1].Item1, Is.EqualTo(dataContainerDeletedWithRelations2.ID));
      Assert.That(tuples[1].Item2, Is.SameAs(updateDbCommandBuilderDeleted3.Object));
      Assert.That(tuples[2].Item1, Is.EqualTo(dataContainerDeletedWithoutRelations.ID));
      Assert.That(tuples[2].Item2, Is.SameAs(deleteDbCommandBuilderDeleted1.Object));
      Assert.That(tuples[3].Item1, Is.EqualTo(dataContainerDeletedWithRelations1.ID));
      Assert.That(tuples[3].Item2, Is.SameAs(deleteDbCommandBuilderDeleted2.Object));
      Assert.That(tuples[4].Item1, Is.EqualTo(dataContainerDeletedWithRelations2.ID));
      Assert.That(tuples[4].Item2, Is.SameAs(deleteDbCommandBuilderDeleted3.Object));
    }

    [Test]
    public void CreateForSave_Unchanged ()
    {
      var dataContainerUnchanged = DataContainer.CreateForExisting(DomainObjectIDs.Order4, null, pd => pd.DefaultValue);

      StubTableDefinitionFinder(DomainObjectIDs.Order4, _tableDefinition1);

      var result = _factory.CreateForSave(new[] { dataContainerUnchanged });

      _tableDefinitionFinderStrictMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand)result).Tuples.ToList();

      Assert.That(tuples.Count, Is.EqualTo(0));
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
}
