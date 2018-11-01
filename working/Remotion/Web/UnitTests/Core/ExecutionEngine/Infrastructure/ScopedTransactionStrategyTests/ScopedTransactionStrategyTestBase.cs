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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  public class ScopedTransactionStrategyTestBase
  {
    public interface ITransactionFactory
    {
      ITransaction Create ();
    }

    private IWxeFunctionExecutionListener _executionListenerStub;
    private ITransactionScope _scopeMock;
    private ITransactionFactory _transactionFactoryMock;
    private ITransaction _transactionMock;
    private TransactionStrategyBase _outerTransactionStrategyMock;
    private WxeContext _context;
    private MockRepository _mockRepository;
    private IWxeFunctionExecutionContext _executionContextMock;
    private TransactionStrategyBase _childTransactionStrategyMock;

    [SetUp]
    public virtual void SetUp ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      _context = wxeContextFactory.CreateContext (new TestFunction());

      _mockRepository = new MockRepository();
      _executionListenerStub = MockRepository.Stub<IWxeFunctionExecutionListener>();
      
      _transactionFactoryMock = MockRepository.StrictMock<ITransactionFactory>();
      _transactionMock = MockRepository.StrictMock<ITransaction>();
      _transactionFactoryMock.Stub (stub => stub.Create ()).Return (_transactionMock);

      _scopeMock = MockRepository.StrictMock<ITransactionScope> ();
      _executionContextMock = MockRepository.StrictMock<IWxeFunctionExecutionContext>();
      _outerTransactionStrategyMock = MockRepository.StrictMock<TransactionStrategyBase>();
      _childTransactionStrategyMock = MockRepository.StrictMock<TransactionStrategyBase> ();
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
      get { return _outerTransactionStrategyMock; }
    }

    protected TransactionStrategyBase ChildTransactionStrategyMock
    {
      get { return _childTransactionStrategyMock; }
    }

    public ITransactionFactory TransactionFactoryMock
    {
      get { return _transactionFactoryMock; }
    }

    protected ITransaction TransactionMock
    {
      get { return _transactionMock; }
    }
    
    protected ITransactionScope ScopeMock
    {
      get { return _scopeMock; }
    }

    protected IWxeFunctionExecutionListener ExecutionListenerStub
    {
      get { return _executionListenerStub; }
    }

    protected IWxeFunctionExecutionContext ExecutionContextMock
    {
      get { return _executionContextMock; }
    }

    protected void SetChild (ScopedTransactionStrategyBase strategy, TransactionStrategyBase childStrategy)
    {
      PrivateInvoke.SetNonPublicField (strategy, "_child", childStrategy);
    }

    protected ScopedTransactionStrategyBase CreateScopedTransactionStrategy (bool autoCommit, TransactionStrategyBase parentTransactionStrategy)
    {
      _executionContextMock.BackToRecord();
      _executionContextMock.Stub (stub => stub.GetInParameters()).Return (new object[0]).Repeat.Any();
      _executionContextMock.Replay();

      _transactionMock.BackToRecord();
      _transactionMock.Stub (stub => stub.EnsureCompatibility (Arg<IEnumerable>.Is.NotNull));
      _transactionMock.Replay();

      _transactionFactoryMock.Replay();

      var strategy = MockRepository.PartialMock<ScopedTransactionStrategyBase> (
          autoCommit, (Func<ITransaction>) _transactionFactoryMock.Create, parentTransactionStrategy, _executionContextMock);
      strategy.Replay();

      SetChild (strategy, ChildTransactionStrategyMock);

      _executionContextMock.BackToRecord();
      _transactionMock.BackToRecord();

      return strategy;
    }

    protected void InvokeOnExecutionPlay (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.BackToRecord();
      _childTransactionStrategyMock.Stub (stub => stub.OnExecutionPlay (_context, _executionListenerStub));
      _childTransactionStrategyMock.Replay ();

      _transactionMock.BackToRecord();
      _transactionMock.Stub (stub => stub.EnterScope()).Return (ScopeMock);
      _transactionMock.Replay();

      strategy.OnExecutionPlay (Context, ExecutionListenerStub);

      _transactionMock.BackToRecord();
      _childTransactionStrategyMock.BackToRecord ();
    }

    protected void InvokeOnExecutionPause (ScopedTransactionStrategyBase strategy)
    {
      _childTransactionStrategyMock.BackToRecord ();
      _childTransactionStrategyMock.Stub (stub => stub.OnExecutionPause (_context, _executionListenerStub));
      _childTransactionStrategyMock.Replay ();

      _scopeMock.BackToRecord();
      _scopeMock.Stub (stub => stub.Leave());
      _scopeMock.Replay();

      strategy.OnExecutionPause (Context, _executionListenerStub);

      _childTransactionStrategyMock.BackToRecord ();
      _scopeMock.BackToRecord();
    }
  }
}
