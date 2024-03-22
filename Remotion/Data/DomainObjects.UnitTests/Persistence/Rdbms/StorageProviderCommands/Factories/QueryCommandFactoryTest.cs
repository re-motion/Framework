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
using Moq;
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

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class QueryCommandFactoryTest : StandardMappingTest
  {
    private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStrictMock;
    private Mock<IObjectReaderFactory> _objectReaderFactoryStrictMock;
    private Mock<IObjectReader<DataContainer>> _dataContainerReader1Stub;
    private Mock<IDataStoragePropertyDefinitionFactory> _dataStoragePropertyDefinitionFactoryStrictMock;

    private QueryCommandFactory _factory;

    private QueryParameter _queryParameter1;
    private QueryParameter _queryParameter2;
    private QueryParameter _queryParameter3;
    private Mock<IQuery> _queryStub;
    private ObjectIDStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SerializedObjectIDStoragePropertyDefinition _property3;
    private Mock<IObjectReader<IQueryResultRow>> _resultRowReaderStub;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStrictMock = new Mock<IDbCommandBuilderFactory>(MockBehavior.Strict);

      _objectReaderFactoryStrictMock = new Mock<IObjectReaderFactory>(MockBehavior.Strict);
      _dataStoragePropertyDefinitionFactoryStrictMock = new Mock<IDataStoragePropertyDefinitionFactory>(MockBehavior.Strict);

      _factory = new QueryCommandFactory(
          _objectReaderFactoryStrictMock.Object,
          _dbCommandBuilderFactoryStrictMock.Object,
          _dataStoragePropertyDefinitionFactoryStrictMock.Object);

      _dataContainerReader1Stub = new Mock<IObjectReader<DataContainer>>();
      _resultRowReaderStub = new Mock<IObjectReader<IQueryResultRow>>();

      _queryParameter1 = new QueryParameter("first", DomainObjectIDs.Order1);
      _queryParameter2 = new QueryParameter("second", DomainObjectIDs.Order3.Value);
      _queryParameter3 = new QueryParameter("third", DomainObjectIDs.Official1);
      var collection = new QueryParameterCollection { _queryParameter1, _queryParameter2, _queryParameter3 };

      _queryStub = new Mock<IQuery>();
      _queryStub.Setup(stub => stub.Statement).Returns("statement");
      _queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection(collection, true));

      _property1 = ObjectIDStoragePropertyDefinitionObjectMother.Create("Test");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();
      _property3 = SerializedObjectIDStoragePropertyDefinitionObjectMother.Create("Test");
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Order1))
          .Returns(_property1)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Order3.Value))
          .Returns(_property2)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Official1))
          .Returns(_property3)
          .Verifiable();

      var commandBuilderStub = new Mock<IDbCommandBuilder>();
      var expectedParametersWithType = GetExpectedParametersForQueryStub();
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForQuery("statement", expectedParametersWithType))
          .Returns(commandBuilderStub.Object)
          .Verifiable();

      _objectReaderFactoryStrictMock.Setup(mock => mock.CreateDataContainerReader()).Returns(_dataContainerReader1Stub.Object).Verifiable();

      var result = _factory.CreateForDataContainerQuery(_queryStub.Object);

      _dataStoragePropertyDefinitionFactoryStrictMock.Verify();
      _dbCommandBuilderFactoryStrictMock.Verify();
      _objectReaderFactoryStrictMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(MultiObjectLoadCommand<DataContainer>)));
      var command = ((MultiObjectLoadCommand<DataContainer>)result);
      Assert.That(command.DbCommandBuildersAndReaders.Length, Is.EqualTo(1));
      Assert.That(command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs(commandBuilderStub.Object));
      Assert.That(command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs(_dataContainerReader1Stub.Object));
    }

    [Test]
    public void CreateForDataContainerQuery_TooComplexParameter ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.Statement).Returns("statement");
      queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection { new QueryParameter("p1", Tuple.Create(1, "a")) });

      var compoundProperty = CompoundStoragePropertyDefinitionObjectMother.CreateWithTwoProperties();

      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(stub => stub.CreateStoragePropertyDefinition(Tuple.Create(1, "a")))
          .Returns(compoundProperty);

      Assert.That(() => _factory.CreateForDataContainerQuery(queryStub.Object), Throws.InvalidOperationException.With.Message.EqualTo(
          "The query parameter 'p1' is mapped to 2 database-level values. Only values that map to a single database-level value can be used as query "
          + "parameters."));
    }

    [Test]
    public void CreateForDataContainerQuery_ParameterYieldsUnsupportedStorageProperty ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.Statement).Returns("statement");
      queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection { new QueryParameter("p1", Tuple.Create(1, "a")) });

      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(stub => stub.CreateStoragePropertyDefinition(Tuple.Create(1, "a")))
          .Returns(new UnsupportedStoragePropertyDefinition(typeof(string), "X.", null));

      Assert.That(
          () => _factory.CreateForDataContainerQuery(queryStub.Object),
          Throws.TypeOf<InvalidOperationException>()
              .With.Message.EqualTo(
                  "The query parameter 'p1' cannot be converted to a database value: This operation is not supported because the storage property is "
                  + "invalid. Reason: X.")
              .And.InnerException.TypeOf<NotSupportedException>());
    }

    [Test]
    public void CreateForCustomQuery ()
    {
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Order1))
          .Returns(_property1)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Order3.Value))
          .Returns(_property2)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Official1))
          .Returns(_property3)
          .Verifiable();

      var commandBuilderStub = new Mock<IDbCommandBuilder>();

      var expectedParametersWithType = GetExpectedParametersForQueryStub();
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForQuery("statement", expectedParametersWithType))
          .Returns(commandBuilderStub.Object)
          .Verifiable();

      _objectReaderFactoryStrictMock.Setup(mock => mock.CreateResultRowReader()).Returns(_resultRowReaderStub.Object).Verifiable();

      var result = _factory.CreateForCustomQuery(_queryStub.Object);

      Assert.That(result, Is.TypeOf(typeof(MultiObjectLoadCommand<IQueryResultRow>)));
      var command = ((MultiObjectLoadCommand<IQueryResultRow>)result);
      Assert.That(command.DbCommandBuildersAndReaders.Length, Is.EqualTo(1));
      Assert.That(command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs(commandBuilderStub.Object));
      Assert.That(command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs(_resultRowReaderStub.Object));
    }

    [Test]
    public void CreateForScalarQuery ()
    {
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Order1))
          .Returns(_property1)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Order3.Value))
          .Returns(_property2)
          .Verifiable();
      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateStoragePropertyDefinition(DomainObjectIDs.Official1))
          .Returns(_property3)
          .Verifiable();

      var commandBuilderStub = new Mock<IDbCommandBuilder>();
      var expectedParametersWithType =
          new[]
          {
              new QueryParameterWithType(
                  new QueryParameter(_queryParameter1.Name, DomainObjectIDs.Order1.Value, _queryParameter1.ParameterType),
                  StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_property1).StorageTypeInfo),
              new QueryParameterWithType(_queryParameter2, _property2.ColumnDefinition.StorageTypeInfo),
              new QueryParameterWithType(
                  new QueryParameter(_queryParameter3.Name, DomainObjectIDs.Official1.ToString(), _queryParameter3.ParameterType),
                  StoragePropertyDefinitionTestHelper.GetSingleColumn(_property3.SerializedIDProperty).StorageTypeInfo)
          };
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForQuery("statement", expectedParametersWithType))
          .Returns(commandBuilderStub.Object)
          .Verifiable();

      var result = _factory.CreateForScalarQuery(_queryStub.Object);

      _dataStoragePropertyDefinitionFactoryStrictMock.Verify();
      _dbCommandBuilderFactoryStrictMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(ScalarValueLoadCommand)));
      var command = ((ScalarValueLoadCommand)result);
      Assert.That(command.DbCommandBuilder, Is.SameAs(commandBuilderStub.Object));
    }

    [Test]
    public void CreateForScalarQuery_TooComplexParameter ()
    {
      var queryStub = new Mock<IQuery>();
      queryStub.Setup(stub => stub.Statement).Returns("statement");
      queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection { new QueryParameter("p1", Tuple.Create(1, "a")) });

      var compoundProperty = CompoundStoragePropertyDefinitionObjectMother.CreateWithTwoProperties();

      _dataStoragePropertyDefinitionFactoryStrictMock
          .Setup(stub => stub.CreateStoragePropertyDefinition(Tuple.Create(1, "a")))
          .Returns(compoundProperty);

      Assert.That(() => _factory.CreateForScalarQuery(queryStub.Object), Throws.InvalidOperationException.With.Message.EqualTo(
          "The query parameter 'p1' is mapped to 2 database-level values. Only values that map to a single database-level value can be used as query "
          + "parameters."));
    }

    private QueryParameterWithType[] GetExpectedParametersForQueryStub ()
    {
      return new[]
             {
                 new QueryParameterWithType(
                     new QueryParameter(_queryParameter1.Name, DomainObjectIDs.Order1.Value, _queryParameter1.ParameterType),
                     StoragePropertyDefinitionTestHelper.GetIDColumnDefinition(_property1).StorageTypeInfo),
                 new QueryParameterWithType(_queryParameter2, _property2.ColumnDefinition.StorageTypeInfo),
                 new QueryParameterWithType(
                     new QueryParameter(_queryParameter3.Name, DomainObjectIDs.Official1.ToString(), _queryParameter3.ParameterType),
                     StoragePropertyDefinitionTestHelper.GetSingleColumn(_property3.SerializedIDProperty).StorageTypeInfo)
             };
    }
  }
}
