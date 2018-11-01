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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class UpdatedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private Guid _value1;
    private DateTime _value2;
    private ColumnValue _columnValue1;
    private ColumnValue _columnValue2;
    private UpdatedColumnsSpecification _updatedColumnsSpecification;
    private ISqlDialect _sqlDialectStub;
    private IDbCommand _dbCommandStub;
    private StringBuilder _statement;

    [SetUp]
    public void SetUp ()
    {
      _column1 = ColumnDefinitionObjectMother.IDColumn;
      _column2 = ColumnDefinitionObjectMother.TimestampColumn;

      _value1 = Guid.NewGuid ();
      _value2 = DateTime.Now;

      _columnValue1 = new ColumnValue (_column1, _value1);
      _columnValue2 = new ColumnValue (_column2, _value2);

      _updatedColumnsSpecification = new UpdatedColumnsSpecification (new[] { _columnValue1, _columnValue2 });

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect> ();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand> ();
      _statement = new StringBuilder ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'columnValues' cannot be empty.\r\nParameter name: columnValues")]
    public void Initialization_Empty ()
    {
      new UpdatedColumnsSpecification (new ColumnValue[0]);
    }

    [Test]
    public void AppendColumnValueAssignments ()
    {
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("ID")).Return ("[delimited ID]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Timestamp")).Return ("[delimited Timestamp]");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("ID")).Return ("pID");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("Timestamp")).Return ("pTimestamp");

      var parameter1StrictMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameter1StrictMock.Expect (mock => mock.ParameterName = "pID");
      parameter1StrictMock.Expect (mock => mock.ParameterName).Return ("pID");
      parameter1StrictMock.Expect (mock => mock.Value = _value1);
      parameter1StrictMock.Expect (mock => mock.DbType = DbType.Guid);
      parameter1StrictMock.Replay ();

      var parameter2StrictMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();
      parameter2StrictMock.Expect (mock => mock.ParameterName = "pTimestamp");
      parameter2StrictMock.Expect (mock => mock.ParameterName).Return ("pTimestamp");
      parameter2StrictMock.Expect (mock => mock.Value = _value2);
      parameter2StrictMock.Expect (mock => mock.DbType = DbType.DateTime);
      parameter2StrictMock.Replay ();

      var dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection> ();

      _dbCommandStub.Stub (stub => stub.Parameters).Return (dataParameterCollectionMock);
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (parameter1StrictMock).Repeat.Once ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (parameter2StrictMock).Repeat.Once ();

      dataParameterCollectionMock.Expect (mock => mock.Add (parameter1StrictMock)).Return (0).Repeat.Once ();
      dataParameterCollectionMock.Expect (mock => mock.Add (parameter2StrictMock)).Return (1).Repeat.Once ();
      dataParameterCollectionMock.Replay ();

      _updatedColumnsSpecification.AppendColumnValueAssignments (_statement, _dbCommandStub, _sqlDialectStub);

      dataParameterCollectionMock.VerifyAllExpectations ();
      parameter1StrictMock.VerifyAllExpectations ();
      parameter2StrictMock.VerifyAllExpectations ();
      Assert.That (_statement.ToString (), Is.EqualTo ("[delimited ID] = pID, [delimited Timestamp] = pTimestamp"));
      Assert.That (_dbCommandStub.Parameters, Is.SameAs (dataParameterCollectionMock));
    }
  }
}