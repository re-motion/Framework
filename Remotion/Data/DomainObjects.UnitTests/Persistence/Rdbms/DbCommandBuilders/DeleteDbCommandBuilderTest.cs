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
  public class DeleteDbCommandBuilderTest : StandardMappingTest
  {
    private Mock<IComparedColumnsSpecification> _comparedColumnsSpecificationStrictMock;
    private Mock<ISqlDialect> _sqlDialectStub;
    private Mock<IDbDataParameter> _dbDataParameterStub;
    private Mock<IDataParameterCollection> _dataParameterCollectionMock;
    private Mock<IDbCommand> _dbCommandStub;
    private Mock<IDbCommandFactory> _dbCommandFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _comparedColumnsSpecificationStrictMock = new Mock<IComparedColumnsSpecification>(MockBehavior.Strict);

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
    public void Create_WithDefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"));
      var builder = new DeleteDbCommandBuilder(
          tableDefinition,
          _comparedColumnsSpecificationStrictMock.Object,
          _sqlDialectStub.Object);

      _sqlDialectStub.Setup(mock => mock.DelimitIdentifier("Table")).Returns("[delimited Table]");

      _comparedColumnsSpecificationStrictMock.Setup(stub => stub.AddParameters(_dbCommandStub.Object, _sqlDialectStub.Object)).Verifiable();
      _comparedColumnsSpecificationStrictMock
          .Setup(stub => stub.AppendComparisons(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect) => statement.Append("[ID] = @ID"))
          .Verifiable();

      var result = builder.Create(_dbCommandFactoryStub.Object);

      _comparedColumnsSpecificationStrictMock.Verify();
      Assert.That(result.CommandText, Is.EqualTo("DELETE FROM [delimited Table] WHERE [ID] = @ID;"));
    }

    [Test]
    public void Create_WithCustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition, new EntityNameDefinition("customSchema", "Table"));
      var builder = new DeleteDbCommandBuilder(
          tableDefinition,
          _comparedColumnsSpecificationStrictMock.Object,
          _sqlDialectStub.Object);

      _sqlDialectStub.Setup(mock => mock.DelimitIdentifier("Table")).Returns("[delimited Table]").Verifiable();
      _sqlDialectStub.Setup(mock => mock.DelimitIdentifier("customSchema")).Returns("[delimited customSchema]").Verifiable();

      _comparedColumnsSpecificationStrictMock.Setup(stub => stub.AddParameters(_dbCommandStub.Object, _sqlDialectStub.Object)).Verifiable();
      _comparedColumnsSpecificationStrictMock
          .Setup(stub => stub.AppendComparisons(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect) => statement.Append("[ID] = @ID"))
          .Verifiable();

      var result = builder.Create(_dbCommandFactoryStub.Object);

      _comparedColumnsSpecificationStrictMock.Verify();
      Assert.That(result.CommandText, Is.EqualTo("DELETE FROM [delimited customSchema].[delimited Table] WHERE [ID] = @ID;"));
    }
  }
}
