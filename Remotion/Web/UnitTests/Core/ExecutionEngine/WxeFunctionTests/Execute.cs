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
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxeFunctionTests
{
  [TestFixture]
  public class Execute
  {
    private WxeContext _context;
    private Mock<IWxeFunctionExecutionListener> _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      TestFunction rootFunction = new TestFunction();
      WxeContextFactory contextFactory = new WxeContextFactory();
      _context = contextFactory.CreateContext(rootFunction);

      _executionListenerMock = new Mock<IWxeFunctionExecutionListener>(MockBehavior.Strict);
    }

    [Test]
    public void Test_NoException ()
    {
      TestFunction2 function = new TestFunction2();
      function.SetExecutionListener(_executionListenerMock.Object);

      var sequence = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionStop(_context)).Verifiable();

      function.Execute(_context);

      _executionListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Test_WithTransactionStrategy ()
    {
      var transactionModeMock = new Mock<ITransactionMode>(MockBehavior.Strict);
      TestFunction2 function = new TestFunction2(transactionModeMock.Object);
      var transactionStrategyMock = new Mock<TransactionStrategyBase>();
      transactionModeMock.Setup(mock => mock.CreateTransactionStrategy(function, _context)).Returns(transactionStrategyMock.Object).Verifiable();
      transactionStrategyMock
          .Setup(mock => mock.CreateExecutionListener(It.IsNotNull<IWxeFunctionExecutionListener>()))
          .Returns(_executionListenerMock.Object)
          .Verifiable();

      var sequence = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionStop(_context)).Verifiable();

      function.Execute(_context);

      _executionListenerMock.Verify();
      transactionModeMock.Verify();
      sequence.Verify();
      Assert.That(function.ExecutionListener, Is.SameAs(_executionListenerMock.Object));
    }

    [Test]
    public void Test_ReEntryAfterThreadAbort ()
    {
      TestFunction2 function = new TestFunction2();
      function.SetExecutionListener(_executionListenerMock.Object);

      var step1 = new Mock<WxeStep>();
      step1.Setup(mock => mock.Execute(_context)).Callback((WxeContext context) => WxeThreadAbortHelper.Abort()).Verifiable();
      function.Add(step1.Object);

      var step2 = new Mock<WxeStep>();
      step2.Setup(mock => mock.Execute(_context)).Verifiable();
      function.Add(step2.Object);

      var sequence1 = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence1).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence1).Setup(mock => mock.OnExecutionPause(_context)).Verifiable();

      try
      {
        function.Execute(_context);
        Assert.Fail();
      }
      catch (ThreadAbortException)
      {
        WxeThreadAbortHelper.ResetAbort();
      }

      _executionListenerMock.Verify();
      sequence1.Verify();
      step1.Verify(mock => mock.Execute(_context), Times.Once);
      step2.Verify(mock => mock.Execute(_context), Times.Never);

      _executionListenerMock.Reset();
      step1.Reset();
      step1.Reset();

      var sequence2 = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence2).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence2).Setup(mock => mock.OnExecutionStop(_context)).Verifiable();

      function.Execute(_context);

      _executionListenerMock.Verify();
      sequence2.Verify();
      step1.Verify(mock => mock.Execute(_context), Times.Once);
      step2.Verify(mock => mock.Execute(_context), Times.Once);
    }

    [Test]
    public void Test_ThreadAbort_WithFatalException ()
    {
      TestFunction2 function = new TestFunction2();
      function.SetExecutionListener(_executionListenerMock.Object);

      var step1 = new Mock<WxeStep>();
      step1.Setup(mock => mock.Execute(_context)).Callback((WxeContext context) => WxeThreadAbortHelper.Abort()).Verifiable();
      function.Add(step1.Object);

      var fatalExecutionException = new WxeFatalExecutionException(new Exception("Pause exception"), null);

      var sequence = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPause(_context)).Throws(fatalExecutionException).Verifiable();

      try
      {
        function.Execute(_context);
        Assert.Fail();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException, Is.SameAs(fatalExecutionException));
        WxeThreadAbortHelper.ResetAbort();
      }
      sequence.Verify();
    }

    [Test]
    public void Test_FailAfterException ()
    {
      TestFunction2 function = new TestFunction2();
      function.SetExecutionListener(_executionListenerMock.Object);

      var step1 = new Mock<WxeStep>();
      Exception stepException = new Exception("StepException");
      step1.Setup(mock => mock.Execute(_context)).Throws(stepException).Verifiable();
      function.Add(step1.Object);

      var sequence = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionFail(_context, stepException)).Verifiable();

      try
      {
        function.Execute(_context);
        Assert.Fail();
      }
      catch (WxeUnhandledException actualException)
      {
        Assert.That(actualException.InnerException, Is.SameAs(stepException));
      }

      _executionListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Test_FailAfterExceptionAndFailInListener ()
    {
      TestFunction2 function = new TestFunction2();
      function.SetExecutionListener(_executionListenerMock.Object);

      var step1 = new Mock<WxeStep>();
      Exception stepException = new Exception("StepException");
      step1.Setup(mock => mock.Execute(_context)).Throws(stepException).Verifiable();
      function.Add(step1.Object);

      Exception listenerException = new Exception("ListenerException");

      var sequence = new VerifiableSequence();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionPlay(_context)).Verifiable();
      _executionListenerMock.InVerifiableSequence(sequence).Setup(mock => mock.OnExecutionFail(_context, stepException)).Throws(listenerException).Verifiable();

      try
      {
        function.Execute(_context);
        Assert.Fail();
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That(actualException.InnerException, Is.SameAs(stepException));
        Assert.That(actualException.OuterException, Is.SameAs(listenerException));
      }

      _executionListenerMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Test_UseNullListener ()
    {
      TestFunction2 function = new TestFunction2();
      function.Execute(_context);
    }
  }
}
