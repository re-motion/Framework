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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class SqlQueryGeneratorTest
  {
    private ISqlPreparationStage _preparationStageMock;
    private IMappingResolutionStage _resolutionStageMock;
    private ISqlGenerationStage _generationStageMock;

    private SqlQueryGenerator _sqlQueryGenerator;

    private QueryModel _queryModel;

    [SetUp]
    public void SetUp ()
    {
      _preparationStageMock = MockRepository.GenerateStrictMock<ISqlPreparationStage> ();
      _resolutionStageMock = MockRepository.GenerateStrictMock<IMappingResolutionStage> ();
      _generationStageMock = MockRepository.GenerateStrictMock<ISqlGenerationStage> ();

      _sqlQueryGenerator = new SqlQueryGenerator (_preparationStageMock, _resolutionStageMock, _generationStageMock);

      _queryModel = QueryModelObjectMother.Create();
    }

    [Test]
    public void CreateSqlQuery ()
    {
      var fakePreparationResult = CreateSqlStatement();
      _preparationStageMock
          .Expect (mock => mock.PrepareSqlStatement (_queryModel, null))
          .Return (fakePreparationResult);
      var fakeResolutionResult = CreateSqlStatement();
      _resolutionStageMock
          .Expect (mock => mock.ResolveSqlStatement (Arg.Is (fakePreparationResult), Arg<MappingResolutionContext>.Is.TypeOf))
          .Return (fakeResolutionResult);
      _generationStageMock
          .Expect (mock => mock.GenerateTextForOuterSqlStatement (Arg<SqlCommandBuilder>.Is.TypeOf, Arg.Is (fakeResolutionResult)))
          .WhenCalled (mi =>
          {
            var sqlCommandBuilder = ((SqlCommandBuilder) mi.Arguments[0]);
            sqlCommandBuilder.Append ("TestTest");
            sqlCommandBuilder.SetInMemoryProjectionBody (Expression.Constant (null));
          });

      var result = _sqlQueryGenerator.CreateSqlQuery (_queryModel);

      _preparationStageMock.VerifyAllExpectations();
      _resolutionStageMock.VerifyAllExpectations();
      _generationStageMock.VerifyAllExpectations();

      Assert.That (result.SqlCommand.CommandText, Is.EqualTo ("TestTest"));
    }

    [Test]
    public void CreateSqlQuery_QueryKindEntity ()
    {
      var selectProjection = CreateEntityDefinitionExpression();
      CheckCreateSqlQuery_SelectedEntityType (typeof (Order), selectProjection);
    }

    [Test]
    public void CreateSqlQuery_QueryKindEntity_WrappedInUnaryExpressions ()
    {
      var selectProjection = Expression.Convert (Expression.Convert (CreateEntityDefinitionExpression (), typeof (object)), typeof (Order));
      CheckCreateSqlQuery_SelectedEntityType (typeof (Order), selectProjection);
    }

    [Test]
    public void CreateSqlQuery_QueryKindOther ()
    {
      var selectProjection = Expression.Constant (null);
      CheckCreateSqlQuery_SelectedEntityType (null, selectProjection);
    }

    [Test]
    public void CreateSqlQuery_NotSupportedException_InPreparationStage ()
    {
      var exception = new NotSupportedException ("Bla.");
      _preparationStageMock
          .Stub (mock => mock.PrepareSqlStatement (_queryModel, null))
          .Throw (exception);

      Assert.That (
          () => _sqlQueryGenerator.CreateSqlQuery (_queryModel),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (
              "There was an error preparing or resolving query 'from Order o in null select null' for SQL generation. Bla."));
    }

    [Test]
    public void CreateSqlQuery_NotSupportedException_InResolutionStage ()
    {
      var exception = new NotSupportedException ("Bla.");
      _preparationStageMock
          .Stub (mock => mock.PrepareSqlStatement (_queryModel, null))
          .Return (CreateSqlStatement ());

      _resolutionStageMock
          .Stub (mock => mock.ResolveSqlStatement (Arg<SqlStatement>.Is.Anything, Arg<MappingResolutionContext>.Is.TypeOf))
          .Throw (exception);

      Assert.That (
          () => _sqlQueryGenerator.CreateSqlQuery (_queryModel),
          Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo (
              "There was an error preparing or resolving query 'from Order o in null select null' for SQL generation. Bla."));
    }

    [Test]
    public void CreateSqlQuery_NotSupportedException_InGenerationStage ()
    {
      var exception = new NotSupportedException ("Bla.");
      _preparationStageMock
          .Stub (mock => mock.PrepareSqlStatement (_queryModel, null))
          .Return (CreateSqlStatement ());
      _resolutionStageMock
          .Stub (mock => mock.ResolveSqlStatement (Arg<SqlStatement>.Is.Anything, Arg<MappingResolutionContext>.Is.TypeOf))
          .Return (CreateSqlStatement ());
      _generationStageMock
          .Stub (mock => mock.GenerateTextForOuterSqlStatement (Arg<SqlCommandBuilder>.Is.TypeOf, Arg<SqlStatement>.Is.Anything))
          .Throw (exception);

      Assert.That (
          () => _sqlQueryGenerator.CreateSqlQuery (_queryModel),
          Throws.TypeOf<NotSupportedException> ().With.Message.EqualTo (
              "There was an error generating SQL for the query 'from Order o in null select null'. Bla."));
    }

    [Test]
    public void CreateSqlQuery_UnmappedItemException_InResolutionStage ()
    {
      var exception = new UnmappedItemException ("Bla.");
      _preparationStageMock
          .Stub (mock => mock.PrepareSqlStatement (_queryModel, null))
          .Return (CreateSqlStatement ());

      _resolutionStageMock
          .Stub (mock => mock.ResolveSqlStatement (Arg<SqlStatement>.Is.Anything, Arg<MappingResolutionContext>.Is.TypeOf))
          .Throw (exception);

      Assert.That (
          () => _sqlQueryGenerator.CreateSqlQuery (_queryModel),
          Throws.TypeOf<UnmappedItemException> ().With.Message.EqualTo (
              "Query 'from Order o in null select null' contains an unmapped item. Bla."));
    }

    private void CheckCreateSqlQuery_SelectedEntityType (Type expectedSelectedEntityType, Expression selectProjection)
    {
      _preparationStageMock
          .Stub (mock => mock.PrepareSqlStatement (_queryModel, null))
          .Return (CreateSqlStatement());

      var fakeResolutionResult = CreateSqlStatement (selectProjection);
      _resolutionStageMock
          .Stub (mock => mock.ResolveSqlStatement (Arg<SqlStatement>.Is.Anything, Arg<MappingResolutionContext>.Is.TypeOf))
          .Return (fakeResolutionResult);

      _generationStageMock
          .Stub (mock => mock.GenerateTextForOuterSqlStatement (Arg<SqlCommandBuilder>.Is.TypeOf, Arg<SqlStatement>.Is.Anything))
          .WhenCalled (mi =>
          {
            var sqlCommandBuilder = ((SqlCommandBuilder) mi.Arguments[0]);
            sqlCommandBuilder.Append ("TestTest");
            sqlCommandBuilder.SetInMemoryProjectionBody (Expression.Constant (null));
          });
      
      var result = _sqlQueryGenerator.CreateSqlQuery (_queryModel);

      Assert.That (result.SelectedEntityType, Is.EqualTo (expectedSelectedEntityType));
    }

    private SqlStatement CreateSqlStatement (Expression selectProjection = null)
    {
      return new SqlStatement (
          new StreamedSequenceInfo (typeof (IQueryable<Order>), Expression.Constant (null, typeof (Order))),
          selectProjection ?? Expression.Constant (null, typeof (Order)),
          new SqlTable[0],
          null,
          null,
          Enumerable.Empty<Ordering> (),
          null,
          false,
          null,
          null,
          Enumerable.Empty<SetOperationCombinedStatement>());
    }

    private SqlEntityDefinitionExpression CreateEntityDefinitionExpression ()
    {
      return new SqlEntityDefinitionExpression (typeof (Order), "t0", "o", e => e.GetColumn (typeof (int), "id", true));
    }
  }
}