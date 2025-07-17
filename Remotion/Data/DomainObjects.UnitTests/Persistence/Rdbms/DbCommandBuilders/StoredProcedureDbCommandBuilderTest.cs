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
using System.Data;
using System.Data.Common;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders;

[TestFixture]
public class StoredProcedureDbCommandBuilderTest
{
  private Mock<IDataParameterDefinition> _valueParameterDefinitionMock;
  private QueryParameter _valueQueryParameter;
  private QueryParameterWithDataParameterDefinition _valueQueryParameterWithDataDefinition;

  [SetUp]
  public void SetUp ()
  {
    _valueParameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    _valueQueryParameter = new QueryParameter("#param1", 5, QueryParameterType.Value);
    _valueQueryParameterWithDataDefinition = new QueryParameterWithDataParameterDefinition(_valueQueryParameter, _valueParameterDefinitionMock.Object);
  }

  [Test]
  public void Create_UsesDbCommandFactoryToCreateQueryCommand ()
  {
    var dataParameterCollectionStrictMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    var dbCommandStub = new Mock<DbCommand>();
    dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(dataParameterCollectionStrictMock.Object);
    dbCommandStub.SetupProperty(stub => stub.CommandText);

    var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

    var commandBuilder = new StoredProcedureDbCommandBuilder("Procedure", [], GetSqlDialectStub());
    var result = commandBuilder.Create(dbCommandFactoryStub.Object);

    dataParameterCollectionStrictMock.Verify();
    _valueParameterDefinitionMock.Verify();
    Assert.That(result, Is.SameAs(dbCommandStub.Object));
  }

  [Test]
  public void Create_TransfersCommandTextToQueryCommand ()
  {
    var dataParameterCollectionStrictMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    _valueParameterDefinitionMock.Setup(mock => mock.GetParameterValue(5)).Returns(5);

    var dbCommandStub = new Mock<DbCommand>();
    dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(dataParameterCollectionStrictMock.Object);
    dbCommandStub.SetupProperty(stub => stub.CommandText);

    var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

    var commandBuilder = new StoredProcedureDbCommandBuilder("Procedure", [], GetSqlDialectStub());
    var result = commandBuilder.Create(dbCommandFactoryStub.Object);

    dataParameterCollectionStrictMock.Verify();
    _valueParameterDefinitionMock.Verify();
    Assert.That(result.CommandText, Is.EqualTo("Procedure"));
  }

  [Test]
  public void Create_ThrowsOnTextParameter ()
  {
    var dataParameterCollectionStrictMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    var dbCommandStub = new Mock<DbCommand>();
    dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(dataParameterCollectionStrictMock.Object);
    dbCommandStub.SetupProperty(stub => stub.CommandText);

    var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

    var textQueryParameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
    var textQueryParameter = new QueryParameter("#textParam", "replacementText", QueryParameterType.Text);
    var textQueryParameterWithDataDefinition = new QueryParameterWithDataParameterDefinition(textQueryParameter, textQueryParameterDefinitionMock.Object);

    var commandBuilder = new StoredProcedureDbCommandBuilder("Procedure", [ textQueryParameterWithDataDefinition ], GetSqlDialectStub());
    Assert.That(() => commandBuilder.Create(dbCommandFactoryStub.Object), Throws.InstanceOf<NotSupportedException>());
  }

  [Test]
  public void Create_AddsValueParameter ()
  {
    var dataParameterCollectionStrictMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    var dbCommandStub = new Mock<DbCommand>();
    dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(dataParameterCollectionStrictMock.Object);
    dbCommandStub.SetupProperty(stub => stub.CommandText);

    var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

    var dbDataParameterStub = new Mock<DbParameter>();

    _valueParameterDefinitionMock.Setup(mock => mock.GetParameterValue(5)).Returns(5);
    _valueParameterDefinitionMock.Setup(mock => mock.CreateDataParameter(dbCommandStub.Object, _valueQueryParameter.Name, 5))
        .Returns(dbDataParameterStub.Object)
        .Verifiable();

    dataParameterCollectionStrictMock.Setup(mock => mock.Add(dbDataParameterStub.Object)).Returns(0).Verifiable();

    var commandBuilder = new StoredProcedureDbCommandBuilder("Procedure", [ _valueQueryParameterWithDataDefinition ], GetSqlDialectStub());
    var result = commandBuilder.Create(dbCommandFactoryStub.Object);

    dataParameterCollectionStrictMock.Verify();
    _valueParameterDefinitionMock.Verify();
  }

  [Test]
  public void Create_SetsCommandTypeToStoredProcedure ()
  {
    var dataParameterCollectionStrictMock = new Mock<DbParameterCollection>(MockBehavior.Strict);

    var dbCommandStub = new Mock<DbCommand>();
    dbCommandStub.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(dataParameterCollectionStrictMock.Object);
    dbCommandStub.SetupProperty(stub => stub.CommandType);

    var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
    dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

    var commandBuilder = new StoredProcedureDbCommandBuilder("Procedure", [ ], GetSqlDialectStub());
    var result = commandBuilder.Create(dbCommandFactoryStub.Object);
    Assert.That(result.CommandType, Is.EqualTo(CommandType.StoredProcedure));
  }

  private static ISqlDialect GetSqlDialectStub ()
  {
    var stub = new Mock<ISqlDialect>();
    stub.Setup(_ => _.GetParameterName(It.IsAny<string>())).Returns<string>(arg => "#" + arg);
    return stub.Object;
  }
}
