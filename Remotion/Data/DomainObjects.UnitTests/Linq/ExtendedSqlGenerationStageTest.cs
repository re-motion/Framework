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
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class ExtendedSqlGenerationStageTest: StandardMappingTest
  {
    private ExtendedSqlGenerationStage _stage;

    private SqlCommandBuilder _commandBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _stage = new ExtendedSqlGenerationStage();
    
      _commandBuilder = new SqlCommandBuilder ();
    }

    [Test]
    public void GenerateTextForOuterSelectExpression_UsesExtendedVisitor ()
    {
      var constructorInfo = typeof (ObjectID).GetConstructor (new[] { typeof (string), typeof (object) });
      Assertion.IsNotNull (constructorInfo);
      var newObjectIDExpression = Expression.New (
          constructorInfo,
          new SqlColumnDefinitionExpression (typeof (string), "t0", "CustomerClassID", false),
          Expression.Convert (new SqlColumnDefinitionExpression (typeof (Guid), "t0", "CustomerID", false), typeof (object)));
      var compoundExpression = NamedExpression.CreateNewExpressionWithNamedArguments (newObjectIDExpression);

      var someSetOperationsMode = SetOperationsMode.StatementIsSetCombined;
      _stage.GenerateTextForOuterSelectExpression (_commandBuilder, compoundExpression, someSetOperationsMode);

      Assert.That (_commandBuilder.GetInMemoryProjectionBody().NodeType, Is.EqualTo (ExpressionType.Call));
      var getObjectIDOrNullMethod =
          MemberInfoFromExpressionUtility.GetMethod (() => ExtendedSqlGeneratingOuterSelectExpressionVisitor.GetObjectIDOrNull (null, null));
      Assert.That (((MethodCallExpression) _commandBuilder.GetInMemoryProjectionBody()).Method, Is.EqualTo (getObjectIDOrNullMethod));
    } 

    [Test]
    public void GenerateTextForOuterSelectExpression_PassesOnSetOperationsMode ()
    {
      var stage = new DefaultSqlGenerationStage();

      var projectionWithMethodCall = Expression.Call (
          MemberInfoFromExpressionUtility.GetMethod (() => SomeMethod (null)),
          new NamedExpression (null, Expression.Constant ("")));

      Assert.That (
          () => stage.GenerateTextForOuterSelectExpression (_commandBuilder, projectionWithMethodCall, SetOperationsMode.StatementIsSetCombined),
          Throws.TypeOf<NotSupportedException>());
      Assert.That (
          () => stage.GenerateTextForOuterSelectExpression (_commandBuilder, projectionWithMethodCall, SetOperationsMode.StatementIsNotSetCombined),
          Throws.Nothing);
    }

    private static string SomeMethod (string s)
    {
      throw new NotImplementedException();
    }
  }
}