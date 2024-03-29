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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class QueryDbCommandBuilderTest
  {
    private Mock<IStorageTypeInformation> _valueParameterStorageTypeInformationMock;
    private Mock<IStorageTypeInformation> _textParameterStorageTypeInformationMock;
    private QueryParameter _valueQueryParameter;
    private QueryParameter _textQueryParameter;
    private QueryParameterWithType _valueQueryParameterWithType;
    private QueryParameterWithType _textQueryParameterWithType;
    private QueryDbCommandBuilder _commandBuilder;

    [SetUp]
    public void SetUp ()
    {
      _valueParameterStorageTypeInformationMock = new Mock<IStorageTypeInformation>(MockBehavior.Strict);
      _textParameterStorageTypeInformationMock = new Mock<IStorageTypeInformation>(MockBehavior.Strict);

      _valueQueryParameter = new QueryParameter("@param1", 5,QueryParameterType.Value);
      _textQueryParameter = new QueryParameter("@param2", "test", QueryParameterType.Text);

      _valueQueryParameterWithType = new QueryParameterWithType(_valueQueryParameter, _valueParameterStorageTypeInformationMock.Object);
      _textQueryParameterWithType = new QueryParameterWithType(_textQueryParameter, _textParameterStorageTypeInformationMock.Object);

      _commandBuilder = new QueryDbCommandBuilder(
          "Statement @param1 @param2", new[] { _valueQueryParameterWithType, _textQueryParameterWithType }, new Mock<ISqlDialect>().Object);
    }

    [Test]
    public void Create ()
    {
      var dataParameterCollectionStrictMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      var dbCommandStub = new Mock<IDbCommand>();
      dbCommandStub.Setup(stub => stub.Parameters).Returns(dataParameterCollectionStrictMock.Object);
      dbCommandStub.SetupProperty(stub => stub.CommandText);

      var dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(dbCommandStub.Object);

      var dbDataParameterStub = new Mock<IDbDataParameter>();
      _valueParameterStorageTypeInformationMock
          .Setup(mock => mock.CreateDataParameter(dbCommandStub.Object, 5))
          .Returns(dbDataParameterStub.Object)
          .Verifiable();

      dataParameterCollectionStrictMock.Setup(mock => mock.Add(dbDataParameterStub.Object)).Returns(0).Verifiable();

      var result = _commandBuilder.Create(dbCommandFactoryStub.Object);

      dataParameterCollectionStrictMock.Verify();
      _valueParameterStorageTypeInformationMock.Verify();
      _textParameterStorageTypeInformationMock.Verify();
      Assert.That(result, Is.SameAs(dbCommandStub.Object));
      Assert.That(result.CommandText, Is.EqualTo("Statement @param1 test"));
    }
  }
}
