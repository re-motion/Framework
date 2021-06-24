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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop : ScopedTransactionStrategyTestBase
  {
    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters ()).Returns (new object[0]).Verifiable();
      ScopeMock.Setup (mock => mock.Leave()).Verifiable();
      TransactionMock.Setup (mock => mock.Release()).Verifiable();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      TransactionMock.Setup (mock => mock.Commit ()).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters()).Returns (new object[0]).Verifiable();
      ScopeMock.Setup (mock => mock.Leave()).Verifiable();
      TransactionMock.Setup (mock => mock.Release()).Verifiable();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithParentTransactionStrategy ()
    {
      var strategy = CreateScopedTransactionStrategy (true, OuterTransactionStrategyMock);
      var expectedObjects = new[] { new object() };

      InvokeOnExecutionPlay (strategy);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      TransactionMock.Setup (mock => mock.Commit ()).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters()).Returns (expectedObjects).Verifiable();
      OuterTransactionStrategyMock.Setup (mock => mock.EnsureCompatibility (expectedObjects)).Verifiable();
      ScopeMock.Setup (mock => mock.Leave()).Verifiable();
      TransactionMock.Setup (mock => mock.Release()).Verifiable();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithCommitTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      strategy.InSequence (sequence).Protected().Setup ("CommitTransaction", true).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters ()).Returns (new object[0]).Verifiable();
      ScopeMock.Setup (mock => mock.Leave ()).Verifiable();
      TransactionMock.Setup (mock => mock.Release ()).Verifiable();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithReleaseTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters ()).Returns (new object[0]).Verifiable();
      ScopeMock.Setup (mock => mock.Leave ()).Verifiable();
      strategy.InSequence (sequence).Protected().Setup ("ReleaseTransaction", true).Verifiable();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithNullScope ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      Assert.That (strategy.Scope, Is.Null);
      Assert.That (
          () => strategy.OnExecutionStop (Context, ExecutionListenerStub),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("OnExecutionStop may not be invoked unless OnExecutionPlay was called first."));
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Throws (innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new WxeFatalExecutionException (new Exception ("ChildStrategy Exception"), null);

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Throws (innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_CommitThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      TransactionMock.Setup (mock => mock.Commit ()).Throws (innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_GetOutParameterThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters()).Throws (innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_EnsureCompatibility_ThrowsBecauseOfIncompatibleOutParameters ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var invalidOperationException = new InvalidOperationException ("Completely bad objects!");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();

      ExecutionContextMock.Setup (mock => mock.GetOutParameters()).Returns (new object[0]).Verifiable();
      OuterTransactionStrategyMock.Setup (mock => mock.EnsureCompatibility (It.IsAny<IEnumerable>())).Throws (invalidOperationException).Verifiable();

      Assert.That (
          () => strategy.OnExecutionStop (Context, ExecutionListenerStub),
          Throws
              .TypeOf<WxeException> ()
              .With.Message.EqualTo (
                  "One or more of the output parameters returned from the WxeFunction are incompatible with the function's parent transaction. "
                  + "Completely bad objects!")
              .And.InnerException.SameAs (invalidOperationException));
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_EnsureCompatibilityThrowsUnexpected_GetsBubbledOut ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();

      ExecutionContextMock.Setup (mock => mock.GetOutParameters ()).Returns (new object[0]).Verifiable();
      OuterTransactionStrategyMock.Setup (mock => mock.EnsureCompatibility (It.IsAny<IEnumerable>())).Throws (innerException).Verifiable();

      Assert.That (
          () => strategy.OnExecutionStop (Context, ExecutionListenerStub),
          Throws.Exception.SameAs (innerException));
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters ()).Returns (new object[0]).Verifiable();
      ScopeMock.Setup (mock => mock.Leave()).Throws (innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetOutParameters ()).Returns (new object[0]).Verifiable();
      ScopeMock.Setup (mock => mock.Leave()).Verifiable();
      TransactionMock.Setup (mock => mock.Release()).Throws (innerException).Verifiable();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }
      Assert.That (strategy.Scope, Is.Null);
    }
  }
}
