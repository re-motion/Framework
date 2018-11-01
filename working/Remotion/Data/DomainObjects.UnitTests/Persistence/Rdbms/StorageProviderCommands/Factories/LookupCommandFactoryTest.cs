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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Persistence.StorageProviderCommands;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class LookupCommandFactoryTest : StandardMappingTest
  {
    private TableDefinitionFinder _tableDefinitionFinder;

    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private IObjectReaderFactory _objectReaderFactoryStrictMock;
    private IDbCommandBuilder _dbCommandBuilder1Stub;
    private IDbCommandBuilder _dbCommandBuilder2Stub;
    private IObjectReader<Tuple<ObjectID, object>> _timestampReader1Stub;
    private IObjectReader<Tuple<ObjectID, object>> _timestampReader2Stub;
    private IObjectReader<DataContainer> _dataContainerReader1Stub;
    private IObjectReader<DataContainer> _dataContainerReader2Stub;
    
    private LookupCommandFactory _factory;

    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;
    
    public override void SetUp ()
    {
      base.SetUp();

      _tableDefinitionFinder = new TableDefinitionFinder (new RdbmsPersistenceModelProvider());

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();
      _objectReaderFactoryStrictMock = MockRepository.GenerateStrictMock<IObjectReaderFactory>();
      _dbCommandBuilder1Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = MockRepository.GenerateStub<IDbCommandBuilder>();
      _timestampReader1Stub = MockRepository.GenerateStub<IObjectReader<Tuple<ObjectID, object>>> ();
      _timestampReader2Stub = MockRepository.GenerateStub<IObjectReader<Tuple<ObjectID, object>>> ();
      _dataContainerReader1Stub = MockRepository.GenerateStub<IObjectReader<DataContainer>> ();
      _dataContainerReader2Stub = MockRepository.GenerateStub<IObjectReader<DataContainer>> ();

      _factory = new LookupCommandFactory (
          TestDomainStorageProviderDefinition,
          _dbCommandBuilderFactoryStrictMock,
          _objectReaderFactoryStrictMock,
          _tableDefinitionFinder);

      _tableDefinition1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition);
      _tableDefinition2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition);

      _objectID1 = CreateObjectID (_tableDefinition1);
      _objectID2 = CreateObjectID (_tableDefinition1);
      _objectID3 = CreateObjectID (_tableDefinition2);
    }

    [Test]
    public void CreateForSingleIDLookup ()
    {
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns().ToArray();
      var expectedComparedColumns = 
          new[] { new ColumnValue (StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition1.ObjectIDProperty), _objectID1.Value) };

      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSelect (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg<IEnumerable<ColumnValue>>.List.Equal (expectedComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder1Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IRdbmsStorageEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForSingleIDLookup (_objectID1);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf<SingleDataContainerAssociateWithIDCommand<IRdbmsProviderCommandExecutionContext>>());
      
      var associateCommand = (SingleDataContainerAssociateWithIDCommand<IRdbmsProviderCommandExecutionContext>) result;
      Assert.That (associateCommand.ExpectedObjectID, Is.EqualTo (_objectID1));
      Assert.That (associateCommand.InnerCommand, Is.TypeOf (typeof (SingleObjectLoadCommand<DataContainer>)));

      var loadCommand = ((SingleObjectLoadCommand<DataContainer>) associateCommand.InnerCommand);
      Assert.That (loadCommand.DbCommandBuilder, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (loadCommand.ObjectReader, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_SingleIDLookup ()
    {
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns();
      var expectedComparedColumns = 
          new[] { new ColumnValue (StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition1.ObjectIDProperty), _objectID1.Value) };
      _dbCommandBuilderFactoryStrictMock
          .Stub (
              stub => stub.CreateForSelect (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg<IEnumerable<ColumnValue>>.List.Equal (expectedComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder1Stub);

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IRdbmsStorageEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForSortedMultiIDLookup (new[] { _objectID1 });

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerAssociateWithIDsCommand)));
      Assert.That (((MultiDataContainerAssociateWithIDsCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerAssociateWithIDsCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      Assert.That (dbCommandBuilderTuples[0].Item1, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (dbCommandBuilderTuples[0].Item2, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_TableDefinition_MultipleIDLookup_AndMultipleTables ()
    {
      var expectedSelectedColumns1 = _tableDefinition1.GetAllColumns();
      var expectedComparedColumns1 = new ColumnValueTable (
          new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition1.ObjectIDProperty) },
          new[]
          {
            new ColumnValueTable.Row (new[] { _objectID1.Value }), 
            new ColumnValueTable.Row (new[] { _objectID2.Value }), 
          });

      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForSelect (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns1),
                  Arg<ColumnValueTable>.Matches (t => ColumnValueTableTestHelper.AreEqual (expectedComparedColumns1, t)),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder1Stub);

      var expectedSelectedColumns2 = _tableDefinition2.GetAllColumns();
      var expectedComparedColumns2 = 
          new[] { new ColumnValue (StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition2.ObjectIDProperty), _objectID3.Value) };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForSelect (
                  Arg.Is (_tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (_tableDefinition2.GetAllColumns()),
                  Arg<IEnumerable<ColumnValue>>.List.Equal (expectedComparedColumns2),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder2Stub);

      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IRdbmsStorageEntityDefinition) _tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns1)))
          .Return (_dataContainerReader1Stub);
      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IRdbmsStorageEntityDefinition) _tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns2)))
          .Return (_dataContainerReader2Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForSortedMultiIDLookup (new[] { _objectID1, _objectID2, _objectID3 });

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf (typeof (MultiDataContainerAssociateWithIDsCommand)));
      Assert.That (((MultiDataContainerAssociateWithIDsCommand) result).Command, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>) ((MultiDataContainerAssociateWithIDsCommand) result).Command).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (2));

      // Convert to Dictionary because the order of tuples is not defined
      var dbCommandBuilderDictionary = dbCommandBuilderTuples.ToDictionary (tuple => tuple.Item1, tuple => tuple.Item2);

      Assert.That (dbCommandBuilderDictionary.ContainsKey (_dbCommandBuilder1Stub), Is.True);
      Assert.That (dbCommandBuilderDictionary[_dbCommandBuilder1Stub], Is.SameAs (_dataContainerReader1Stub));
      Assert.That (dbCommandBuilderDictionary.ContainsKey (_dbCommandBuilder2Stub), Is.True);
      Assert.That (dbCommandBuilderDictionary[_dbCommandBuilder2Stub], Is.SameAs (_dataContainerReader2Stub));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
        "Multi-ID lookups can only be performed for ObjectIDs from this storage provider.")]
    public void CreateForSortedMultiIDLookup_DifferentStorageProvider ()
    {
      _objectReaderFactoryStrictMock
          .Stub (mock => mock.CreateDataContainerReader (Arg<IRdbmsStorageEntityDefinition>.Is.Anything, Arg<IEnumerable<ColumnDefinition>>.Is.Anything))
          .Return (_dataContainerReader1Stub);

      _factory.CreateForSortedMultiIDLookup (new[] { DomainObjectIDs.Official1 });
    }

    [Test]
    public void CreateForMultiTimestampLookup ()
    {
      var expectedSelectedColumns1 =
          new[]
          {
              StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition1.ObjectIDProperty), 
              StoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition (_tableDefinition1.ObjectIDProperty), 
              StoragePropertyDefinitionTestHelper.GetSingleColumn (_tableDefinition1.TimestampProperty)
          };
      var expectedComparedColumns1 = new ColumnValueTable (
          new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition1.ObjectIDProperty) },
          new[]
          {
            new ColumnValueTable.Row (new[] { _objectID1.Value }), 
            new ColumnValueTable.Row (new[] { _objectID2.Value }), 
          });
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              mock => mock.CreateForSelect (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns1),
                  Arg<ColumnValueTable>.Matches (t => ColumnValueTableTestHelper.AreEqual (expectedComparedColumns1, t)),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder1Stub);

      var expectedSelectedColumns2 =
          new[]
          {
              StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition2.ObjectIDProperty),
              StoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition (_tableDefinition2.ObjectIDProperty),
              StoragePropertyDefinitionTestHelper.GetSingleColumn (_tableDefinition2.TimestampProperty)
          };
      var expectedComparedColumns2 =
          new[] { new ColumnValue (StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_tableDefinition2.ObjectIDProperty), _objectID3.Value) };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              mock => mock.CreateForSelect (
                  Arg.Is (_tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns2),
                  Arg<IEnumerable<ColumnValue>>.List.Equal (expectedComparedColumns2),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilder2Stub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateTimestampReader (
                  Arg.Is (_tableDefinition1),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns1)))
          .Return (_timestampReader1Stub);
      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateTimestampReader (
                  Arg.Is (_tableDefinition2),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns2)))
          .Return (_timestampReader2Stub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForMultiTimestampLookup (new[] { _objectID1, _objectID2, _objectID3 });

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<Tuple<ObjectID, object>>, IEnumerable<ObjectLookupResult<object>>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (MultiObjectLoadCommand<Tuple<ObjectID, object>>)));

      var commandBuildersAndReaders = ((MultiObjectLoadCommand<Tuple<ObjectID, object>>) innerCommand).DbCommandBuildersAndReaders;
      Assert.That (commandBuildersAndReaders.Length, Is.EqualTo (2));
      Assert.That (commandBuildersAndReaders[0].Item1, Is.SameAs (_dbCommandBuilder1Stub));
      Assert.That (commandBuildersAndReaders[0].Item2, Is.SameAs (_timestampReader1Stub));
      Assert.That (commandBuildersAndReaders[1].Item1, Is.SameAs (_dbCommandBuilder2Stub));
      Assert.That (commandBuildersAndReaders[1].Item2, Is.SameAs (_timestampReader2Stub));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
            "Multi-ID lookups can only be performed for ObjectIDs from this storage provider.")]
    public void CreateForMultiTimestampLookup_DifferentStorageProvider ()
    {
      _objectReaderFactoryStrictMock
          .Stub (mock => mock.CreateTimestampReader (Arg<IRdbmsStorageEntityDefinition>.Is.Anything, Arg<IEnumerable<ColumnDefinition>>.Is.Anything))
          .Return (_timestampReader1Stub);
      
      _factory.CreateForMultiTimestampLookup (new[] { DomainObjectIDs.Official1 });
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID(classDefinition, Guid.NewGuid());
    }

    private IStorageProviderCommand<TIn, IRdbmsProviderCommandExecutionContext> CheckDelegateBasedCommandAndReturnInnerCommand<TIn, TResult> (
        IStorageProviderCommand<TResult, IRdbmsProviderCommandExecutionContext> command)
    {
      Assert.That (
          command,
          Is.TypeOf (typeof (DelegateBasedCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>)));
      return ((DelegateBasedCommand<TIn, TResult, IRdbmsProviderCommandExecutionContext>) command).Command;
    }
  }
}