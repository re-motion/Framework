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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class QueryDbCommandBuilderTest
  {
    private IStorageTypeInformation _valueParameterStorageTypeInformationMock;
    private IStorageTypeInformation _textParameterStorageTypeInformationMock;
    private QueryParameter _valueQueryParameter;
    private QueryParameter _textQueryParameter;
    private QueryParameterWithType _valueQueryParameterWithType;
    private QueryParameterWithType _textQueryParameterWithType;
    private QueryDbCommandBuilder _commandBuilder;

    [SetUp]
    public void SetUp ()
    {
      _valueParameterStorageTypeInformationMock = MockRepository.GenerateStrictMock<IStorageTypeInformation>();
      _textParameterStorageTypeInformationMock = MockRepository.GenerateStrictMock<IStorageTypeInformation>();

      _valueQueryParameter = new QueryParameter ("@param1", 5,QueryParameterType.Value);
      _textQueryParameter = new QueryParameter ("@param2", "test", QueryParameterType.Text);

      _valueQueryParameterWithType = new QueryParameterWithType (_valueQueryParameter, _valueParameterStorageTypeInformationMock);
      _textQueryParameterWithType = new QueryParameterWithType (_textQueryParameter, _textParameterStorageTypeInformationMock);

      _commandBuilder = new QueryDbCommandBuilder (
          "Statement @param1 @param2", new[] { _valueQueryParameterWithType, _textQueryParameterWithType }, MockRepository.GenerateStub<ISqlDialect>());
    }

    [Test]
    public void Create ()
    {
      var dataParameterCollectionStrictMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();

      var dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      dbCommandStub.Stub (stub => stub.Parameters).Return (dataParameterCollectionStrictMock);

      var executionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      executionContextStub.Stub (stub => stub.CreateDbCommand()).Return(dbCommandStub);

      var dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _valueParameterStorageTypeInformationMock
        .Expect (mock => mock.CreateDataParameter (dbCommandStub, 5))
        .Return (dbDataParameterStub);
      _valueParameterStorageTypeInformationMock.Replay();

      _textParameterStorageTypeInformationMock.Replay();

      dataParameterCollectionStrictMock.Expect (mock => mock.Add (dbDataParameterStub)).Return (0);
      dataParameterCollectionStrictMock.Replay();
      
      var result = _commandBuilder.Create (executionContextStub);

      dataParameterCollectionStrictMock.VerifyAllExpectations();
      _valueParameterStorageTypeInformationMock.VerifyAllExpectations();
      _textParameterStorageTypeInformationMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (dbCommandStub));
      Assert.That (result.CommandText, Is.EqualTo ("Statement @param1 test"));
    }
  }
}