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
using Remotion.Data;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

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
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _executionListenerStub = new Mock<IWxeFunctionExecutionListener>();
      
      _transactionFactoryMock = new Mock<ITransactionFactory> (MockBehavior.Strict);
      _transactionMock = new Mock<ITransaction> (MockBehavior.Strict);
      _transactionFactoryMock.Setup (stub => stub.Create ()).Returns (_transactionMock.Object);

      _scopeMock = new Mock<ITransactionScope> (MockBehavior.Strict);
      _executionContextMock = new Mock<IWxeFunctionExecutionContext> (MockBehavior.Strict);
      _outerTransactionStrategyMock = new Mock<TransactionStrategyBase> (MockBehavior.Strict);
      _childTransactionStrategyMock = new Mock<TransactionStrategyBase> (MockBehavior.Strict);
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    protected WxeContext Context
    {
      get { return _context; }
    }

    protected TransactionStrategyBase OuterTransactionStrategyMock
    {
      get { return _outerTransactionStrategyMock.Object; }
    }

    protected TransactionStrategyBase ChildTransactionStrategyMock
    {
      get { return _childTransactionStrategyMock.Object; }
    }

    public ITransactionFactory TransactionFactoryMock
    {
      get { return _transactionFactoryMock.Object; }
    }

    protected ITransaction TransactionMock
    {
      get { return _transactionMock.Object; }
    }
    
    protected ITransactionScope ScopeMock
    {
      get { return _scopeMock.Object; }
    }

    protected IWxeFunctionExecutionListener ExecutionListenerStub
    {
      get { return _executionListenerStub.Object; }
    }

    protected IWxeFunctionExecutionContext ExecutionContextMock
    {
      get { return _executionContextMock.Object; }
    }

    protected void SetChild (ScopedTransactionStrategyBase strategy, TransactionStrategyBase childStrategy)
    {
      PrivateInvoke.SetNonPublicField (strategy, "_child", childStrategy);
    }

    protected ScopedTransactionStrategyBase CreateScopedTransactionStrategy (bool autoCommit, TransactionStrategyBase parentTransactionStrategy)
    {
      _executionContextMock.BackToRecord();
      _executionContextMock.Setup (stub => stub.GetInParameters()).Returns (new object[0]);

      _transactionMock.BackToRecord();
      _transactionMock.Setup (stub => stub.EnsureCompatibility (It.IsNotNull<IEnumerable>()));

      var strategy = new Mock<ScopedTransactionStrategyBase> (
          autoCommit, (Func<ITransaction>) _transactionFactoryMock.Object.Create, parentTransactionStrategy, _executionContextMock.Object)          { CallBase = true };

      SetChild (strategy.Object, ChildTransactionStrategyMock);

      _executionContextMock.BackToRecord();
      _transactionMock.BackToRecord();

      return strategy.Object;
    }

    protected void InvokeOnExecutionPlay (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.BackToRecord();
      _childTransactionStrategyMock.Setup (stub => stub.OnExecutionPlay (_context, _executionListenerStub.Object));

      _transactionMock.BackToRecord();
      _transactionMock.Setup (stub => stub.EnterScope()).Returns (ScopeMock);

      strategy.OnExecutionPlay (Context, ExecutionListenerStub);

      _transactionMock.BackToRecord();
      _childTransactionStrategyMock.BackToRecord ();
    }

    protected void InvokeOnExecutionPause (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.BackToRecord ();
      _childTransactionStrategyMock.Setup (stub => stub.OnExecutionPause (_context, _executionListenerStub.Object));

      _scopeMock.BackToRecord();
      _scopeMock.Setup (stub => stub.Leave());

      strategy.OnExecutionPause (Context, _executionListenerStub.Object);

      _childTransactionStrategyMock.BackToRecord ();
      _scopeMock.BackToRecord();
    }
  }
}
