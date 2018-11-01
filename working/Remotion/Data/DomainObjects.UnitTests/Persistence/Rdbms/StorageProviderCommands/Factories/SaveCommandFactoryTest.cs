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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class SaveCommandFactoryTest : StandardMappingTest
  {
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;
    private ITableDefinitionFinder _tableDefinitionFinderStrictMock;
    private SaveCommandFactory _factory;
    private TableDefinition _tableDefinition1;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();
      _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();
      _tableDefinitionFinderStrictMock = MockRepository.GenerateStrictMock<ITableDefinitionFinder>();

      _factory = new SaveCommandFactory (
          _dbCommandBuilderFactoryStrictMock,
          _rdbmsPersistenceModelProvider,
          _tableDefinitionFinderStrictMock);

      _tableDefinition1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
    }

    [Test]
    public void CreateForSave_New ()
    {
      var dataContainerNew1 = DataContainer.CreateNew (DomainObjectIDs.Computer1);
      SetPropertyValue (dataContainerNew1, typeof (Computer), "SerialNumber", "123456");
      var dataContainerNew2 = DataContainer.CreateNew (DomainObjectIDs.Computer2);
      SetPropertyValue (dataContainerNew2, typeof (Computer), "SerialNumber", "654321");

      var dataContainerNewWithoutRelations = DataContainer.CreateNew (DomainObjectIDs.Official1);

      var insertDbCommandBuilderNew1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var insertDbCommandBuilderNew2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var insertDbCommandBuilderNew3 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderNew1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderNew2 = MockRepository.GenerateStub<IDbCommandBuilder>();

      var tableDefinition1 = (TableDefinition) dataContainerNew1.ID.ClassDefinition.StorageEntityDefinition;
      var tableDefinition2 = (TableDefinition) dataContainerNew2.ID.ClassDefinition.StorageEntityDefinition;
      var tableDefinition3 = (TableDefinition) dataContainerNewWithoutRelations.ID.ClassDefinition.StorageEntityDefinition;

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub =>
              stub.CreateForInsert (
                  Arg.Is (tableDefinition1),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckInsertedComputerColumns (c, dataContainerNew1))))
          .Return (insertDbCommandBuilderNew1).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub =>
              stub.CreateForInsert (
                  Arg.Is (tableDefinition2),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckInsertedComputerColumns (c, dataContainerNew2))))
          .Return (insertDbCommandBuilderNew2).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub =>
              stub.CreateForInsert (
                  Arg.Is (tableDefinition3),
                  Arg<IEnumerable<ColumnValue>>.Is.Anything))
          .Return (insertDbCommandBuilderNew3).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinition1),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckUpdatedComputerColumns (c, dataContainerNew1, true, false, false)),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerNew1, tableDefinition1))))
          .Return (updateDbCommandBuilderNew1).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinition2),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckUpdatedComputerColumns (c, dataContainerNew2, true, false, false)),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerNew2, tableDefinition2))))
          .Return (updateDbCommandBuilderNew2).Repeat.Once();

      StubTableDefinitionFinder (dataContainerNew1.ID, tableDefinition1);
      StubTableDefinitionFinder (dataContainerNew2.ID, tableDefinition2);
      StubTableDefinitionFinder (dataContainerNewWithoutRelations.ID, tableDefinition3);
      _tableDefinitionFinderStrictMock.Replay();

      var result = _factory.CreateForSave (
          new[]
          {
              dataContainerNew1,
              dataContainerNew2,
              dataContainerNewWithoutRelations
          });

      _tableDefinitionFinderStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand) result).Tuples.ToList();

      Assert.That (tuples.Count, Is.EqualTo (5));
      Assert.That (tuples[0].Item1, Is.EqualTo (dataContainerNew1.ID));
      Assert.That (tuples[0].Item2, Is.SameAs (insertDbCommandBuilderNew1));
      Assert.That (tuples[1].Item1, Is.EqualTo (dataContainerNew2.ID));
      Assert.That (tuples[1].Item2, Is.SameAs (insertDbCommandBuilderNew2));
      Assert.That (tuples[2].Item1, Is.EqualTo (dataContainerNewWithoutRelations.ID));
      Assert.That (tuples[2].Item2, Is.SameAs (insertDbCommandBuilderNew3));
      Assert.That (tuples[3].Item1, Is.EqualTo (dataContainerNew1.ID));
      Assert.That (tuples[3].Item2, Is.SameAs (updateDbCommandBuilderNew1));
      Assert.That (tuples[4].Item1, Is.EqualTo (dataContainerNew2.ID));
      Assert.That (tuples[4].Item2, Is.SameAs (updateDbCommandBuilderNew2));
    }

    [Test]
    public void CreateForSave_Changed ()
    {
      var dataContainerChangedSerialNumber = DataContainer.CreateForExisting (DomainObjectIDs.Computer1, null, pd => pd.DefaultValue);
      SetPropertyValue (dataContainerChangedSerialNumber, typeof (Computer), "SerialNumber", "123456");
      var dataContainerChangedEmployee = DataContainer.CreateForExisting (DomainObjectIDs.Computer2, null, pd => pd.DefaultValue);
      SetPropertyValue (dataContainerChangedEmployee, typeof (Computer), "Employee", DomainObjectIDs.Employee2);
      var dataContainerChangedMarkedAsChanged = DataContainer.CreateForExisting (DomainObjectIDs.Computer3, null, pd => pd.DefaultValue);
      dataContainerChangedMarkedAsChanged.MarkAsChanged();

      var updateDbCommandBuilderChangedSerialNumber = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderChangedEmployee = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderMarkedAsChanged = MockRepository.GenerateStub<IDbCommandBuilder>();

      var tableDefinitionChangedSerialNumber = (TableDefinition) dataContainerChangedSerialNumber.ID.ClassDefinition.StorageEntityDefinition;
      var tableDefinitionChangedEmployee = (TableDefinition) dataContainerChangedEmployee.ID.ClassDefinition.StorageEntityDefinition;
      var tableDefinitionMarkedAsChanged = (TableDefinition) dataContainerChangedMarkedAsChanged.ID.ClassDefinition.StorageEntityDefinition;

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinitionChangedSerialNumber),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckUpdatedComputerColumns (c, dataContainerChangedSerialNumber, false, true, false)),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckComparedColumns (c, dataContainerChangedSerialNumber, tableDefinitionChangedSerialNumber))))
          .Return (updateDbCommandBuilderChangedSerialNumber).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinitionChangedEmployee),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckUpdatedComputerColumns (c, dataContainerChangedEmployee, true, false, false)),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckComparedColumns (c, dataContainerChangedEmployee, tableDefinitionChangedSerialNumber))))
          .Return (updateDbCommandBuilderChangedEmployee).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinitionMarkedAsChanged),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckUpdatedComputerColumns (c, dataContainerChangedMarkedAsChanged, false, false, true)),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckComparedColumns (c, dataContainerChangedMarkedAsChanged, tableDefinitionMarkedAsChanged))))
          .Return (updateDbCommandBuilderMarkedAsChanged).Repeat.Once();

      StubTableDefinitionFinder (dataContainerChangedSerialNumber.ID, tableDefinitionChangedSerialNumber);
      StubTableDefinitionFinder (dataContainerChangedEmployee.ID, tableDefinitionChangedSerialNumber);
      StubTableDefinitionFinder (dataContainerChangedMarkedAsChanged.ID, tableDefinitionMarkedAsChanged);
      _tableDefinitionFinderStrictMock.Replay();

      var result =
          _factory.CreateForSave (new[] { dataContainerChangedSerialNumber, dataContainerChangedEmployee, dataContainerChangedMarkedAsChanged });

      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand) result).Tuples.ToList();

      _tableDefinitionFinderStrictMock.VerifyAllExpectations();
      Assert.That (tuples.Count, Is.EqualTo (3));
      Assert.That (tuples[0].Item1, Is.EqualTo (dataContainerChangedSerialNumber.ID));
      Assert.That (tuples[0].Item2, Is.SameAs (updateDbCommandBuilderChangedSerialNumber));
      Assert.That (tuples[1].Item1, Is.EqualTo (dataContainerChangedEmployee.ID));
      Assert.That (tuples[1].Item2, Is.SameAs (updateDbCommandBuilderChangedEmployee));
      Assert.That (tuples[2].Item1, Is.EqualTo (dataContainerChangedMarkedAsChanged.ID));
      Assert.That (tuples[2].Item2, Is.SameAs (updateDbCommandBuilderMarkedAsChanged));
    }

    [Test]
    public void CreateForSave_Deleted ()
    {
      var dataContainerDeletedWithoutRelations = DataContainer.CreateForExisting (DomainObjectIDs.Official1, null, pd => pd.DefaultValue);
      dataContainerDeletedWithoutRelations.Delete();
      var dataContainerDeletedWithRelations1 = DataContainer.CreateForExisting (DomainObjectIDs.Computer1, null, pd => pd.DefaultValue);
      dataContainerDeletedWithRelations1.Delete();
      var dataContainerDeletedWithRelations2 = DataContainer.CreateForExisting (DomainObjectIDs.Computer2, null, pd => pd.DefaultValue);
      dataContainerDeletedWithRelations2.Delete();

      var tableDefinition1 = (TableDefinition) dataContainerDeletedWithoutRelations.ID.ClassDefinition.StorageEntityDefinition;
      var tableDefinition2 = (TableDefinition) dataContainerDeletedWithRelations1.ID.ClassDefinition.StorageEntityDefinition;
      var tableDefinition3 = (TableDefinition) dataContainerDeletedWithRelations2.ID.ClassDefinition.StorageEntityDefinition;

      var updateDbCommandBuilderDeleted2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var updateDbCommandBuilderDeleted3 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted1 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted2 = MockRepository.GenerateStub<IDbCommandBuilder>();
      var deleteDbCommandBuilderDeleted3 = MockRepository.GenerateStub<IDbCommandBuilder>();


      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinition2),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckUpdatedComputerColumns (c, dataContainerDeletedWithRelations1, true, false, false)),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerDeletedWithRelations1, tableDefinition2))))
          .Return (updateDbCommandBuilderDeleted2).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForUpdate (
                  Arg.Is (tableDefinition3),
                  Arg<IEnumerable<ColumnValue>>.Matches (
                      c => CheckUpdatedComputerColumns (c, dataContainerDeletedWithRelations2, true, false, false)),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerDeletedWithRelations2, tableDefinition3))))
          .Return (updateDbCommandBuilderDeleted3).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForDelete (
                  Arg.Is (tableDefinition1),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerDeletedWithoutRelations, tableDefinition1))))
          .Return (deleteDbCommandBuilderDeleted1).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForDelete (
                  Arg.Is (tableDefinition2),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerDeletedWithRelations1, tableDefinition2))))
          .Return (deleteDbCommandBuilderDeleted2).Repeat.Once();
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForDelete (
                  Arg.Is (tableDefinition3),
                  Arg<IEnumerable<ColumnValue>>.Matches (c => CheckComparedColumns (c, dataContainerDeletedWithRelations2, tableDefinition3))))
          .Return (deleteDbCommandBuilderDeleted3).Repeat.Once();


      StubTableDefinitionFinder (dataContainerDeletedWithoutRelations.ID, tableDefinition1);
      StubTableDefinitionFinder (dataContainerDeletedWithRelations1.ID, tableDefinition2);
      StubTableDefinitionFinder (dataContainerDeletedWithRelations2.ID, tableDefinition3);
      _tableDefinitionFinderStrictMock.Replay();

      var result = _factory.CreateForSave (
          new[]
          {
              dataContainerDeletedWithoutRelations,
              dataContainerDeletedWithRelations1,
              dataContainerDeletedWithRelations2
          });

      _tableDefinitionFinderStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand) result).Tuples.ToList();

      Assert.That (tuples.Count, Is.EqualTo (5));
      Assert.That (tuples[0].Item1, Is.EqualTo (dataContainerDeletedWithRelations1.ID));
      Assert.That (tuples[0].Item2, Is.SameAs (updateDbCommandBuilderDeleted2));
      Assert.That (tuples[1].Item1, Is.EqualTo (dataContainerDeletedWithRelations2.ID));
      Assert.That (tuples[1].Item2, Is.SameAs (updateDbCommandBuilderDeleted3));
      Assert.That (tuples[2].Item1, Is.EqualTo (dataContainerDeletedWithoutRelations.ID));
      Assert.That (tuples[2].Item2, Is.SameAs (deleteDbCommandBuilderDeleted1));
      Assert.That (tuples[3].Item1, Is.EqualTo (dataContainerDeletedWithRelations1.ID));
      Assert.That (tuples[3].Item2, Is.SameAs (deleteDbCommandBuilderDeleted2));
      Assert.That (tuples[4].Item1, Is.EqualTo (dataContainerDeletedWithRelations2.ID));
      Assert.That (tuples[4].Item2, Is.SameAs (deleteDbCommandBuilderDeleted3));
    }

    [Test]
    public void CreateForSave_Unchanged ()
    {
      var dataContainerUnchanged = DataContainer.CreateForExisting (DomainObjectIDs.Order4, null, pd => pd.DefaultValue);

      StubTableDefinitionFinder (DomainObjectIDs.Order4, _tableDefinition1);
      _tableDefinitionFinderStrictMock.Replay();

      var result = _factory.CreateForSave (new[] { dataContainerUnchanged });

      _tableDefinitionFinderStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerSaveCommand)));
      var tuples = ((MultiDataContainerSaveCommand) result).Tuples.ToList();

      Assert.That (tuples.Count, Is.EqualTo (0));
    }

    private void StubTableDefinitionFinder (ObjectID objectID, TableDefinition tableDefinition)
    {
      _tableDefinitionFinderStrictMock.Expect (mock => mock.GetTableDefinition (objectID)).Return (tableDefinition);
    }

    private bool CheckComparedColumns (
        IEnumerable<ColumnValue> columnValues, DataContainer dataContainer, TableDefinition tableDefinition)
    {
      var comparedColumnValues = columnValues.ToArray();

      Assert.That (comparedColumnValues[0].Column, 
          Is.SameAs (StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (tableDefinition.ObjectIDProperty)));
      Assert.That (comparedColumnValues[0].Value, Is.SameAs (dataContainer.ID.Value));
      if (dataContainer.ClassDefinition.GetPropertyDefinitions().All (propertyDefinition => !propertyDefinition.IsObjectID))
      {
        Assert.That (comparedColumnValues[1].Column, Is.SameAs (StoragePropertyDefinitionTestHelper.GetSingleColumn (tableDefinition.TimestampProperty)));
        Assert.That (comparedColumnValues[1].Value, Is.SameAs (dataContainer.Timestamp));
      }
      return true;
    }
    
    private bool CheckUpdatedComputerColumns (
        IEnumerable<ColumnValue> columnValues,
        DataContainer dataContainer,
        bool expectEmployee,
        bool expectSerialNumber,
        bool expectClassID)
    {
      CheckColumnValue (
          "SerialNumber", expectSerialNumber, columnValues, GetPropertyValue (dataContainer, typeof (Computer), "SerialNumber"));
      CheckColumnValue (
          "EmployeeID", expectEmployee, columnValues, GetObjectIDValue (dataContainer, typeof (Computer), "Employee"));
      CheckColumnValue ("ClassID", expectClassID, columnValues, dataContainer.ID.ClassID);

      var expectedColumnCount =
          (expectEmployee ? 1 : 0)
          + (expectSerialNumber ? 1 : 0)
          + (expectClassID ? 1 : 0);
      Assert.That (columnValues.Count(), Is.EqualTo (expectedColumnCount));

      return true;
    }

    private bool CheckInsertedComputerColumns (IEnumerable<ColumnValue> columnValues, DataContainer dataContainer)
    {
      CheckColumnValue ("ID", true, columnValues, dataContainer.ID.Value);
      CheckColumnValue ("ClassID", true, columnValues, dataContainer.ID.ClassID);
      CheckColumnValue ("SerialNumber", true, columnValues, GetPropertyValue (dataContainer, typeof (Computer), "SerialNumber"));
      CheckColumnValue ("EmployeeID", false, columnValues, GetObjectIDValue (dataContainer, typeof (Computer), "Employee"));

      Assert.That (columnValues.Count(), Is.EqualTo (3));

      return true;
    }

    private object GetObjectIDValue (DataContainer dataContainer, Type declaringType, string shortPropertyName)
    {
      var objectID = (ObjectID) GetPropertyValue (dataContainer, declaringType, shortPropertyName);
      return objectID != null ? objectID.Value : null;
    }

    private void CheckColumnValue (
        string columnName,
        bool shouldBeIncluded,
        IEnumerable<ColumnValue> columnValues,
        object expectedValue)
    {
      var column = columnValues.FirstOrDefault (cv => cv.Column.Name == columnName);
      if (column.Column != null)
      {
        Assert.That (shouldBeIncluded, Is.True, "Column '{0}' was found, but not expected.", columnName);
        Assert.That (column.Value, Is.EqualTo (expectedValue));
      }
      else
        Assert.That (shouldBeIncluded, Is.False, "Column '{0}' was expected, but not found.", columnName);
    }
  }
}