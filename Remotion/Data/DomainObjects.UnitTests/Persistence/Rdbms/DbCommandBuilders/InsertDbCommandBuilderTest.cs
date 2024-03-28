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
  public class InsertDbCommandBuilderTest : StandardMappingTest
  {
    private Mock<IInsertedColumnsSpecification> _insertedColumnsSpecificationStub;
    private Mock<ISqlDialect> _sqlDialectStub;
    private Mock<IDbCommand> _dbCommandStub;
    private Mock<IDbDataParameter> _dbDataParameterStub;
    private Mock<IDataParameterCollection> _dataParameterCollectionMock;
    private Mock<IDbCommandFactory> _dbCommandFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _insertedColumnsSpecificationStub = new Mock<IInsertedColumnsSpecification>();

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
    public void Create_DefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"));
      var builder = new InsertDbCommandBuilder(tableDefinition, _insertedColumnsSpecificationStub.Object, _sqlDialectStub.Object);

      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table")).Returns("[delimited Table]");

      _insertedColumnsSpecificationStub
          .Setup(stub => stub.AppendColumnNames(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand dbCommand, ISqlDialect sqlDialect) => statement.Append("[Column1], [Column2], [Column3]"));
      _insertedColumnsSpecificationStub
          .Setup(stub => stub.AppendColumnValues(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand dbCommand, ISqlDialect sqlDialect) => statement.Append("5, 'test', true"));

      var result = builder.Create(_dbCommandFactoryStub.Object);

      Assert.That(result.CommandText, Is.EqualTo("INSERT INTO [delimited Table] ([Column1], [Column2], [Column3]) VALUES (5, 'test', true);"));
    }

    [Test]
    public void Create_CustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customSchema", "Table"));
      var builder = new InsertDbCommandBuilder(tableDefinition, _insertedColumnsSpecificationStub.Object, _sqlDialectStub.Object);

      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("customSchema")).Returns("[delimited customSchema]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table")).Returns("[delimited Table]");

      _insertedColumnsSpecificationStub
          .Setup(stub => stub.AppendColumnNames(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand dbCommand, ISqlDialect sqlDialect) => statement.Append("[Column1], [Column2], [Column3]"));
      _insertedColumnsSpecificationStub
          .Setup(stub => stub.AppendColumnValues(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand dbCommand, ISqlDialect sqlDialect) => statement.Append("5, 'test', true"));

      var result = builder.Create(_dbCommandFactoryStub.Object);

      Assert.That(result.CommandText, Is.EqualTo("INSERT INTO [delimited customSchema].[delimited Table] ([Column1], [Column2], [Column3]) VALUES (5, 'test', true);"));
    }



  }
}
