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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class QueryCommandFactoryTest : StandardMappingTest
  {
    private Mock<IDbCommandBuilderFactory> _dbCommandBuilderFactoryStrictMock;
    private Mock<IObjectReaderFactory> _objectReaderFactoryStrictMock;
    private Mock<IObjectReader<DataContainer>> _dataContainerReader1Stub;
    private Mock<IDataStoragePropertyDefinitionFactory> _dataStoragePropertyDefinitionFactoryStub;
    private Mock<IDataParameterDefinitionFactory> _dataParameterDefinitionFactoryStrictMock;

    private QueryCommandFactory _factory;

    private QueryParameter _queryParameter1;
    private QueryParameter _queryParameter2;
    private QueryParameter _queryParameter3;
    private IDataParameterDefinition _dataParameterDefinition1;
    private IDataParameterDefinition _dataParameterDefinition2;
    private IDataParameterDefinition _dataParameterDefinition3;
    private Mock<IQuery> _queryStub;
    private Mock<IObjectReader<IQueryResultRow>> _resultRowReaderStub;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilderFactoryStrictMock = new Mock<IDbCommandBuilderFactory>(MockBehavior.Strict);

      _objectReaderFactoryStrictMock = new Mock<IObjectReaderFactory>(MockBehavior.Strict);
      _dataStoragePropertyDefinitionFactoryStub = new Mock<IDataStoragePropertyDefinitionFactory>();
      _dataParameterDefinitionFactoryStrictMock = new Mock<IDataParameterDefinitionFactory>(MockBehavior.Strict);

      _factory = new QueryCommandFactory(
          _objectReaderFactoryStrictMock.Object,
          _dbCommandBuilderFactoryStrictMock.Object,
          _dataStoragePropertyDefinitionFactoryStub.Object,
          _dataParameterDefinitionFactoryStrictMock.Object);

      _dataContainerReader1Stub = new Mock<IObjectReader<DataContainer>>();
      _resultRowReaderStub = new Mock<IObjectReader<IQueryResultRow>>();

      _queryParameter1 = new QueryParameter("first", DomainObjectIDs.Order1);
      _queryParameter2 = new QueryParameter("second", DomainObjectIDs.Order3.Value);
      _queryParameter3 = new QueryParameter("third", new[]{ DomainObjectIDs.Official1, DomainObjectIDs.Official2 });
      var collection = new QueryParameterCollection { _queryParameter1, _queryParameter2, _queryParameter3 };

      _queryStub = new Mock<IQuery>();
      _queryStub.Setup(stub => stub.Statement).Returns("statement");
      _queryStub.Setup(stub => stub.Parameters).Returns(new QueryParameterCollection(collection, true));

      _dataParameterDefinition1 = new Mock<IDataParameterDefinition>().Object;
      _dataParameterDefinition2 = new Mock<IDataParameterDefinition>().Object;
      _dataParameterDefinition3 = new Mock<IDataParameterDefinition>().Object;
    }

    [Test]
    public void CreateForDataContainerQuery ()
    {
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter1))
          .Returns(_dataParameterDefinition1)
          .Verifiable();
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter2))
          .Returns(_dataParameterDefinition2)
          .Verifiable();
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter3))
          .Returns(_dataParameterDefinition3)
          .Verifiable();

      var commandBuilderStub = new Mock<IDbCommandBuilder>();
      var expectedParametersWithType = GetExpectedParametersForQueryStub();
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForQuery("statement", expectedParametersWithType))
          .Returns(commandBuilderStub.Object)
          .Verifiable();

      _objectReaderFactoryStrictMock.Setup(mock => mock.CreateDataContainerReader()).Returns(_dataContainerReader1Stub.Object).Verifiable();

      var result = _factory.CreateForDataContainerQuery(_queryStub.Object);

      _dataParameterDefinitionFactoryStrictMock.Verify();
      _dbCommandBuilderFactoryStrictMock.Verify();
      _objectReaderFactoryStrictMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(MultiObjectLoadCommand<DataContainer>)));
      var command = ((MultiObjectLoadCommand<DataContainer>)result);
      Assert.That(command.DbCommandBuildersAndReaders.Length, Is.EqualTo(1));
      Assert.That(command.DbCommandBuildersAndReaders[0].Item1, Is.SameAs(commandBuilderStub.Object));
      Assert.That(command.DbCommandBuildersAndReaders[0].Item2, Is.SameAs(_dataContainerReader1Stub.Object));
    }

    [Test]
    public void CreateForCustomQuery ()
    {
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter1))
          .Returns(_dataParameterDefinition1)
          .Verifiable();
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter2))
          .Returns(_dataParameterDefinition2)
          .Verifiable();
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter3))
          .Returns(_dataParameterDefinition3)
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
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter1))
          .Returns(_dataParameterDefinition1)
          .Verifiable();
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter2))
          .Returns(_dataParameterDefinition2)
          .Verifiable();
      _dataParameterDefinitionFactoryStrictMock
          .Setup(mock => mock.CreateDataParameterDefinition(_queryParameter3))
          .Returns(_dataParameterDefinition3)
          .Verifiable();

      var commandBuilderStub = new Mock<IDbCommandBuilder>();
      var expectedParametersWithType = GetExpectedParametersForQueryStub();
      _dbCommandBuilderFactoryStrictMock
          .Setup(stub => stub.CreateForQuery("statement", expectedParametersWithType))
          .Returns(commandBuilderStub.Object)
          .Verifiable();

      var result = _factory.CreateForScalarQuery(_queryStub.Object);

      _dataParameterDefinitionFactoryStrictMock.Verify();
      _dbCommandBuilderFactoryStrictMock.Verify();

      Assert.That(result, Is.TypeOf(typeof(ScalarValueLoadCommand)));
      var command = (ScalarValueLoadCommand)result;
      Assert.That(command.DbCommandBuilder, Is.SameAs(commandBuilderStub.Object));
    }

    private QueryParameterWithDataParameterDefinition[] GetExpectedParametersForQueryStub ()
    {
      return new[]
             {
                 new QueryParameterWithDataParameterDefinition(_queryParameter1, _dataParameterDefinition1),
                 new QueryParameterWithDataParameterDefinition(_queryParameter2, _dataParameterDefinition2),
                 new QueryParameterWithDataParameterDefinition(_queryParameter3, _dataParameterDefinition3)
             };
    }
  }
}
