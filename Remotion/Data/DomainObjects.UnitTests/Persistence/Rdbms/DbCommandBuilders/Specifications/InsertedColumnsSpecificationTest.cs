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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class InsertedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;

    private Guid _value1;
    private object _value2;
    private ColumnValue _columnValue1;
    private ColumnValue _columnValue2;
    private Mock<ISqlDialect> _sqlDialectStub;
    private InsertedColumnsSpecification _insertedColumnsSpecification;
    private Mock<IDbCommand> _dbCommandStub;
    private StringBuilder _statement;

    [SetUp]
    public void SetUp ()
    {
      _column1 = ColumnDefinitionObjectMother.IDColumn;
      _column2 = ColumnDefinitionObjectMother.TimestampColumn;

      _value1 = Guid.NewGuid();
      _value2 = DateTime.Now;

      _columnValue1 = new ColumnValue(_column1, _value1);
      _columnValue2 = new ColumnValue(_column2, _value2);

      _insertedColumnsSpecification = new InsertedColumnsSpecification(new[] { _columnValue1,  _columnValue2 });

      _sqlDialectStub = new Mock<ISqlDialect>();
      _dbCommandStub = new Mock<IDbCommand>();
      _statement = new StringBuilder();
    }

    [Test]
    public void AppendColumnNames ()
    {
      _sqlDialectStub.Setup (stub => stub.DelimitIdentifier ("ID")).Returns ("[ID]");
      _sqlDialectStub.Setup (stub => stub.DelimitIdentifier ("Timestamp")).Returns ("[Timestamp]");

      _insertedColumnsSpecification.AppendColumnNames(_statement, _dbCommandStub.Object, _sqlDialectStub.Object);

      Assert.That(_statement.ToString(), Is.EqualTo("[ID], [Timestamp]"));
    }

    [Test]
    public void AppendColumnValues ()
    {
      var parameter1StrictMock = new Mock<IDbDataParameter> (MockBehavior.Strict);
      parameter1StrictMock.SetupSet (mock => mock.ParameterName = "@ID").Verifiable();
      parameter1StrictMock.Setup (mock => mock.ParameterName).Returns ("@ID").Verifiable();
      parameter1StrictMock.SetupSet (mock => mock.Value = _value1).Verifiable();
      parameter1StrictMock.SetupSet (mock => mock.DbType = DbType.Guid).Verifiable();

      var parameter2StrictMock = new Mock<IDbDataParameter> (MockBehavior.Strict);
      parameter2StrictMock.SetupSet (mock => mock.ParameterName = "@Timestamp").Verifiable();
      parameter2StrictMock.Setup (mock => mock.ParameterName).Returns ("@Timestamp").Verifiable();
      parameter2StrictMock.SetupSet (mock => mock.Value = _value2).Verifiable();
      parameter2StrictMock.SetupSet (mock => mock.DbType = DbType.DateTime).Verifiable();

      var dataParameterCollectionMock = new Mock<IDataParameterCollection> (MockBehavior.Strict);

      _dbCommandStub.Setup (stub => stub.Parameters).Returns (dataParameterCollectionMock.Object);
      _dbCommandStub.Setup (stub => stub.CreateParameter()).Returns (parameter1StrictMock.Object);
      _dbCommandStub.Setup (stub => stub.CreateParameter()).Returns (parameter2StrictMock.Object);

      dataParameterCollectionMock.Setup (mock => mock.Add (parameter1StrictMock.Object)).Returns (0).Verifiable();
      dataParameterCollectionMock.Setup (mock => mock.Add (parameter2StrictMock.Object)).Returns (1).Verifiable();

      _sqlDialectStub.Setup (stub => stub.GetParameterName ("ID")).Returns ("@ID");
      _sqlDialectStub.Setup (stub => stub.GetParameterName ("Timestamp")).Returns ("@Timestamp");

      _insertedColumnsSpecification.AppendColumnValues(_statement, _dbCommandStub.Object, _sqlDialectStub.Object);

      dataParameterCollectionMock.Verify (mock => mock.Add (parameter2StrictMock.Object), Times.Once());
      parameter1StrictMock.Verify();
      parameter2StrictMock.Verify();
      Assert.That(_statement.ToString(), Is.EqualTo("@ID, @Timestamp"));
      Assert.That(_dbCommandStub.Object.Parameters, Is.SameAs(dataParameterCollectionMock.Object));
    }
  }
}
