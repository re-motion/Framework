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
    private Mock<IDataParameterDefinition> _valueParameterDefinition;
    private Mock<IDataParameterDefinition> _textParameterDefinition;
    private QueryParameter _valueQueryParameter;
    private QueryParameter _textQueryParameter;
    private QueryParameterWithDataParameterDefinition _valueQueryParameterWithDataDefinition;
    private QueryParameterWithDataParameterDefinition _textQueryParameterWithDataDefinition;
    private QueryDbCommandBuilder _commandBuilder;

    [SetUp]
    public void SetUp ()
    {
      _valueParameterDefinition = new Mock<IDataParameterDefinition>(MockBehavior.Strict);
      _textParameterDefinition = new Mock<IDataParameterDefinition>(MockBehavior.Strict);

      _valueQueryParameter = new QueryParameter("@param1", 5,QueryParameterType.Value);
      _textQueryParameter = new QueryParameter("@param2", "test", QueryParameterType.Text);

      _valueQueryParameterWithDataDefinition = new QueryParameterWithDataParameterDefinition(_valueQueryParameter, _valueParameterDefinition.Object);
      _textQueryParameterWithDataDefinition = new QueryParameterWithDataParameterDefinition(_textQueryParameter, _textParameterDefinition.Object);

      _commandBuilder = new QueryDbCommandBuilder(
          "Statement @param1 @param2", new[] { _valueQueryParameterWithDataDefinition, _textQueryParameterWithDataDefinition }, new Mock<ISqlDialect>().Object);
    }

    [Test]
    public void Create ()
    {
      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      _valueParameterDefinition.Setup(mock => mock.GetParameterValue(5)).Returns(5);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var executionContextStub = new Mock<IRdbmsProviderCommandExecutionContext>();
      executionContextStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var dbDataParameterStub = new Mock<IDbDataParameter>();
      _valueParameterDefinition
          .Setup(mock => mock.CreateDataParameter(dbCommandStub.Object, _valueQueryParameter.Name, 5))
          .Returns(dbDataParameterStub.Object)
          .Verifiable();

      dataParameterCollectionStrictMock.Setup(mock => mock.Add(dbDataParameterStub.Object)).Returns(0).Verifiable();

      var result = _commandBuilder.Create(executionContextStub.Object);

      dataParameterCollectionStrictMock.Verify();
      _valueParameterDefinition.Verify();
      _textParameterDefinition.Verify();
      Assert.That(result, Is.SameAs(dbCommandStub.Object));
      Assert.That(result.CommandText, Is.EqualTo("Statement @param1 test"));
    }
  }
}
