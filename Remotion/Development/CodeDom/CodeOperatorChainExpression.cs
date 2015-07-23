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
using System.CodeDom;

namespace Remotion.Development.CodeDom
{

public class CodeOperatorChainExpression: CodeBinaryOperatorExpression
{
  public CodeOperatorChainExpression (CodeBinaryOperatorType binaryOperator, params CodeExpression[] expressions)
    : base ()
  {
    CodeBinaryOperatorExpression binaryOpEx = this;
    for (int i = 0; i < (expressions.Length - 2); ++i)
    {
      CodeExpression condition = expressions[i];
      binaryOpEx.Left = condition;
      binaryOpEx.Operator = binaryOperator;
      binaryOpEx.Right = new CodeBinaryOperatorExpression ();
      binaryOpEx = (CodeBinaryOperatorExpression) binaryOpEx.Right;
    }
    binaryOpEx.Left = expressions[expressions.Length - 2];
    binaryOpEx.Operator = binaryOperator;
    binaryOpEx.Right = expressions[expressions.Length - 1];
  }
}

public class CodeBooleanAndExpression: CodeOperatorChainExpression
{
  public CodeBooleanAndExpression (params CodeExpression[] conditions)
    : base (CodeBinaryOperatorType.BooleanAnd, conditions)
  {
  }
}

public class CodeBooleanOrExpression: CodeOperatorChainExpression
{
  public CodeBooleanOrExpression (params CodeExpression[] conditions)
    : base (CodeBinaryOperatorType.BooleanOr, conditions)
  {
  }
}

}
