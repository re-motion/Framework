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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class SelectDbCommandBuilderTest : StandardMappingTest
  {
    private Mock<ISelectedColumnsSpecification> _selectedColumnsStub;
    private Mock<IComparedColumnsSpecification> _comparedColumnsStrictMock;
    private Mock<IOrderedColumnsSpecification> _orderedColumnsStub;

    private Mock<ISqlDialect> _sqlDialectStub;
    private Mock<IDbCommand> _dbCommandStub;
    private Mock<IDbDataParameter> _dbDataParameterStub;
    private Mock<IDataParameterCollection> _dataParameterCollectionMock;
    private Mock<IDbCommandFactory> _dbCommandFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _selectedColumnsStub = new Mock<ISelectedColumnsSpecification>();
      _selectedColumnsStub
          .Setup(stub => stub.AppendProjection(It.IsAny<StringBuilder>(), It.IsAny<ISqlDialect>()))
          .Callback((StringBuilder statement, ISqlDialect _) => statement.Append("[Column1], [Column2], [Column3]"));
      _comparedColumnsStrictMock = new Mock<IComparedColumnsSpecification>(MockBehavior.Strict);
      _orderedColumnsStub = new Mock<IOrderedColumnsSpecification>();

      _sqlDialectStub = new Mock<ISqlDialect>();
      _sqlDialectStub.Setup(stub => stub.StatementDelimiter).Returns(";");
      _dbDataParameterStub = new Mock<IDbDataParameter>();
      _dataParameterCollectionMock = new Mock<IDataParameterCollection>(MockBehavior.Strict);

      _dbCommandStub = new Mock<IDbCommand>();
      _dbCommandStub.Setup(stub => stub.CreateParameter()).Returns(_dbDataParameterStub.Object);
      _dbCommandStub.Setup(stub => stub.Parameters).Returns(_dataParameterCollectionMock.Object);
      _dbCommandStub.SetupProperty(stub => stub.CommandText);

      _dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      _dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(_dbCommandStub.Object);
    }

    [Test]
    public void Create_DefaultSchema_WithOrderings ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"));
      var builder = new SelectDbCommandBuilder(
          tableDefinition,
          _selectedColumnsStub.Object,
          _comparedColumnsStrictMock.Object,
          _orderedColumnsStub.Object,
          _sqlDialectStub.Object);

      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table")).Returns("[delimited Table]");

      _comparedColumnsStrictMock.Setup(stub => stub.AddParameters(_dbCommandStub.Object, _sqlDialectStub.Object)).Verifiable();
      _comparedColumnsStrictMock
          .Setup(stub => stub.AppendComparisons(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect) => statement.Append("[ID] = @ID"))
          .Verifiable();

      _orderedColumnsStub.Setup(stub => stub.IsEmpty).Returns(false);
      _orderedColumnsStub
          .Setup(stub => stub.AppendOrderings(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder statement, ISqlDialect _) => statement.Append("[Name] ASC, [City] DESC"));

     var result = builder.Create(_dbCommandFactoryStub.Object);

     _comparedColumnsStrictMock.Verify();
     Assert.That(
          result.CommandText,
          Is.EqualTo(
              "SELECT [Column1], [Column2], [Column3] FROM [delimited Table] WHERE [ID] = @ID ORDER BY [Name] ASC, [City] DESC;"));
    }

    [Test]
    public void Create_CustomSchema_NoOrderings ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition, new EntityNameDefinition("customSchema", "Table"));
      var builder = new SelectDbCommandBuilder(
          tableDefinition,
          _selectedColumnsStub.Object,
          _comparedColumnsStrictMock.Object,
          _orderedColumnsStub.Object,
          _sqlDialectStub.Object);

      _sqlDialectStub.Setup(mock => mock.DelimitIdentifier("customSchema")).Returns("[delimited customSchema]");
      _sqlDialectStub.Setup(mock => mock.DelimitIdentifier("Table")).Returns("[delimited Table]");

      _comparedColumnsStrictMock.Setup(stub => stub.AddParameters(_dbCommandStub.Object, _sqlDialectStub.Object)).Verifiable();
      _comparedColumnsStrictMock
          .Setup(stub => stub.AppendComparisons(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand _, ISqlDialect _) => statement.Append("[ID] = @ID"))
          .Verifiable();

      _orderedColumnsStub.Setup(stub => stub.IsEmpty).Returns(true);

      var result = builder.Create(_dbCommandFactoryStub.Object);

      _comparedColumnsStrictMock.Verify();
      Assert.That(
          result.CommandText, Is.EqualTo("SELECT [Column1], [Column2], [Column3] FROM [delimited customSchema].[delimited Table] WHERE [ID] = @ID;"));
    }
  }
}
