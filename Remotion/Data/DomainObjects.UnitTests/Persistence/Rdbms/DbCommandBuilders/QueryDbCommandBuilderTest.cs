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
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class QueryDbCommandBuilderTest
  {
    private class TestQueryDbCommandBuilder : QueryDbCommandBuilder
    {
      [NotNull]
      private readonly IReadOnlyDictionary<string, string> _parameterTextRepresentations;

      public TestQueryDbCommandBuilder ([NotNull] string statement, [NotNull] IReadOnlyCollection<QueryParameterWithDataParameterDefinition> parameters, [NotNull] ISqlDialect sqlDialect,
          [NotNull] IReadOnlyDictionary<string, string> parameterTextRepresentations)
          : base(statement, parameters, sqlDialect)
      {
        _parameterTextRepresentations = parameterTextRepresentations;
      }

      protected override string GetParameterTextRepresentation (QueryParameterWithDataParameterDefinition parameterWithDefinition)
      {
        if (_parameterTextRepresentations.TryGetValue(parameterWithDefinition.QueryParameter.Name, out var representation))
          return representation;

        return base.GetParameterTextRepresentation(parameterWithDefinition);
      }
    }

    private Mock<IDataParameterDefinition> _valueParameterDefinitionMock;
    private Mock<IDataParameterDefinition> _textParameterDefinitionMock;
    private QueryParameter _valueQueryParameter;
    private QueryParameter _textQueryParameter;
    private QueryParameterWithDataParameterDefinition _valueQueryParameterWithDataDefinition;
    private QueryParameterWithDataParameterDefinition _textQueryParameterWithDataDefinition;

    [SetUp]
    public void SetUp ()
    {
      _valueParameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
      _textParameterDefinitionMock = new Mock<IDataParameterDefinition>(MockBehavior.Strict);

      _valueQueryParameter = new QueryParameter("#param1", 5, QueryParameterType.Value);
      _textQueryParameter = new QueryParameter("#param2", "test", QueryParameterType.Text);

      _valueQueryParameterWithDataDefinition = new QueryParameterWithDataParameterDefinition(_valueQueryParameter, _valueParameterDefinitionMock.Object);
      _textQueryParameterWithDataDefinition = new QueryParameterWithDataParameterDefinition(_textQueryParameter, _textParameterDefinitionMock.Object);
    }

    [Test]
    public void Create_UsesDbCommandFactoryToCreateQueryCommand ()
    {
      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var commandBuilder = new QueryDbCommandBuilder("Statement @param1 @param2", [], GetSqlDialectStub());
      var result = commandBuilder.Create(dbCommandFactoryStub.Object);

      dataParameterCollectionStrictMock.Verify();
      _valueParameterDefinitionMock.Verify();
      _textParameterDefinitionMock.Verify();
      Assert.That(result, Is.SameAs(dbCommandStub.Object));
    }

    [Test]
    public void Create_TransfersCommandTextToQueryCommand ()
    {
      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      _valueParameterDefinitionMock.Setup(mock => mock.GetParameterValue(5)).Returns(5);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var commandBuilder = new QueryDbCommandBuilder("Statement @param1 @param2", [], GetSqlDialectStub());
      var result = commandBuilder.Create(dbCommandFactoryStub.Object);

      dataParameterCollectionStrictMock.Verify();
      _valueParameterDefinitionMock.Verify();
      _textParameterDefinitionMock.Verify();
      Assert.That(result.CommandText, Is.EqualTo("Statement @param1 @param2"));
    }

    [Test]
    public void Create_ReplacesTextParameter ()
    {
      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      _valueParameterDefinitionMock.Setup(mock => mock.GetParameterValue(5)).Returns(5);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var commandBuilder = new QueryDbCommandBuilder("Statement #param1 #param2", [ _textQueryParameterWithDataDefinition ], GetSqlDialectStub());
      var result = commandBuilder.Create(dbCommandFactoryStub.Object);

      dataParameterCollectionStrictMock.Verify();
      _valueParameterDefinitionMock.Verify();
      _textParameterDefinitionMock.Verify();
      Assert.That(result.CommandText, Is.EqualTo("Statement #param1 test"));
    }

    [Test]
    public void Create_AddsValueParameter ()
    {
      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var dbDataParameterStub = new Mock<IDbDataParameter>();

      _valueParameterDefinitionMock.Setup(mock => mock.GetParameterValue(5)).Returns(5);
      _valueParameterDefinitionMock.Setup(mock => mock.CreateDataParameter(dbCommandStub.Object, _valueQueryParameter.Name, 5))
          .Returns(dbDataParameterStub.Object)
          .Verifiable();

      dataParameterCollectionStrictMock.Setup(mock => mock.Add(dbDataParameterStub.Object)).Returns(0).Verifiable();

      var commandBuilder = new QueryDbCommandBuilder("Statement @param1 @param2", [ _valueQueryParameterWithDataDefinition ], GetSqlDialectStub());
      var result = commandBuilder.Create(dbCommandFactoryStub.Object);

      dataParameterCollectionStrictMock.Verify();
      _valueParameterDefinitionMock.Verify();
      _textParameterDefinitionMock.Verify();
    }

    [Test]
    public void Create_ReplacesParameterWithTextRepresentation ()
    {
      var statement = """
                      Start #with_underscore with_underscore  #1startsWithNumber
                      #param1 param1 #param11
                      #noTextRepresentation End
                      """;
      var parameterTextRepresentations = new Dictionary<string, string>
                                         {
                                             { "#with_underscore", "UnderscoreValue"},
                                             { "#1startsWithNumber", "StartsWithNumberValue"},
                                             { "#param1", "Value1"},
                                             { "#param11", "Value2"}
                                         };
      var expected = """
                      Start UnderscoreValue with_underscore  StartsWithNumberValue
                      Value1 param1 Value2
                      #noTextRepresentation End
                      """;

      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);
      for (int i = 0; i < parameterTextRepresentations.Count; i++)
        dataParameterCollectionStrictMock.Setup(mock => mock.Add(It.IsAny<IDbDataParameter>())).Returns(i);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var valueParameters = parameterTextRepresentations.Keys.Select(
          name => new QueryParameterWithDataParameterDefinition(
              new QueryParameter(name, null, QueryParameterType.Value),
              new Mock<IDataParameterDefinition>().Object))
          .ToArray();

      var commandBuilder = new TestQueryDbCommandBuilder(
          statement,
          valueParameters,
          GetSqlDialectStub(),
          parameterTextRepresentations);
      var result = commandBuilder.Create(dbCommandFactoryStub.Object);

      Assert.That(result.CommandText, Is.EqualTo(expected));
      dataParameterCollectionStrictMock.Verify();
      _valueParameterDefinitionMock.Verify();
      _textParameterDefinitionMock.Verify();
    }

    private static ISqlDialect GetSqlDialectStub ()
    {
      var stub = new Mock<ISqlDialect>();
      stub.Setup(_=>_.GetParameterName(It.IsAny<string>())).Returns<string>(arg => "#" + arg);
      return stub.Object;
    }
  }
}
