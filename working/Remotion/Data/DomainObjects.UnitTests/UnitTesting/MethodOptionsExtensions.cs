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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.UnitTesting
{
  public static class MethodOptionsExtensions
  {
    /// <summary>
    /// Provides support for ordered Rhino.Mocks expectations without dedicated <see cref="MockRepository"/> instance. Create an 
    /// <see cref="OrderedExpectationCounter"/>, then call this method with that counter for all expectations that should occur 
    /// in the same order as declared.
    /// Uses <see cref="IMethodOptions{T}.WhenCalled"/> internally; use <see cref="WhenCalledOrdered{T}"/> when you need to specify your own
    /// <see cref="IMethodOptions{T}.WhenCalled"/> handler.
    /// </summary>
    public static IMethodOptions<T> Ordered<T> (this IMethodOptions<T> options, OrderedExpectationCounter counter, string message = null)
    {
      return WhenCalledOrdered (options, counter, mi => { }, message);
    }

    /// <summary>
    /// Provides support for ordered Rhino.Mocks expectations without dedicated <see cref="MockRepository"/> instance. Create an 
    /// <see cref="OrderedExpectationCounter"/>, then call this method with that counter for all expectations that should occur 
    /// in the same order as declared.
    /// Use this method rather then <see cref="Ordered{T}"/> when you need to specify your own <see cref="IMethodOptions{T}.WhenCalled"/> handler.
    /// </summary>
    public static IMethodOptions<T> WhenCalledOrdered<T> (
        this IMethodOptions<T> options, 
        OrderedExpectationCounter counter, 
        Action<MethodInvocation> whenCalledAction,
        string message = null)
    {
      var expectedPosition = counter.GetNextExpectedPosition ();
      return options.WhenCalled (
          mi =>
          {
            counter.CheckPosition (mi.Method.ToString(), expectedPosition, message);
            whenCalledAction (mi);
          });
    }

    public static IMethodOptions<T> WhenCalledWithCurrentTransaction<T> (
        this IMethodOptions<T> options, ClientTransaction expectedTransaction, Action<MethodInvocation> whenCalledAction)
    {
      return options.WhenCalled (
          mi =>
          {
            Assert.That (ClientTransaction.Current, Is.SameAs (expectedTransaction));
            whenCalledAction (mi);
          });
    }

    public static IMethodOptions<T> WithCurrentTransaction<T> (this IMethodOptions<T> options, ClientTransaction expectedTransaction)
    {
      return options.WhenCalled (mi => Assert.That (ClientTransaction.Current, Is.SameAs (expectedTransaction)));
    }
  }
}