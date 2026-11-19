// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Queries;

[TestFixture]
public class ByRefLikeAwareEvaluatableExpressionFilterTest
{
  private ByRefLikeAwareEvaluatableExpressionFilter _filter;

  [SetUp]
  public void SetUp ()
  {
    _filter = new ByRefLikeAwareEvaluatableExpressionFilter();
  }

  [Test]
  public void IsEvaluatableMethodCall_MethodCallReturnsByRefLikeType_ReturnsFalse ()
  {
    var asSpanMethod = typeof(MemoryExtensions).GetMethod(nameof(MemoryExtensions.AsSpan), new[] { typeof(string) })!;
    var methodCallExpression = Expression.Call(asSpanMethod, Expression.Constant("test"));
    Assert.That(methodCallExpression.Type.IsByRefLike, Is.True);

    var result = _filter.IsEvaluatableMethodCall(methodCallExpression);

    Assert.That(result, Is.False);
  }

  [Test]
  public void IsEvaluatableMethodCall_MethodCallReturnsNonByRefLikeType_ReturnsTrue ()
  {
    var concatMethod = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) })!;
    var methodCallExpression = Expression.Call(concatMethod, Expression.Constant("a"), Expression.Constant("b"));
    Assert.That(methodCallExpression.Type.IsByRefLike, Is.False);

    var result = _filter.IsEvaluatableMethodCall(methodCallExpression);

    Assert.That(result, Is.True);
  }
}
