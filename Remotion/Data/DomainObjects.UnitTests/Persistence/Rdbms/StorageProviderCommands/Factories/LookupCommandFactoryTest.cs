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

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class LookupCommandFactoryTest : StandardMappingTest
  {
    private TableDefinitionFinder _tableDefinitionFinder;

    private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStrictMock;
    private Mock<IObjectReaderFactory> _objectReaderFactoryStrictMock;
    private Mock<IDbCommandBuilder> _dbCommandBuilder1Stub;
    private Mock<IDbCommandBuilder> _dbCommandBuilder2Stub;
    private Mock<IObjectReader<Tuple<ObjectID, object>>> _timestampReader1Stub;
    private Mock<IObjectReader<Tuple<ObjectID, object>>> _timestampReader2Stub;
    private Mock<IObjectReader<DataContainer>> _dataContainerReader1Stub;
    private Mock<IObjectReader<DataContainer>> _dataContainerReader2Stub;

    private LookupCommandFactory _factory;

    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectID _objectID3;

    public override void SetUp ()
    {
      base.SetUp();

      _tableDefinitionFinder = new TableDefinitionFinder(new RdbmsPersistenceModelProvider());

      _dbCommandBuilderFactoryStrictMock = new Mock<IDbCommandBuilderFactory>(MockBehavior.Strict);
      _objectReaderFactoryStrictMock = new Mock<IObjectReaderFactory>(MockBehavior.Strict);
      _dbCommandBuilder1Stub = new Mock<IDbCommandBuilder>();
      _dbCommandBuilder2Stub = new Mock<IDbCommandBuilder>();
      _timestampReader1Stub = new Mock<IObjectReader<Tuple<ObjectID, object>>>();
      _timestampReader2Stub = new Mock<IObjectReader<Tuple<ObjectID, object>>>();
      _dataContainerReader1Stub = new Mock<IObjectReader<DataContainer>>();
      _dataContainerReader2Stub = new Mock<IObjectReader<DataContainer>>();

      _factory = new LookupCommandFactory(
          TestDomainStorageProviderDefinition,
          _dbCommandBuilderFactoryStrictMock.Object,
          _objectReaderFactoryStrictMock.Object,
          _tableDefinitionFinder);

      _tableDefinition1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition);
      _tableDefinition2 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition);

      _objectID1 = CreateObjectID(_tableDefinition1);
      _objectID2 = CreateObjectID(_tableDefinition1);
      _objectID3 = CreateObjectID(_tableDefinition2);
    }

    [Test]
    public void CreateForSingleIDLookup ()
    {
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns().ToArray();
      var expectedComparedColumns =
          new[] { new ColumnValue(StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition1.ObjectIDProperty), _objectID1.Value) };

      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForSelect(_tableDefinition1, expectedSelectedColumns, expectedComparedColumns, new OrderedColumn[0]))
          .Returns(_dbCommandBuilder1Stub.Object);

      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateDataContainerReader(_tableDefinition1, expectedSelectedColumns))
          .Returns(_dataContainerReader1Stub.Object)
          .Verifiable();

      var result = _factory.CreateForSingleIDLookup(_objectID1);

      _objectReaderFactoryStrictMock.Verify();

      Assert.That(result, Is.TypeOf<SingleDataContainerAssociateWithIDCommand>());

      var associateCommand = (SingleDataContainerAssociateWithIDCommand)result;
      Assert.That(associateCommand.ExpectedObjectID, Is.EqualTo(_objectID1));
      Assert.That(associateCommand.InnerCommand, Is.TypeOf(typeof(SingleObjectLoadCommand<DataContainer>)));

      var loadCommand = ((SingleObjectLoadCommand<DataContainer>)associateCommand.InnerCommand);
      Assert.That(loadCommand.DbCommandBuilder, Is.SameAs(_dbCommandBuilder1Stub.Object));
      Assert.That(loadCommand.ObjectReader, Is.SameAs(_dataContainerReader1Stub.Object));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_SingleIDLookup ()
    {
      var expectedSelectedColumns = _tableDefinition1.GetAllColumns();
      var expectedComparedColumns =
          new[] { new ColumnValue(StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition1.ObjectIDProperty), _objectID1.Value) };
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForSelect(_tableDefinition1, expectedSelectedColumns, expectedComparedColumns, new OrderedColumn[0]))
          .Returns(_dbCommandBuilder1Stub.Object);

      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateDataContainerReader(_tableDefinition1, expectedSelectedColumns))
          .Returns(_dataContainerReader1Stub.Object)
          .Verifiable();

      var result = _factory.CreateForSortedMultiIDLookup(new[] { _objectID1 });

      _objectReaderFactoryStrictMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(MultiDataContainerAssociateWithIDsCommand)));
      Assert.That(((MultiDataContainerAssociateWithIDsCommand)result).Command, Is.TypeOf(typeof(MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>)((MultiDataContainerAssociateWithIDsCommand)result).Command).DbCommandBuildersAndReaders;
      Assert.That(dbCommandBuilderTuples.Length, Is.EqualTo(1));
      Assert.That(dbCommandBuilderTuples[0].Item1, Is.SameAs(_dbCommandBuilder1Stub.Object));
      Assert.That(dbCommandBuilderTuples[0].Item2, Is.SameAs(_dataContainerReader1Stub.Object));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_TableDefinition_MultipleIDLookup_AndMultipleTables ()
    {
      var expectedSelectedColumns1 = _tableDefinition1.GetAllColumns();
      var expectedComparedColumns1 = new ColumnValueTable(
          new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition1.ObjectIDProperty) },
          new[]
          {
            new ColumnValueTable.Row(new[] { _objectID1.Value }),
            new ColumnValueTable.Row(new[] { _objectID2.Value }),
          });
      _dbCommandBuilderFactoryStrictMock
          .Setup(
              stub => stub.CreateForSelect(
                  _tableDefinition1,
                  expectedSelectedColumns1,
                  It.Is<ColumnValueTable>(t => ColumnValueTableTestHelper.AreEqual(expectedComparedColumns1, t)),
                  new OrderedColumn[0]))
          .Returns(_dbCommandBuilder1Stub.Object)
          .Verifiable();

      var expectedSelectedColumns2 = _tableDefinition2.GetAllColumns();
      var expectedComparedColumns2 = new[] { new ColumnValue(StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition2.ObjectIDProperty), _objectID3.Value) };
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForSelect(_tableDefinition2, expectedSelectedColumns2, expectedComparedColumns2, new OrderedColumn[0]))
          .Returns(_dbCommandBuilder2Stub.Object)
          .Verifiable();

      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateDataContainerReader(_tableDefinition1, expectedSelectedColumns1))
          .Returns(_dataContainerReader1Stub.Object)
          .Verifiable();
      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateDataContainerReader(_tableDefinition2, expectedSelectedColumns2))
          .Returns(_dataContainerReader2Stub.Object)
          .Verifiable();

      var result = _factory.CreateForSortedMultiIDLookup(new[] { _objectID1, _objectID2, _objectID3 });

      _objectReaderFactoryStrictMock.Verify();
      _dbCommandBuilderFactoryStrictMock.Verify();
      Assert.That(result, Is.TypeOf(typeof(MultiDataContainerAssociateWithIDsCommand)));
      Assert.That(((MultiDataContainerAssociateWithIDsCommand)result).Command, Is.TypeOf(typeof(MultiObjectLoadCommand<DataContainer>)));

      var dbCommandBuilderTuples =
          ((MultiObjectLoadCommand<DataContainer>)((MultiDataContainerAssociateWithIDsCommand)result).Command).DbCommandBuildersAndReaders;
      Assert.That(dbCommandBuilderTuples.Length, Is.EqualTo(2));

      // Convert to Dictionary because the order of tuples is not defined
      var dbCommandBuilderDictionary = dbCommandBuilderTuples.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

      Assert.That(dbCommandBuilderDictionary.ContainsKey(_dbCommandBuilder1Stub.Object), Is.True);
      Assert.That(dbCommandBuilderDictionary[_dbCommandBuilder1Stub.Object], Is.SameAs(_dataContainerReader1Stub.Object));
      Assert.That(dbCommandBuilderDictionary.ContainsKey(_dbCommandBuilder2Stub.Object), Is.True);
      Assert.That(dbCommandBuilderDictionary[_dbCommandBuilder2Stub.Object], Is.SameAs(_dataContainerReader2Stub.Object));
    }

    [Test]
    public void CreateForSortedMultiIDLookup_DifferentStorageProvider ()
    {
      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateDataContainerReader(It.IsAny<IRdbmsStorageEntityDefinition>(), It.IsAny<IEnumerable<ColumnDefinition>>()))
          .Returns(_dataContainerReader1Stub.Object);
      Assert.That(
          () => _factory.CreateForSortedMultiIDLookup(new[] { DomainObjectIDs.Official1 }),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Multi-ID lookups can only be performed for ObjectIDs from this storage provider."));
    }

    [Test]
    public void CreateForMultiTimestampLookup ()
    {
      var expectedSelectedColumns1 =
          new[]
          {
              StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition1.ObjectIDProperty),
              StoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition(_tableDefinition1.ObjectIDProperty),
              StoragePropertyDefinitionTestHelper.GetSingleColumn(_tableDefinition1.TimestampProperty)
          };
      var expectedComparedColumns1 = new ColumnValueTable(
          new[] { StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition1.ObjectIDProperty) },
          new[]
          {
            new ColumnValueTable.Row(new[] { _objectID1.Value }),
            new ColumnValueTable.Row(new[] { _objectID2.Value }),
          });
      _dbCommandBuilderFactoryStrictMock
          .Setup(
              mock => mock.CreateForSelect(
                  _tableDefinition1,
                  expectedSelectedColumns1,
                  It.Is<ColumnValueTable>(t => ColumnValueTableTestHelper.AreEqual(expectedComparedColumns1, t)),
                  new OrderedColumn[0]))
          .Returns(_dbCommandBuilder1Stub.Object)
          .Verifiable();

      var expectedSelectedColumns2 =
          new[]
          {
              StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition2.ObjectIDProperty),
              StoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition(_tableDefinition2.ObjectIDProperty),
              StoragePropertyDefinitionTestHelper.GetSingleColumn(_tableDefinition2.TimestampProperty)
          };
      var expectedComparedColumns2 =
          new[] { new ColumnValue(StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_tableDefinition2.ObjectIDProperty), _objectID3.Value) };
      _dbCommandBuilderFactoryStrictMock
          .Setup(mock => mock.CreateForSelect(_tableDefinition2, expectedSelectedColumns2, expectedComparedColumns2, new OrderedColumn[0]))
          .Returns(_dbCommandBuilder2Stub.Object)
          .Verifiable();

      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateTimestampReader(_tableDefinition1, expectedSelectedColumns1))
          .Returns(_timestampReader1Stub.Object)
          .Verifiable();
      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateTimestampReader(_tableDefinition2, expectedSelectedColumns2))
          .Returns(_timestampReader2Stub.Object)
          .Verifiable();

      var result = _factory.CreateForMultiTimestampLookup(new[] { _objectID1, _objectID2, _objectID3 });

      _objectReaderFactoryStrictMock.Verify();
      _dbCommandBuilderFactoryStrictMock.Verify();

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<Tuple<ObjectID, object>>, IEnumerable<ObjectLookupResult<object>>>(result);
      Assert.That(innerCommand, Is.TypeOf(typeof(MultiObjectLoadCommand<Tuple<ObjectID, object>>)));

      var commandBuildersAndReaders = ((MultiObjectLoadCommand<Tuple<ObjectID, object>>)innerCommand).DbCommandBuildersAndReaders;
      Assert.That(commandBuildersAndReaders.Length, Is.EqualTo(2));
      Assert.That(commandBuildersAndReaders[0].Item1, Is.SameAs(_dbCommandBuilder1Stub.Object));
      Assert.That(commandBuildersAndReaders[0].Item2, Is.SameAs(_timestampReader1Stub.Object));
      Assert.That(commandBuildersAndReaders[1].Item1, Is.SameAs(_dbCommandBuilder2Stub.Object));
      Assert.That(commandBuildersAndReaders[1].Item2, Is.SameAs(_timestampReader2Stub.Object));
    }

    [Test]
    public void CreateForMultiTimestampLookup_DifferentStorageProvider ()
    {
      _objectReaderFactoryStrictMock
          .Setup(mock => mock.CreateTimestampReader(It.IsAny<IRdbmsStorageEntityDefinition>(), It.IsAny<IEnumerable<ColumnDefinition>>()))
          .Returns(_timestampReader1Stub.Object);
      Assert.That(
          () => _factory.CreateForMultiTimestampLookup(new[] { DomainObjectIDs.Official1 }),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Multi-ID lookups can only be performed for ObjectIDs from this storage provider."));
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);
      classDefinition.SetStorageEntity(entityDefinition);

      return new ObjectID(classDefinition, Guid.NewGuid());
    }

    private IRdbmsProviderCommand<TIn> CheckDelegateBasedCommandAndReturnInnerCommand<TIn, TResult> (
        IRdbmsProviderCommand<TResult> command)
    {
      Assert.That(
          command,
          Is.TypeOf(typeof(DelegateBasedCommand<TIn, TResult>)));
      return ((DelegateBasedCommand<TIn, TResult>)command).Command;
    }
  }
}
