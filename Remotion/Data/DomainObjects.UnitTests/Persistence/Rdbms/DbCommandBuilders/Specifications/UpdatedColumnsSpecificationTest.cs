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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.NUnit.UnitTesting;

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
    private Mock<ISqlDialect> _sqlDialectStub;
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

      _updatedColumnsSpecification = new UpdatedColumnsSpecification(new[] { _columnValue1, _columnValue2 });

      _sqlDialectStub = new Mock<ISqlDialect>();
      _dbCommandStub = new Mock<IDbCommand>();
      _statement = new StringBuilder();
    }

    [Test]
    public void Initialization_Empty ()
    {
      Assert.That(
          () => new UpdatedColumnsSpecification(new ColumnValue[0]),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'columnValues' cannot be empty.", "columnValues"));
    }

    [Test]
    public void AppendColumnValueAssignments ()
    {
      var parameter1Stub = new Mock<IDbDataParameter>();
      parameter1Stub.Setup(_ => _.ParameterName).Returns("pID").Verifiable();

      var parameter2Stub = new Mock<IDbDataParameter>();
      parameter2Stub.Setup(_ => _.ParameterName).Returns("pTimestamp").Verifiable();

      var dataParameterCollectionMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      _dbCommandStub.Setup(_ => _.Parameters).Returns(dataParameterCollectionMock.Object);
      _dbCommandStub.SetupProperty(_ => _.CommandText);
      _dbCommandStub
          .SetupSequence(_ => _.CreateParameter())
          .Returns(parameter1Stub.Object)
          .Returns(parameter2Stub.Object);

      dataParameterCollectionMock.Setup(_ => _.Add(parameter1Stub.Object)).Returns(0).Verifiable();
      dataParameterCollectionMock.Setup(_ => _.Add(parameter2Stub.Object)).Returns(1).Verifiable();

      _sqlDialectStub.Setup(_ => _.DelimitIdentifier("ID")).Returns("[delimited ID]");
      _sqlDialectStub.Setup(_ => _.DelimitIdentifier("Timestamp")).Returns("[delimited Timestamp]");

      _sqlDialectStub.Setup(_ => _.GetParameterName("ID")).Returns("pID");
      _sqlDialectStub.Setup(_ => _.GetParameterName("Timestamp")).Returns("pTimestamp");

      _sqlDialectStub
          .Setup(_ => _.CreateDataParameter(_dbCommandStub.Object, _column1.StorageTypeInfo, "pID", _value1))
          .Returns(parameter1Stub.Object);
      _sqlDialectStub
          .Setup(_ => _.CreateDataParameter(_dbCommandStub.Object, _column2.StorageTypeInfo, "pTimestamp", _value2))
          .Returns(parameter2Stub.Object);

      _updatedColumnsSpecification.AppendColumnValueAssignments(_statement, _dbCommandStub.Object, _sqlDialectStub.Object);

      dataParameterCollectionMock.Verify();
      Assert.That(_statement.ToString(), Is.EqualTo("[delimited ID] = pID, [delimited Timestamp] = pTimestamp"));
      Assert.That(_dbCommandStub.Object.Parameters, Is.SameAs(dataParameterCollectionMock.Object));
    }
  }
}
