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
using System.Collections;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop : ScopedTransactionStrategyTestBase
  {
    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy(false, NullTransactionStrategy.Null).Object;

      InvokeOnExecutionPlay(strategy);
      var sequence = new VerifiableSequence();
      ChildTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetOutParameters()).Returns(new object[0]).Verifiable();
      ScopeMock.InVerifiableSequence(sequence).Setup(mock => mock.Leave()).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();

      strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);

      VerifyAll();
      sequence.Verify();
      Assert.That(strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;

      InvokeOnExecutionPlay(strategy);
      var sequence = new VerifiableSequence();
      ChildTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Commit()).Verifiable();
      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetOutParameters()).Returns(new object[0]).Verifiable();
      ScopeMock.InVerifiableSequence(sequence).Setup(mock => mock.Leave()).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();

      strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);

      VerifyAll();
      sequence.Verify();
      Assert.That(strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithParentTransactionStrategy ()
    {
      var strategy = CreateScopedTransactionStrategy(true, OuterTransactionStrategyMock.Object).Object;
      var expectedObjects = new[] { new object() };

      InvokeOnExecutionPlay(strategy);
      var sequence = new VerifiableSequence();
      ChildTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Commit()).Verifiable();

      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetOutParameters()).Returns(expectedObjects).Verifiable();
      OuterTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.EnsureCompatibility(expectedObjects)).Verifiable();

      ScopeMock.InVerifiableSequence(sequence).Setup(mock => mock.Leave()).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();

      strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);

      VerifyAll();
      sequence.Verify();
      Assert.That(strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithCommitTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay(strategy.Object);
      var sequence = new VerifiableSequence();
      ChildTransactionStrategyMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object))
          .Verifiable();
      strategy
          .InVerifiableSequence(sequence)
          .Protected()
          .Setup("CommitTransaction", true)
          .Verifiable();
      ExecutionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.GetOutParameters())
          .Returns(new object[0])
          .Verifiable();
      ScopeMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Leave())
          .Verifiable();
      TransactionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Release())
          .Verifiable();

      strategy.Object.OnExecutionStop(Context, ExecutionListenerStub.Object);

      VerifyAll();
      strategy.Verify();
      sequence.Verify();
      Assert.That(strategy.Object.Scope, Is.Null);
    }

    [Test]
    public void Test_WithReleaseTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy(false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay(strategy.Object);
      var sequence = new VerifiableSequence();
      ChildTransactionStrategyMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object))
          .Verifiable();
      ExecutionContextMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.GetOutParameters())
          .Returns(new object[0])
          .Verifiable();
      ScopeMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Leave())
          .Verifiable();
      strategy
          .Protected()
          .Setup("ReleaseTransaction", true)
          .Verifiable();

      strategy.Object.OnExecutionStop(Context, ExecutionListenerStub.Object);

      VerifyAll();
      strategy.Verify();
      sequence.Verify();
      Assert.That(strategy.Object.Scope, Is.Null);
    }

    [Test]
    public void Test_WithNullScope ()
    {
      var strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;

      Assert.That(strategy.Scope, Is.Null);
      Assert.That(
          () => strategy.OnExecutionStop(Context, ExecutionListenerStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("OnExecutionStop may not be invoked unless OnExecutionPlay was called first."));
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;
      var innerException = new ApplicationException("InnerListener Exception");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Throws(innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;
      var innerException = new WxeFatalExecutionException(new Exception("ChildStrategy Exception"), null);

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Throws(innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_CommitThrows ()
    {
      var strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;
      var innerException = new ApplicationException("Commit Exception");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      TransactionMock.Setup(mock => mock.Commit()).Throws(innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_GetOutParameterThrows ()
    {
      var strategy = CreateScopedTransactionStrategy(false, OuterTransactionStrategyMock.Object).Object;
      var innerException = new ApplicationException("GetOutParameters Exception");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      ExecutionContextMock.Setup(mock => mock.GetOutParameters()).Throws(innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_EnsureCompatibility_ThrowsBecauseOfIncompatibleOutParameters ()
    {
      var strategy = CreateScopedTransactionStrategy(false, OuterTransactionStrategyMock.Object).Object;
      var invalidOperationException = new InvalidOperationException("Completely bad objects!");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();

      ExecutionContextMock.Setup(mock => mock.GetOutParameters()).Returns(new object[0]).Verifiable();
      OuterTransactionStrategyMock.Setup(mock => mock.EnsureCompatibility(It.IsAny<IEnumerable>())).Throws(invalidOperationException).Verifiable();

      Assert.That(
          () => strategy.OnExecutionStop(Context, ExecutionListenerStub.Object),
          Throws
              .TypeOf<WxeException>()
              .With.Message.EqualTo(
                  "One or more of the output parameters returned from the WxeFunction are incompatible with the function's parent transaction. "
                  + "Completely bad objects!")
              .And.InnerException.SameAs(invalidOperationException));

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_EnsureCompatibilityThrowsUnexpected_GetsBubbledOut ()
    {
      var strategy = CreateScopedTransactionStrategy(false, OuterTransactionStrategyMock.Object).Object;
      var innerException = new ApplicationException("GetOutParameters Exception");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();

      ExecutionContextMock.Setup(mock => mock.GetOutParameters()).Returns(new object[0]).Verifiable();
      OuterTransactionStrategyMock.Setup(mock => mock.EnsureCompatibility(It.IsAny<IEnumerable>())).Throws(innerException).Verifiable();

      Assert.That(
          () => strategy.OnExecutionStop(Context, ExecutionListenerStub.Object),
          Throws.Exception.SameAs(innerException));

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateScopedTransactionStrategy(false, NullTransactionStrategy.Null).Object;
      var innerException = new Exception("Leave Exception");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      ExecutionContextMock.Setup(mock => mock.GetOutParameters()).Returns(new object[0]).Verifiable();
      ScopeMock.Setup(mock => mock.Leave()).Throws(innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException.InnerException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateScopedTransactionStrategy(false, NullTransactionStrategy.Null).Object;
      var innerException = new Exception("Release Exception");

      InvokeOnExecutionPlay(strategy);
      ChildTransactionStrategyMock.Setup(mock => mock.OnExecutionStop(Context, ExecutionListenerStub.Object)).Verifiable();
      ExecutionContextMock.Setup(mock => mock.GetOutParameters()).Returns(new object[0]).Verifiable();
      ScopeMock.Setup(mock => mock.Leave()).Verifiable();
      TransactionMock.Setup(mock => mock.Release()).Throws(innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException.InnerException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(strategy.Scope, Is.Null);
    }
  }
}
