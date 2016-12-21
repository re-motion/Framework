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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class UnionSelectDbCommandBuilderTest : StandardMappingTest
  {
    private ISelectedColumnsSpecification _originalSelectedColumnsStub;
    private ISqlDialect _sqlDialectStub;
    private IDbCommand _dbCommandStub;
    private IOrderedColumnsSpecification _orderedColumnsStub;
    private TableDefinition _table1;
    private TableDefinition _table2;
    private TableDefinition _table3;
    private ISelectedColumnsSpecification _fullSelectedColumnsStub;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;
    private IComparedColumnsSpecification _comparedColumnsStrictMock;

    public override void SetUp ()
    {
      base.SetUp();

      _originalSelectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification> ();
      _comparedColumnsStrictMock = MockRepository.GenerateStrictMock<IComparedColumnsSpecification>();
      _fullSelectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification> ();
      _orderedColumnsStub = MockRepository.GenerateStub<IOrderedColumnsSpecification> ();
      
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.StatementDelimiter).Return (";");

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      
      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand()).Return (_dbCommandStub);

      Guid.NewGuid();

      _table1 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      _table2 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table2"));
      _table3 = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table3"));
    }

    [Test]
    public void Create_WithOneUnionedTable ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, null, _table1);

      var builder = new UnionSelectDbCommandBuilder (
          unionViewDefinition,
          _originalSelectedColumnsStub,
          _comparedColumnsStrictMock,
          _orderedColumnsStub,
          _sqlDialectStub);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[delimited customSchema]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table1")).Return ("[delimited Table1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FKID")).Return ("[delimited FKID]");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("FKID")).Return ("pFKID");

      _orderedColumnsStub.Stub (stub => stub.UnionWithSelectedColumns (_originalSelectedColumnsStub)).Return (_fullSelectedColumnsStub);
      _orderedColumnsStub.Stub (stub => stub.IsEmpty).Return (false);
      _orderedColumnsStub
          .Stub (stub => stub.AppendOrderings (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1] ASC, [Column2] DESC"));

      var adjustedSelectedColumnsStub = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));
      _fullSelectedColumnsStub
          .Stub (stub => stub.AdjustForTable (_table1))
          .Return (adjustedSelectedColumnsStub);

      _comparedColumnsStrictMock
          .Expect (stub => stub.AddParameters (_dbCommandStub, _sqlDialectStub))
          .Repeat.Once();
      _comparedColumnsStrictMock
          .Expect (stub => stub.AppendComparisons (
              Arg<StringBuilder>.Is.Anything,
              Arg.Is (_dbCommandStub),
              Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[delimited FKID] = pFKID"))
          .Repeat.Once();
      _comparedColumnsStrictMock.Replay();
      
      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (
          result.CommandText,
          Is.EqualTo ("SELECT [Column1], [Column2], [Column3] FROM [delimited Table1] WHERE [delimited FKID] = pFKID ORDER BY [Column1] ASC, [Column2] DESC;"));
      _sqlDialectStub.VerifyAllExpectations ();
      _comparedColumnsStrictMock.VerifyAllExpectations();
    }

    [Test]
    public void Create_WithSeveralUnionedTables ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          null,
          _table1,
          _table2,
          _table3);

      var builder = new UnionSelectDbCommandBuilder (
          unionViewDefinition,
          _originalSelectedColumnsStub,
          _comparedColumnsStrictMock,
          _orderedColumnsStub,
          _sqlDialectStub);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[delimited customSchema]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table1")).Return ("[delimited Table1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table2")).Return ("[delimited Table2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table3")).Return ("[delimited Table3]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("FKID")).Return ("[delimited FKID]");
      _sqlDialectStub.Stub (stub => stub.GetParameterName ("FKID")).Return ("pFKID");

      _orderedColumnsStub.Stub (stub => stub.UnionWithSelectedColumns (_originalSelectedColumnsStub)).Return (_fullSelectedColumnsStub);
      _orderedColumnsStub.Stub (stub => stub.IsEmpty).Return (false);
      _orderedColumnsStub
          .Stub (stub => stub.AppendOrderings (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1] ASC, [Column2] DESC"));


      var adjustedSelectedColumnsStub1 = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub1
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));
      var adjustedSelectedColumnsStub2 = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub2
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], NULL, [Column3]"));
      var adjustedSelectedColumnsStub3 = MockRepository.GenerateStub<ISelectedColumnsSpecification>();
      adjustedSelectedColumnsStub3
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("NULL, [Column2], [Column3]"));
      _fullSelectedColumnsStub
          .Stub (stub => stub.AdjustForTable (_table1))
          .Return (adjustedSelectedColumnsStub1);
      _fullSelectedColumnsStub
          .Stub (stub => stub.AdjustForTable (_table2))
          .Return (adjustedSelectedColumnsStub2);
      _fullSelectedColumnsStub
          .Stub (stub => stub.AdjustForTable (_table3))
          .Return (adjustedSelectedColumnsStub3);


      _fullSelectedColumnsStub
          .Stub (stub => stub.AppendProjection (Arg<StringBuilder>.Is.Anything, Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1], [Column2], [Column3]"));

      _comparedColumnsStrictMock
          .Expect (stub => stub.AddParameters (_dbCommandStub, _sqlDialectStub))
          .Repeat.Once ();
      _comparedColumnsStrictMock
          .Expect (stub => stub.AppendComparisons (
              Arg<StringBuilder>.Is.Anything,
              Arg.Is (_dbCommandStub),
              Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[delimited FKID] = pFKID"))
          .Repeat.Times (3);
      _comparedColumnsStrictMock.Replay ();

      var result = builder.Create (_commandExecutionContextStub);

      Assert.That (
          result.CommandText,
          Is.EqualTo (
              "SELECT [Column1], [Column2], [Column3] FROM [delimited Table1] WHERE [delimited FKID] = pFKID UNION ALL "
              + "SELECT [Column1], NULL, [Column3] FROM [delimited Table2] WHERE [delimited FKID] = pFKID UNION ALL "
              + "SELECT NULL, [Column2], [Column3] FROM [delimited customSchema].[delimited Table3] WHERE [delimited FKID] = pFKID "
              + "ORDER BY [Column1] ASC, [Column2] DESC;"));
      _sqlDialectStub.VerifyAllExpectations();
      _comparedColumnsStrictMock.VerifyAllExpectations();
    }
  }
}