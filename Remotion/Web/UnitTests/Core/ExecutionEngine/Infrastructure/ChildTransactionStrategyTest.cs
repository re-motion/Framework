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

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ChildTransactionStrategyTest
  {
    private ChildTransactionStrategy _strategy;
    private Mock<ITransaction> _parentTransactionMock;
    private Mock<ITransaction> _childTransactionMock;
    private Mock<IWxeFunctionExecutionContext> _executionContextStub;
    private Mock<TransactionStrategyBase> _outerTransactionStrategyMock;
    private Mock<IWxeFunctionExecutionListener> _executionListenerStub;
    private WxeContext _context;

    [SetUp]
    public void SetUp ()
    {
      _context = WxeContextFactory.Create(new TestFunction());
      _outerTransactionStrategyMock = new Mock<TransactionStrategyBase>(MockBehavior.Strict);
      _parentTransactionMock = new Mock<ITransaction>(MockBehavior.Strict);
      _childTransactionMock = new Mock<ITransaction>(MockBehavior.Strict);
      _executionContextStub = new Mock<IWxeFunctionExecutionContext>();
      _executionListenerStub = new Mock<IWxeFunctionExecutionListener>();

      _executionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);
      _parentTransactionMock.Setup(stub => stub.CreateChild()).Returns(_childTransactionMock.Object);
      _childTransactionMock.Setup(stub => stub.EnsureCompatibility(It.IsNotNull<IEnumerable>()));

      _strategy = new ChildTransactionStrategy(true, _outerTransactionStrategyMock.Object, _parentTransactionMock.Object, _executionContextStub.Object);

      _outerTransactionStrategyMock.Reset();
      _parentTransactionMock.Reset();
      _childTransactionMock.Reset();
      _executionContextStub.Reset();
      _executionListenerStub.Reset();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_strategy.Transaction, Is.SameAs(_childTransactionMock.Object));
      Assert.That(_strategy.OuterTransactionStrategy, Is.SameAs(_outerTransactionStrategyMock.Object));
      Assert.That(_strategy.ExecutionContext, Is.SameAs(_executionContextStub.Object));
      Assert.That(_strategy.AutoCommit, Is.True);
      Assert.That(_strategy.IsNull, Is.False);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      var innerExecutionListenerStub = new Mock<IWxeFunctionExecutionListener>();
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener(innerExecutionListenerStub.Object);

      Assert.That(executionListener, Is.InstanceOf(typeof(ChildTransactionExecutionListener)));
      Assert.That(((ChildTransactionExecutionListener)executionListener).InnerListener, Is.SameAs(innerExecutionListenerStub.Object));
    }

    [Test]
    public void ReleaseTransaction ()
    {
      var sequence = new VerifiableSequence();
      _childTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();
      _outerTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.UnregisterChildTransactionStrategy(_strategy)).Verifiable();

      PrivateInvoke.InvokeNonPublicMethod(_strategy, "ReleaseTransaction");

      _outerTransactionStrategyMock.Verify();
      _parentTransactionMock.Verify();
      _childTransactionMock.Verify();
      _executionContextStub.Verify();
      _executionListenerStub.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnExecutionStop ()
    {
      _childTransactionMock.Setup(stub => stub.EnterScope()).Returns(new Mock<ITransactionScope>().Object);
      _strategy.OnExecutionPlay(_context, _executionListenerStub.Object);
      _childTransactionMock.Reset();

      var sequence = new VerifiableSequence();
      _childTransactionMock.InVerifiableSequence(sequence).Setup(stub => stub.Commit()); ;
      _executionContextStub.InVerifiableSequence(sequence).Setup(stub => stub.GetOutParameters()).Returns(new object[0]);
      _outerTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.EnsureCompatibility(It.IsNotNull<IEnumerable>()));
      _childTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();
      _outerTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.UnregisterChildTransactionStrategy(_strategy)).Verifiable();

      _strategy.OnExecutionStop(_context, _executionListenerStub.Object);

      _outerTransactionStrategyMock.Verify();
      _parentTransactionMock.Verify();
      _childTransactionMock.Verify();
      _executionContextStub.Verify();
      _executionListenerStub.Verify();
      sequence.Verify();
    }

    [Test]
    public void OnExecutionFail ()
    {
      _childTransactionMock.Setup(stub => stub.EnterScope()).Returns(new Mock<ITransactionScope>().Object);
      _strategy.OnExecutionPlay(_context, _executionListenerStub.Object);
      _childTransactionMock.Reset();

      var sequence = new VerifiableSequence();
      _childTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();
      _outerTransactionStrategyMock.InVerifiableSequence(sequence).Setup(mock => mock.UnregisterChildTransactionStrategy(_strategy)).Verifiable();

      _strategy.OnExecutionFail(_context, _executionListenerStub.Object, new Exception("Inner Exception"));

      _outerTransactionStrategyMock.Verify();
      _parentTransactionMock.Verify();
      _childTransactionMock.Verify();
      _executionContextStub.Verify();
      _executionListenerStub.Verify();
      sequence.Verify();
    }
  }
}
