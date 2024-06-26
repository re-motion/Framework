// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.Moq.UnitTesting.Threading
{
  /// <summary>
  /// A helper class for testing locking decorators.
  /// </summary>
  /// <typeparam name="T">The interface type of the decorator.</typeparam>
  partial class LockingDecoratorTestHelper<T>
      where T : class
  {
    private readonly T _lockingDecorator;
    private readonly object _lockObject;
    private readonly Mock<T> _innerMock;

    public LockingDecoratorTestHelper (T lockingDecorator, object lockObject, Mock<T> innerMock)
    {
      ArgumentUtility.CheckNotNull("lockingDecorator", lockingDecorator);
      ArgumentUtility.CheckNotNull("lockObject", lockObject);
      ArgumentUtility.CheckNotNull("innerMock", innerMock);

      _lockingDecorator = lockingDecorator;
      _lockObject = lockObject;
      _innerMock = innerMock;
    }

    public void ExpectSynchronizedDelegation<TResult> (Expression<Func<T, TResult>> action, TResult fakeResult)
        where TResult : notnull
    {
      ArgumentUtility.CheckNotNull("action", action);
      ArgumentUtility.CheckNotNull("fakeResult", fakeResult);

      ExpectSynchronizedDelegation(action, fakeResult, r => Assert.That(r, Is.EqualTo(fakeResult)));
    }

    public void ExpectSynchronizedDelegation<TResult> (Expression<Func<T, TResult>> action, TResult fakeResult, Action<TResult> resultChecker) where TResult : notnull
    {
      ExpectSynchronizedDelegation(action, action.Compile(), fakeResult, resultChecker);
    }

    public void ExpectSynchronizedDelegation<TResult> (Expression<Func<T, TResult>> expectAction, Func<T, TResult> action, TResult fakeResult, Action<TResult> resultChecker)
        where TResult : notnull
    {
      _innerMock
          .Setup(expectAction)
          .Returns(fakeResult)
          .Callback(new InvocationAction(_ => LockTestHelper.CheckLockIsHeld(_lockObject)))
          .Verifiable();

      var actualResult = action.Invoke(_lockingDecorator);

      _innerMock.Verify();
      resultChecker(actualResult);
    }

    public void ExpectSynchronizedDelegation (Expression<Action<T>> action)
    {
      ExpectSynchronizedDelegation(action, action.Compile());
    }

    public void ExpectSynchronizedDelegation (Expression<Action<T>> expectAction, Action<T> action)
    {
      ArgumentUtility.CheckNotNull("action", action);

      _innerMock
          .Setup(expectAction)
          .Callback(new InvocationAction(_ => LockTestHelper.CheckLockIsHeld(_lockObject)))
          .Verifiable();

      action.Invoke(_lockingDecorator);

      _innerMock.Verify();
    }
  }
}
