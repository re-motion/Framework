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
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  public class ScopedTransactionStrategyTestBase
  {
    public interface ITransactionFactory
    {
      ITransaction Create ();
    }

    private Mock<IWxeFunctionExecutionListener> _executionListenerStub;
    private Mock<ITransactionScope> _scopeMock;
    private Mock<ITransactionFactory> _transactionFactoryMock;
    private Mock<ITransaction> _transactionMock;
    private Mock<TransactionStrategyBase> _outerTransactionStrategyMock;
    private WxeContext _context;
    private Mock<IWxeFunctionExecutionContext> _executionContextMock;
    private Mock<TransactionStrategyBase> _childTransactionStrategyMock;

    [SetUp]
    public virtual void SetUp ()
    {
      _context = WxeContextFactory.Create(new TestFunction());

      _executionListenerStub = new Mock<IWxeFunctionExecutionListener>();

      _transactionFactoryMock = new Mock<ITransactionFactory>(MockBehavior.Strict);
      _transactionMock = new Mock<ITransaction>(MockBehavior.Strict);
      _transactionFactoryMock.Setup(stub => stub.Create()).Returns(_transactionMock.Object);

      _scopeMock = new Mock<ITransactionScope>(MockBehavior.Strict);
      _executionContextMock = new Mock<IWxeFunctionExecutionContext>(MockBehavior.Strict);
      _outerTransactionStrategyMock = new Mock<TransactionStrategyBase>(MockBehavior.Strict);
      _childTransactionStrategyMock = new Mock<TransactionStrategyBase>(MockBehavior.Strict);
    }

    protected WxeContext Context
    {
      get { return _context; }
    }

    protected Mock<TransactionStrategyBase> OuterTransactionStrategyMock
    {
      get { return _outerTransactionStrategyMock; }
    }

    protected Mock<TransactionStrategyBase> ChildTransactionStrategyMock
    {
      get { return _childTransactionStrategyMock; }
    }

    public  Mock<ITransactionFactory> TransactionFactoryMock
    {
      get { return _transactionFactoryMock; }
    }

    protected Mock<ITransaction> TransactionMock
    {
      get { return _transactionMock; }
    }

    protected Mock<ITransactionScope> ScopeMock
    {
      get { return _scopeMock; }
    }

    protected Mock<IWxeFunctionExecutionListener> ExecutionListenerStub
    {
      get { return _executionListenerStub; }
    }

    protected Mock<IWxeFunctionExecutionContext> ExecutionContextMock
    {
      get { return _executionContextMock; }
    }

    protected void SetChild (ScopedTransactionStrategyBase strategy, TransactionStrategyBase childStrategy)
    {
      PrivateInvoke.SetNonPublicField(strategy, "_child", childStrategy);
    }

    protected Mock<ScopedTransactionStrategyBase> CreateScopedTransactionStrategy (bool autoCommit, TransactionStrategyBase parentTransactionStrategy)
    {
      _executionContextMock.Reset();
      _executionContextMock.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      _transactionMock.Reset();
      _transactionMock.Setup(stub => stub.EnsureCompatibility(It.IsNotNull<IEnumerable>()));

      var strategy = new Mock<ScopedTransactionStrategyBase>(
          autoCommit, (Func<ITransaction>)_transactionFactoryMock.Object.Create, parentTransactionStrategy, _executionContextMock.Object) { CallBase = true };

      SetChild(strategy.Object, ChildTransactionStrategyMock.Object);

      _executionContextMock.Reset();
      _transactionMock.Reset();

      return strategy;
    }

    protected void InvokeOnExecutionPlay (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.Reset();
      _childTransactionStrategyMock.Setup(stub => stub.OnExecutionPlay(_context, _executionListenerStub.Object));

      _transactionMock.Reset();
      _transactionMock.Setup(stub => stub.EnterScope()).Returns(ScopeMock.Object);

      strategy.OnExecutionPlay(Context, ExecutionListenerStub.Object);

      _transactionMock.Reset();
      _childTransactionStrategyMock.Reset();
    }

    protected void InvokeOnExecutionPause (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.Reset();
      _childTransactionStrategyMock.Setup(stub => stub.OnExecutionPause(_context, _executionListenerStub.Object));

      _scopeMock.Reset();
      _scopeMock.Setup(stub => stub.Leave());

      strategy.OnExecutionPause(Context, _executionListenerStub.Object);

      _childTransactionStrategyMock.Reset();
      _scopeMock.Reset();
    }

    protected void VerifyAll ()
    {
      _transactionFactoryMock.Verify();
      _transactionMock.Verify();
      _scopeMock.Verify();
      _executionContextMock.Verify();
      _outerTransactionStrategyMock.Verify();
      _childTransactionStrategyMock.Verify();
    }
  }
}
