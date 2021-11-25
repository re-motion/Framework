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
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionPlay:ScopedTransactionStrategyTestBase
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
      var sequence = new MockSequence();
      TransactionMock.InSequence(sequence).Setup(mock => mock.EnterScope()).Returns(ScopeMock.Object).Verifiable();
      ChildTransactionStrategyMock.InSequence(sequence).Setup(mock => mock.OnExecutionPlay(Context, ExecutionListenerStub.Object)).Verifiable();

      _strategy.OnExecutionPlay(Context, ExecutionListenerStub.Object);

      VerifyAll();
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_AfterPause ()
    {
      InvokeOnExecutionPlay(_strategy);
      InvokeOnExecutionPause(_strategy);
      InvokeOnExecutionPlay(_strategy);
    }

    [Test]
    public void Test_EnterScopeThrows ()
    {
      var innerException = new Exception("Enter Scope Exception");
      TransactionMock.Setup(mock => mock.EnterScope()).Throws(innerException).Verifiable();

      try
      {
        _strategy.OnExecutionPlay(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException.InnerException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithScope ()
    {
      InvokeOnExecutionPlay(_strategy);
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));
      Assert.That(
          () => _strategy.OnExecutionPlay(Context, ExecutionListenerStub.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "OnExecutionPlay may not be invoked twice without calling OnExecutionStop, OnExecutionPause, or OnExecutionFail in-between."));
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var innerException = new ApplicationException("InnerListener Exception");
      
      var sequence = new MockSequence();
      TransactionMock.InSequence(sequence).Setup(mock => mock.EnterScope()).Returns(ScopeMock.Object).Verifiable();
      ChildTransactionStrategyMock.InSequence(sequence).Setup(mock => mock.OnExecutionPlay(Context, ExecutionListenerStub.Object)).Throws(innerException).Verifiable();

      try
      {
        _strategy.OnExecutionPlay(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(_strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var innerException = new WxeFatalExecutionException(new Exception("InnerListener Exception"), null);

      var sequence = new MockSequence();
      TransactionMock.InSequence(sequence).Setup(mock => mock.EnterScope()).Returns(ScopeMock.Object).Verifiable();
      ChildTransactionStrategyMock.InSequence(sequence).Setup(mock => mock.OnExecutionPlay(Context, ExecutionListenerStub.Object)).Throws(innerException).Verifiable();

      try
      {
        _strategy.OnExecutionPlay(Context, ExecutionListenerStub.Object);
        Assert.Fail("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException, Is.SameAs(innerException));
      }

      VerifyAll();
      Assert.That(_strategy.Scope, Is.Not.Null);
    }
  }
}
