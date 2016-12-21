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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Persistence.StorageProviderCommands;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class RelationLookupCommandFactoryTest : StandardMappingTest
  {
    private RdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private IObjectReaderFactory _objectReaderFactoryStrictMock;
    private IDbCommandBuilder _dbCommandBuilderStub;
    private IObjectReader<DataContainer> _dataContainerReaderStub;
    private IObjectReader<ObjectID> _objectIDReaderStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _fakeStorageProviderCommandFactory;

    private RelationLookupCommandFactory _factory;
    
    private TableDefinition _tableDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private ObjectID _foreignKeyValue;
    private IRdbmsStoragePropertyDefinition _foreignKeyStoragePropertyDefinitionStrictMock;

    private ColumnValue[] _fakeComparedColumns;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider();

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();
      _objectReaderFactoryStrictMock = MockRepository.GenerateStrictMock<IObjectReaderFactory>();
      _dbCommandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder> ();
      _dataContainerReaderStub = MockRepository.GenerateStub<IObjectReader<DataContainer>> ();
      _objectIDReaderStub = MockRepository.GenerateStub<IObjectReader<ObjectID>> ();
      _fakeStorageProviderCommandFactory = MockRepository.GenerateStub<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();

      _factory = new RelationLookupCommandFactory (
                _fakeStorageProviderCommandFactory,
                _dbCommandBuilderFactoryStrictMock,
                _rdbmsPersistenceModelProvider,
                _objectReaderFactoryStrictMock);

      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition);

      _foreignKeyValue = CreateObjectID (_tableDefinition);
      _foreignKeyStoragePropertyDefinitionStrictMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition>();

      _fakeComparedColumns = new[] { new ColumnValue (ColumnDefinitionObjectMother.IDColumn, _foreignKeyValue.Value) };
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_NoSortExpression ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithTable (classType: typeof (Order), storageProviderDefinition: TestDomainStorageProviderDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);
      var oppositeTable = (TableDefinition) relationEndPointDefinition.ClassDefinition.StorageEntityDefinition;

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = _tableDefinition.GetAllColumns ();
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForSelect (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IRdbmsStorageEntityDefinition) oppositeTable),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var dbCommandBuilderTuples = ((MultiObjectLoadCommand<DataContainer>) result).DbCommandBuildersAndReaders;
      Assert.That (dbCommandBuilderTuples.Length, Is.EqualTo (1));
      Assert.That (dbCommandBuilderTuples[0].Item1, Is.SameAs (_dbCommandBuilderStub));
      Assert.That (dbCommandBuilderTuples[0].Item2, Is.SameAs (_dataContainerReaderStub));
    }

    [Test]
    public void CreateForRelationLookup_TableDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_tableDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var spec1 = CreateSortedPropertySpecification (
          classDefinition,
          SortOrder.Descending,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn);
      var spec2 = CreateSortedPropertySpecification (classDefinition, SortOrder.Ascending, ColumnDefinitionObjectMother.TimestampColumn);

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = _tableDefinition.GetAllColumns();
      var expectedOrderedColumns = new[]
                                   {
                                       new OrderedColumn (ColumnDefinitionObjectMother.IDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.ClassIDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.TimestampColumn, SortOrder.Ascending)
                                   };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForSelect (
                  Arg.Is ((TableDefinition) classDefinition.StorageEntityDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (expectedOrderedColumns)))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateDataContainerReader (
                  Arg.Is ((IRdbmsStorageEntityDefinition) _tableDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_dataContainerReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_NoSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = _unionViewDefinition.ObjectIDProperty.GetColumns().ToArray();
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForSelect (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (new OrderedColumn[0])))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateObjectIDReader (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_objectIDReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      _objectReaderFactoryStrictMock.VerifyAllExpectations ();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();

      var innerCommand =
          CheckDelegateBasedCommandAndReturnInnerCommand<IEnumerable<ObjectLookupResult<DataContainer>>, IEnumerable<DataContainer>> (result);
      Assert.That (innerCommand, Is.TypeOf (typeof (IndirectDataContainerLoadCommand)));
      var indirectLoadCommand = (IndirectDataContainerLoadCommand) innerCommand;
      Assert.That (indirectLoadCommand.StorageProviderCommandFactory, Is.SameAs (_fakeStorageProviderCommandFactory));
      Assert.That (indirectLoadCommand.ObjectIDLoadCommand, Is.TypeOf (typeof (MultiObjectIDLoadCommand)));
      Assert.That (((MultiObjectIDLoadCommand) (indirectLoadCommand.ObjectIDLoadCommand)).DbCommandBuilders, Is.EqualTo (new[] { _dbCommandBuilderStub }));
      Assert.That (((MultiObjectIDLoadCommand) indirectLoadCommand.ObjectIDLoadCommand).ObjectIDReader, Is.SameAs (_objectIDReaderStub));
    }

    [Test]
    public void CreateForRelationLookup_UnionViewDefinition_WithSortExpression ()
    {
      var classDefinition = CreateClassDefinition (_unionViewDefinition);
      var relationEndPointDefinition = CreateForeignKeyEndPointDefinition (classDefinition);

      var spec1 = CreateSortedPropertySpecification (
          classDefinition,
          SortOrder.Descending,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn);
      var spec2 = CreateSortedPropertySpecification (classDefinition, SortOrder.Ascending, ColumnDefinitionObjectMother.TimestampColumn);

      _foreignKeyStoragePropertyDefinitionStrictMock.Expect (mock => mock.SplitValueForComparison (_foreignKeyValue)).Return (_fakeComparedColumns);
      _foreignKeyStoragePropertyDefinitionStrictMock.Replay ();

      var expectedSelectedColumns = _unionViewDefinition.ObjectIDProperty.GetColumns ().ToArray ();
      var expectedOrderedColumns = new[]
                                   {
                                       new OrderedColumn (ColumnDefinitionObjectMother.IDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.ClassIDColumn, SortOrder.Descending),
                                       new OrderedColumn (ColumnDefinitionObjectMother.TimestampColumn, SortOrder.Ascending)
                                   };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForSelect (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns),
                  Arg.Is (_fakeComparedColumns),
                  Arg<IEnumerable<OrderedColumn>>.List.Equal (expectedOrderedColumns)))
          .Return (_dbCommandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay();

      _objectReaderFactoryStrictMock
          .Expect (
              mock => mock.CreateObjectIDReader (
                  Arg.Is (_unionViewDefinition),
                  Arg<IEnumerable<ColumnDefinition>>.List.Equal (expectedSelectedColumns)))
          .Return (_objectIDReaderStub);
      _objectReaderFactoryStrictMock.Replay();

      _factory.CreateForRelationLookup (
          relationEndPointDefinition,
          _foreignKeyValue,
          new SortExpressionDefinition (new[] { spec1, spec2 }));

      _objectReaderFactoryStrictMock.VerifyAllExpectations();
      _foreignKeyStoragePropertyDefinitionStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateForRelationLookup_EmptyViewDefinition ()
    {
      var emptyViewDefintion = EmptyViewDefinitionObjectMother.Create (TestDomainStorageProviderDefinition);
      var classDefinition = CreateClassDefinition (emptyViewDefintion);
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (classDefinition);
      var relationEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var result = _factory.CreateForRelationLookup (relationEndPointDefinition, _foreignKeyValue, null);

      Assert.That (result, Is.TypeOf (typeof (FixedValueCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>)));
      var fixedValueCommand = (FixedValueCommand<IEnumerable<DataContainer>, IRdbmsProviderCommandExecutionContext>) result;
      Assert.That (fixedValueCommand.Value, Is.EqualTo (Enumerable.Empty<DataContainer>()));
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

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition,
        SortOrder sortOrder,
        ColumnDefinition sortedColumn)
    {
      return CreateSortedPropertySpecification (
          classDefinition,
          new SimpleStoragePropertyDefinition (typeof (int), sortedColumn),
          sortOrder);
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition,
        SortOrder sortOrder,
        ColumnDefinition sortedColumn1,
        ColumnDefinition sortedColumn2)
    {
      return CreateSortedPropertySpecification (
          classDefinition,
          new ObjectIDStoragePropertyDefinition (
              new SimpleStoragePropertyDefinition (typeof (int), sortedColumn1), new SimpleStoragePropertyDefinition (typeof (int), sortedColumn2)),
          sortOrder);
    }

    private RelationEndPointDefinition CreateForeignKeyEndPointDefinition (ClassDefinition classDefinition)
    {
      var idPropertyDefinition = CreateForeignKeyPropertyDefinition (classDefinition);
      return new RelationEndPointDefinition (idPropertyDefinition, false);
    }

    private PropertyDefinition CreateForeignKeyPropertyDefinition (ClassDefinition classDefinition)
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID (classDefinition, "OrderTicket");
      propertyDefinition.SetStorageProperty (_foreignKeyStoragePropertyDefinitionStrictMock);
      return propertyDefinition;
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition, IStoragePropertyDefinition columnDefinition, SortOrder sortOrder)
    {
      var sortedPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition);
      sortedPropertyDefinition.SetStorageProperty (columnDefinition);
      return new SortedPropertySpecification (sortedPropertyDefinition, sortOrder);
    }

    private ClassDefinition CreateClassDefinition (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      classDefinition.SetStorageEntity (entityDefinition);
      return classDefinition;
    }
  }
}