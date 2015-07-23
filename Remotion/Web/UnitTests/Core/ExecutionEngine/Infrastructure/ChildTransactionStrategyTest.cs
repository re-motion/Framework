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

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ChildTransactionStrategyTest
  {
    private MockRepository _mockRepository;
    private ChildTransactionStrategy _strategy;
    private ITransaction _parentTransactionMock;
    private ITransaction _childTransactionMock;
    private IWxeFunctionExecutionContext _executionContextStub;
    private TransactionStrategyBase _outerTransactionStrategyMock;
    private IWxeFunctionExecutionListener _executionListenerStub;
    private WxeContext _context;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      _context = wxeContextFactory.CreateContext (new TestFunction ());
      _outerTransactionStrategyMock = _mockRepository.StrictMock<TransactionStrategyBase> ();
      _parentTransactionMock = _mockRepository.StrictMock<ITransaction>();
      _childTransactionMock = _mockRepository.StrictMock<ITransaction> ();
      _executionContextStub = _mockRepository.Stub<IWxeFunctionExecutionContext> ();
      _executionListenerStub = _mockRepository.Stub<IWxeFunctionExecutionListener> ();
     
      _executionContextStub.Stub (stub => stub.GetInParameters ()).Return (new object[0]);
      _parentTransactionMock.Stub (stub => stub.CreateChild()).Return (_childTransactionMock);
      _childTransactionMock.Stub (stub => stub.EnsureCompatibility (Arg<IEnumerable>.Is.NotNull));
      _mockRepository.ReplayAll ();

      _strategy = new ChildTransactionStrategy (true, _outerTransactionStrategyMock, _parentTransactionMock, _executionContextStub);
      
      _mockRepository.BackToRecordAll();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (_childTransactionMock));
      Assert.That (_strategy.OuterTransactionStrategy, Is.SameAs (_outerTransactionStrategyMock));
      Assert.That (_strategy.ExecutionContext, Is.SameAs (_executionContextStub));
      Assert.That (_strategy.AutoCommit, Is.True);
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      IWxeFunctionExecutionListener innerExecutionListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener> ();
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener (innerExecutionListenerStub);

      Assert.That (executionListener, Is.InstanceOf(typeof (ChildTransactionExecutionListener)));
      Assert.That (((ChildTransactionExecutionListener) executionListener).InnerListener, Is.SameAs (innerExecutionListenerStub));
    }

    [Test]
    public void ReleaseTransaction ()
    {
      using (_mockRepository.Ordered ())
      {
        _childTransactionMock.Expect (mock => mock.Release ());
        _outerTransactionStrategyMock.Expect (mock => mock.UnregisterChildTransactionStrategy (_strategy));
      }
      _mockRepository.ReplayAll();

      PrivateInvoke.InvokeNonPublicMethod (_strategy, "ReleaseTransaction");

      _mockRepository.VerifyAll();
    }

    [Test]
    public void OnExecutionStop ()
    {
      _childTransactionMock.Stub (stub => stub.EnterScope ()).Return (MockRepository.GenerateStub<ITransactionScope> ());
      _mockRepository.ReplayAll();
      _strategy.OnExecutionPlay (_context, _executionListenerStub);
      _childTransactionMock.BackToRecord ();

      using (_mockRepository.Ordered ())
      {
        _childTransactionMock.Stub (stub => stub.Commit ());
        _executionContextStub.Stub (stub => stub.GetOutParameters ()).Return (new object[0]);
        _childTransactionMock.Stub (stub => stub.EnsureCompatibility (Arg<IEnumerable>.Is.NotNull));
        _outerTransactionStrategyMock.Stub (mock => mock.EnsureCompatibility (Arg<IEnumerable>.Is.NotNull));
        _childTransactionMock.Expect (mock => mock.Release ());
        _outerTransactionStrategyMock.Expect (mock => mock.UnregisterChildTransactionStrategy (_strategy));
      }

      _mockRepository.ReplayAll ();

      _strategy.OnExecutionStop (_context, _executionListenerStub);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void OnExecutionFail ()
    {
      _childTransactionMock.Stub (stub => stub.EnterScope ()).Return (MockRepository.GenerateStub<ITransactionScope> ());
      _mockRepository.ReplayAll ();
      _strategy.OnExecutionPlay (_context, _executionListenerStub);
      _childTransactionMock.BackToRecord ();

      using (_mockRepository.Ordered ())
      {
        _childTransactionMock.Expect (mock => mock.Release ());
        _outerTransactionStrategyMock.Expect (mock => mock.UnregisterChildTransactionStrategy (_strategy));
      }

      _mockRepository.ReplayAll ();

      _strategy.OnExecutionFail(_context, _executionListenerStub, new Exception ("Inner Exception"));

      _mockRepository.VerifyAll ();
    }
  }
}
