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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Linq.Development.UnitTesting.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Remotion.Linq.SqlBackend.Development.UnitTesting;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class ExtendedSqlGeneratingOuterSelectExpressionVisitorTest : StandardMappingTest
  {
    private ISqlGenerationStage _stageMock;
    private SqlCommandBuilder _commandBuilder;
    private SetOperationsMode _someSetOperationsMode;

    public override void SetUp ()
    {
      base.SetUp();

      _stageMock = MockRepository.GenerateStrictMock<ISqlGenerationStage> ();
      _commandBuilder = new SqlCommandBuilder ();

      _someSetOperationsMode = SetOperationsMode.StatementIsSetCombined;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
        "Queries selecting collections are not supported because SQL is not well-suited to returning collections.", 
        MatchType = MessageMatch.Contains)]
    public void GenerateSql_Collection ()
    {
      var expression = Expression.Constant (new Order[] { });
      ExtendedSqlGeneratingOuterSelectExpressionVisitor.GenerateSql (expression, _commandBuilder, _stageMock, _someSetOperationsMode);
    }

    [Test]
    public void GetObjectIDOrNull_NotNull ()
    {
      var result = ExtendedSqlGeneratingOuterSelectExpressionVisitor.GetObjectIDOrNull (DomainObjectIDs.Order1.ClassID, DomainObjectIDs.Order1.Value);

      Assert.That (result, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void GetObjectIDOrNull_Null ()
    {
      var result = ExtendedSqlGeneratingOuterSelectExpressionVisitor.GetObjectIDOrNull (DomainObjectIDs.Order1.ClassID, null);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void VisitNewExpression_ForObjectID_GeneratesNullCheckInInMemoryProjection ()
    {
      var constructorInfo = typeof (ObjectID).GetConstructor (new[] { typeof (string), typeof (object) });
      Assertion.IsNotNull (constructorInfo);
      var newObjectIDExpression = Expression.New (
          constructorInfo,
          new SqlColumnDefinitionExpression (typeof (string), "t0", "CustomerClassID", false),
          Expression.Convert (new SqlColumnDefinitionExpression (typeof (Guid), "t0", "CustomerID", false), typeof (object)));
      var compoundExpression = NamedExpression.CreateNewExpressionWithNamedArguments (newObjectIDExpression);

      ExtendedSqlGeneratingOuterSelectExpressionVisitor.GenerateSql (compoundExpression, _commandBuilder, _stageMock, _someSetOperationsMode);

      Assert.That (_commandBuilder.GetCommandText(), Is.EqualTo ("[t0].[CustomerClassID] AS [m0],[t0].[CustomerID] AS [m1]"));

      Expression<Func<IDatabaseResultRow, ObjectID>> expectedInMemoryProjection =
          row => ExtendedSqlGeneratingOuterSelectExpressionVisitor.GetObjectIDOrNull (
              row.GetValue<string> (new ColumnID ("m0", 0)), 
              row.GetValue<object> (new ColumnID ("m1", 1)));
      var expectedInMemoryProjectionBody = PartialEvaluatingExpressionVisitor.EvaluateIndependentSubtrees (
          expectedInMemoryProjection.Body,
          new TestEvaluatableExpressionFilter());
      SqlExpressionTreeComparer.CheckAreEqualTrees (expectedInMemoryProjectionBody, _commandBuilder.GetInMemoryProjectionBody());
    } 
  }
}