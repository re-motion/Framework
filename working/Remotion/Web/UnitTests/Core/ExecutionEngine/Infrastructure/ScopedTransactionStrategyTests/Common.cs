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
using NUnit.Framework;
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Common : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      ExecutionContextMock.BackToRecord();
      ExecutionContextMock.Stub (stub => stub.GetInParameters()).Return (new object[0]).Repeat.Any();
      ExecutionContextMock.Replay();

      TransactionMock.BackToRecord();
      TransactionMock.Stub (stub => stub.EnsureCompatibility (Arg<IEnumerable>.Is.NotNull));
      TransactionMock.Replay();

      _strategy = MockRepository.PartialMock<ScopedTransactionStrategyBase> (
          true, (Func<ITransaction>) (() => TransactionMock), OuterTransactionStrategyMock, ExecutionContextMock);
      _strategy.Replay();

      ExecutionContextMock.BackToRecord();
      TransactionMock.BackToRecord();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.ExecutionContext, Is.SameAs (ExecutionContextMock));
      Assert.That (_strategy.AutoCommit, Is.True);
      Assert.That (_strategy.IsNull, Is.False);
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (OuterTransactionStrategyMock));
      Assert.That (_strategy.Child, Is.SameAs (NullTransactionStrategy.Null));
    }

    [Test]
    public void GetTransaction ()
    {
      TransactionMock.Expect (mock => mock.To<ITransaction>()).Return (TransactionMock);
      TransactionMock.Replay();
      Assert.That (_strategy.GetNativeTransaction<ITransaction>(), Is.SameAs (TransactionMock));
    }

    [Test]
    public void Commit ()
    {
      TransactionMock.Expect (mock => mock.Commit());
      MockRepository.ReplayAll();

      _strategy.Commit();

      MockRepository.VerifyAll();
    }

    [Test]
    public void Rollback ()
    {
      TransactionMock.Expect (mock => mock.Rollback());
      MockRepository.ReplayAll();

      _strategy.Rollback();

      MockRepository.VerifyAll();
    }

    [Test]
    public void GetChild ()
    {
      SetChild (_strategy, ChildTransactionStrategyMock);

      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      var childTransaction = MockRepository.GenerateStub<ITransaction>();
      TransactionMock.Expect (mock => mock.CreateChild()).Return (childTransaction);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      MockRepository.ReplayAll();

      TransactionStrategyBase childTransactionStrategy = _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub, Context);

      MockRepository.VerifyAll();
      Assert.That (childTransactionStrategy, Is.InstanceOf (typeof (ChildTransactionStrategy)));
      Assert.That (((ChildTransactionStrategy)childTransactionStrategy).AutoCommit, Is.True);
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).Transaction, Is.SameAs (childTransaction));
      Assert.That (childTransactionStrategy.OuterTransactionStrategy, Is.SameAs (_strategy));
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).ExecutionContext, Is.SameAs (childExecutionContextStub));
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).Scope, Is.Null);
      Assert.That (_strategy.Child, Is.SameAs (childTransactionStrategy));
    }

    [Test]
    public void CreateChildTransactionStrategy_AfterPlay ()
    {
      InvokeOnExecutionPlay (_strategy);

      var childTransaction = MockRepository.GenerateStub<ITransaction> ();
      TransactionMock.Expect (mock => mock.CreateChild ()).Return (childTransaction);

      ITransactionScope childScope = MockRepository.GenerateStub<ITransactionScope>();
      childTransaction.Expect (mock => mock.EnterScope ()).Return (childScope);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext> ();
      childExecutionContextStub.Stub (stub => stub.GetInParameters ()).Return (new object[0]);

      MockRepository.ReplayAll ();

      TransactionStrategyBase childTransactionStrategy = _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub, Context);

      MockRepository.VerifyAll ();
      Assert.That (childTransactionStrategy, Is.InstanceOf (typeof (ChildTransactionStrategy)));
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).AutoCommit, Is.True);
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).Transaction, Is.SameAs (childTransaction));
      Assert.That (childTransactionStrategy.OuterTransactionStrategy, Is.SameAs (_strategy));
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).ExecutionContext, Is.SameAs (childExecutionContextStub));
      Assert.That (((ChildTransactionStrategy) childTransactionStrategy).Scope, Is.SameAs (childScope));
      Assert.That (_strategy.Child, Is.SameAs (childTransactionStrategy));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
      "The transaction strategy already has an active child transaction strategy. "
      + "This child transaction strategy must first be unregistered before invoking CreateChildTransactionStrategy again.")]
    public void CreateChildTransactionStrategy_TwiceWithoutUnregister ()
    {
      var childTransaction = MockRepository.GenerateStub<ITransaction>();
      TransactionMock.Expect (mock => mock.CreateChild()).Return (childTransaction);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Stub (stub => stub.GetInParameters()).Return (new object[0]);

      MockRepository.ReplayAll();

      _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub, Context);
      _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub, Context);
    }

    [Test]
    public void CreateChildTransactionStrategy_AfterPlay_Throws ()
    {
      InvokeOnExecutionPlay (_strategy);

      var childTransaction = MockRepository.GenerateStub<ITransaction> ();
      TransactionMock.Expect (mock => mock.CreateChild ()).Return (childTransaction);

      ApplicationException innerException = new ApplicationException("EnterScope Exception");
      childTransaction.Expect (mock => mock.EnterScope ()).Throw (innerException);

      var childExecutionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext> ();
      childExecutionContextStub.Stub (stub => stub.GetInParameters ()).Return (new object[0]);

      MockRepository.ReplayAll ();

      try
      {
        _strategy.CreateChildTransactionStrategy (true, childExecutionContextStub, Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException e)
      {
        MockRepository.VerifyAll();
        Assert.That (e.InnerException, Is.SameAs (innerException));
        Assert.That (_strategy.Child, Is.InstanceOf (typeof (ChildTransactionStrategy)));
      }
    }

    [Test]
    public void UnregisterChildTransactionStrategy ()
    {
      SetChild (_strategy, ChildTransactionStrategyMock);
      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));

      _strategy.UnregisterChildTransactionStrategy (ChildTransactionStrategyMock);

      Assert.That (_strategy.Child, Is.SameAs (NullTransactionStrategy.Null));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Unregistering a child transaction strategy that is different from the presently registered strategy is not supported.")]
    public void UnregisterChildTransactionStrategy_TryingToUnregisterDifferentChildThrows ()
    {
      SetChild (_strategy, ChildTransactionStrategyMock);
      Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));

      try
      {
        _strategy.UnregisterChildTransactionStrategy (NullTransactionStrategy.Null);
      }
      finally
      {
        Assert.That (_strategy.Child, Is.SameAs (ChildTransactionStrategyMock));
      }
    }
  }
}
