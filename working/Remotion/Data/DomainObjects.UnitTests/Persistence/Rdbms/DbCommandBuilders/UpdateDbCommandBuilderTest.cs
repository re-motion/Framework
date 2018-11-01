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
  public class UpdateDbCommandBuilderTest : StandardMappingTest
  {
    private IComparedColumnsSpecification _comparedColumnsSpecificationStrictMock;
    private IUpdatedColumnsSpecification _updatedColumnsSpecificationStub;

    private ISqlDialect _sqlDialectStub;
    private IDbDataParameter _dbDataParameterStub;
    private IDataParameterCollection _dataParameterCollectionMock;

    private IDbCommand _dbCommandStub;

    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _comparedColumnsSpecificationStrictMock = MockRepository.GenerateStrictMock<IComparedColumnsSpecification>();
      _updatedColumnsSpecificationStub = MockRepository.GenerateStub<IUpdatedColumnsSpecification>();

      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.StatementDelimiter).Return (";");
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dataParameterCollectionMock = MockRepository.GenerateStrictMock<IDataParameterCollection>();

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub);
      _dbCommandStub.Stub (stub => stub.Parameters).Return (_dataParameterCollectionMock);

      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub.Stub (stub => stub.CreateDbCommand()).Return (_dbCommandStub);
    }

    [Test]
    public void Create_WithDefaultSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      var builder = new UpdateDbCommandBuilder (
          tableDefinition,
          _updatedColumnsSpecificationStub,
          _comparedColumnsSpecificationStrictMock,
          _sqlDialectStub);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table")).Return ("[delimited Table]");

      _updatedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnValueAssignments (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1] = 5, [Column2] = 'test', [Column3] = true"));

      _comparedColumnsSpecificationStrictMock.Expect (stub => stub.AddParameters (_dbCommandStub, _sqlDialectStub));
      _comparedColumnsSpecificationStrictMock
          .Expect (
              stub => stub.AppendComparisons (
                  Arg<StringBuilder>.Is.Anything,
                  Arg.Is (_dbCommandStub),
                  Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));
      _comparedColumnsSpecificationStrictMock.Replay ();

      var result = builder.Create (_commandExecutionContextStub);

      _comparedColumnsSpecificationStrictMock.VerifyAllExpectations();
      Assert.That (
          result.CommandText, Is.EqualTo ("UPDATE [delimited Table] SET [Column1] = 5, [Column2] = 'test', [Column3] = true WHERE [ID] = @ID;"));
    }

    [Test]
    public void Create_WithCustomSchema ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition ("customSchema", "Table"));
      var builder = new UpdateDbCommandBuilder (
          tableDefinition,
          _updatedColumnsSpecificationStub,
          _comparedColumnsSpecificationStrictMock,
          _sqlDialectStub);

      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Table")).Return ("[delimited Table]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("customSchema")).Return ("[delimited customSchema]");

      _updatedColumnsSpecificationStub
          .Stub (stub => stub.AppendColumnValueAssignments (Arg<StringBuilder>.Is.Anything, Arg.Is (_dbCommandStub), Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[Column1] = 5, [Column2] = 'test', [Column3] = true"));

      _comparedColumnsSpecificationStrictMock.Expect (stub => stub.AddParameters (_dbCommandStub, _sqlDialectStub));
      _comparedColumnsSpecificationStrictMock
          .Expect (
              stub => stub.AppendComparisons (
                  Arg<StringBuilder>.Is.Anything,
                  Arg.Is (_dbCommandStub),
                  Arg.Is (_sqlDialectStub)))
          .WhenCalled (mi => ((StringBuilder) mi.Arguments[0]).Append ("[ID] = @ID"));
      _comparedColumnsSpecificationStrictMock.Replay ();

      var result = builder.Create (_commandExecutionContextStub);

      _comparedColumnsSpecificationStrictMock.VerifyAllExpectations();
      Assert.That (
          result.CommandText,
          Is.EqualTo (
              "UPDATE [delimited customSchema].[delimited Table] SET [Column1] = 5, [Column2] = 'test', [Column3] = true WHERE [ID] = @ID;"));
    }
  }
}