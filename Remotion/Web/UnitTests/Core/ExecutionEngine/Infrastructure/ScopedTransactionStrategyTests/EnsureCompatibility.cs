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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class EnsureCompatibility : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;
    }

    [Test]
    public void Test ()
    {
      var object1 = new object();
      var object2 = new object();

      var sequence = new VerifiableSequence();
      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetInParameters()).Returns(new[] { object1, object2 }).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2 })))).Verifiable();

      new RootTransactionStrategy(false, () => TransactionMock.Object, NullTransactionStrategy.Null, ExecutionContextMock.Object);

      VerifyAll();
      sequence.Verify();
    }

    [Test]
    public void Test_WithIncompatibleInParameters ()
    {
      var object1 = new object();
      var invalidOperationException = new InvalidOperationException("Objects no good!");

      ExecutionContextMock.Setup(mock => mock.GetInParameters()).Returns(new[] { object1 });
      TransactionMock
          .Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1 }))))
          .Throws(invalidOperationException);

      Assert.That(
          () => new RootTransactionStrategy(false, () => TransactionMock.Object, NullTransactionStrategy.Null, ExecutionContextMock.Object),
          Throws
              .TypeOf<WxeException>()
              .With.Message.EqualTo(
                  "One or more of the input parameters passed to the WxeFunction are incompatible with the function's transaction. Objects no good!")
              .And.InnerException.SameAs(invalidOperationException));
    }

    [Test]
    public void Test_WithNullValue ()
    {
      var object1 = new object();
      var object2 = new object();

      var sequence = new VerifiableSequence();
      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetInParameters()).Returns(new[] { object1, null, object2 }).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2 })))).Verifiable();

      new RootTransactionStrategy(false, () => TransactionMock.Object, NullTransactionStrategy.Null, ExecutionContextMock.Object);

      VerifyAll();
      sequence.Verify();
    }

    [Test]
    public void Test_Recursively ()
    {
      var object1 = new object();
      var object2 = new object();
      var object3 = new object();

      var sequence = new VerifiableSequence();
      ExecutionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.GetInParameters())
          .Returns(new[] { object1, new[] { object2, object3 } })
          .Verifiable();
      TransactionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2, object3 }))))
          .Verifiable();

      new RootTransactionStrategy(false, () => TransactionMock.Object, NullTransactionStrategy.Null, ExecutionContextMock.Object);

      VerifyAll();
      sequence.Verify();
    }

    [Test]
    public void Test_RecursivelyWithNullValue ()
    {
      var object1 = new object();
      var object2 = new object();
      var object3 = new object();

      var sequence = new VerifiableSequence();
      ExecutionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.GetInParameters())
          .Returns(new[] { object1, new[] { object2, null, object3 } })
          .Verifiable();
      TransactionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2, object3 }))))
          .Verifiable();

      new RootTransactionStrategy(false, () => TransactionMock.Object, NullTransactionStrategy.Null, ExecutionContextMock.Object);

      VerifyAll();
      sequence.Verify();
    }
  }
}
