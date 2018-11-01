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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionFail : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;
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
      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException));
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithReleaseTransactionOverride ()
    {
      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException));
        ScopeMock.Expect (mock => mock.Leave ());
        _strategy.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "ReleaseTransaction"));
      }

      MockRepository.ReplayAll ();

      _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionFail may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      Assert.That (_strategy.Scope, Is.Null);

      _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var innerException = new WxeFatalExecutionException  (new Exception( "InnerListener Exception"), null);

      InvokeOnExecutionPlay (_strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context,ExecutionListenerStub, _failException)).Throw (innerException);

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException));
        ScopeMock.Expect (mock => mock.Leave ()).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException));
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_ChildStrategyThrows_And_LeaveThrows ()
    {
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ()).Throw (outerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ChildStrategyThrows_And_ReleaseThrows ()
    {
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (_strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionFail (Context, ExecutionListenerStub, _failException)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (outerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionFail (Context, ExecutionListenerStub, _failException);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Null);
    }
  }
}
