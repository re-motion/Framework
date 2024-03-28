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
  public class UnionSelectDbCommandBuilderTest : StandardMappingTest
  {
    private Mock<ISelectedColumnsSpecification> _originalSelectedColumnsStub;
    private Mock<ISqlDialect> _sqlDialectStub;
    private Mock<IDbCommand> _dbCommandStub;
    private Mock<IOrderedColumnsSpecification> _orderedColumnsStub;
    private TableDefinition _table1;
    private TableDefinition _table2;
    private TableDefinition _table3;
    private Mock<ISelectedColumnsSpecification> _fullSelectedColumnsStub;
    private Mock<IDbCommandFactory> _dbCommandFactoryStub;
    private Mock<IComparedColumnsSpecification> _comparedColumnsStrictMock;

    public override void SetUp ()
    {
      base.SetUp();

      _originalSelectedColumnsStub = new Mock<ISelectedColumnsSpecification>();
      _comparedColumnsStrictMock = new Mock<IComparedColumnsSpecification>(MockBehavior.Strict);
      _fullSelectedColumnsStub = new Mock<ISelectedColumnsSpecification>();
      _orderedColumnsStub = new Mock<IOrderedColumnsSpecification>();

      _sqlDialectStub = new Mock<ISqlDialect>();
      _sqlDialectStub.Setup(stub => stub.StatementDelimiter).Returns(";");

      _dbCommandStub = new Mock<IDbCommand>();
      _dbCommandStub.SetupProperty(stub => stub.CommandText);

      _dbCommandFactoryStub = new Mock<IDbCommandFactory>();
      _dbCommandFactoryStub.Setup(stub => stub.CreateDbCommand()).Returns(_dbCommandStub.Object);

      Guid.NewGuid();

      _table1 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table1"));
      _table2 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table2"));
      _table3 = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition("customSchema", "Table3"));
    }

    [Test]
    public void Create_WithOneUnionedTable ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, null, _table1);

      var builder = new UnionSelectDbCommandBuilder(
          unionViewDefinition,
          _originalSelectedColumnsStub.Object,
          _comparedColumnsStrictMock.Object,
          _orderedColumnsStub.Object,
          _sqlDialectStub.Object);

      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("customSchema")).Returns("[delimited customSchema]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table1")).Returns("[delimited Table1]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("FKID")).Returns("[delimited FKID]");
      _sqlDialectStub.Setup(stub => stub.GetParameterName("FKID")).Returns("pFKID");

      _orderedColumnsStub.Setup(stub => stub.UnionWithSelectedColumns(_originalSelectedColumnsStub.Object)).Returns(_fullSelectedColumnsStub.Object);
      _orderedColumnsStub.Setup(stub => stub.IsEmpty).Returns(false);
      _orderedColumnsStub
          .Setup(stub => stub.AppendOrderings(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("[Column1] ASC, [Column2] DESC"));

      var adjustedSelectedColumnsStub = new Mock<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub
          .Setup(stub => stub.AppendProjection(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("[Column1], [Column2], [Column3]"));
      _fullSelectedColumnsStub
          .Setup(stub => stub.AdjustForTable(_table1))
          .Returns(adjustedSelectedColumnsStub.Object);

      _comparedColumnsStrictMock
          .Setup(stub => stub.AddParameters(_dbCommandStub.Object, _sqlDialectStub.Object))
          .Verifiable();
      _comparedColumnsStrictMock
          .Setup(stub => stub.AppendComparisons(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect) => statement.Append("[delimited FKID] = pFKID"))
          .Verifiable();

      var result = builder.Create(_dbCommandFactoryStub.Object);

      Assert.That(
          result.CommandText,
          Is.EqualTo("SELECT [Column1], [Column2], [Column3] FROM [delimited Table1] WHERE [delimited FKID] = pFKID ORDER BY [Column1] ASC, [Column2] DESC;"));
      _sqlDialectStub.Verify();
      _comparedColumnsStrictMock.Verify();
    }

    [Test]
    public void Create_WithSeveralUnionedTables ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          null,
          _table1,
          _table2,
          _table3);

      var builder = new UnionSelectDbCommandBuilder(
          unionViewDefinition,
          _originalSelectedColumnsStub.Object,
          _comparedColumnsStrictMock.Object,
          _orderedColumnsStub.Object,
          _sqlDialectStub.Object);

      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("customSchema")).Returns("[delimited customSchema]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table1")).Returns("[delimited Table1]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table2")).Returns("[delimited Table2]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Table3")).Returns("[delimited Table3]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("FKID")).Returns("[delimited FKID]");
      _sqlDialectStub.Setup(stub => stub.GetParameterName("FKID")).Returns("pFKID");

      _orderedColumnsStub.Setup(stub => stub.UnionWithSelectedColumns(_originalSelectedColumnsStub.Object)).Returns(_fullSelectedColumnsStub.Object);
      _orderedColumnsStub.Setup(stub => stub.IsEmpty).Returns(false);
      _orderedColumnsStub
          .Setup(stub => stub.AppendOrderings(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("[Column1] ASC, [Column2] DESC"));

      var adjustedSelectedColumnsStub1 = new Mock<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub1
          .Setup(stub => stub.AppendProjection(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("[Column1], [Column2], [Column3]"));
      var adjustedSelectedColumnsStub2 = new Mock<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub2
          .Setup(stub => stub.AppendProjection(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("[Column1], NULL, [Column3]"));
      var adjustedSelectedColumnsStub3 = new Mock<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub3
          .Setup(stub => stub.AppendProjection(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("NULL, [Column2], [Column3]"));
      _fullSelectedColumnsStub
          .Setup(stub => stub.AdjustForTable(_table1))
          .Returns(adjustedSelectedColumnsStub1.Object);
      _fullSelectedColumnsStub
          .Setup(stub => stub.AdjustForTable(_table2))
          .Returns(adjustedSelectedColumnsStub2.Object);
      _fullSelectedColumnsStub
          .Setup(stub => stub.AdjustForTable(_table3))
          .Returns(adjustedSelectedColumnsStub3.Object);

      _fullSelectedColumnsStub
          .Setup(stub => stub.AppendProjection(It.IsAny<StringBuilder>(), _sqlDialectStub.Object))
          .Callback((StringBuilder stringBuilder, ISqlDialect sqlDialect) => stringBuilder.Append("[Column1], [Column2], [Column3]"));

      _comparedColumnsStrictMock
          .Setup(stub => stub.AddParameters(_dbCommandStub.Object, _sqlDialectStub.Object))
          .Verifiable();

      _comparedColumnsStrictMock
          .Setup(stub => stub.AppendComparisons(It.IsAny<StringBuilder>(), _dbCommandStub.Object, _sqlDialectStub.Object))
          .Callback((StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect) => statement.Append("[delimited FKID] = pFKID"));

      var result = builder.Create(_dbCommandFactoryStub.Object);

      Assert.That(
          result.CommandText,
          Is.EqualTo(
              "SELECT [Column1], [Column2], [Column3] FROM [delimited Table1] WHERE [delimited FKID] = pFKID UNION ALL "
              + "SELECT [Column1], NULL, [Column3] FROM [delimited Table2] WHERE [delimited FKID] = pFKID UNION ALL "
              + "SELECT NULL, [Column2], [Column3] FROM [delimited customSchema].[delimited Table3] WHERE [delimited FKID] = pFKID "
              + "ORDER BY [Column1] ASC, [Column2] DESC;"));
      _sqlDialectStub.Verify();
      _comparedColumnsStrictMock.Verify();
    }
  }
}
