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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionFail : ScopedTransactionStrategyTestBase
  {
    private Mock<ScopedTransactionStrategyBase> _strategy;
    private Exception _failException;

    public override void SetUp ()
    {
      base.SetUp ();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      _failException = new ApplicationException ("Fail Exception");
    }

    [Test]
    public void Test ()
    {
      InvokeOnExecutionPlay (_strategy.Object);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.InSequence (sequence).Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Verifiable();
      ScopeMock.InSequence (sequence).Setup (mock => mock.Leave ()).Verifiable();
      TransactionMock.InSequence (sequence).Setup (mock => mock.Release ()).Verifiable();

      _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Null);
    }

    [Test]
    public void Test_WithReleaseTransactionOverride ()
    {
      InvokeOnExecutionPlay (_strategy.Object);
      var sequenceCounter = 0;
      ChildTransactionStrategyMock
          .Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException))
          .InSequence (ref sequenceCounter, 0)
          .Verifiable();
      ScopeMock
          .Setup (mock => mock.Leave ())
          .InSequence (ref sequenceCounter, 1)
          .Verifiable();
      _strategy
          .Protected().Setup ("ReleaseTransaction", true)
          .InSequence (ref sequenceCounter, 2)
          .Verifiable();

      _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);

      VerifyAll();
      _strategy.Verify();
      Assert.That (_strategy.Object.Scope, Is.Null);
    }

    [Test]
    public void Test_WithNullScope ()
    {
      Assert.That (_strategy.Object.Scope, Is.Null);
      Assert.That (
          () => _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("OnExecutionFail may not be invoked unless OnExecutionPlay was called first."));
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (_strategy.Object);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.InSequence (sequence).Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Throws (innerException).Verifiable();
      ScopeMock.InSequence (sequence).Setup (mock => mock.Leave ()).Verifiable();
      TransactionMock.InSequence (sequence).Setup (mock => mock.Release ()).Verifiable();

      try
      {
        _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var innerException = new WxeFatalExecutionException (new Exception( "InnerListener Exception"), null);

      InvokeOnExecutionPlay (_strategy.Object);
      ChildTransactionStrategyMock.Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Throws (innerException).Verifiable();

      try
      {
        _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy.Object);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.InSequence (sequence).Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Verifiable();
      ScopeMock.InSequence (sequence).Setup (mock => mock.Leave ()).Throws (innerException).Verifiable();

      try
      {
        _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (_strategy.Object);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.InSequence (sequence).Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Verifiable();
      ScopeMock.InSequence (sequence).Setup (mock => mock.Leave ()).Verifiable();
      TransactionMock.InSequence (sequence).Setup (mock => mock.Release ()).Throws (innerException).Verifiable();

      try
      {
        _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Null);
    }

    [Test]
    public void Test_ChildStrategyThrows_And_LeaveThrows ()
    {
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy.Object);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.InSequence (sequence).Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Throws (innerException).Verifiable();
      ScopeMock.InSequence (sequence).Setup (mock => mock.Leave ()).Throws (outerException).Verifiable();

      try
      {
        _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ChildStrategyThrows_And_ReleaseThrows ()
    {
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (_strategy.Object);
      var sequence = new MockSequence();
      ChildTransactionStrategyMock.InSequence (sequence).Setup (mock => mock.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException)).Throws (innerException).Verifiable();
      ScopeMock.InSequence (sequence).Setup (mock => mock.Leave ()).Verifiable();
      TransactionMock.InSequence (sequence).Setup (mock => mock.Release ()).Throws (outerException).Verifiable();

      try
      {
        _strategy.Object.OnExecutionFail (Context, ExecutionListenerStub.Object, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      VerifyAll();
      Assert.That (_strategy.Object.Scope, Is.Null);
    }
  }
}
