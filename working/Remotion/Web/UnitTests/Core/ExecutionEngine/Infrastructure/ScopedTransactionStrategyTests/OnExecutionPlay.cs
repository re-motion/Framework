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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionPlay:ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp ();

      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test ()
    {
      using (MockRepository.Ordered ())
      {
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (ScopeMock);
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPlay (Context, ExecutionListenerStub));
      }

      MockRepository.ReplayAll ();

      _strategy.OnExecutionPlay (Context, ExecutionListenerStub);

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_AfterPause ()
    {
      InvokeOnExecutionPlay (_strategy);
      InvokeOnExecutionPause (_strategy);
      InvokeOnExecutionPlay (_strategy);
    }

    [Test]
    public void Test_EnterScopeThrows ()
    {
      var innerException = new Exception ("Enter Scope Exception");
      TransactionMock.Expect (mock => mock.EnterScope ()).Throw (innerException);

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionPlay (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
            "OnExecutionPlay may not be invoked twice without calling OnExecutionStop, OnExecutionPause, or OnExecutionFail in-between.")]
    public void Test_WithScope ()
    {
      InvokeOnExecutionPlay (_strategy);
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      _strategy.OnExecutionPlay (Context, ExecutionListenerStub);
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var innerException = new ApplicationException ("InnerListener Exception");
      
      using (MockRepository.Ordered ())
      {
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (ScopeMock);
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPlay (Context, ExecutionListenerStub)).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        _strategy.OnExecutionPlay (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var innerException = new WxeFatalExecutionException (new Exception ("InnerListener Exception"), null);

      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.EnterScope()).Return (ScopeMock);
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionPlay (Context, ExecutionListenerStub)).Throw (innerException);
      }

      MockRepository.ReplayAll();

      try
      {
        _strategy.OnExecutionPlay (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Not.Null);
    }
  }
}
