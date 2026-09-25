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
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace Remotion.Data.DomainObjects.Queries;

/// <summary>
/// <see cref="IEvaluatableExpressionFilter"/> that prevents evaluation by ref values.
/// </summary>
/// <remarks>
/// By-ref values are not evaluatable in an expression tree and thus not evaluatable by re-linq.
/// With C# 14 first class span types, some expressions are retargeted to span overloads.
/// While the usage get rewritten, it is also necessary to disable re-linqs evaluation as
/// it tries to constant fold the expression otherwise, which causes runtime exceptions.
/// </remarks>
public class ByRefLikeAwareEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
{
  /// <inheritdoc />
  public override bool IsEvaluatableMethodCall (MethodCallExpression node)
  {
    ArgumentNullException.ThrowIfNull(node);

    return !node.Type.IsByRefLike && base.IsEvaluatableMethodCall(node);
  }
}
