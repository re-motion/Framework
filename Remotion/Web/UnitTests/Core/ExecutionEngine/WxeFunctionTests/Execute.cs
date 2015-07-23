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
using System.Threading;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class Execute
  {
    private MockRepository _mockRepository;
    private WxeContext _context;
    private IWxeFunctionExecutionListener _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction();
      WxeContextFactory contextFactory = new WxeContextFactory();
      _context = contextFactory.CreateContext (rootFunction);
      _mockRepository = new MockRepository();

      _executionListenerMock = _mockRepository.StrictMock<IWxeFunctionExecutionListener>();
    }

    [Test]
    public void Test_NoException ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.SetExecutionListener (_executionListenerMock);

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }

      _mockRepository.ReplayAll();

      function.Execute (_context);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_WithTransactionStrategy ()
    {
      ITransactionMode transactionModeMock = _mockRepository.StrictMock<ITransactionMode>();
      TestFunction2 function = new TestFunction2 (transactionModeMock);
      TransactionStrategyBase transactionStrategyMock = MockRepository.GenerateMock<TransactionStrategyBase>();
      transactionModeMock.Expect (mock => mock.CreateTransactionStrategy (function, _context)).Return (transactionStrategyMock);
      transactionStrategyMock.Expect (mock => mock.CreateExecutionListener (Arg<IWxeFunctionExecutionListener>.Is.NotNull))
          .Return (_executionListenerMock);

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }

      _mockRepository.ReplayAll ();

      function.Execute (_context);

      _mockRepository.VerifyAll ();
      Assert.That (function.ExecutionListener, Is.SameAs (_executionListenerMock));
    }

    [Test]
    public void Test_ReEntryAfterThreadAbort ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.SetExecutionListener (_executionListenerMock);
      
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      step1.Expect (mock => mock.Execute (_context)).WhenCalled (invocation => Thread.CurrentThread.Abort ()).Repeat.Once();
      function.Add (step1);

      WxeStep step2 = MockRepository.GenerateMock<WxeStep>();
      step2.Expect (mock => mock.Execute (_context));
      function.Add (step2);

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context));
      }
      _mockRepository.ReplayAll();

      try
      {
        function.Execute (_context);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort();
      }

      _mockRepository.VerifyAll();
      _mockRepository.BackToRecordAll();

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionStop (_context));
      }
      _mockRepository.ReplayAll();

      function.Execute (_context);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_ThreadAbort_WithFatalException ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.SetExecutionListener (_executionListenerMock);

      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      step1.Expect (mock => mock.Execute (_context)).WhenCalled (invocation => Thread.CurrentThread.Abort ());
      function.Add (step1);

      var fatalExecutionException = new WxeFatalExecutionException (new Exception ("Pause exception"), null);

      using (_mockRepository.Ordered ())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionPause (_context)).Throw (fatalExecutionException);
      }
      _mockRepository.ReplayAll ();

      try
      {
        function.Execute (_context);
        Assert.Fail ();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (fatalExecutionException));
        Thread.ResetAbort ();
      }
    } 

    [Test]
    public void Test_FailAfterException ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.SetExecutionListener (_executionListenerMock);
      
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      Exception stepException = new Exception ("StepException");
      step1.Expect (mock => mock.Execute (_context)).Throw (stepException);
      function.Add (step1);

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionFail (_context, stepException));
      }
      _mockRepository.ReplayAll();

      try
      {
        function.Execute (_context);
        Assert.Fail();
      }
      catch (WxeUnhandledException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (stepException));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_FailAfterExceptionAndFailInListener ()
    {
      TestFunction2 function = new TestFunction2 ();
      function.SetExecutionListener (_executionListenerMock);
      
      WxeStep step1 = MockRepository.GenerateMock<WxeStep> ();
      Exception stepException = new Exception ("StepException");
      step1.Expect (mock => mock.Execute (_context)).Throw (stepException);
      function.Add (step1);

      Exception listenerException = new Exception ("ListenerException");

      using (_mockRepository.Ordered())
      {
        _executionListenerMock.Expect (mock => mock.OnExecutionPlay (_context));
        _executionListenerMock.Expect (mock => mock.OnExecutionFail (_context, stepException)).Throw (listenerException);
      }
      _mockRepository.ReplayAll();

      try
      {
        function.Execute (_context);
        Assert.Fail();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (stepException));
        Assert.That (actualException.OuterException, Is.SameAs (listenerException));
      }

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Test_UseNullListener ()
    {
      TestFunction2 function = new TestFunction2();
      function.Execute (_context);
    }
  }
}
