// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace Remotion.Development.Moq.UnitTesting
{
  /// <summary>
  /// Provides functionality for testing decorator methods that do nothing else but forward to the equivalent methods on a decorated object.
  /// </summary>
  /// <typeparam name="TInterface">The type of the interface.</typeparam>
  partial class DecoratorTestHelper<TInterface>
      where TInterface : class
  {
    private readonly TInterface _decorator;
    private Mock<TInterface> _decoratedMock;

    public DecoratorTestHelper (TInterface decorator, Mock<TInterface> decoratedMock)
    {
      if (decorator == null)
        throw new ArgumentNullException("decorator");

      if (decoratedMock == null)
        throw new ArgumentNullException("decoratedMock");

      _decorator = decorator;
      _decoratedMock = decoratedMock;
    }

    public void CheckDelegation<TR> (Expression<Func<TInterface, TR>> mockSetupExpression, TR fakeResult)
    {
      CheckDelegation(mockSetupExpression, fakeResult, result => Assert.That(result, Is.EqualTo(fakeResult)));
    }

    public void CheckDelegation<TR> (Expression<Func<TInterface, TR>> mockSetupExpression, TR fakeResult, Action<TR> decoratorResultChecker)
    {
      _decoratedMock.Setup(mockSetupExpression).Returns(fakeResult).Verifiable();

      var result = mockSetupExpression.Compile().Invoke(_decorator);

      _decoratedMock.Verify();
      decoratorResultChecker(result);
    }

    public void CheckDelegation (Expression<Action<TInterface>> mockSetupExpression)
    {
      _decoratedMock.Setup(mockSetupExpression).Verifiable();

      mockSetupExpression.Compile().Invoke(_decorator);

      _decoratedMock.Verify();
    }

    public void CheckDelegationWithContinuation<TR> (
        Expression<Func<TInterface, TR>> mockSetupExpression,
        TR fakeResult,
        Func<IInvocation, TR> whenCalled)
    {
      _decoratedMock.Setup(mockSetupExpression).Returns(fakeResult).Callback(whenCalled).Verifiable();

      var result = mockSetupExpression.Compile().Invoke(_decorator);

      _decoratedMock.Verify();
      Assert.That(result, Is.EqualTo(fakeResult));
    }

    public void CheckDelegationWithContinuation (
        Expression<Action<TInterface>> mockSetupExpression,
        Action<IInvocation> whenCalled)
    {
      _decoratedMock.Setup(mockSetupExpression).Callback(whenCalled).Verifiable();

      mockSetupExpression.Compile().Invoke(_decorator);

      _decoratedMock.Verify();
    }
  }
}
