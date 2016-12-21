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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class QueryCommandFactoryTest : StandardMappingTest
  {
    private IDbCommandBuilderFactory _dbCommandBuilderFactoryStrictMock;
    private IObjectReaderFactory _objectReaderFactoryStrictMock;
    private IObjectReader<DataContainer> _dataContainerReader1Stub;
    private IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactoryStrictMock;

    private QueryCommandFactory _factory;

    private QueryParameter _queryParameter1;
    private QueryParameter _queryParameter2;
    private QueryParameter _queryParameter3;
    private IQuery _queryStub;
    private ObjectIDStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SerializedObjectIDStoragePropertyDefinition _property3;
    private IObjectReader<IQueryResultRow> _resultRowReaderStub;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStrictMock = MockRepository.GenerateStrictMock<IDbCommandBuilderFactory>();

      _objectReaderFactoryStrictMock = MockRepository.GenerateStrictMock<IObjectReaderFactory>();
      _dataStoragePropertyDefinitionFactoryStrictMock = MockRepository.GenerateStrictMock<IDataStoragePropertyDefinitionFactory> ();

      _factory = new QueryCommandFactory (
          _objectReaderFactoryStrictMock,
          _dbCommandBuilderFactoryStrictMock,
          _dataStoragePropertyDefinitionFactoryStrictMock);

      _dataContainerReader1Stub = MockRepository.GenerateStub<IObjectReader<DataContainer>>();
      _resultRowReaderStub = MockRepository.GenerateStub<IObjectReader<IQueryResultRow>>();

      _queryParameter1 = new QueryParameter ("first", DomainObjectIDs.Order1);
      _queryParameter2 = new QueryParameter ("second", DomainObjectIDs.Order3.Value);
      _queryParameter3 = new QueryParameter ("third", DomainObjectIDs.Official1);
      var collection = new QueryParameterCollection { _queryParameter1, _queryParameter2, _queryParameter3 };

      _queryStub = MockRepository.GenerateStub<IQuery>();
      _queryStub.Stub (stub => stub.Statement).Return ("statement");
      _queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection (collection, true));

      _property1 = ObjectIDStoragePropertyDefinitionObjectMother.Create ("Test");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ();
      _property3 = SerializedObjectIDStoragePropertyDefinitionObjectMother.Create ("Test");
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Order1))
          .Return (_property1);
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Order3.Value))
          .Return (_property2);
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Official1))
          .Return (_property3);

      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder>();
      var expectedParametersWithType = GetExpectedParametersForQueryStub();
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForQuery (Arg.Is ("statement"), Arg<IEnumerable<QueryParameterWithType>>.List.Equal (expectedParametersWithType)))
          .Return (commandBuilderStub);

      _objectReaderFactoryStrictMock.Expect (mock => mock.CreateDataContainerReader()).Return (_dataContainerReader1Stub);

      var result = _factory.CreateForDataContainerQuery (_queryStub);

      _dataStoragePropertyDefinitionFactoryStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations();
      _objectReaderFactoryStrictMock.VerifyAllExpectations();

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<DataContainer>)));
      var command = ((MultiObjectLoadCommand<DataContainer>) result);
      Assert.That (command.DbCommandBuildersAndReaders.Length, Is.EqualTo (1));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs (commandBuilderStub));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs (_dataContainerReader1Stub));
    }

    [Test]
    public void CreateForDataContainerQuery_TooComplexParameter ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.Statement).Return ("statement");
      queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection { new QueryParameter ("p1", Tuple.Create (1, "a")) });
      
      var compoundProperty = CompoundStoragePropertyDefinitionObjectMother.CreateWithTwoProperties ();

      _dataStoragePropertyDefinitionFactoryStrictMock
          .Stub (stub => stub.CreateStoragePropertyDefinition (Tuple.Create (1, "a")))
          .Return (compoundProperty);
      _dataStoragePropertyDefinitionFactoryStrictMock.Replay ();

      Assert.That (() => _factory.CreateForDataContainerQuery (queryStub), Throws.InvalidOperationException.With.Message.EqualTo (
          "The query parameter 'p1' is mapped to 2 database-level values. Only values that map to a single database-level value can be used as query "
          + "parameters."));
    }

    [Test]
    public void CreateForDataContainerQuery_ParameterYieldsUnsupportedStorageProperty ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.Statement).Return ("statement");
      queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection { new QueryParameter ("p1", Tuple.Create (1, "a")) });

      _dataStoragePropertyDefinitionFactoryStrictMock
          .Stub (stub => stub.CreateStoragePropertyDefinition (Tuple.Create (1, "a")))
          .Return (new UnsupportedStoragePropertyDefinition (typeof (string), "X.", null));
      _dataStoragePropertyDefinitionFactoryStrictMock.Replay ();

      Assert.That (
          () => _factory.CreateForDataContainerQuery (queryStub), 
          Throws.TypeOf<InvalidOperationException>()
              .With.Message.EqualTo (
                  "The query parameter 'p1' cannot be converted to a database value: This operation is not supported because the storage property is "
                  + "invalid. Reason: X.")
              .And.InnerException.TypeOf<NotSupportedException>());
    }

    [Test]
    public void CreateForCustomQuery ()
    {
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Order1))
          .Return (_property1);
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Order3.Value))
          .Return (_property2);
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Official1))
          .Return (_property3);

      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder> ();

      var expectedParametersWithType = GetExpectedParametersForQueryStub();
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForQuery (Arg.Is ("statement"), Arg<IEnumerable<QueryParameterWithType>>.List.Equal (expectedParametersWithType)))
          .Return (commandBuilderStub);

      _objectReaderFactoryStrictMock.Expect (mock => mock.CreateResultRowReader ()).Return (_resultRowReaderStub);

      var result = _factory.CreateForCustomQuery (_queryStub);

      Assert.That (result, Is.TypeOf (typeof (MultiObjectLoadCommand<IQueryResultRow>)));
      var command = ((MultiObjectLoadCommand<IQueryResultRow>) result);
      Assert.That (command.DbCommandBuildersAndReaders.Length, Is.EqualTo (1));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs (commandBuilderStub));
      Assert.That (command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs (_resultRowReaderStub));
    }

    [Test]
    public void CreateForScalarQuery ()
    {
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Order1))
          .Return (_property1);
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Order3.Value))
          .Return (_property2);
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (DomainObjectIDs.Official1))
          .Return (_property3);
      _dataStoragePropertyDefinitionFactoryStrictMock.Replay ();

      var commandBuilderStub = MockRepository.GenerateStub<IDbCommandBuilder> ();
      var expectedParametersWithType =
          new[]
          {
              new QueryParameterWithType (
                  new QueryParameter (_queryParameter1.Name, DomainObjectIDs.Order1.Value, _queryParameter1.ParameterType),
                  StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_property1).StorageTypeInfo),
              new QueryParameterWithType (_queryParameter2, _property2.ColumnDefinition.StorageTypeInfo),
              new QueryParameterWithType (
                  new QueryParameter (_queryParameter3.Name, DomainObjectIDs.Official1.ToString(), _queryParameter3.ParameterType),
                  StoragePropertyDefinitionTestHelper.GetSingleColumn (_property3.SerializedIDProperty).StorageTypeInfo)
          };
      _dbCommandBuilderFactoryStrictMock
          .Expect (
              stub => stub.CreateForQuery (Arg.Is ("statement"), Arg<IEnumerable<QueryParameterWithType>>.List.Equal (expectedParametersWithType)))
          .Return (commandBuilderStub);
      _dbCommandBuilderFactoryStrictMock.Replay ();

      var result = _factory.CreateForScalarQuery (_queryStub);

      _dataStoragePropertyDefinitionFactoryStrictMock.VerifyAllExpectations ();
      _dbCommandBuilderFactoryStrictMock.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf (typeof (ScalarValueLoadCommand)));
      var command = ((ScalarValueLoadCommand) result);
      Assert.That (command.DbCommandBuilder, Is.SameAs (commandBuilderStub));
    }

    [Test]
    public void CreateForScalarQuery_TooComplexParameter ()
    {
      var queryStub = MockRepository.GenerateStub<IQuery> ();
      queryStub.Stub (stub => stub.Statement).Return ("statement");
      queryStub.Stub (stub => stub.Parameters).Return (new QueryParameterCollection { new QueryParameter ("p1", Tuple.Create (1, "a")) });

      var compoundProperty = CompoundStoragePropertyDefinitionObjectMother.CreateWithTwoProperties ();

      _dataStoragePropertyDefinitionFactoryStrictMock
          .Stub (stub => stub.CreateStoragePropertyDefinition (Tuple.Create (1, "a")))
          .Return (compoundProperty);
      _dataStoragePropertyDefinitionFactoryStrictMock.Replay ();

      Assert.That (() => _factory.CreateForScalarQuery (queryStub), Throws.InvalidOperationException.With.Message.EqualTo (
          "The query parameter 'p1' is mapped to 2 database-level values. Only values that map to a single database-level value can be used as query "
          + "parameters."));
    }

    private QueryParameterWithType[] GetExpectedParametersForQueryStub ()
    {
      return new[]
             {
                 new QueryParameterWithType (
                     new QueryParameter (_queryParameter1.Name, DomainObjectIDs.Order1.Value, _queryParameter1.ParameterType),
                     StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_property1).StorageTypeInfo),
                 new QueryParameterWithType (_queryParameter2, _property2.ColumnDefinition.StorageTypeInfo),
                 new QueryParameterWithType (
                     new QueryParameter (_queryParameter3.Name, DomainObjectIDs.Official1.ToString(), _queryParameter3.ParameterType),
                     StoragePropertyDefinitionTestHelper.GetSingleColumn (_property3.SerializedIDProperty).StorageTypeInfo)
             };
    }
  }
}