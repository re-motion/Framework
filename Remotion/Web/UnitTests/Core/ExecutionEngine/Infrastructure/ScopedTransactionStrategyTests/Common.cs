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
using NUnit.Framework;
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Common : ScopedTransactionStrategyTestBase
  {
    private Mock<ScopedTransactionStrategyBase> _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      ExecutionContextMock.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      TransactionMock.Setup(stub => stub.EnsureCompatibility(It.IsNotNull<IEnumerable>()));

      _strategy = new Mock<ScopedTransactionStrategyBase>(
          true, (Func<ITransaction>) (() => TransactionMock.Object), OuterTransactionStrategyMock.Object, ExecutionContextMock.Object) { CallBase = true };
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_strategy.Object.ExecutionContext, Is.SameAs(ExecutionContextMock.Object));
      Assert.That(_strategy.Object.AutoCommit, Is.True);
      Assert.That(_strategy.Object.IsNull, Is.False);
      Assert.That(_strategy.Object.OuterTransactionStrategy, Is.SameAs(OuterTransactionStrategyMock.Object));
      Assert.That(_strategy.Object.Child, Is.SameAs(NullTransactionStrategy.Null));
    }

    [Test]
    public void GetTransaction ()
    {
      TransactionMock.Setup(mock => mock.To<ITransaction>()).Returns(TransactionMock.Object).Verifiable();
      Assert.That(_strategy.Object.GetNativeTransaction<ITransaction>(), Is.SameAs(TransactionMock.Object));
    }

    [Test]
    public void Commit ()
    {
      TransactionMock.Setup(mock => mock.Commit()).Verifiable();

      _strategy.Object.Commit();

      VerifyAll();
      _strategy.Verify();
    }

    [Test]
    public void Rollback ()
    {
      TransactionMock.Setup(mock => mock.Rollback()).Verifiable();

      _strategy.Object.Rollback();

      VerifyAll();
      _strategy.Verify();
    }

    [Test]
    public void GetChild ()
    {
      SetChild(_strategy.Object, ChildTransactionStrategyMock.Object);

      Assert.That(_strategy.Object.Child, Is.SameAs(ChildTransactionStrategyMock.Object));
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      var childTransaction = new Mock<ITransaction>();
      TransactionMock.Setup(mock => mock.CreateChild()).Returns(childTransaction.Object).Verifiable();

      var childExecutionContextStub = new Mock<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      TransactionStrategyBase childTransactionStrategy = _strategy.Object.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, Context);

      VerifyAll();
      _strategy.Verify();
      childTransaction.Verify();
      childExecutionContextStub.Verify();
      Assert.That(childTransactionStrategy, Is.InstanceOf(typeof(ChildTransactionStrategy)));
      Assert.That(((ChildTransactionStrategy)childTransactionStrategy).AutoCommit, Is.True);
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).Transaction, Is.SameAs(childTransaction.Object));
      Assert.That(childTransactionStrategy.OuterTransactionStrategy, Is.SameAs(_strategy.Object));
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).ExecutionContext, Is.SameAs(childExecutionContextStub.Object));
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).Scope, Is.Null);
      Assert.That(_strategy.Object.Child, Is.SameAs(childTransactionStrategy));
    }

    [Test]
    public void CreateChildTransactionStrategy_AfterPlay ()
    {
      InvokeOnExecutionPlay(_strategy.Object);

      var childTransaction = new Mock<ITransaction>();
      TransactionMock.Setup(mock => mock.CreateChild()).Returns(childTransaction.Object).Verifiable();

      var childScope = new Mock<ITransactionScope>();
      childTransaction.Setup(mock => mock.EnterScope()).Returns(childScope.Object).Verifiable();

      var childExecutionContextStub = new Mock<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      TransactionStrategyBase childTransactionStrategy = _strategy.Object.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, Context);

      VerifyAll();
      _strategy.Verify();
      childTransaction.Verify();
      childScope.Verify();
      childExecutionContextStub.Verify();
      Assert.That(childTransactionStrategy, Is.InstanceOf(typeof(ChildTransactionStrategy)));
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).AutoCommit, Is.True);
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).Transaction, Is.SameAs(childTransaction.Object));
      Assert.That(childTransactionStrategy.OuterTransactionStrategy, Is.SameAs(_strategy.Object));
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).ExecutionContext, Is.SameAs(childExecutionContextStub.Object));
      Assert.That(((ChildTransactionStrategy) childTransactionStrategy).Scope, Is.SameAs(childScope.Object));
      Assert.That(_strategy.Object.Child, Is.SameAs(childTransactionStrategy));
    }

    [Test]
    public void CreateChildTransactionStrategy_TwiceWithoutUnregister ()
    {
      var childTransaction = new Mock<ITransaction>();
      TransactionMock.Setup(mock => mock.CreateChild()).Returns(childTransaction.Object).Verifiable();

      var childExecutionContextStub = new Mock<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      _strategy.Object.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, Context);
      Assert.That(
          () => _strategy.Object.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, Context),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The transaction strategy already has an active child transaction strategy. "
                  + "This child transaction strategy must first be unregistered before invoking CreateChildTransactionStrategy again."));
    }

    [Test]
    public void CreateChildTransactionStrategy_AfterPlay_Throws ()
    {
      InvokeOnExecutionPlay(_strategy.Object);

      var childTransaction = new Mock<ITransaction>();
      TransactionMock.Setup(mock => mock.CreateChild()).Returns(childTransaction.Object).Verifiable();

      ApplicationException innerException = new ApplicationException("EnterScope Exception");
      childTransaction.Setup(mock => mock.EnterScope()).Throws(innerException).Verifiable();

      var childExecutionContextStub = new Mock<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      try
      {
        _strategy.Object.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, Context);
        Assert.Fail("Expected Exception");
      }
      catch (WxeFatalExecutionException e)
      {
        VerifyAll();
        _strategy.Verify();
        childTransaction.Verify();
        childExecutionContextStub.Verify();
        Assert.That(e.InnerException, Is.SameAs(innerException));
        Assert.That(_strategy.Object.Child, Is.InstanceOf(typeof(ChildTransactionStrategy)));
      }
    }

    [Test]
    public void UnregisterChildTransactionStrategy ()
    {
      SetChild(_strategy.Object, ChildTransactionStrategyMock.Object);
      Assert.That(_strategy.Object.Child, Is.SameAs(ChildTransactionStrategyMock.Object));

      _strategy.Object.UnregisterChildTransactionStrategy(ChildTransactionStrategyMock.Object);

      Assert.That(_strategy.Object.Child, Is.SameAs(NullTransactionStrategy.Null));
    }

    [Test]
    public void UnregisterChildTransactionStrategy_TryingToUnregisterDifferentChildThrows ()
    {
      SetChild(_strategy.Object, ChildTransactionStrategyMock.Object);
      Assert.That(_strategy.Object.Child, Is.SameAs(ChildTransactionStrategyMock.Object));

      Assert.That(
          () => _strategy.Object.UnregisterChildTransactionStrategy(NullTransactionStrategy.Null),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Unregistering a child transaction strategy that is different from the presently registered strategy is not supported."));

      Assert.That(_strategy.Object.Child, Is.SameAs(ChildTransactionStrategyMock.Object));
    }
  }
}
